using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class RedirectUrl
    {
        public List<string> matchedUrls;
        internal UrlUtils obj;
        public bool Score { get; set; } = false;
        public int Count { get; set; } = 0;

        public RedirectUrl()
        {
            // default contstructor
        }


        /// <summary>
        /// Working constructor. Nearlky all properties are housed in teh UrlUtils obj.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlHeaderMaps"></param>
        public RedirectUrl(string url, string[,] urlHeaderMaps)
        {
            obj = new UrlUtils(url);
            matchedUrls = new List<string>();
            CheckUrlHeaderMaps(obj, urlHeaderMaps);
        }

        /// <summary>
        /// check to see if the url from obj contains one of the urlHeaderMap entries.
        /// e.g. www.google.com redirecting to /google/ on the new site
        /// </summary>
        /// <param name="obj"></param>
        private void CheckUrlHeaderMaps(UrlUtils obj, string[,] urlHeaderMaps)
        {
            for (int i = 0; i < urlHeaderMaps.GetLength(0); i++)
            {
                if (obj.SanitizedUrl.Contains(urlHeaderMaps[i, 0]))
                {
                    obj.HasHeaderMap = true;
                    obj.SetUrlHeaderMap(urlHeaderMaps[i, 0], urlHeaderMaps[i, 1]);
                }
            }
        }

        /// <summary>
        /// scan every url in newUrlSiteMap list. if the resource directory of the url contains the object's UrlTail, add it to matched urls
        /// return the results of BasicScanMatchedUrls()
        /// </summary>
        /// <param name="newUrlSiteMap"></param>
        internal bool BasicUrlFinder(HashSet<string> newUrlSiteMap)
        {
            foreach (var url in newUrlSiteMap)
            {
                string temp = obj.BasicTruncateString(url);
                if (temp.Contains(obj.UrlTail) && CheckParentAndResourceDirs(obj, url))
                    AddMatchedUrl(url);
            }
            return BasicScanMatchedUrls();
        }

        /// <summary>
        /// this method simply catches all urls that can immediately be removed from being evaluated.
        /// If there is a header map, does the url start with that header map? if yes, return true. If no, return false
        /// Else, if there's no header map, does the url's parent directory match the old url's parent directory? return either true or false
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="url"></param>
        private bool CheckParentAndResourceDirs(UrlUtils obj, string url)
        {
            if (!CheckResourceDirs(obj, url))
                return false;
            if (obj.HasHeaderMap)
                return url.StartsWith(obj.urlHeaderMap[1]);
            else if (obj.IsParentDir)
                return url.StartsWith(obj.UrlHead);
            else
                return true;
        }

        private bool CheckResourceDirs(UrlUtils obj, string url)
        {
            if (obj.UrlTail.Contains("."))
            {
                if (url.Contains("."))
                {
                    string x = obj.UrlTail.Split(".")[1];
                    string y = url.Split(".")[1];
                    return (x == y) ? true : false;
                }
                else return false;
            }
            return true;
        }

        /// <summary>
        /// basic scan of matched urls
        /// if no urls were found, return false to report none were found
        /// if exactly one match is found, return true to report a match was found
        /// </summary>
        private bool BasicScanMatchedUrls()
        {
            return (Count == 1) ? SetNewUrl() : false;
        }

        /// <summary>
        /// scan every url in newUrlSiteMap. If the url's resource directory contains the first chunk from obj.urlChunks[], add it to a list of potential matches
        /// return whatever AdvancedScanMatchedUrls finds
        /// </summary>
        /// <param name="newUrlSiteMap"></param>
        internal bool AdvancedUrlFinder(HashSet<string> newUrlSiteMap)
        {
            foreach (var url in newUrlSiteMap)
            {
                string temp = obj.BasicTruncateString(url);
                if (temp.Contains(obj.GetChunk(0)) && CheckParentAndResourceDirs(obj, url))
                    matchedUrls.Add(url);
            }
            Count = matchedUrls.Count;
            //return AdvancedScanMatchedUrls();
            return AdvancedScanMatchedUrlsV2();
        }

        ///// <summary>
        ///// Create two temporary lists.
        ///// build string temp out of chunks from original url. for every iteration of the for loop, add a new chunk to the end of the url
        ///// check if chunk is contained in each of the new urls. If it is, keep. If not, remove from passive list and run RemoveFromMatchedUrls method
        ///// if Count == 0, obviously no matching url wasn't found. Return false
        ///// if Count == 1, a single match has been found. Return true
        ///// if Count > 1, run the for loop again to build a new substring of originalUrl
        ///// </summary>
        //private bool AdvancedScanMatchedUrls()
        //{
        //    List<string> activeList = new List<string>();
        //    List<string> passiveList = matchedUrls.ToList();

        //    for (int i = 0; i < obj.GetChunkLength(); i++)
        //    {
        //        activeList.Clear();
        //        activeList = passiveList.ToList();
        //        string temp = obj.BuildChunk(i);
        //        foreach (var url in activeList)
        //        {
        //            if (!url.Contains(temp))
        //            {
        //                passiveList.Remove(url);
        //                RemoveFromMatchedUrls(url);
        //            }
        //        }
        //        if (Count == 0)
        //            return false;
        //        if (Count == 1)
        //            return FinalAdvancedScanCheck(i);
        //    }
        //    return false;
        //}

        /// <summary>
        /// Create two temporary lists.
        /// build string temp out of chunks from original url. for every iteration of the for loop, add a new chunk to the end of the url
        /// check if chunk is contained in each of the new urls. If it is, keep. If not, remove from passive list and run RemoveFromMatchedUrls method
        /// if Count == 0, obviously no matching url wasn't found. Return false
        /// if Count == 1, a single match has been found. Return true
        /// if Count > 1, run the for loop again to build a new substring of originalUrl
        /// </summary>
        private bool AdvancedScanMatchedUrlsV2()
        {
            Dictionary<string, int> activeList = new Dictionary<string, int>();
            var tupleList = new List<Tuple<string, int, int>>();
            int x = 1;
            string temp = "";
            foreach (var url in matchedUrls)
            {
                int y = 0;
                string[] tempArray = url.Split(new Char[] { '-', '/' });
                for (int i = 0; i < tempArray.Length; i++)
                {
                    if (obj.urlChunksV2.Contains(tempArray[i]))
                        y++;
                }
                if (!activeList.ContainsKey(url))
                    activeList.Add(url, y);
            }

            matchedUrls.Clear();
            matchedUrls = activeList.Keys.ToList();

            foreach (var keyValuePair in activeList)
            {
                if (keyValuePair.Value < x)
                    RemoveFromMatchedUrls(keyValuePair.Key);
                else
                {
                    RemoveFromMatchedUrls(temp);
                    temp = keyValuePair.Key;
                    x = keyValuePair.Value;
                }
            }
            return (matchedUrls.Count == 1) ? SetNewUrl() : false;
        }


        private bool AdvancedScanMatchedUrlsV3()
        {
            var tupleList = new List<Tuple<string, int, int>>();
            int x = 1,
                y = 1;
            string temp = "";
            foreach (var url in matchedUrls)
            {
                int z = 0;
                string[] tempArray = url.Split(new Char[] { '-', '/' });
                for (int i = 0; i < tempArray.Length; i++)
                {
                    if (obj.urlChunksV2.Contains(tempArray[i]))
                        z++;
                }
                if (!tupleList.Exists(i => i.Item1 == url))
                    tupleList.Add(new Tuple<string, int, int>(url, z, AdvancedScanUrlResourceDirV2(url)));
            }

            matchedUrls.Clear();
            foreach (var item in tupleList)
            {
                matchedUrls.Add(item.Item1);
            }
            Count = matchedUrls.Count;

            foreach (var item in tupleList)
            {
                if (item.Item2 < x && item.Item3 < y)
                    RemoveFromMatchedUrls(item.Item1);
                else
                {
                    RemoveFromMatchedUrls(temp);
                    temp = item.Item1;
                    x = item.Item2;
                    y = item.Item3;
                }
            }
            return (matchedUrls.Count == 1) ? SetNewUrl() : false;
        }

        private int AdvancedScanUrlResourceDirV2(string url)
        {
            int j = 0;
            string[] chunks = obj.ReturnResourceDirChunks(url);
            for (int i = 0; i < chunks.Length; i++)
            {
                if (obj.IsChunkFoundInResource(chunks[i]))
                    j++;
            }
            return j;
        }

        //private bool FinalAdvancedScanCheck(int i)
        //{
        //    if (i < obj.GetChunkLength())
        //    {
        //        string temp = obj.BuildChunk(i + 1);
        //        return (matchedUrls.First().Contains(temp)) ? SetNewUrl() : false;
        //    }
        //    else return true;
        //}

        /// <summary>
        /// set the new url to the only matched url left in matchedUrls list
        /// </summary>
        private bool SetNewUrl()
        {
            obj.NewUrl = matchedUrls.First();
            Score = true;
            return true;
        }

        /// <summary>
        /// remove a matched url from matchedUrls list that corresponds with the value of url string
        /// </summary>
        /// <param name="url"></param>
        private void RemoveFromMatchedUrls(string url)
        {
            matchedUrls.Remove(url);
            int c = Count - 1;
            Count = c;
        }

        /// <summary>
        /// if the hasUrlHeaderMap bool is set to true, check to see if the old url's parent directory matches string temp. if not, return false
        /// if the hasUrlHeaderMap bool is set to false, check if string temp contains parent directory of old url. if not, return false
        /// return true by default
        /// </summary>
        /// <param name="temp"></param>
        private bool BasicCheckUrlParentDir(string temp)
        {
            if (obj.HasHeaderMap)
                return (UrlHeaderMatch(temp));
            else
                return (temp.Contains(obj.UrlHead));
        }

        /// <summary>
        /// return private string sanitizedUrl
        /// </summary>
        internal string GetSanitizedUrl()
        {
            return obj.SanitizedUrl;
        }

        /// <summary>
        /// return private string originalUrl
        /// </summary>
        /// <returns></returns>
        internal string GetOriginalUrl()
        {
            return obj.OriginalUrl;
        }

        /// <summary>
        /// return private string newUrl
        /// </summary>
        /// <returns></returns>
        internal string GetNewUrl()
        {
            return obj.NewUrl;
        }

        /// <summary>
        /// returns either true or false depending on whether or not the HeaderMap[1] contains the string temp
        /// </summary>
        /// <param name="temp"></param>
        public bool UrlHeaderMatch(string temp)
        {
            return obj.urlHeaderMap[1].Contains(temp);
        }

        /// <summary>
        /// add potential url match
        /// </summary>
        /// <param name="link"></param>
        public void AddMatchedUrl(string link)
        {
            matchedUrls.Add(link);
            int i = Count + 1;
            Count = i;
        }
    }
}