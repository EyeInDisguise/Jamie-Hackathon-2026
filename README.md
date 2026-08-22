Jamie Hackathon 2026

A 2D speedrun platformer built in Unity, controlled with a custom RFID-based ability system.

Players move through a short platforming course and physically scan RFID tokens to switch between four abilities:

1 — Dash

2 — Wall Jump

3 — Gravity Flip

4 — Time Stop

The RFID reader is built with an ESP32 and MFRC522 module. It reads an ability stored on an RFID tag and sends the matching number key to the computer over Bluetooth, allowing the physical scanner to work directly with the Unity input system.

Features

2D platforming movement

Coyote time and jump buffering

Variable jump height

Dash ability

Wall jump and wall slide

Gravity flip

Time stop for hazards

RFID-based physical ability switching

Bluetooth keyboard input from an ESP32

Speedrun timer

Local leaderboard

Fastest-run ghost playback

Tutorial and main game scenes

Ability HUD

Project Structure

Jamie-Hackathon-2026/
├── Assets/             # Unity assets, scripts and scenes
├── Packages/           # Unity package configuration
├── ProjectSettings/    # Unity project settings
├── RFIDReader/         # ESP32 / PlatformIO project
│   ├── src/
│   │   └── main.cpp
│   └── platformio.ini
└── README.md

RFID Hardware

The controller uses:

ESP32

MFRC522 RFID reader

RFID tags / key fobs

Bluetooth HID keyboard output

The MFRC522 communicates with the ESP32 over SPI.

The current ability strings stored on the RFID tags are:

dash000000000000
wall000000000000
gravity000000000
time000000000000

These map to keyboard inputs:

Dash        -> 1
Wall Jump   -> 2
Gravity     -> 3
Time Stop   -> 4

RFID Software

The hardware project uses PlatformIO with the Arduino framework.

Main dependencies include:

ESP32-BLE-CompositeHID

NimBLE-Arduino

Callback

Arduino_MFRC522v2

To build the RFID project:

cd RFIDReader
pio run

To upload to the ESP32:

pio run -t upload

To open the serial monitor:

pio device monitor

The serial monitor runs at 115200 baud.

Unity Input

The RFID reader behaves like a Bluetooth keyboard, so Unity does not need a custom serial or Bluetooth integration layer.

Scanning a token sends one of the number keys 1–4, which uses the same ability-selection logic as normal keyboard input.

This also means the game can still be tested without the physical RFID hardware.

Goal

The project explores combining a physical controller with a digital platformer. Instead of selecting abilities only through a keyboard or controller, players interact with physical RFID tokens to change their character's movement mechanics during a speedrun.
