using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RedirectMachine
{
    class Program
    {
        // lost list 
        static List<string> lostList = new List<string>();
        // found list 
        static List<string> foundList = new List<string>();
        // old list
        static List<string> osUrls = new List<string>();
        // new list
        static List<string> nsUrls = new List<string>();

        public static List<string> osBlogList = new List<string>();
        public static List<string> osDoctorList = new List<string>();
        public static string[] osBlogParams = { "/news/", "/blogs/" };
        public static string[] osDoctorParams = { "/provider/", "/providers/" };
        public static string[] osDoctorTitles = { "md", "do", "mph", "pa-c", "macp", "mba", "agacnp-bc", "np", "np-c", "fnp-c", "msn", "aprn", "phd", "ms", "cnm", "facs", "facog", "facp", "dpm", "mmci", "rd", "cdn", "msc", "facmg", "fapa", "mhs", "faaa", "np-bc", "cgc", "facr", "med", "whnp-bc" };
        public static List<string[]> osParamArrays = new List<string[]>();


        public static List<string> nsBlogList = new List<string>();
        public static List<string> nsDoctorList = new List<string>();
        public static string[] nsBlogParams = { "/blog/" };
        public static string[] nsDoctorParams = { "/doctors/" };

        // list out number of found urls
        static int foundMatch = 0;
        static int lostMatch = 0;


        static void Main(string[] args)
        {

            // push params into param list arrays
            //osParamArrays.Add(osBlogParams, osDoctorParams);


            // initialize paths to files
            string osUrlFile = @"C:\Users\timothy.darrow\Dropbox\Coding\Scorpion\C#\RedirectMachine\OldSiteUrls.csv";
            string nsUrlFile = @"C:\Users\timothy.darrow\Dropbox\Coding\Scorpion\C#\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // add params to the respective array lists
            osParamArrays.Add(osBlogParams);
            osParamArrays.Add(osDoctorParams);


            ReadCSV(osUrls, osUrlFile, false);
            ReadCSV(nsUrls, nsUrlFile, true);


            Console.WriteLine("size of oldUrls list: " + osUrls.Count);
            Console.WriteLine("size of newUrls list: " + nsUrls.Count);
            Console.WriteLine("size of osBlogList: " + osBlogList.Count);
            Console.WriteLine("size of nsBlogList: " + nsBlogList.Count);
            Console.WriteLine("size of osDoctorList: " + osDoctorList.Count);
            Console.WriteLine("size of nsDoctorList: " + nsDoctorList.Count);

            Console.WriteLine("begin search: ");

            // search url lists for new items
            findUrl(osBlogList, nsBlogList);
            findUrl(osDoctorList, nsDoctorList);

            buildCSV(lostList, lostUrlFile);
            buildCSV(foundList, foundUrlFile);

            //buildCSV(osDoctorList, @"C:\Users\timothy.darrow\Downloads\osDoctorList.csv");
            //buildCSV(nsDoctorList, @"C:\Users\timothy.darrow\Downloads\nsDoctorList.csv");

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine("lostMatch count: " + lostMatch);
            Console.WriteLine("foundMatch count: " + foundMatch);

            Console.WriteLine("lostList count: " + lostList.Count);
            Console.WriteLine("foundList count: " + foundList.Count);


            Console.WriteLine("Run time: " + elapsedTime);
        }

        // End of main function

        // function to find doctors in url
        public static void findDoctorUrl(List<string> oldList, List<string> newList)
        {
            foreach (var path in oldList)
            {

                bool temp = checkList(path, newList);
                if (temp == false)
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


        public static void findUrl(List<string> oldList, List<string> newList)
        {
            foreach (var path in oldList)
            {
                bool temp = checkList(path, newList);
                if (temp == false)
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


        // delete checklist 
        public static bool checkList(string value, List<string> urls)
        {
            // get last piece of url in string
            string subString = TruncateString(value, 48);

            foreach (var item in urls)
            {
                if (item.Contains(subString))
                {
                    foundList.Add(value + "," + item);
                    return true;
                }
            }
            return false;
        }


        public static string TruncateString(string value, int maxLength)
        {
            string temp = value;
            // Step 1: remove slash on end of url if found
            if (temp.EndsWith("/"))
                temp = temp.Substring(0, temp.Length - 1);
            else if (temp.EndsWith("/*"))
                temp = temp.Substring(0, temp.Length - 2);

            // Step 2: get url text after last slash in url
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);

            // Step 3: truncate temporary value to maxLength
            if (string.IsNullOrEmpty(temp)) return temp;
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        static void ReadCSV(List<string> list, string filePath, bool conditional)
        {
            // add CSV file contents to list
            bool foundHome;

            using (var reader = new StreamReader(@"" + filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    foundHome = false;
                    if (conditional == false)
                    {
                        if ((checkParams(osBlogList, osBlogParams, line) == true) || (checkParams(osDoctorList, osDoctorParams, line) == true))
                            foundHome = true;

                        //foundHome = checkParams(osBlogList, osBlogParams, line);
                        //foundHome = checkParams(osDoctorList, osDoctorParams, line);

                    }
                    else
                    {
                        if ((checkParams(nsBlogList, nsBlogParams, line) == true) || (checkParams(nsDoctorList, nsDoctorParams, line) == true))
                            foundHome = true;
                    }

                    // if a parameter couldn't be found, change 
                    if (foundHome == false)
                        list.Add(line);
                }

                list.Sort();
            }
        }

        // check if item is in array of potential solutions
        static bool checkParams(List<string> list, string[] variables, string value)
        {
            foreach (var item in variables)
            {
                if (value.Contains(item))
                {
                    // add item to list if found
                    list.Add(value);
                    return true;
                }
            }
            return false;
        }

        // check if item is in array of potential solutions
        static bool checkParams(List<string> list, string value)
        {
            // var i is a reference to an array of parameters passed into this method
            foreach (var i in list)
            {
                // var j is a reference to a specific parameter found in the array of parameters from i
                foreach (var j in i)
                {
                    // if a match is found, add it to the appropriate list to be searched later
                    if (value.Contains(j))
                    {
                        // add i to list if found
                        list.Add(value);
                        return true;
                    }
                }
            }
            Console.WriteLine('test');
            return false;
        }

        // method that builds a new CSV for the user to view at the specified file path
        static void buildCSV(List<string> list, string filePath)
        {
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    var newLine = string.Format("{0}", item);
                    tw.WriteLine(newLine);
                }
            }
        }
    }
}
