﻿using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace FMSC.Core.Windows.Controls
{
    public static class ControlUtils
    {
        public const string RestrictedChars = "!@#$%^&*:;\'\"<>/?{}\\`~";

        public static bool TextIsInteger(object sender, TextCompositionEventArgs e)
        {
            return !(e.Text.All(x => char.IsDigit(x) ||
                (e.Text == "-" && sender is TextBox tb && tb.CaretIndex == 0)
            ));
        }

        public static bool TextIsUnsignedInteger(object sender, TextCompositionEventArgs e)
        {
            return string.IsNullOrEmpty(e.Text) ? false : !e.Text.All(char.IsDigit);
        }
        
        public static bool TextIsDouble(object sender, TextCompositionEventArgs e)
        {
            return string.IsNullOrEmpty(e.Text) ? false : !(e.Text.All(x => char.IsDigit(x) ||
                (e.Text == "." && sender is TextBox tb && !tb.Text.Contains(".")) ||
                (e.Text == "-" && sender is TextBox tb2 && tb2.CaretIndex == 0)
            ));
        }

        public static bool TextIsUnsignedDouble(object sender, TextCompositionEventArgs e)
        {
            return string.IsNullOrEmpty(e.Text) ? false : !(e.Text.All(x => char.IsDigit(x) ||
                (e.Text == "." && sender is TextBox tb && !tb.Text.Contains("."))
            ));
        }

        public static bool TextHasRestrictedCharacters(object sender, TextCompositionEventArgs e, string restrictedChars = RestrictedChars)
        {
            return e.Text.Any(x => restrictedChars.Contains(x));
        }
    }
}
