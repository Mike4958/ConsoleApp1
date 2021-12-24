
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


#if RASPBERRY
            Console.WriteLine("Waiting 1.5 minutes to allow Wifi Connection");
            Thread.Sleep(90000);
#endif

            Class1 myClass11 = new();

            //Task MyTask = MyClass11.FloatSwitchStatusAlertWatch();

// send device connected message
//_ = myClass11.SendDeviceToCloudMessagesAsync();
//    //await _vm.RunSampleAsync();
//    //await _vm.SendEventsToTopic();

// receive remote light control events


//_ = myClass11.ReceiveCloudToDeviceMessageAsync();


//_ = myClass11.FloatSwitchStatusAlertWatch();
// _ = myClass11.MyMain();

#if RASPBERRY
            Thread.Sleep(10000);
            myClass11.MyTaskDelay = 600;
#else
            myClass11.MyTaskDelay = 60;
#endif

            while (true)
            {
                try 
                {
                    int iCount = 0;
                    int iCount2 = 0;
                    Random r = new();

                    while (iCount < myClass11.MyTaskDelay)
                    {

                        iCount += 1;                  
#if HOURLYCOUNT
                        // Setting iCount randomly back (-) a few seconds will cause While Loop to last longer. This will allow the HourlyCount vary
                        iCount2 = r.Next((int)(myClass11.MyTaskDelay * (.1)), (int)(myClass11.MyTaskDelay * (.35)));
                        iCount += r.Next(1, iCount2);

                        Thread.Sleep((Int32)iCount2 * 1000);
                        _ = myClass11.HourlyCount += 1;

#endif

                        Thread.Sleep((Int32)(1 * 1000));
                    }
#if HOURLYCOUNT

#endif
                    myClass11.FloatSwitchStatusAlertWatch();


                    _ = myClass11.SendDeviceToCloudMessages();

                //if (myClass11.SendEventData) { _ = myClass11.MyMain(); }
                }
                catch (Exception)
                {

                }
                finally
                {
                    Console.WriteLine("Still Running " + DateTime.Now.ToString());

                };
            }
        }

     //   public static IHostBuilder CreateHostBuilder(string[] args) =>
     //Host.CreateDefaultBuilder(args)
     //    .UseSystemd()
     //    .ConfigureServices((hostContext, services) =>
     //    {
     //        services.AddHostedService<Worker>();
     //    });

    }
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }


}