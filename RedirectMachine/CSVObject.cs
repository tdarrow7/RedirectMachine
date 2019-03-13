using System;
using System.Collections.Generic;
using System.IO;

namespace RedirectMachine
{
    internal class CSVObject
    {

        private string[,] osParams =  {
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

        private string[,] urlHeaderMaps = {
            { "https://www.google.com", "/googleness/" }
        };

        //List<RedirectUrl> redirectUrls = new List<RedirectUrl>();

        public CSVObject()
        {
        }

        internal void ReadOldUrlsIntoList(string osUrlFile)
        {
            throw new NotImplementedException();
        }

        internal void ReadNewUrlsIntoList(string nsUrlFile)
        {
            // Purpose: add CSV file contents to list
            using (var reader = new StreamReader(@"" + nsUrlFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.ToLower();
                    //list.Add(line);
                }
                //list.Sort();
            }
        }
    }
}