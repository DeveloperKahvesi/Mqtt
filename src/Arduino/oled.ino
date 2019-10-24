#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>

#define OLED_Address 0x3C
Adafruit_SSD1306 oled(1);
WiFiClient espClient;
PubSubClient client(espClient);

char lines[4][20];
int currIndex = 0;

void AddLcdLine(char *line) {
	if (currIndex < 4)
	{
		strcpy(lines[currIndex], line);
		currIndex++;
	}
	else {
		for (size_t i = 0; i < 3; i++)
		{
			strcpy(lines[i], lines[i+1]);
		}
		strcpy(lines[3], line);
	}	
    oled.clearDisplay();
    oled.setTextColor(WHITE);
    oled.setCursor(0, 0);
    for (size_t i = 0; i < 4; i++)
    {  
        oled.println(lines[i]);        
    }
    oled.display();    
}


void callback(char* topic, byte* payload, unsigned int length) {   
    char buf[length];
    memcpy(buf,payload,length);
    AddLcdLine(buf);
}

void setup() {
    oled.begin(SSD1306_SWITCHCAPVCC, OLED_Address);

    WiFi.begin("xxx", "xxx");

    AddLcdLine("Connecting to WiFi");
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);        
    }
    AddLcdLine("Connected");
    AddLcdLine("Connecting to MQTT");

    client.setServer("www.baltavista.com", 8883);

    while (!client.connected()) {
      if (client.connect("ESP8266Clientx", "testuser", "testpass" )) {  
        AddLcdLine("Connected");   
        client.subscribe("test1");
        client.setCallback(callback);
      } else {  
        AddLcdLine("failed with state ");
       
        delay(2000);  
      }
    }

}



void loop() {
  client.loop();  
}