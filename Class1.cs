//using GalaSoft.MvvmLight.Ioc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Gpio;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    

    public class Class1
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Class1()
        {
            
#if DEBUG
            // Create event triggers for DEBUG PI port pins that need to monitored for chnages.
            // These  changes will cause a Event Trigger
            //Gpio.RegisterCallbackForPinValueChangedEvent(ButtonPinNumber, PinEventTypes.Rising, PinChangeEventHandler);
            Console.WriteLine("DEBUG PI COMPLIE");
#else
            Console.WriteLine("REGULAR COMPLIE");
#endif
            //Task task = InitializeDeviceState();
            InitializeDeviceState();

            // send device connected message
            //Task task1 = SendDeviceToCloudMessagesAsync();
            //    //await _vm.RunSampleAsync();
            //    //await _vm.SendEventsToTopic();

            //receive remote light control events
           //Task task2 = ReceiveCloudToDeviceMessageAsync();


            //Task task3 = FloatSwitchStatusAlertWatch();

        }

        //public async Task InitializeDeviceState()
        public void InitializeDeviceState()
        {

#if DEBUG
            gpioController = new GpioController();
#else
            Console.WriteLine("REGULAR COMPLIE");

#endif
            Twin twin;

            // Example --- int me2 = ("5" == "5") ? 1 : 0;
            twin = myDeviceClient.GetTwinAsync().Result;
            //Twin twin = myDeviceClient.GetTwinAsync();
            //twin.Properties.Desired
            registryManager = RegistryManager.CreateFromConnectionString("HostName=MyIOTHubFeb2019.azure-devices.net;DeviceId=MyAzureID2;SharedAccessKey=ZaF8Ehv4nSW1R5J+/iEX2U/KuQ0JwZVz0jG+y9jcuyE=");
            //registryManager = RegistryManager.CreateFromConnectionString("HostName=MyIOTHubFeb2019.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=O61Lh8sR/AKURvN7xo6G05AJzlJsBcG78Fx2TVH8CHY=");
            //HostName=MyIOTHubFeb2019.azure-devices.net;DeviceId=MyAzureID2;SharedAccessKey=ZaF8Ehv4nSW1R5J+/iEX2U/KuQ0JwZVz0jG+y9jcuyE=
            myDeviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, twin).GetAwaiter().GetResult();

            ledPin = (twin.Properties.Desired["floatSwitchTwo"] == "ON") ? 1 : 5;

            twin.Properties.Desired["floatSwitchTwo"] = "DESIREDOFF222";

            twin.Properties.Reported["floatSwitchTwo"] = "REPORTEDOFF3333";

            registryManager.UpdateTwinAsync("MyAzureID2", twin, "AAAAAAAAAFY=").GetAwaiter();

            MainViewModel();

            Console.WriteLine("Sending sample start time as reported property");
        }

        // This Task is never fired when using Device SDK 1.29.0-preview-004
        private static async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine("desired property change:");
            Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));
            Console.WriteLine("Sending current time as reported property");
            TwinCollection reportedProperties = new TwinCollection
            {
                ["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now
            };

            await myDeviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }
#region Azure IoT Hub Settings

        /// <summary>
        /// Endpoint of the Event Grid Topic.
        /// Update this with your own endpoint from the Azure Portal.
        /// </summary>
        private const string TOPIC_ENDPOINT = "https://myeventgridtopic1.eastus2-1.eventgrid.azure.net/api/events";

        /// <summary>
        /// Key of the Event Grid Topic.
        /// Update this with your own key from the Azure Portal.
        /// </summary>
        private readonly string KEY = "pkMsXJVQ9GEYpRybtZ3jdy2mDX431i36SemrXeApk/Y=";

        // TODO: Enter values for <topic-name> and <region>. You can find this topic endpoint value
        // in the "Overview" section in the "Event Grid Topics" blade in Azure Portal.
        private static readonly string topicEndpoint = "https://myeventgridtopic1.eastus2-1.eventgrid.azure.net/api/events";

        // TODO: Enter value for <topic-key>. You can find this in the "Access Keys" section in the
        // "Event Grid Topics" blade in Azure Portal.
        private static readonly string topicKey = "pkMsXJVQ9GEYpRybtZ3jdy2mDX431i36SemrXeApk/Y=";


        //ZaF8Ehv4nSW1R5J+/iEX2U/KuQ0JwZVz0jG+y9jcuyE=
        //private static readonly string DeviceKey = "cj6vhwu485x5VUn6A5ElBWgH0lBIkLXGNL2DCnEjAaA=";
        private static readonly string DeviceKey = "ZaF8Ehv4nSW1R5J+/iEX2U/KuQ0JwZVz0jG+y9jcuyE=";

        //MyAzureID2

        //private static string DeviceId => "MyRasp2";
        private static string DeviceId => "MyAzureID2";

        //
        //private static readonly string IotHubUri = "Firt-Iot-Hub.azure-devices.net";
        private static readonly string IotHubUri = "MyIOTHubFeb2019.azure-devices.net";

        private static readonly DeviceClient myDeviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
        //EmailDistributionGroup = "eagan_mike@hotmail.com";

        private static readonly TopicCredentials topicCredentials = new(topicKey);
        private static readonly EventGridClient client = new(topicCredentials);
        #endregion

        private readonly string topicHostname = new Uri(topicEndpoint).Host;

#if DEBUG
        public GpioController Gpio { get; set; } = new GpioController();

#else

#endif


        private static GpioController gpioController;

#region Set_GPIO_PinsNbrs

        private int pin_FloatSwitchLow = 4;
        private int pin_FloatSwitchMedium = 5;
        private int pin_FloatSwitchHigh = 6;
        private int pin_FloatSwitchIndicatorLedLow = 23;
        private int pin_FloatSwitchIndicatorLedMedium = 24;
        private int pin_FloatSwitchIndicatorLedHigh = 25;

        /// <summary>Property - Pin_FloatSwitchHigh is rendered as GPIO Input Pin 
        ///          
        /// <example>
        /// <code>
        /// </code>
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments#code"/>
        /// </example>    
        /// </summary>
        private int Pin_FloatSwitchHigh { get => pin_FloatSwitchHigh; set => pin_FloatSwitchMedium = value; }

        /// <summary>Property - <c>Pin_FloatSwitchHigh</c> is rendered as GPIO Input Pin 
        /// </summary>
        public int Pin_FloatSwitchMedium { get => pin_FloatSwitchMedium; set => pin_FloatSwitchMedium = value; }

        /// <summary>This method changes the point's location by
        ///    the given x- and y-offsets.
        /// <example>For example:
        /// <code>
        ///    Point p = new Point(3,5);
        ///    p.Translate(-1,3);
        /// </code>
        /// results in <c>p</c>'s having the value (2,8).
        /// </example>
        /// </summary>
        /// 
        public int Pin_FloatSwitchLow { get => pin_FloatSwitchLow; set => pin_FloatSwitchLow = value; }
        public int Pin_FloatSwitchIndicatorLedHigh { get => pin_FloatSwitchIndicatorLedHigh; set => pin_FloatSwitchIndicatorLedHigh = value; }
        public int Pin_FloatSwitchIndicatorLedMedium { get => pin_FloatSwitchIndicatorLedMedium; set => pin_FloatSwitchIndicatorLedMedium = value; }
        public int Pin_FloatSwitchIndicatorLedLow { get => pin_FloatSwitchIndicatorLedLow; set => pin_FloatSwitchIndicatorLedLow = value; }

