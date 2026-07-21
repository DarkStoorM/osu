# Custom osu! fork

This fork is my small personal playground, where I make adjustments to the client for my own needs. I would not recommend trying this fork, even if you found out about the `Typing` ruleset from anywhere, because it's not fun for the first time users, and I'm not going to bother making things look pretty, because I simply don't care.

I've been playing on osu!lazer for a long time, not only because I don't care about ranked plays, but because I can do whatever I want to this client, no matter how bad the code is.

- [Custom osu! fork](#custom-osu-fork)
  - [Changes to osu!taiko](#changes-to-osutaiko)
    - [Custom Mod](#custom-mod)
    - [Custom Skinnable `UR` Counter and Average `HitError` Counter](#custom-skinnable-ur-counter-and-average-hiterror-counter)
    - [Changes to Statistics Screen](#changes-to-statistics-screen)
    - [Disabled the Flying Taiko Hit Animation](#disabled-the-flying-taiko-hit-animation)
  - [Custom Ruleset: Typing](#custom-ruleset-typing)
    - [Motivation](#motivation)
    - [A TL;DR On How To Use This](#a-tldr-on-how-to-use-this)
    - [ZERO Skin Elements](#zero-skin-elements)
    - [Work-in-progress](#work-in-progress)
    - [Custom Mod Requirement](#custom-mod-requirement)
    - [Finger Guide](#finger-guide)
    - [About the Custom Mod](#about-the-custom-mod)
      - [What About Other Settings?](#what-about-other-settings)
        - [Increased Spacing](#increased-spacing)
        - [Banned Consonants](#banned-consonants)
        - [Word Seed](#word-seed)
      - [Customisation](#customisation)
    - [Skinnable WPM](#skinnable-wpm)
    - [WPM In Beatmap Attributes](#wpm-in-beatmap-attributes)
    - [Nothing New](#nothing-new)
    - [Questionable Change](#questionable-change)
    - [Why Not Extract Ruleset?](#why-not-extract-ruleset)
    - [Difficulty Calculator for Typing Ruleset](#difficulty-calculator-for-typing-ruleset)
    - [Keyboard layouts](#keyboard-layouts)
    - [Sharing Disclaimer](#sharing-disclaimer)
  - [Key Timing Distribution](#key-timing-distribution)
  - [Skinnable Key Timing Distribution](#skinnable-key-timing-distribution)
    - [Missing Features](#missing-features)
  
---

## Changes to osu!taiko

Since osu!taiko is my main mode, I needed to find another way of enjoying the game besides downloading gazillion of beatmaps.

### Custom Mod

> TL;DR: I implemented `TaikoModFullRandom` that creates a new beatmap for the current session. More information about this mod is available in [this gist](https://gist.github.com/DarkStoorM/060db882956e249bb029a71e471f73c4).

This is my very old idea of a beatmap generator way back from Stable that removes the contents of the entire beatmap and regenerates them with new contents.

Now, thanks to how awesome things are in osu!lazer, rather than generating the same beatmap for Stable, polluting the entire songs list, the custom mod lets me swap the beatmap contents on the fly. So, without having to go through a quite tedious process of generating new `.osu` files every time I want to play with a different roll, I can do this directly inside lazer and play "infinite" amount of variations of the same map.

![img](https://i.imgur.com/koTGuII.png)

---

### Custom Skinnable `UR` Counter and Average `HitError` Counter

Because the unstable Rate in osu! throws all inputs into the same bin, I had to split it into `Don UR` / `Kat UR` to see which hand is actually more unstable during the gameplay, especially since my playstyle is `DD/KK`.

The same goes for the *average Hit Error*, but this was more for offset correction purposes (and to see if the automatic correction does its job).

![img](https://i.imgur.com/JOW5cby.png)

### Changes to Statistics Screen

I added the `UR`/`HitError` values to the result screen, below the timing distribution:

![img](https://i.imgur.com/FsL4mWe.png)

### Disabled the Flying Taiko Hit Animation

Pretty sure you still can't skin the flying hits out as you can in Stable, and some PRs were opened for this, but got rejected, so I just disabled it myself, even if the solution may not be quite right.

---

## Custom Ruleset: Typing

![img](https://i.imgur.com/5WSMhgA.png)

For the lack of a better name of course.

### Motivation

I've been stalling on this for quite a long time and finally, I decided to move the typing practice to osu!. Normally, I wouldn't care, but the main motivation was to learn touch-typing on [Corne](https://i.imgur.com/6dwFKmk.png), which I bought a while ago.

I got pretty much bored of MonkeyType, and since I had it configured to the `scrolling tape`, it really reminded me of osu!taiko, so I created pretty much identical ruleset to this.

![img](https://i.imgur.com/4AewVy9.png)

These are just random, ranked words generated from curated dictionaries. I parsed all books from Project Gutenberg, ordered words by frequency and manually went through the list and left 5000 words. I didn't feel like there was a need for some fancy text generation algorithms, but at least it could use some n-gram bias, though.

- `Curated` - A custom, scored words list, which I took from `5k` and reevaluated based on some personal preferences:
  - Heavy cross-hand bias per letter
  - Less same-finger vertical movement
  - Less counter-roll movement
  - 1-9 length words, cross-hand only 2/4 length words for even-length mode

> [!Note]
> Only applies to QWERTY. I only left words that were somewhat mechanically nice to type for muscle memory grinding (for me at least, since I type on `split ortholinear QWERTY`).

- `English 0K` - 300 words (300, because the main mod is primarily odd-length words)
- `English 1K` - 1000 words without second pass
- `English 5k` - as above, but 5000

### A TL;DR On How To Use This

This ruleset is primarily made for `Words` mod, where you type random words from the selected dictionaries, the beatmap contents are **replaced**.

- Go to Mod Selection
- Select `Words` Mod
- Customise the `Words` Mod mostly by adjusting the `Beat Length`:
  - `Half`(*) - letters land on 1/4, for lower BPMs to make it play faster (100 BPM evaluates to around 80 WPM, 150 BPM -> 120 BPM etc.)
  - `Full` - default, letters land on 1/2, used for 200+BPM
  - `Double`
- Probably add `Constant Speed` if there are any `Scroll Speed` changes
- Mod Selection shows the approximate `WPM` based on the most common BPM next to `Difficulty Attributes`
- Adjust `Scroll Speed` with keybinds (F3-F4 by default)

> [!Note]
> (*) The `Beat Length` adjustment in the mod customisation does not define the actual `Beat Length`, it can halve the current beat length, double it, or leave intact.

### ZERO Skin Elements

I decided against making any skin components that would make this look nice and went with just the playfield and simple letters. That was my design choice, but there is a good reason behind this:

- Parsing incoming words on the fly is already hard, especially if you are not a native English speaker
- Nearby skin elements might be too distracting

So, this basically means there will be none of these ever:

- Playfield styling
- Ruleset Character
- Non-miss `HitObject` Judgement display (`max`, `good` etc.)

I don't mind the playfield being basically empty. I could technically make them optional, just like the `Skinnable Key Timing Distribution` and `WPM Counter`, but it's not really worth it.

### Work-in-progress

This ruleset will be in *work-in-progress* state for a long time, because I don't feel like adding or fixing stuff, as the foundation is already sufficient for me to play.

> [!Note]
> The mod is applied to all visible difficulties, so the structure of the current beatmap difficulty does not matter, because the mod replaces the entire beatmap. This will result in all difficulties having the same Star Rating if custom Seed is applied.
>
> If all beatmaps have the same Drain Time, they will have a very similar Star Rating. This is intentional given how the mod works and it does not matter which difficulty is played with this mod.

### Custom Mod Requirement

Since the whole idea was to play with generated words, it's mandatory to have the `Words` mods enabled:

![img](https://i.imgur.com/KztcqtG.png)

It's still possible to play the actual beatmap with random letters, but I disabled the difficulty calculation for this and explicitly require `Words` mod to be present in order to calculate the star rating. It's not correct anyway, but I'm just experimenting. There is a better ruleset for this in the ruleset collection in the repository, ppy added a `Typer` ruleset, which I just now discovered, go play that for random letters and preserved beatmap objects.

The reason for such low amount of mods is that it's pretty much what's sufficient:

- `NoFail` - this was not supposed to be here since `Difficulty Adjustment` was added to adjust HP drain, but it's just faster to keybind this instead
- `DoubleTime` / `HalfTime` for rate adjustment
- `Constant Speed` to remove annoying scroll speed changes
- `Difficulty Adjustment` mostly for `OD` adjustment and scroll speed fine-tuning, sometimes for `HP`

> [!Note]
> While `Scroll Speed` can be adjusted through the `Difficulty Adjustment` mod, the keybinds for `Increase/Decrease` scroll speed will also work here.

### Finger Guide

I had this weird idea of coloring the objects so they map to specific fingers, which I don't even think is useful, but I left it in anyway. Might be more distracting later on, but I think the color-to-finger mapping adaptation is quite fast.

![alt](https://i.imgur.com/HwEotLn.png)

![alt](https://i.imgur.com/rgroVQ7.png)

(This is my osu! layer for this ruleset)

### About the Custom Mod

This mod recommended to be customised, because beatmaps are so different, it's literally impossible to make all of them playable with a click of a button.

I wanted to be able to play this ruleset on a wide range of beatmaps, so I added the following setting: `Adjust Beat Length`. This will effectively double or halve the BPM to let me play for example a 100 BPM beatmap. The reason why is that the mod is generating hit objects at 1/4 for words, 1/2 for spaces between the words. This can be quite slow, so without having to use the DoubleTime to fine-tune and play on rate changed song, I can play it on 200 BPM, which is roughly 65 WPM.

> [!Note]
> 100 BPM with halved beat length equals to 200 BPM with default full beat length.

With this, and with DoubleTime/HalfTime, I can play everything adjusted to my comfortable speed. For example, I play at around 100 WPM only (around 150 BPM in this ruleset), so to adjust to this speed, I can play something that was mapped to:

- 300 BPM + `full beat length`
- 240 BPM + `full beat length`
- 100 BPM + `half beat length` + DoubleTime rate changed up to 120 BPM
- 180 BPM + `half beat length` + HalfTime rate changed down to 140-150 BPM

#### What About Other Settings?

There is another setting, which I'm not sure is even worth talking about, because I don't know if what I assumed is correct. Initially, I went with the same strategy as my Random mod for Taiko: odd length pattern lands on beat, even doesn't. I assumed that even length patterns are harder to play, so I extracted them into a separate setting, which makes them disabled by default, plus you can customise the chance for generating even length words.

The `Toggle` for generating even length words might seem redundant with the `Chance` slider, but my reasoning here is that it's better to turn something off, preserving the slider value, so whenever I'd like to use it again with the same value, I can just quickly use the toggle.

For the same exact reason, I increased the strain slightly on word length, which bumps the star rating with this setting, because I believe that off-beats are just harder, even if a pair of off-beats is actually an odd length pattern, which effectively makes no difference between a five object pattern and a pair of two 2-length words, since technically the whole length is equal.

##### Increased Spacing

There is also a setting to increase the spacing between words, because why not?

This, instead of inserting a 1/2 break, inserts a full 1/1 break, which gives enough recovery time between the words, allowing to play on higher BPMs, since there is more time to fully parse the incoming word. That comes with a cost of lowered difficulty, though, since the sustained speed and typing fatigue decreases with such breaks, so, glad the difficulty calculation can pick that up (somewhat).

##### Banned Consonants

This will skip all words containing any of the consonants from this component. You can ban a maximum of 8 consonants. You can simply type something like `xcvbj`.

Not really recommended for usual gameplay, this is more like a *chill* setting.

##### Word Seed

This makes *everything* deterministic. Custom seed should only be used if you intend on repeating the play on the same beatmap, because this effectively makes *all* visible beatmaps to be regenerated with the same seed.

It results in the same exact Star Rating across all beatmaps with the same properties, especially in a beatmapset, e.g. if all difficulties in a beatmap have the same drain time and no BPM changes, the Star Rating will be the same.

It would seem like there is no point in using the seed, because ultimately the only difference would be how individual songs feel.

#### Customisation

![alt](https://i.imgur.com/vDsnvC5.png)

### Skinnable WPM

I will just briefly mention that there is a new component for WPM, but it's basically the same counter that is used for `Clicks Per Second`, but it's measuring overall inputs for the current beatmap playtime that will average out as you play to give an approximate realtime WPM.

### WPM In Beatmap Attributes

Since I wanted a WPM preview with the mods applied on the beatmaps, I added it to the song selection:

![alt](https://i.imgur.com/AtLbgVa.png)

![alt](https://i.imgur.com/dhPX6WR.png)

### Nothing New

Most of the code was copied from Taiko ruleset + I don't know what I'm doing.

### Questionable Change

I changed how the game shows converts by hardcoding the taiko ruleset name, because when I play my own ruleset, I want Taiko maps to appear, because my beatmap listing consists of **only** taiko maps and I'm not going to bother downloading standard maps just for this:

```plaintext
// BeatmapInfoExtensions.cs:75
if (beatmap.Ruleset.ShortName == "taiko" && ruleset.ShortName == "typing")
    return true;
```

### Why Not Extract Ruleset?

There is a Ruleset Collection in osu! repository, but that requires the ruleset to be its own, separate release with other rulesets cut out. Since I sometimes switch to Taiko, I don't want to launch a different game + I can modify things directly here.

### Difficulty Calculator for Typing Ruleset

While this was completely unnecessary, because in the end, all words are random, and I believe you can't measure the difficulty of randomness, I did it anyway, because I was just curious how it will look like.

I myself did no research on measuring the typing difficulty whatsoever, I did end up asking slop gpt, though. There, I said it. I even asked it to come up with some formulas, but it was so dumb, I had to delete absolutely everything and throw random functions around instead. I only left the concepts from it, which I turned into the `Skills` from the difficulty calculator. Still, had to come up with the code myself.

While I had four other rulesets at my disposal, it would make zero sense to copy what they do if I have no clue what the hell I am doing. I understand that the code for difficulty calculation will look weird to anyone who knows actual Maths, this is still just a personal project, so I don't think I care enough. That also applies to code quality.

I included a bunch of tests which I used to tweak the values, which are in the end based on aiming to get a four stars beatmap from 140 BPM over three minutes of gameplay (that's around 90 BPM I think). I don't know how Strain Skills work anyway, so I just rolled with it and only glanced at other skills without checking what they do, then tweaked the values, looking at graphs.

Both test scene and console project were written by AI, because I couldn't care less about something I would use once (slop gpt, to be specific, I don't give a shit about the agentic slop people use).

### Keyboard layouts

A very important note: while this ruleset is obviously unranked and the star rating is not accurate at all, the main mod required for Star Difficulty calculation allows you to select a keyboard layout (qwerty/dvorak/colemak) to adjust the calculation. This is purely for gauging the difficulty if you are playing with the selected layout. This was added mostly out of curiosity rather than supporting layouts the potential player might use.

I did this, because the Star Difficulty calculation contains the following measurements and I needed to see the differences in:

- Key Travel (favouring counter-natural rolling direction - index>outwards)
- Retrigger (key repetition, a.k.a double-tap, depends on used finger)
- Row Switch

Out of all six skills included in the difficulty calculator, these three depend on the keyboard layout, because of the physical properties of each of them: key position, distance difference and row location. Each layout will differ across these three skills, altering the star rating, but this does not mean that someone playing on Dvorak (less strain across said skills) will play on Qwerty layout to bump the star rating.

This is **only** informational and makes no sense to to use a different one than your every day typing layout.

Still, this was a very interesting experiment to see the following order in the difficulty across layouts (given that I can NOT confirm that any of the difficulty calculations are correct, since tons of factors are still missing):

- Qwerty - Hardest
- Colemak - `^ -10% to -15%`
- Colemak-DH - `^ -1% to -3%`
- Dvorak - Easiest, `^ -5% to -10%`

> [!Note]
> The above **does not** apply to the `Curated` dictionary.

In that order, Dvorak resulting in around 15% difficulty drop overall from Qwerty.

Does it matter? Nope, it's not something that should be measure anyway. It's like you asked the PP Committee team to rework Taiko PP to calculate performance based on playstyles other than assumed `full alt KDDK`: `DDKK`/`KKDD`/`DKKD`/`DDDK`. One would have to be insane enough to create difficulty calculators for all of them and make sure people don't lie.

> [!Note]
> All the difficulty calculation assumes Touch-Typing. It's basically like Taiko assuming `full alt KDDK`, but `DDKK` playstyles getting free PP from 1/6.

### Sharing Disclaimer

I'm intentionally not sharing this, because I made this for myself, but, if you happened to find this, I don't think you will have fun playing. I'm too lazy to write a better documentation for the rulesets/mods, or make it easier to use in general.

Also, I won't be sharing this in the [Custom Ruleset Directory](https://github.com/ppy/osu/discussions/13096), I dont think it's worth posting something this unpolished and badly coded.

## Key Timing Distribution

A hacky keyboard preview with unstable rate per key. The layout is the default staggered, can't be bothered figuring out the split layouts. The keys will be taken from the mod directly since you can now select the layout from the customisation.

![alt](https://i.imgur.com/TWqHIYB.png)

## Skinnable Key Timing Distribution

Initially, I added the Unstable Rate preview to the result screen as mentioned above, but I decided to also add this to the skin editor instead of creating a keyboard layout with flashing keys as you type. This was the laziest implementation and I didn't even want to do it anyway, but since I'd sometimes want to have a live preview of the Unstable Rate for videos, I just reused the result screen component.

I had to change the `Alpha` on it though, so it's not as distracting

![alt](https://i.imgur.com/JbBuUDS.png)

### Missing Features

Some stuff I *might* add if I won't get lazy:

- Replays - ~~the game is not recording the current session, so I will probably just copy it from osu!taiko.~~ The game can record replays, but it seems that it's not possible to watch them in custom rulesets(?). Maybe I'm missing something, but it looks like the replays are hard locked to the main rulesets.
