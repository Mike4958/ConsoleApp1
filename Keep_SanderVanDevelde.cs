namespace ConsoleApp1
{
    internal class Keep_SanderVanDevelde
    {
    }

    //private static void ExampleMain(string[] args)
    //{

    //    //// Set various callback messages: connection status, device twin, a single named method, multiple methods
    //    SetVariousCallbackMethods(deviceClient);

    //    //// open connection explicitly
    //    deviceClient.OpenAsync().Wait();

    //    //// Force the retrieval of the devicetwin
    //    ForceDeviceTwinRetrieval(deviceClient);

    //    //// send a single message
    //    SendSingleMessage(deviceClient);

    //    //// send multiple message in batch in one call
    //    SendBatchOfMessages(deviceClient);

    //    //// Send a file to the Blob storage configured in the IoT Hub
    //    SendFileToBlobStorage(deviceClient);

    //    //// Stall application closing
    //    Console.WriteLine("Press a key to exit...");
    //    Console.ReadKey();
    //}


    //private static void SetVariousCallbackMethods(DeviceClient deviceClient)
    //{
    //    deviceClient.SetConnectionStatusChangesHandler(ConnectionStatusChangeHandler);

    //    deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, deviceClient).ConfigureAwait(false);

    //    deviceClient.SetMethodHandlerAsync("Ping", HandleMethodPing, deviceClient).ConfigureAwait(false);

    //    deviceClient.SetMethodDefaultHandlerAsync(HandleDefaultMethods, deviceClient).ConfigureAwait(false);

    //        //// Check for COMMAND messages (Warning: BLOCKING calls); Do not forget to return a 'Complete' or 'reject' or 'abandon'
    //        Thread thread = new Thread(() => ThreadBodyCommand(deviceClient));
    //    thread.Start();

    //    Console.WriteLine("Callback methods are set");
    //}

    //private static void ConnectionStatusChangeHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
    //{
    //    Console.WriteLine($"Connection Status Changed to {status} ({reason}) at {DateTime.Now}");
    //}

    //private static async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
    //{
    //    Console.WriteLine("One or more device twin desired properties changed:");
    //    Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

    //    var client = userContext as DeviceClient;

    //    var reportedProperties = new TwinCollection
    //    {
    //        ["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now
    //    };

    //    await client.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);

    //    Console.WriteLine("Sent current time as reported property to device twin");
    //}

    //private static Task<MethodResponse> HandleMethodPing(MethodRequest methodRequest, object userContext)
    //{
    //    Console.WriteLine($"method {methodRequest.Name} handled with body {methodRequest.DataAsJson}");

    //    return Task.FromResult(new MethodResponse(new byte[0], 200));
    //}

    //private static Task<MethodResponse> HandleDefaultMethods(MethodRequest methodRequest, object userContext)
    //{
    //    Console.WriteLine($"Default method {methodRequest.Name} handled with body {methodRequest.DataAsJson}");

    //    return Task.FromResult(new MethodResponse(new byte[0], 200));
    //}

    //private static async void ThreadBodyCommand(object deviceClient)
    //{
    //    var client = deviceClient as DeviceClient;

    //    Console.WriteLine("Waiting for C2D messages (aka commands)");

    //    while (true)
    //    {
    //        // The following line is blocking until a timeout occurs
    //        using var message = await client.ReceiveAsync();

    //        if (message == null)
    //        {
    //            Console.WriteLine("Timeout. Command is null");
    //            continue;
    //        }

    //        string data = Encoding.UTF8.GetString(message.GetBytes());

    //        Console.WriteLine($"A message is received with body {data}");

    //        await client.CompleteAsync(message); // mark the message as handled
    //        //await client.RejectAsync(message); // drops the message as unhandled
    //        //await client.AbandonAsync(message); // puts message back on queue
    //    }
    //}

    //private static void ForceDeviceTwinRetrieval(DeviceClient deviceClient)
    //{
    //    //// Force the retrieval and handling of the desired twin properties

    //    var twin = deviceClient.GetTwinAsync().Result;

    //    OnDesiredPropertyChanged(twin.Properties.Desired, deviceClient).Wait();

    //    Console.WriteLine("The Devicetwin is forced retrieved");
    //}

    //private static void SendSingleMessage(DeviceClient deviceClient)
    //{
    //    string jsonData = "{ \"single\":true }";

    //    using var message = new Message(Encoding.UTF8.GetBytes(jsonData));

    //    message.Properties.Add("messagetype", "normal");

    //    deviceClient.SendEventAsync(message).Wait();

    //    Console.WriteLine("A single message is sent");
    //}

    //private static void SendBatchOfMessages(DeviceClient deviceClient)
    //{
    //    string jsonDataOne = "{ \"number\": \"one\" }";
    //    using var messageOne = new Message(Encoding.UTF8.GetBytes(jsonDataOne));
    //    messageOne.Properties.Add("batch", "true");

    //    string jsonDataTwo = "{ \"number\": \"two\" }";
    //    using var messageTwo = new Message(Encoding.UTF8.GetBytes(jsonDataTwo));
    //    messageTwo.Properties.Add("batch", "true");

    //    deviceClient.SendEventBatchAsync(new List<Message> { messageOne, messageTwo }).Wait();

    //    Console.WriteLine("Messages are sent in batch");
    //}

}
