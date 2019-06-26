using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace RedirectMachine
{
    class Program
    {
        /// <summary>
        /// Let the games begin
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string root = @"S:\S-Z\Timothy Darrow\Redirect Machine";
            RedirectJobFinder jobs = new RedirectJobFinder(root);
            jobs.Run();
        }
    }
}
