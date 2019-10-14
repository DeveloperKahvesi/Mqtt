package main

import (
	"fmt"
	"log"
	"net/url"
	"os"
	"os/signal"
	"syscall"
	"time"

	MQTT "github.com/eclipse/paho.mqtt.golang"
	client "github.com/influxdata/influxdb1-client"
)

func insertInflux(payload []byte) {

	host, err := url.Parse(fmt.Sprintf("http://%s:%d", "www.baltavista.com", 8086))
	if err != nil {
		log.Fatal(err)
	}

	con, err := client.NewClient(client.Config{URL: *host})
	if err != nil {
		log.Fatal(err)
	}

	var pts = make([]client.Point, 1)
	pts[0] = client.Point{
		Measurement: "esp",
		Fields: map[string]interface{}{
			"value": string(payload),
		},
		Time: time.Now(),
	}

	bps := client.BatchPoints{
		Database: "testdb",
		Points:   pts,
	}

	_, err = con.Write(bps)
	if err != nil {
		log.Println("Insert data error:")
		log.Fatal(err)
	}
}

var handler MQTT.MessageHandler = func(client MQTT.Client, msg MQTT.Message) {
	fmt.Printf("Go subscriber : %s\n", msg.Payload())
	insertInflux(msg.Payload())
}

func main() {

	c := make(chan os.Signal, 1)
	signal.Notify(c, os.Interrupt, syscall.SIGTERM)

	opts := MQTT.NewClientOptions().AddBroker("tcp://www.baltavista.com:8883")
	opts.SetUsername("testuser")
	opts.SetPassword("testpass")

	topic := "test1"

	opts.OnConnect = func(c MQTT.Client) {
		if token := c.Subscribe(topic, 0, handler); token.Wait() && token.Error() != nil {
			panic(token.Error())
		} else {
			fmt.Printf("Go subscriber subscribed to topic :%s\n", topic)
		}
	}
	client := MQTT.NewClient(opts)
	if token := client.Connect(); token.Wait() && token.Error() != nil {
		panic(token.Error())
	} else {
		fmt.Printf("Go subscriber connected to server\n")
	}
	<-c
}
