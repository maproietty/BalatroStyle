# CLAUDE_FRAMEWORK.md
# Balatro-Style Unity Game — Claude Code Reference

## Purpose
This is the single source of truth for Claude Code when working on this project. Read this file at the start of every session before making any changes. After completing work, update the **Current Phase** and **Changelog** sections below.

---

## Project Summary
- **Game:** A poker-based card game with Balatro-inspired visuals
- **Engine:** Unity 6 LTS, 2D URP (Universal Render Pipeline)
- **Language:** C#
- **Visual Style:** CRT scanlines, pixel art, neon glows, dark backgrounds, retro-arcade aesthetic, screen shake, satisfying card physics
- **Audio:** Deferred — no audio work until post-launch update

---

## Folder Structure
```
BalatroStyle/
├── CLAUDE_FRAMEWORK.md          # This file — project source of truth
├── .gitignore
├── .claude/
│   ├── qa-standards.md          # QA review criteria (do not modify)
│   └── commands/
│       ├── status.md            # /status — show project state
│       ├── done.md              # /done — finish task + update framework + QA
│       ├── qa.md                # /qa — strict code review
│       ├── review.md            # /review — architecture + pattern check
│       └── juice.md             # /juice — add visual polish to a system
├── Assets/
│   ├── Scenes/                  # Main.unity, Menu.unity
│   ├── Scripts/
│   │   ├── Cards/               # Card.cs, Deck.cs, Hand.cs, CardData.cs
│   │   ├── Game/                # GameManager.cs, ScoreManager.cs, RoundManager.cs
│   │   ├── UI/                  # HUD.cs, MenuController.cs
│   │   └── Effects/             # ScreenShake.cs, CameraEffects.cs
│   ├── Shaders/                 # CRT.shader, Glow.shader
│   ├── Sprites/
│   │   ├── Cards/               # card_front.png, card_back.png, suits/
│   │   └── UI/                  # buttons, panels, icons
│   ├── Prefabs/                 # Card.prefab, Chip.prefab
│   └── Resources/               # Runtime-loaded assets
└── ProjectSettings/             # Input, Quality, Graphics, URP settings
```

---

## Claude Code Commands
Custom slash commands live in `.claude/commands/`. Use them as needed:

| Command   | Purpose                                                    |
|-----------|------------------------------------------------------------|
| `/status` | Print current phase, open tasks, known issues              |
| `/done`   | Wrap up a task: update framework, run QA, commit message   |
| `/qa`     | Strict code review against qa-standards.md                 |
| `/review` | Architecture review — coupling, patterns, scalability      |
| `/juice`  | Add visual polish (particles, easing, shake, glow) to a system |

QA standards are in `.claude/qa-standards.md` — read-only reference, do not modify.

---

## Coding Conventions
- **Namespace:** `BalatroStyle`
- **MonoBehaviour scripts** use `PascalCase` filenames matching class names
- Use `[SerializeField]` for inspector-exposed private fields
- Use `ScriptableObject` for card data definitions (suits, ranks, special properties)
- Prefer custom coroutines for animations (no DOTween — zero external dependencies)
- Keep game state in a central `GameManager` singleton
- Use **events/delegates** for decoupled communication between systems
- Comment all public methods and complex logic
- Keep MonoBehaviours thin — extract logic into plain C# classes where possible
- Cache `GetComponent` calls in `Awake()`/`Start()` — never call in `Update()`
- No `FindObjectOfType` in `Update()` loops
- No magic numbers — use `const` or `[SerializeField]`

---

## Key Visual Targets
These effects define the Balatro feel. Prioritize them in this order:

