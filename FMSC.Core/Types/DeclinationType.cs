using System;
using System.ComponentModel;

namespace FMSC.Core
{
    public enum DeclinationType
    {
        [Description("Magnetic Declination")]
        MagDec = 0,
        [Description("Deed Rotation")]
        DeedRot = 1
    }

    public static partial class Types
    {
        public static DeclinationType ParseDeclinationType(String value)
        {
            switch (value.ToLower())
            {
                case "0":
                case "md":
                case "mag":
                case "magdec":
                case "magnetic":
                case "magnetic declination": return DeclinationType.MagDec;
                case "1":
                case "dr":
                case "deed":
                case "deedrot":
                case "deed rotation": return DeclinationType.DeedRot;
            }

            if (value.Length > 2 && value.Contains(" "))
                return ParseDeclinationType(value.Split(' ')[0]);

            throw new Exception("Unknown DeclinationType");
        }
    }
}
