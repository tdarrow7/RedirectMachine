using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class RedirectUrl
    {
        private int count;
        private bool score;
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
        /// <returns></returns>
        internal bool BasicUrlFinder(List<string> newUrlSiteMap)
        {
            foreach (var url in newUrlSiteMap)
            {
                if (url.Contains("/about-us/"))
                    Console.WriteLine("found the url");
                string temp = obj.BasicTruncateString(url);
                if (temp.Contains(obj.UrlTail) && MatchDirectoryHeaderMaps(obj, url))
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
        /// <returns></returns>
        private bool MatchDirectoryHeaderMaps(UrlUtils obj, string url)
        {
            if (obj.HasHeaderMap)
                return url.StartsWith(obj.urlHeaderMap[1]);
            else if (obj.IsParentDir)
                return url.StartsWith(obj.UrlHead);
            else
                return url.StartsWith(obj.UrlHead);
        }

        /// <summary>
        /// basic scan of matched urls
        /// if no urls were found, return false to report none were found
        /// if exactly one match is found, return true to report a match was found
        /// </summary>
        private bool BasicScanMatchedUrls()
        {
            if (Count == 0)
                return false;
            else if (Count == 1)
            {
                SetNewUrl();
                return true;
            }
            else
            {
                Count = 0;
                List<string> list1 = matchedUrls.ToList();
                foreach (var url in list1)
                {
                    if (!BasicCheckUrlParentDir(obj.TruncateStringHead(url)))
                        RemoveFromMatchedUrls(url);
                }
                if (Count == 1)
                {
                    SetNewUrl();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// scan every url in newUrlSiteMap. If the url's resource directory contains the first chunk from obj.urlChunks[], add it to a list of potential matches
        /// return whatever AdvancedScanMatchedUrls finds
        /// </summary>
        /// <param name="newUrlSiteMap"></param>
        /// <returns></returns>
        internal bool AdvancedUrlFinder(List<string> newUrlSiteMap)
        {
            foreach (var url in newUrlSiteMap)
            {
                string temp = obj.BasicTruncateString(url);
                if (temp.Contains(obj.GetChunk(0)) && MatchDirectoryHeaderMaps(obj, url))
                    matchedUrls.Add(url);
            }
            Count = matchedUrls.Count;
            return AdvancedScanMatchedUrls();
        }

        /// <summary>
        /// Create two temporary lists.
        /// build string temp out of chunks from original url. for every iteration of the for loop, add a new chunk to the end of the url
        /// check if chunk is contained in each of the new urls. If it is, keep. If not, remove from passive list and run RemoveFromMatchedUrls method
        /// if Count == 0, obviously no matching url wasn't found. Return false
        /// if Count == 1, a single match has been found. Return true
        /// if Count > 1, run the for loop again to build a new substring of originalUrl
        /// </summary>
        public bool AdvancedScanMatchedUrls()
        {
            List<string> activeList = new List<string>();
            List<string> passiveList = matchedUrls.ToList();

            for (int i = 0; i < obj.GetChunkLength(); i++)
            {
                activeList.Clear();
                activeList = passiveList.ToList();
                string temp = obj.BuildChunk(i);
                foreach (var url in activeList)
                {
                    if (!url.Contains(temp))
                    {
                        passiveList.Remove(url);
                        RemoveFromMatchedUrls(url);
                    }
                }
                if (Count == 0)
                    return false;
                if (Count == 1)
                {
                    SetNewUrl();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// set the new url to the only matched url left in matchedUrls list
        /// </summary>
        private void SetNewUrl()
        {
            obj.NewUrl = matchedUrls.First();
            Score = true;
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
        /// return the substring of the string that is passed into this function.
        /// This method is overloaded with a bool. The bool indicates to the function that it must return a substring
        /// 1) if true, includes the string j rather than excluding it, or
        /// 2) if false, returns a substring that excludes string j.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="x"></param>
        internal string GetSubString(string i, string j, bool x)
        {
            int index = GetLastIndex(i, j);
            return (x == true) ? i.Substring(0, index + j.Length) : i.Substring(0, index);
        }

        /// <summary>
        /// return the substring of the string that is passed into this function.
        /// This method is overloaded with an int. The int indicates to the function that it must rerun that many times.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        internal string GetSubString(string i, string j, int x)
        {
            var pos = 0;
            string temp = i;
            while (pos <= x)
            {
                int index = GetLastIndex(i, j);
                temp = temp.Substring(0, index);
                pos++;
            }
            return temp;
        }

        /// <summary>
        /// return FIRST position of j variable in string i.
        /// If j is not found in i, return i.Length
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static int GetFirstIndex(string i, string j)
        {
            return i.Contains(j) ? i.IndexOf(j) : i.Length;
        }

        /// <summary>
        /// return LAST position of j variable in string i.
        /// If j is not found in i, return i.Length
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static int GetLastIndex(string i, string j)
        {
            return i.Contains(j) ? i.LastIndexOf(j) : i.Length;
        }

        /// <summary>
        /// returns either true or false depending on whether or not the HeaderMap[1] contains the string temp
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
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