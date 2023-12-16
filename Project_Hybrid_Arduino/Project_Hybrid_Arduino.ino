const int voltageIn = 5;
const float referenceResistance = 100000;
const float errorMargin = 2000;

const int amountOfAnalogPins = 2;
int analogPins[amountOfAnalogPins] = {0, 1};

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

Input left(   5100,   0b1);
Input right(  10000,   0b10);
Input jump(   20000,  0b100);
Input crouch( 51000,  0b1000);
Input attack( 68000,  0b10000);
Input dash(   100000,  0b100000);

const int inputLength = 6;
Input inputList[inputLength] = { left, right, jump, crouch, attack, dash };

float CheckTile(int analogPin) {
  int raw = 0;
  raw = analogRead(analogPin);
  
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
	Serial.begin(9600);
}

void loop(){

  byte bitFlag = 0;

  for (int i = 0; i < amountOfAnalogPins; i++){
    const float resistance = CheckTile(analogPins[i]);

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
  
}