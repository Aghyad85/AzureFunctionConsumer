using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace csharpguitar_elx;



public class y
{
    private readonly ILogger _logger;

    public y(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<y>();
    }

    public static string _globalString = "*** Begin process ***";
    public static object thisLock = new object();

    //  "disabled": "TIMER_FUNCTION_GO",
    [Function("y")]
    public async Task Run([TimerTrigger("%TIMER_FUNCTION_SCHEDULE%")] TimerInfo myTimer, CancellationToken cancellationToken)
    {
        Random r = new Random();
        var number = r.Next(1, 100);
        if (number % 4 == 0)
        {
             _logger.LogInformation($"C# Timer trigger function started execution at: {DateTime.Now} for scenario 'alpha'");
            //runs long (72 seconds), helpful when you want to trigger a cancellation token
            int length = 24;
            for (int i = 0; i < length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    if (cancellationToken.CanBeCanceled)
                    {
                         _logger.LogInformation($"Function invocation was successfully cancelled at: {DateTime.Now}.");
                         _logger.LogInformation($"cancellationToken.CanBeCanceled had a value of: {cancellationToken.CanBeCanceled}");
                         _logger.LogInformation($"The unique identifier: {Guid.NewGuid()}");
                         _logger.LogInformation($"***NOTE*** this is where you can do code clean up, just before being shutdown!");
                         _logger.LogInformation($"***NOTE*** the invocation did not complete, it was cancelled while executing, the code was on iteration {i} of {length}.");
                        break;
                    }
                    else
                    {
                         _logger.LogInformation($"Function invocation cancellation was requested at: {DateTime.Now}.");
                         _logger.LogInformation($"cancellationToken.CanBeCanceled had a value of: {cancellationToken.CanBeCanceled} and therefore could not be cancelled.");
                         _logger.LogInformation($"The unique identifier: {Guid.NewGuid()}");
                         _logger.LogInformation($"***NOTE*** although the code received a cancellation request, the invocation could not be cancelled.");
                    }
                }
                 _logger.LogInformation($"This Function Invocation will loop {length} times.  Current iteration is: {i}");
                Thread.Sleep(5000);
            }
             _logger.LogInformation($"C# Timer trigger function completed execution at: {DateTime.Now} for scenario 'alpha'");
        }
        else
        {
            number = r.Next(1, 100);
            if (number % 4 == 0)
            {
                 _logger.LogInformation($"C# Timer trigger function started execution at: {DateTime.Now} for scenario 'beta'");
                //tip: the implementation of the async/await pattern is wrong and causes big problems
                Stopwatch timer = new Stopwatch();
                timer.Start();
                /* Incorrect */
                 _logger.LogInformation($"Calling InsertAsync()");
                await InsertAsync();
                 _logger.LogInformation($"Calling UpdateAsync()");
                await UpdateAsync();
                /* --------- */
                /* Correct */
                //Task<string> globalInsertStatus = InsertAsync();
                //var globalUpdateStatus = UpdateAsync();
                /* ------- */
                timer.Stop();
                 _logger.LogInformation($"Calling InsertAsync() and UpdateAsync() methods took {timer.Elapsed}");
                 _logger.LogInformation("****************************************************************************");
                 _logger.LogInformation($"The time interval for this timer function is: {Environment.GetEnvironmentVariable("TIMER_FUNCTION_SCHEDULE")}");
                 _logger.LogInformation($"TimerInfo PastDue: {myTimer.IsPastDue}.");
                 _logger.LogInformation($"ScheduleStatus Last: {myTimer.ScheduleStatus.Last}");
                 _logger.LogInformation($"ScheduleStatus LastUpdated: {myTimer.ScheduleStatus.LastUpdated}");
                 _logger.LogInformation($"ScheduleStatus Next: {myTimer.ScheduleStatus.Next}");
                 _logger.LogInformation("****************************************************************************");
                 _logger.LogInformation($"Timer execution interval is: {Environment.GetEnvironmentVariable("TIMER_FUNCTION_SCHEDULE")} but took {timer.Elapsed} to complete.");
                 _logger.LogInformation("****************************************************************************");
                 _logger.LogInformation($"C# Timer trigger function completed execution at: {DateTime.Now} for scenario 'beta'");
                /* Correct */
                // _logger.LogInformation($"The InsertAsync() value of _globalString is {await globalInsertStatus} for scenario 'beta'");
                // _logger.LogInformation($"The UpdateAsync() value of _globalString was {await globalUpdateStatus} for scenario 'beta'");
                /* ------- */
            }
            number = r.Next(1, 100);
            if (number % 2 == 0)
            {
                 _logger.LogInformation($"C# Timer trigger function started execution at: {DateTime.Now} for scenario 'gamma'");
                Thread.Sleep(2000);
                 _logger.LogInformation($"C# Timer trigger function completed execution at: {DateTime.Now} for scenario 'gamma'");
            }
            else
            {
                number = r.Next(1, 100);
                if (number % 2 == 0)
                {
                     _logger.LogInformation($"C# Timer trigger function started execution at: {DateTime.Now} for scenario 'delta'");
                    try
                    {
                        Thread.Sleep(5000);
                        throw new FunctionInvocationException("Explain what can cause a stack overflow exception");
                    }
                    catch (FunctionInvocationException fie)
                    {
                         _logger.LogInformation($"A {fie.GetType()} was thrown.  Was this a hanlded or unhandled exception?");
                         _logger.LogInformation($"C# Timer trigger function completed execution at: {DateTime.Now} for scenario 'delta'");
                    }
                }
                else
                {
                     _logger.LogInformation($"C# Timer trigger function started execution at: {DateTime.Now} for scenario 'epsilon'");
                    Thread.Sleep(10000);
                    throw new FunctionInvocationException("Ouch!  Was this a handled or unhandled exception?");
                    //code execution will cease before logging this, why?
                     _logger.LogInformation($"C# Timer trigger function completed execution at: {DateTime.Now} for scenario 'epsilon'");
                }
            }
        }
    }

    public static async Task<string> InsertAsync()
    {
        lock (_globalString)
        {
            _globalString = $"'InsertAsync() begin {DateTime.Now}'";
            //The Sleep() is a simulated Insert into a very busy data repository
            System.Threading.Thread.Sleep(41000);
            _globalString = $"'InsertAsync() end {DateTime.Now}'";
        }

        await Task.Delay(1000);
        return _globalString;
    }
    public static async Task<string> UpdateAsync()
    {
        lock (_globalString)
        {
            _globalString = $"'UpdateAsync() begin {DateTime.Now}'";
            //The Sleep() is a simulated Update on a very busy data repository
            System.Threading.Thread.Sleep(49000);
            _globalString = $"'UpdateAsync() end {DateTime.Now}'";
        }

        await Task.Delay(1000);
        return _globalString;
    }
}