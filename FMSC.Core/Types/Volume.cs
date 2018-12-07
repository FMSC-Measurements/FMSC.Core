using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FMSC.Core
{
    public enum Volume
    {
        [Description("Board Foot")]
        BoardFoot = 0, //= 144cubIn (12" x 12" x 1")
        [Description("Cubic Foot")]
        CubicFoot = 1, // 1728cubIn
        [Description("Cubic Meter")]
        CubicMeter = 2
        //[Description("Cubic Inch")]
        //CubicInch = 3
    }

    public static partial class Types
    {
        public static Volume ParseVolume(String value)
        {
            switch (value.ToLower())
            {
                case "0":
                case "bf":
                case "board":
                case "board feet":
                case "board foot":
                case "boardfeet":
                case "boardfoot": return Volume.BoardFoot;
                case "1":
                case "cf":
                case "cubic foot":
                case "cubic feet":
                case "cubicfeet":
                case "cubitfoot": return Volume.CubicFoot;
                case "2":
                case "cm":
                case "cubic meter":
                case "cubicmeter": return Volume.CubicMeter;
                //case "3":
                //case "ci":
                //case "cubic inches":
                //case "cubic inch":
                //case "cubicinches":
                //case "cubicinch": return Volume.CubicMeter;
            }

            if (value.Length > 2 && value.Contains(" "))
                return ParseVolume(value.Split(' ')[0]);

            throw new Exception("Unknown Distance Type");
        }
    }
}
