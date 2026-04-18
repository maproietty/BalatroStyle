# BalatroStyle — Design Document

*Source of truth: Notion page "BalatroStyle - Design Document"*
*This is a local snapshot. For the live version, refer to Notion.*
*Snapshot date: April 18, 2026*

---

## The Pitch

A single-player poker roguelite set in a drug-tinged occult parlor. Each night you return to play 5–7 hands of a Hold'em-inspired variant — dealt 7 cards (2 hole + 5 community), pick your best 5. Score explodes through stacked charm effects from three substance families: psychedelic (rule-warping visions), smoke (slow-burn, hypnotic), and chemical (precise, stackable multipliers). The parlor's stakes rise faster than your build naturally scales, so every night is a race — upgrade your charms and survive the boss hand, or the parlor takes its cut. Five nights is the baseline run, ending in a final hand against The House itself — but the parlor is bottomless. With the right charms and bankroll, players can keep pushing past the baseline, chasing the dragon into nights no one's meant to walk out of.

---

## Core Design Pillars

These are the decisions locked in during the concept phase. Each one should be protected as later mechanics get added.

### Player Experience — Combo Builder
The game is about the joy of discovering wild charm synergies. Every design choice should protect and amplify the "aha" moment of watching a build come together.

### Feel — Score Explosions (Balatro-style)
When a combo lands, the satisfying moment is multipliers stacking on multipliers, numbers cascading up. This is the emotional payoff.

### Core Mechanic — 7-Card Hold'em Selection
Each hand deals 7 cards (2 hole + 5 community). Player picks their best 5. The selection puzzle is the distinctive twist vs. other poker roguelites — most make you play what you're dealt; this one asks *which 5 of these 7 make my build sing?*

### Per-Hand Agency — Pure Pre-Hand Strategy
The player does not manipulate cards during the hand. All strategy lives in the pre-hand charm build and the final 5-of-7 selection. Keeps the game meditative and focused on build-crafting.

### Primary Tension — Runaway Thresholds
Minimum scores escalate aggressively. The player can't coast — every shop visit and every charm choice has to push the build forward faster than the thresholds climb.

### Run Structure — Chaptered Nights
Each run is divided into "nights at the parlor." A night has 5–7 hands, ending in a boss hand with a special modifier. Clear the night to proceed. Nights give runs a narrative rhythm — each one can carry its own dealer, crowd, or house rule.

### Chasing the Dragon — Unbounded Pursuit
The parlor never forces the player to stop. Five nights is the baseline run (ending at The House), but the player's bankroll and charm loadout determine how far past it they can push. The emotional loop is *the chase* — that first combo of Night 1 gives you the high, and every night after is the player trying to recapture it against escalating stakes. You never fully catch it. That's the point. This pillar ties the core tension (runaway thresholds), the theme (drug-occult mysticism), and the passing-out motif into one coherent loop.

---

## Theme — The Drug-Tinged Occult Parlor

A candlelit mystic parlor where charms are literal talismans, and the mysticism is intertwined with recreational drug culture. Oracle rituals, psychonaut visions, smoke-hazed divination. Aesthetic touchstones: Jodorowsky's *Holy Mountain*, 70s counterculture occult, modern psychonaut scenes, Hunter S. Thompson's chaos-mysticism.

### Charm Families

Three substance-coded families, each with a distinct mechanical and visual identity. Cross-family combos create the most interesting builds — a psychedelic rule-breaker plus chemical stackers plus a smoke trigger produces a wildly different run identity than three of any one family.

**Psychedelic — Rule-warping visions.**
These charms change *how scoring works*. Initial direction: effects like "flushes don't need matching suits," "straights can wrap around," "pairs count as three-of-a-kind." Cosmic and searching. Visuals: sacred geometry, entity encounters, fractal patterns.

**Smoke — Slow-burn and hypnotic.**
These charms trigger over time or under specific conditions. Initial direction: effects like "every third hand, double base score," "if you've played exactly X hands this night, +Y mult." Velvet and incense. Visuals: hookah smoke curls, dim lantern light, opium lounge textures.

**Chemical — Precise, stackable multipliers.**
The mathematical engine — straightforward numeric effects that stack cleanly. Initial direction: effects like "+3 mult per face card," "x2 chips if you end on a heart." Modern and clinical. Visuals: beakers, pills, reagent vials, laboratory precision.

---

## What a Hand Looks Like

1. Start the hand with your current charm loadout active.
2. Seven cards are dealt (2 hole + 5 community), revealed together.
3. Player selects their best 5 of the 7.
4. Scoring resolves: base poker hand value → charm triggers cascade → final score.
5. If the final score meets or exceeds the threshold, advance. Otherwise, the night ends in failure.

## What a Night Looks Like

- **Buy-in.** Paying into a night costs dollars from the player's bankroll. Baseline-run nights (1–5) have set buy-ins; post-House nights scale aggressively. Can't afford the buy-in, can't play the night — the run ends.
- **Hands.** 5–7 regular hands with escalating thresholds. Chips earned per hand fund the shop; dollars earned by scoring OVER the threshold fund reshuffles and future buy-ins.
- **Shops.** Interspersed between hands (exact frequency TBD). Spend chips to buy charms; spend dollars to reshuffle the shop's offerings at an escalating cost.
- **Lifeline.** If the player misses a threshold, they're offered one rescue per night: burn bankroll, re-deal the hand at a harder threshold. Refusing ends the run.
- **Boss hand.** Night ends with a boss hand whose dealer manipulation (hole tampering, community manipulation, or scoring interference) gives the night its personality.
- **Clear to proceed.** Clearing the night advances the run. Threshold scaling steepens, dealer manipulation grows more frequent and severe. Post-House, both accelerate hard.

