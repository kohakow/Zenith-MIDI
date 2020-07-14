﻿using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenithEngine.DXHelper
{
    public class BlendStateKeeper : DeviceInitiable
    {
        public static implicit operator BlendState(BlendStateKeeper keeper) => keeper.BlendState;

        public BlendState BlendState { get; private set; }
        public BlendStateDescription Description;

        public static BlendStateDescription BasicBlendDescription()
        {
            var renderTargetDesc = new RenderTargetBlendDescription();
            renderTargetDesc.IsBlendEnabled = true;
            renderTargetDesc.SourceBlend = BlendOption.SourceAlpha;
            renderTargetDesc.DestinationBlend = BlendOption.InverseSourceAlpha;
            renderTargetDesc.BlendOperation = BlendOperation.Add;
            renderTargetDesc.SourceAlphaBlend = BlendOption.InverseDestinationAlpha;
            renderTargetDesc.DestinationAlphaBlend = BlendOption.One;
            renderTargetDesc.AlphaBlendOperation = BlendOperation.Add;
            renderTargetDesc.RenderTargetWriteMask = ColorWriteMaskFlags.All;

            BlendStateDescription blendDesc = new BlendStateDescription();
            blendDesc.AlphaToCoverageEnable = false;
            blendDesc.IndependentBlendEnable = false;
            for(int i = 0; i < blendDesc.RenderTarget.Length; i++)
                blendDesc.RenderTarget[i] = renderTargetDesc;

            return blendDesc;
        }

        public BlendStateKeeper() : this(BasicBlendDescription()) { }
        public BlendStateKeeper(BlendStateDescription desc)
        {
            Description = desc;
        }

        protected override void InitInternal()
        {
            BlendState = new BlendState(Device, Description);
        }

        protected override void DisposeInternal()
        {
            BlendState.Dispose();
        }

        public IDisposable UseOn(DeviceContext ctx) =>
            new Applier<BlendState>(this, () => ctx.OutputMerger.BlendState, val => ctx.OutputMerger.BlendState = val);
    }
}
