using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            var finder = new RedirectFinder();
            finder.Run();
        }
    }
}
