---
description: Full project audit — scan entire Assets/ folder for errors, orphaned
             files, structural drift, and stale known issues. Logs findings to Notion.
---

Read CLAUDE_FRAMEWORK.md and .claude/qa-standards.md in full before starting.

This is a whole-project sweep, not a session-scoped review. Scan every file under
Assets/Scripts/ and cross-reference against the approved structure and conventions
in CLAUDE_FRAMEWORK.md. Do not limit scope to recently changed files.

IMPORTANT: This audit is designed to run without requiring developer input.
Fix everything that is safe to fix automatically. For anything ambiguous or
potentially destructive, log it clearly to the Notion Known Issues database
with a plain-language explanation — do not block the sprint waiting for approval.
The developer will review flagged items in Notion at their own pace.

---

## Step 1 — Orphaned Scripts

List every .cs file under Assets/Scripts/. For each, verify at least one is true:
- Attached to a GameObject in Main.unity or Menu.unity
- Referenced by another script or prefab
- Explicitly listed under "Scaffolded Scripts" in CLAUDE_FRAMEWORK.md

For any file that fails all three:
- Do not delete it
- Add a comment block at the top of the file:
  // AUDIT FLAG: This script has no confirmed attachment or reference.
  // Review in Unity Editor — attach to a GameObject or delete if unused.
- Log it in the Notion Known Issues database:
  Issue: "Orphaned script — [filename]"
  Status: Open
  Affected File: [path]
  Resolution Notes: "No scene attachment or reference found. Verify in Unity Editor."

---

## Step 2 — Structural Drift

Compare the actual Assets/ folder layout against the approved structure in
CLAUDE_FRAMEWORK.md. For each mismatch found:

- If a file is in the wrong directory and the correct destination is unambiguous:
  Move it automatically and log the move in the audit report.

- If a required folder is missing:
  Create it automatically.

- If a folder exists but is not in the approved structure:
  Do not delete it. Log it in Notion Known Issues with a plain explanation:
  "Unexpected folder found at [path] — may be safe to remove, review when convenient."

---

## Step 3 — Temp and Test File Detection

Scan all files under Assets/ and flag anything where the filename contains:
test, temp, old, backup, copy, draft, unused

Also flag:
- .cs files where the class name doesn't match the filename
- Empty files (0 bytes or whitespace only)

For each match:
- Do not delete it
- Log in Notion Known Issues:
  Issue: "Possible temp/test file — [filename]"
  Status: Open
  Affected File: [path]
  Resolution Notes: "Filename suggests this may be leftover. Safe to delete if not needed."

---

## Step 4 — Namespace Consistency

Scan every .cs file under Assets/Scripts/.
For any class not declared inside the `BalatroStyle` namespace:
- Add the namespace wrapper directly — this is always safe to auto-correct
- Log the fix in the audit auto-fixed list

---

## Step 5 — Singleton Discipline

Scan all .cs files for: static instance, static _instance, Instance property,
or DontDestroyOnLoad used on any class other than GameManager.

For each violation found:
- Do not refactor automatically
- Log in Notion Known Issues:
  Issue: "Unexpected singleton — [ClassName] in [filename]"
  Status: Open
  Affected File: [path]
  Resolution Notes: "Only GameManager should use singleton pattern per architecture rules.
  This class should communicate via events/delegates instead."

---

## Step 6 — Duplicate Logic Detection

Look for scripts that appear to serve overlapping responsibilities
(e.g. two camera shake scripts, two score managers, two deck handlers).

For each suspected duplicate pair:
- Do not merge or delete automatically
- Log in Notion Known Issues:
  Issue: "Possible duplicate — [FileA] and [FileB]"
  Status: Open
  Affected File: both paths
  Resolution Notes: "These scripts appear to have overlapping responsibilities.
  Recommend keeping [preferred file] and consolidating logic. Review when convenient."

---

## Step 7 — Magic Numbers Sweep

Scan all .cs files for numeric literals appearing directly in code outside of
const declarations or [SerializeField] defaults.

For each magic number:
- If the correct constant name is obvious (e.g. 52 → DECK_SIZE, 8 → MAX_ANTES):
  Replace it with a named const directly.
- If the purpose is ambiguous:
  Add an inline comment: // TODO: extract to named constant
  Log in Notion Known Issues with the file and line.

---

## Step 8 — Known Issues Cross-Check

Read the Known Issues section of CLAUDE_FRAMEWORK.md.

For each listed issue:
- Check the current codebase to determine if it is still open or has been resolved
- If resolved: mark it "Resolved" in both CLAUDE_FRAMEWORK.md and Notion Known Issues,
  and add a brief Resolution Notes entry describing what fixed it
- If still open: leave it unchanged
- If new issues were discovered in steps 1–7 that aren't yet listed:
  Add them to both CLAUDE_FRAMEWORK.md Known Issues and Notion Known Issues

---

## Audit Report

Produce a clean structured summary for the sprint wrap-up:

**Auto-Fixed**
List everything corrected automatically with file paths.

**Logged to Notion**
List everything flagged and written to Notion Known Issues.
For each: the issue title and a one-sentence plain-language explanation
the developer will understand without Unity or coding knowledge.

**Known Issues Synced**
What was marked resolved, what was added, what was left unchanged.

**Clean**
List every audit category that came back with no issues.
