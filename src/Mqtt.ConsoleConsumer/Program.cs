using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mqtt.ConsoleConsumer
{
    class Program
    {
        private static IManagedMqttClient _mqttClient;

        static async Task Main(string[] args)
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("Ali_Consumer")
                    .WithCredentials("testuser", "testpass")
                    .WithTcpServer("www.baltavista.com", 8883)
                    .WithCleanSession(true)
                    .Build())
                .Build();

            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            await _mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("test1").Build());
            await _mqttClient.StartAsync(options);
            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"({DateTime.Now}):{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

            Console.WriteLine("-- MQTT Consumer started ---");
            Console.ReadLine();
        }

    }
}