#endregion

#region Set_PinvValueProperties
        private PinValue pinValueFloatSwitchLow;
        private PinValue pinValueFloatSwitchMedium;
        private PinValue pinValueFloatSwitchHigh;
        private PinValue pinvalueFloatSwitchIndicatorLedLow;
        private PinValue pinvalueFloatSwitchIndicatorLedMedium;
        private PinValue pinvalueFloatSwitchIndicatorLedHigh;

        public PinValue PinValueFloatSwitchLow { get { return pinValueFloatSwitchLow; } set { pinValueFloatSwitchLow = value; } }
        public PinValue PinValueFloatSwitchMedium { get => pinValueFloatSwitchMedium; set => pinValueFloatSwitchMedium = value; }
        public PinValue PinValueFloatSwitchHigh { get => pinValueFloatSwitchHigh; set => pinValueFloatSwitchHigh = value; }
        public PinValue PinvalueFloatSwitchIndicatorLedLow { get => pinvalueFloatSwitchIndicatorLedLow; set => pinvalueFloatSwitchIndicatorLedLow = value; }
        public PinValue PinvalueFloatSwitchIndicatorLedMedium { get => pinvalueFloatSwitchIndicatorLedMedium; set => pinvalueFloatSwitchIndicatorLedMedium = value; }
        public PinValue PinvalueFloatSwitchIndicatorLedHigh { get => pinvalueFloatSwitchIndicatorLedHigh; set => pinvalueFloatSwitchIndicatorLedHigh = value; }

#endregion

        private string strFloatSwitchStatusMsg;

        private readonly int pin_LedPin = 17;
        private readonly int pin_PIRLedPin = 27;
        private readonly int pin_PIR_Pin = 16;

        private int ledPin;
        private int pIR_LedPin;
        private int pIR_Pin;

        /// <summary>
        ///     Mike Drives the specified value onto the general purpose I/O (GPIO) pin according
        ///     to the current drive mode for the pin if the pin is configured as an output,
        ///     or updates the latched output value for the pin if the pin is configured as an
        ///     input.
        /// </summary>
        public int LedPin { get { return ledPin; } set { ledPin = value; RaisePropertyChanged(); } }

        public int PIRLedPin { get { return pIR_LedPin; } set { pIR_LedPin = value; RaisePropertyChanged(); } }

        public int PIR_Pin { get { return pIR_Pin; } set { pIR_Pin = value; RaisePropertyChanged(); } }


        public string StrFloatSwitchStatusMsg { get => strFloatSwitchStatusMsg; set => strFloatSwitchStatusMsg = value; }
        private string GpioMsg { get => gpioMsg; set => gpioMsg = value; }

#region Display Fields

        #endregion

#region btnToggel

        private bool btnToggleHighIsChecked;
        private bool btnToggleMiddleIsChecked;
        private bool btnToggleLowIsChecked;

        public bool BtnToggleHighIsChecked
        {
            get { return btnToggleHighIsChecked; }
            set { btnToggleHighIsChecked = value; RaisePropertyChanged(); }
        }

        public bool BtnToggleMiddleIsChecked
        {
            get { return btnToggleMiddleIsChecked; }
            set { btnToggleMiddleIsChecked = value; RaisePropertyChanged(); }
        }

        public bool BtnToggleLowIsChecked
        {
            get { return btnToggleLowIsChecked; }
            set { btnToggleLowIsChecked = value; RaisePropertyChanged(); }
        }

        #endregion

        //private static int ButtonPinNumber;

