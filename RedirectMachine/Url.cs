using System.Collections.Generic;

namespace RedirectMachine
{
    public class UrlDto
    {
        public string OriginalUrl { get; set; }
        public string RemappedParentDir { get; set; }
        public string SanitizedUrl { get; set; }
        public string NewUrl { get; set; }
        public int Count { get; set; }
        public string Flag { get; set; } = "no match";
        public string UrlParentDir { get; set; }
        public string UrlResourceDir { get; set; }
        public bool Score { get; set; }
        public bool StartsWithSlash { get; set; }
        public bool EndsWithSlash { get; set; }
        public string[] UrlResourceDirChunks { get; set; }
        public string[] UrlAllChunks { get; set; }
        public List<string> matchedUrls = new List<string>();

        public UrlDto(string url)
        {
            OriginalUrl = url;
        }

    }
}