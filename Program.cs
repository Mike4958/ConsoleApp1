
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
//using GalaSoft.MvvmLight.Ioc;
using System.Threading;
using System.Threading.Tasks;
//using Windows.UI.Core;

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

            Class1 myClass11 = new Class1();

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
                    //MyClass11.SendDeviceToCloudMessagesAsync();
                    //Console.WriteLine("Still Running");


                    //Thread.Sleep(60000);
                    //Task.Delay(15000)
                    //    .Wait();

                    //Task.Delay((Int32)(myClass11.MyTaskDelay * 1000));
                    Thread.Sleep((Int32)(myClass11.MyTaskDelay * 1000));


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
                //_ = myClass11.MyMain();

            }

            // Loaded += async (sender, args) =>
            // {
            //    // send device connected message
            //    //await SendDeviceToCloudMessagesAsync();
            //    //await _vm.RunSampleAsync();
            //    //await _vm.SendEventsToTopic();

            //    // receive remote light control events
            //    //await _vm.ReceiveCloudToDeviceMessageAsync();

            //    //_vm.FloatSwitchStatusAlertWatch().Start();


            //  };


            Console.WriteLine(args.Length);
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