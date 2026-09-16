# Post-hackathon polish roadmap

The goal is a small, finished portfolio game that still feels like the hackathon project I made. I do not want to bury the RFID idea under unrelated features or replace everything with systems I cannot explain.

The original submission is preserved by the `hackathon-submission` tag. New work continues on `main`.

## Working rule

Each step gets its own commit. Before I commit it, I will:

1. Play the changed part in Unity.
2. Check the Console for new errors.
3. Test keyboard input without the RFID controller.
4. Run the C# build check when scripts changed.
5. Read the diff and remove accidental scene or IDE changes.
6. Add a plain-language entry to `docs/DEVLOG.md` explaining the change.
7. Commit and push before starting the next step.

Large features are split by visible result. A level, menu, or camera behaviour should never share a commit with an unrelated cleanup.

## Order of work

### 1. Clean baseline

This makes later commits easy to review.

- Ignore Rider's `.idea` files.
- Remove the remaining compiler warning in `CameraZoomTrigger`.
- Confirm Main Menu, Tutorial, Level 1, restart, finish, leaderboard, ghost, and Web build still work.
- Write down any bugs found during that run instead of fixing all of them in the same commit.

Commit: `chore: clean up the polish baseline`

Done when the project compiles without warnings and the current game can be completed once from the main menu.

### 2. Functional level select

Build the useful version before styling it.

- Add a Level Select scene between the main menu and gameplay.
- Give it a back button and one button per playable level.
- Store the scene name and display name in a small, easy-to-read component.
- Keep Tutorial separate so it remains easy to find.
- Add locked levels and saved progress only after there are enough real levels for locking to matter.
- Keep every button usable with the same input method as the game.

Commit: `feat: add functional level select`

Done when I can go Main Menu → Level Select → any available level → Main Menu without using the Unity editor.

### 3. Camera foundation

Keep the current `PlayerCamera` and improve it. It already supports a dead zone, horizontal look-ahead, and smooth zoom, so replacing it now would add work without fixing the design problems by itself.

- Replace the one-value zoom triggers with camera-area profiles.
- Let an area set zoom, offset, follow damping, and optional X/Y bounds.
- Handle overlapping areas using priority or a stack so leaving one trigger does not reset another active trigger.
- Smooth direction changes so the look-ahead does not snap when the player turns.
- Snap the camera cleanly after a restart or teleport.
- Add subtle screen shake later, after normal tracking feels good.

Commit: `feat: add camera areas and smoother follow`

Done when the camera shows the next decision, never reveals empty space outside a room, and does not fight the player during jumps or gravity flips.

### 4. Reusable level kit

Build levels from a small set of reliable pieces.

- Player spawn and finish.
- Kill volume and crusher.
- Moving platform or moving hazard with clear travel endpoints.
- Camera area.
- Ability teaching sign.
- Restart shortcut for testing.
- A simple environment palette with clear ground, danger, background, and interactable colours.

Commit: `feat: add reusable level building pieces`

Done when a blank scene can become a playable greybox without copying objects from several old scenes.

### 5. Levels, one at a time

Every level should use a teach → test → twist structure. The GDC talk on *Celeste* is useful here because it covers the process behind hundreds of short platforming rooms instead of treating a level as one long obstacle course.

Target length: roughly 30–90 seconds for a clean first completion. Speedrunning can make the same space last longer without padding it.

Suggested sequence:

1. **Tutorial pass** — teach movement, selection, activation, and the finish condition with fewer signs and safer practice spaces.
2. **Level 1: Dash** — teach a safe dash gap, test direction choice, then combine dash with a normal jump.
3. **Level 2: Wall Jump** — teach one wall, test alternating walls, then combine wall jump with dash.
4. **Level 3: Gravity** — introduce gravity flip in a safe room, then use it for route planning rather than surprise deaths.
5. **Level 4: Time Stop** — show hazard timing before the player is punished, then combine stopped hazards with movement.
6. **Final course** — short sections that require deliberate ability switching and reward a clean speedrun route.

Use one commit per level, for example: `level: build the wall-jump course`.

Done when each room has a clear purpose, a visible destination, a safe way to learn new behaviour, and no blind jumps.

### 6. UI redesign

The UI gets its visual pass after the real screens and levels exist.

- Choose one font family, one spacing scale, and a small colour palette.
- Keep the gameplay HUD limited to current ability, controls when needed, and timer information.
- Design normal, hover, selected, keyboard-focus, pressed, and disabled states together.
- Make the level select show names and a small useful detail such as best time or ability focus.
- Replace temporary images that do not share a visual style.
- Check the UI at the smallest supported browser size, not only at 1920×1080.
- Keep text high contrast and do not communicate ability state through colour alone.

Possible commits:

- `style: rebuild the main menu and level select`
- `style: improve the gameplay HUD`
- `style: redesign finish and leaderboard screens`

### 7. Game feel pass

Add feedback only where it explains an action or makes an important moment feel better.

