using System.Collections.Generic;
using System.Diagnostics;

namespace RedirectMachine
{
    internal class RedirectFinder
    {
        public RedirectFinder()
        {
        }

        internal void Run()
        {
            //initialize paths to files
            //string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\OldSiteUrls.csv";
            string osUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\TestBatch.csv";
            string nsUrlFile = @"C:\Users\timothy.darrow\source\repos\RedirectMachine\NewSiteUrls.csv";
            string lostUrlFile = @"C:\Users\timothy.darrow\Downloads\LostUrls.csv";
            string foundUrlFile = @"C:\Users\timothy.darrow\Downloads\FoundUrls.csv";
            string probabilityDictionary = @"C:\Users\timothy.darrow\Downloads\Probabilities.csv";

            ////string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\OldSiteUrls.csv";
            //string osUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\TestBatch.csv";
            //string nsUrlFile = @"C:\Users\timot\source\repos\RedirectMachine\NewSiteUrls.csv";
            //string lostUrlFile = @"C:\Users\timot\Downloads\LostUrls.csv";
            //string foundUrlFile = @"C:\Users\timot\Downloads\FoundUrls.csv";
            //string probabilityDictionary = @"C:\Users\timot\Downloads\Probabilities.csv";

            // lost and found lists
            List<string> lostList = new List<string>();
            List<string> foundList = new List<string>();

            // Create new CSV Objects
            var oldUrlCSV = new CSVObject();
            var newUrlCSV = new CSVObject();

            // read both old urls and new urls into CSV List
            oldUrlCSV.ReadOldUrlsIntoList(osUrlFile);
            oldUrlCSV.ReadNewUrlsIntoList(nsUrlFile);

            // site url lists
            //static List<string> osUrls = new List<string>();
            //static List<URLObject> osUrls = new List<URLObject>();
            //List<URLObject> osUrls = new List<URLObject>();
            //List<string> nsUrls = new List<string>();
        }
    }
}