---

## Design Decisions

### Charm Acquisition — Two-Currency Economy

- **Chips** — shop currency. Earned per hand cleared, spent to buy charms from the shop.
- **Dollars (bankroll)** — the dual-purpose resource. Earned primarily by scoring ABOVE the threshold (overshoot = dollars; the bigger the overkill, the bigger the payout). Dollars are spent on two things: reshuffling the shop (at an escalating per-reshuffle cost, Cloverpit-style) OR buying into the next night.
- The dual use of dollars creates the core economic tension: do I reshuffle now for a better charm, or save for the night ahead? Every dollar has two possible homes, and the player never has enough. This directly reinforces "chasing the dragon" — every dollar spent on reshuffles is a dollar that won't carry you to the next night.

### Meta-Progression
First-time players only have access to a small selection of charms. They unlock additional charms by meeting requirements set per charm (similar to Cloverpit).

### Player Identity
The story line remains vague. Players start fresh every run — no character select, no persistent protagonist.

### Boss Hand Design
Threshold spikes are the primary identifier of a boss hand. Bosses also carry abilities that negatively impact gameplay, but those debuffs can become benefits depending on the player's charm setup — meaning boss variety creates opportunity for some builds, not just pure punishment.

### Night-Level Flavor — The Passing-Out Motif
As the threshold increases through a night, the room gets progressively darker and blurrier — as if the player is slowly passing out. The cards and table always remain clear. After each threshold cleared, the player feels: *"I lived to see another night."*

### Charm Slot Economy
The player starts with 5 charm slots. At least one charm has the ability to increase slot capacity by 2 while it's active (similar to Cloverpit's house charm). Slot expansion is a core build lever, not just a stat.

### Dealer Manipulation — Three Systematic Surfaces
The dealer is the implicit opponent. Although the player isn't playing against another hand, the dealer exerts pressure by manipulating cards and the table. Manipulation frequency and severity increase with thresholds and boss hands. Every dealer effect falls into one of three surfaces:

- **Hole card tampering** — the dealer targets the player's "personal" cards. Examples: "your hole cards are swapped for the bottom of the deck," "one hole card is dealt face-down and revealed only at scoring," "hole cards cannot contribute to a flush this hand."
- **Community manipulation** — the dealer constrains the shared table cards. Examples: "the river is drawn only from face cards," "the flop contains no suit matches," "one community card is dealt face-down."
- **Scoring interference** — the dealer alters how final scoring resolves. Examples: "flushes score 50% less," "no multiplier can exceed x5 this hand," "pairs are treated as high cards."

The three-surface model keeps dealer effects readable AND creates clear lanes for counter-charms (e.g., "immune to hole card tampering," "community cards always reveal face-up," "ignore one scoring interference per night").

### Run Length
- **5 nights as the baseline run**, ending in a climactic hand against The House — the parlor's owner, the entity behind every dealer. That gives closure-seekers a meaningful win state and a clean narrative arc: pulled deeper each night, finally confronting what runs it all.
- **Beyond the baseline, nights continue indefinitely.** Each additional night is a buy-in from the player's bankroll; as long as they can afford the buy-in and survive the threshold, they keep playing. Post-House thresholds scale toward insurmountable — these nights aren't meant to be beaten, they're meant to be chased.
- The passing-out motif crescendos into endless territory: the room goes fully black, even the cards start blurring at the edges, the player runs on muscle memory and charm stacks. Directly expresses the "chasing the dragon" fantasy.

### Failure and Recovery — The Lifeline
- When the player misses a threshold, they're auto-offered a rescue. Taking the Lifeline costs bankroll (dollars) and re-deals the hand with a HARDER threshold. Refusing ends the run.
- This turns failure into a real decision: burn bankroll and face a tougher hand (with the same charm loadout that just fell short), or accept the loss. Players with a deep bankroll and a rescuable build can claw back; players who overspent in the shop get punished. Thematically it lands perfectly — the drug-parlor version of "one more hit."
- Starting-point sub-decisions (open for playtest): one Lifeline per night (not per hand); re-deal gives all 7 new cards; dealer manipulation still applies; Lifeline cost scales with the night number.

### Family Balance — Both Paths Viable
- Pure builds offer consistency and clear thematic identity — "tonight I'm running a psychedelic build." Mixed builds offer explosive discovery and unexpected cross-family synergies. Neither path should dominate.
- Practical levers for keeping both competitive: each pure family could grant a set bonus at 5+ charms of that family (rewarding purity), while certain charms explicitly reward cross-family presence (e.g., "+X mult for every family represented in your loadout"). Rarity and power curves need to be balanced so pure paths don't hit a ceiling mixed paths avoid, or vice versa.
- This is the most playtest-intensive balance approach, but it produces the widest build space and respects player agency most.

---

## Remaining Work (Playtest / Tuning)

These are not concept-blocking — they're numbers and catalog work for later sessions.

- Specific numeric tuning: threshold curves, buy-in scaling, Lifeline cost curve, shop reshuffle cost curve, bankroll earn rates per overshoot tier
- Charm count and specific designs per family (likely ~8–12 charms per family as a starter set, plus cross-family synergy charms)
- Boss hand catalog (specific dealer manipulations per night, and what The House does at the Night 5 climax)
- Art direction bible for each family's visual language
- UI/UX for the three-surface dealer model (how the player reads what the dealer is doing)

---

## Parking Lot

*(empty — add here as thoughts surface)*

---

*End of snapshot.*