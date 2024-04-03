using System;
using System.Net;

namespace FMSC.Core.Utilities
{
    public class WebClientEx : WebClient
    {
        private readonly int _Timeout;

        public WebClientEx(int seconds = 30)
        {
            _Timeout = seconds;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = _Timeout * 1000;
            return w;
        }
    }
}
