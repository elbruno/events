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
  Serial.print("threshold =");
  Serial.println(threshold);
}

void loop() {
  int temp;
  sensorValue = analogRead(GSR);
  temp = threshold - sensorValue;
  Serial.print("threshold=");
  Serial.print(threshold);
  Serial.print(" sensorValue=");
  Serial.print(sensorValue);
  Serial.print(" temp=");
  Serial.println(temp);
  if (abs(temp) > 50)
  {
    sensorValue = analogRead(GSR);
    temp = threshold - sensorValue;
    if (abs(temp) > 50) {
      digitalWrite(BUZZER, HIGH);
      Serial.println("YES!");
      delay(3000);
      digitalWrite(BUZZER, LOW);
      delay(1000);
    }
  }
}
