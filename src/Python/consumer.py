

import paho.mqtt.client as mqtt

def on_message(client, userdata, message):
   print("Message Recieved: " + message.payload.decode())


client = mqtt.Client()
client.on_message = on_message

client.username_pw_set(username="testuser",password="testpass")
client.connect("www.baltavista.com", 8883)

client.subscribe("test1", qos=1)

client.loop_forever()
