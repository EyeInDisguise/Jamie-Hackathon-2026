# Jamie Hackathon 2026

A 2D speedrun platformer built in Unity with a custom RFID-based ability switching system.

The player can switch between four movement abilities by scanning physical RFID tags with an ESP32 and MFRC522 reader. The ESP32 acts as a Bluetooth keyboard and sends number key inputs to Unity.

## Abilities

| Ability | Key |
| --- | --- |
| Dash | `1` |
| Wall Jump | `2` |
| Gravity Flip | `3` |
| Time Stop | `4` |

The RFID tags store one of these 16-byte strings:

```text
dash000000000000
wall000000000000
gravity000000000
time000000000000
```

## How It Works

```text
RFID tag
   |
   v
MFRC522 reader
   |
   v
ESP32
   |
   v
Bluetooth keyboard input
   |
   v
Unity Input System
   |
   v
Player ability changes
```

Because the ESP32 behaves like a normal keyboard, the game can also be tested without the RFID hardware by pressing the number keys directly.

## Features

- 2D platforming movement
- Coyote time
- Jump buffering
- Variable jump height
- Dash
- Wall slide and wall jump
- Gravity flipping
- Time stop
- Moving hazards
- RFID-based ability switching
- Bluetooth HID input
- Speedrun timer
- Local leaderboard
- Ghost replay
- Tutorial level
- Ability HUD

## Inspiration
How on earth did I get this idea?!??!?!
The idea for this project came from a couple of things I had been doing and seeing recently.

Not long before this hackathon, I made a 2D platformer for a game jam on itch.io where the theme was **gravity**. That got me thinking more about movement mechanics and how changing one core rule of movement can completely change how a platformer feels.

Then recently I was browsing Facebook Marketplace and saw someone selling **Disney Infinity** figures.

That reminded me of games where physical figures or objects interact with the game itself, and I started thinking about whether I could make something similar in a much simpler way.

The basic idea became:

```text
Physical token
    |
    v
RFID reader
    |
    v
Ability changes in-game

## Project Structure

```text
Jamie-Hackathon-2026/
├── Assets/
├── Packages/
├── ProjectSettings/
├── RFIDReader/
│   ├── include/
│   ├── lib/
│   ├── src/
│   │   └── main.cpp
│   ├── test/
│   └── platformio.ini
└── README.md
```

## RFID Hardware

The RFID controller uses an ESP32 and MFRC522 RFID reader.

The ESP32 firmware is built using PlatformIO with the Arduino framework.

### Libraries

The RFID project uses:

```text
ESP32-BLE-CompositeHID
NimBLE-Arduino
Callback
Arduino_MFRC522v2
```

### Build

From the `RFIDReader` directory:

```bash
pio run
```

### Upload

```bash
pio run -t upload
```

### Serial Monitor

```bash
pio device monitor
```

The serial monitor runs at:

```text
115200 baud
```

## Unity Input

The RFID reader behaves as a Bluetooth keyboard.

The ability mappings are:

```text
1 = Dash
2 = Wall Jump
3 = Gravity Flip
4 = Time Stop
```

Unity receives the same number key input whether it comes from the physical RFID scanner or from a normal keyboard.

This means the game can still be developed and tested without having the RFID hardware connected.

## Game Loop

The game is designed as a short speedrun level.

The player moves through the level and switches between abilities depending on the obstacle they are trying to pass.

The timer starts when the player enters the start trigger and stops when they reach the finish trigger.

After finishing, the player can enter their name and submit their time to the local leaderboard.

The fastest recorded run can also be replayed as a ghost.

---

## Resources Used

Some resources that were beneficial throughout development.

### RFID

[ESP32 with MFRC522 RFID Reader](https://randomnerdtutorials.com/esp32-mfrc522-rfid-reader-arduino/)

### Player Controller

These helped with things such as coyote time, jump buffering and general 2D platformer movement:

[Unity 2D Platformer Complete Tutorial](https://generalistprogrammer.com/tutorials/unity-2d-platformer-complete-tutorial-game-development)

[YouTube Tutorial](https://www.youtube.com/watch?v=g95rDlLfF1U)

### Time Trial

[YouTube Tutorial](https://www.youtube.com/watch?v=GkAQh2QzdJA)

### In General

This guy is the goat:

[Tarodev](https://www.youtube.com/@Tarodev)

---

## Notion
I was recording some stuff on Notion you can check out here
https://opalescent-wildcat-81e.notion.site/Jamie-Williams-Hackathon-2026-3c309feb1bf180458d28fb4e68c4aab8?source=copy_link

## AI Usage

I ain't no saint, especially doing this project solo.

AI tools **were** used during development for programming assistance, debugging and explanations.

### How AI Was Used

AI was mainly used for:

- explaining programming errors
- debugging Unity and C# issues
- debugging ESP32 and PlatformIO issues
- troubleshooting RFID behaviour
- troubleshooting Bluetooth HID behaviour
- suggesting implementation approaches
- reviewing and refactoring code
- explaining unfamiliar APIs and libraries
- helping with Git and project structure
- helping write project documentation (this README :D)

AI was also used quite a bit when implementing the ghost time trial system since that ended up being one of the harder systems to put together :(

### Development Process

AI-generated code and suggestions were reviewed and tested before being added to the project.

Changes were modified where necessary to fit the project and to make sure I understood what the code was doing.

AI was used as a development tool rather than as a replacement for testing or understanding the implementation.

### My Contribution

The project concept, game design, RFID controller idea, implementation decisions, hardware integration, Unity integration, 3D printing, testing and final submission were completed by me.

I was responsible for deciding what features to implement, integrating the different systems together and testing the final result.

### Transparency

This section is included to clearly document where AI assistance was used during development.
