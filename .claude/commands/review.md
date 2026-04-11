---
description: Architecture review — check coupling, patterns, and scalability
---

Read CLAUDE_FRAMEWORK.md for project context and Architecture Notes.
Then scan all scripts in Assets/Scripts/.

Review for:

1. **Coupling** — Are systems communicating only through events/delegates? Flag any direct cross-references between unrelated systems (e.g., Hand directly calling ScoreManager).

2. **Singleton discipline** — Only GameManager should be a singleton. Flag any other classes using static instances.

3. **Data flow** — Is game state flowing in one direction? Flag any circular update patterns.

4. **Scalability** — Will the current structure support Jokers, special cards, and modifiers without major refactoring? Note any areas that need interfaces or abstraction now.

5. **Consistency** — Are naming conventions, namespace usage, and file organization consistent across all scripts?

For each issue: state the file, the concern, and your recommended fix.
Do NOT auto-fix during review — present findings and wait for approval.
