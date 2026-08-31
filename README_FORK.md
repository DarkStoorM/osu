# Custom osu! fork

TL;DR

This is not a technical README, but more of a thing that kind of reads like a blog post, so, a heads-up, if you hate reading, close this document. Also, this is not some AI auto-generated crap.

---

This fork is my small personal playground, where I make adjustments to the client for my own needs. I would not recommend trying this fork, even if you found out about the `Typing` ruleset from anywhere, because it's not fun for the first time users, and I'm not going to bother making things look pretty, because I simply don't care.

I've been playing on osu!lazer for a long time, not only because I don't care about ranked plays, but because I can do whatever I want to this client, no matter how bad the code is.

- [Custom osu! fork](#custom-osu-fork)
  - [Changes to osu!taiko](#changes-to-osutaiko)
    - [Custom Mod](#custom-mod)
    - [Custom Skinnable `UR` Counter and Average `HitError` Counter](#custom-skinnable-ur-counter-and-average-hiterror-counter)
    - [Changes to Statistics Screen](#changes-to-statistics-screen)
  - [Custom Ruleset: Typing](#custom-ruleset-typing)
    - [Motivation](#motivation)
    - [The Big Question... What, and Why?](#the-big-question-what-and-why)
      - [About Beatmap Content Replacement](#about-beatmap-content-replacement)
    - [A Very Poor Screenshot + Explanation](#a-very-poor-screenshot--explanation)
      - [About the Word Frequency and Word Generation](#about-the-word-frequency-and-word-generation)
      - [About Dictionaries](#about-dictionaries)
      - [Note On Manually Curated Dictionary](#note-on-manually-curated-dictionary)
      - [Note on Curated Full Alt dictionary](#note-on-curated-full-alt-dictionary)
    - [A TL;DR On How To Use This](#a-tldr-on-how-to-use-this)
      - [Note on Letter Spacing](#note-on-letter-spacing)
    - [ZERO Skin Elements](#zero-skin-elements)
    - [Work-in-progress](#work-in-progress)
    - [Custom Mod Requirement](#custom-mod-requirement)
    - [Finger Guide](#finger-guide)
    - [About the Custom Mod](#about-the-custom-mod)
      - [Snapping Words to 1/1](#snapping-words-to-11)
        - [Unintentional Bonus Mode](#unintentional-bonus-mode)
      - [Banned Consonants](#banned-consonants)
      - [Forced Cross-Hand on New Words](#forced-cross-hand-on-new-words)
        - [Important Note on Cross-Hand Play](#important-note-on-cross-hand-play)
      - [Word Seed](#word-seed)
      - [Space Between Words as Bonus](#space-between-words-as-bonus)
        - [The Issues](#the-issues)
      - [Customisation](#customisation)
    - [Scoring Changes](#scoring-changes)
    - [WPM In Beatmap Attributes](#wpm-in-beatmap-attributes)
      - [Other Beatmap Attributes](#other-beatmap-attributes)
    - [Nothing New](#nothing-new)
    - [Questionable Change](#questionable-change)
    - [Why Not Extract Ruleset?](#why-not-extract-ruleset)
    - [Difficulty Calculator for Typing Ruleset](#difficulty-calculator-for-typing-ruleset)
    - [Keyboard layouts](#keyboard-layouts)
    - [Sharing Disclaimer](#sharing-disclaimer)
  - [Key Timing Distribution](#key-timing-distribution)
  - [Skinnable Key Timing Distribution](#skinnable-key-timing-distribution)
  - [Replays](#replays)
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

The same goes for the _average Hit Error_, but this was more for offset correction purposes (and to see if the automatic correction does its job).

![img](https://i.imgur.com/JOW5cby.png)

### Changes to Statistics Screen

I added the `UR`/`HitError` values to the result screen, below the timing distribution:

![img](https://i.imgur.com/FsL4mWe.png)

---

## Custom Ruleset: Typing

![img](https://i.imgur.com/5WSMhgA.png)

For the lack of a better name of course.

### Motivation

I've been stalling on this for quite a long time and finally, I decided to move the typing practice to osu!. Normally, I wouldn't care, but the main motivation was to learn touch-typing on [Corne](https://i.imgur.com/6dwFKmk.png), which I bought a while ago.

I got pretty much bored of MonkeyType, and since I had it configured to the `scrolling tape`, it really reminded me of osu!taiko, so I created pretty much identical ruleset to this. To be clear, this is not the main source of typing practice due to multiple factors that differ from normal typing and this gameplay, and I guess everyone is aware of this.

### The Big Question... What, and Why?

So, this is something very much against the vision of osu!. It's supposed to be played with a special mod that basically deletes someone's work (swaps contents in place for current session).

To someone who doesn't really play osu!: you get a background song, you tap words to the actual beat. In reality, it wouldn't be much different than playing music and sitting on Monkeytype, but you know, there is no "layer of freedom" with you being forced to play consistently, no errors allowed, and it's inside osu!, which is nice.

> [!Note]
> This is already explained later in this document, but this ruleset should not be treated as something you would enjoy playing for more than 30 minutes. It's the same case as with typing tests. Here, the only difference between typing sessions is choosing a different song and a random seed. Still, you just play to `1/2` beat, without any meaningful beatmap structure.
>
> On another note, I call the letter insertion `1/2`, which is what we actually call `Single Tap` in general, so this is a half of the Quarter Note provided by beatmap. 1/4 is just _stream_.

#### About Beatmap Content Replacement

As somewhere mentioned (I guess), the main goal is to "type words". Attempting to generate actual words with the existing beatmap contents would make _absolutely no sense_, because words would be split by object spacing in time.

Of course, I've done no research on this, but I'm 100% sure the cognitive load would be too much to consider it playable that way. It would be too hard to parse incoming words while also being split by variable time spacing. Just, no.

I took the same approach as my osu!taiko mod: take the first and last object from the beatmap and insert random stuff in-between. Those two objects define the `Playable Bounds`. **All** existing objects from the selected beatmap are deleted. Again, this might feel like a middle finger towards the mappers like with my osu!taiko mod, because I just got bored of the current content.

Now, back to the second part of the Big Question, _Why?_

Well, two answers:

- Having an excuse to play osu! again, but do something new at the same time as I don't want to play other modes
- Having some more fun with typing on new keyboard

But yeah, the real answer is that I was just curious if such thing would even be playable inside osu!, and so far, I mean personally, I find it fun once you start picking up some more speed.

### A Very Poor Screenshot + Explanation

![img](https://i.imgur.com/pLbAL4T.png)

> [!Important]
> I really need to record a video on this, should record one soon if I don't find anything else to add to this ruleset :thinking: (I hope so)

These are just random, ranked words generated from curated dictionaries. I parsed all books from Project Gutenberg, ordered words by frequency and manually went through the list and left 2500 words. I didn't feel like there was a need for some fancy text generation algorithms, but at least it could use some n-gram bias, though.

#### About the Word Frequency and Word Generation

Initially, I decided to use Zipf's law to generate words since I had all the words ordered by frequency after parsing, but later I realised that it made absolutely no sense to use it. Reasons:

- Customisation allows for forced cross-hand play
- Customisation also allows banning consonants

Using Zipf's law and some bias tweaks allows generating a more natural sequence of words that are easier to predict. But, since the
customisation can skip words, the potentially predicted words are discarded, making the algorithm useless. On top of that, there is a small window preventing the same word for being picked, so that's that.

Words are now generated by `Weighted Random`, where different word lengths have different weight attached to them. The words themselves are just random. With that, I ditched the word frequency altogether, using a uniform distribution for word index. Why not using word frequency as weight? I will let you answer that yourself.

Refer to `WeightedRandomWordGenerator.wordWeightsByLength` for weights.

I think this is a better solution as this also allows for better control over what is generated, but with a small catch of invalidating every replay if the weights get changed (different weights, different words, making stored replays have invalid frames). Although, this only applied to the `Words` mod, I don't really care about the default random letters from automatic beatmap conversion.

#### About Dictionaries

The full dictionary contained 5000 words, but due to some changes, I only left odd-length words, leaving 2500 in total.

- `Curated` - A custom, scored words list, which I took from `Extended` and reevaluated based on some personal preferences:
  - Heavy cross-hand bias per letter
  - Less same-finger vertical movement
  - Less counter-roll movement
  - 1-9 length words
- `CuratedFullAlt` - Similarly to `Curated`, but this one contains all words even outside of the `Extended` dictionary. More information about this dictionary is below the `Curated` dictionary section

> [!Note]
> Information about `Curated` dictionary only applies to QWERTY. I only left words that were somewhat mechanically nice to type for muscle memory grinding (for me at least, since I type on `split ortholinear QWERTY`).

- `Basic` - 250 words
- `Advanced` - ~1250 words without second curation pass
- `Extended` - ~2500

#### Note On Manually Curated Dictionary

While the `Curated` dictionary was initially taken from what I used in the dictionary separation by words count, I felt it needed a change.

Also, I recently decided to remove all even-length words from the `Curated` dictionary. Since this is a personal ruleset anyway, I didn't care much about these since off-beats are always awkward. I remade the `Curated` dictionary by mixing the previous words list I had, with a new, scored OANC list (the Written, Open American National Corpus). The words deletion cut the dictionary in half, so the new mix allows for way more variety.

A note though, the OANC mix contains **mostly** cross-hand words, which I just appended to the previous list. The reason is simple: there is no need to mimic the words generation to make it feel like a typing test app. That initially was the goal, but I realised this feature belongs in typing tests, especially since I am using custom word scoring.

And another, important note: the new curated dictionary is quite overwhelmed with words longer than 3, which was not as much of a problem earlier. That's around `150 : 350` ratio for just five letters long words (previously: `1:3.2`), which in combination with seven letters long words will sound insane for something available and selected by default.

Approximate breakdown for word lengths:

- `3` ~ 150
- `5` ~ 350
- `7` ~ 275
- `9` ~ 45
- `11` 8

In my defense, the new words that were added, were scored in a way that every letter lands on different hand, having at most one letter on the same hand. This only applies to the second part of the dictionary, as the first half is just the standard word frequency list (mostly). Some exceptions were made, e.g. natural rolls, like the word: `serpent`:

```plaintext
// Natural roll defines consecutive letters landing from "outward fingers, going inwards" on the same hand
L: S E R
R: P
L: E
R: N
L: T
```

So, with that in mind, even if the longer letters inflate the dictionary a bit, they should be easier to type, at least in theory, but what do I know... sounds counterintuitive. Longer words, more stamina and focus required. I manually went through the list again and removed some weird stuff, and again, only kept those that were actually nicer to type after the scoring script spat them out.

A kinda questionable change to the custom dictionary was that I left some words that represent names and countries, because some of them were actually nice to type (for me, sorry), even if most typing test apps deem them inappropriate (too uncommon in written english).

Another reason why the inflation happened was that I took all 4 and 6 letter words and pluralized them since all even-length words got removed. Not all of them got in, of course. The dictionary would be even bigger, but during the scoring, I decided to "ban" some awkward letter transitions, but we're not gonna talk about that.

#### Note on Curated Full Alt dictionary

This is a new, **experimental** addition to the list, which made me go back to the dictionary preparation. This was from a dictionary state before scoring the `Extended` dictionary from **OANC** and Project **Gutenberg**.

The main `Curated` dictionary was initially created by scoring the words in a way that they heavily favoured hand alternating with some leniency, allowing key repetitions and some same-hand movements.

With this however, I decided to ignore these rules and took **only** those words that had all their letters land on the opposite hand for a **true** full alt experience (along with the forced cross hand customisation). Now, I know, that's now really typing, but this ruleset is not a typing test either.

There is also a reason behind this experimental dictionary. In a rhythm game, you are not really taking a typing test, and this is also not about typing natural english-like text with words predicted by algorithms. Rhythm games are about the eye-hand coordination, about the mapping an on-screen object to a finger and then the key.

While this is true that linguistic familiarity helps a ton here as your brain predicts which finger is going to be used next. You would already know how to type an incoming word, but in the end, it's all just about reacting with the correct finger to the next letter. It will probably not be accurate at all, but imagine extending osu!taiko to have 26 inputs and trying to hit a uniquely coloured object.

I don't think that word complexity makes a huge difference here, because with players having exceptionally good reaction time in osu! already, reading absurdly fast scroll speeds, the adaptation would be pretty smooth, no matter if the words are something they have never seen before. A word is just a pattern represented by a group of inputs. It doesn't matter as much how complex the word is, it later comes down to how quickly you can translate a letter in the word to correct finger and spot.

### A TL;DR On How To Use This

This ruleset is primarily made for `Words` mod, where you type random words from the selected dictionaries, the beatmap contents are **replaced**, no structure is preserved, as intended. This is the same concept as `TaikoModFullRandom`. I am fully aware of the fact that those beatmaps make zero sense structurally.

- Go to Mod Selection
- Select `Words` Mod
- Customise the `Words` Mod mostly by adjusting the `Letter Spacing`:
  - `Narrow`(\*) - letters land on `1/4`, used for lower BPMs to make the game "play faster" without changing the song rate (100 BPM evaluates to around 80 WPM, 150 BPM -> 120 BPM etc.)
  - `Default` - default, letters land on `1/2`, used for 200~ BPM (~80WPM) or 100~ BPM (40 WPM)
  - `Wide` - letters land on `1/1`. Not really used, maybe for starter WPMs around 40-60 (if songs were mapped to 200-300 BPM)
- Probably add `Constant Speed` if there are any `Scroll Speed` changes
- Mod Selection shows the approximate `WPM` based on the most common BPM next to `Difficulty Attributes`
- Adjust `Scroll Speed` with keybinds (F3-F4 by default)

#### Note on Letter Spacing

Very short:

```plaintext
          1/1
    beat   | 4 2 4 | . . . | . . . |
Narrow     W O R D
Default    W . O . R . D
Wide       W . . . O . . . R . . . D
```

### ZERO Skin Elements

I decided against making any skin components that would make this look nice and went with just the playfield and simple letters. That was my design choice, but there is a good reason behind this:

- Parsing incoming words on the fly is already hard, especially if you are not a native English speaker
- Nearby skin elements might be too distracting

So, this basically means there will be none of these ever:

- Playfield styling
- Ruleset Character
- Non-miss `HitObject` Judgement display (`max`, `good` etc.)

I don't mind the playfield being basically empty. I could technically make them optional, just like the `Skinnable Key Timing Distribution`, but it's not really worth it.

By default, whole playfield is intentionally empty, so cluttering the window is going to be on player's side.

### Work-in-progress

This ruleset will be in _work-in-progress_ state for a long time, because I don't feel like adding or fixing stuff, as the foundation is already sufficient for me to play.

> [!Note]
> The mod is applied to all visible difficulties, so the structure of the current beatmap difficulty does not matter, because the mod replaces the entire beatmap. This will result in all difficulties having the same Star Rating if custom Seed is applied.
>
> If all beatmaps have the same Drain Time, they will have a very similar Star Rating. This is intentional given how the mod works and it does not matter which difficulty is played with this mod.

### Custom Mod Requirement

Since the whole idea was to play with generated words, it's mandatory to have the `Words` mods enabled:

![img](https://i.imgur.com/3O2Sfxr.png)

It's still possible to play the actual beatmap with random letters, but I disabled the difficulty calculation for this and explicitly require `Words` mod to be present in order to calculate the star rating. It's not correct anyway, but I'm just experimenting. There is a better ruleset for this in the ruleset collection in the repository, ppy added a `Typer` ruleset, which I just now discovered, go play that for random letters and preserved beatmap objects.

The reason for such low amount of mods is that it's pretty much what's sufficient:

- `NoFail` - this was not supposed to be here since `Difficulty Adjustment` was added to adjust HP drain, but it's just faster to keybind this instead
- `DoubleTime` / `HalfTime` for rate adjustment
- `Constant Speed` to remove annoying scroll speed changes
- `Difficulty Adjustment` mostly for `OD` adjustment and scroll speed fine-tuning, sometimes for `HP`

> [!Note]
> While `Scroll Speed` can be adjusted through the `Difficulty Adjustment` mod, the keybinds for `Increase/Decrease` scroll speed will also work in case the beatmap Scroll Speed is super low and the keybind adjustment happens to not increase as much on the maximum value (`100`).

### Finger Guide

I had this weird idea of colouring the objects so they map to specific fingers, which I don't even think is useful, but I left it in anyway. Might be more distracting later on, but I think the colour-to-finger mapping adaptation is quite fast.

I also didn't add any images for this, because I wasn't planning on keeping this mod. Not like anyone would choose this to learn touch-typing over things like `Keybr`.

![alt](https://i.imgur.com/OFvaKPn.png)

![alt](https://i.imgur.com/rgroVQ7.png)

(This is my osu! layer for this ruleset)

The reason I didn't add a guide image for this is because osu!lazer already allows you to add a custom image through skinning, so I won't bother preparing coloured layouts. You can add one yourself through:

- Skin Editor -> File -> Edit Externally
  - Add a custom image to the folder opened by osu!
  - Go back to osu! and import changes
  - Select: `Skinnable Sprite`
  - Select your image from the dropdown (if this is the only image in the folder, it will be there by default)

![alt](https://i.imgur.com/sNxBqc7.png)

### About the Custom Mod

This mod recommended to be customised, because beatmaps are so different, it's literally impossible to make all of them playable with a click of a button.

I wanted to be able to play this ruleset on a wide range of beatmaps, so I added the following setting: `Letter Spacing`. This will effectively double or halve the BPM to let me play for example a 100 BPM beatmap. The reason why is that the mod is generating hit objects at 1/4 for words, 1/2 for spaces between the words. This can be quite slow, so without having to use the DoubleTime to fine-tune and play on rate changed song, I can play it on 200 BPM, which is roughly 80 WPM.

_Words Per Minute_ can be calculated simply by dividing BPM by 5. Average word is assumed to be 5 letters, thus 5 beats, but since letters are spaces by 1/2, this value becomes 2.5. This only applies to `Default Letter Spacing`.

E.g. if a song is 260 BPM and default settings are used (like playing 130 BPM), the WPM is approximately 104.

> [!Note]
> For example, 120 WPM can be set through the following
>
> - 150 BPM + `Narrow Letter Spacing` (on 1/4): `150 BPM / 1.25` or `150 BPM * 0.8` ~ 120 WPM
> - 300 BPM + `Default Letter Spacing` (on 1/2): `300 BPM / 2.5` or `300 BPM * 0.4` ~120 WPM
> - 240 BPM + DT -> 300 BPM + `Default Letter Spacing`

I'm not really covering the `Wide Letter Spacing` (`5` divisor or `0.2` BPM multiplier), because it's meant for lower WPM, which I'm not really interested in.

But, in short, `60 WPM` with this customisation can be set on things like:

- 240 BPM + DT -> 300 BPM

#### Snapping Words to 1/1

There is also a setting to increase the spacing between words, because why not?

This, instead of inserting a one letter break, it inserts at least a full 1/1 break, which gives enough recovery time between the words, allowing to play on higher BPMs, since there is more time to fully parse the incoming word.

That comes with a cost of lowered difficulty and score, though, since the sustained speed and typing fatigue decreases with such breaks, so, glad the difficulty calculation can pick that up (somewhat, but it's still jank).

This has been changed from the initial double spacing, which created uncomfortable and variable off-beats.

To visualise how this looks, here's an example for all 1/3/5/7 letters long words (above this it wraps around, conveniently):

```plaintext
X marks where the next word starts

   |-full beat
   | . |-half beat
   | . . . | . . . | . . . | . . . |
1  I . . . X . . . | . . . | . . . |
3  H E R . | . . . X . . . | . . . |
5  P L A Y S . . . X . . . | . . . |
7  W I T H O U T . | . . . X . . . |
// Wrap around after 8 letters: 1 -> 3 -> 5 -> 7
9  M E L B O U R N E . . . X . . . |
11 I N D I V I D U A L S . | . . . X
```

To explain the spacing choice:

- Words 1/5 letters long actually **end** on full beat, they use the same `one beat` spacing
- Words 3/7 letters long **end** on `half beat`, so the spacing has to be `half beat + whole beat`

The reason why 3/7 letters long have wider spacing is because the next word would appear too quick and it would not feel much different than the default mode without the extra spacing.

Also, the spacing is different between the two pairs, because I didn't want the words to end on `half beat` if I always used the same spacing. It would kind of feel awkward to play.

Technically, there is still a tiny detail about the rhythm, when starting the word on an `even beat` (every other 1/1), but i don't want to get too deep on this.

There is a small exception to this rule when the words have small spacing (I think), you sort of start ignoring the "off half beat" if patterns are quite dense.

##### Unintentional Bonus Mode

_Surprisingly_, the word snapping customisation kind of works really well on higher BPMs + `Narrow Letter Spacing` for more "burst-oriented" gameplay. Even `Bonus Spaces` are somewhat playable with this.

For example, if you can burst 160WPM, you can play a 200BPM beatmap with `Narrow Letter Spacing` and the word snap.

#### Banned Consonants

This will skip all words containing any of the consonants from this component. You can ban a maximum of 8 consonants. You can simply type something like `xcvbj`.

Not really recommended for usual gameplay, this is more like a _chill_ setting.

#### Forced Cross-Hand on New Words

I decided to add this option and make it `on` by default.

The reasoning behind is that I already kind of took the inspiration from taiko, where alternating hands make some patterns more comfortable to play, so that's also why the custom dictionary was extended to contain more _almost_ "full alt" words, with some exceptions, e.g. allowing at most one or two keys on same hand on longer words, and `-ing` trigram.

Taking this further, I changed how the mod behaves by forcing the next word to start on different hand. Look at the following example:

| Previously                                                        | Now                      |
| ----------------------------------------------------------------- | ------------------------ |
| figh**t** - `L` into `L` `[nope]` <- kinda meh transition to next | figh**t** - `L` into `R` |
| go**d** - `L` into `L` `[nope]`                                   | poin**t** - `L` into `R` |
| dither**s**                                                       | ma**n**                  |

Not like it matters anyway, some transitions are not that awkward, e.g. same letter, natural roll. I didn't really wanted to think much about this, so I just rolled with it.

In general, hand switch is more comfortable, because in this situation, when one hand types, another one is getting ready to type, so this can be treated as difficulty reduction I guess (?)

Anyway, this can be disabled as I didn't want to force the mod to generate words like that.

##### Important Note on Cross-Hand Play

While, as already mentioned, this is based on taiko gameplay favouring hand alternating, this is obviously not how you normally type, because naturally, typists develop "chording" or they "violate" the touch-typing rules to type even faster.

None of this really applies here, because:

> - You are required to be 100% consistent for perfect score.

Consistency in typing means your time between key presses are nearly identical, which means your consistency drops when chording, which then translates into not hitting the correct `HitWindow` for all letters. So, sadly, chording might not be useful here, because you have to force yourself to wait for correct letter judgement.

To be clear, not like it's a bad thing, because every day typing is not a game, you don't have to type at perfect rate to everyone. Chording is fun.

Anyway, take a look at this example:

- `[R] po` | `[L] wer` | `[R] poin` | `[L] t`
- Sometimes even with left hand overreaching `[R] po` | `[L] wer` | `[R] poi` | `[L] nt`

Chording helps with "reducing" the keystrokes, but I'm not getting into the theory of this. It increases the Unstable Rate unless you force your hands to get into chording position and only type at consistent rate, which I believe is more taxing (?)

#### Word Seed

This makes _everything_ deterministic. Custom seed should only be used if you intend on repeating the play on the same beatmap, because this effectively makes _all_ visible beatmaps to be regenerated with the same seed.

It results in the same exact Star Rating across all beatmaps with the same properties, especially in a beatmapset, e.g. if all difficulties in a beatmap have the same drain time and no BPM changes, the Star Rating will be the same.

It would seem like there is no point in using the seed, because ultimately the only difference would be how individual songs feel.

#### Space Between Words as Bonus

Although out of order, had to save this one for last. I really didn't want to add this, but I felt like the mod wouldn't be "whole" without it. The TL;DR of it is that it creates some issues I don't know how to deal with.

Either way, normally, between the words there is an extra spacing so you have extra time to parse next word and don't have to press space. What this option does is that in the place of that spacing, it inserts a `bonus` HitObject:

![alt](https://i.imgur.com/84ldmoa.png)

I didn't really have anything better to put there as a clickable Space, so I used the `|` character, which is scaled up so it doesn't mix with the text.

The bonus is calculated to be 25% extra of maximum score, so you get `1_250_000` with an SS.

I initially set it to 10%, but it's really hard to play with this as it's a constant stream...

I'm not really using it myself, because this technically creates an actual stream that basically forces you to 100% lock in or you die (there are literally no recovery times since you are always pressing something). This is basically the main reason why, by default, space was never really intended to be used.

So, I will only treat this as an experiment on `ScoreProcessor` and optional objects.

##### The Issues

My design choice was to not make it a required `HitObject`, e.g. adding the same object that is used for letters, but with Space, so I made a new one, whose `HitResult` is `LargeBonus`, which makes it so that you can miss it safely or even press a different key on it without losing combo.

Now, the issue is that while I wanted this to be fully optional, its `HitWindow` may overlap with the letters if you play on low `OD`. This created an issue where you are actually required to hit it, otherwise you will note-lock, because you will attempt to start typing a word when `Space` hit window was still active, which, sadly sucks, but I couldn't figure anything out here.

If I were to make the `Space` hit window very narrow, which would make it harsher than OD10 taiko hit to fix this (somewhere around OD10 mania MAX), it would "work", but man, try hitting space a bit late and you start note-locking again.

#### Customisation

![alt](https://i.imgur.com/B5zYJU4.png)

### Scoring Changes

While Scoring was basically taken from osu!taiko, I made some small changes:

- `Overall Difficulty` will award up to +25% extra score, which is calculated from a set range of OD. I initially set it at 5-10, so for `OD 6`, the bonus is 5% and so on. Below this value, the score is deducted for balance purposes to not make `OD 0` be worth more than `OD 5`, which requires more accuracy
- `Perfect` takes a bigger max score portion, because there was no difference between Perfect and Great and there was no real reason to play more accurately. I bumped it from 300 to 325 (an arbitrary number for now). This is a combo between high OD and an indirect force on accuracy. This might result in tightening the window for Perfects on high OD, though
- The `Words` mod can insert `Space` between the words, which grant extra score. This bonus score is set to 25% of total max score. Because it is very note-lock prone on faster beatmaps, the bonus is high.
- The `Words` mod allows adjusting the Letter Spacing. It directly affects the score, because it can double the amount of objects or halve it. This will change the score from 2x to 0.5x

Anyway, the extra `Space` keys customisation is not just free score. It's effectively a constant stream, which greatly increases the risk of missing/note-locking, especially on higher BPM. On slow beatmaps it's a "whatever". I'm aware that all this doesn't really answer the question why `Spaces` are bonus objects, this was just the decision I made.

That being said, a quick breakdown of total score based on the Letter Spacing:

| Spaces / Letter Spacing | Narrow        | Default       | Wide        |
| ----------------------- | ------------- | ------------- | ----------- |
| Off                     | 2 000 000     | 1 000 000     | 500 000     |
| On                      | 2 **500 000** | 1 **250 000** | **625 000** |

`Spaces` are excluded from **Overall Difficulty** multiplier, so on `OD10`, the maximum score for SS with `Spaces` is `3 000 000`. Should barely be possible on higher WPMs anyway.

### WPM In Beatmap Attributes

Since I wanted a WPM preview with the mods applied on the beatmaps, I added it to the song selection:

![alt](https://i.imgur.com/AtLbgVa.png)

![alt](https://i.imgur.com/dhPX6WR.png)

#### Other Beatmap Attributes

Since WPM is not really enough, other attributes are also kind of important.

OD grants extra score, so it would be nice to also have an adjustable value visible in the song/mod select. Same with the HP to be honest, because high HP can be devastating on accuracy drops, so, good to know what's the current difficulty on the beatmap.

I also added the computed `Total Score` to the beatmap attributes with a breakdown thanks to `Additional Metrics`:

![alt](https://i.imgur.com/vMTmEfA.png)

Although, it does not look very good in the mod select if HP is a two-digit...

![alt](https://i.imgur.com/wFhuzyy.png)

That's a _whatever_.

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

_I am aware that you can just drop the built `.dll` onto your main osu!, but I made other changes that won't allow me to drop the other ones._

### Difficulty Calculator for Typing Ruleset

While this was completely unnecessary, because in the end, all words are random, and I believe you can't measure the difficulty of randomness, I did it anyway, because I was just curious how it will look like.

I myself did no research on measuring the typing difficulty whatsoever, I did end up asking slop gpt, though. There, I said it. I even asked it to come up with some formulas, but it was so dumb, I had to delete absolutely everything and throw random functions around instead. I only left the concepts from it, which I turned into the `Skills` from the difficulty calculator. Still, had to come up with the code myself.

While I had four other rulesets at my disposal, it would make zero sense to copy what they do if I have no clue what the hell I am doing. I understand that the code for difficulty calculation will look weird to anyone who knows actual Maths, this is still just a personal project, so I don't think I care enough. That also applies to code quality.

I included a bunch of tests which I used to tweak the values, which are in the end based on aiming to get a four stars beatmap from 140 BPM (112 WPM) over three minutes of gameplay. I don't know how Strain Skills work anyway, so I just rolled with it and only glanced at other skills without checking what they do, then tweaked the values, looking at graphs.

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

![alt](https://i.imgur.com/elxKdHD.png)

Couldn't really come up with anything better for now, I just needed the Unstable Rate. The size is hardcoded, though, couldn't figure out how to make the auto scale to work. The cards show the key with successful hits (green) and missed (red). The colours were taken directly from Star Rating, except I made it darker, because with more surface area, they were too prominent.

The colour spectrum from SR, where starting point (green) is `80`, ends on `260` (dark blue).

![alt](https://i.imgur.com/qz0Jbcp.png)

The current colour spectrum for keys don't include Blue, I just started from Green, which naturally says "good", and the value for Unstable Rate where the colour starts fading into others is `80`. Just an arbitrary value I chose for the lowest I could personally reach while playing, so there was no real tweaking for what the best value would be.

Conveniently, the mid point where the player should consider the UR to be bad is where the text colour changes from black to yellow, which is at `160` Unstable Rate. After that, the keys become darker, meaning that the play was either offset or just bad and needs improvement.

When the text colour is black, you aim for improvement from pink to orange, which is a jump from ~`150` to `120` UR. Then it's a pure grind to yellow.

## Skinnable Key Timing Distribution

Initially, I added the Unstable Rate preview to the result screen as mentioned above, but I decided to also add this to the skin editor instead of creating a keyboard layout with flashing keys as you type. This was the laziest implementation and I didn't even want to do it anyway, but since I'd sometimes want to have a live preview of the Unstable Rate for videos, I just reused the result screen component.

Might be a bit too distracting, though...

![alt](https://i.imgur.com/qamL2EV.png)

## Replays

I initially rejected the idea of having replays due to some technical issues (skill issues), but after another swing at it, I attempted to make the replays work, which I thought would work with everything that is provided by osu! out of the box.

But, since apparently, the replays are locked to the legacy rulesets, custom rulesets have to store the replays through their own internals, I took some time to look into it again. In general, and after some playtests, the replays should work for regular gameplay and failed scores (*).

> [!Note]
> (*) There was some jank happening with failed scores where replay frames were generated with time going in reverse.

I still don't know how to use osu! Realm, so it will eventually break.

> [!Important]
> Replay importing is not possible for now, I don't think I care enough to look into that.

### Missing Features

Some stuff I _might_ add/do if I won't get lazy:

- Finally extract the typing ruleset and publicly share it
