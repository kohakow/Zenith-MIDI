﻿using SharpDX.Direct3D11;
using System;
using System.Windows.Media;
using ZenithEngine.DXHelper;
using ZenithEngine.MIDI;
using ZenithEngine.ModuleUI;

namespace ZenithEngine.Modules
{
    public interface IModuleRender : IDisposable
    {
        string Name { get; }
        string Description { get; }
        bool Initialized { get; }
        ImageSource PreviewImage { get; }
        ISerializableContainer SettingsControl { get; }
        public double StartOffset { get; }
        void Init(DeviceGroup device, MidiPlayback midi, RenderStatus status);
        void RenderFrame(DeviceContext context, IRenderSurface renderSurface);
        void ReloadTrackColors();
    }
}
