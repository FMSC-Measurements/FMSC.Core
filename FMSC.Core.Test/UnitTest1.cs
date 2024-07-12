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
    }
}