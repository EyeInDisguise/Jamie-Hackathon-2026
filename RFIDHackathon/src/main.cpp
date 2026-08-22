#include <Arduino.h>
#include <KeyboardDevice.h>
#include <BleCompositeHID.h>

#include <MFRC522v2.h>
#include <MFRC522DriverSPI.h>
#include <MFRC522DriverPinSimple.h>
#include <MFRC522Debug.h>
#include <soc/rtc_cntl_reg.h>

int writemode = 0; // 1 for write, 0 for read

BleCompositeHID compositeHID("Hackathon Keyboard", "EyeInDisguise", 100);
BLEHostConfiguration bleHostConfig;
KeyboardDevice* keyboard;

MFRC522DriverPinSimple ss_pin(5);

MFRC522DriverSPI driver{ss_pin}; // Create SPI driver
//MFRC522DriverI2C driver{};     // Create I2C driver
MFRC522 mfrc522{driver};         // Create MFRC522 instance

MFRC522::MIFARE_Key key;

byte blockAddress = 2;
byte newBlockData[17] = {"dash000000000000"};
byte bufferblocksize = 18;
byte blockDataRead[18];
char scannedAbility[17] = {};


void setup() {
  WRITE_PERI_REG(RTC_CNTL_BROWN_OUT_REG, 0);
    Serial.begin(115200);
    while (!Serial);
    mfrc522.PCD_Init();    // Init MFRC522 board.
    //MFRC522Debug::PCD_DumpVersionToSerial(mfrc522, Serial);	// Show details of PCD - MFRC522 Card Reader details.
    Serial.println(F("Scan card to write data..."));
    
    // Prepare key - all keys are set to FFFFFFFFFFFF at chip delivery from the factory.
    for (byte i = 0; i < 6; i++) {
      key.keyByte[i] = 0xFF;
    }

    bleHostConfig.setHidType(HID_KEYBOARD);

    keyboard = new KeyboardDevice();
    compositeHID.addDevice(keyboard);
    compositeHID.begin(bleHostConfig);

    Serial.println("Waiting for connection");
    delay(3000);
}

void loop() {
  if (compositeHID.isConnected()) {
    //int buttonState = digitalRead(26);

    //if (buttonState == LOW) {
    //    Serial.println("Pressing modifier key " + String(KEY_A));
    //    keyboard->keyPress(KEY_A);
    //    delay(10);
    //    keyboard->keyRelease(KEY_A);
    //}
    

    // Reset the loop if no new card present on the sensor/reader. This saves the entire process when idle.
    if (mfrc522.PICC_IsNewCardPresent()) {
      // Select one of the cards
      if (mfrc522.PICC_ReadCardSerial()) {
        Serial.print("----------------\nCard UID: ");
        MFRC522Debug::PrintUID(Serial, (mfrc522.uid));
        Serial.println();

        if (mfrc522.PCD_Authenticate(0x60, blockAddress, &key, &(mfrc522.uid)) != 0) {
          Serial.println("Authentication failed.");
          mfrc522.PICC_HaltA();
          mfrc522.PCD_StopCrypto1();
          delay(1000);
          return;
        }

        if (writemode == 1) {
          Serial.println("Writing data to card...");

          if (mfrc522.MIFARE_Write(blockAddress, newBlockData, 16) != 0) {
            Serial.println("Write failed.");
          } else {
            Serial.print("Data written successfully in block: ");
            Serial.println(blockAddress);
          }
        } else {
          Serial.println("Reading data from card...");

          if (mfrc522.MIFARE_Read(blockAddress, blockDataRead, &bufferblocksize) != 0) {
            Serial.println("Read failed.");
          } else {
            memcpy(scannedAbility, blockDataRead, 16);
            scannedAbility[16] = '\0';

            if (strcmp(scannedAbility, "dash000000000000") == 0) {
              keyboard->keyPress(KEY_1);
              delay(10);
              keyboard->keyRelease(KEY_1);
            } else if (strcmp(scannedAbility, "wall000000000000") == 0) {
              keyboard->keyPress(KEY_2);
              delay(10);
              keyboard->keyRelease(KEY_2);
            } 
            else if (strcmp(scannedAbility, "gravity000000000") == 0) {
              keyboard->keyPress(KEY_3);
              delay(10);
              keyboard->keyRelease(KEY_3);
            }
              else if (strcmp(scannedAbility, "timestop00000000") == 0) {
              keyboard->keyPress(KEY_4);
              delay(10);
              keyboard->keyRelease(KEY_4);
            }

            Serial.println("Read successfully!");
            Serial.print("Data in block ");
            Serial.print(blockAddress);
            Serial.print(": ");

            for (byte i = 0; i < 16; i++) {
              Serial.print((char)blockDataRead[i]);
            }

            Serial.println();
          }
        }

        // Halt communication with the card
        mfrc522.PICC_HaltA();
        mfrc522.PCD_StopCrypto1();

        delay(1000);  // Delay for readability
      }
    }
  }
}