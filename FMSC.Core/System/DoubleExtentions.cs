namespace System
{
    public static class DoubleExtentions
    {
        public static double DEFAULT_EPSILON = 0.000001f;

        #region Equals

        public static bool EqualsEx(this double x, double y)
        {
            return x.EqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool EqualsEx(this double x, double y, double epsilon)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (x == y) { return true; }
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator

            // Handle NaN, Infinity.
            if (double.IsInfinity(x) | double.IsNaN(x))
                return x.Equals(y);
            else if (double.IsInfinity(y) | double.IsNaN(y))
                return x.Equals(y);
            else 
            {
                return Math.Abs(x - y) < epsilon;
            }
        }

        public static bool EqualsEx(this double x, double? y)
        {
            if (y == null) { return false; }
            return x.EqualsEx(y.Value, DEFAULT_EPSILON);
        }

        public static bool EqualsEx(this double x, double? y, double epsilon)
        {
            if (y == null) { return false; }
            return x.EqualsEx(y.Value, epsilon);
        }

        public static bool EqualsEx(this double? x, double y)
        {
            
            if (x == null) { return false; }
            return x.Value.EqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool EqualsEx(this double? x, double y, double epsilon)
        {
            if (x == null) { return false; }
            return x.Value.EqualsEx(y, epsilon);
        }

        public static bool EqualsEx(this double? x, double? y)
        {
            if (x == null && y == null) { return true; }
            else if (x != null)
            {
                return x.Value.EqualsEx(y, DEFAULT_EPSILON);
            }
            else
            {
                return y.Value.EqualsEx(x, DEFAULT_EPSILON);
            }
        }

        public static bool EqualsEx(this double? x, double? y, double epsilon)
        {
            if (x == null && y == null) { return true; }
            else if (x != null)
            {
                return x.Value.EqualsEx(y, epsilon);
            }
            else
            {
                return y.Value.EqualsEx(x, epsilon);
            }
        }

        #endregion

        #region Less Than Or Equals

        public static bool LessThanOrEqualsEx(this double x, double y)
        {
            return x.LessThanOrEqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool LessThanOrEqualsEx(this double x, double y, double epsilon)
        {
            if (x.EqualsEx(y, epsilon))
            {
                return true;
            }
            else
            {
                return x < y;
            }
        }

        #endregion

        #region Less Than

        public static bool LessThanEx(this double x, double y)
        {
            return x.LessThanEx(y, DEFAULT_EPSILON);
        }

        public static bool LessThanEx(this double x, double y, double epsilon)
        {
            if (x.EqualsEx(y, epsilon))
            {
                return false;
            }
            else
            {
                return x < y;
            }
        }

        #endregion

        #region Greater Than Or Equals

        public static bool GreaterThanOrEqualsEx(this double x, double y)
        {
            return x.GreaterThanOrEqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool GreaterThanOrEqualsEx(this double x, double y, double epsilon)
        {
            if (x.EqualsEx(y, epsilon))
            {
                return true;
            }
            else
            {
                return x > y;
            }
        }

        #endregion

        #region Greater Than 

        public static bool GreaterThanEx(this double x, double y)
        {
            return x.GreaterThanEx(y, DEFAULT_EPSILON);
        }

        public static bool GreaterThanEx(this double x, double y, double epsilon)
        {
            if (x.EqualsEx(y, epsilon))
            {
                return false;
            }
            else
            {
                return x > y;
            }
        }

        #endregion

    }
}
