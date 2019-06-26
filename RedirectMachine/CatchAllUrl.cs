using System;
using System.Collections.Generic;
using System.Text;

namespace RedirectMachine
{
    class CatchAllUrl
    {
        public string OriginalUrl { get; set; }
        public int Count { get; set; } = 1;


        public CatchAllUrl(string originalUrl)
        {
            OriginalUrl = originalUrl;
        }

        public CatchAllUrl()
        {
        }

        internal void IncreaseCount()
        {
            int count = Count;
            Count = count++;
        }
    }
}
