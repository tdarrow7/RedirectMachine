using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RedirectMachine
{
    class Program
    {
        // lost and found lists
        static List<string> lostList = new List<string>();
        public static List<string> foundList = new List<string>();

        // site url lists
        //static List<string> osUrls = new List<string>();
        static List<URLObject> osUrls = new List<URLObject>();
        static List<string> nsUrls = new List<string>();

        public static Dictionary<string, int> priorityList = new Dictionary<string, int>();


        public static string[,] osParams =  { 
            { "/bakersfield/pages/ehealth/kramescontent/", "/bakersfield/" }, 
            { "/castle/event/", "/castle/classes-and-events/" },
            { "/castle/pages/ehealth/kramescontent/", "/castle/" },
            { "/castle/pages/ehealth/adamcontent/", "/castle/" },
            { "/castle/pages/oham/orgunitdetails.aspx", "/castle/" },
            { "/castle/pages/pnrs/", "/doctors/" },
            { "/castle/pages/search/searchresults.aspx", "/castle/" },
            { "/central-valley/pages/ehealth/kramescontent/", "/blog/" },
            { "/central-valley/pages/pnrs/", "/doctors/" },
            { "/clear-lake/pages/ehealth/kramescontent/", "/clear-lake/" },
            { "/feather-river/pages/ehealth/kramescontent/", "/feather-river/" },
            { "/glendale/Pages/eHealth/KramesContent/", "/glendale/" },
            { "/glendale/pages/pnrs/providerprofile.aspx/", "/doctors/" },
            { "/howard-memorial/Pages/ehealth/kramescontent/", "/howard-memorial/" },
            { "/howard-memorial/pages/news/newssearchresult.aspx", "/blog/" },
            { "/lodimemorial/Pages/ehealth/kramescontent/", "/lodi-memorial/" },
            { "/lodimemorial/pages/enrs/eventscartregistration.aspx", "/lodi-memorial/" },
            { "/lodimemorial/pages/enrs/eventssearchresult.aspx", "/lodi-memorial/" },
            { "/napa-valley/Pages/ehealth/kramescontent/", "/st-helena/" },
            { "/napa-valley/Pages/OHAM/", "/st-helena/" },
            { "/nw/Pages/ehealth/kramescontent/", "/portland/" },
            { "/nw/pages/news/newssearchresult.aspx", "/blog/" },
            { "/nw/Pages/PNRS/", "/portland" },
            { "/Pages/eHealth/AdamContent/", "/blog/" },
            { "/Pages/eHealth/kramesContent/", "/blog/" },
            { "/Pages/ENRS/", "/" },
            { "/Pages/pnrs/", "/doctors" },
            { "/portland/event/", "/portland/" },
            { "/Portland/Pages/ehealth/kramescontent/", "/portland/" },
            { "/Portland/Pages/OHAM/", "/portland/" },
            { "/Portland/pages/pnrs/", "/portland/" },
            { "/Simi-Valley/event/", "/simi-valley/" },
            { "/Simi-Valley/pages/ehealth/", "/simi-valley/" },
            { "/Simi-Valley/pages/enrs/", "/simi-valley/" },
            { "/Simi-Valley/Pages/OHAM/", "/simi-valley/" },
            { "/sonora/Pages/ehealth/kramescontent/", "/sonora/" },
            { "/sonora-regional/Pages/ehealth/kramescontent/", "/sonora/" },
            { "/sthelena/Pages/OHAM/", "/st-helena/" },
            { "/TehachapiValley/Pages/eHealth/KramesContent/", "/tehachapi-valley/" },
            { "/TehachapiValley/Pages/OHAM/", "/tehachapi-valley/" },
            { "/Tillamook/Pages/ehealth/kramescontent/", "/tillamook/" },
            { "/Tillamook/pages/enrs/", "/tillamook/" },
            { "/Tillamook/Pages/OHAM/", "/tillamook/" },
            { "/Tillamook/pages/pnrs/", "/doctors/" },
            { "/trmc/Pages/ehealth/kramescontent/", "/tillamook/" },
            { "/trmc/pages/pnrs/", "/doctors/" },
            { "/ukiah-valley/pages/clinicaltrials/", "/ukiah-valley/" },
            { "/ukiah-valley/Pages/OHAM/", "/locations/" },
            { "/vallejo/Pages/ehealth/kramescontent/", "/vallejo/" },
            { "/walla-walla/", "/find-a-location/" },
            { "/white-memorial/event/", "/white-memorial/" },
            { "/white-memorial/Pages/ehealth/kramescontent/", "/white-memorial/" },
            { "/white-memorial/pages/enrs/", "/white-memorial/" },
            { "/white-memorial/Pages/OHAM/", "/white-memorial/" },
            { "/white-memorial/pages/pnrs/providerprofile.aspx", "/doctors/" }
        };
        /*
        public static string[,] subProjects =
        {
            { "/bakersfield/", "/bakersfield/" },
            { "/castle/", "/castle/" },
            { "/clear-lake/", "/clear-lake/" },
            { "/lodimemorial/", "/lodi-memorial/" },
            { "/tehachapivalley/", "tehachapi-valley" },
            { "/sthelena/", "/st-helena/" },
            { "/sonora/", "/sonora/" },
            { "/simi-valley/", "/simi-valley/" },
            { "/reedley/", "/reedley/" },
            { "/selma/", "/selma/" },
            { "/rideout/", "/rideout/" },
            { "/portland/", "/portland/" },
            { "/glendale/", "/glendale/" },
            { "/howard-memorial/", "/howard-memorial/" },
            { "/hanford/", "/hanford/" },
            { "/feather-river/", "/feather-river/" },
            { "/vallejo/", "/vallejo/" },
            { "/tillamook/", "/tillamook/" },
            { "/white-memorial/", "/white-memorial/" },
            { "/ukiah-valley/", "/ukiah-valley/" }
        };
        */
        public static int subProjectCounter = 0;

        // list out number of found urls
        static int foundMatch = 0;
        static int lostMatch = 0;
        static void Main(string[] args)
        {

            //initialize paths to files
            string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldBlogUrls.csv";
            string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
            string probabilityDictionary = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";

            ////string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldSiteUrls.csv";
            ////string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldBlogUrls.csv";
            //string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\TestBatch.csv";
            //string nsUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\NewSiteUrls.csv";
            //string lostUrlFile = @"C:\Users\timot\Downloads\LostUrls.csv";
            //string foundUrlFile = @"C:\Users\timot\Downloads\FoundUrls.csv";
            //string probabilityDictionary = @"C:\Users\timot\Downloads\Probabilities.csv";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //ReadCSV(osUrls, osUrlFile, osParams);
            ReadCSV(osUrls, osUrlFile, osParams, true);
            //AddUrls(foundList, osParams);
            //ReadCSV(osUrls, osUrlFile, true);
            ReadCSV(nsUrls, nsUrlFile);

            Console.WriteLine("begin search: ");

            // search url lists for new items
            findUrl(osUrls, nsUrls);
            ScanUrlObjects(osUrls);

            buildCSV(lostList, lostUrlFile);
            buildCSV(foundList, foundUrlFile);
            buildCSV(priorityList, probabilityDictionary);

            // stop stopwatch and record elapsed time
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine($"lostMatch count: {lostMatch}");
            Console.WriteLine($"foundMatch count: {foundMatch}");
            Console.WriteLine($"lostList count: {lostList.Count}");
            Console.WriteLine($"foundList count: {foundList.Count}");
            Console.WriteLine($"sub project counter: {subProjectCounter}");
            Console.WriteLine($"Run time: {elapsedTime}");
        }

        /*---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
        */

        static void ReadCSV(List<string> list, string filePath)
        {
            // Purpose: add CSV file contents to list
            using (var reader = new StreamReader(@"" + filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.ToLower();
                    list.Add(line);
                }
                list.Sort();
            }
        }

        //static void ReadCSV(List<string> list, string filePath, string[,] keyVals)
        //{
        //    // Purpose: Overload original ReadCSV method to search for catchAlls.
        //    int counter = 0;
        //    using (var reader = new StreamReader(@"" + filePath))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            bool catchAll = false;
        //            var line = reader.ReadLine();
        //            // When new line is read, reset catchAll property. Trim qoutes from line var temporarily
        //            var temp = line.ToLower().Trim('"');
        //            for (int i = 0; i < keyVals.GetLength(0); i++)
        //            {
        //                // Check if temp variable starts with any of the keyVal parameters. If found, do not add line to list
        //                if (temp.StartsWith(keyVals[i, 0].ToString().ToLower()))
        //                {
        //                    catchAll = true;
        //                    counter++;
        //                    break;
        //                }
        //            }
        //            if (catchAll == false)
        //                list.Add(line);
        //        }
        //        list.Sort();
        //        // using counter variable, let console know how many lines were skipped
        //        Console.WriteLine($"Counter: {counter}");
        //    }
        //}

        static void ReadCSV(List<URLObject> list, string filePath, bool x)
        {
            // Purpose: while iterating through CSV, create list of potential candidates for catchall strings.
            using (var reader = new StreamReader(@"" + filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().ToLower();
                    // Add new URLObject to list
                    list.Add(new URLObject(line));
                }
            }
        }

        static void ReadCSV(List<URLObject> list, string filePath, string[,] keyVals, bool x)
        {
            // Purpose: while iterating through CSV, create list of potential candidates for catchall strings.
            int counter = 0;
            using (var reader = new StreamReader(@"" + filePath))
            {
                while (!reader.EndOfStream)
                {
                    bool catchAll = false;
                    var line = reader.ReadLine();
                    // When new line is read, reset catchAll property. Trim qoutes from var line temporarily
                    var temp = line.ToLower().Trim('"');
                    for (int i = 0; i < keyVals.GetLength(0); i++)
                    {
                        // Check if temp variable starts with any of the keyVal parameters. If found, do not add line to list
                        if (temp.StartsWith(keyVals[i, 0].ToString().ToLower()))
                        {
                            catchAll = true;
                            counter++;
                            break;
                        }
                    }
                    if (catchAll == false)
                        list.Add(new URLObject(line));
                }
                // using counter variable, let console know how many lines were skipped
                Console.WriteLine($"Counter: {counter}");
            }
        }

        public static void AddUrls(List<string> list, string[,] keyVals)
        {
            // purpose of function: add catch-alls to List
            for (int i = 0; i < keyVals.GetLength(0); i++)
            {
                foundList.Add($"{keyVals[i, 0].ToString()}*,{keyVals[i, 1].ToString()}");
            }
        }

        public static void findUrl(List<URLObject> oldList, List<string> newList)
        {
            // Purpose: check every item in List<> oldList and compare with items in List<> newList.
            foreach (var obj in oldList)
            {
                // Pass URLObject into CheckList method. If result is true, match has been found
                if (CheckList(obj, newList))
                    foundMatch++;
                // Pass URLObject into AdvCheckList method. If result is true, match has been found
                else if (AdvCheckList(obj, newList))
                    foundMatch++;
                // couldn't find a match. add to lost list
                else
                    lostMatch++;
            }
        }

        public static bool CheckList(URLObject obj, List<string> urls)
        {
            // get last piece of url in string
            foreach (var item in urls)
            {
                obj.CheckUrl(item);
            }
            // will return whether or not it has found a match
            return obj.ScanMatchedUrls();
        }

        public static bool AdvCheckList(URLObject obj, List<string> urls)
        {
            // reset matchedUrls for this current url and start over
            obj.ClearMatches();
            foreach (var item in urls)
            {
                // check if item contains part of old url
                obj.AdvCheckUrl(item);
            }
            // will return whether or not it has found a match
            return obj.AdvScanUrls();
        }

        public static void ScanUrlObjects(List<URLObject> list)
        {
            foreach (var obj in list)
            {
                // if score is positive, url is found and add redirect to foundList. If negative, add old url to lostList
                if (obj.GetScore() < 1)
                    lostList.Add(obj.GetOriginalUrl());
                else
                    foundList.Add($"{obj.GetOriginalUrl()},{obj.GetNewUrl()}");
            }
        }


        public static void checkDictionary(URLObject obj)
        {
            List<string> urlProbables = obj.GetUrlProbabilities();
            foreach (var url in urlProbables)
            {
                if (!priorityList.ContainsKey(url))
                {
                    priorityList.Add(url, 1);
                }
                else
                {
                    int value = priorityList[url];
                    value++;
                    priorityList[url] = value;
                }
            }
        }

        static void buildCSV(List<string> list, string filePath)
        {
            // Purpose: builds a new CSV for the user to view at the specified file path
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
        }

        static void buildCSV(Dictionary<string, int> list, string filePath)
        {
            // Purpose: builds a new CSV for the user to view at the specified file path
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    string line = $"{item.Key},{item.Value}";
                    tw.WriteLine(line);
                }
            }
        }
    }
}
