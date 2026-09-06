# Game Samples

Just leaving a notice here since some of the samples may come from the Resources repository, which has a different licencing.

## key-caps.wav

I felt like this sample was fitting, but I had to do some modifications in order to use it in the `TypingRuleset`.

- Original source of the file from `osu-resources`: [key-caps.mp3](https://github.com/ppy/osu-resources/blob/master/osu.Game.Resources/Samples/Keyboard/key-caps.mp3)
- osu!resources Licence: [CC-BY-NC 4.0](https://github.com/ppy/osu-resources/blob/master/LICENCE.md)
- Modifications: trimmed and amplified audio, exported to `.wav` to avoid `mp3` padding (the pesky silence at start)
- Usage: forced default sample for `HitNormal`

```cs
// DrawableTypingHitObject.cs
public override IEnumerable<HitSampleInfo> GetSamples() => new[] { new HitSampleInfo("key-caps") };
```

---
