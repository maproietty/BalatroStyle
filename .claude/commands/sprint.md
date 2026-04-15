---
description: Full dev sprint — review → plan → build → juice → QA → audit → wrap up + Notion sync
argument-hint: "[optional: feature or system to target, e.g. 'card hover system']"
---

You are running a complete development sprint on this Balatro-style Unity project.
Execute every phase below in order. Do not skip a phase. Do not stop between phases
unless you hit a blocker that requires a developer decision about game direction or
design — describe the blocker clearly and wait for input before continuing.
Technical judgment calls (code structure, file placement, naming) should be made
autonomously using the conventions in CLAUDE_FRAMEWORK.md.

If an argument was passed to this command (e.g. `/sprint "card hover system"`), treat
it as the target feature for phases 2–4. If no argument was given, choose the
highest-priority unchecked item from the current phase in CLAUDE_FRAMEWORK.md.

---

## Phase 1 — Review

Read CLAUDE_FRAMEWORK.md in full.
Also read the "Gameplay Vision & Design Notes" page in the Notion workbook
so the intended player experience informs what gets built this sprint.

Report concisely:
- Current phase name and completion percentage (count [x] vs [ ] items)
- All remaining [ ] items in the current phase
- Any scripts listed under "Scaffolded Scripts" not yet wired into scenes
- All entries in the Known Issues section
- The single highest-priority task you will build this sprint (state it explicitly)
- Any relevant notes from the Gameplay Vision page that apply to this task

Do not modify any files in this phase.

---

## Phase 2 — Plan

Based on the review, write an explicit step-by-step build plan for the chosen task.
The plan must include:
- Which files will be created or modified
- What each file's responsibility is
- How the new code connects to existing systems (via events/delegates, not direct refs)
- Any ScriptableObjects, prefabs, or scene wiring needed after the code is written
- Estimated number of new public methods (each needs XML docs)
- How the Gameplay Vision notes influenced any design decisions in the plan

State the plan clearly before writing any code. This is your contract for the sprint.

---

## Phase 3 — Build

Implement everything in the plan. Follow all coding conventions from CLAUDE_FRAMEWORK.md:

- Namespace: `BalatroStyle` on every class
- `[SerializeField]` for all inspector-exposed fields — no public fields
- XML doc comments on every public method and class
- No magic numbers — use `const` or `[SerializeField]`
- No `Debug.Log` — remove or replace with a conditional logger
- No DOTween — custom coroutines with `Mathf.SmoothStep` for all animation
- Cache all `GetComponent` calls in `Awake()` — never call in `Update()`
- No `FindObjectOfType` in `Update()` loops
- Communicate between systems using `static event Action<...>` delegates
- Subscribe/unsubscribe in `OnEnable`/`OnDisable`
- MonoBehaviours stay thin — extract logic into plain C# classes where possible
- `GameManager` is the only singleton

After writing each file, briefly confirm what it does and how it connects.

---

## Phase 4 — Juice

Read the Key Visual Targets and Color Palette sections of CLAUDE_FRAMEWORK.md.
Also re-read any relevant Gameplay Vision notes (e.g. how the developer described
wanting a moment to feel — "snappy", "satisfying", "weighty").

Evaluate what Balatro feel is missing from the system you just built, then add polish:

- **Easing** — replace any linear lerps with `Mathf.SmoothStep`, EaseOutBack curves,
  or spring physics (position + velocity + damping in `Update`)
- **Screen shake** — trigger `ScreenShake` on impactful moments; scale intensity to
  the magnitude of the event (e.g. score value)
- **Particles** — add chip scatter, spark bursts, or glow trails where the moment
  calls for it; use object pooling if spawning frequently
- **Glow pulses** — flash neon glow on state changes (selection, scoring, dealing)
- **Scale pops** — brief overshoot on appear/select: 1.0 → 1.15 → 1.0 over ~0.2s
- **Timing** — add small coroutine delays between sequential events for dramatic pacing

Use only the approved color palette from CLAUDE_FRAMEWORK.md.
Use coroutines, not DOTween.
Do not add new external dependencies.

---

## Phase 5 — QA

Read `.claude/qa-standards.md` in full.

Review every file created or modified in this sprint against all criteria:

### Code Quality
- All classes inside the `BalatroStyle` namespace
- All public methods have XML doc comments
- No unused `using` statements
- No empty `Start()` or `Update()` stubs
- No magic numbers
- Null checks on `GetComponent` and `Find` calls
- No `Debug.Log` in production code

### Architecture
- MonoBehaviours are thin
- No direct cross-references between unrelated systems
- ScriptableObjects used for data, not hardcoded values
- `GameManager` is the only singleton
- No circular dependencies

