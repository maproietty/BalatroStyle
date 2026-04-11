---
description: Add visual juice to a system — particles, easing, shake, glow
---

Read CLAUDE_FRAMEWORK.md, specifically the Key Visual Targets and Color Palette sections.

The user will specify which system needs juice (e.g., "scoring", "card play", "selection").

For that system, evaluate what's missing from the Balatro feel and add polish:

- **Easing** — Replace any linear lerps with SmoothStep, EaseOutBack, or spring curves
- **Screen shake** — Trigger on impactful moments, scale intensity to magnitude
- **Particles** — Add chip scatter, spark bursts, or glow trails where appropriate
- **Glow pulses** — Flash the neon glow on state changes (selection, scoring, dealing)
- **Scale pops** — Brief overshoot scale on appear/select (1.0 → 1.15 → 1.0)
- **Timing** — Add small delays between sequential events for dramatic pacing

Use only the approved color palette. Use coroutines, not DOTween.
After adding juice, run /done to wrap up.
