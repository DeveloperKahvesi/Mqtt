using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mqtt.ConsolePublisher
{
    class Program
    {
        private static IManagedMqttClient _mqttClient;

        static async Task Main(string[] args)
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(
                    new MqttClientOptionsBuilder()
                    .WithClientId("AliPublisher")
                    .WithCredentials("testuser", "testpass")
                    .WithTcpServer("www.baltavista.com", 8883)
                    .WithCleanSession(true)
                    .Build())
                .Build();

            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            await _mqttClient.StartAsync(options);

            Console.WriteLine("-- MQTT Producer started -- ");

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var keyBoardTask = Task.Run(() =>
                {
                    Console.WriteLine("Press enter to cancel");
                    Console.ReadKey();
                    cancellationTokenSource.Cancel();
                });

                try
                {
                    var result = PublishAsync(cancellationTokenSource.Token, 1000);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Task was cancelled");
                }
                await keyBoardTask;
            }

        }


        static Task PublishAsync(CancellationToken token, int sleepMiliseconds = 1000)
        {
            Task task = null;
            task = Task.Run(() =>
            {
                var rnd = new Random();


                while (true)
                {
                    if (token.IsCancellationRequested)
                        throw new TaskCanceledException(task);

                    if (!_mqttClient.IsStarted || !_mqttClient.IsConnected)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    var payload1 = $"{DateTime.Now} : {rnd.Next(0, 50)}";
                    var payload2 = $"{rnd.Next(0, 50)}";

                    var message = new MqttApplicationMessageBuilder()
                         .WithTopic("ali/test")
                         .WithPayload(payload1)
                         .WithExactlyOnceQoS()
                         .WithRetainFlag()
                         .Build();

                    _mqttClient.PublishAsync(message);
                    Console.WriteLine($"PUBLISHED: {payload1}");
                    Thread.Sleep(sleepMiliseconds);
                }
            });
            return task;
        }
    }
}