### Balatro Visual Standards
- Card hover includes wobble/tilt (not just scale)
- Scoring triggers screen shake
- Number displays use rolling animation
- All UI elements use the approved color palette
- New visual elements feel consistent with the CRT/neon aesthetic

### File Hygiene
- No files outside the approved folder structure
- No orphaned scripts
- Prefabs in `Assets/Prefabs/`, shaders in `Assets/Shaders/`, sprites in `Assets/Sprites/`

### Performance
- No `FindObjectOfType` in `Update` loops
- No string concatenation in `Update` loops
- Object pooling for frequently spawned objects
- No unnecessary `GetComponent` every frame

Fix every issue directly — do not just report it.
After all fixes, state the total number of issues found and fixed.

---

## Phase 6 — Audit

Run a full project-wide sweep across the entire Assets/ folder.
This is not limited to files touched this sprint — it covers everything.
Handle all findings autonomously — fix what is safe, log everything else to
Notion Known Issues in plain language. Do not stop and wait for developer approval.

### Orphaned Scripts
List every .cs file under Assets/Scripts/. For each, verify at least one is true:
- Attached to a GameObject in Main.unity or Menu.unity
- Referenced by another script or prefab
- Explicitly listed under "Scaffolded Scripts" in CLAUDE_FRAMEWORK.md

For any orphaned file: add an // AUDIT FLAG comment at the top and log to Notion.
Do not delete.

### Structural Drift
Compare actual Assets/ layout against approved structure in CLAUDE_FRAMEWORK.md:
- Files in the wrong directory → move automatically if destination is unambiguous
- Missing required folders → create automatically
- Unexpected folders → log to Notion, do not delete

### Temp and Test File Detection
Flag filenames containing: test, temp, old, backup, copy, draft, unused
Also flag .cs files where class name doesn't match filename, and empty files.
Log all findings to Notion. Do not delete.

### Namespace Consistency
Fix any class not inside the `BalatroStyle` namespace directly — always safe to auto-correct.

### Singleton Discipline
Scan for singleton patterns outside GameManager. Log violations to Notion. Do not refactor.

### Duplicate Logic Detection
Identify scripts with overlapping responsibilities. Log suspected duplicates to Notion
with a plain-language explanation. Do not merge or delete.

### Magic Numbers Sweep
Replace obvious magic numbers with named constants directly.
For ambiguous ones, add a // TODO: extract to named constant comment and log to Notion.

### Known Issues Cross-Check
- Mark resolved issues as "Resolved" in both CLAUDE_FRAMEWORK.md and Notion
- Add newly discovered issues to both
- Leave genuinely open issues unchanged

---

## Phase 7 — Wrap Up

### 7a. Update CLAUDE_FRAMEWORK.md
- Mark completed checklist items with [x]
- Add a changelog row: today's date, what was built, QA fix count, audit summary
- Update Architecture Notes if any new design decisions were made
- Update Known Issues to reflect audit findings
- If all items in the current phase are now complete, advance "Current Phase"
  to the next phase

### 7b. Sync to Notion
Using the Notion MCP, update the BalatroStyle workbook:

**Phase Tracker database:**
- For every checklist item completed this sprint, find its row and set:
  Status → "Complete"
  Completed In Sprint → today's date

**Sprint Log database:**
- Create a new row for this sprint:
  Sprint → "Sprint — [today's date]"
  Date → today's date
  What Was Built → one or two sentence plain-language summary
  Juice Added → brief description of visual polish added
  QA Fixes → number of issues found and fixed in Phase 5
  Audit Findings → "[N] auto-fixed, [M] logged to Known Issues"
  Commit Message → the suggested commit from step 7c
  Phase at Time of Sprint → current phase name

**Known Issues database:**
- For every issue resolved this sprint: set Status → "Resolved", add Resolution Notes
- For every new issue found in the audit: create a new row in plain language
  that the developer can understand without coding knowledge

### 7c. Commit Message
Suggest a git commit message in conventional format:
```
feat(system): short description
```

### 7d. Final Sprint Summary
Print a clean end-of-sprint summary:

**Built:** what was implemented
**Juice:** what visual polish was added
**QA:** how many issues were found and fixed
**Audit:** what was auto-fixed vs logged to Notion for awareness
**Notion:** confirm Phase Tracker, Sprint Log, and Known Issues are updated
**Unity Editor steps:** list any manual wiring the developer must do in the
  Unity Editor before the next sprint (plain language, step by step)
**Next sprint:** the next recommended task from the checklist
