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

        ////string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
        //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
        //string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
        //string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
        //string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
        //string catchAllFile = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";

        //string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldSiteUrls.csv";
        string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\TestBatch.csv";
        string nsUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\NewSiteUrls.csv";
        string lostUrlFile = @"C:\Users\timot\Downloads\LostUrls.csv";
        string foundUrlFile = @"C:\Users\timot\Downloads\FoundUrls.csv";
        string catchAllFile = @"C:\Users\timot\Downloads\Probabilities.csv";


        /// <summary>
        /// default working constructor
        /// </summary>
        public RedirectFinder()
        {
        }

        /// <summary>
        /// Start the finder program
        /// </summary>
        internal void Run()
        {
            ImportNewUrlsIntoList(nsUrlFile);
            ImportOldUrlsIntoList(osUrlFile);
            FindUrlMatches();
            catchAllCSV.ExportCatchAllsToCSV(catchAllFile);
            CreateNewCSVs();
        }

        /// <summary>
        /// Add CSV file contents to list
        /// Sort results of list alphabetically
        /// </summary>
        /// <param name="urlFile"></param>
        private void ImportNewUrlsIntoList(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    newUrlSiteMap.Add(reader.ReadLine().ToLower());
                }
                newUrlSiteMap.Sort();
            }
        }

        /// <summary>
        /// For Every line in CSV, read line and check if line belongs in a catchAll. If not, create new RedirectUrl Object.
        /// </summary>
        /// <param name="urlFile"></param>
        internal void ImportOldUrlsIntoList(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string url = reader.ReadLine();
                    if (!catchAllCSV.CheckCatchallParams(url))
                        redirectUrls.Add(new RedirectUrl(url, urlHeaderMaps));
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
            foreach (var obj in redirectUrls)
            {
                if (!obj.BasicUrlFinder(newUrlSiteMap) || !obj.AdvancedUrlFinder(newUrlSiteMap))
                    catchAllCSV.CheckNewCatchAlls(obj.GetSanitizedUrl());
            }
        }

        /// <summary>
        /// Scan all objects in redirectUrls list and put them in either the foundList or lostList, depending on their score
        /// Send both temporary lists to buildCSV method to print both found and lost lists
        /// </summary>
        internal void CreateNewCSVs()
        {
            List<string> foundList = new List<string>();
            List<string> lostList = new List<string>();
            foreach (var obj in redirectUrls)
            {
                if (obj.Score == true)
                    foundList.Add($"{obj.GetOriginalUrl()},{obj.GetNewUrl()}");
                else
                    lostList.Add($"{obj.GetOriginalUrl()}");
            }
            ExportToCSV(foundList, foundUrlFile);
            ExportToCSV(lostList, lostUrlFile);
        }

        /// <summary>
        /// build CSV from specified list of strings and export to specified filePath
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filePath"></param>
        internal void ExportToCSV(List<string> list, string filePath)
        {
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
        }
    }

    
}