# NetNoder

NetNoder allows you to start/stop and monitor a node.js express server from a .NET application.

## Installation

### nuget
	Install-Package netnoder

### npm
	npm install netnoder

## Usage on the .NET side

### Setting up the server

	var nodeServer = new NodeServer(new NodeServerSettings
	{
	    WindowStyle = ProcessWindowStyle.Hidden,
	    EntryPointFilePath = @"C:\code\personal\github\netnoder\TestNodeServer\app.js",
	    KillPassword = "MyPasswordToStopServer",
	    Location = new NodeServerLocation
	    {
	        Host = "localhost",
	        Port = 9514,
	        Protocol = "http"
	    }
	});

### Starting and stopping it

	var success = nodeServer.Start();
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

### Monitoring the server

You can set up the server to be monitored so that if the server exits for any reason, it will be restarted automatically. Here is an example:


	var nodeServer = new NodeServer(new NodeServerSettings
	{
		WindowStyle = ProcessWindowStyle.Hidden,
		EntryPointFilePath = @"C:\code\personal\github\netnoder\TestNodeServer\app.js",
		KillPassword = "MyPasswordToStopServer",
		MonitorInterval = 5000,
		Location = new NodeServerLocation
		{
		    Host = "localhost",
		    Port = 9514,
		    Protocol = "http"
		}
	});

	var success = nodeServer.Start();
	if (success)
	{
	    Console.WriteLine("Node server started just fine at " + nodeServer.Address);
	    Console.WriteLine("Monitoring Server Now");
	    nodeServer.Monitor();
	}
	else
	{
	    Console.WriteLine("Could not start node server");
	}

### Options

All options are available in both .NET and node.js.

- Data (**Dictionary[string, object]**) (*optional*): A dictionary that can be used to pass any data to the node.js application.
- EntryPointFilePath (**string**) (*required*): JavaScript file that needs to be run.
- KillPassword (**string**) (*optional*): Password sent to the node.js server in order to validate the kill action. This is to prevent anyone from killing the server.l
- Location (**NodeServerLocation**) (*required*): this object allows you to specify host, port and protocol of the node.js server. Default: `null`
- ExpectedServerStartupTime (**int**) (*optional*): Amount of time that it should take the node.js server to wind up. If the server takes longer then the time specified, the `Start` method will return `false`. Default: `1000`
- WindowStyle (**ProcessWindowStyle**) (*optional*): How to display the node.js shell that is started. Default: `ProcessWindowStyle.Normal`;
- MonitorInterval (**int**) (*optional*): Set interval for the .NET app to monitor the node.js server in order to ensure that it is running. Default: `10000`;

### Properties

- Address (**string**): provides a string with the full URI of the node.js server (i.e.: "http://localhost:3000")
- Monitoring (**bool**): `true` if the server is currently being monitored by the .NET app.
- Settings (**NodeServerSettings**): the options that were provided in the constructor.


### Methods

**All methods are synchronous**.

- bool Start() - Attempts to start the node.js server. Returns `true` on success or `false` on failure.
- bool Stop() - Attempts to stop the node.js server. Returns `true` on success or `false` on failure.
- void Monitor() - Checks to see if the server is up on an interval that can be defined via the `MonitorInterval` option. If the server goes down, Monitor will attempt to bring it back up by executing `Start()` again.
- void StopMonitoring - Stops monitoring the node.js server.


## Usage on the node.js side

### Import express
	var express = require('express');
	var app = express();

### Import netnoder and pass in the express app
	var netnoder = require('netnoder')(app);

### Start the server

	netnoder.listen(function(){
	    console.log('Netnoder server listening on port ' + netnoder.location.host + ':' + netnoder.location.port);
	});

### Or start using the express app

	app.listen(netnoder.location.port, netnoder.location.host, function(){
	    console.log('Netnoder server listening on port ' + netnoder.location.host + ':' + netnoder.location.port);
	});

### You can access all of the options and settings from the .NET side:

	console.log(netnoder.location.host) ==> "localhost"


### Methods

- listen([backlog], [callback]) - Starts listening on the host and port specified on the .NET side. This method call the express listen method so you can still pass all the arguments that you can pass to express; just remember to skip the host and port because those are passed automatically.

## Contributing

If you would like to contribute, you may do so to the development branch.
