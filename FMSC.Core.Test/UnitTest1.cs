using FMSC.Core.Xml.KML;

namespace FMSC.Core.Test
{
    public class UnitTest1
    {
        [Fact]
        public void LoadFile()
        {
            var doc = KmlDocument.LoadFile(".kml");

            Assert.True(doc != null);
        }

        [Fact]
        public void TestIntersection()
        {
            Point point1 = new Point(0, 0);
            Point point2 = new Point(1, 1);
            Point point3= new Point(1, 0);
            Point point4 = new Point(0, 1);

            Assert.True(MathEx.LineSegmentsIntersect(point1, point2, point3, point4));
        }
    }
}