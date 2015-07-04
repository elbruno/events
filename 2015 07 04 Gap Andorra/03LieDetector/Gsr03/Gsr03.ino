const int BUZZER = 3;
const int GSR = A2;
int threshold = 0;
int sensorValue;

void setup() {
	long sum = 0;
	Serial.begin(9600);
	pinMode(BUZZER, OUTPUT);
	digitalWrite(BUZZER, LOW);
	delay(1000);

	for (int i = 0; i < 500; i++)
	{
		sensorValue = analogRead(GSR);
		sum += sensorValue;
		delay(5);
	}
	threshold = sum / 500;
}

void loop() {
	sensorValue = analogRead(GSR);
	Serial.print(threshold);
	Serial.print(",");
	Serial.println(sensorValue);
}
