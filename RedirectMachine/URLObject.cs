﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedirectMachine
{
    class URLObject
    {
        private string originalUrl, head, tail, newUrl, redirect;
        private int score, count;
        public List<string> matchedUrls;
        
        public URLObject()
        {
            // default constructor
        }

        
        public URLObject(string originalUrl)
        {
            // create working constructor
            this.originalUrl = originalUrl;
            head = TruncateStringHead(originalUrl);
            tail = TruncateString(originalUrl, 48);
            score = 0;
            matchedUrls = new List<string>();
        }

        
        public string GetOriginalUrl()
        {
            // Purpose: return private string originalUrl
            return originalUrl;
        }

        public string GetNewUrl()
        {
            // Purpose: return private string newUrl
            return newUrl;
        }

        public string GetUrlSub()
        {
            // Purpose: return private stirng urlSub
            return tail;
        }

        
        public int GetScore()
        {
            // Purpose: return private int score
            return score;
        }

        public int GetCount()
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

        public void CheckUrl(string url)
        {
            string temp = TruncateString(url, 48);
            Console.WriteLine($"oldUrl: {GetUrlSub()}, newUrl: {temp}");
            if (url.Contains(head))
                AddMatchedUrl(url);
        }

        public bool ScanMatchedUrls()
        {
            
            if (count == 0)
            {
                return false;
            }
                
            else if (count == 1)
            {
                newUrl = matchedUrls.First();
                AddScore();
                return true;
            }
            else
            {
                foreach (var url in matchedUrls)
                {
                    string temp = TruncateStringHead(url);
                    if (!temp.Contains(head))
                        matchedUrls.Remove(temp);
                    count--;
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

        public string TruncateStringHead(string value)
        {
            // Purpose: return first chunk of url. 
            // Check if url starts with http or https. If it does, grab entire domain of url
            // if that doesn't exist, return the first chunk of the url in between the first two '/'
            string temp = value;
            int index = 2;
            if (value.StartsWith("http"))
                temp = TrimFullUrl(value);
            if (temp.StartsWith("/"))
                temp = temp.Substring(1);
            if (temp.EndsWith("/"))
                temp = temp.Substring(0, temp.Length - 1);
            return value.Substring(0, index);
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

        public static int GetFirstIndex(string i, string j)
        {
            // Purpose of method: return first position of j variable in string i.
            if (i.Contains(j))
                return i.IndexOf(j);
            else
                return i.Length;
        }

        public static int GetLastIndex(string i, string j)
        {
            // Purpose of method: return last position of j variable in string i.
            if (i.Contains(j))
                return i.LastIndexOf(j);
            else
                return i.Length;
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

        public string TrimFullUrl(string value)
        {
            int index = GetFirstIndex(value, "//");
            string temp = value.Substring(index, value.Length);
            temp = temp.Substring(0, GetFirstIndex(temp, "/"));
            return temp.Substring(GetFirstIndex(temp, "."), GetLastIndex(temp, "."));
        }

    }

    

}
