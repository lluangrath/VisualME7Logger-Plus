﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace VisualME7Logger
{
    static class Program
    {
        public static string ME7LoggerDirectory;
        public static bool DebugOutput;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ME7LoggerDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg == "-DebugOutput")
                    {
                        DebugOutput = true;
                    }
                    else if (arg == "-Debug")
                    {
                        Debug = true;
                    }
                    else
                    {
                        ME7LoggerDirectory = arg;
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SettingsForm());
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
