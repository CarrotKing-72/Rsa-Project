# Asphalt Oasis

A 2D top-down grid-based game built in Unity as a university project. You play as a dog navigating a city environment, collecting seeds while managing rising body temperature caused by the urban heat island effect. Bring seeds back to the hub and plant them to grow a green corridor through to the park.

---

## Play the Game

**[Play on itch.io](https://carrotking.itch.io/asphalt-oasis-web)**

Or download the latest build from the [Releases](../../releases) tab and run the executable.

**System Requirements**
- OS: Windows 10 or later
- No installation required, run the `.exe` directly

---

## How to Play

You start in the **Hub**. Your goal is to collect seeds in the levels and use them at the **Planting Station** in the Garden to unlock a path to the park.

**Controls**

| Key | Action |
|-----|--------|
| `W A S D` | Move one tile at a time |
| `E` | Interact with Planting Station |
| `Escape` | Pause / unpause |

**Core Loop**

1. Walk into the level portal to enter a level
2. Collect seeds scattered across the map
3. Reach the exit tile to return to the Hub with your seeds
4. Go to the Garden and press `E` at the Planting Station to spend seeds and unlock path sections
5. Unlock all 5 sections to reach the park and complete the game

**Watch your temperature.** Concrete tiles raise your body temperature. Once it crosses the danger threshold your health starts to drain. Grass and tree tiles cool you down. If your health hits zero you return to the Hub and lose the seeds you were carrying.

Each level is procedurally generated so the layout, seed positions, and exit location are different every run.

---

## Project Structure

```
Assets/
  Scripts/
    Level scripts/          # LevelGenerator, GridManager, GameManager, EndSquare, GoToLevel
      Seed Planting progress/ # PathTile, PathManager, PlantingStation
    Player Scripts/         # PlayerController, PlayerStats, SeedInventory, Seed
    UI & Visual scripts/    # HubUISync, HeatVignette
  Scenes/                   # MainMenu, Hub, Level, Garden
  Tilemaps/                 # Tile assets and palettes
```

---

## Asset Credits

- [Kenney RPG Urban Pack](https://kenney.nl/) (16x16 tiles, PPU 16)
- Cute Fantasy Free
- CP_V1 City Pack
- Outdoor_Decor_Free sprites
- DEVNIK 2D Pixel Art UI Pack
- Font: Thaleah Fat (TextMeshPro)

---

## Built With

- Unity 6000.2.7f2
- C#
- TextMeshPro
- GitHub Desktop for version control
