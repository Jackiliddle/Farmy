# 🐇 Shoo, Bunnies! — Unity Game Project

A 3D **garden defense game** built with Unity.  
Grow your crops, defend them from pesky rabbits, and try to survive as long as you can!

---

## 🕹️ Gameplay Overview

In **Shoo, Bunnies!**, you play as a farmer protecting your crops from waves of hungry rabbits.  
Grow your veggies, shoo away intruders, and rack up points before your garden is completely eaten!

---

## 🚀 Features

- 🥕 **Crop Growth System** — Crops smoothly grow over time using scale interpolation.  
- 🐇 **Rabbit AI** — Rabbits wander, detect the player, flee, and circle crops before eating them.  
- 💥 **Particle & Sound Effects** — Visual and audio feedback for actions like eating, shooing, and growing.  
- 🎮 **Difficulty Selector** — Players can choose Easy, Medium, or Hard difficulty.  
- ⏱️ **Countdown Start** — A pre-game countdown before gameplay begins.  
- 🧠 **Game State Management** — Handles pause/resume, restart, and main menu transitions.  
- 🎵 **Music Toggle** — Background music can be turned on or off easily.  
- 🐾 **Animated Player Movement** — Includes walking animations, sounds, and dirt particle effects.

---

## 🧩 Core Scripts Overview

### 🪴 `CropGrower.cs`
Controls the visual growth of crops over time.  
- Scales each “Veggie” object from zero to full size.  
- Triggered when the game starts.  

### 🧱 `DetectCollisions.cs`
Handles the player’s interaction with rabbits.  
- Space key triggers a “shoo” action.  
- Plays sound/particle effects and awards points when rabbits are hit.  

### 🎚️ `Difficulty.cs`
Attached to difficulty selection buttons.  
- Starts the game with corresponding difficulty settings via `GameManager.StartGame()`.

### 🧠 `GameManager.cs`
Central control hub for game state.  
- Manages score, high score, UI, pause/resume, and instructions.  
- Starts countdowns, triggers crop growth, and monitors game over conditions.  

### 🎵 `MusicToggle.cs`
Toggles background music on/off.  
- Simple play/pause control via `AudioSource`.  

### 🕳️ `PestSpawner.cs`
Spawns rabbits from burrows based on difficulty.  
- Burrows pop up with animations and audio FX.  
- Spawns waves of rabbits with adjustable timing and counts.  

### 🚶 `PlayerController.cs`
Handles player movement and animations.  
- Uses Unity’s physics for smooth motion.  
- Plays walking sounds and dirt particles when moving.  

### 🐰 `RabbitMover.cs`
Defines rabbit AI behavior.  
- Rabbits wander, circle around crops (“laps”), and eat after enough rotations.  
- Flee when the player gets close.  
- Notifies the game manager when a veggie is eaten or rabbit is destroyed.  

### 🥦 `Veggie.cs`
Represents a crop in the garden.  
- Notifies `GameManager` on destruction to check for game over.

---

## ⚙️ How to Set Up in Unity

1. **Create a new Unity 3D project.**
2. Add all scripts from the `Scripts/` folder.
3. Create the following **tags**:
   - `Veggie`
   - `Rabbit`
   - `Burrow`
   - `Player`
4. Set up **GameObjects**:
   - 🥬 *Veggies*: Add `CropGrower` and `Veggie` scripts.
   - 🕳️ *Burrows*: Place under a `Burrows` parent and tag each with `Burrow`.
   - 🐇 *Rabbits*: Add `RabbitMover`, set `scoreValue`, and assign `monchAudioFX`.
   - 🎮 *Game Manager*: Attach `GameManager`, assign UI references and `PestSpawner`.
   - 👩‍🌾 *Player*: Add `PlayerController`, `DetectCollisions`, Rigidbody, Animator, and Collider.
5. Add audio clips and particle effects for:
   - Cash, Shoo, Pop, Burrow spawn, Eating, and Walking.
6. Configure UI for:
   - Title screen, difficulty buttons, restart button, pause menu, and instruction panel.

---

## 🧠 Game Flow

1. **Title Screen** → Choose difficulty.  
2. **Countdown** → 3...2...1...GO!  
3. **Gameplay**
   - Crops grow.  
   - Rabbits spawn and try to eat crops.  
   - Player defends and scores by “shooing” rabbits.  
4. **Game Over** → All veggies eaten.  
5. **Restart or Return to Main Menu.**

---

## 🧾 Notes

- The `ReturnToMainMenu()` function in `GameManager` is essential — do **not remove** it!  
- `HideAllBurrows()` in `PestSpawner` prevents early burrow visibility.  
- `Update()` in `Difficulty.cs` is intentionally empty but can be safely removed.

---

## 🛠️ Technical Details

| System | Description |
|--------|--------------|
| **Unity Version** | 2021.3+ recommended |
| **Language** | C# |
| **Input** | Keyboard (WASD + Space + ESC) |
| **Physics** | Rigidbody-based player movement |
| **Audio** | Uses Unity’s `AudioSource.PlayOneShot()` for SFX |

---


