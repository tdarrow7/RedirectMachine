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
        public string Flag { get; set; } = "no match";

        public RedirectUrl()
        {
        }


        /// <summary>
        /// Working constructor. Nearlky all properties are housed in teh UrlUtils obj.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlHeaderMaps"></param>
        public RedirectUrl(string originalUrl, string[,] urlHeaderMaps)
        {
            urlUtil = new UrlUtils(originalUrl);
            matchedUrls = new List<string>();
            CheckUrlHeaderMaps(urlUtil, urlHeaderMaps);
        }

        /// <summary>
        /// check to see if the url from obj contains one of the urlHeaderMap entries.
        /// e.g. www.google.com redirecting to /google/ on the new site
        /// </summary>
        /// <param name="urlUtilsObject"></param>
        private void CheckUrlHeaderMaps(UrlUtils urlUtilsObject, string[,] urlHeaderMaps)
        {
            for (int i = 0; i < urlHeaderMaps.GetLength(0); i++)
            {
                if (urlUtilsObject.SanitizedUrl.Contains(urlHeaderMaps[i, 0]))
                {
                    urlUtilsObject.HasHeaderMap = true;
                    urlUtilsObject.SetUrlHeaderMap(urlHeaderMaps[i, 0], urlHeaderMaps[i, 1]);
                }
            }
        }

        /// <summary>
        /// scan every url in newUrlSiteMap list. if the resource directory of the url contains the object's UrlTail, add it to matched urls
        /// return the results of BasicScanMatchedUrls()
        /// set the flag to a value of 1
        /// </summary>
        /// <param name="newUrlSiteMap"></param>
        internal bool BasicUrlFinder(List<Tuple<string, string>> newUrlSiteMap)
        {
            SetFlag(1);
            foreach (var url in newUrlSiteMap)
            {
                string resource = url.Item2;
                if (resource.Contains(urlUtil.UrlResourceDir) && CheckParentAndResourceDirs(urlUtil, url.Item1))
                    AddMatchedUrl(url.Item1);
            }
            return BasicScanMatchedUrls();
        }

        /// <summary>
        /// this method simply catches all urls that can immediately be removed from being evaluated.
        /// If there is a header map, does the url start with that header map? if yes, return true. If no, return false
        /// Else, if there's no header map, does the url's parent directory match the old url's parent directory? return either true or false
        /// </summary>
        /// <param name="urlUtilsObject"></param>
        /// <param name="url"></param>
        private bool CheckParentAndResourceDirs(UrlUtils urlUtilsObject, string url)
        {
            if (!CheckResourceDirs(urlUtilsObject, url))
                return false;
            if (urlUtilsObject.HasHeaderMap)
                return url.StartsWith(urlUtilsObject.urlHeaderMap[1]);
            else if (urlUtilsObject.IsParentDir)
                return url.StartsWith(urlUtilsObject.UrlParentDir);
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
        internal bool AdvancedUrlFinder(List<Tuple<string, string>> newUrlSiteMap)
        {
            SetFlag(2);
            resetMatchedUrls();
            var tupleList = new List<Tuple<string, int, int>>();
            foreach (var url in newUrlSiteMap)
            {
                string resource = url.Item2;
                string[] allOriginalUrlChunks = urlUtil.ReturnAllUrlChunks();
                string[] originalUrlResourceChunks = urlUtil.ReturnUrlResourceChunks();
                if (UrlMatchAnyChunks(url.Item2, originalUrlResourceChunks) && CheckParentAndResourceDirs(urlUtil, url.Item1))
                {
                    if (!tupleList.Exists((Tuple<string, int, int> i) => i.Item1 == url.Item1))
                        tupleList.Add(new Tuple<string, int, int>(url.Item1, urlUtil.ReturnUrlMatches(url.Item1, allOriginalUrlChunks), urlUtil.ReturnUrlMatches(url.Item2, originalUrlResourceChunks) * 2));
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
        internal bool ReverseAdvancedUrlFinder(List<Tuple<string, string>> newUrlSiteMap)
        {
            SetFlag(3);
            resetMatchedUrls();
            var tupleList = new List<Tuple<string, int, int>>();
            foreach (var url in newUrlSiteMap)
            {
                string resource = urlUtil.UrlResourceDir;
                string[] allNewUrlChunks = urlUtil.SplitUrlChunks(url.Item1);
                string[] newUrlResourceChunks = urlUtil.SplitUrlChunks(url.Item2);

                if (UrlMatchAnyChunks(resource, newUrlResourceChunks) && CheckParentAndResourceDirs(urlUtil, url.Item1))
                {
                    if (!tupleList.Exists((Tuple<string, int, int> i) => i.Item1 == url.Item1))
                        tupleList.Add(new Tuple<string, int, int>(url.Item1, urlUtil.ReturnUrlMatches(urlUtil.OriginalUrl, allNewUrlChunks), urlUtil.ReturnUrlMatches(urlUtil.OriginalUrl, newUrlResourceChunks) * 2));
                }
            }
            return AdvancedScanMatchedUrls(tupleList);
        }

        private bool UrlMatchAnyChunks(string url, string[] chunks)
        {
            foreach (var chunk in chunks)
            {
                if (url.Contains(chunk))
                    return true;
            }
            return false;
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


        /// <summary>
        /// reset matchedUrls list to zero
        /// scan each url in the list of urls passed into the function
        /// Truncate the url and return the resource directory and assign that string to string temp
        /// if first chunk in the resource directory of the original url in question is contained in temp and the url passes the CheckParentAndResourceDirs() audit, add as a potential matched url
        /// return the ScanMatchedUrlsByChunk bool
        /// </summary>
        /// <param name="newUrlSiteMap"></param>
        /// <returns></returns>
        internal bool UrlChunkFinder(List<Tuple<string, string>> newUrlSiteMap)
        {
            SetFlag(4);
            resetMatchedUrls();
            List<Tuple<string, string>> possibleMatchList = new List<Tuple<string, string>>();
            foreach (var url in newUrlSiteMap)
            {
                string temp = url.Item2;
                string[] originalUrlResourceChunks = urlUtil.ReturnUrlResourceChunks();

                if (temp.Contains(originalUrlResourceChunks[0]) && CheckParentAndResourceDirs(urlUtil, url.Item1))
                    possibleMatchList.Add(new Tuple<string, string>(url.Item1, url.Item2));
                    //AddMatchedUrl(url.Item1);
            }
            return ScanMatchedUrlsByChunk(possibleMatchList);
        }

        /// <summary>
        /// Create two temporary lists.
        /// build string temp out of chunks from original url. for every iteration of the for loop, add a new chunk to the end of the url
        /// check if chunk is contained in each of the new urls. If it is, keep. If not, remove from passive list and run RemoveFromMatchedUrls method
        /// if Count == 0, obviously no matching url wasn't found. Return false
        /// if Count == 1, a single match has been found. Return true
        /// if Count > 1, run the for loop again to build a new substring of originalUrl
        /// once the entire resource dir has been built from the chunks and there are still more than one matched url, run the passive list through the AdvancedUrlFinder and return its result
        /// </summary>
        private bool ScanMatchedUrlsByChunk(List<Tuple<string, string>> possibleMatchList)
        {
            List<string> activeList = new List<string>();
            //List<string> passiveList = matchedUrls.ToList();

            for (int i = 0; i < urlUtil.ReturnUrlResourceChunkLength(); i++)
            {
                activeList.Clear();
                foreach (var tuple in possibleMatchList)
                {
                    activeList.Add(tuple.Item1);
                    AddMatchedUrl(tuple.Item1);
                }
                //activeList = possibleMatchList.ToList();
                string temp = urlUtil.BuildChunk(i);
                foreach (var url in activeList)
                {
                    if (!url.Contains(temp))
                    {
                        possibleMatchList.RemoveAll(item => item.Item1 == url);
                        RemoveFromMatchedUrls(url);
                    }
                }
                if (Count == 0)
                    return false;
                if (Count == 1)
                    return CheckFinalUrlChunk(i);
            }
            return AdvancedUrlFinder(possibleMatchList);
        }

        /// <summary>
        /// if at least one more larger substring can be built from the chunks of the original url, build that substring
        ///     check if that substring is contained in the last remaining matched url in the list
        ///     if contained, return true. Else, return false
        /// else, by default return true
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
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

        private void SetFlag(int i)
        {
            string[] flags = { "Great Match", "Good Match", "Decent Match", "Please Check Me" };
            if (Flag != "Please Check Me")
                Flag = flags[i];
        }
    }
}