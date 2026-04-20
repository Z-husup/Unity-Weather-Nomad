<img width="1598" height="1200" alt="image" src="https://github.com/user-attachments/assets/ef830537-1c66-406c-9bfa-74173ff2a515" /># 🌍 Weather Nomads

*A small atmospheric game about restoring balance in a living world.*

---

## 🎮 About the Game

**Weather Nomads** is a minimalistic simulation where the player controls wandering priests who influence the world through elemental forces.

<img width="1536" height="1024" alt="Intro" src="https://github.com/user-attachments/assets/e0a805d9-853a-47f6-abcb-c6c5e29141ec" />

Each action changes the environment:

- 🌧 Rain can restore life — or flood cities  
- 🔥 Fire can remove floods — or cause drought  

Your goal is simple:

> **Stabilize all regions and restore balance to the planet.**

---
<img width="1536" height="1024" alt="Inspiration" src="https://github.com/user-attachments/assets/d5ec4beb-122e-44a3-8e98-973056d2c074" />


## 🧠 Core Gameplay

- Switch between different priests (Rain / Fire)
- Move across a spherical planet
- Interact with regions using aura effects
- Manage consequences of your actions
- Balance destruction and restoration

---

## 🌍 Region System

The world is divided into dynamically generated regions:

### Types:
- Grassland
- City

### States:
- Normal
- Drought (grassland)
- Flood (city)

### Interactions:

| Action              | Result           |
|--------------------|------------------|
| Rain → Drought     | Restores grass   |
| Fire → Grass       | Causes drought   |
| Rain → City        | Causes flood     |
| Fire → Flood       | Restores city    |

---
<img width="1536" height="1024" alt="tutorial" src="https://github.com/user-attachments/assets/88b1b2b4-a0bc-4724-82db-9bc66d9cf0be" />

## 🧩 Features

- 🌐 Procedural planet region generation (Fibonacci sphere)
- 🎭 State-based visual system (Grass / Drought / City / Flood)
- ✨ Smooth transitions between states
- 📱 In-game messenger system with feedback
- 📊 Real-time region status tracking (Stable / In Danger)
- 🧭 Intro screens (Start / Inspiration / Tutorial)
- 🏁 End-game condition with finish screen

---

## 🛠️ Tech Stack

- Unity (WebGL build)
- C#
- TextMeshPro
- Custom UI system

---

## 🚀 How to Run

### Option 1 — Unity