1. **CRT Post-Processing** — scanlines, subtle barrel distortion, vignette, slight color bleed
2. **Card Hover Wobble** — cards tilt toward cursor with spring physics, slight scale-up on hover
3. **Neon Glow** — UI elements and selected cards emit colored glow (bloom + emissive materials)
4. **Screen Shake** — on scoring, intensity scales with score magnitude
5. **Rolling Numbers** — score counters tick up rapidly with easing, not instant
6. **Chip Particles** — poker chips scatter with physics on big scores
7. **Dark Palette** — deep greens (#1a1a2e), purples (#16213e), blacks (#0f0f23) with bright accent pops (gold #f0c040, red #e74c3c, blue #3498db)
8. **Animated Background** — subtle swirling/pulsing pattern behind the play area

---

## Color Palette
```
Background Dark:    #0f0f23
Background Mid:     #1a1a2e
Background Light:   #16213e
Card White:         #f5f5f0
Card Red:           #e74c3c
Card Black:         #2c2c2c
Gold Accent:        #f0c040
Blue Accent:        #3498db
Green Accent:       #2ecc71
Purple Accent:      #9b59b6
Neon Glow:          #00ff88
Score Text:         #ffffff
Multiplier Text:    #ff6b6b
```

---

## Development Phases

### Phase 0 — Setup ✓ COMPLETE
- [x] Install Unity Hub + Unity 6 LTS
- [x] Create new 2D (URP) project named BalatroStyle
- [x] Initialize Claude Code in project folder
- [x] Set up folder structure
- [x] Create .gitignore
- [ ] Initialize Git repo (user to run `git init`)

### Phase 1 — Core Visuals (CURRENT)
- [x] CRT shader (Assets/Shaders/CRT.shader)
- [x] Neon Glow shader (Assets/Shaders/Glow.shader)
- [x] Animated background shader + fit script (Assets/Shaders/AnimatedBackground.shader, Assets/Scripts/Effects/AnimatedBackground.cs)
- [x] Create Quad GameObject with AnimatedBackground material + script — Unity Editor
- [ ] Assign CRT shader as a Renderer Feature (URP Full Screen Pass) — Unity Editor
- [ ] Set up pixel-perfect camera and URP Volume (Bloom, Vignette, Chromatic Aberration)

### Phase 2 — Card System
- [ ] Design card sprite (front face with suit/rank)
- [ ] Design card back sprite
- [ ] Build Card prefab with hover/select animations
- [ ] Card flip animation (front/back)
- [ ] Hand layout system (fan cards in arc)
- [ ] Card drag & drop / selection system
- [ ] Card wobble/tilt on hover

### Phase 3 — Game Logic
- [ ] Deck data model (52 cards, shuffle, draw)
- [ ] Poker hand evaluation system
- [ ] Scoring system (chips × multiplier)
- [ ] Round/ante progression system
- [ ] Discard mechanic
- [ ] Joker card system (modifiers/special effects)

### Phase 4 — Effects & Juice
- [ ] Screen shake on big scores
- [ ] Score counter with rolling number animation
- [ ] Particle effects (chips, sparks on scoring)
- [ ] Card deal animation (from deck to hand)
- [ ] Played cards slide to scoring area

### Phase 5 — UI & Menus
- [ ] HUD layout (score, multiplier, hands left, discards left)
- [ ] Main menu screen
- [ ] Game over / results screen
- [ ] Retro-styled font integration

### Phase 6 — Polish & Ship
- [ ] Playtesting pass — tune scoring balance
- [ ] Performance optimization
- [ ] Build executable (Windows/Mac)

### Deferred — Audio (post-launch update)
- Background music, card SFX, score SFX, UI sounds

---

## Current Phase
**Phase 1 — Core Visuals** (in progress)

### Scaffolded Scripts (ready, need wiring in scenes)
- `Scripts/Game/GameManager.cs` — singleton, state machine, events
- `Scripts/Game/ScoreManager.cs` — chips×multiplier scoring, rolling counter events
- `Scripts/Game/RoundManager.cs` — ante/blind progression
- `Scripts/Cards/CardData.cs` — ScriptableObject for card definition
- `Scripts/Cards/Deck.cs` — shuffle, draw, discard
- `Scripts/Cards/Hand.cs` — fan layout, card management
- `Scripts/Cards/Card.cs` — hover wobble (spring physics), selection, move coroutine
- `Scripts/Effects/ScreenShake.cs` — auto-shakes on score events
- `Scripts/Effects/CameraEffects.cs` — bloom pulse, chromatic aberration on score
- `Scripts/Effects/AnimatedBackground.cs` — scales background Quad to cover the camera each frame (pairs with AnimatedBackground.shader)

---

## Architecture Notes
- **ScriptableObjects for card data** — `CardData.cs` defines suit, rank, chip value, sprite. Create 52 assets in Editor and drag into `Deck.allCards`.
- **No DOTween dependency** — all animations use custom coroutines with `Mathf.SmoothStep`. Keeps the project dependency-free.
- **Event-driven architecture** — `GameManager`, `ScoreManager`, `RoundManager` communicate via `static event Action<...>` delegates. MonoBehaviours subscribe in `OnEnable`/`OnDisable`.
- **CRT effect implementation** — Written as a URP HLSL shader. Must be added as a "Full Screen Pass Renderer Feature" in the URP Renderer asset (Settings/Renderer2D.asset). Create a Material using this shader and assign it there.
- **Spring physics for card wobble** — Implemented directly in `Card.cs Update()` — no physics engine needed.
- **Only one singleton** — `GameManager` is the sole singleton. `ScoreManager` and `RoundManager` are regular MonoBehaviours; `GameManager` and `RoundManager` each hold a `[SerializeField] private ScoreManager` reference wired in the scene.
- **Win condition event** — `RoundManager.OnAllAntesCleared` fires when all 8 antes are cleared. Subscribe in a future win-screen script.
- **Audio deferred** — No audio systems, managers, or placeholder hooks. Will be added in a future update.

---

## Known Issues
- CRT and Glow shaders are written but not yet assigned in the Unity Editor — needs a Renderer Feature or Material setup.
- `Card.cs` uses `OnMouseDown`/`OnMouseEnter` — requires a Box Collider 2D on the Card prefab.
- `CameraEffects.cs` requires a URP Global Volume with Bloom, Vignette, and ChromaticAberration overrides.
- `GameManager` and `RoundManager` each have a `[SerializeField] private ScoreManager scoreManager` — these must be wired up in the scene Inspector before play.

---

## Changelog
| Date       | Change                                                                                        | Phase     |
|------------|-----------------------------------------------------------------------------------------------|-----------|
| 2026-04-11 | Project created, Notion workspace set up                                                      | 0 - Setup |
| 2026-04-11 | Folder structure, .gitignore, core scripts, CRT+Glow shaders                                 | 0→1       |
| 2026-04-11 | Audio deferred to post-launch; slash commands + QA system created                            | 0→1       |
| 2026-04-11 | QA pass on scaffold: 22 issues fixed (extra singletons, magic numbers, Debug.Log, XML docs)   | 1         |
| 2026-04-14 | Animated background: palette-matched swirl shader + camera-fit script (edit-mode preview)     | 1         |
| 2026-04-14 | Background Quad wired in scene; QA: 1 issue fixed (removed unused cached MeshRenderer field)  | 1         |

---

## Claude Code Session Prompt
Paste this at the start of each Claude Code session:

```
You are helping me build a Balatro-style card game in Unity 6 (2D URP).
Read the CLAUDE_FRAMEWORK.md file in the project root for full context
on folder structure, coding conventions, current progress, and known issues.
Always check that file before making changes. After completing work, update
the Current Phase, Changelog, and Architecture Notes sections as needed.
The project has a QA supervisor in .claude/qa-standards.md — all code must
pass those standards. Run /done [task name] when finishing any task.
```
