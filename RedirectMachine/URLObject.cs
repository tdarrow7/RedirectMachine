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
                    
                }
            }

                return false;
        }
    }


}
