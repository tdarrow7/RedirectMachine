using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedirectMachine
{
    internal class CatchAllUtils
    {
        internal List<Tuple<string, string>> catchAllParams;

        Dictionary<string, CatchAllUrl> catchAllList;
        internal int CatchAllCount = 0;
        
        /// <summary>
        /// default working constructor
        /// </summary>
        public CatchAllUtils()
        {
            catchAllList = new Dictionary<string, CatchAllUrl>();
            catchAllParams = new List<Tuple<string, string>>();
        }


        /// <summary>
        /// Create working list of all pararameters that need to be checked.
        /// </summary>
        /// <param name="urlFile"></param>
        public void GenerateCatchAllParams(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] tempArray = reader.ReadLine().ToLower().Split(",");
                    catchAllParams.Add(new Tuple<string, string>(tempArray[0], tempArray[1]));
                }
            }
        }

        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool CheckExistingCatchallParams(string url)
        {
            foreach (var tuple in catchAllParams)
            {
                if (url.StartsWith(tuple.Item1))
                {
                    CatchAllCount++;
                    return true;
                }
                    
            }
            if (url.Contains("?"))
            {
                CatchAllCount++;
                string urlSub = url.Split("?")[0] + "?";
                catchAllParams.Add(new Tuple<string, string>(urlSub, "(needs to be determined)"));
                checkIfCatchAllIsCreated(urlSub);
                return true;
            }
            return false;
        }

        /// <summary>
        /// check to see if we already have the catchall
        /// </summary>
        /// <param name="url"></param>
        internal void checkIfCatchAllIsCreated(string url)
        {
            if (!catchAllList.ContainsKey(url))
            {
                catchAllList.Add(url, new CatchAllUrl(url));
                Console.WriteLine(url);
            }
            else
            {
                catchAllList[url].IncreaseCount();
                Console.WriteLine(url);
            }
                
        }


        /// <summary>
        /// Sort catchAllList and then export catchAllList to CSV to specified filepath
        /// </summary>
        /// <param name="filePath"></param>
        public void ExportCatchAllsToCSV(string filePath)
        {
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                tw.WriteLine("Potential Probability,Number of times seen");
                foreach (var keyValuePair in catchAllList)
                {
                    //if (keyValuePair.Value.Count > 1)
                        tw.WriteLine($"{keyValuePair.Key},{keyValuePair.Value.Count}");
                }
                foreach (var tuple in catchAllParams)
                {
                    tw.WriteLine($"{tuple.Item1}*,{tuple.Item2}");
                }
            }
            Console.WriteLine($"Number of urls turned to catchalls: {CatchAllCount}");
        }
    }
}