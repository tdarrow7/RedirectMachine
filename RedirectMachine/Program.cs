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
        //static List<URLObject> osUrls = new List<URLObject>();
        static List<RedirectUrl> osUrls = new List<RedirectUrl>();
        static List<string> nsUrls = new List<string>();

        public static Dictionary<string, int> catchAllDictionary = new Dictionary<string, int>();
        

        public static string[,] osParams =  { 
            { "/events/details/", "/classes-events/" },
            { "/events/event-results/", "/classes-events/" },
            { "/events/search-results/", "/classes-events/" },
            { "/events/smart-panel-overflow/", "/classes-events/" },
            { "/lifestyle-health-classes-and-events/lifestyle-health-calendar-of-events/", "/classes-events" },
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

        public static string[,] urlHeaderMaps =
        {
            { "https://www.google.com", "/googleness/" }
        };

        // list out number of found urls
        static int foundMatch = 0;
        static int lostMatch = 0;


        static void Main(string[] args)
        {
            //initialize paths to files
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
            //string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
            //string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
            //string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
            //string probabilityDictionary = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";

            //string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldSiteUrls.csv";
            string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\TestBatch.csv";
            string nsUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timot\Downloads\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timot\Downloads\FoundUrls.csv";
            string probabilityDictionary = @"C:\Users\timot\Downloads\Probabilities.csv";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //ReadCSV(osUrls, osUrlFile, osParams);
            ReadCSV(osUrls, osUrlFile, osParams);
            //AddUrls(foundList, osParams);
            //ReadCSV(osUrls, osUrlFile, true);
            ReadCSV(nsUrls, nsUrlFile);

            Console.WriteLine("begin search: ");

            // search url lists for new items
            findUrl(osUrls, nsUrls);
            ScanUrlObjects(osUrls);
            List < KeyValuePair<string, int> > catchAllList = catchAllDictionary.ToList();
            catchAllList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            catchAllList.Reverse();


            buildCSV(lostList, lostUrlFile);
            buildCSV(foundList, foundUrlFile);
            buildCatchAllCSV(catchAllList, probabilityDictionary);
            //buildCSV(catchAllDictionary, probabilityDictionary);

            // stop stopwatch and record elapsed time
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine($"lostMatch count: {lostMatch}");
            Console.WriteLine($"foundMatch count: {foundMatch}");
            Console.WriteLine($"lostList count: {lostList.Count}");
            Console.WriteLine($"foundList count: {foundList.Count}");
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

        static void ReadCSV(List<RedirectUrl> list, string filePath, string[,] keyVals)
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
                        list.Add(new RedirectUrl(line));
                    
                }
                // using counter variable, let console know how many lines were skipped
                Console.WriteLine($"Counter: {counter}");
            }
        }

        public static void findUrl(List<RedirectUrl> oldList, List<string> newList)
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

        public static void CheckUrlHeaderMaps(RedirectUrl obj)
        {
            var temp = obj.GetOriginalUrl();
            if (temp.StartsWith("http"))
            {
                for (int i = 0; i < urlHeaderMaps.Length; i++)
                {
                    if (temp.Contains(urlHeaderMaps[i, 0]))
                    {
                        obj.AddUrlHeaderMap(urlHeaderMaps[i, 0], urlHeaderMaps[i, 1]);
                    }

                }
            }
        }

        public static bool CheckList(RedirectUrl obj, List<string> urls)
        {
            // get last piece of url in string
            foreach (var item in urls)
            {
                obj.CheckUrl(item);
            }
            // will return whether or not it has found a match
            return obj.ScanMatchedUrls();
        }

        public static bool AdvCheckList(RedirectUrl obj, List<string> urls)
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

        public static void ScanUrlObjects(List<RedirectUrl> list)
        {
            foreach (var obj in list)
            {
                // if score is positive, url is found and add redirect to foundList. If negative, add old url to lostList
                if (obj.GetScore() < 1)
                    //lostList.Add(obj);
                    lostList.Add($"{obj.GetOriginalUrl()}");
                else
                    foundList.Add($"{obj.GetOriginalUrl()},{obj.GetNewUrl()}");
            }
        }


        public static void CheckDictionary(string url)
        {
            if (!url.EndsWith("/"))
                url = url + "/";
            if (!catchAllDictionary.ContainsKey(url))
            {
                catchAllDictionary.Add(url, 1);
            }
            else
            {
                int value = catchAllDictionary[url];
                value++;
                catchAllDictionary[url] = value;
            }
            var count = url.Count(i => i == '/');
            if (count > 2)
            {
                url = url.Substring(0, url.Length - 1);
                int index = url.LastIndexOf("/");
                url = url.Substring(0, index);
                CheckDictionary(url);
            }
        }

        public static void SortCatchAlls()
        {
            
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

        static void buildLostCSV(List<RedirectUrl> list, string filePath)
        {
            // Purpose: builds a new CSV for the user to view at the specified file path
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    string[] urlMatches = item.matchedUrls.ToArray();
                    tw.WriteLine(item);
                    if (urlMatches.Length == 0)
                    {
                        tw.WriteLine($"{item.GetOriginalUrl()}, ");
                    }
                    else
                    {
                        for (int i = 0; i < urlMatches.Length; i++)
                        {
                            if (i == 0)
                                tw.WriteLine($"{item.GetOriginalUrl()},{urlMatches[i]}");
                            else
                                tw.WriteLine($" ,{urlMatches[i]}");
                        }
                    }
                    
                }

            }
        }

        static void buildCatchAllCSV(List<KeyValuePair<string, int>> list, string filePath)
        {

            int count = list.Count;
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    tw.WriteLine(item.Key + "," + item.Value);
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
