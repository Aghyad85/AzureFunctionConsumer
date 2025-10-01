using Azure.Storage.Blobs;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace csharpguitar_elx;

 // "disabled": "BLOB_FUNCTION_GO",

public class x
{
    private readonly ILogger<x> _logger;

    public x(ILogger<x> logger)
    {
        _logger = logger;
    }

    //[Disable("BLOB_FUNCTION_GO")]

    [Function(nameof(x))]
    public async Task Run([BlobTrigger("elx/{name}", Connection = "BLOB_CONNECTION")] Stream myBlob, string name, Uri uri, IDictionary<string, string> metadata)
    {
        //using var blobStreamReader = new StreamReader(stream);
        //var content = await blobStreamReader.ReadToEndAsync();
        //_logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, content);


        _logger.LogInformation($"C# Blob trigger function processed blob named: {name} with a size of: {myBlob.Length} bytes");

        var connectionString = Environment.GetEnvironmentVariable("BLOB_CONNECTION");
        var container = new BlobContainerClient(connectionString, "elx");
        var blob = container.GetBlobClient(name);

        _logger.LogInformation($"************************* blob properties for: {name} *************************");
        _logger.LogInformation($"Blob: {name} has an ETAG of {blob.GetProperties().Value.ETag}");
        _logger.LogInformation($"Blob: {name} has a creation time of {blob.GetProperties().Value.CreatedOn}");
        _logger.LogInformation($"Blob: {name} has a last modified value of {blob.GetProperties().Value.LastModified}");
        _logger.LogInformation($"*******************************************************************************");

        Type uriType = typeof(Uri);
        PropertyInfo[] properties = uriType.GetProperties();
        foreach (PropertyInfo uriProp in properties)
        {
            _logger.LogInformation($"File Property Name: {uriProp.Name} Value: {uriProp.GetValue(uri, null)}");
        }

        foreach (KeyValuePair<string, string> data in metadata)
        {
            _logger.LogInformation($"User-Defined Metadata Key  = {data.Key}");
            _logger.LogInformation($"User-Defined Metadata Value  = {data.Value}");
        }
        if (metadata.Count == 0)
        {
            _logger.LogInformation("No user-defined metadata was found.");
        }
    }
}