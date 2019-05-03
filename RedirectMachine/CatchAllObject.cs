using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedirectMachine
{
    internal class CatchAllObject
    {
        internal string[,] catchAllParams =  {
            { "http://www.ricehospital.com/event/", "/classes-events/" },
            { "https://www.ricehospital.com/event/", "/classes-events/" },
            { "http://www.ricehospital.com/events/", "/classes-events/" },
            { "https://www.ricehospital.com/events/", "/classes-events/" },
            { "http://redwoodareahospital.org/event/", "/classes-events/" },
            { "https://redwoodareahospital.org/event/", "/classes-events/" },
            { "http://redwoodareahospital.org/events/", "/classes-events/" },
            { "https://redwoodareahospital.org/events/", "/classes-events/" },
            { "https://www.ricehospital.com/wellness-resources/support-groups/", "/classes-events/" },
            { "https://www.ricehospital.com/contact/", "/contact-us/" },
            { "https://redwoodareahospital.org/category/", "/blog/" },
            { "http://ricehospice.com/tag/", "/blog/" },
            { "https://www.ricehospital.com/blog/author/", "/blog/" },
            { "https://www.ricehospital.com/blog/category/", "/blog/" },
            { "https://www.ricehospital.com/blog/tag/", "/blog/" },
            { "http://discoveracmc.com/author/", "/blog/" },
            { "http://discoveracmc.com/category/", "/blog/" },
            { "http://discoveracmc.com/tag/", "/blog/" },
            { "https://www.ricehospital.com/blog/nursery/", "/blog/" },
            { "https://www.ricehospital.com/care-services/birth-suites/planning-for-baby/", "/services/birthing-services/carris-health-rice-memorial-hospital-birthing-services/" },
            { "http://discoveracmc.com/wp-content/uploads/", "/" },
            { "https://www.ricehospital.com/wp-content/", "/" }
        };

        Dictionary<string, int> catchAllList;

        /// <summary>
        /// default working constructor
        /// </summary>
        public CatchAllObject()
        {
            catchAllList = new Dictionary<string, int>();
        }

        /// <summary>
        /// checks to see if the url belongs to one of the existing osParam catchAlls. If it does, don't do anything further with it. Essentially ignore that object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool CheckCatchallParams(string url)
        {
            for (int i = 0; i < catchAllParams.GetLength(0); i++)
            {
                if (url.StartsWith(catchAllParams[i, 0].ToString().ToLower()))
                    return true;
            }
            if (url.Contains("?"))
            {
                var obj = new UrlUtils(url);
                CheckNewCatchAlls(obj.SanitizedUrl);
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
            if (!url.EndsWith("/"))
                url = url + "/";
            if (!catchAllList.ContainsKey(url))
                catchAllList.Add(url, 1);
            else
                catchAllList[url] = catchAllList[url] + 1;
        }

        /// <summary>
        /// Sort catchAllList and then export catchAllList to CSV to specified filepath
        /// </summary>
        /// <param name="filePath"></param>
        internal void ExportCatchAllsToCSV(string filePath)
        {
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                tw.WriteLine("Potential Probability,Number of times seen");
                foreach (var keyValuePair in catchAllList)
                {
                    if (keyValuePair.Value > 1)
                        tw.WriteLine($"{keyValuePair.Key},{keyValuePair.Value}");
                }

                for (int i = 0; i < catchAllParams.GetLength(0); i++)
                {
                    tw.WriteLine($"{catchAllParams[i, 0]}*, {catchAllParams[i, 1]}");
                }
            }
        }

        //internal string[] ExportCatchAllParams()
        //{
        //}
    }
}