#region Miscellaneous

        private bool _isAzureConnected;
        private string _cloudToDeviceLog;

        private string floatSwitchEmailMsg = "";
        private string yesterdayDay;
        private string gpioMsg;

        private string textBlockString;
        private string emailDistributionGroup = "eagan_mike@hotmail.com";
        private double myTaskDelay = 1;

        public bool IsOn { get; set; }

        public bool IsAzureConnected
        {
            get { return _isAzureConnected; }
            set { _isAzureConnected = value; RaisePropertyChanged(); }
        }

        public string CloudToDeviceLog
        {
            get { return _cloudToDeviceLog; }
            set { _cloudToDeviceLog = value; RaisePropertyChanged(); }
        }
        public string TextBlockString
        {
            get { return textBlockString; }
            set { textBlockString = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 
        ///     Email Distribution group
        /// </summary>
        public string EmailDistributionGroup
        {
            get { return emailDistributionGroup; }
            set { emailDistributionGroup = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 
        ///     Number of Seconds to wait before repeating Condition Test of State
        /// </summary>
        public double MyTaskDelay
        {
            get { return myTaskDelay; }
            set { myTaskDelay = value; RaisePropertyChanged(); }
        }

        public string FloatSwitchEmailMsg
        {
            get { return floatSwitchEmailMsg; }
            set { floatSwitchEmailMsg = value; }
        }

        public string YesterdayDay
        {
            get { return yesterdayDay; }
            set { yesterdayDay = value; RaisePropertyChanged(); }
        }

        public object TextBlockSlider { get; set; }
        #endregion

#region switchEventAlerttimeDifference

        private string switchEventAlertTimeDifferenceTop = "5"; //Default Number of minutes to wait before sending Alert email
        private string switchEventAlertTimeDifferenceMiddle = "5"; //Default Number of minutes to wait before sending Alert email
        private string switchEventAlertTimeDifferenceHighErr = "5"; //Default Number of minutes to wait before sending Alert email
        private string switchEventAlertTimeDifferenceHighReset = "5";

        /// <summary>
        /// 
        ///     Number of minutes to wait before sending Alert email
        /// </summary>
        public string SwitchEventAlertTimeDifferenceTop
        {
            get { return switchEventAlertTimeDifferenceTop; }
            set { switchEventAlertTimeDifferenceTop = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 
        ///     Number of minutes to wait before sending Alert email
        /// </summary>
        public string SwitchEventAlertTimeDifferenceMiddle
        {
            get { return switchEventAlertTimeDifferenceMiddle; }
            set { switchEventAlertTimeDifferenceMiddle = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 
        ///     Number of minutes to wait before sending Alert email
        /// </summary>
        public string SwitchEventAlertTimeDifferenceHighErr
        {
            get { return switchEventAlertTimeDifferenceHighErr; }
            set { switchEventAlertTimeDifferenceHighErr = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 
        ///     Number of minutes to wait before sending Alert email
        /// </summary>
        public string SwitchEventAlertTimeDifferenceReset
        {
            get { return switchEventAlertTimeDifferenceHighReset; }
            set { switchEventAlertTimeDifferenceHighReset = value; RaisePropertyChanged(); }
        }

        #endregion

#region alertTriggerd

        //private string alertTriggeredTimeErrLast;
        private string alertTriggeredTimeTop;
        private string alertTriggeredTimeMiddle;
        private string alertTriggeredTimeErr;
        private string alertTriggeredTimeReset;
        private string alertTriggeredTimeTopLast;
        private string alertTriggeredTimeMiddleLast;
        private string alertTriggeredTimeErrLast;
        private string alertTriggeredTimeResetLast;
        private RegistryManager registryManager;

        //private RegistryManager registryManager;

        public string AlertTriggeredTimeTopLast
        {
            get { return alertTriggeredTimeTopLast; }
            set { alertTriggeredTimeTopLast = value; RaisePropertyChanged(); }
        }

        public string AlertTriggeredTimeTop
        {
            get { return alertTriggeredTimeTop; }
            set { alertTriggeredTimeTop = value; RaisePropertyChanged(); }
        }

        public string AlertTriggeredTimeMiddleLast
        {
            get { return alertTriggeredTimeMiddleLast; }
            set
            {
                alertTriggeredTimeMiddleLast = value; RaisePropertyChanged();
            }
        }

        public string AlertTriggeredTimeMiddle
        {
            get { return alertTriggeredTimeMiddle; }
            set { alertTriggeredTimeMiddle = value; RaisePropertyChanged(); }
        }

        public string AlertTriggeredTimeErrLast
        {
            get { return alertTriggeredTimeErrLast; }
            set { alertTriggeredTimeErrLast = value; RaisePropertyChanged(); }
        }

        public string AlertTriggeredTimeErr
        {
            get { return alertTriggeredTimeErr; }
            set { alertTriggeredTimeErr = value; RaisePropertyChanged(); }
        }

        public string AlertTriggeredTimeResetLast
        {
            get { return alertTriggeredTimeResetLast; }
            set { alertTriggeredTimeResetLast = value; RaisePropertyChanged(); }
        }

        public string AlertTriggeredTimeReset
        {
            get { return alertTriggeredTimeReset; }
            set { alertTriggeredTimeReset = value; RaisePropertyChanged(); }
        }

        //public MainPage RootPage { get => rootPage; set => rootPage = value; }

        #endregion

        public void MainViewModel()
        {

            //DeviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
            EmailDistributionGroup = "eagan_mike@hotmail.com";

            //topicCredentials = new TopicCredentials(topicKey);
            //client = new EventGridClient(topicCredentials);

            //            Twin twin = await _moduleClient.GetTwinAsync().ConfigureAwait(false);
            //using (var manager = RegistryManager.CreateFromConnectionString("Your IoT Hub ConnectionString"))
            //{
            //   var twin = await manager.GetTwinAsync("your device id");
            //   twin.Properties.Desired["YourProperty"] = "some value";
            //   await manager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
            //}            


            if (null != gpioController)
            {


                gpioController.OpenPin(Pin_FloatSwitchLow);
                gpioController.SetPinMode(Pin_FloatSwitchLow, PinMode.InputPullUp);
                gpioController.RegisterCallbackForPinValueChangedEvent(Pin_FloatSwitchLow, PinEventTypes.Rising, Pin_FloatSwitchLow_ValueChanged);
                //Pin_FloatSwitchHigh = gpioController.OpenPin(6);
                //Pin_FloatSwitchHigh.SetDriveMode(GpioPinDriveMode.InputPullUp);  // GpioPinDriveMode.InputPullUp
                //Pin_FloatSwitchHigh.DebounceTimeout = TimeSpan.FromSeconds(2);

                gpioController.OpenPin(pin_FloatSwitchIndicatorLedLow);
                gpioController.SetPinMode(pin_FloatSwitchIndicatorLedLow, PinMode.Output);
                gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.Low);
                gpioController.RegisterCallbackForPinValueChangedEvent(pin_FloatSwitchIndicatorLedLow, PinEventTypes.Falling, Pin_FloatSwitchIndicatorLedLow_ValueChanged);
                //Pin_FloatSwitchIndicatorLedLow = GpioController.OpenPin(23);
                //Pin_FloatSwitchIndicatorLedLow.SetDriveMode(GpioPinDriveMode.Output);
                //Pin_FloatSwitchIndicatorLedLow.DebounceTimeout = TimeSpan.FromSeconds(1);
                //Pin_FloatSwitchIndicatorLedLow.Write(GpioPinValue.Low);

                gpioController.OpenPin(Pin_FloatSwitchMedium);
                gpioController.SetPinMode(Pin_FloatSwitchMedium, PinMode.InputPullUp);
                gpioController.RegisterCallbackForPinValueChangedEvent(Pin_FloatSwitchMedium, PinEventTypes.Rising, Pin_FloatSwitchMedium_ValueChanged);
                //Pin_FloatSwitchMedium = GpioController.OpenPin(5);
                //Pin_FloatSwitchMedium.SetDriveMode(GpioPinDriveMode.InputPullUp); // GpioPinDriveMode.InputPullUp
                //Pin_FloatSwitchMedium.DebounceTimeout = TimeSpan.FromSeconds(2);

                gpioController.OpenPin(pin_FloatSwitchIndicatorLedMedium);
                gpioController.SetPinMode(pin_FloatSwitchIndicatorLedMedium, PinMode.Output);
                gpioController.Write(pin_FloatSwitchIndicatorLedMedium, PinValue.Low);
                gpioController.RegisterCallbackForPinValueChangedEvent(pin_FloatSwitchIndicatorLedMedium, PinEventTypes.Falling, Pin_FloatSwitchIndicatorLedMedium_ValueChanged);
                //Pin_FloatSwitchIndicatorLedMedium = GpioController.OpenPin(24);
                //Pin_FloatSwitchIndicatorLedMedium.SetDriveMode(GpioPinDriveMode.Output);
                //Pin_FloatSwitchIndicatorLedMedium.DebounceTimeout = TimeSpan.FromSeconds(1);
                //Pin_FloatSwitchIndicatorLedMedium.Write(GpioPinValue.Low);

                gpioController.OpenPin(Pin_FloatSwitchHigh);
                gpioController.SetPinMode(Pin_FloatSwitchHigh, PinMode.InputPullUp);
                gpioController.RegisterCallbackForPinValueChangedEvent(Pin_FloatSwitchHigh, PinEventTypes.Rising, Pin_FloatSwitchHigh_ValueChanged);
                //Pin_FloatSwitchLow = GpioController.OpenPin(4);
                //Pin_FloatSwitchLow.SetDriveMode(GpioPinDriveMode.InputPullUp);  // GpioPinDriveMode.InputPullUp
                //Pin_FloatSwitchLow.DebounceTimeout = TimeSpan.FromSeconds(2);

                gpioController.OpenPin(pin_FloatSwitchIndicatorLedHigh);
                gpioController.SetPinMode(pin_FloatSwitchIndicatorLedHigh, PinMode.Output);
                gpioController.Write(pin_FloatSwitchIndicatorLedHigh, PinValue.Low);
                gpioController.RegisterCallbackForPinValueChangedEvent(pin_FloatSwitchIndicatorLedHigh, PinEventTypes.Falling, Pin_FloatSwitchIndicatorLedHigh_ValueChanged);
                //Pin_FloatSwitchIndicatorLedHigh = GpioController.OpenPin(25);
                //Pin_FloatSwitchIndicatorLedHigh.SetDriveMode(GpioPinDriveMode.Output);
                //Pin_FloatSwitchIndicatorLedHigh.DebounceTimeout = TimeSpan.FromSeconds(1);
                //Pin_FloatSwitchIndicatorLedHigh.Write(GpioPinValue.Low);


                gpioController.OpenPin(17);
                gpioController.OpenPin(27);
                gpioController.OpenPin(16);

                gpioController.SetPinMode(17, PinMode.Output);
                //LedPin.SetDriveMode(GpioPinDriveMode.Output);
                //LedPin.DebounceTimeout = TimeSpan.FromSeconds(1);   //NOT Needed When GPIOPinDrive type Output State

                gpioController.SetPinMode(27, PinMode.Output);
                //PIR_LedPin.SetDriveMode(GpioPinDriveMode.Output);
                //PIR_LedPin.DebounceTimeout = TimeSpan.FromSeconds(1);  // NOT Needed When GPIOPinDrive type Output State

                gpioController.SetPinMode(16, PinMode.Output);
                //PIR_Pin.SetDriveMode(GpioPinDriveMode.Input);
                //PIR_Pin.DebounceTimeout = TimeSpan.FromSeconds(1);

                //if (Pin_FloatSwitchHigh) { Pin_FloatSwitchHigh.Write(GpioPinValue.Low)};

                //RunSampleAsync().GetAwaiter().GetResult();
            }


            //if (FloatSwitchStatusAlertWatch() == null)
            //{   //This if may not be neeed
            //    var t = FloatSwitchStatusAlertWatch();
            //    t.Start();
            //};

        }


        #region FloatSwitchesValueChanged
        private void Pin_FloatSwitchLow_ValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {

            try
            {


                // Cool Stuff                    PinValueChangedEventArgs pinValueChangedEventArgs1 = new(pinValueChangedEventArgs.ChangeType,pin_FloatSwitchHigh);
                // Cool Stuff                    Pin_FloatSwitchIndicatorLedLow_ValueChanged(sender, pinValueChangedEventArgs: pinValueChangedEventArgs1);








                PinValueFloatSwitchLow = gpioController.Read(pinValueChangedEventArgs.PinNumber);
                Console.WriteLine(DateTime.Now + " " + pinValueChangedEventArgs.PinNumber + " pin = " + PinValueFloatSwitchLow);

                try
                {
                    //PinValue FloatSwitchStatus = sender.Read(); //"Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString()

                    // StrFloatSwitchStatusMsg = "Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString();
                    // TextBlockString = StrFloatSwitchStatusMsg;

                    if (PinValueFloatSwitchLow == PinValue.High)  // ***  Floatswitch Open
                    {
                        //GpioMsg = "PIR_Activated";
                        //PirLEDStatus = true;
                        BtnToggleLowIsChecked = false;
                        gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.Low);
                        //gpioController.Write((int)sender, PinValue.Low);   // ***  Reason Floatswitch is Open

                    }
                    else                                    // *** Floatswitch Closed
                    {
                        BtnToggleLowIsChecked = true;
                        gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.High);
                        //gpioController.Write((int)sender, PinValue.High);  // *** Reason Floatswitch Close

                    }
                }
                //SendDeviceToCloudMessagesAsync();
                catch (Exception ex)
                {
                    // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    // () =>
                    // {
                    Debug.WriteLine(ex.Message);
                    // });
                }
            }
            catch (Exception ex)
            {
                // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                // () =>
                // {
                Debug.WriteLine(ex.Message);
                // });
            }
            finally
            {
                //Turn the LED Status Light on - we're ready for another picture
                //LightPIR_LED(false);
            }
            //throw new NotImplementedException();

        }

        private void Pin_FloatSwitchIndicatorLedLow_ValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {

            try
            {
                pinvalueFloatSwitchIndicatorLedLow = gpioController.Read(pinValueChangedEventArgs.PinNumber);
                Console.WriteLine(DateTime.Now + " " + pinValueChangedEventArgs.PinNumber + " pin = " + pinvalueFloatSwitchIndicatorLedLow);

                try
                {
                    //PinValue FloatSwitchStatus = sender.Read(); //"Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString()

                    // StrFloatSwitchStatusMsg = "Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString();
                    // TextBlockString = StrFloatSwitchStatusMsg;

                    if (pinvalueFloatSwitchIndicatorLedLow == PinValue.High)  // ***  Floatswitch Open
                    {
                        //GpioMsg = "PIR_Activated";
                        //PirLEDStatus = true;
                        //BtnToggleLowIsChecked = false;
                        //gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.Low);
                        //gpioController.Write((int)sender, PinValue.Low);   // ***  Reason Floatswitch is Open

                    }
                    else                                    // *** Floatswitch Closed
                    {
                        //BtnToggleLowIsChecked = true;
                        //gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.High);
                        //gpioController.Write((int)sender, PinValue.High);  // *** Reason Floatswitch Close

                    }
                }
                //await SendDeviceToCloudMessagesAsync();
                catch (Exception ex)
                {
                    // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    // () =>
                    // {
                    Debug.WriteLine(ex.Message);
                    // });
                }
            }
            catch (Exception ex)
            {
                // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                // () =>
                // {
                Debug.WriteLine(ex.Message);
                // });
            }
            finally
            {
                //Turn the LED Status Light on - we're ready for another picture
                //LightPIR_LED(false);
            }
            //throw new NotImplementedException();
        }

        private void Pin_FloatSwitchMedium_ValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            try
            {
                PinValueFloatSwitchMedium = gpioController.Read(pinValueChangedEventArgs.PinNumber);
                Console.WriteLine(DateTime.Now + " " + pinValueChangedEventArgs.PinNumber + " pin = " + PinValueFloatSwitchMedium);

                try
                {
                    //PinValue FloatSwitchStatus = sender.Read(); //"Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString()

                    // StrFloatSwitchStatusMsg = "Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString();
                    // TextBlockString = StrFloatSwitchStatusMsg;

                    if (PinValueFloatSwitchMedium == PinValue.High)  // ***  Floatswitch Open
                    {
                        //GpioMsg = "PIR_Activated";
                        //PirLEDStatus = true;
                        BtnToggleMiddleIsChecked = false;
                        gpioController.Write(pin_FloatSwitchIndicatorLedMedium, PinValue.Low);
                        //gpioController.Write((int)sender, PinValue.Low);   // ***  Reason Floatswitch is Open

                    }
                    else                                    // *** Floatswitch Closed
                    {
                        BtnToggleMiddleIsChecked = true;
                        gpioController.Write(pin_FloatSwitchIndicatorLedMedium, PinValue.High);
                        //gpioController.Write((int)sender, PinValue.High);  // *** Reason Floatswitch Close

                    }
                }
                //await SendDeviceToCloudMessagesAsync();
                catch (Exception ex)
                {
                    // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    // () =>
                    // {
                    Debug.WriteLine(ex.Message);
                    // });
                }
            }
            catch (Exception ex)
            {
                // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                // () =>
                // {
                Debug.WriteLine(ex.Message);
                // });
            }
            finally
            {
                //Turn the LED Status Light on - we're ready for another picture
                //LightPIR_LED(false);
            }
            //throw new NotImplementedException();

        }

        private void Pin_FloatSwitchIndicatorLedMedium_ValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            try
            {
                pinvalueFloatSwitchIndicatorLedMedium = gpioController.Read(pinValueChangedEventArgs.PinNumber);
                Console.WriteLine(DateTime.Now + " " + pinValueChangedEventArgs.PinNumber + " pin = " + pinvalueFloatSwitchIndicatorLedMedium);

                try
                {
                    //PinValue FloatSwitchStatus = sender.Read(); //"Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString()

                    // StrFloatSwitchStatusMsg = "Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString();
                    // TextBlockString = StrFloatSwitchStatusMsg;

                    if (pinvalueFloatSwitchIndicatorLedMedium == PinValue.High)  // ***  Floatswitch Open
                    {
                        //GpioMsg = "PIR_Activated";
                        //PirLEDStatus = true;
                        //BtnToggleLowIsChecked = false;
                        //gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.Low);
                        //gpioController.Write((int)sender, PinValue.Low);   // ***  Reason Floatswitch is Open

                    }
                    else                                    // *** Floatswitch Closed
                    {
                        //BtnToggleLowIsChecked = true;
                        //gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.High);
                        //gpioController.Write((int)sender, PinValue.High);  // *** Reason Floatswitch Close

                    }
                }
                //await SendDeviceToCloudMessagesAsync();
                catch (Exception ex)
                {
                    // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    // () =>
                    // {
                    Debug.WriteLine(ex.Message);

                    // });
                }
            }
            catch (Exception ex)
            {
                // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                // () =>
                // {
                Debug.WriteLine(ex.Message);
                // });
            }
            finally
            {
                //Turn the LED Status Light on - we're ready for another picture
                //LightPIR_LED(false);
            }
            throw new NotImplementedException();
        }

        private void Pin_FloatSwitchHigh_ValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            try
            {
                PinValueFloatSwitchHigh = gpioController.Read(pinValueChangedEventArgs.PinNumber);
                Console.WriteLine(DateTime.Now + " " + pinValueChangedEventArgs.PinNumber + " pin = " + PinValueFloatSwitchHigh);

                try
                {
                    //PinValue FloatSwitchStatus = sender.Read(); //"Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString()

                    // StrFloatSwitchStatusMsg = "Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString();
                    // TextBlockString = StrFloatSwitchStatusMsg;

                    if (PinValueFloatSwitchHigh == PinValue.High)  // ***  Floatswitch Open
                    {
                        //GpioMsg = "PIR_Activated";
                        //PirLEDStatus = true;
                        BtnToggleHighIsChecked = false;
                        gpioController.Write(pin_FloatSwitchIndicatorLedHigh, PinValue.Low);
                        //gpioController.Write((int)sender, PinValue.Low);   // ***  Reason Floatswitch is Open

                    }
                    else                                    // *** Floatswitch Closed
                    {
                        BtnToggleHighIsChecked = true;
                        gpioController.Write(pin_FloatSwitchIndicatorLedHigh, PinValue.High);
                        //gpioController.Write((int)sender, PinValue.High);  // *** Reason Floatswitch Close

                    }
                }
                //await SendDeviceToCloudMessagesAsync();
                catch (Exception ex)
                {
                    // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    // () =>
                    // {
                    Debug.WriteLine(ex.Message);
                    // });
                }
            }
            catch (Exception ex)
            {
                // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                // () =>
                // {
                Debug.WriteLine(ex.Message);
                // });
            }
            finally
            {
                //Turn the LED Status Light on - we're ready for another picture
                //LightPIR_LED(false);
            }
            //throw new NotImplementedException();
        }

        private void Pin_FloatSwitchIndicatorLedHigh_ValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            try
            {
                pinvalueFloatSwitchIndicatorLedHigh = gpioController.Read(pinValueChangedEventArgs.PinNumber);
                Console.WriteLine(DateTime.Now + " " + pinValueChangedEventArgs.PinNumber + " pin = " + pinvalueFloatSwitchIndicatorLedHigh);

                try
                {
                    //PinValue FloatSwitchStatus = sender.Read(); //"Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString()

                    // StrFloatSwitchStatusMsg = "Float Switch High Status: " + FloatSwitchStatus.ToString() + "; Time: " + DateTime.Now.ToString();
                    // TextBlockString = StrFloatSwitchStatusMsg;

                    if (pinvalueFloatSwitchIndicatorLedHigh == PinValue.High)  // ***  Floatswitch Open
                    {
                        //GpioMsg = "PIR_Activated";
                        //PirLEDStatus = true;
                        //BtnToggleLowIsChecked = false;
                        //gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.Low);
                        //gpioController.Write((int)sender, PinValue.Low);   // ***  Reason Floatswitch is Open

                    }
                    else                                    // *** Floatswitch Closed
                    {
                        //BtnToggleLowIsChecked = true;
                        //gpioController.Write(pin_FloatSwitchIndicatorLedLow, PinValue.High);
                        //gpioController.Write((int)sender, PinValue.High);  // *** Reason Floatswitch Close

                    }
                }
                //await SendDeviceToCloudMessagesAsync();
                catch (Exception ex)
                {
                    // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    // () =>
                    // {
                    Debug.WriteLine(ex.Message);
                    // });
                }
            }
            catch (Exception ex)
            {
                // await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                // () =>
                // {
                Debug.WriteLine(ex.Message);
                // });
            }
            finally
            {
                //Turn the LED Status Light on - we're ready for another picture
                //LightPIR_LED(false);
            }
            throw new NotImplementedException();
        }

        #endregion

        public async Task<Task> FloatSwitchStatusAlertWatch()
        {
            bool AlertTriggeredReset = false;
            bool AlertTriggeredMiddle = false;
            bool AlertTriggeredTop = false;
            bool AlertTriggeredErr = false;

            //int AlertDailyCounterLow = 0;
            int AlertDailyCounterReset = 0;
            int AlertDailyCounterLow = 0;
            int AlertDailyCounterMiddle = 0;
            int AlertDailyCounterHigh = 0;
            int AlertDailyCounterErr = 0;
            bool isParsable;
            int StrToInt;

            // Define dates.
            DateTime SwitchEventTime;
            DateTime SwitchEventAlertTimeReset = DateTime.Now;
            DateTime SwitchEventAlertTimeTop = DateTime.Now;
            DateTime SwitchEventAlertTimeMiddle = DateTime.Now;
            DateTime SwitchEventAlertTimeErr = DateTime.Now;

            AlertTriggeredTimeReset = "";
            AlertTriggeredTimeMiddle = "";
            AlertTriggeredTimeTop = "";
            AlertTriggeredTimeErr = "";

            while (true)
            {
                try
                {
                    SwitchEventTime = DateTime.Now;

                    if (BtnToggleMiddleIsChecked == false & BtnToggleHighIsChecked == false)
                    {
                        // Condition1 is true.
                        // Sub Pump is working

                        StrFloatSwitchStatusMsg = "All Float Switches, Low Time: " + DateTime.Now.ToString();
                        TextBlockString = StrFloatSwitchStatusMsg;

                        if (AlertTriggeredMiddle == true | AlertTriggeredTop == true | AlertTriggeredErr == true)
                        {
                            AlertTriggeredMiddle = false;
                            AlertTriggeredTop = false;
                            AlertTriggeredErr = false;

                            //SwitchEventAlertTimeReset = SwitchEventTime;
                            //if(AlertTriggeredReset == false) { AlertTriggeredTimeResetLast = AlertTriggeredTimeReset; }
                            SwitchEventAlertTimeReset = SwitchEventTime;
                            AlertTriggeredReset = true;
                            AlertTriggeredTimeResetLast = AlertTriggeredTimeReset;
                            AlertDailyCounterReset += 1;

                            AlertTriggeredTimeReset = SwitchEventAlertTimeReset.ToString();

                            SendEventsToTopic();
                        }
                        else
                        {
                            isParsable = Int32.TryParse(SwitchEventAlertTimeDifferenceReset, out StrToInt);
                            isParsable = false; // remove this line after interval.minutes is greater than 60 minutes is fighured out

                            if (isParsable)
                            {
                                //SwitchEventAlertTimeMiddle = new DateTime();

                                TimeSpan interval = SwitchEventTime - SwitchEventAlertTimeReset;
                                if (interval.Minutes >= StrToInt)
                                {

                                    // this needs worked when interval.minutes is greater than 60 minutes 
                                    //AlertTriggeredReset = false;
                                }

                                if (AlertTriggeredReset == false)
                                {
                                    AlertTriggeredReset = true;
                                    SwitchEventAlertTimeReset = SwitchEventTime;
                                    AlertTriggeredTimeResetLast = AlertTriggeredTimeReset;
                                    AlertDailyCounterReset += 1;

                                    AlertTriggeredTimeReset = SwitchEventAlertTimeReset.ToString();
                                    SendEventsToTopic();
                                }

                            }
                        }

                    }

                    if (BtnToggleMiddleIsChecked == true & BtnToggleHighIsChecked == false)
                    {
                        // Condition1 is false and Condition2 is true.
                        // Float switch Medium appears to have fault and Float Switch Hign sees potential flooding
                        // Send Alarm - Total Failue

                        StrFloatSwitchStatusMsg = "Float switch Medium appears to indicate potential flooding;  Time: " + DateTime.Now.ToString();
                        TextBlockString = StrFloatSwitchStatusMsg;

                        isParsable = Int32.TryParse(SwitchEventAlertTimeDifferenceMiddle, out StrToInt);
                        if (isParsable)
                        {

                            TimeSpan interval = SwitchEventTime - SwitchEventAlertTimeMiddle;
                            if (interval.Minutes >= StrToInt)
                            {
                                AlertTriggeredMiddle = false;
                            }
                            if (AlertTriggeredMiddle == false)
                            {
                                AlertTriggeredMiddle = true;
                                SwitchEventAlertTimeMiddle = SwitchEventTime;
                                AlertTriggeredTimeMiddleLast = AlertTriggeredTimeMiddle;
                                AlertDailyCounterMiddle += 1;

                                AlertTriggeredTimeMiddle = SwitchEventAlertTimeMiddle.ToString();
                                SendEventsToTopic();
                            }

                        }
                    }

                    if (BtnToggleMiddleIsChecked == true & BtnToggleHighIsChecked == true)
                    {
                        // Condition1 and Condition2 are false. Condition3 and Condition4 are true.
                        // Float switch Medium indicationg Initial failure
                        // Send Alarm - Initial Failue

                        StrFloatSwitchStatusMsg = "Float switch Medium & High are indicating Major flooding;  Time: " + DateTime.Now.ToString();
                        TextBlockString = StrFloatSwitchStatusMsg;

                        isParsable = Int32.TryParse(SwitchEventAlertTimeDifferenceTop, out StrToInt);
                        if (isParsable)
                        {
                            TimeSpan interval = SwitchEventTime - SwitchEventAlertTimeTop;
                            if (interval.Minutes >= StrToInt)
                            {
                                AlertTriggeredTop = false;
                            }
                            if (AlertTriggeredTop == false)
                            {
                                AlertTriggeredTop = true;
                                SwitchEventAlertTimeTop = SwitchEventTime;
                                AlertTriggeredTimeTopLast = AlertTriggeredTimeTop;
                                AlertDailyCounterHigh += 1;

                                AlertTriggeredTimeTop = SwitchEventAlertTimeTop.ToString();
                                SendEventsToTopic();
                            }

                        }
                    }

                    if (BtnToggleMiddleIsChecked == false & BtnToggleHighIsChecked == true)
                    {
                        // Condition1, Condition2, and Condition3 are false.
                        StrFloatSwitchStatusMsg = "Not Sure here;  Time: " + DateTime.Now.ToString();
                        TextBlockString = StrFloatSwitchStatusMsg;

                        isParsable = Int32.TryParse(SwitchEventAlertTimeDifferenceHighErr, out StrToInt);
                        if (isParsable)
                        {
                            TimeSpan interval = SwitchEventTime - SwitchEventAlertTimeErr;
                            if (interval.Minutes >= StrToInt)
                            {
                                AlertTriggeredErr = false;
                            }

                            if (AlertTriggeredErr == false)
                            {
                                AlertTriggeredErr = true;
                                SwitchEventAlertTimeErr = SwitchEventTime;
                                AlertTriggeredTimeErrLast = AlertTriggeredTimeErr;
                                AlertDailyCounterErr += 1;

                                AlertTriggeredTimeErr = SwitchEventAlertTimeErr.ToString();
                                SendEventsToTopic();
                            }
                        }
                    }

                    if (YesterdayDay != SwitchEventTime.Day.ToString() & "8" != SwitchEventTime.Hour.ToString()) // 'Hour' defaults to 24-hour clock0800 AM 
                    {   // Send daily Alert Status Count Email
                        YesterdayDay = SwitchEventTime.Day.ToString();

                        StrFloatSwitchStatusMsg = "Daily Report;  Time: " + DateTime.Now.ToString();
                        TextBlockString = StrFloatSwitchStatusMsg;

                        AlertDailyCounterReset = 0;
                        AlertDailyCounterLow = 0;
                        AlertDailyCounterMiddle = 0;
                        AlertDailyCounterHigh = 0;
                        AlertDailyCounterErr = 0;

                        SendEventsToTopic();

                    }

                }
                catch (Exception)
                {
                    throw;
                }

                finally
                {
                    Console.WriteLine("FloatSwitchStatusAlertWatch - Still Running " + DateTime.Now.ToString());
                    //Task task3 = InitializeDeviceState();


                    // this task runs all the time with Delay as defined below
                    //Thread.Sleep((Int32)(MyTaskDelay * 1000));
                    await Task.Delay((Int32)(MyTaskDelay * 1000));//
                }

            }

        }

        /// <summary>
        /// Send events to Event Grid Topic.
        /// </summary>
        public void SendEventsToTopic()
        {
            string msgEmailSub = (StrFloatSwitchStatusMsg.Length > 0) ? StrFloatSwitchStatusMsg : "This email is being generated via a Test Event from the application";

            //List<UserDetails> details = new List<UserDetails>();
            List<EventGridEvent> eventsList = new();
            eventsList.Add(new EventGridEvent()
            {

                Id = Guid.NewGuid().ToString(),
                EventType = "Subpump.Status.Alert",
                Data = new ContosoItemReceivedEventData()
                {
                    Emailbody = DateTime.Now.ToString("HH:mm:ss") + msgEmailSub,
                    Emailsubject = FloatSwitchEmailMsg.Length > 0 ? FloatSwitchEmailMsg : "FloatSwitch Status Warning",
                    //emaildistribution = "eagan_mike@hotmail.com; 8124994801@txt.att.net; 8126987932@vtext.net"
                    Emaildistribution = EmailDistributionGroup //"eagan_mike@hotmail.com; 8124994801@txt.att.net"
                },
                EventTime = DateTime.Now,
                Subject = "FloatSwitch",
                DataVersion = "2.0"
            });

            //userdetails user = new userdetails();
            //details.Add(new userdetails { emailsubject = "FlostSwitch Status Warning", emailbody = FloatSwitchEmailMsg, eventtime = DateTime.Now.ToString() });
            FloatSwitchEmailMsg = ""; //JsonConvert.SerializeObject(details);


            client.PublishEventsAsync(topicHostname, eventsList).GetAwaiter().GetResult();
            Console.Write("SendEventsToTopic::Published events to Event Grid topic. " + DateTime.Now.ToString());
            Console.Write(StrFloatSwitchStatusMsg);
            //Console.ReadLine();

            //var telemetryDataPoint = new
            //{
            //    emailbody = "This is a message within my Float Switch Email Warning",
            //    emailsubject = "FlostSwitch",
            //    //eventtime = DateTime.Now.ToString()
            //};
            //var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            //var message = new Message(Encoding.ASCII.GetBytes(messageString));


            //// Create a HTTP client which we will use to post to the Event Grid Topic
            //var httpClient = new HttpClient();

            //// Add key in the request headers
            //httpClient.DefaultRequestHeaders.Add("aeg-sas-key", KEY);

            //// Event grid expects event data as JSON
            ////string json = JsonConvert.SerializeObject(FloatSwitchEmailMsg);

            //// Create request which will be sent to the topic
            ////StringContent content = new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json");
            ////var content = new StringContent(JsonConvert.SerializeObject(telemetryDataPoint), Encoding.UTF8, "application/json");
            //var content = new StringContent(JsonConvert.SerializeObject(telemetryDataPoint), Encoding.UTF8);

            //// Send request
            //Console.WriteLine("Sending event to Event Grid...");
            //HttpResponseMessage result = await httpClient.PostAsync(TOPIC_ENDPOINT, content);

            //// Show result
            //Console.WriteLine($"Event sent with result: {result.ReasonPhrase}");
            //Console.WriteLine();


        }

        #region PropertChangeEvents

        void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            //Test Comment no meaing at all
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private void PinChangeEventHandler(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            Debug.Write($"Interrupt Thread:{Thread.CurrentThread.ManagedThreadId}");


            if (pinValueChangedEventArgs.ChangeType == PinEventTypes.Rising)
            {

#if DEBUG

                if (Gpio.Read((int)sender) == PinValue.Low)
                {
                    Gpio.Write((int)sender, PinValue.High);
                    //Gpio..Write(pin, ((ledOn) ? PinValue.High : PinValue.Low));
                }
                else
                {
                    Gpio.Write((int)sender, PinValue.Low);
                }
                Thread.Sleep(1000);  //Provide time for IO status to Change
#endif

            }
            throw new NotImplementedException();
        }

        //private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        //{
        //    Console.WriteLine("\tDesired property changed:");
        //    Console.WriteLine($"\t{desiredProperties.ToJson()}");

        //    Console.WriteLine("\tSending current time as reported property");
        //    TwinCollection reportedProperties = new();
        //    reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now;

        //    await myDeviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        //}

        //static IList<EventGridEvent> GetEventsList()
        //{
        //    List<EventGridEvent> eventsList = new List<EventGridEvent>();

        //    for (int i = 0; i < 2; i++)
        //    {
        //        eventsList.Add(new EventGridEvent()
        //        {

        //            Id = Guid.NewGuid().ToString(),
        //            EventType = "Contoso.Items.ItemReceived",
        //            Data = new ContosoItemReceivedEventData()
        //            {
        //                emailbody = "Contoso Item SKU #1",
        //                emailsubject = "Email Subject"
        //            },
        //            EventTime = DateTime.Now,
        //            Subject = "Door1",
        //            DataVersion = "2.0"
        //        });
        //    }

        //    return eventsList;
        //}

        #endregion

        //public async Task SendDeviceToCloudMessagesAsync()
        public async Task<Task> SendDeviceToCloudMessagesAsync()
        {
            try
            {
                //Task task3 = InitializeDeviceState();

                var telemetryDataPoint = new
                {
                    deviceId = DeviceId,
                    //message = "Hello"
                    message = GpioMsg
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

                // Twin twin = await myDeviceClient.GetTwinAsync();
                //twin.Properties.Desired
                //twin.Properties.Desired["mike"] = "Test";

                //RegistryManager registryManager = Microsoft.Azure.Devices.RegistryManager.CreateFromConnectionString(connectionString);
                //  registryManager.UpdateTwins2Async()

                //Device device = registryManager.GetDeviceAsync(deviceId).Result;
                //Twin deviceTwin = await registryManager.GetTwinAsync(deviceId);

                //device.Status = DeviceStatus.Enabled;
                //device.ETag = deviceTwin.ETag;
                //Device result = await registryManager.UpdateDeviceAsync(device, true);

                await myDeviceClient.SendEventAsync(message);
                Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                Console.WriteLine("SendDeviceToCloudMessagesAsync");

                IsAzureConnected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        public async Task RunSampleAsync()
        {
            //        await DeviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null).ConfigureAwait(false);

            Console.WriteLine("Retrieving twin...");
            Twin twin = await myDeviceClient.GetTwinAsync().ConfigureAwait(false);

            Console.WriteLine("\tInitial twin value received:");
            Console.WriteLine($"\t{twin.ToJson()}");

            Console.WriteLine("Sending sample start time as reported property");
            TwinCollection reportedProperties = new();
            reportedProperties["DateTimeLastAppLaunch"] = DateTime.Now;

            await myDeviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);

            Console.WriteLine("Waiting 30 seconds for IoT Hub Twin updates...");
            Console.WriteLine($"Use the IoT Hub Azure Portal to change the Twin desired properties within this time.");


            Thread.Sleep(30 * 1000);
            //await Task.Delay(30 * 1000);
        }

        public async Task<Task> ReceiveCloudToDeviceMessageAsync()
        {
            CloudToDeviceLog = "Receiving events...";
            Debug.WriteLine("\nReceiving cloud to device messages from service");

            int ButtonPinNumber = 5;
            int LedPinNumber = 16;

            while (true)
            {
                //myDeviceClient.GetTwinAsync()
                Microsoft.Azure.Devices.Client.Message receivedMessage = await myDeviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                CloudToDeviceLog += "\nReceived message: " + msg;

                if (msg == "on")
                {
#if DEBUG
                    Gpio.Write(pin_LedPin, PinValue.Low);
                    //LedPin.Write(PinValue.Low);
#endif
                }

                if (msg == "off")
                {
#if DEBUG
                    Gpio.Write(pin_LedPin, PinValue.High);
                    //LedPin.Write(PinValue.High);
#endif
                }

                if (msg == "PIR_Activated")
                {
#if DEBUG
                    Gpio.Write(pin_PIR_Pin, PinValue.High);
                    //PIR_Pin.Write(PinValue.High);
#endif
                }

                if (msg == "PIR_Deactivated")
                {
#if DEBUG
                    Gpio.Write(pin_PIR_Pin, PinValue.Low);
                    //PIR_Pin.Write(PinValue.Low);
#endif
                }
                Console.WriteLine("ReceiveCloudToDeviceMessageAsync - Still Running");
                await myDeviceClient.CompleteAsync(receivedMessage);
            }


        }

        ///This captures the "Data" portion of an EventGridEvent on a custom topic
        class ContosoItemReceivedEventData
        {
            private string emailsubject;
            private string emailbody;
            private string emaildistribution1;

            [JsonProperty(PropertyName = "emailbody")]
            public string Emailbody { get => emailbody; set => emailbody = value; }

            [JsonProperty(PropertyName = "emailsubject")]
            public string Emailsubject { get => emailsubject; set => emailsubject = value; }

            [JsonProperty(PropertyName = "emaildistribution")]
            public string Emaildistribution { get => emaildistribution1; set => emaildistribution1 = value; }

        }

        /// Simple business object. A PartId is used to identify the type of part 
        /// but the part name can change. 
        class Userdetails
        {
            private string emailsubject;
            private string emailbody;
            private string eventtime;
            public string Emailsubject { get => emailsubject; set => emailsubject = value; }
            public string Emailbody { get => emailbody; set => emailbody = value; }
            public string Eventtime { get => eventtime; set => eventtime = value; }
        }

    }


}

