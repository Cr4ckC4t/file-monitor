using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace FileMonitor
{
    internal class Program
    {
        private static void OnCreated(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Created: {e.FullPath}");

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e) =>
            Console.WriteLine($"Renamed:\n    Old: {e.OldFullPath}\n    New: {e.FullPath}");


        static void MonitorPath(String path)
        {
            var watcher = new FileSystemWatcher(@path);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            while (true) ;
        }

        private static List<Thread> threads = new List<Thread>();

        static int Main()
        {
            Console.WriteLine(">>> Starting File Monitor");

            Console.CancelKeyPress += delegate
            {
                foreach (Thread thread in threads)
                {
                    Console.WriteLine("[+] Stopping monitor for " + thread.Name);
                    thread.Abort();
                }
            };

            List<string> paths = new List<string>
            {
                Path.GetTempPath(),
                // add other paths you wish to monitor here
            };

            foreach (string path in paths)
            {
                Thread t = new Thread(() => MonitorPath(path));
                t.Name = "Monitor: " + path;
                threads.Add(t);
                t.Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine(">>> File Monitor Stopped");
            return 0;
        }
    }
}
