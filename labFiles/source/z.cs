using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.IO; // added for StreamReader
using System.Threading.Tasks; // added for async/Task

namespace csharpguitar_elx;

public class z
{
    private readonly ILogger<z> _logger;

    public z(ILogger<z> logger)
    {
        _logger = logger;
    }
            
    [Function("z")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "csharpguitar")] HttpRequest req, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        int length = 40;
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        string cancel = data?["cancel"];

        if (cancel == "yes")
        {
            for (int i = 0; i < length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    if (cancellationToken.CanBeCanceled)
                    {
                        _logger.LogInformation($"Function invocation was successfully cancelled at: {DateTime.Now} using the '{req.Method}' method.");
                        _logger.LogInformation($"cancellationToken.CanBeCanceled had a value of: {cancellationToken.CanBeCanceled}");
                        _logger.LogInformation($"The unique identifier: {Guid.NewGuid()}");
                        break;
                    }
                    else
                    {
                        _logger.LogInformation($"Function invocation cancellation was requested at: {DateTime.Now} using the '{req.Method}' method.");
                        _logger.LogInformation($"cancellationToken.CanBeCanceled had a value of: {cancellationToken.CanBeCanceled} and therefore could not be cancelled.");
                        _logger.LogInformation($"The unique identifier: {Guid.NewGuid()}");
                    }
                }
                _logger.LogInformation($"This Function Invocation will loop {length} times.  Current iteration is: {i}");
                Thread.Sleep(5000);
            }
        }

        string responseMessage = string.IsNullOrEmpty(cancel)
            ? $"HTTP triggered function executed successfully using the '{req.Method}' method. {Guid.NewGuid()}"
            : $"A value of {cancel} was recevied by this HTTP triggered function using the '{req.Method}' method.";

        return new OkObjectResult(responseMessage);
    }
}