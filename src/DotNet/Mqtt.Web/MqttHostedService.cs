using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Mqtt.Web.Hubs;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mqtt.Web
{
    public class MqttHostedService : IHostedService
    {
        private readonly IHubContext<MqttHub> _hubContext;
        private IManagedMqttClient _mqttClient;

        public MqttHostedService(IHubContext<MqttHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Consume();
        }

        private async Task Consume()
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId("Ali_SgnlaR_Consumer")
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
                _hubContext.Clients.All.SendAsync("ReceiveMessage", $"({DateTime.Now}):{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _mqttClient.StopAsync();
        }
    }
}
