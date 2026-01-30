# Spiny Flannel Society - Unity Project

## 🎮 Quick Start Guide

### Opening the Project
1. Open **Unity Hub**
2. Click **Add** → **Add project from disk**
3. Navigate to this folder and select it
4. Unity will import and set up the project (first import takes a few minutes)
5. Unity version: **2022.3 LTS** or newer recommended

### First Time Setup
After Unity finishes importing:

1. **Install Required Packages** (should auto-install via manifest.json):
   - Cinemachine
   - Timeline
   - TextMeshPro
   - Input System

2. **Run Full Project Setup**:
   - Menu: `SFS → Setup → Full Project Setup`
   - This creates animator controllers and configures everything

3. **Open the Main Scene**:
   - `Assets/_SFS/Scenes/SFS_MainScene.unity`

4. **Press Play** to test!

---

## 🏗️ Project Structure

```
Assets/_SFS/
├── Animations/           # Animator controllers
│   ├── PlayerAnimator.controller
│   └── NPCAnimator.controller
├── Materials/            # URP materials
│   ├── PlayerMaterial.mat
│   ├── GroundMaterial.mat
│   └── CollectibleMaterial.mat
├── Scenes/               # Game scenes
│   └── SFS_MainScene.unity
└── Scripts/              # All game code
    ├── Audio/            # Sound systems
    ├── Camera/           # Camera controls
    ├── Core/             # Events, managers, data
    ├── Editor/           # Unity editor tools
    ├── Narrative/        # Story systems, NPCs
    ├── Player/           # Player controller
    ├── UI/               # User interface
    ├── Utility/          # Debug tools
    └── World/            # Zones, collectibles
```

---

## 🎭 The 7 Story Beats

The game progresses through 7 emotional beats:

| Beat | Name | Tone | Zone Position |
|------|------|------|---------------|
| 1 | **Arrival** | Gentle | Z: 0 |
| 2 | **First Contact** | Melancholic | Z: 60 |
| 3 | **Compression** | Gentle | Z: 130 |
| 4 | **Choosing Rest** | Hopeful | Z: 200 |
| 5 | **The Society** | Melancholic | Z: 270 |
| 6 | **Shared Difficulty** | Hopeful | Z: 340 |
| 7 | **Quiet Belonging** | Grounded | Z: 420 |

---

## 🎹 Controls

| Action | Key |
|--------|-----|
| Move | WASD / Arrow Keys |
| Jump | Space |
| Pause | Escape |
| Debug Display | F1 |
| Skip to Beat | Shift + 1-7 |

---

## 🔧 Debug Tools

- **F1**: Toggle beat/tone display
- **Shift + 1-7**: Teleport to specific story beat
- Menu: `SFS → Build Test Scene` - Regenerate complete test scene

---

## ✨ Key Features

### Neuroaffirming Design
- **Coyote Time**: 0.2s grace period after leaving ledges
- **Jump Buffer**: 0.15s input buffering
- **Reduced Motion**: Settings option for camera smoothing
- **Beat 6 Support**: Extra timing forgiveness when companion is present

### Animation System
- Animator parameters respond to story beats
- `IdleIntensity` varies by emotional tone (0.3 gentle → 0.7 intense)
- `EmotionTone` integer maps to current tone
- All transitions use damped smoothing

### Audio Layers
- Base ambient layer
- Tension layer (beats 3, 5)
- Calm layer (beats 4, 7)
- Society layer (beats 5, 6, 7)

---

## 📋 Creating Custom Content

### Adding a New Zone
1. Create empty GameObject with BoxCollider (IsTrigger = true)
2. Add `StoryBeatZone` component
3. Set the `beat` field to desired story beat
4. Add specialized zone scripts as needed:
   - `RestZone` for Beat 4
   - `CompressionZone` for Beat 3
   - `SupportedMovementZone` for Beat 6

### Adding Society Members
1. Create character mesh/model
2. Add `Animator` with NPCAnimator controller
3. Add `SocietyMember` script
4. Tag as "SocietyMember"
5. Set on Layer 10 (NPC)

### Custom Cutscenes
1. Menu: `SFS → Create Timeline Template`
2. Select beat and customize
3. Use `CutsceneTrigger` to activate

---

## 🐛 Troubleshooting

**Missing Scripts**: Run `SFS → Setup → Full Project Setup`

**No Movement**: Check Player has CharacterController component

**Camera Issues**: Verify camera has `ThirdPersonCameraRig` and target is set

**No Audio**: Add AudioSource to AudioController object

**Animator Warnings**: Run `SFS → Setup Animator Controllers`

---

## 📄 License

Created for the Spiny Flannel Society project.
A neuroaffirming 3D platformer exploring belonging through gentle gameplay.
