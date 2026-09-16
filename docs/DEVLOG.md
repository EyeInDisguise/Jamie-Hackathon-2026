# Development log

This is the plain-language companion to the Git history. It records what changed, why it changed, how the important part works, and what still needs to be checked in Unity.

Future entries use this structure:

- **What changed** — the player-visible result and affected files.
- **Why** — the problem the change solves.
- **How it works** — enough detail to explain the code without reading every line.
- **Validation** — checks already completed and any Play Mode checks still needed.

## `f015355` — Kill player when crushed by moving hazards

### What changed

- Moving hazards now move through their `Rigidbody2D` during `FixedUpdate`.
- A moving hazard exposes its current movement velocity.
- The player checks current physics contacts for a real crushing situation.
- A crush resets the run. In a scene without `RunResetManager`, such as the tutorial, the player returns to their original spawn position.

Files:

- `Assets/Scripts/MovingHazard.cs`
- `Assets/Scripts/PlayerController.cs`

### Why

Touching a moving platform should not automatically kill the player. A death should happen only when the platform is actively pushing the player into another solid surface.

### How it works

`MovingHazard` calculates where it should move next and stores the velocity needed to reach that position. It uses `Rigidbody2D.MovePosition`, which lets Unity's 2D physics system create reliable contacts while the kinematic body moves.

The player reads its current `ContactPoint2D` contacts. It looks for:

1. A contact belonging to a `MovingHazard`.
2. That hazard moving toward the player along the contact normal.
3. Another contact whose normal faces the opposite direction.

All three conditions are required. This means standing on a moving platform with free space above is safe, while being pushed between that platform and a ceiling is a crush.

Time stop sets the hazard velocity to zero, so a frozen hazard cannot cause a new crush death. A short reset cooldown prevents old contacts from causing a second reset immediately after respawning.

### Validation

- C# project compiled successfully.
- Still needs the four Play Mode checks listed in the task summary: crush against a surface, ride with open space, time-stop before closure, and ordinary narrow passage.

## `0cad348` — Add post-hackathon polish roadmap

### What changed

Added `docs/POLISH_ROADMAP.md` with the implementation order, commit boundaries, definitions of done, research links, rules for avoiding generic generated work, and reusable prompts.

### Why

The project has several connected improvements. A fixed order prevents level-select, camera, level-design, and UI work from becoming one large commit or repeatedly undoing each other.

### How it works

The plan starts with a stable baseline, then builds a functional level flow, improves the camera, creates reusable level pieces, builds levels one at a time, and finishes with UI and game-feel passes. Every stage has a visible success condition and suggested commit message.

### Validation

- Documentation-only change.
- Markdown diff checked before commit.

## `5bd2375` — Clean up the polish baseline

### What changed

- Added `/.idea/` to `.gitignore` so Rider's personal project files do not enter future commits.
- Replaced the deprecated camera lookup with `FindAnyObjectByType<PlayerCamera>()`.
- A camera zoom trigger now reports a clear setup error and disables itself if no `PlayerCamera` exists.
- Removed old commented-out debug messages from `CameraZoomTrigger`.

Files:

- `.gitignore`
- `Assets/Scripts/CameraZoomTrigger.cs`

### Why

Generated IDE settings make commits noisy and are different on every computer. The old Unity lookup also produced a compiler warning, and a missing camera reference would have caused a null-reference error when the player entered the trigger.

### How it works

The trigger first uses the camera assigned in the Inspector. If the field is empty, it searches the active scene for any `PlayerCamera`. If none exists, the component logs which trigger is misconfigured and disables itself before it can receive trigger callbacks.

### Validation

- C# project compiled with zero warnings and zero errors.
- Git working tree was clean after the commit.
- Full menu-to-finish Play Mode smoke test remains the human baseline check.
