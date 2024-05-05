﻿using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zenith
{
    class InstallSettings
    {
        public string PreviousVersion = "";
        public string VersionName = "2.1.3";
        public string LanguagesVersion = "";
        public bool AutoUpdate = false;
        public bool Installed = false;
        public string SelectedLanguage = "";
        public string InstallerVer = "";

        public static readonly string SettingsPath = "Settings\\meta.kvs";

        public InstallSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                if(!Directory.Exists(Path.GetDirectoryName(SettingsPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                SaveConfig();
            }
            else
            {
                var stream = new StreamReader(new GZipStream(File.Open(SettingsPath, FileMode.Open), CompressionMode.Decompress));
                var text = stream.ReadToEnd();
                var obj = (dynamic)JsonConvert.DeserializeObject(text);
                stream.Close();
                if (obj.version != null) VersionName = obj.version;
                if (obj.langsVersion != null) LanguagesVersion = obj.langsVersion;
                if (obj.selectedLang != null) SelectedLanguage = obj.selectedLang;
                if (obj.autoUpdate != null) AutoUpdate = obj.autoUpdate;
                if (obj.installed != null) Installed = obj.installed;
                if (obj.installerVer != null) InstallerVer = obj.installerVer;
            }
        }

        public void SaveConfig()
        {
            var jobj = new JObject
            {
                { "version", VersionName },
                { "langsVersion", LanguagesVersion },
                { "selectedLang", SelectedLanguage },
                { "autoUpdate", AutoUpdate },
                { "installed", Installed },
                { "installerVer", InstallerVer }
            };
            var stream = new StreamWriter(new GZipStream(File.Open(SettingsPath, FileMode.Create), CompressionMode.Compress));
            stream.Write(JsonConvert.SerializeObject(jobj));
            stream.Close();
        }
    }
}
