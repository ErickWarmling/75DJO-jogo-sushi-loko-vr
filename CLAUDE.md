# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Sushi Loko VR** is a VR escape room game developed as Assignment 2 for the 75DJO (Game Development) course at UDESC. Built in Unity targeting Meta Quest 2/3 via Android/OpenXR.

- **Engine**: Unity 6000.3.10f1
- **Target Platform**: Android — Meta Quest 2/3 (OpenXR)
- **Graphics Pipeline**: Universal Render Pipeline (URP) 17.3.0
- **Scripting**: C# with IL2CPP backend
- **App ID**: `com.ErickAugustoWarmling.SushiLokoVR`

## Build & Run

This is a Unity project — there are no CLI build commands. All actions go through the Unity Editor.

- **Open project**: Open the `SushiLokoVR/` folder in Unity 6000.3.10f1
- **Play in Editor**: Use the XR Interaction Simulator (configured in Samples) to test without a headset
- **Build for device**: File → Build Profiles → Android → Build (requires Android SDK + Meta Quest Link)
- **Deploy**: Side-load the APK to Meta Quest 2 or 3 via ADB or Meta Quest Developer Hub
- **Min Android SDK**: API level 32

## Architecture

### Scene Structure

The game has 3 scenes forming a linear escape room:
1. **Reception** (`Scenes/`) — Entry area, teleportation-only movement
2. **Kitchen** — Core gameplay: prepare a Yakisoba dish using grab/poke interactions
3. **Outdoor Area** — Delivery destination, triggers game completion

### Key Scripts (`Assets/VRTemplateAssets/Scripts/`)

| Script | Purpose |
|---|---|
| `StepManager.cs` | Manages tutorial coaching card progression |
| `XRKnob.cs` | Knob interaction handling poke and turn inputs |
| `LaunchProjectile.cs` | Projectile mechanics |
| `HandSubsystemManager.cs` | Hand tracking lifecycle management |
| `XRPokeFollowAffordanceFill.cs` | Visual feedback for poke interactions |
| `BooleanToggleVisualsController.cs` | Drives visual state changes on boolean toggles |
| `AnchorVisuals.cs` | Spatial anchor visualization |
| `VideoPlayerRenderTexture.cs` | Video playback to render textures |
| `Callout.cs` | In-world UI annotation/callout system |
| `Rotator.cs` | Simple continuous rotation utility |

### XR Interaction Stack

- **XR Interaction Toolkit 3.4.1** — primary interaction framework (grab, poke, teleport)
- **OpenXR + Meta OpenXR 2.5.0** — runtime layer for Quest hardware
- **XR Hands 1.7.3** — hand tracking (used alongside controllers)
- **Interaction profiles**: Meta Quest Touch Controller, Touch Pro, Touch Plus

### Asset Organization

- `Assets/VRTemplateAssets/` — game scripts and core gameplay assets
- `Assets/Scenes/` — MainScene, BasicScene, SampleScene
- `Assets/Samples/` — XR SDK sample assets and XR Interaction Simulator
- Third-party packs: Japanese food/restaurant models, Chemistry Lab environment, QuickOutline

## Version Control Notes

Git LFS is configured for binary Unity assets (scenes, models, materials, textures). When cloning, ensure `git lfs pull` is run to fetch binaries. Without LFS, Unity scenes and asset files will be broken pointer files.

## Game Design Reference

Detailed scene-by-scene specifications are in `documentation/game-specifications.md`. The assignment grading rubric (interactions required, scoring criteria) is in `documentation/general-orientations.md`. Setup tutorials for Unity and VR tooling are in `documentation/tutorial-*.md`.
