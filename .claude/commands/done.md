---
description: Wrap up a completed task — update framework, run QA, suggest commit
---

You just finished a task. Follow these steps in order:

1. Read CLAUDE_FRAMEWORK.md for current project state.

2. Update CLAUDE_FRAMEWORK.md:
   - Mark completed items with [x] in the current phase checklist
   - Add a row to the Changelog with today's date and what was done
   - Update Architecture Notes if any design decisions were made
   - Update Known Issues if any new quirks were introduced
   - If all items in the current phase are complete, advance "Current Phase" to the next phase

3. Run the QA review:
   - Read .claude/qa-standards.md
   - Review every file changed in this session against those criteria
   - Fix any issues found directly — don't just report them
   - If fixes were made, add "QA: [N] issues fixed" to the changelog entry

4. Suggest a git commit message in conventional format:
   ```
   feat(system): short description of what was built
   ```

5. Print a short summary: what was done, what's next.
