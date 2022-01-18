using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {

        public static void Main(string[] args)
        {

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Hello World!");


#if RASPBERRY
            Console.WriteLine("Waiting 1.5 minutes to allow Wifi Connection");
            //Thread.Sleep(90000);
            Thread.Sleep(60000);
#endif

            Class1 class1 = new();
            Class1 myClass11 = class1;

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
            Thread.Sleep(15000);
            
            //myClass11.MyTaskDelay = 120;
#else
            myClass11.MyTaskDelay = 60;
#endif
            Task Floattask = myClass11.FloatSwitchStatusAlertWatch();
            Task tsWhile;

            CancellationTokenSource ts = new CancellationTokenSource();


            while (true)
            {
                try
                {
                    int iCount = 0;
                    int iCount2 = 0;
                    Random r = new();

                    while (iCount < myClass11.MyTaskDelay)
                    {
                        Console.ForegroundColor = ConsoleColor.White;

                        iCount += myClass11.WhileSpeed;
                        Console.WriteLine("Main WhileLoop Count = " + iCount.ToString());


#if HOURLYCOUNT && !RASPBERRY
                        // Setting iCount randomly back (-) a few seconds will cause While Loop to last longer. This will allow the HourlyCount vary
                        iCount2 = r.Next((int)(myClass11.MyTaskDelay * (.1)), (int)(myClass11.MyTaskDelay * (.35)));
                        iCount += r.Next(1, iCount2);

                        Thread.Sleep((Int32)iCount2 * 1000);
                        _ = myClass11.HourlyCount += 1;

#endif
                        //await Task.Delay(1000);
                        //Thread.Sleep((Int32)(myClass11.WhileSpeed * 1000));
                        //myClass11.FloatSwitchStatusAlertWatch();
                        tsWhile = Task.Run(() =>
                        {
                            Task.Delay((int)(myClass11.WhileSpeed * 1000)).Wait();
                            //ts.Cancel();
                        });

                        tsWhile.Wait(); // Over all this takes longer than a second to End

                    }
#if HOURLYCOUNT

#endif
                    //Task.Run(() =>
                    //{
                        myClass11.SendDeviceToCloudMessages();
                    //});

                    //if (myClass11.SendEventData) { _ = myClass11.MyMain(); }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Main: " + ex.Message);
                    Debug.WriteLine("Mian: " + ex.Message);
                    Console.ResetColor();
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