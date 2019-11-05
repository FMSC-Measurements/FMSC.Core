using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FMSC.Core
{
    public enum Area
    {
        [Description("Square Feet")]
        FeetSq = 0,
        [Description("Square Meter")]
        MeterSq = 1,
        [Description("Acre")]
        Acre = 2,
        [Description("Hectare")]
        HectaAcre = 3
    }
}
