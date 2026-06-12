# Custom osu! fork

This fork is my small personal playground, where I make adjustments to the client for my own needs.

I've been playing on osu!lazer for a long time, not only because I don't care about ranked plays, but because I can do whatever I want to this client, no matter how bad the code is.

- [Custom osu! fork](#custom-osu-fork)
  - [Changes to osu!taiko](#changes-to-osutaiko)
    - [Custom Mod](#custom-mod)
    - [Custom Skinnable `UR` Counter and Average `HitError` Counter](#custom-skinnable-ur-counter-and-average-hiterror-counter)
    - [Changes to Statistics Screen](#changes-to-statistics-screen)
    - [Disabled the Flying Taiko Hit Animation](#disabled-the-flying-taiko-hit-animation)
  - [Custom Ruleset: Typing](#custom-ruleset-typing)
    - [Motivation](#motivation)
    - [Work-in-progress](#work-in-progress)
    - [Custom Mod Requirement](#custom-mod-requirement)
    - [Nothing New](#nothing-new)
    - [Questionable Change](#questionable-change)
    - [Why Not Extract Ruleset?](#why-not-extract-ruleset)
  
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

I don't want to get into the details how it works, these are just random ranked words based on MonkeyType dictionaries, which already has them ordered by frequency. I didn't feel like there was a need for some fancy text generation algorithms.

### Work-in-progress

This ruleset will be in *work-in-progress* state for a long time, because I don't feel like adding or fixing stuff, as the foundation is already sufficient for me to play.

### Custom Mod Requirement

Since the whole idea was to play with generated words, it's mandatory to have the `English` mods enabled:

![img](https://i.imgur.com/F1ClATl.png)

It's still possible to play the actual beatmap with random letters, but I disabled the difficulty calculation for this and explicitly require `English` mod to be present in order to calculate the star rating. It's not correct anyway, but I'm just experimenting. There is a better ruleset for this in the ruleset collection in the repository, ppy added a `Typer` ruleset, which I just now discovered, go play that for random letters and preserved beatmap objects.

The reason for such low amount of mods is that it's pretty much what's sufficient:

- `DoubleTime` / `HalfTime` for rate adjustment
- `Constant Speed` to remove annoying scroll speed changes
- `Difficulty Adjustment` to replace `Hardrock`/`Easy`

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
