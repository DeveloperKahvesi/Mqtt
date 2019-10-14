

import paho.mqtt.client as mqtt


client = mqtt.Client()

client.username_pw_set(username="testuser",password="testpass")
client.connect("www.baltavista.com", 8883)

client.publish("test","hello server")

client.loop_forever()
 
