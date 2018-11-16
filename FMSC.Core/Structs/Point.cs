using System;

namespace FMSC.Core
{
    public struct Point : IEquatable<Point>
    {
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        internal double x;
        internal double y;
        
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        
        public bool IsEmpty
        {
            get
            {
                return X == 0 && Y == 0;
            }
        }
        
        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }


        public override bool Equals(object obj)
        {
            return obj is Point point && Equals(point);
        }

        public bool Equals(Point other)
        {
            return other.X == this.X && other.Y == this.Y;
        }

        public void Offset(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }
        
        public void Offset(Point p)
        {
            Offset(p.X, p.Y);
        }
    }
}