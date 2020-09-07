﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Zenith.Models
{
    public class SaveableModel : INotifyPropertyChanged, IDisposable
    {
        FileSystemWatcher watcher = new FileSystemWatcher();

        string savePath;
        protected object l = new object();
        string lastJson = "";

        bool watcherReading = false;

        async Task<string> TryReadFile(string path)
        {
            for (int i = 0; i < 100; i++)
            {
                if (!File.Exists(path)) throw new FileNotFoundException();
                try
                {
                    return File.ReadAllText(path);
                }
                catch { await Task.Delay(100); }
            }
            throw new Exception("Unable to read settings file after 100 attempts");
        }

        public SaveableModel(string savePath)
        {
            savePath = Path.GetFullPath(savePath);
            this.savePath = savePath;

            watcher.Path = Path.GetDirectoryName(savePath);
            watcher.Filter = Path.GetFileName(savePath);
            watcher.EnableRaisingEvents = true;
            watcher.Changed += Watcher_Changed;

            PropertyChanged += SaveableModel_PropertyChanged;

            ReadSettings();
        }

        protected void SaveSettings()
        {
            if (watcherReading) return;
            lock (l)
            {
                var json = JsonConvert.SerializeObject(this);
                lastJson = json;
                File.WriteAllText(savePath, json);
            }
        }

        protected void ReadSettings()
        {
            lock (l)
            {
                try
                {
                    watcherReading = true;
                    var json = TryReadFile(savePath).GetAwaiter().GetResult();
                    if (json == lastJson) return;
                    var data = (JObject)JsonConvert.DeserializeObject(json);
                    foreach (var t in this.GetType().GetProperties())
                    {
                        if (data.ContainsKey(t.Name))
                        {
                            try
                            {
                                var val = data[t.Name].ToObject(t.PropertyType);
                                t.SetValue(this, val);
                            }
                            catch { }
                        }
                    }
                }
                catch (FileNotFoundException)
                { }
                finally
                {
                    watcherReading = false;
                }
            }
        }

        private void SaveableModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveSettings();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            ReadSettings();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}