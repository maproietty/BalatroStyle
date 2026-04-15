---
description: One-time setup — build the BalatroStyle Notion workbook from the project framework
---

Run this command once to scaffold the entire Notion workspace from CLAUDE_FRAMEWORK.md.
Do not run again after setup — use /sprint to keep Notion updated going forward.

---

## Step 1 — Read the Framework

Read CLAUDE_FRAMEWORK.md in full. You will use it as the source of truth for
everything created in Notion. Extract:
- All development phases and their names
- Every checklist item under each phase, noting which are [x] complete and which are [ ] open
- All Known Issues
- The full Changelog table
- All Scaffolded Scripts listed under Current Phase

---

## Step 2 — Create the Workbook Structure

Using the Notion MCP, create the following pages and databases inside a top-level
Notion page called "BalatroStyle — Dev Tracker".

### 2a. Phase Tracker (database)
One row per checklist item across all phases. Columns:
- Task (title) — the checklist item text
- Phase — which phase it belongs to (Phase 0, Phase 1, etc.)
- Status — "Complete" or "To Do" (map from [x] and [ ] in the framework)
- Completed In Sprint — leave blank for now; /sprint will fill this in
- Notes — any relevant architecture or wiring notes

Populate every row from the framework checklist now.

### 2b. Sprint Log (database)
One row per sprint run. Columns:
- Sprint (title) — auto-named by date, e.g. "Sprint — 2026-04-11"
- Date
- What Was Built — summary of the feature or system
- Juice Added — visual polish added this sprint
- QA Fixes — number of issues found and fixed
- Audit Findings — number of issues auto-fixed, number logged for awareness
- Commit Message — the suggested git commit from /done
- Phase at Time of Sprint — which phase was active

Leave this database empty for now — /sprint will populate it.

### 2c. Known Issues (database)
One row per known issue. Columns:
- Issue (title) — description of the issue
- Status — "Open" or "Resolved"
- Affected File(s) — which script or asset is involved
- Resolution Notes — filled in when resolved

Populate from the Known Issues section of CLAUDE_FRAMEWORK.md now.

### 2d. Gameplay Vision (page)
A blank page titled "Gameplay Vision & Design Notes".
Add a single placeholder line: "Add your gameplay vision, feel, and design intentions here."
This page is for the developer to write in — Claude reads it at the start of each sprint
to understand the intended player experience before building.

---

## Step 3 — Confirm Setup

After creating all databases and pages, report:
- The name and Notion URL of each database created
- The total number of rows populated in Phase Tracker
- The total number of rows populated in Known Issues
- Confirm the Sprint Log is empty and ready

Then print these instructions for the developer:

---
SETUP COMPLETE. Here is what was created in Notion:

1. Phase Tracker — mirrors your CLAUDE_FRAMEWORK.md checklist.
   Every sprint will automatically mark completed items here.

2. Sprint Log — a running record of every sprint.
   Date, what was built, QA results, and commit message logged automatically.

3. Known Issues — mirrors your Known Issues section.
   Resolved issues will be marked automatically after each sprint audit.

4. Gameplay Vision — a page for you to write in freely.
   Describe how you want the game to feel, what moments should be satisfying,
   what the card selection should feel like, anything. Claude reads this before
   building each sprint so your intentions shape the code, not just the checklist.

You do not need to maintain Notion manually. /sprint handles all updates.
---
