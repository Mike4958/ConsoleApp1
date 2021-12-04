using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Class2
    {

        public Class2()
        {
            //IOT Device
            //And add the following code to the constructor.
            BackgroundWorker backgroundWorker = new();
            // Hide this line from ERROR -------backgroundWorker.DoWork += ReceiveFeedback;
            backgroundWorker.RunWorkerAsync();
                     
        }

        // 
        public void MyExampleMain()
        {
            //Application
            //And finally, we need to map the SoundAlarm method to the incoming remote method call. To do this, add the following line in the Main method.
            // Hide this line from ERROR -------client.SetMethodHandlerAsync("SoundAlarm", SoundAlarm, null);




        }

        ////And add the following code to the constructor.
        //var backgroundWorker = new BackgroundWorker();
        //backgroundWorker.DoWork += ReceiveFeedback;
        //backgroundWorker.RunWorkerAsync();


        //Applicatiom
        //Cloud to Device Messaging
        //When we want to send one-way notifications or commands to our devices, we can use cloud to device messages.To do this, we will expand on the EngineManagement application we created in our earlier posts, by adding the following controls, which, in our scenario, will allow us to start the fans of the selected engine.
        
        // Hide this line from ERROR -------private readonly ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=youriothubname.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=yoursharedaccesskey");
        //private async void ButtonStartFans_Click(object sender, EventArgs e)

        //{
        //    var message = new Microsoft.Azure.Devices.Message();

        //    message.Properties.Add(new KeyValuePair<string, string>("StartFans", "true"));
        //    // Hide this line from ERROR -------message.Ack = DeliveryAcknowledgement.Full; // Used for getting delivery feedback

        //    // Hide this line from ERROR -------await serviceClient.SendAsync(comboBoxSerialNumber.Text, message);

        //}


        //IOT Device
        //Process Message On Device
        //Once we have sent our message, we will need to process it on our device.For this, we are going to update the client application of our simulated engine by adding the following method.
        //private static async void ReceiveMessageFromCloud(object sender, DoWorkEventArgs e)

        //{
        //    // Continuously wait for messages
        //    while (true)
        //    {
        //        // Hide this line from ERROR -------var message = await client.ReceiveAsync();
        //        // Check if message was received
        //        if (message == null)
        //        {
        //            continue;
        //        }
        //        try
        //        {
        //            if (message.Properties.ContainsKey("StartFans") && message.Properties["StartFans"] == "true")
        //            {
        //                // This would start the fans
        //                Console.WriteLine("Fans started!");
        //            }
        //            // Hide this line from ERROR -------await client.CompleteAsync(message);
        //        }
        //        catch (Exception)
        //        {
        //            // Send to deadletter
        //            // Hide this line from ERROR ------- await client.RejectAsync(message);
        //        }
        //    }
        //}

        //Application
        //Message Feedback
        //Although cloud to device messages are a one-way communication style, we can request feedback on the delivery of the message, allowing us to invoke retries or start compensation when the message fails to be delivered.To do this, implement the following method in our EngineManagement backend application.
        //private async void ReceiveFeedback(object sender, DoWorkEventArgs e)
        //{
        //    var feedbackReceiver = serviceClient.GetFeedbackReceiver();
        //    while (true)
        //    {
        //        var feedbackBatch = await feedbackReceiver.ReceiveAsync();
        //        // Check if feedback messages were received
        //        if (feedbackBatch == null)
        //        {
        //            continue;
        //        }
        //        // Loop through feedback messages
        //        foreach (var feedback in feedbackBatch.Records)
        //        {
        //            // Hide this line from ERROR -------if (feedback.StatusCode != FeedbackStatusCode.Success)
        //            // Hide this line from ERROR -------{
        //            // Handle compensation here
        //            // Hide this line from ERROR -------}

        //        }
        //        await feedbackReceiver.CompleteAsync(feedbackBatch);
        //    }
        //}



        // Application
        // Call Remote Method
        // Another feature when sending messages from the cloud to our devices is to call a remote method on the device, which we call invoking a direct method.This type of communication is used when we want to have an immediate confirmation of the outcome of the command (unlike setting the desired state and communicating back reported properties, which has been explained in the previous two blog posts). Let’s update the EngineManagement application by adding the following controls, which would allow us to send an alarm message to the engine, sounding the alarm and displaying a message.

        //Now add the following event handler for clicking the Send Alarm button.
        //private async void ButtonSendAlarm_Click(object sender, EventArgs e)

        //{
        //    // Hide this line from ERROR -------var methodInvocation = new CloudToDeviceMethod("SoundAlarm") { ResponseTimeout = TimeSpan.FromSeconds(300) };

        //    // Hide this line from ERROR -------methodInvocation.SetPayloadJson(JsonConvert.SerializeObject(new { message = textBoxMessage.Text }));
        //    // Hide this line from ERROR -------CloudToDeviceMethodResult response = null;

        //    try
        //    {
        //        // Hide this line from ERROR -------response = await serviceClient.TcpClient (comboBoxSerialNumber.Text, methodInvocation);
        //        // Hide this line from ERROR ------- Or
        //        // Hide this line from ERROR ------- Invoke the direct method asynchronously and get the response from the simulated device.
        //        // Hide this line from ERROR ------- https://github.com/Azure-Samples/azure-iot-samples-csharp/blob/main/iot-hub/Quickstarts/InvokeDeviceMethod/Program.cs
        
        // var response = await s_serviceClient.InvokeDeviceMethodAsync("MyDotnetDevice", methodInvocation);

        //    }
        //    catch (IotHubException)
        //    {
        //        // Do nothing
        //    }

        //    // Hide this line from ERROR -------
        //    // Hide this line from ERROR ------- if (response != null(&) & JObject.Parse( response.GetPayloadAsJson()).GetValue("acknowledged").Value<bool>())
        //    // Hide this line from ERROR -------{
        //    // Hide this line from ERROR -------MessageBox.Show("Message was acknowledged.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    // Hide this line from ERROR -------}
        //    // Hide this line from ERROR -------else
        //    // Hide this line from ERROR -------{
        //    // Hide this line from ERROR -------MessageBox.Show("Message was not acknowledged!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    // Hide this line from ERROR -------}
        //}

        //IOT Device
        ////And in our simulated device, implement the SoundAlarm remote method which is being called.
        //private static Task<MethodResponse> SoundAlarm(MethodRequest methodRequest, object userContext)

        //{
        //    // On a real engine this would sound the alarm as well as show the message
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.WriteLine($"Alarm sounded with message: {JObject.Parse(methodRequest.DataAsJson).GetValue("message").Value<string>()}! Type yes to acknowledge.");
        //    Console.ForegroundColor = ConsoleColor.White;

        //    var response = JsonConvert.SerializeObject(new { acknowledged = Console.ReadLine() == "yes" });

        //    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(response), 200));

        //}






    }
}
