using System;
using System.Windows;

namespace FMSC.Core
{
    public static class MathEx
    {
        public static double Distance(Point p1, Point p2)
        {
            return Distance(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static double Distance(double aX, double aY, double bX, double bY)
        {
            return Math.Sqrt(Math.Pow(bX - aX, 2) + Math.Pow(bY - aY, 2));
        }


        public static Point RotatePoint(Point point, double angle, Point rPoint)
        {
            return RotatePoint(point.X, point.Y, angle, rPoint.X, rPoint.Y);
        }

        public static Point RotatePoint(double x, double y, double angle, double rX, double rY)
        {
            return new Point(
                    (Math.Cos(angle) * (x - rX) - Math.Sin(angle) * (y - rY) + rX),
                    (Math.Sin(angle) * (x - rX) + Math.Cos(angle) * (y - rY) + rY));
        }


        //Compute the dot product AB . AC
        private static double DotProduct(Point pointA, Point pointB, Point pointC)
        {
            return DotProduct(pointA.X, pointA.Y, pointB.X, pointB.Y, pointC.X, pointC.Y);
        }
        
        //Compute the dot product AB . AC
        private static double DotProduct(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            return (bX - aX) * (cX - bX) + (bY - aY) * (cY - bY);
        }


        //Compute the cross product AB x AC
        private static double CrossProduct(Point pointA, Point pointB, Point pointC)
        {
            return CrossProduct(pointA.X, pointA.Y, pointB.X, pointB.Y, pointC.X, pointC.Y);
        }

        //Compute the cross product AB x AC
        private static double CrossProduct(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            return (bX - aX) * (cY - aY) - bY - aY * (cX - aX);
        }
        

        //Compute the distance from AB to C
        //if isSegment is true, AB is a segment, not a line.
        public static double DistanceToLine(Point pointA, Point pointB, Point pointC, bool isSegment = true)
        {
            return DistanceToLine(pointA.X, pointA.Y, pointB.X, pointB.Y, pointC.X, pointC.Y, isSegment);
        }

        public static double DistanceToLine(double x, double y, double l1x, double l1y, double l2x, double l2y, bool isSegment = true)
        {
            double segmentLengthSquared = (l2x - l1x) * (l2x - l1x) + (l2y - l1y) * (l2y - l1y);

            if (segmentLengthSquared == 0.0)
            {
                // The line segment is just a point.
                return Distance(x, y, l1x, l1y);
            }

            double t = ((x - l1x) * (l2x - l1x) + (y - l1y) * (l2y - l1y)) / segmentLengthSquared;

            if (t < 0)
            {
                // The closest point is the starting endpoint of the line segment.
                return Distance(x, y, l1x, l1y);
            }
            else if (t > 1)
            {
                // The closest point is the ending endpoint of the line segment.
                return Distance(x, y, l2x, l2y);
            }
            else
            {
                // The closest point is on the line segment.
                double closestX = l1x + t * (l2x - l1x);
                double closestY = l1y + t * (l2y - l1y);

                return isSegment ?
                    Math.Sqrt(Math.Pow(x - closestX, 2) + Math.Pow(y - closestY, 2)) :
                    Math.Sqrt(Math.Pow(closestX, 2) + Math.Pow(closestY, 2));
            }
        }


        public static double CalculateAngleBetweenPoints(double aX, double aY, double bX, double bY, double cX, double cY)
        {
            return Convert.RadiansToDegrees(
                Math.Acos(
                    DotProduct(aX, aY, bX, bY, cX, cY) / 
                        (Math.Sqrt(Math.Pow(aX - bX, 2) + Math.Pow(aY - bY, 2)) *
                            Math.Sqrt(Math.Pow(cX - bX, 2) + Math.Pow(cY - bY, 2)))
            ));
        }

        public static double CalculateNextPointDir(double aX, double aY, double bX, double bY, double nX, double nY)
        {
            return (nX - aX) * (bY - aY) - (nY - aY) * (bX - aX);
        }


        public static bool PointOnSegment(Point point1, Point point2, Point pointTest)
        {
            if (point2.x <= Math.Max(point1.x, pointTest.x) && point2.x >= Math.Min(point1.x, pointTest.x) &&
                point2.y <= Math.Max(point1.y, pointTest.y) && point2.y >= Math.Min(point1.y, pointTest.y))
                return true;

            return false;
        }


        public static PointOrientation PointsOrientation(Point point1, Point point2, Point point3)
        {
            double val = (point2.y - point1.y) * (point3.x - point2.x) - (point2.x - point1.x) * (point3.y - point2.y);

            if (val == 0) return PointOrientation.Collinear;
            return val > 0 ? PointOrientation.Clockwise : PointOrientation.CounterClockwise;
        }


        public static bool LineSegmentsIntersect(Point s1, Point e1, Point s2, Point e2)
        {
            var o1 = PointsOrientation(s1, e1, s2);
            var o2 = PointsOrientation(s1, e1, e2);
            var o3 = PointsOrientation(s2, e2, s1);
            var o4 = PointsOrientation(s2, e2, e1);

            if (o1 != o2 && o3 != o4) return true;
            if (o1 == 0 && PointOnSegment(s1, s2, e1)) return true;
            if (o2 == 0 && PointOnSegment(s1, e2, e1)) return true;
            if (o3 == 0 && PointOnSegment(s2, s1, e2)) return true;
            if (o4 == 0 && PointOnSegment(s2, e1, e2)) return true;


            return false;
        }
    }
}
