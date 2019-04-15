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
        internal bool BasicUrlFinder(List<string> newUrlSiteMap)
        {
            foreach (var url in newUrlSiteMap)
            {
                string temp = urlUtil.BasicTruncateString(url);
                if (temp.Contains(urlUtil.UrlResourceDir) && CheckParentAndResourceDirs(urlUtil, url))
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
                return url.StartsWith(urlUtils.UrlParentDir);
            else
                return true;
        }

        private bool CheckResourceDirs(UrlUtils urlUtils, string url)
        {
            if (urlUtils.UrlResourceDir.Contains("."))
            {
                if (url.Contains("."))
                {
                    string x = urlUtils.UrlResourceDir.Split(".")[1];
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
        internal bool AdvancedUrlFinder(List<string> newUrlSiteMap)
        {
            resetMatchedUrls();
            var tupleList = new List<Tuple<string, int, int>>();
            foreach (var url in newUrlSiteMap)
            {
                string temp = urlUtil.BasicTruncateString(url);
                string[] allOriginalUrlChunks = urlUtil.ReturnAllUrlChunks();
                string[] originalUrlResourceChunks = urlUtil.ReturnUrlResourceChunks();

                if (temp.Contains(originalUrlResourceChunks[0]) && CheckParentAndResourceDirs(urlUtil, url))
                {
                    if (!tupleList.Exists((Tuple<string, int, int> i) => i.Item1 == url))
                        tupleList.Add(new Tuple<string, int, int>(url, this.urlUtil.ReturnUrlMatches(url, allOriginalUrlChunks), this.urlUtil.ReturnUrlMatches(url, originalUrlResourceChunks) * 2));
                }
            }
            return AdvancedScanMatchedUrls(tupleList);
        }

        /// <summary>
        /// create temporary tupleList to pass to other methods
        /// scan every url in newUrlSiteMap. 
        /// set string temp to the urlUtil resource directory value
        /// If temp contains the first chunk from urlUtil.urlResourceChunks[] and it passes the CheckParentAndResourceDirs() audit
        /// As long as the tupleList doesn't contain the url already (failsafe against duplicates)
        ///     set allUrlChunks[] array to take in the contents from the urlUtil.SplitUrlChunks(url) method. Note that the url being split is the newly scanned url
        ///     set urlResourceChunks[] array to take in the contents from the urlUtil.SplitUrlChunks() method by passing in the results of the BasicTruncateString(url) method. Note that the url being split is the newly scanned url
        ///     create a new tuple:
        ///         Item1 = potential url
        ///         Item2 = the number of matches the allUrlChunks[] entries were seen in the url
        /// return whatever AdvancedScanMatchedUrls finds
        /// </summary>
        /// <param name="newUrlSiteMap"></param>
        internal bool ReverseAdvancedUrlFinder(List<string> newUrlSiteMap)
        {
            resetMatchedUrls();
            var tupleList = new List<Tuple<string, int, int>>();
            foreach (var url in newUrlSiteMap)
            {
                string temp = urlUtil.UrlResourceDir;
                string[] allNewUrlChunks = urlUtil.SplitUrlChunks(url);
                string[] newUrlResourceChunks = urlUtil.SplitUrlChunks(urlUtil.BasicTruncateString(url));

                if (temp.Contains(newUrlResourceChunks[0]) && CheckParentAndResourceDirs(urlUtil, url))
                {
                    if (!tupleList.Exists((Tuple<string, int, int> i) => i.Item1 == url))
                        tupleList.Add(new Tuple<string, int, int>(url, urlUtil.ReturnUrlMatches(urlUtil.OriginalUrl, allNewUrlChunks), urlUtil.ReturnUrlMatches(urlUtil.OriginalUrl, newUrlResourceChunks) * 2));
                }
            }
            return AdvancedScanMatchedUrls(tupleList);
        }

        /// <summary>
        /// Set initial int a (total matches in url) and b (double the total matches found in url resource dir) to 1
        /// for each item in the tuple list:
        /// if either the tuple's total url matches (Item2) are less than a OR total resource dir matches (Item3) are less than b, skip
        /// else if both Item2 > a AND Item3 > b, this is a much better match than all other previously found potential urls
        ///     call resetMatchedUrls() function
        ///     add the tuple's url (Item1) to the newly emptied matchedUrls list.
        ///     set a to the tuple's Item2 and b to the tuple's Item3
        /// else just add the tuple's Item1 to the matchedUrls
        /// check if the matchedUrls Count == 1. If true, url is found, setNewUrl, and return true
        ///     else, return false
        /// </summary>
        /// <param name="tupleList"></param>
        /// <returns></returns>
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

        internal bool UrlChunkFinder(List<string> newUrlSiteMap)
        {
            resetMatchedUrls();
            foreach (var url in newUrlSiteMap)
            {
                string temp = urlUtil.BasicTruncateString(url);
                string[] originalUrlResourceChunks = urlUtil.ReturnUrlResourceChunks();

                if (temp.Contains(originalUrlResourceChunks[0]) && CheckParentAndResourceDirs(urlUtil, url))
                    AddMatchedUrl(url);
            }
            return ScanMatchedUrlsByChunk();
        }

        /// <summary>
        /// Create two temporary lists.
        /// build string temp out of chunks from original url. for every iteration of the for loop, add a new chunk to the end of the url
        /// check if chunk is contained in each of the new urls. If it is, keep. If not, remove from passive list and run RemoveFromMatchedUrls method
        /// if Count == 0, obviously no matching url wasn't found. Return false
        /// if Count == 1, a single match has been found. Return true
        /// if Count > 1, run the for loop again to build a new substring of originalUrl
        /// </summary>
        private bool ScanMatchedUrlsByChunk()
        {
            List<string> activeList = new List<string>();
            List<string> passiveList = matchedUrls.ToList();

            for (int i = 0; i < urlUtil.ReturnUrlResourceChunkLength(); i++)
            {
                activeList.Clear();
                activeList = passiveList.ToList();
                string temp = urlUtil.BuildChunk(i);
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
                    return CheckFinalUrlChunk(i);
            }
            return AdvancedUrlFinder(passiveList);
        }

        private bool CheckFinalUrlChunk(int i)
        {
            if (i < urlUtil.GetChunkLength())
            {
                string temp = urlUtil.BuildChunk(i + 1);
                return (matchedUrls.First().Contains(temp)) ? SetNewUrl() : false;
            }
            else return true;
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
                return (temp.Contains(urlUtil.UrlParentDir));
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
    }
}