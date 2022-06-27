// Copyright (c) Pixel Crushers. All rights reserved.

using System.Text;

namespace PixelCrushers
{

    public enum EncodingType
    {
        Default,
        ASCII,
        Unicode,
        UTF7,
        UTF8,
        UTF32,
        ISO_8859_1
    }

    public static class EncodingTypeTools
    {

        public static Encoding GetEncoding(EncodingType encodingType)
        {
            switch (encodingType)
            { // Return values modified for WinRT compatibility:
                case EncodingType.ASCII: return Encoding.UTF8; //Encoding.ASCII;
                case EncodingType.Unicode: return Encoding.Unicode;
                case EncodingType.UTF32: return Encoding.Unicode; //Encoding.UTF32;
                case EncodingType.UTF7: return Encoding.Unicode; //Encoding.UTF7;
                case EncodingType.UTF8: return Encoding.UTF8;
                case EncodingType.ISO_8859_1: return Encoding.GetEncoding("iso-8859-1");
                default: return Encoding.UTF8; //Encoding.Default;
            }
        }

    }

}
