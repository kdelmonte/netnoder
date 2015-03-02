using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Build.Utilities;
using NetNoder.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NetNoder
{
    public class NodeServer
    {
        private string _commandLineArguments;
        public readonly NodeServerSettings Settings;

        public NodeServer(){}

        public NodeServer(NodeServerSettings settings)
        {
            Settings = settings;
            BuildCommandLineArguments();
        }

        public string Address
        {
            get { return GetAddress(); }
        }

        public bool Monitoring { get; protected set; }
        private Thread MonitoringThead { get; set; }

        private Stream Post(string route, object data = null, int timeout = 100000)
        {
            return HttpPostJson(Address + route.TrimStart('/'), data, timeout);
        }

        public void Monitor()
        {
            MonitoringThead = new Thread(() =>
            {
                try
                {
                    Thread.CurrentThread.IsBackground = true;
                    while (true)
                    {
                        try
                        {
                            Start();
                            Thread.Sleep(Settings.MonitorInterval);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            });
            MonitoringThead.Start();
            Monitoring = true;
        }

        public void StopMonitoring()
        {
            if (MonitoringThead != null)
            {
                MonitoringThead.Abort();
            }
            Monitoring = false;
        }

        public bool Ping()
        {
            try
            {
                Post("netnoderping");
                return true;
            }
            catch (Exception ex)
            {
                // If the error is anything else then it must 
                // mean that the server is running
                return ex.Message != "Unable to connect to the remote server";
            }
        }

        private void BuildCommandLineArguments()
        {
            var commandLineBuilder = new CommandLineBuilder();
            commandLineBuilder.AppendFileNameIfNotNull(PrepareJsonForCommandLine(JsonConvertCamelCase(Settings)));

            _commandLineArguments = commandLineBuilder.ToString();
        }

        public bool Start()
        {
            try
            {
                if (Ping()) return true;
                var codeFile = new FileInfo(Settings.EntryPointFilePath);
                if (!codeFile.Exists)
                {
                    throw new Exception("Code file does not exist");
                }

                if (_commandLineArguments == null)
                {
                    BuildCommandLineArguments();
                }
                var startupCommand = "node " + codeFile.Name + " --netnoder " + _commandLineArguments;
#if DEBUG
                startupCommand += " --debug";
#endif
                var app = new Process
                {
                    StartInfo = new ProcessStartInfo("cmd", "/c " + startupCommand)
                    {
                        WorkingDirectory = codeFile.DirectoryName,
                        UseShellExecute = true,
                        WindowStyle = Settings.WindowStyle
                    }
                };

                if (Settings.WindowStyle == ProcessWindowStyle.Hidden)
                {
                    app.StartInfo.CreateNoWindow = true;
                }
                app.Start();
                if (Settings.ExpectedServerStartupTime > 0)
                {
                    Thread.Sleep(Settings.ExpectedServerStartupTime);
                }
                return Ping();
            }
            catch (Exception ex)
            {
                throw new Exception("Error Starting Node Server at:" + Settings.EntryPointFilePath, ex);
            }
        }

        public bool Stop()
        {
            try
            {
                var response = new StreamReader(Post("netnoderkill", new {pw = Settings.KillPassword})).ReadToEnd();
                return response == "Accepted";
            }
            catch (Exception ex)
            {
                return ex.Message == "Unable to connect to the remote server";
            }
        }

        private string GetAddress()
        {
            return Settings.Location.Protocol + "://" + Settings.Location.Host +
                   (Settings.Location.Port == 0 ? "" : ":" + Settings.Location.Port) + "/";
        }

        private static Stream HttpPostJson(string url, object data = null, int timeout = 100000)
        {
            var http = (HttpWebRequest) WebRequest.Create(new Uri(url));
            http.ContentType = "application/json";
            http.Method = "POST";
            http.Timeout = timeout;

            var encoding = new ASCIIEncoding();
            var json = JsonConvertCamelCase(data ?? new {});
            var bytes = encoding.GetBytes(json);

            var newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            return response.GetResponseStream();
        }

        private static string JsonConvertCamelCase(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        private static string PrepareJsonForCommandLine(string json)
        {
            return json.Replace("\"", "<netnoderdq>").Replace("'", "<netnodersq>");
        }
    }
}