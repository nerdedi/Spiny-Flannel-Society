# Animation Quick Start Guide

Having trouble with Unity animations? Here are **3 easy options** to get your game animating immediately.

---

## Option 1: Procedural Animation (EASIEST - No Clips Needed!)

Just add the **SimpleProceduralAnimator** component to your player:

1. Select your Player GameObject
2. Click **Add Component** → search for `SimpleProceduralAnimator`
3. Drag your player's visual mesh to the **Visual Target** field
4. Done! Animations work immediately via code.

The procedural animator handles:
- ✅ Idle breathing/bobbing
- ✅ Walk/run step animations
- ✅ Jump squash & stretch
- ✅ Landing impact
- ✅ Death animation

---

## Option 2: Generate Placeholder Clips (One Click)

Generate simple placeholder animations that work with Unity's Animator system:

1. In Unity menu: **SFS > Setup > Generate All Placeholder Animations (One Click!)**
2. Then run: **SFS > Setup > Assign Animations to Controllers**
3. Done! Your Animator Controllers now have working clips.

The placeholders include:
- Player: Idle, Walk, Run, Jump, Fall, Land, Die
- NPC: Idle variants, Acknowledged, BelongingIdle

---

## Option 3: Use Both (Recommended)

The **PlayerAnimatorDriver** automatically detects which system to use:
- If you have valid animation clips → uses Unity Animator
- If no clips → falls back to SimpleProceduralAnimator

This means you can:
1. Start with procedural animations (works immediately)
2. Add real animation clips later when you're ready
3. The system auto-switches with no code changes

---

## Troubleshooting

### "My character doesn't animate"
- Check if Animator component exists on your character
- Make sure animation clips are assigned to states (run Option 2)
- Or just use Option 1 for instant results

### "Animator has errors about missing clips"
- Run: **SFS > Setup > Generate All Placeholder Animations (One Click!)**
- Then: **SFS > Setup > Assign Animations to Controllers**

### "I want to use my own animations"
1. Import your animation clips
2. Open `Assets/_SFS/Animations/PlayerAnimator.controller`
3. Click each state and assign your clip to the **Motion** field

### "Procedural animations look too subtle/extreme"
Adjust values in the SimpleProceduralAnimator inspector:
- `Idle Bob Amount` - how much vertical movement when idle
- `Walk Bob Amount` - step bounce intensity
- `Jump Squash/Stretch` - exaggeration on jumps

---

## Component Reference

### SimpleProceduralAnimator
Attach to player or NPC. Animates transforms directly via code.
- **Visual Target**: The mesh/model to animate (not the root)
- Automatically called by PlayerAnimatorDriver

### PlayerAnimatorDriver
Bridges player input to animations. Auto-detects best system.
- Works with Unity Animator OR SimpleProceduralAnimator
- Handles story beat integration

### PlaceholderAnimationGenerator (Editor only)
Menu: `SFS > Setup > ...`
- Generates simple animation clips
- Auto-assigns clips to controller states
