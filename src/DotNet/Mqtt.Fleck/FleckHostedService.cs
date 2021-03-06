using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fleck;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace Mqtt.Fleck
{
    public class FleckHostedService : IHostedService
    {
        private WebSocketServer _server;
        private IManagedMqttClient _mqttClient;
        private List<IWebSocketConnection> _allSockets = new List<IWebSocketConnection>();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var task = Task.Run(async () =>
            {
                _server = new WebSocketServer("ws://0.0.0.0:9191");
                _server.Start(socket =>
                {
                    socket.OnOpen = () => _allSockets.Add(socket);
                    socket.OnClose = () => _allSockets.Remove(socket);
                });

                await ConsumeAsync();
            });
            return task;
        }

        private async Task ConsumeAsync()
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("Ali_Fleck_Consumer")
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
                var message = $"({DateTime.Now}):{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}";
                _allSockets.ForEach(s => s.Send(message));
                Console.WriteLine($"FLECK SERVED : {message}");
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _mqttClient.StopAsync().Wait();
            _mqttClient.Dispose();

            _allSockets.ForEach(s =>
            {
                s.Close();
            });
            _allSockets.Clear();
            _server.Dispose();

            return Task.CompletedTask;
        }
    }
}
