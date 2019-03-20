using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedirectMachine
{
    public class RedirectUrl
    {
        private string originalUrl, head, tail, newUrl, sanitizedUrl;
        private int score, count;
        public List<string> matchedUrls;
        public string[] urlChunks;
        public string[] urlHeaderMap = new string[2];
        private bool isParentDir = false;
        private bool hasUrlHeaderMap = false;

        public RedirectUrl()
        {
            // default contstructor
        }

        public RedirectUrl(URLObject obj)
        {
            // create working constructor
            originalUrl = obj.GetOriginalUrl();
            tail = obj.GetTail();
            head = obj.GetHead();
            sanitizedUrl = obj.GetSanitizedUrl();
            score = 0;
            matchedUrls = new List<string>();
            urlChunks = tail.Split("-").ToArray();
            CheckUrlHeaderMaps(obj);
        }

        private void CheckUrlHeaderMaps(URLObject obj)
        {
            throw new NotImplementedException();
        }

        public void CheckUrl(string url)
        {
            string temp = TruncateString(url, 48);
            if (temp.Contains(tail))
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
            //Console.WriteLine($"count for {tail} is: {count}");
            if (count == 0)
                return false;

            else if (count == 1)
            {
                newUrl = matchedUrls.First();
                AddScore();
                return true;
            }
            else
            {
                count = 0;
                List<string> list1 = new List<string>();
                foreach (var url in matchedUrls)
                {
                    list1.Add(url);
                }
                foreach (var url in list1)
                {
                    string temp = TruncateStringHead(url);
                    if (hasUrlHeaderMap)
                    {
                        if (!UrlHeaderMatch(temp))
                        {
                            matchedUrls.Remove(temp);
                            count--;
                        }
                    }
                    else
                    {
                        if (!temp.Contains(head))
                        {
                            matchedUrls.Remove(temp);
                            count--;
                        }
                    }
                }
                if (count == 1)
                {
                    newUrl = matchedUrls.First();
                    AddScore();
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
            // check to see if the resource currently being looked at is the parent directory. If it is, set isParentDir to true
            if (tail.Contains(temp))
            {
                isParentDir = true;
            }
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

        public static string GetSubString(string i, string j, bool x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with a bool. The bool indicates to the function that it must return a substring
            // 1) if true, includes the string j rather than excluding it, or
            // 2) if false, returns a substring that excludes string j.
            int index = GetLastIndex(i, j);
            string temp;
            if (x == true)
            {
                temp = i.Substring(0, index + j.Length);
            }
            else
                temp = i.Substring(0, index);
            return temp;
        }

        public static string GetSubString(string i, string j, int x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with an int. The int indicates to the function that it must rerun that many times.
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
        /// Increase score 
        /// </summary>
        public void AddScore()
        {
            // Purpose of method: add score
            score++;
        }

        /// <summary>
        /// returns either true or false depending on whether or not the HeaderMap[1] contains the string temp
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public bool UrlHeaderMatch(string temp)
        {
            return urlHeaderMap[1].Contains(temp);
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
    }
}