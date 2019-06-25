using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RedirectMachine
{
    internal class RedirectFinder
    {
        // declare all universally needed variables 
        public List<CatchAllUtils> catchalls = new List<CatchAllUtils>();
        public static List<Tuple<string, string>> newUrlSiteMap = new List<Tuple<string, string>>();
        //public static List<RedirectUrl> redirectUrls = new List<RedirectUrl>();
        public static List<UrlDto> urlDtos = new List<UrlDto>();
        public static CatchAllUtils catchAllUtilObject = new CatchAllUtils();
        UrlUtils utils = new UrlUtils();
        RedirectUrl redirectUrlUtils = new RedirectUrl();
        List<string> lostList = new List<string>();
        List<string> foundList = new List<string>();
        HashSet<string> existingRedirects = new HashSet<string>();

        public int FoundCount = 0;
        public int LostCount = 0;

        public static string[,] urlHeaderMaps = {
            { "https://www.google.com", "/googleness/" }
        };

        string osUrlFile = @"c:\users\timothy.darrow\source\repos\redirectmachine\OldSiteUrls.csv";
        //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
        string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
        //string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestNewSiteUrls.csv";
        string osCatchAllUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteCatchAlls.csv";
        string existingRedirectsFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\ExistingRedirects.csv";
        string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
        string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
        string catchAllFile = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";

        //string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldSiteUrls.csv";
        ////string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\TestBatch.csv";
        //string nsUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\NewSiteUrls.csv";
        ////string nsUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\TestNewSiteUrls.csv";
        //string osCatchAllUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldSiteCatchAlls.csv";
        //string existingRedirectsFile = @"C:\Users\timot\source\repos\RedirectMachine\ExistingRedirects.csv";
        //string lostUrlFile = @"C:\Users\timot\Downloads\LostUrls.csv";
        //string foundUrlFile = @"C:\Users\timot\Downloads\FoundUrls.csv";
        //string catchAllFile = @"C:\Users\timot\Downloads\Probabilities.csv";


        /// <summary>
        /// default working constructor
        /// </summary>
        public RedirectFinder()
        {
        }

        /// <summary>
        /// Start the finder program
        /// Initialize stopwatch.
        /// Import both the old urls and new urls into lists
        /// compare the new urls to the old urs using FindUrlMatches
        /// Export all found catchalls to a CSV file
        /// Export the lost url list and found url list to their respective CSVs
        /// Stop and display the recorded time on the stopwatch.
        /// </summary>
        internal void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("begin search: ");
            ImportExistingRedirects(existingRedirectsFile);
            ImportNewUrlsIntoList(nsUrlFile);
            ImportOldUrlsIntoList(osUrlFile);
            catchAllUtilObject.GenerateCatchAllParams(osCatchAllUrlFile);
            //FindUrlMatches(redirectUrls);
            FindUrlMatches(urlDtos);
            //StartThreads();
            catchAllUtilObject.ExportCatchAllsToCSV(catchAllFile);
            ExportNewCSVs();
            Console.WriteLine("end of exports");
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine($"Run time: {elapsedTime}");
        }

        

        /// <summary>
        /// Add CSV file contents to list
        /// Sort results of list alphabetically
        /// </summary>
        /// <param name="urlFile"></param>
        private void ImportNewUrlsIntoList(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string[] tempArray = reader.ReadLine().ToLower().Split(",");
                    newUrlSiteMap.Add(new Tuple<string, string>(tempArray[0], "/" + tempArray[1] + "/"));
                }
            }
        }

        /// <summary>
        /// For Every line in CSV, read line and check if line belongs in a catchAll. If not, create new RedirectUrl Object.
        /// </summary>
        /// <param name="urlFile"></param>
        private void ImportOldUrlsIntoList(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string url = reader.ReadLine().ToLower();
                    if (!catchAllUtilObject.CheckExistingCatchallParams(url) && !existingRedirects.Contains(url))
                        //redirectUrls.Add(new RedirectUrl(url, urlHeaderMaps));
                        urlDtos.Add(createUrlDto(url));
                }
            }
        }


        /// <summary>
        /// return a UrlDto object with all the props from UrlUtils class
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private UrlDto createUrlDto(string url)
        {
            UrlDto urlDto = new UrlDto(url);
            urlDto.UrlParentDir = utils.TruncateStringHead(url);
            urlDto.UrlResourceDir = utils.TruncateString(url, 48);
            urlDto.UrlResourceDirChunks = utils.ReturnUrlChunks(urlDto.UrlResourceDir);
            urlDto.UrlAllChunks = utils.ReturnUrlChunks(url);
            urlDto.RemappedParentDir = utils.ReturnRemappedUrlParentDir(url, urlHeaderMaps);
            return urlDto;
        }

        private void ImportExistingRedirects(string urlFile)
        {
            using (var reader = new StreamReader(@"" + urlFile))
            {
                while (!reader.EndOfStream)
                {
                    string url = reader.ReadLine().ToLower();
                    existingRedirects.Add(url);
                }
            }
        }

        /// <summary>
        /// check every item in List<RedirectUrl> redirectUrls and compare with items in List<> newUrlSiteMap.
        ///     Try the BasicUrlFinder() method
        ///     Try the AdvancedUrlFinder() method
        ///     Try the ReverseAdvancedUrlFinder() method
        ///     Try the UrlChunkFinder() method
        ///     If a match still wasn't found, add to catchalls
        /// </summary>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        public void FindUrlMatches(List<UrlDto> chunkOfRedirects)
        {
            foreach (var urlDto in chunkOfRedirects)
            {
                //if (oldUrl.BasicUrlFinder(newUrlSiteMap) || oldUrl.AdvancedUrlFinder(newUrlSiteMap) || oldUrl.ReverseAdvancedUrlFinder(newUrlSiteMap) || oldUrl.UrlChunkFinder(newUrlSiteMap)) { }
                //else
                if (!redirectUrlUtils.findMatching301(urlDto, newUrlSiteMap))
                {
                    urlDto.Flag = "no match";
                    if (!urlDto.OriginalUrl.Contains("."))
                    {
                        catchAllUtilObject.checkIfCatchAllIsCreated(urlDto.OriginalUrl);
                        catchAllUtilObject.CatchAllCount++;
                    }

                }
            }
        }
         
        //public void StartThreads()
        //{
        //    int count = 1000,
        //        i = redirectUrls.Count / count,
        //        j = 0,
        //        modulus = redirectUrls.Count % count,
        //        length;
        //    List<RedirectUrl> chunkOfRedirects;
        //    Thread[] threads = new Thread[i];
        //    for (int k = 0; k < threads.Length; k++)
        //    {
        //        length = (k == threads.Length - 1) ? count + modulus : count;
        //        chunkOfRedirects = GetChunkOfRedirects(j, length);
        //        Console.WriteLine($"j: {j}");
        //        Console.WriteLine($"length: {length}");
        //        threads[k] = new Thread(() => this.FindUrlMatches(chunkOfRedirects));
        //        j += length;
        //    }
        //    for (int k = 0; k < threads.Length; k++)
        //    {
        //        threads[k].Start();
        //    }

        // }

        //internal List<RedirectUrl> GetChunkOfRedirects(int index, int length)
        //{
        //    List<RedirectUrl> result = redirectUrls.GetRange(index, length);
        //    return result;
        //}

        /// <summary>
        /// Scan all objects in redirectUrls list and put them in either the foundList or lostList, depending on their score
        /// Send both temporary lists to buildCSV method to print both found and lost lists
        /// </summary>
        internal void ExportNewCSVs()
        {
            List<string> foundList = new List<string>();
            List<string> lostList = new List<string>();

            foundList.Add("Old Site Url,Redirected Url,Flag");
            lostList.Add("Old Site Url, Potential Redirected Url");
            foreach (var urlDto in urlDtos)
            {
                if (urlDto.Score == true)
                {
                    FoundCount++;
                    foundList.Add($"{urlDto.OriginalUrl},{urlDto.NewUrl}, {urlDto.Flag}");
                }
                    
                else
                {
                    LostCount++;
                    if (urlDto.matchedUrls.Count > 0)
                    {
                        
                        string[] arrayOfMatches = urlDto.matchedUrls.ToArray();
                        for (int i = 0; i < arrayOfMatches.Length; i++)
                        {
                            if (i == 0)
                                lostList.Add($"{urlDto.OriginalUrl},{arrayOfMatches[i]}");
                            else
                                lostList.Add($",{arrayOfMatches[i]}");
                        }
                    }
                    else
                        lostList.Add($"{urlDto.OriginalUrl}");
                }
            }
            ExportToCSV(foundList, foundUrlFile);
            ExportToCSV(lostList, lostUrlFile);
            Console.WriteLine($"number of found urls: {FoundCount}");
            Console.WriteLine($"number of lost urls: {LostCount}");
        }

        /// <summary>
        /// build CSV from specified list of strings and export to specified filePath
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filePath"></param>
        internal void ExportToCSV(List<string> list, string filePath)
        {
            using (TextWriter tw = new StreamWriter(@"" + filePath))
            {
                foreach (var item in list)
                {
                    tw.WriteLine(item);
                }
            }
        }
    }
} 