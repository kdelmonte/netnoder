using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NetNoder;
using NetNoder.Models;

namespace TestConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's start a node server from a .NET App");

            // Setup node server
            var nodeServer = new NodeServer(new NodeServerSettings
            {
                WindowStyle = ProcessWindowStyle.Normal,
                EntryPointFilePath = @"C:\code\personal\github\netnoder\TestNodeServer\app.js",
                KillPassword = "MyPasswordToStopServer",
                MonitorInterval = 5000,
                Location = new NodeServerLocation
                {
                    Host = "localhost",
                    Port = 9514,
                    Protocol = "http"
                },
                Data = new Dictionary<string, object>
                {
                    {"TestSetting", "Value"}
                }
            });

            // Start it up
            var success = nodeServer.Start(); // Or .Monitor() if you want to bring the server back up if it goes down

            if (success)
            {
                Console.WriteLine("Node server started just fine at " + nodeServer.Address);
                Console.WriteLine("Stopping server in 5 seconds");
                Thread.Sleep(5000);
                nodeServer.Stop();
            }
            else
            {
                Console.WriteLine("Could not start node server");
            }
            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
        }
    }
}
