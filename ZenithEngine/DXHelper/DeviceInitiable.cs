﻿namespace ZenithEngine.DXHelper
{
    public abstract class PureDeviceInitiable : IDeviceInitiable
    {
        public bool Initialized { get; private set; } = false;
        public DeviceGroup Device { get; private set; } = null;

        protected virtual void InitInternal() { }
        protected virtual void DisposeInternal() { }

        public virtual void Dispose()
        {
            if (!Initialized) return;
            DisposeInternal();
            Device = null;
            Initialized = false;
        }

        public virtual void Init(DeviceGroup device)
        {
            if (Initialized) return;
            Device = device;
            InitInternal();
            Initialized = true;
        }
    }
}
