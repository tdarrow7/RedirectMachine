using System;
using System.Collections.Generic;

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


        public CatchAllObject()
        {
            catchAllList = new Dictionary<string, int>();
        }

        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool CheckCatchallParams(URLObject obj)
        {
            var temp = obj.GetOriginalUrl();

            for (int i = 0; i < catchAllParams.GetLength(0); i++)
            {
                // Check if temp variable starts with any of the keyVal parameters. If found, do not add line to list
                if (temp.StartsWith(catchAllParams[i, 0].ToString().ToLower()))
                {
                    return true;
                }
            }
            // if url has any query parameters, automatically set up as catchall and flag as a url to skip
            if (obj.CheckForQueryStrings())
            {
                CheckNewCatchAlls(obj.GetSanitizedUrl());
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
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
    }

    
}