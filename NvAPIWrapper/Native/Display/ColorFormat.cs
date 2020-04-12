﻿namespace NvAPIWrapper.Native.Display
{
    public enum ColorFormat : byte
    {
        RGB = 0,
        YUV422,
        YUV444,
        YUV420,
        Default = 0xFE,
        Auto = 0xFF
    }
}
