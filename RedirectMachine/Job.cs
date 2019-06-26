using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedirectMachine
{
    internal class Job
    {
        public string Directory { get; set; }
        public string OldSiteUrlFilePath { get; set; }
        public string NewSiteUrlFilePath { get; set; }
        public string ExistingRedirectFilePath { get; set; }
        public string LoggerFile { get; set; }
        public string OutputFolder { get; set; }
        public string OutputFoundRedirects { get; set; }
        public string OutputLostRedirects { get; set; }
        public string OutputCatchalls { get; set; }
        //public List<string> LogEntries = new List<string>();
        //public CatchAllUtils catchAllUtilObject = new CatchAllUtils();
        //public List<Tuple<string, string>> newUrlSiteMap = new List<Tuple<string, string>>();
        //public List<UrlDto> urlDtos = new List<UrlDto>();

        public Job(string directory)
        {
            Directory = directory;
            OldSiteUrlFilePath = System.IO.Directory.GetFiles(Directory, "*OldSiteUrls.csv")[0];
            NewSiteUrlFilePath = System.IO.Directory.GetFiles(Directory, "*NewSiteUrls.csv")[0];
            ExistingRedirectFilePath = System.IO.Directory.GetFiles(Directory, "*ExistingRedirects.csv")[0];
            LoggerFile = Path.Combine(Directory, @"log.txt");
            OutputFolder = Path.Combine(Directory, @"Output");
            OutputFoundRedirects = Path.Combine(OutputFolder, @"FoundRedirects.csv");
            OutputLostRedirects = Path.Combine(OutputFolder, @"LostRedirects.csv");
            OutputCatchalls = Path.Combine(OutputFolder, @"PossibleCatchalls.csv");
        }

        public void Start()
        {
            //buildLoggerFile();
            //LogEntries.Add($"Beginning of log for {Path.GetDirectoryName(Directory)}");
            //catchAllUtilObject.GenerateCatchAllParams(ExistingRedirectFilePath);
            //ImportNewUrlsIntoList(NewSiteUrlFilePath);
            //DirectoryInfo dr = System.IO.DirectoryInfo.CreateDirectory(OutputFolder);
            System.IO.Directory.CreateDirectory(@"" + OutputFolder);
            Console.WriteLine("starting");
            startRedirectFinder();
        }

        private void startRedirectFinder()
        {
            RedirectFinder redirectFinder = new RedirectFinder();
            redirectFinder.osUrlFile = OldSiteUrlFilePath;
            redirectFinder.nsUrlFile = NewSiteUrlFilePath;
            redirectFinder.existingRedirectsFile = ExistingRedirectFilePath;
            redirectFinder.catchAllFile = OutputCatchalls;
            redirectFinder.foundUrlFile = OutputFoundRedirects;
            redirectFinder.lostUrlFile = OutputLostRedirects;
            redirectFinder.Run();
        }

        private void buildLoggerFile()
        {
            try
            {

                // Delete the file if it exists.
                if (File.Exists(LoggerFile))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    File.Delete(LoggerFile);
                }


                // Create the file.
                File.Create(LoggerFile);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            
        }

        private void writeToLog(string v)
        {
            using (StreamWriter fs = new StreamWriter(LoggerFile))
            {
                // Add some information to the file.
                fs.WriteLine(v);
            }
        }

        /// <summary>
        /// Add CSV file contents to list
        /// Sort results of list alphabetically
        /// </summary>
        /// <param name="urlFile"></param>
        //private void ImportNewUrlsIntoList(string urlFile)
        //{
        //    using (var reader = new StreamReader(@"" + urlFile))
        //    {
        //        while (!reader.EndOfStream)
        //        {
        //            string[] tempArray = reader.ReadLine().ToLower().Split(",");
        //            newUrlSiteMap.Add(new Tuple<string, string>(tempArray[0], "/" + tempArray[1] + "/"));
        //        }
        //    }
        //}
    }
}