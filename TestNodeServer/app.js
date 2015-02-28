process.on('uncaughtException', function(err) {
    console.log(err.message + ': ' + err.stack);
}); 
var express = require('express');
var app = express();
var netnoder = require('netnoder')(app);

netnoder.listen(function(){
    console.log('Netnoder server listening on port ' + netnoder.location.host + ':' + netnoder.location.port);
});
