using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace RedirectMachine
{
    internal class RedirectFinder
    {
        // declare all universally needed variables
        public List<CatchAllObject> catchalls = new List<CatchAllObject>();
        public List<string> newUrlSiteMap = new List<string>();
        public List<RedirectUrl> redirectUrls = new List<RedirectUrl>();
        public static Dictionary<string, CatchAllObject> catchAllList = new Dictionary<string, CatchAllObject>();
        List<string> lostList = new List<string>();
        List<string> foundList = new List<string>();

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

        string[,] urlHeaderMaps = {
            { "https://www.google.com", "/googleness/" }
        };

        // default contstructor
        public RedirectFinder()
        {
        }

        internal void Run()
        {
            //initialize paths to files
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
            string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
            string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
            string probabilityDictionary = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";
            
            // read both old urls and new urls into CSV List
            ReadNewUrlsIntoList(nsUrlFile);
            ReadOldUrlsIntoList(osUrlFile);

            // call method to find url Matches
            FindUrlMatches();
        }

        private void FindUrlMatches()
        {

            throw new NotImplementedException();
        }

        private void ReadNewUrlsIntoList(string urlFile)
        {
            // Purpose: add CSV file contents to list
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.ToLower();
                    newUrlSiteMap.Add(line);
                }
                newUrlSiteMap.Sort();
            }
        }

        /// <summary>
        /// For Every line in CSV, read line and check if line belongs in a catchAll. If not, create new RedirectUrl Object.
        /// </summary>
        /// <param name="urlFile"></param>
        internal void ReadOldUrlsIntoList(string urlFile)
        {
            // Purpose: add CSV file contents to list
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    var obj = new URLObject(reader.ReadLine());

                    if (!CheckCatchallParams(obj))
                        redirectUrls.Add(new RedirectUrl(obj));
                }
            }
        }

        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CheckCatchallParams(URLObject obj)
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
            if (obj.CheckForQueryStrings())
                CheckNewCatchAlls(obj);
            return false;
        }

        /// <summary>
        /// Try to find new catchAll entry in existing catchall entries. If found, increase count for catchall by 1. If not, create new Catchall entry.
        /// </summary>
        /// <param name="obj"></param>
        private void CheckNewCatchAlls(URLObject obj)
        {
            string url = obj.GetSanitizedUrl();
            if (catchAllList.ContainsKey(url))
                catchAllList[url].IncreaseCount();
            else
                catchAllList.Add(url, new CatchAllObject(obj));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        public static void findUrl(List<URLObject> oldList, List<string> newList)
        {
            // Purpose: check every item in List<> oldList and compare with items in List<> newList.
            foreach (var obj in oldList)
            {
                CheckUrlHeaderMaps(obj);
                // Pass URLObject into CheckList method. If result is true, match has been found
                if (CheckList(obj, newList))
                    foundMatch++;
                // Pass URLObject into AdvCheckList method. If result is true, match has been found
                else if (AdvCheckList(obj, newList))
                    foundMatch++;
                // couldn't find a match. add to lost list
                else
                {
                    CheckDictionary(obj.CheckVars(obj.GetSanitizedUrl()));
                    lostMatch++;
                }
            }
        }
    }

    
}