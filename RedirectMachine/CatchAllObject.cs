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
        public bool CheckCatchallParams(string url)
        {
            foreach (var tuple in catchAllParams)
            {
                if (url.StartsWith(tuple.Item1))
                    return true;
            }
            if (url.Contains("?"))
            {
                string urlSub = url.Split("?")[0] + "?";
                CheckNewCatchAlls(urlSub);
                return true;
            }
            return false;
        }

        /// <summary>
        /// check to see if we already have the catchall
        /// </summary>
        /// <param name="url"></param>
        internal void CheckNewCatchAlls(string url)
        {
            if (!catchAllList.ContainsKey(url))
            {
                catchAllList.Add(url, new CatchAllUrl(url));
                if (url.EndsWith("?"))
                    catchAllParams.Add(new Tuple<string, string>(url, "(needs to be determined)"));
            }
            else
                catchAllList[url].IncreaseCount();
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
                    if (keyValuePair.Value.Count > 1)
                        tw.WriteLine($"{keyValuePair.Key}*,{keyValuePair.Value.Count}");
                }
                foreach (var tuple in catchAllParams)
                {
                    tw.WriteLine($"{tuple.Item1},{tuple.Item2}");
                }
            }
        }
    }
}