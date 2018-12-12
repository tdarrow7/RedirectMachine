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
        public static string[,] osParams = { { "/bakersfield/pages/ehealth/kramescontent/", "/bakersfield/*" }, { "/castle/event/", "/castle/classes-and-events/*" }, { "/central-valley/ages/ehealth/kramescontent/", "/blog/*" } };
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
            string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
            string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\FoundUrls.csv";

            string osDocUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSite_docs.csv";
            string osBlogUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSite_blog.csv";


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ReadCSV(osUrls, osUrlFile);
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
            //FilterUrls(osUrls, osParams);
            findUrl(osUrls, nsUrls);

            foreach (var item in osParams)
            {
                Console.WriteLine($"item[0]: {item[0]}");
                Console.WriteLine($"item[1]: {item[1]}");
            }

            buildCSV(lostList, lostUrlFile);
            buildCSV(foundList, foundUrlFile);

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

        // End of main function

        public static void FilterUrls(List<string> list, string[,] i)
        {
            foreach (var j in i)
            {
                list.RemoveAll(item => item == j[0].ToString());
                list.Add(j[1].ToString());
                Console.WriteLine($"line 102 - j[1]: {j[1].ToString()}");
            }
        }

        public static void findUrl(List<string> oldList, List<string> newList)
        {
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
            // Step 1: remove unnecessary contents on end of url if found
            // Step 2: get url text after last slash in url
            // Step 3: truncate temporary value to maxLength
            string temp = value;
            int index = value.Length;
            if (temp.EndsWith("/"))
                index = getIndex(temp, "/");
            else if (temp.EndsWith("-"))
                index = getIndex(temp, "-");
            else if (temp.EndsWith("/*"))
                index = getIndex(temp, "/*");
            else if (temp.Contains(".aspx"))
            {
                index = getIndex(temp, ".aspx");
            }
            temp = temp.Substring(0, index);

            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            if (string.IsNullOrEmpty(temp)) return temp;
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        public static int getIndex(string i, string j)
        {
            return i.LastIndexOf(j) - 1;
        }

        static void ReadCSV(List<string> list, string filePath)
        {
            // add CSV file contents to list
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

        static void PrintList(List<string> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
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
