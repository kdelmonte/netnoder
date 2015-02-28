using System.Collections.Generic;
using System.Diagnostics;

namespace NetNoder.Models
{
    public class NodeServerSettings
    {
        public Dictionary<string, object> Data;
        public string EntryPointFilePath;
        public int ExpectedServerStartupTime = 1000;
        public string KillPassword;
        public NodeServerLocation Location;
        public int MonitorInterval = 10000;
        public ProcessWindowStyle WindowStyle = ProcessWindowStyle.Normal;
    }
}