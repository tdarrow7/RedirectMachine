using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class RedirectUrl
    {
        //private string originalUrl, head, tail, newUrl, sanitizedUrl;
        private int count;
        private bool score;
        public List<string> matchedUrls;
        public string[] urlChunks;
        internal UrlUtils obj;

        public bool Score { get; set; } = false;
        public int Count { get; set; }

        public RedirectUrl()
        {
            // default contstructor
        }

        public RedirectUrl(string url, string[,] urlHeaderMaps)
        {
            obj = new UrlUtils(url);

            // create working constructor
            //originalUrl = obj.GetOriginalUrl();
            //tail = obj.GetTail();
            //head = obj.GetHead();
            //sanitizedUrl = obj.GetSanitizedUrl();
            //score = 0;
            //matchedUrls = new List<string>();
            //urlChunks = tail.Split("-").ToArray();
            CheckUrlHeaderMaps(urlHeaderMaps);
            //var urlObject = obj;
        }

        /// <summary>
        /// check to see if the url from obj contains one of the urlHeaderMap entries.
        /// e.g. www.google.com redirecting to /google/ on the new site
        /// </summary>
        /// <param name="obj"></param>
        private void CheckUrlHeaderMaps(string[,] urlHeaderMaps)
        {
            for (int i = 0; i < urlHeaderMaps.Length; i++)
            {
                
                if (obj.SanitizedUrl.Contains(urlHeaderMaps[i, 0]))
                {
                    obj.HasHeaderMap = true;
                    obj.urlHeaderMap[0] = urlHeaderMaps[i, 0];
                    obj.urlHeaderMap[1] = urlHeaderMaps[i, 1];
                }
            }
        }

        public void CheckUrl(string url)
        {
            string temp = TruncateString(url, 48);
            if (temp.Contains(obj.UrlTail))
            {
                AddMatchedUrl(url);
            }
        }

        public void AdvCheckUrl(string url)
        {
            string temp = TruncateString(url, 48);
            if (temp.Contains(urlChunks[0]))
            {
                AddMatchedUrl(url);
            }
        }

        /// <summary>
        /// Basic scan of urls
        /// </summary>
        /// <returns></returns>
        public bool ScanMatchedUrls()
        {
            if (count == 0)
                return false;

            else if (count == 1)
            {
                obj.NewUrl = matchedUrls.First();
                Score = true;
                return true;
            }
            else
            {
                count = 0;
                List<string> list1 = matchedUrls.ToList();
                foreach (var url in list1)
                {
                    string temp = TruncateStringHead(url);
                    if (obj.HasHeaderMap)
                    {
                        if (!obj.urlHeaderMap[1].Contains(temp))
                        {
                            matchedUrls.Remove(temp);
                            count--;
                        }
                    }
                    else
                    {
                        if (!temp.Contains(obj.UrlHead))
                        {
                            matchedUrls.Remove(temp);
                            count--;
                        }
                    }
                }
                if (count == 1)
                {
                    obj.NewUrl = matchedUrls.First();
                    Score = true;
                    return true;
                }
            }
            return false;
        }

        internal bool AdvancedUrlFinder(List<string> newUrlSiteMap)
        {
            throw new NotImplementedException();
        }

        internal bool BasicUrlFinder(List<string> newUrlSiteMap)
        {
            foreach (var url in newUrlSiteMap)
            {
                string temp = TruncateString(url, 48);
                if (temp.Contains(obj.UrlTail))
                {
                    AddMatchedUrl(url);
                    count++;
                }
            }
            return BasicScan();
        }

        /// <summary>
        /// basic scan of matched urls
        /// if no urls were found, return false to report none were found
        /// if exactly one match is found, return true to report a match was found
        /// </summary>
        private bool BasicScan()
        {
            if (count == 0)
                return false;
            else if (count == 1)
            {
                SetNewUrl();
                return true;
            }
            else
            {
                count = 0;
                List<string> list1 = matchedUrls.ToList();
                foreach (var url in list1)
                {
                    string temp = TruncateStringHead(url);
                    if (!BasicCheckUrlParentDir(temp))
                        RemoveFromMatchedUrls(url);
                }
                if (count == 1)
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
            int c = --Count;
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
            {
                if (!UrlHeaderMatch(temp))
                    return false;
            }
            else
            {
                if (!temp.Contains(obj.UrlHead))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AdvancedScan()
        {
            count = matchedUrls.Count;
            List<string> activeList = new List<string>();
            List<string> passiveList = new List<string>();

            passiveList = matchedUrls.ToList();
            for (int i = 0; i < urlChunks.Length; i++)
            {
                activeList.Clear();
                activeList = passiveList.ToList();
                string temp = BuildChunk(i);
                foreach (var url in activeList)
                {
                    if (!url.Contains(temp))
                    {
                        passiveList.Remove(url);
                        RemoveFromMatchedUrls(url);
                    }
                }
                if (count == 0)
                    return false;

                if (count == 1)
                {
                    SetNewUrl();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return truncated string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string TruncateString(string value)
        {
            // Purpose of method: retrieve usable/searchable end of url from variable value.
            // Get url text after last slash in url
            string temp = CheckVars(value);
            int index = value.Length;
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            return temp;
        }

        /// <summary>
        /// retrieve usable/searchable end of url from variable value.
        /// Get url text after last slash in url,
        /// truncate temporary value to maxLength
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public string TruncateString(string value, int maxLength)
        {
            string temp = CheckVars(value);
            int index = temp.Length;
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        /// <summary>
        /// Purpose: return first chunk of url.
        /// Check if url starts with http or https.If it does, grab entire domain of url
        /// if that doesn't exist, return the first chunk of the url in between the first two ' / '
        /// check to see if the resource currently being looked at is the parent directory. If it is, set isParentDir to true
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string TruncateStringHead(string value)
        {
            string temp = value;
            if (temp.StartsWith("/"))
                temp = temp.Substring(1);
            int index = temp.IndexOf("/");
            if (index <= -1)
                index = temp.Length;
            if (obj.UrlTail.Contains(temp))
                obj.IsParentDir = true;
            temp = temp.Substring(0, index).ToLower();
            return temp;
        }

        /// <summary>
        /// remove unnecessary contents on end of url if found
        /// </summary>
        /// <param name="value"></param>
        public string CheckVars(string value)
        {
            if (value.Contains("?"))
                value = GetSubString(value, "?", false);
            if (value.Contains("."))
                value = GetSubString(value, ".", false);
            if (value.EndsWith("/"))
                value = GetSubString(value, "/", false);
            if (value.EndsWith("/*"))
                value = GetSubString(value, "/*", false);
            if (value.EndsWith("-"))
                value = GetSubString(value, "-", false);
            value = Regex.Replace(value, "--", "-");
            value = Regex.Replace(value, "---", "-");
            value = Regex.Replace(value, "dont", "don-t");
            value = Regex.Replace(value, "cant", "can-t");
            return value;
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
            count++;
        }

        /// <summary>
        /// return a string build from a series of chunks from the working url
        /// </summary>
        /// <param name="index"></param>
        public string BuildChunk(int index)
        {
            string temp = urlChunks[0];
            for (int i = 1; i < index; i++)
            {
                temp = temp + "-" + urlChunks[i];
            }
            return temp;
        }
    }
}