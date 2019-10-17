
from flask import Flask, render_template
from flask_mqtt import Mqtt
from flask_socketio import SocketIO
from influxdb import InfluxDBClient


app = Flask(__name__)
app.config['SECRET'] = 'my secret key'
app.config['TEMPLATES_AUTO_RELOAD'] = True
app.config['MQTT_BROKER_URL'] = 'www.baltavista.com'
app.config['MQTT_BROKER_PORT'] = 8883
app.config['MQTT_USERNAME'] = 'testuser'
app.config['MQTT_PASSWORD'] = 'testpass'
app.config['MQTT_KEEPALIVE'] = 5
app.config['MQTT_TLS_ENABLED'] = False

mqtt = Mqtt(app)
socketio = SocketIO(app)

def getInfluxData():
    client = InfluxDBClient("www.baltavista.com", 8086, "", "", "testdb")
    result = client.query("select * from esp order by time desc limit 50")    
    return  result

@app.route('/')
def index():    
    return render_template('index.html')

@app.route('/getData')
def influxData():    
    return dict(getInfluxData().raw)



@mqtt.on_message()
def handle_mqtt_message(client, userdata, message):   
    print(message.payload.decode())
    socketio.emit('mqtt_message', data=message.payload.decode())

@mqtt.on_connect()
def handle_connect(client, userdata, flags, rc):
    mqtt.subscribe('test1')


if __name__ == '__main__':


    socketio.run(app, host='0.0.0.0', port=5000, use_reloader=False, debug=False)