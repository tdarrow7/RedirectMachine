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

        public static List<string> osBlogList = new List<string>();
        public static List<string> osDoctorList = new List<string>();
        //public static List<string[]> priorityList = new List<string[]>();

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
            { "/lodimemorial/Pages/ehealth/kramescontent/default.aspx/", "/lodi-memorial/" },
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
            { "/white-memorial/", "/white-memorial/" },
            { "/tehachapivalley/", "/tehachapivalley/" },
            { "/bakersfield/", "/bakersfield/" }

        };
        public static string[] osDoctorTitles = { "-md", "-do", "-mph", "-pa-c", "-macp", "-mba", "-agacnp-bc", "-np", "-np-c", "-fnp-c", "-msn", "-aprn", "-phd", "-ms", "-cnm", "-facs", "-facog", "-facp", "-dpm", "-mmci", "-rd", "-cdn", "-msc", "-facmg", "-fapa", "-mhs", "-faaa", "-np-bc", "-cgc", "-facr", "-med", "-whnp-bc" };

        public static List<string> nsBlogList = new List<string>();
        public static List<string> nsDoctorList = new List<string>();
        public static string[] nsBlogParams = { "/blog/" };
        public static string[] nsDoctorParams = { "/doctors/" };

        // list out number of found urls
        static int foundMatch = 0;
        static int lostMatch = 0;

        static int osDoctorCount = 0;


        static void Main(string[] args)
        {

            // initialize paths to files
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
            string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
            string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
            string probabilityDictionary = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ReadCSV(osUrls, osUrlFile, osParams);
            ReadCSV(osUrls, osUrlFile, osParams, true);
            ReadCSV(osUrls, osUrlFile, true);
            ReadCSV(nsUrls, nsUrlFile);

            //LogList(osUrls);


            Console.WriteLine($"size of oldUrls list: {osUrls.Count}");
            Console.WriteLine($"size of newUrls list: {nsUrls.Count}");
            Console.WriteLine($"size of osBlogList: {osBlogList.Count}");
            Console.WriteLine($"size of nsBlogList: {nsBlogList.Count}");
            Console.WriteLine($"size of osDoctorList: {osDoctorList.Count}");
            Console.WriteLine($"size of nsDoctorList: {nsDoctorList.Count}");

            Console.WriteLine("begin search: ");

            // search url lists for new items
            //findUrl(osBlogList, nsBlogList);
            //findDocUrl(osDoctorList, nsDoctorList, osDoctorTitles);
            FilterUrls(osUrls, osParams);
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
            Console.WriteLine($"Run time: {elapsedTime}");
            Console.WriteLine($"osDoctor Count: {osDoctorCount}");
        }

        

        public static string TrimExcess(string line)
        {
            string x = line;
            if (x.Contains("?"))
                x = GetSubString(x, "?");
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
                //priorityList[line] = (x => priorityList[line] += 1);
            }
            var count = line.Count(x => x == '/');
            if (count >= 2)
            {
                line = GetSubString(line, "/", 2);
                checkDictionary(line);
            }
        }

        public static void FilterUrls(List<string> list, string[,] keyVals)
        {
            // purpose of function: the reps multidimensional array houses a key/value pair
            // The key is a url that occurs repeatedly in the old site map.
            // The value is the new site's url that will be where all urls using the key will redirect to

            for (int i = 0; i < keyVals.GetLength(0); i++)
            {
                list.RemoveAll(item => item == keyVals[i, 0].ToString().ToLower());
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
                    lostMatch++;
                    lostList.Add(path);
                }
                else
                {
                    foundMatch++;
                }
            }
        }

        public static bool checkList(string value, List<string> urls)
        {
            // get last piece of url in string
            string subString = TruncateString(value, 48);
            foreach (var item in urls)
            {
                if (item.Contains(subString))
                {
                    string s = value + "," + item;
                    TruncateList(s, foundList);
                    //foundList.Add(s);
                    return true;
                }
            }
            return false;
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

        public static string TruncateString(string value, int maxLength)
        {
            // Purpose of method: retrieve usable/searchable end of url from variable value.
            // Step 1: remove unnecessary contents on end of url if found
            // Step 2: get url text after last slash in url
            // Step 3: truncate temporary value to maxLength
            string temp = value;
            int index = value.Length;
            if (temp.EndsWith("/"))
                temp = GetSubString(temp, "/");
            else if (temp.EndsWith("-"))
                temp = GetSubString(temp, "-");
            else if (temp.EndsWith("/*"))
                temp = GetSubString(temp, "/*");
            else if (temp.Contains(".aspx"))
                temp = GetSubString(temp, ".aspx");

            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            if (string.IsNullOrEmpty(temp)) return temp;
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        public static int getIndex(string i, string j)
        {
            // Purpose of method: return position of j variable in string i. Specifically build for method TruncateString
            return i.LastIndexOf(j);
        }

        public static string GetSubString(string i, string j)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            int index = getIndex(i, j);
            string temp = i.Substring(0, index);
            return temp;
        }

        public static string GetSubString(string i, string j, bool x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with a bool. The bool indicates to the function that it must return
            // a substring that includes the string j rather than excluding it.
            int index = getIndex(i, j);
            string temp = i.Substring(0, index + j.Length);
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
                list.Sort();
                Console.WriteLine($"Counter: {counter}");
            }
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
