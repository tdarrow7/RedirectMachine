using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedirectMachine
{
    class URLObject
    {
        private string originalUrl, urlSub, newUrl;
        private int score, count;
        public List<string> matchedUrls;
        
        public URLObject()
        {
            // default constructor
            score = 0;
            matchedUrls = new List<string>();
        }

        
        public URLObject(string originalUrl, string urlSub)
        {
            // create working constructor
            this.originalUrl = originalUrl;
            this.urlSub = urlSub;
            score = 0;
            matchedUrls = new List<string>();
            
        }

        
        public string GetOriginalUrl()
        {
            // Purpose: return private string originalUrl
            return originalUrl;
        }

        public string GetUrlSub()
        {
            // Purpose: return private stirng urlSub
            return urlSub;
        }

        
        public int GetScore()
        {
            // Purpose: return private int score
            return score;
        }

        public void AddScore()
        {
            // Purpose of method: add score
            score++;
        }

        
        public void SubtractScore()
        {
            // Purpose of method: subtract score
            score--;
        }

        public void AddMatchedUrl(string link)
        {
            // Purpose: add potential url match
            matchedUrls.Add(link);
            count++;
        }

        public void RemoveMatchedUrl(string link)
        {
            matchedUrls.Remove(link);
            count--;
        }

        public bool ScanMatchedUrls(string l)
        {
            if (count == 1)
            {
                newUrl = matchedUrls.First();
                return true;
            }
            else
            {
                foreach (var url in matchedUrls)
                {
                    string temp = TruncateString(url);
                }
            }

                return false;
        }

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

        public string TruncateString(string value, int maxLength)
        {
            // Purpose of method: retrieve usable/searchable end of url from variable value.
            
            // Get url text after last slash in url,
            // truncate temporary value to maxLength
            string temp = CheckVars(value);
            int index = value.Length;
            int pos = temp.LastIndexOf("/") + 1;
            temp = temp.Substring(pos, temp.Length - pos);
            if (string.IsNullOrEmpty(temp)) return temp;
            return temp.Length <= maxLength ? temp : temp.Substring(0, maxLength);
        }

        public string CheckVars(string value)
        {
            // Purpose: remove unnecessary contents on end of url if found
            if (value.EndsWith("/"))
                value = GetSubString(value, "/", false);
            else if (value.EndsWith("/*"))
                value = GetSubString(value, "/*", false);
            else if (value.EndsWith("-"))
                value = GetSubString(value, "-", false);
            else if (value.Contains("."))
                value = GetSubString(value, ".", false);
            return value;
        }

        public static int GetIndex(string i, string j)
        {
            // Purpose of method: return position of j variable in string i.
            return i.LastIndexOf(j);
        }

        public static string GetSubString(string i, string j, bool x)
        {
            // Purpose of method: return the substring of the string that is passed into this function.
            // This method is overloaded with a bool. The bool indicates to the function that it must return a substring
            // 1) if true, includes the string j rather than excluding it, or
            // 2) if false, returns a substring that excludes string j.
            int index = GetIndex(i, j);
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
                int index = GetIndex(i, j);
                temp = temp.Substring(0, index);
                pos++;
            }
            return temp;
        }
    }


}
