---
description: Design session — brainstorm mechanics, capture decisions, update vision. No code written.
argument-hint: "[topic to explore, e.g. 'joker system' or 'scoring progression']"
---

You are running a design-only session. No code will be written. No files
outside of vision.md will be modified. This session is purely for thinking,
discussing, and capturing design decisions.

---

## Step 1 — Orient

Read CLAUDE_FRAMEWORK.md to understand the current phase and what's been built.
Read vision.md in full to understand existing design intentions.

Report briefly:
- What phase the project is in and what's been built so far
- Any relevant design notes already in vision.md that relate to today's topic
- The topic you'll be exploring this session (from the argument, or ask the developer)

---

## Step 2 — Explore

Lead a focused design conversation with the developer on the chosen topic.

Your role is to ask good questions, offer options with trade-offs, and help
the developer make decisions they're happy with. You are a collaborator, not
an authority — the developer's instincts and preferences always win.

Useful approaches depending on the topic:

**For mechanics (jokers, scoring, combos):**
- Present 2–3 distinct design directions with a plain-language description of how each feels to play
- Ask which direction resonates and why
- Narrow down to one and flesh it out with specifics

**For progression (antes, difficulty, leveling):**
- Walk through what a typical session arc would feel like under different systems
- Ask what frustration and satisfaction should look like at different stages
- Confirm win/loss conditions and pacing

**For visual moments:**
- Ask the developer to describe the moment in plain language first
- Translate that into what the screen would actually do (shake intensity, particle type, sound cue placeholder, animation timing)
- Confirm it matches what they imagined

**For open questions in vision.md:**
- Surface them one at a time
- Work through each with the developer until it's decided or deliberately left open

Do not move to Step 3 until the developer signals they're satisfied with the discussion.

---

## Step 3 — Capture

Update vision.md with everything decided in this session:
- Write decisions in plain language under the relevant section
- Move resolved open questions out of "Open Questions" into the appropriate section
- Add any new open questions that came up
- Do not overwrite what the developer has already written — append or refine only

After updating, show the developer a summary of exactly what changed in vision.md
so they can confirm it reflects what they intended.

---

## Step 4 — Flag for Sprint

If any decisions made today have direct implications for upcoming build work,
list them clearly:

- What was decided
- Which phase or system it affects
- Whether it changes anything already built (flag as a known issue if so)

These notes will be picked up automatically by the next /sprint.
Do not make any code changes. The sprint will handle implementation.
