using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedirectMachine
{
    internal class CatchAllObject
    {
        private string[,] catchAllParams =  {
            //{ "/events/details/", "/classes-events/" },
            //{ "/events/event-results/", "/classes-events/" },
            //{ "/events/search-results/", "/classes-events/" },
            //{ "/events/smart-panel-overflow/", "/classes-events/" },
            //{ "/lifestyle-health-classes-and-events/lifestyle-health-calendar-of-events/", "/classes-events/" },
            //{ "/for-the-health-of-it/full-blog-listing/?searchId", "/blog/" },
            //{ "/locations/location-clinics/clinic-profile/", "/locations/" },
            //{ "/locations/results/", "/locations/" },
            //{ "/locations/profile/?id=", "/locations/" },
            //{ "/locations/monticello/enewsletter/", "/locations/centracare-monticello/" },
            //{ "/location-tabs-test/", "/locations" },
            //{ "/patients-visitors/cheer-cards/", "/ecards/" },
            //{ "/about-us/news-publications/news/?searchId", "/blog/" },
            //{ "/for-the-health-of-it/search-results/?searchId", "/blog/" },
            //{ "/about-us/news-publications/news/?year", "/blog/" },
            //{ "/providers/results/?searchId=", "/our-doctors/" },
            //{ "/providers/results/?termId=", "/our-doctors/" },
            //{ "/search-for-pages/results/?searchId", "/site-search/" },
            //{ "/services/?c=", "/our-services/" },
            { "/about-us/news-publications/news/2013/", "/blog/" }
        };

        Dictionary<string, int> catchAllList;

        /// <summary>
        /// default working constructor
        /// </summary>
        public CatchAllObject()
        {
            catchAllList = new Dictionary<string, int>();
        }

        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool CheckCatchallParams(string url)
        {
            for (int i = 0; i < catchAllParams.GetLength(0); i++)
            {
                if (url.StartsWith(catchAllParams[i, 0].ToString().ToLower()))
                    return true;
            }
            if (url.Contains("?"))
            {
                var obj = new UrlUtils(url);
                CheckNewCatchAlls(obj.SanitizedUrl);
                return true;
            }
            return false;
        }

        /// <summary>
        /// check to see if we already have the catchall
        /// </summary>
        /// <param name="url"></param>
        internal void CheckNewCatchAlls(string url)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            if (!catchAllList.ContainsKey(url))
                catchAllList.Add(url, 1);
            else
                catchAllList[url] = catchAllList[url] + 1;
        }

        /// <summary>
        /// Sort catchAllList and then export catchAllList to CSV to specified filepath
        /// </summary>
        /// <param name="filePath"></param>
        internal void ExportCatchAllsToCSV(string filePath)
        {
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                tw.WriteLine("Potential Probability,Number of times seen");
                foreach (var keyValuePair in catchAllList)
                {
                    if (keyValuePair.Value > 1)
                        tw.WriteLine($"{keyValuePair.Key},{keyValuePair.Value}");
                }
            }
        }
    }
}