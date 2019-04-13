using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class RedirectUrl
    {
        public List<string> matchedUrls;
        internal UrlUtils urlUtil;
        public bool Score { get; set; } = false;
        public int Count { get; set; } = 0;
        private int totalResourceChunkMatches;
        private int totalUrlChunkMatches;


        public RedirectUrl()
        {
            // default contstructor
        }


        /// <summary>
        /// Working constructor. Nearlky all properties are housed in teh UrlUtils obj.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlHeaderMaps"></param>
        public RedirectUrl(string originalUrl, string[,] urlHeaderMaps)
        {
            urlUtil = new UrlUtils(originalUrl);
            totalResourceChunkMatches = (urlUtil.IsResourceFile) ? 2 : 1;
            totalUrlChunkMatches = (urlUtil.IsResourceFile) ? 2 : 1;
            matchedUrls = new List<string>();
            CheckUrlHeaderMaps(urlUtil, urlHeaderMaps);
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
                string temp = urlUtil.BasicTruncateString(url);
                if (temp.Contains(urlUtil.UrlTail) && CheckParentAndResourceDirs(urlUtil, url))
                    AddMatchedUrl(url);
            }
            return BasicScanMatchedUrls();
        }

        /// <summary>
        /// this method simply catches all urls that can immediately be removed from being evaluated.
        /// If there is a header map, does the url start with that header map? if yes, return true. If no, return false
        /// Else, if there's no header map, does the url's parent directory match the old url's parent directory? return either true or false
        /// </summary>
        /// <param name="urlUtils"></param>
        /// <param name="url"></param>
        private bool CheckParentAndResourceDirs(UrlUtils urlUtils, string url)
        {
            if (!CheckResourceDirs(urlUtils, url))
                return false;
            if (urlUtils.HasHeaderMap)
                return url.StartsWith(urlUtils.urlHeaderMap[1]);
            else if (urlUtils.IsParentDir)
                return url.StartsWith(urlUtils.UrlHead);
            else
                return true;
        }

        private bool CheckResourceDirs(UrlUtils urlUtils, string url)
        {
            if (urlUtils.UrlTail.Contains("."))
            {
                if (url.Contains("."))
                {
                    string x = urlUtils.UrlTail.Split(".")[1];
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
            resetMatchedUrls();
            var tupleList = new List<Tuple<string, int, int>>();
            foreach (var url in newUrlSiteMap)
            {
                string temp = this.urlUtil.BasicTruncateString(url);

                if (temp.Contains(this.urlUtil.GetChunk(0)) && CheckParentAndResourceDirs(this.urlUtil, url))
                {
                    if (!tupleList.Exists((Tuple<string, int, int> i) => i.Item1 == url))
                    {
                        string[] allUrlChunks = this.urlUtil.ReturnAllUrlChunks();
                        string[] urlTailChunks = this.urlUtil.ReturnUrlTailChunks();
                        tupleList.Add(new Tuple<string, int, int>(url, this.urlUtil.ReturnUrlMatches(url, allUrlChunks), this.urlUtil.ReturnUrlMatches(url, urlTailChunks) * 2));
                    }
                }
            }
            return AdvancedScanMatchedUrls(tupleList);
        }

        /// <summary>
        /// scan every url in newUrlSiteMap. If the url's resource directory contains the first chunk from obj.urlChunks[], add it to a list of potential matches
        /// return whatever AdvancedScanMatchedUrls finds
        /// </summary>
        /// <param name="newUrlSiteMap"></param>
        internal bool ReversedAdvancedUrlFinder(HashSet<string> newUrlSiteMap)
        {
            resetMatchedUrls();
            var tupleList = new List<Tuple<string, int, int>>();
            foreach (var url in newUrlSiteMap)
            {
                string temp = this.urlUtil.UrlTail;

                if (temp.Contains(this.urlUtil.GetChunk(0)) && CheckParentAndResourceDirs(this.urlUtil, url))
                {
                    if (!tupleList.Exists((Tuple<string, int, int> i) => i.Item1 == url))
                    {
                        string[] allUrlChunks = this.urlUtil.SplitUrlChunks(url);
                        string[] urlTailChunks = this.urlUtil.SplitUrlChunks(this.urlUtil.BasicTruncateString(url));
                        tupleList.Add(new Tuple<string, int, int>(url, this.urlUtil.ReturnUrlMatches(url, allUrlChunks), this.urlUtil.ReturnUrlMatches(url, urlTailChunks) * 2));
                    }
                }
            }
            return AdvancedScanMatchedUrls(tupleList);
        }

        private bool AdvancedScanMatchedUrls(List<Tuple<string, int, int>> tupleList)
        {
            int a = 1,
                b = 1;
            foreach (var item in tupleList)
            {
                if (item.Item2 < a || item.Item3 < b) { }
                else if (item.Item2 > a && item.Item3 > b)
                {
                    resetMatchedUrls();
                    AddMatchedUrl(item.Item1);
                    a = item.Item2;
                    b = item.Item3;
                }
                else
                    AddMatchedUrl(item.Item1);
            }
            return (Count == 1) ? SetNewUrl() : false;
        }

        /// <summary>
        /// set the new url to the only matched url left in matchedUrls list
        /// </summary>
        private bool SetNewUrl()
        {
            urlUtil.NewUrl = matchedUrls.First();
            Score = true;
            return true;
        }


        /// <summary>
        /// set matchUrls list to zero, and reset count to zero
        /// </summary>
        private void resetMatchedUrls()
        {
            matchedUrls.Clear();
            Count = 0;
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
            if (urlUtil.HasHeaderMap)
                return (UrlHeaderMatch(temp));
            else
                return (temp.Contains(urlUtil.UrlHead));
        }

        /// <summary>
        /// return private string sanitizedUrl
        /// </summary>
        internal string GetSanitizedUrl()
        {
            return urlUtil.SanitizedUrl;
        }

        /// <summary>
        /// return private string originalUrl
        /// </summary>
        /// <returns></returns>
        internal string GetOriginalUrl()
        {
            return urlUtil.OriginalUrl;
        }

        /// <summary>
        /// return private string newUrl
        /// </summary>
        /// <returns></returns>
        internal string GetNewUrl()
        {
            return urlUtil.NewUrl;
        }

        /// <summary>
        /// returns either true or false depending on whether or not the HeaderMap[1] contains the string temp
        /// </summary>
        /// <param name="temp"></param>
        public bool UrlHeaderMatch(string temp)
        {
            return urlUtil.urlHeaderMap[1].Contains(temp);
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

        private int CheckResourceChunks(string[] chunks)
        {
            int x = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                if (urlUtil.UrlTail.Contains(chunks[i]))
                    x++;
            }
            return x;
        }

        private int CheckTotalUrlChunks (string[] chunks)
        {
            int x = 0;
            for (int i = 0; i < chunks.Length; i++)
            {
                if (urlUtil.OriginalUrl.Contains(chunks[i]))
                    x++;
            }
            return x;
        }

    }
}