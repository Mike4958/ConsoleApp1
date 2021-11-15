#define NOTRASPBERRY2

using System;
//using GalaSoft.MvvmLight.Ioc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging;
//using Windows.UI.Core;

namespace ConsoleApp1
{
    internal class Program
    {

        

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


#if NOTRASPBERRY2
            Console.WriteLine("Waiting 1.5 minutes to allow Wifi Connection");
            Thread.Sleep(90000);
#endif

            Class1 myClass11 = new Class1();

            //Task MyTask = MyClass11.FloatSwitchStatusAlertWatch();

            // send device connected message
            _ = myClass11.SendDeviceToCloudMessagesAsync();
            //    //await _vm.RunSampleAsync();
            //    //await _vm.SendEventsToTopic();

            // receive remote light control events
            _ = myClass11.ReceiveCloudToDeviceMessageAsync();
            _ = myClass11.FloatSwitchStatusAlertWatch();

            Thread.Sleep(10000);

            myClass11.MyTaskDelay = 10;
 
            while (true)
            {
            
                //MyClass11.SendDeviceToCloudMessagesAsync();
                Console.WriteLine("Still Running");

                //Thread.Sleep(2000);
                Task.Delay(2000)
                    .Wait();

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



        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .UseSystemd()
         .ConfigureServices((hostContext, services) =>
         {
             services.AddHostedService<Worker>();
         });


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