using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;

namespace VisualME7.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ME7LoggerDirectory;
        public static bool DebugOutput;

        public App()
        {
            ME7LoggerDirectory = System.IO.Path.GetDirectoryName(Environment.CurrentDirectory);
            //if (args.Length > 0)
            //{
            //    foreach (string arg in args)
            //    {
            //        if (arg == "-DebugOutput")
            //        {
            //            DebugOutput = true;
            //        }
            //        else if (arg == "-Debug")
            //        {
            //            Debug = true;
            //        }
            //        else
            //        {
            //            ME7LoggerDirectory = arg;
            //        }
            //    }
            //}
        }

        static object LockObj = new object();
        public static bool Debug = false;
        public static void WriteDebug(string line)
        {
            if (Debug)
            {
                lock (LockObj)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(ME7LoggerDirectory, "DEBUG.TXT"), true))
                    {
                        sw.WriteLine("{0:H:mm:ss.ffff}: {1}", DateTime.Now, line);
                    }
                }
            }
        }
    }
}
