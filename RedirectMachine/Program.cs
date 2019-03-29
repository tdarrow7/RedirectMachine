using System;
using System.Diagnostics;

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
