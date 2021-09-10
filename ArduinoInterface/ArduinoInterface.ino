Y /**
 * Proof of Concept of Facilitating the Selection of a Bionic Prosthesis Using Virtual Reality for an Amputated Patient
 * Authors: Jeanne Evrard & Gregoire van Oldeneel
 * UCLouvain, EPL
 * academic year 2019-2020
 */
 
/* Designed for an arduino uno micro-controller, connected via USB port to the computer.
 * Remark: The following code implements the interface of 2 buttons and 2 electrodes. 
 * For the presented setup, only electrodes are required.
 */
const int analogInPin0 = A0;    // Analog input pin
const int analogInPin1 = A1;    // Analog input pin
const int LedRed = 10;          // PWM output Led
const int LedGreen = 9;         // PWM output Led
const int Threshold_0 = 1500;   // Set threshold (mV) for led lighting
const int Threshold_1 = 1500;   // Set threshold (mV) for led lighting
int EmgStrength_0 = 0;          // emg0 value in 8-bit
int EmgStrength_1 = 0;          // emg1 value in 8-bit
int msg = 0; // msg sent by serial port, 32 bit.
// msg type: |bt--|----|----|Zrot|Yrot|Xrot|emg1|emg0|
const int emg0_place = 0;       // emg0 place in msg (bit)
const int emg1_place = 8;       // emg1 place in msg (bit)

const int buttonPin01 = 5;      // Digital input button1
const int buttonPin02 = 6;      // Digital input button2
const int button01_place = 30;  // button0 place in msg (bit)
const int button02_place = 31;  // button1 place in msg (bit)


void setup() // Initial pass
{
  Serial.begin(9600);
  // Inputs for buttons
  pinMode(buttonPin01,INPUT);
  pinMode(buttonPin02,INPUT);
  // Outputs for Leds
  pinMode(LedRed,OUTPUT);
  pinMode(LedGreen,OUTPUT);
  // Both buttons set as high while open
  digitalWrite(buttonPin01,HIGH);
  digitalWrite(buttonPin02,HIGH);
}


void loop()
{
  msg = 0; // reset msg value

  int Elect_0_Val = map(analogRead(analogInPin0), 0, 1023, 0, 5000); // Read and convert analog value into mV value
  delay(1);
  int Elect_1_Val = map(analogRead(analogInPin1), 0, 1023, 0, 5000);
  delay(1);
  bool button01 = digitalRead(buttonPin01); // Read button pins
  bool button02 = digitalRead(buttonPin02);

  /*=====================================*/
  /*====== EMG signals acquisition ======*/
  /*=====================================*/
  EmgStrength_0 = map(Elect_0_Val, 0, 5000, 0, 255);// convert mV value into a 8-bit value
  msg = msg + bit(emg0_place)*EmgStrength_0;        // Integration into the msg at the right place.
  EmgStrength_1 = map(Elect_1_Val, 0, 5000, 0, 255);
  msg = msg + bit(emg1_place)*EmgStrength_1;  
    
 if(Elect_0_Val >= Threshold_0){
    analogWrite(LedRed, map(Elect_0_Val, Threshold_0,5000, 10 ,255));      // Light the Led according to the emg strength (500 Hz)
  }
  else{analogWrite(LedRed, 0); }

  if(Elect_1_Val >= Threshold_1){
    analogWrite(LedGreen, map(Elect_1_Val, Threshold_1,5000, 10 ,255));
  }
   else{analogWrite(LedGreen, 0); }

  /*=====================================*/
  /*==== Buttons status acquisition ====*/
  /*=====================================*/
  if(button01==LOW)
  {
  msg = msg + bit(button01_place); // Integration into the msg at the right place.
  }
  if(button02==LOW)
  { 
  msg = msg + bit(button02_place);
  }

  /*=====================================*/
  /*============ msg sending ============*/
  /*=====================================*/
  if(msg != 0)
 {
    Serial.flush();
    Serial.println(msg);// Send the message
    delay(100);         // Wait for 100 ms before sending the next message
 }
}
