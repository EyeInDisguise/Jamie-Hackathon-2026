# Jamie Hackathon 2026

A 2D speedrun platformer where physical RFID tokens change your abilities.

I built this solo in 2–3 days for a hackathon using Unity, an ESP32 and an MFRC522 RFID reader. The reader sends keyboard inputs over Bluetooth, so you can also play with a normal keyboard without the hardware.

## Current status

I'm continuing development after the hackathon: tuning movement, improving level readability and the ability HUD, and fixing the browser experience.

The updated Web build has been built and run locally on Windows. It uses a custom responsive template to fit the browser window. A public link to the updated demo is not available yet.

Crush detection, reset behaviour and race/leaderboard edge cases are still being worked on. This is a work in progress, not a finished release.

## Controls

Click inside the game to give it keyboard focus.

| Input | Action |
| --- | --- |
| A / D | Move |
| Space | Jump; release early for a shorter jump |
| 1 | Select Dash |
| 2 | Select Wall Jump |
| 3 | Select Gravity Flip |
| 4 | Select Time Stop |
| Left Shift | Activate the selected Dash, Gravity Flip or Time Stop ability |
| Space while airborne against a wall, with ability 2 selected | Wall jump |

Scanning an RFID token selects the corresponding ability, just like pressing a number key.

## Running the project

### 1. Clone and download the assets

Install Git with Git LFS support, then run:

```sh
git clone https://github.com/EyeInDisguise/Jamie-Hackathon-2026.git
cd Jamie-Hackathon-2026
git lfs install
git lfs pull
```

Images, audio and other binary assets use **Git LFS**. If an image or audio file is only a few lines of text beginning with `version https://git-lfs.github.com/spec/v1`, it is a pointer rather than the actual media. Finish downloading the LFS files before opening Unity.

### 2. Open in Unity

1. Install **Unity 6000.5.6f1** through Unity Hub, including **Web Build Support**.
2. In Hub, choose **Add project from disk** and select the cloned folder containing `Assets`, `Packages` and `ProjectSettings`.
3. Open the project and wait for asset importing to finish.
4. Open `Assets/Scenes/MainMenu.unity` and press Play.

### 3. Build for the browser

1. Save your scenes and exit Play mode.
2. Open **File → Build Profiles** and activate the Web profile.
3. Check that the enabled scenes are `MainMenu`, `Tutorial` and `Level1`, with `MainMenu` first.
4. Open the active profile's **Player Settings → Resolution and Presentation**.
5. Select the **Responsive** WebGL template and use **960 × 540** as the default canvas dimensions.
6. Choose **Build And Run**, exporting outside `Assets`, for example to `C:\UnityBuilds\RFID-Web-Polish` on Windows.

If the profile has its own Player Settings, change those settings rather than the global ones. The custom template lives in `Assets/WebGLTemplates/Responsive` and is used when Unity exports a new build.

Unity starts a local server and opens the build in your browser. Do not launch a Web build by double-clicking `index.html` using a `file://` address.

**Compression:** compressed Web builds need matching server configuration, or a build made with **Decompression Fallback** enabled. The older Brotli-compressed build in `JamieHackathonGame` needs appropriate `Content-Encoding: br` headers; a basic static server may not serve it correctly.

Changing scenes or scripts does not update an existing exported build. Rebuild to include those changes. The older export may differ from the current source project.

## Gameplay

The idea is to find a fast route through the course, switching abilities to suit each obstacle. The timer starts at the start trigger, and reaching the finish lets you submit a name and time to the local leaderboard.

The game includes:

- Movement with acceleration, coyote time, jump buffering and variable jump height
- Dash, wall slide/jump, gravity flip and time stop
- Moving hazards
- A tutorial and ability HUD
- A speedrun timer, local leaderboard and ghost replay
- Keyboard and physical RFID ability selection

## Why RFID?

Not long before this hackathon, I made a 2D platformer for a game jam where the theme was **gravity**. That got me thinking about how changing one movement rule could change the way a platformer feels.

Then I saw someone selling **Disney Infinity** figures on Facebook Marketplace. It reminded me of games where physical objects interact with the game, and I wanted to try a smaller version of that idea.

The result was physical tokens for different movement abilities:

```text
RFID token → MFRC522 reader → ESP32 → Bluetooth keyboard input → Unity
```

Using Bluetooth HID means Unity receives ordinary keyboard input rather than needing a custom Bluetooth connection inside the game.

## RFID controller

### Hardware and setup

- ESP32
- MFRC522 RFID reader, connected over SPI
- RFID tags or key fobs

Power the ESP32, pair the Bluetooth device named **Hackathon Keyboard**, then click inside the running game and scan a token.

The tags use these ability strings:

```text
dash000000000000
wall000000000000
gravity000000000
time000000000000
```

They map to keys `1`, `2`, `3` and `4` respectively.

### Firmware

The firmware uses PlatformIO with the Arduino framework. Its libraries include:

- ESP32-BLE-CompositeHID
- NimBLE-Arduino
- Callback
- Arduino_MFRC522v2

From the `RFIDReader` directory:

```sh
pio run                 # Build
pio run -t upload       # Upload to the connected ESP32
pio device monitor      # Serial monitor (115200 baud)
```

## Project structure

```text
Jamie-Hackathon-2026/
├── Assets/
│   ├── Scenes/
│   ├── Scripts/
│   └── WebGLTemplates/
│       └── Responsive/
├── Packages/
├── ProjectSettings/
├── RFIDReader/
│   ├── src/
│   │   └── main.cpp
│   └── platformio.ini
├── JamieHackathonGame/    # Older exported browser build
└── README.md
```

## Resources and credits

These resources helped me while building the project:

- [ESP32 with MFRC522 RFID reader](https://randomnerdtutorials.com/esp32-mfrc522-rfid-reader-arduino/)
- [Unity 2D platformer tutorial](https://generalistprogrammer.com/tutorials/unity-2d-platformer-complete-tutorial-game-development)
- [Player movement video](https://www.youtube.com/watch?v=g95rDlLfF1U)
- [Time-trial video](https://www.youtube.com/watch?v=GkAQh2QzdJA)
- [Tarodev](https://www.youtube.com/@Tarodev)
- [Music: Get Kominami](https://getkominami.com/bgm)
- [Button sound effects: Pixabay](https://pixabay.com/sound-effects/search/button%20click/)

[My hackathon notes on Notion](https://opalescent-wildcat-81e.notion.site/Jamie-Williams-Hackathon-2026-3c309feb1bf180458d28fb4e68c4aab8?source=copy_link)

## AI usage

I used AI during development for programming assistance, debugging, explanations and documentation, including this README. The ghost time-trial system involved more substantial assistance because it was one of the harder parts to put together.

Other uses included troubleshooting Unity/C#, ESP32 firmware, RFID and Bluetooth behaviour; discussing implementation approaches; reviewing and refactoring code; and help with Git and project structure.

I reviewed and tested suggestions and adapted them to the project. The concept, game design, hardware integration, 3D printing, implementation decisions, testing and final hackathon submission were my work and responsibility.