- Small landing dust and a stronger landing animation.
- Clear dash trail and impact feedback.
- A short, readable death/reset effect.
- Hazard anticipation before movement changes direction.
- Ability-change feedback that matches the HUD.
- Very light camera shake on selected impacts, with an option to disable it.
- Balance music and effects, then add separate volume controls.

Avoid adding particles, shake, chromatic effects, and flashes everywhere. One strong response per action will look more intentional.

Commit by behaviour, such as `polish: improve dash feedback`, instead of one giant “game feel” commit.

### 8. Final portfolio pass

- Pause, restart, volume, and return-to-menu controls.
- Saved best times and level progress if the level set is final.
- Full keyboard navigation and readable focus states.
- Test 16:9, a smaller browser window, fullscreen, and WebGL keyboard focus.
- Complete every level from a clean save.
- Check the development and release Web builds.
- Capture a short gameplay clip showing RFID input and the game reacting.
- Update the README with final screenshots, a playable link, controls, technical highlights, and what I learned.

Commit: `release: prepare the polished portfolio build`

## Rules for avoiding generic generated work

- Start from a problem visible in the current game, not a list of fashionable features.
- Use references for one decision at a time. Camera references answer camera questions; menu references answer menu questions.
- Keep the original ability switching and physical-controller idea at the centre of the game.
- Do not use random gradients, piles of rounded cards, mismatched icon packs, excessive glow, or filler copy to make a screen look “modern”.
- Reuse a small set of colours, type sizes, sounds, transitions, and shapes.
- Prefer a tuned Inspector value over another abstraction when the value is specific to one level.
- Keep scripts small enough that I can explain their state and data flow without reading them line by line.
- Remove comments that only translate obvious code into English. Keep comments that explain a reason, workaround, or non-obvious rule.
- Ask what a player should notice, understand, or decide before adding an effect.
- Playtest after every meaningful change. A polished screenshot cannot prove that movement, camera timing, or menu navigation feels good.

## Prompt template for future work

Official OpenAI guidance for GPT-6 Astra recommends stating the outcome, concrete constraints, writing style, and appropriate verification. This project prompt keeps the model focused on one reviewable change:

```text
Work on one improvement in my Unity 6.5 2D speedrun platformer: [GOAL].

Project context:
- The main idea is switching abilities with keyboard keys or physical RFID tokens.
- Preserve the current controls, serialized tuning, Web build, timer, leaderboard, and ghost unless the task requires a specific change.
- Inspect the existing implementation before deciding what to change.

Success means:
- [PLAYER-VISIBLE RESULT]
- [IMPORTANT EDGE CASE]
- [WHAT MUST STILL WORK]

Constraints:
- Make the smallest cohesive change that solves the problem.
- Match the existing beginner-readable C# style.
- Do not add a package, manager, generic framework, or visual effect unless it clearly earns its place.
- Keep the visual direction consistent with the existing game instead of producing a generic sci-fi/glowing/card-based UI.
- Use current official Unity documentation and 1–2 relevant game references when research would improve the decision.
- I need to understand and explain the final code, so explain any non-obvious state or Unity API briefly.

Validation:
- Compile the C# project.
- Give me a short Play Mode checklist for behaviour that cannot be tested outside Unity.
- Review the diff for unrelated files.
- Stop after the change is working and propose one clear commit message.
```

For a design review, replace the goal with this:

```text
Review this level or UI from the attached screenshot and current scene files. Identify the three issues that most affect player understanding or game feel. Tie each issue to visible evidence, propose the smallest fix, and rank the fixes by impact. Do not redesign the whole game or add features I did not ask for.
```

For level design:

```text
Help me greybox one 30–90 second platforming level around [ABILITY]. Use teach → test → twist. For every room, state what the player learns, what failure teaches them, what the camera must show, and how the speedrun route can reward mastery. Reuse my existing mechanics and avoid introducing a new mechanic just to fill space.
```

## References

- [Unity Cinemachine 3.1 Position Composer](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePositionComposer.html) — dead zones, damping, look-ahead, and screen composition. These ideas also apply to the custom camera.
- [Unity Cinemachine 3.1 Confiner 2D](https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineConfiner2D.html) — reference for keeping a 2D camera inside room boundaries.
- [Level Design Workshop: Designing Celeste](https://www.gdcvault.com/play/1024307/Level-Design-Workshop-Designing-Celeste) — short platforming stages, area layout, and the process behind a large set of levels.
- [Game UI Database](https://www.gameuidatabase.com/) — searchable screenshots and videos. Use it to compare specific screen types rather than copy an entire style.
- [Game Accessibility Guidelines: Basic](https://gameaccessibilityguidelines.com/basic/) — readable text, contrast, input consistency, remapping, saved settings, and audio controls.
- [OpenAI model guidance for GPT-6 Astra](https://developers.openai.com/api/docs/guides/latest-model) — current prompting and verification guidance.
