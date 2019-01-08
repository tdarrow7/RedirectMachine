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
        static List<string> osUrls = new List<string>();
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


        public static int subProjectCounter = 0;

        // list out number of found urls
        static int foundMatch = 0;
        static int lostMatch = 0;
        static void Main(string[] args)
        {

            // initialize paths to files
            string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
            string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
            string probabilityDictionary = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";

            ////string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldSiteUrls.csv";
            //string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\TestBatch.csv";
            //string nsUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\NewSiteUrls.csv";
            //string lostUrlFile = @"C:\Users\timot\Downloads\LostUrls.csv";
            //string foundUrlFile = @"C:\Users\timot\Downloads\FoundUrls.csv";
            //string probabilityDictionary = @"C:\Users\timot\Downloads\Probabilities.csv";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //ReadCSV(osUrls, osUrlFile, osParams);
            ReadCSV(osUrls, osUrlFile, osParams, true);
            AddUrls(foundList, osParams);
            //ReadCSV(osUrls, osUrlFile, true);
            ReadCSV(nsUrls, nsUrlFile);

            Console.WriteLine("begin search: ");

            // search url lists for new items
            findUrl(osUrls, nsUrls);

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
            // Purpose of method: add CSV file contents to list
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

        static void ReadCSV(List<string> list, string filePath, string[,] keyVals)
        {
            // Purpose of method: Overload original ReadCSV method to search for catchAlls.
            // When new line is read, reset catchAll property. Trim qoutes from line var temporarily
            // Check if temp variable starts with any of the keyVal parameters. If found, do not add line to list
            // using counter variable, let console know how many lines were skipped

            int counter = 0;
            using (var reader = new StreamReader(@"" + filePath))
            {
                while (!reader.EndOfStream)
                {
                    bool catchAll = false;
                    var line = reader.ReadLine();
                    var temp = line.ToLower().Trim('"');
                    for (int i = 0; i < keyVals.GetLength(0); i++)
                    {
                        if (temp.StartsWith(keyVals[i, 0].ToString().ToLower()))
                        {
                            catchAll = true;
                            counter++;
                            break;
                        }
                    }
                    if (catchAll == false)
                        list.Add(line);
                }
                list.Sort();
                Console.WriteLine($"Counter: {counter}");
            }
        }

        static void ReadCSV(List<string> list, string filePath, bool x)
        {
            // Purpose of method: while iterating through CSV, create list of potential candidates for catchall strings.
            using (var reader = new StreamReader(@"" + filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.ToLower();
                    line = TrimExcess(line);
                    checkDictionary(line);

                    list.Add(line);
                }
                list.Sort();
            }
        }

        static void ReadCSV(List<string> list, string filePath, string[,] keyVals, bool x)
        {
            // Purpose of method: while iterating through CSV, create list of potential candidates for catchall strings.
            // When new line is read, reset catchAll property. Trim qoutes from line var temporarily
            // Check if temp variable starts with any of the keyVal parameters. If found, do not add line to list
            // using counter variable, let console know how many lines were skipped
            int counter = 0;
            using (var reader = new StreamReader(@"" + filePath))
            {
                while (!reader.EndOfStream)
                {
                    bool catchAll = false;
                    var line = reader.ReadLine();
                    var temp = line.ToLower().Trim('"');
                    for (int i = 0; i < keyVals.GetLength(0); i++)
                    {
                        if (temp.StartsWith(keyVals[i, 0].ToString().ToLower()))
                        {
                            catchAll = true;
                            counter++;
                            break;
                        }
                    }
                    if (catchAll == false)
                        list.Add(line);
                    line = TrimExcess(line);
                    checkDictionary(line);
                }

                
                // commented out for testing purposes
                
                list.Sort();
                Console.WriteLine($"Counter: {counter}");
            }
        }

        /*---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
        */


        public static void AddUrls(List<string> list, string[,] keyVals)
        {
            // purpose of function: add catch-alls to List
            for (int i = 0; i < keyVals.GetLength(0); i++)
            {
                foundList.Add($"{keyVals[i, 0].ToString()}*,{keyVals[i, 1].ToString()}");
            }
        }

        public static void findUrl(List<string> oldList, List<string> newList)
        {
            // Purpose of method: check every item in List<> oldList and compare with items in List<> newList.
            // Pass path variable into checkList method.
            // If checkList returns true, path found a match and was added to foundList List<>
            // If checklist returns false, path did not find a match. Add to lostList List<>
            // ++ either lostMatch or foundMatch
            foreach (var path in oldList)
            {
                if (!checkList(path, newList))
                {
                    if (!AdvCheckList(path, newList))
                    {
                        lostMatch++;
                        lostList.Add(path);
                    }
                }
                else
                    foundMatch++;
            }
            //List<Tuple<string, string>> old = new List<Tuple<string, string>>();
            //List<Tuple<string, string>> newlist  = new List<Tuple<string, string>>();

            //List<Resouces> wat = new List<Resouces>();
        }


        public static bool checkList(string value, List<string> urls)
        {
            // get last piece of url in string
            string subString = TruncateString(value, 48);
            foreach (var item in urls)
            {
                if (item.Contains(subString))
                {
                    foreach (var subProject in subProjects)
                    { 
                        if (value.StartsWith(subProject[0]))
                        {
                            if (item.StartsWith(subProject[1]))
                            {
                                subProjectCounter++;
                            }
                        }
                    }
                    string s = value + "," + item;
                    TruncateList(s, foundList);
                    return true;
                }
            }
            return false;
        }

        public static bool AdvCheckList(string value, List<string> urls)
        {
            string[] tempArray = TruncateString(value, 48).Split('-'); 
            for (int i = 1; i < tempArray.Length; i++)
            {
                int counter = 0;
                string temp = BuildTempString(tempArray, i);
                string s = "";
                foreach (var u in urls)
                {
                    if (u.Contains(temp))
                    {
                        counter++;
                        s = $"{value},{u}";
                    }
                }
                if (counter == 1)
                {
                    TruncateList(s, foundList);
                    return true;
                }
            }
            return false;
        }

        public static string BuildTempString(string[] tempArray, int i)
        {
            // Purpose of method: return a temporary string from an array of strings
            string x = "";
            for (int j = 0; j <= i; j++)
            {
                x = String.Concat(str0: x, str1: tempArray[j]) + "-";
            }
            return x.Substring(0, x.Length - 1);
        }

        public static void TruncateList(string value, List<string> list)
        {
            bool found = false;
            foreach (var i in list)
            {
                if (i == value)
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
                list.Add(value);
        }

        public static string TrimExcess(string line)
        {
            string x = line;
            if (x.Contains("?"))
                x = GetSubString(x, "?", false);
            else if (x.Contains(".aspx"))
            {
                x = GetSubString(x, ".aspx", true);
                return x;
            }
            if (x.Substring(x.Length - 1) != "/")
            {
                int i = x.LastIndexOf("/");
                x = x.Substring(0, i + 1);
            }
            return x;
        }

        public static void checkDictionary(string line)
        {
            if (!priorityList.ContainsKey(line))
            {
                priorityList.Add(line, 1);
            }
            else
            {
                int value = priorityList[line];
                value++;
                priorityList[line] = value;
            }
            var count = line.Count(x => x == '/');
            if (count >= 2)
            {
                line = GetSubString(line, "/", 2);
                checkDictionary(line);
            }
        }

        public static string TruncateString(string value, int maxLength)
        {
            // Purpose of method: retrieve usable/searchable end of url from variable value.
            // Step 1: remove unnecessary contents on end of url if found
            // Step 2: get url text after last slash in url
            // Step 3: truncate temporary value to maxLength
            string temp = value;
            int index = value.Length;
            if (temp.EndsWith("/"))
                temp = GetSubString(temp, "/", false);
            else if (temp.EndsWith("/*"))
                temp = GetSubString(temp, "/*", false);
            else if (temp.EndsWith("-"))
                temp = GetSubString(temp, "-", false);
            else if (temp.Contains("."))
                temp = GetSubString(temp, ".", false);

            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            if (string.IsNullOrEmpty(temp)) return temp;
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        public static int getIndex(string i, string j)
        {
            // Purpose of method: return position of j variable in string i.
            return i.LastIndexOf(j);
        }

        public static string GetSubString(string i, string j, bool x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with a bool. The bool indicates to the function that it must return a substring
            // 1) if true, includes the string j rather than excluding it, or
            // 2) if false, returns a substring that excludes string j.
            int index = getIndex(i, j);
            string temp;
            if (x == true)
            {
                temp = i.Substring(0, index + j.Length);
            }
            else
                temp = i.Substring(0, index);
            return temp;
        }

        public static string GetSubString(string i, string j, int x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with an int. The int indicates to the function that it must rerun that many times.
            var pos = 0;
            string temp = i;
            while (pos <= x)
            {
                int index = getIndex(i, j);
                temp = temp.Substring(0, index);
                pos++;
            }
            return temp;
        }

        static void buildCSV(List<string> list, string filePath)
        {
            // Purpose of method: builds a new CSV for the user to view at the specified file path
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
            // Purpose of method: builds a new CSV for the user to view at the specified file path
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
