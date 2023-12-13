// const int analogPin = 0;
// int raw = 0;
// int voltageIn = 5;
// float voltageDrop = 0;
// float resistance1 = 1000;
// float resistance2 = 0;
// float buffer = 0;
// float errorMargin = 15;

// const float jump = 500;
// const float right = 1000;
// const float left = 2000;

// void setup(){
// 	Serial.begin(9600);
// }

// void loop(){
//   raw = analogRead(analogPin);
//   resistance2 = 0;
//   if(raw){
//     buffer = raw * voltageIn;
//     voltageDrop = (buffer)/1024.0;
//     buffer = (voltageIn / voltageDrop) - 1;
//     resistance2 = resistance1 * buffer;
//     //Serial.print("Voltage Drop: ");
//     //Serial.println(voltageDrop);
//     //Serial.print("Resitance 2: ");
//     //Serial.println(resitance2);
//     delay(1);
//   }
  
//   if (resistance2 > jump - errorMargin && 
//       resistance2 < jump + errorMargin)
//   {
//   	Serial.println("Jump");
//   }
//   else if (resistance2 > right - errorMargin && 
//       	   resistance2 < right + errorMargin)
//   {
//   	Serial.println("Right");
//   }
//   else if (resistance2 > left - errorMargin && 
//       	   resistance2 < left + errorMargin)
//   {
//   	Serial.println("Left");
//   }
// }



const int voltageIn = 5;
const float referenceResistance = 1000;
const float errorMargin = 15;

const int amountOfAnalogPins = 3;
const int analogPins[amountOfAnalogPins] {1, 2, 3};

class Input {
public:
  Input(float resistance, int bitFlag) 
  {
    this->resistance = resistance;
    this->bitFlag = bitFlag;
  }
 
  float resistance;
  int bitFlag;
};

const Input jump(500,   0b1);
const Input right(1000, 0b10);
const Input left(2000,  0b100);

const int inputLength = 3;
const Input inputList[inputLength] { jump, right, left };

int decimalToBinary(int N) 
{ 
    // To store the binary number 
    int B_Number = 0; 
    int cnt = 0; 
    while (N != 0) { 
        int rem = N % 2; 
        int c = pow(10, cnt); 
        B_Number += rem * c; 
        N /= 2; 
        // Count used to store exponent value 
        cnt++; 
    } 
    return B_Number; 
}

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

  int bitFlag = 0;

  for (int i = 0; i < amountOfAnalogPins; i++){
    float resistance = CheckTile(analogPins[i]);

    for (int j = 0; j < inputLength; j++){
      const Input input = inputList[j];
      
      if (resistance < input.resistance + errorMargin && 
          resistance > input.resistance - errorMargin)
      {
        bitFlag |= input.bitFlag;
      }
    }
  }
  if (bitFlag != 0)
  {
    Serial.println(decimalToBinary(bitFlag));
  }
}