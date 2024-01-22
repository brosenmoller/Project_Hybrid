const int voltageIn = 5;
const float referenceResistance = 100000;
const float errorMargin = 2000;

//Mux control pins
const int s0 = 7;
const int s1 = 8;
const int s2 = 9;
const int s3 = 10;

//Mux in "SIG" pin
const int SIG_pin = 0;

int muxChannel[16][4]={
  {0,0,0,0}, //channel 0
  {1,0,0,0}, //channel 1
  {0,1,0,0}, //channel 2
  {1,1,0,0}, //channel 3
  {0,0,1,0}, //channel 4
  {1,0,1,0}, //channel 5
  {0,1,1,0}, //channel 6
  {1,1,1,0}, //channel 7
  {0,0,0,1}, //channel 8
  {1,0,0,1}, //channel 9
  {0,1,0,1}, //channel 10
  {1,1,0,1}, //channel 11
  {0,0,1,1}, //channel 12
  {1,0,1,1}, //channel 13
  {0,1,1,1}, //channel 14
  {1,1,1,1}  //channel 15
};

const int amountOfAnalogPins = 15;
int analogPins[amountOfAnalogPins] = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

class Input {
public:
  Input(float resistance, byte bitFlag) 
  {
    this->resistance = resistance;
    this->bitFlag = bitFlag;
  }
 
  float resistance;
  byte bitFlag;
};

Input left    (10000,   0b1);
Input right   (5100,  0b10);
Input jump    (20000,  0b100);
Input crouch  (51000,  0b1000);
Input attack  (68000,  0b10000);
Input dash    (100000, 0b100000);

const int inputLength = 6;
Input inputList[inputLength] = { left, right, jump, crouch, attack, dash };

int ReadMultiplexer(int channel){
  int controlPin[] = {s0, s1, s2, s3};

  for(int i = 0; i < 4; i ++){
    digitalWrite(controlPin[i], muxChannel[channel][i]);
  }

  return analogRead(SIG_pin);
}

float GetAnalogResistance(int analogPin) {
  int raw = 0;
  raw = ReadMultiplexer(analogPin);
  //Serial.println(raw);

  float tileResistance = 0;
  float buffer = 0;
  float voltageDrop = 0;

  if (raw) {
    buffer = raw * voltageIn;
    voltageDrop = (buffer) / 1024.0;
    buffer = (voltageIn / voltageDrop) - 1;
    tileResistance = referenceResistance * buffer;
  }

  return tileResistance;
}

void setup(){
  pinMode(s0, OUTPUT);
  pinMode(s1, OUTPUT);
  pinMode(s2, OUTPUT);
  pinMode(s3, OUTPUT);

  digitalWrite(s0, LOW);
  digitalWrite(s1, LOW);
  digitalWrite(s2, LOW);
  digitalWrite(s3, LOW);

	Serial.begin(9600);
}

void loop(){

  byte bitFlag = 0;

  for (int i = 0; i < amountOfAnalogPins; i++){
    const float resistance = GetAnalogResistance(analogPins[i]);
    //Serial.println(resistance);

    if (resistance > 200000 || resistance < 1){
      continue;
    }

    //Serial.println(resistance);

    for (int j = 0; j < inputLength; j++){
      const Input input = inputList[j];
      
      if (resistance < input.resistance + errorMargin && 
          resistance > input.resistance - errorMargin)
      {
        bitFlag |= input.bitFlag;
        break;
      }
    }
  }

  if (bitFlag != 0){
    Serial.flush();
    Serial.write(bitFlag);
  }

  delay(10);
  
  // if (bitFlag & jump.bitFlag)
  // {
  //   Serial.println("Jump");
  // }
  // if (bitFlag & right.bitFlag)
  // {
  //   Serial.println("Right");
  // }
  // if (bitFlag & left.bitFlag)
  // {
  //   Serial.println("Left");
  // }
  // if (bitFlag & attack.bitFlag)
  // {
  //   Serial.println("Dash");
  // }
  // if (bitFlag & dash.bitFlag)
  // {
  //   Serial.println("Dash");
  // }
  // if (bitFlag & dash.bitFlag)
  // {
  //   Serial.println("Dash");
  // }
  
}