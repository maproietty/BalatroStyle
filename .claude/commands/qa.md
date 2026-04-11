---
description: Run QA supervisor review on recent changes
---

Read .claude/qa-standards.md for your review criteria.
Then read CLAUDE_FRAMEWORK.md for current project state.

Identify all files modified in the most recent work session
(check git diff or recently modified files in Assets/).

Review every changed file against ALL criteria in qa-standards.md.
Be strict. Fix issues directly — don't just report them.

After all fixes, update CLAUDE_FRAMEWORK.md changelog with:
"QA review pass — [N] issues found and fixed"

If nothing to fix, state: "QA PASS — no issues detected."
