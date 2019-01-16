using System;
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
        public string[] urlChunks;
        
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
            urlChunks = tail.Split("-").ToArray();
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
            return count;
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

        public void ClearMatches()
        {
            matchedUrls.Clear();
        }

        public void CheckUrl(string url)
        {
            string temp = TruncateString(url, 48);
            //Console.WriteLine($"oldUrl: {GetUrlSub()}, newUrl: {temp}");
            if (temp.Contains(tail))
            {
                AddMatchedUrl(url);
            }
        }

        public void AdvCheckUrl(string url)
        {

            
            string temp = TruncateString(url, 48);
            //string chunk = tail.Substring()
            //Console.WriteLine($"oldUrl: {GetUrlSub()}, newUrl: {temp}");
            if (temp.Contains(tail))
            {
                AddMatchedUrl(url);
            }
        }

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
                foreach (var url in matchedUrls)
                {
                    string temp = TruncateStringHead(url);
                    if (!temp.Contains(head))
                    {
                        matchedUrls.Remove(temp);
                        count--;
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

        public bool AdvScanUrls(int index)
        {
            Console.WriteLine("entering AdvScanUrls");
            count = matchedUrls.Count;
            //List<string> list = new List<string>();
            string temp = BuildChunk(index);
            Console.WriteLine($"temp = {temp}");
            Console.WriteLine($"index = {index}");
            Console.WriteLine($"count = {count}");
            Console.WriteLine($"length of urlChunks: {urlChunks.Length}");
            Console.WriteLine($"size of matchedUrls list: {matchedUrls.Count}");
            foreach (var url in matchedUrls)
            {
                Console.WriteLine($"checking if {temp} is in {url}");
                if (!url.Contains(temp))
                {
                    matchedUrls.Remove(url);
                    count--;
                    Console.WriteLine($"removed {url} from matchedUrls");
                }
                else
                {
                    Console.WriteLine($"found {temp} in {url}");
                }
            }
            Console.WriteLine($"matchedUrls.Count is at {matchedUrls.Count}");
            Console.WriteLine($"count is at {count}");
            Console.WriteLine();
            if (count == 0)
            {
                Console.WriteLine("count is at 0. no matches. Returning false");
                return false;
            }
                
            if (count == 1)
            {
                Console.WriteLine("count is at 1. One match. Returning true");
                newUrl = matchedUrls.First();
                return true;
            }
            if (index <= urlChunks.Length)
                Console.WriteLine($"{index} is less than {urlChunks.Length}");
            else
                Console.WriteLine($"{index} is greater than or equal to {urlChunks.Length}");
            return (index <= urlChunks.Length) ? AdvScanUrls(index++) : false;
        }

        public string BuildChunk(int index)
        {
            string temp = chunks[0];
            for (int i = 0; i < index; i++)
            {
                temp = temp + chunks[i];
            }
            return temp;
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
            int index = temp.Length;
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
            
            if (temp.StartsWith("/"))
                temp = temp.Substring(1);
            int index = temp.IndexOf("/");
            if (index <= -1)
                index = temp.Length;
            return temp.Substring(0, index).ToLower();
        }

        public string CheckVars(string value)
        {
            // Purpose: remove unnecessary contents on end of url if found
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
            int index = value.IndexOf("//");
            Console.WriteLine($"Length of value: {value.Length}");
            Console.WriteLine($"index of slashes: {index}");
            string temp = value.Substring(value.IndexOf("//"), value.Length - value.IndexOf("//"));
            temp = temp.Substring(0, GetFirstIndex(temp, "/"));
            return temp.Substring(GetFirstIndex(temp, "."), GetLastIndex(temp, "."));
        }
    }
}
