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
        public static List<string> newUrlSiteMap = new List<string>();
        public static List<RedirectUrl> redirectUrls = new List<RedirectUrl>();
        public static CatchAllObject catchAllCSV = new CatchAllObject();
        List<string> lostList = new List<string>();
        List<string> foundList = new List<string>();

        public static string[,] urlHeaderMaps = {
            { "https://www.google.com", "/googleness/" }
        };

        // default contstructor
        public RedirectFinder()
        {
        }

        /// <summary>
        /// Start the finder program
        /// </summary>
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

                    if (!catchAllCSV.CheckCatchallParams(obj))
                        redirectUrls.Add(new RedirectUrl(obj, urlHeaderMaps));
                }
            }
        }

        /// <summary>
        /// check every item in List<RedirectUrl> redirectUrls and compare with items in List<> newUrlSiteMap.
        /// </summary>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        public void FindUrlMatches()
        {
            // Purpose: 
            foreach (var obj in redirectUrls)
            {
                if (obj.BasicUrlFinder(newUrlSiteMap) || obj.AdvancedUrlFinder(newUrlSiteMap)) {

                }
                else
                {
                    catchAllCSV.CheckNewCatchAlls(obj.GetSanitizedUrl());
                }
            }
        }
    }

    
}