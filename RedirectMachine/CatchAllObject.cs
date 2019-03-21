using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedirectMachine
{
    internal class CatchAllObject
    {
        private string[,] catchAllParams =  {
            { "/events/details/", "/classes-events/" },
            { "/events/event-results/", "/classes-events/" },
            { "/events/search-results/", "/classes-events/" },
            { "/events/smart-panel-overflow/", "/classes-events/" },
            { "/lifestyle-health-classes-and-events/lifestyle-health-calendar-of-events/", "/classes-events/" },
            { "/for-the-health-of-it/full-blog-listing/?searchId", "/blog/" },
            { "/locations/location-clinics/clinic-profile/", "/locations/" },
            { "/locations/results/", "/locations/" },
            { "/locations/profile/?id=", "/locations/" },
            { "/locations/monticello/enewsletter/", "/locations/centracare-monticello/" },
            { "/location-tabs-test/", "/locations" },
            { "/patients-visitors/cheer-cards/", "/ecards/" },
            { "/about-us/news-publications/news/?searchId", "/blog/" },
            { "/for-the-health-of-it/search-results/?searchId", "/blog/" },
            { "/about-us/news-publications/news/?year", "/blog/" },
            { "/providers/results/?searchId=", "/our-doctors/" },
            { "/providers/results/?termId=", "/our-doctors/" },
            { "/search-for-pages/results/?searchId", "/site-search/" },
            { "/services/?c=", "/our-services/" },
            { "/app/files/", "/" }
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
        internal bool CheckCatchallParams(UrlUtils obj)
        {
            var temp = obj.GetOriginalUrl();

            for (int i = 0; i < catchAllParams.GetLength(0); i++)
            {
                if (temp.StartsWith(catchAllParams[i, 0].ToString().ToLower()))
                {
                    return true;
                }
            }
            if (obj.CheckForQueryStrings())
            {
                CheckNewCatchAlls(obj.GetSanitizedUrl());
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
            {
                catchAllList.Add(url, 1);
            }
            else
            {
                int value = catchAllList[url];
                value++;
                catchAllList[url] = value;
            }
        }

        /// <summary>
        /// Sort catchAllList and then export catchAllList to CSV to specified filepath
        /// </summary>
        /// <param name="filePath"></param>
        internal void ExportCatchAllsToCSV(string filePath)
        {
            List<KeyValuePair<string, int>> tempList = catchAllList.ToList();
            tempList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in tempList)
                {
                    string line = $"{item.Key},{item.Value}";
                    tw.WriteLine(line);
                }
            }
        }
    }
}