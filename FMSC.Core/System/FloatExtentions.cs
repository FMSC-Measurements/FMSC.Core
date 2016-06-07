namespace System
{
    public static class FloatExtentions
    {
        public static float DEFAULT_EPSILON = 0.000001f;

        #region Equals

        public static bool EqualsEx(this float x, float y)
        {
            return x.EqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool EqualsEx(this float x, float y, float epsilon)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (x == y) { return true; }
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator

            // Handle NaN, Infinity.
            if (float.IsInfinity(x) | float.IsNaN(x))
                return x.Equals(y);
            else if (float.IsInfinity(y) | float.IsNaN(y))
                return x.Equals(y);
            else 
            {
                return Math.Abs(x - y) < epsilon;
            }
        }

        public static bool EqualsEx(this float x, float? y)
        {
            if (y == null) { return false; }
            return x.EqualsEx(y.Value, DEFAULT_EPSILON);
        }

        public static bool EqualsEx(this float x, float? y, float epsilon)
        {
            if (y == null) { return false; }
            return x.EqualsEx(y.Value, epsilon);
        }

        public static bool EqualsEx(this float? x, float y)
        {
            
            if (x == null) { return false; }
            return x.Value.EqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool EqualsEx(this float? x, float y, float epsilon)
        {
            if (x == null) { return false; }
            return x.Value.EqualsEx(y, epsilon);
        }

        public static bool EqualsEx(this float? x, float? y)
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

        public static bool EqualsEx(this float? x, float? y, float epsilon)
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

        public static bool LessThanOrEqualsEx(this float x, float y)
        {
            return x.LessThanOrEqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool LessThanOrEqualsEx(this float x, float y, float epsilon)
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

        public static bool LessThanEx(this float x, float y)
        {
            return x.LessThanEx(y, DEFAULT_EPSILON);
        }

        public static bool LessThanEx(this float x, float y, float epsilon)
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

        public static bool GreaterThanOrEqualsEx(this float x, float y)
        {
            return x.GreaterThanOrEqualsEx(y, DEFAULT_EPSILON);
        }

        public static bool GreaterThanOrEqualsEx(this float x, float y, float epsilon)
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

        public static bool GreaterThanEx(this float x, float y)
        {
            return x.GreaterThanEx(y, DEFAULT_EPSILON);
        }

        public static bool GreaterThanEx(this float x, float y, float epsilon)
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
