using System;
using System.Collections.Generic;
using System.IO;

namespace RedirectMachine
{
    internal class RedirectJobFinder
    {
        private string root;

        public RedirectJobFinder(string root)
        {
            this.root = root;
        }

        internal void Run()
        {
            List<Job> jobList = setUpJobs();
            Console.WriteLine($"number of jobs: {jobList.Count}");
            startJobs(jobList);
        }

        private void startJobs(List<Job> jobList)
        {
            foreach (var job in jobList)
            {
                job.Start();
            }
        }

        private List<Job> setUpJobs()
        {
            List<Job> jobList = new List<Job>();
            string[] subdirectoryEntries = Directory.GetDirectories(root);
            foreach (string subDirectory in subdirectoryEntries)
            {
                if (checkIfJobIsNeeded(subDirectory))
                {
                    jobList.Add(new Job(subDirectory));
                }
            }
            return jobList;
        }

        private static bool checkIfJobIsNeeded(string directory)
        {
            string[] subDirectories = Directory.GetDirectories(directory);
            return (subDirectories.Length < 1) ? true : false;
        }
    }
}