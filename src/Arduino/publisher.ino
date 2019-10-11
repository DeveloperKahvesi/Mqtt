
#include "Arduino.h"
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include "settings.h"

const int trigPin = 15;
const int echoPin = 13;

long duration;
int distance;


 
WiFiClient espClient;
PubSubClient client(espClient);
 

void setup()
{

  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  
  Serial.begin(115200);

  WiFi.begin(ssid, password);
  
    while (WiFi.status() != WL_CONNECTED) {
      delay(500);
      Serial.println("Connecting to WiFi..");
    }
    Serial.println("Connected to the WiFi network");
  
    client.setServer(mqttServer, mqttPort);
      
    while (!client.connected()) {
      Serial.println("Connecting to MQTT...");
  
      if (client.connect("ESP8266Client", mqttUser, mqttPassword )) {
  
        Serial.println("connected");  
  
      } else {
  
        Serial.print("failed with state ");
        Serial.print(client.state());
        delay(2000);
  
      }
    }
}

int GetDistance() {
  // Clears the trigPin
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  // Sets the trigPin on HIGH state for 10 micro seconds
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  // Reads the echoPin, returns the sound wave travel time in microseconds
  duration = pulseIn(echoPin, HIGH);
  // Calculating the distance
  return duration * 0.034 / 2;
}

void loop()
{

  client.publish("test1",const_cast<char*>(String(RANDOM_REG32).c_str()));
  delay(1000);
}