using System;
using System.Collections.Generic;
using System.Text;

namespace RedirectMachine
{
    class URLObject
    {
        private string originalUrl;
        private string urlSubDirectory;
        private int score = 0;
        public string[] matchedUrls = new string[]{};

        URLObject urlObject = new URLObject();
    }
}
