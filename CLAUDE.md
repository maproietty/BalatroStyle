# BalatroStyle — Claude Code Orientation

You are helping build a Balatro-inspired poker card game in Unity 6 (2D URP).
This file is auto-loaded at the start of every session. Read it first, then
read CLAUDE_FRAMEWORK.md before touching anything.

---

## What This Project Is

A single-player card game with poker hand mechanics, a chip×multiplier scoring
system, and a heavy retro-arcade visual identity: CRT scanlines, neon glow,
screen shake, satisfying card physics. Think Balatro — dark, juicy, addictive.

## Source of Truth

**CLAUDE_FRAMEWORK.md** — read this every session before making any changes.
It contains the folder structure, coding conventions, current phase checklist,
architecture notes, known issues, and changelog.

**design-doc.md** — read this every session alongside the framework. It is a
local snapshot of the Notion page "BalatroStyle - Design Document" and contains
the locked-in design pillars (combo builder, 7-card Hold'em selection, runaway
thresholds, chaptered Nights, chasing-the-dragon loop), the three charm families
(Psychedelic / Smoke / Chemical), the two-currency economy, dealer manipulation
surfaces, and the drug-tinged occult parlor theme. Let it shape *how* you build,
not just *what*. If it ever conflicts with the framework, the design doc wins —
update the framework to match.

## Every Session Start

1. Read CLAUDE_FRAMEWORK.md
2. Read design-doc.md
3. Run /sprint (or whatever command the developer invokes)

Do not write a single line of code before reading both files.

## Core Rules (quick ref — full detail in CLAUDE_FRAMEWORK.md)

- Namespace: `BalatroStyle` on every class
- No DOTween — coroutines with `Mathf.SmoothStep` only
- No `Debug.Log` in production code
- `GameManager` is the only singleton
- Events/delegates for all cross-system communication
- `[SerializeField]` instead of public fields
- Cache `GetComponent` in `Awake()` — never in `Update()`
- XML doc comments on every public method

## What the Developer Handles

The developer is not a programmer. They handle Unity Editor tasks only:
wiring prefabs, assigning materials, adding components, hitting Play to test.
Claude Code handles all code, file creation, and project structure.
When Unity Editor steps are required, list them clearly in plain English,
numbered, at the end of the sprint summary.

## Audio

Fully deferred to post-launch. Do not add any audio systems, managers,
placeholders, or hooks under any circumstances.
