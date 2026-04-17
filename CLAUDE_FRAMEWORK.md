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
- [x] Card flip animation (front/back)
- [x] Hand layout system (fan cards in arc)
- [x] Card drag & drop / selection system
- [x] Card wobble/tilt on hover

### Phase 3 — Game Logic
- [ ] Deck data model (52 cards, shuffle, draw)
- [x] Poker hand evaluation system
- [x] Scoring system (chips × multiplier)
- [ ] Round/ante progression system
- [x] Discard mechanic
- [ ] Joker card system (modifiers/special effects)

### Phase 4 — Effects & Juice
- [x] Screen shake on big scores
- [x] Score counter with rolling number animation
- [ ] Particle effects (chips, sparks on scoring)
- [x] Card deal animation (from deck to hand)
- [ ] Played cards slide to scoring area

### Phase 5 — UI & Menus
- [x] HUD layout (score, multiplier, hands left, discards left)
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
- `Scripts/UI/ScoreHUD.cs` — subscribes to scoring + hand-action events; drives rolling total score, chips×mult label, hands/discards remaining, selection count; scale pops on counters, gold color flash on score roll
- `Scripts/UI/HandNameBanner.cs` — reveals played hand's name + chips×mult when `HandScorer.OnHandEvaluated` fires; scale-pop-overshoot reveal → hold → fade+drift retract. `RequireComponent(typeof(CanvasGroup))`
- `Scripts/Game/GameManager.cs` — singleton, state machine, events
- `Scripts/Game/ScoreManager.cs` — chips×multiplier scoring, rolling counter events
- `Scripts/Game/RoundManager.cs` — ante/blind progression
- `Scripts/Game/HandActionController.cs` — selection-cap gating, play/discard pipeline, hotkeys (`Space`=play, `D`=discard), static events `OnHandPlayed`/`OnHandDiscarded`/`OnSelectionCountChanged`/`OnSelectionRejected`
- `Scripts/Game/HandScorer.cs` — subscribes to `HandActionController.OnHandPlayed`, evaluates hand via `PokerHandEvaluator`, applies chips × multiplier to `ScoreManager`, fires `OnHandEvaluated`, requests hand-strength-scaled screen shake
- `Scripts/Cards/HandType.cs` — poker hand enum (HighCard → RoyalFlush), ordered by strength
- `Scripts/Cards/EvaluatedHand.cs` — readonly struct: type, scoring cards, base chips, base multiplier, display name
- `Scripts/Cards/PokerHandEvaluator.cs` — pure static class; `Evaluate(IReadOnlyList<CardData>) → EvaluatedHand`. Balatro base values, wheel-straight support
- `Scripts/Cards/CardData.cs` — ScriptableObject for card definition
- `Scripts/Cards/Deck.cs` — shuffle, draw, discard
- `Scripts/Cards/Hand.cs` — fan layout, card management, `RefillTo(Deck, handSize)` with staggered reveal
- `Scripts/Cards/Card.cs` — hover wobble (spring physics), flip with reveal pop+glow, cap-aware selection (`TrySetSelected`, `BounceReject`), static events `OnCardRevealed`/`OnSelectToggleRequested`/`OnSelectionChanged`
- `Scripts/Effects/ScreenShake.cs` — auto-shakes on score events + static `ScreenShake.Request(amp, dur)` channel
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
- **Card reveal event** — `Card.OnCardRevealed` (static `Action<Card>`) fires after a face-up flip completes. Subscribe in future systems (e.g. discovery log, tutorial highlight) without coupling to `Hand` or `Deck`.
- **Deal flow** — `Hand.DealCards` instantiates cards face-down, fans them into the arc, waits `dealFlipDelay`, then flips each card with `dealStagger` between flips. The cascading reveal is the sprint's juice signature.
- **Selection pipeline** — `Card.OnMouseDown` fires `OnSelectToggleRequested` (not a direct toggle). `HandActionController` subscribes, enforces `maxSelected` cap (default 5), and calls `Card.TrySetSelected`. Rejected selections trigger `Card.BounceReject` for feedback. This keeps `Card` ignorant of hand-wide rules.
- **Play / Discard pipeline** — `HandActionController.PlaySelected` / `DiscardSelected` consume counts on `GameManager`, remove cards via `Hand.RemoveSelected`, push discards to `Deck`, fire `OnHandPlayed` / `OnHandDiscarded`, then `Hand.RefillTo(deck, handSize)` tops the hand back up with a staggered reveal.
- **Decoupled shake channel** — `ScreenShake.Request(amplitude, duration)` is a static convenience that fires `OnShakeRequested`; any active `ScreenShake` instance handles it. Used by `HandActionController` on play without a direct reference.
- **Hand evaluation pipeline** — `HandActionController.OnHandPlayed(List<CardData>)` → `HandScorer.HandleHandPlayed` → `PokerHandEvaluator.Evaluate` → `ScoreManager.ApplyScore(chips, mult)`. `HandScorer` also fires `OnHandEvaluated(EvaluatedHand)` for future hand-name banners and scales `ScreenShake.Request` by hand strength (SmoothStep lerp across the HandType enum). Evaluator is pure C# — no Unity deps — so it is testable and reusable.
- **Balatro-authentic scoring** — Final score = (`BaseChips` from hand type + sum of each scoring card's `ChipValue`) × `BaseMultiplier`. Only cards that participate in the hand score (e.g. only the pair in a Pair hand), matching Balatro's behavior and setting up per-card chip-fly juice later.
- **HUD pipeline** — `ScoreHUD` is a pure listener: subscribes to `ScoreManager.OnScoreChanged` (for chips×mult + reset detection), `HandScorer.OnHandEvaluated` (authoritative source of the displayed delta — avoids race with `ScoreManager.TotalScore` which is assigned after the `OnScoreChanged` fire), `HandActionController.OnHandPlayed`/`OnHandDiscarded`/`OnSelectionCountChanged`, and `GameManager.OnRoundStarted`. Holds a single `[SerializeField] ScoreManager` ref (same sanctioned pattern as GameManager/RoundManager) solely to read `TotalScore` during `RefreshAll`. `HandNameBanner` subscribes only to `HandScorer.OnHandEvaluated` — decoupled from scoring state.
- **Audio deferred** — No audio systems, managers, or placeholder hooks. Will be added in a future update.

---

## Known Issues
- CRT and Glow shaders are written but not yet assigned in the Unity Editor — needs a Renderer Feature or Material setup.
- `Card.cs` uses `OnMouseDown`/`OnMouseEnter` — requires a Box Collider 2D on the Card prefab.
- `CameraEffects.cs` requires a URP Global Volume with Bloom, Vignette, and ChromaticAberration overrides.
- `GameManager` and `RoundManager` each have a `[SerializeField] private ScoreManager scoreManager` — these must be wired up in the scene Inspector before play.
- `Assets/Settings/` folder (Unity 6 URP auto-generated) is not listed in the approved folder structure; safe to keep, noted for awareness.
- `Assets/Audio/Music/` and `Assets/Audio/SFX/` folders exist but are empty — leftover scaffolding that contradicts the audio-deferred policy. Safe to leave or delete in Editor.
- Card prefab needs `backRenderer` and `glowRenderer` references assigned in the Inspector for the flip and reveal-glow effects to render correctly.
- `HandScorer` component needs to be added to a GameObject (e.g. GameManager) and its `ScoreManager` reference wired in the Inspector.
- `Assets/Resources/Cards/` contains only 5 of 52 `CardData` assets; the poker evaluator can only produce meaningful hand classifications once the full deck is populated.
- `Assets/Scenes/SampleScene/` folder (containing `Global Volume Profile.asset`) is a leftover from the original SampleScene. Safe to leave if referenced by the Main camera's URP Volume, but the folder name no longer matches any scene.
- `ScoreHUD` + `HandNameBanner` need scene wiring: create a world-space or screen-space Canvas, add TMP text labels for total score, chips×mult, hands remaining, discards remaining, selection count, plus a banner RectTransform (with `CanvasGroup`) for the hand-name popup. Wire all `[SerializeField]` refs in the Inspector.
- Minor DRY opportunity: `HandScorer`, `ScoreHUD`, and `HandNameBanner` each loop `EvaluatedHand.ScoringCards` to compute total chips. Candidate for a computed `TotalChips` property on `EvaluatedHand` in a later sprint. Safe to defer.

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
| 2026-04-16 | Card flip system: `Card.Flip` coroutine + `OnCardRevealed` event + staggered deal-then-reveal in `Hand`. Juice: scale-pop + glow pulse on reveal. QA: 1 fix (removed unused `sr` field). Audit: 3 findings logged to Notion. | 2         |
| 2026-04-16 | QA sweep: CardData fields → `[SerializeField]` private + `FormerlySerializedAs`; CameraEffects BloomPulse linear→SmoothStep; removed orphaned `RequireComponent` from Card; removed unused `targetAberration` field + extracted magic number in CameraEffects. QA: 5 issues fixed. | 1–2 |
| 2026-04-17 | Selection + play/discard pipeline: `HandActionController` coordinates cap-aware selection (max 5) via `Card.OnSelectToggleRequested`; static events `OnHandPlayed`/`OnHandDiscarded`/`OnSelectionCountChanged`/`OnSelectionRejected`. `Hand.RefillTo` tops up from Deck with staggered reveal. Juice: `SelectPop` scale overshoot, `BounceReject` recoil on cap, static `ScreenShake.Request` impulse on play. QA: 1 fix (SelectPop used hardcoded `Vector3.one` baseScale, snapped hovered cards). Audit: 0 auto-fixed, 2 logged (new TestScene.unity, Unity 6 Settings/ folder). | 2 |
| 2026-04-17 | Hotfix: `ScreenShake.cs` — adding `using System;` for `Action<float,float>` event introduced `System.Random`/`UnityEngine.Random` ambiguity at `ShakeCoroutine` lines 65–66 (CS0104). Fully-qualified the two `UnityEngine.Random.Range` calls to resolve. | 2 |
| 2026-04-17 | Poker hand evaluation + scoring bridge: `HandType` enum, `EvaluatedHand` struct, pure static `PokerHandEvaluator` (Balatro base values, wheel-straight support), `HandScorer` MonoBehaviour subscribing to `OnHandPlayed` → `ScoreManager.ApplyScore`. Juice: hand-strength-scaled screen shake (SmoothStep lerp, 0.05→0.35 amplitude), `OnHandEvaluated` event hook for future hand-name banner. QA: 0 issues. Audit: 0 auto-fixed, 2 new items logged (HandScorer scene wiring, 5/52 CardData assets); resolved scene-file-mismatch issue (Main.unity + Menu.unity now present). | 3 |
| 2026-04-17 | Hotfix: `PokerHandEvaluator.cs` — `out bool isRoyal` declared inline inside a short-circuited `&&` meant the `out` param was not definitely assigned when count≠5 (CS0165). Hoisted `bool isRoyal = false;` to a separate line before the `IsStraight` call. | 3 |
| 2026-04-17 | HUD system: `ScoreHUD` (rolling total score, chips×mult, hands/discards remaining, selection count; scale pops on counter changes; gold color flash on score roll) + `HandNameBanner` (RequireComponent CanvasGroup; scale-overshoot reveal → hold → fade+drift retract). Event-only coupling: subscribes to `ScoreManager.OnScoreChanged`, `HandScorer.OnHandEvaluated`, `HandActionController` events, `GameManager.OnRoundStarted`. QA: 0 issues. Audit: 0 auto-fixed, 2 new items logged (ScoreHUD/HandNameBanner scene wiring, DRY opportunity on EvaluatedHand.TotalChips); resolved prior TestScene.unity item (file no longer present). | 4–5 |

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
