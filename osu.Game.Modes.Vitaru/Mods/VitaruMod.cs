﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Modes.Mods;
using osu.Game.Modes.Osu.Objects;
using System;
using System.Linq;

namespace osu.Game.Modes.Vitaru.Mods
{
    public class VitaruModNoFail : ModNoFail
    {

    }

    public class VitaruModEasy : ModEasy
    {

    }

    public class VitaruModHidden : ModHidden
    {
        public override string Description => @"Play with no approach circles and fading notes for a slight score advantage.";
        public override double ScoreMultiplier => 1.06;
    }

    public class VitaruModHardRock : ModHardRock
    {
        public override double ScoreMultiplier => 1.06;
        public override bool Ranked => true;
    }

    public class VitaruModSuddenDeath : ModSuddenDeath
    {
        public override bool Ranked => true;
    }

    public class VitaruModDoubleTime : ModDoubleTime
    {
        public override double ScoreMultiplier => 1.12;
    }

    public class VitaruModHalfTime : ModHalfTime
    {
        public override double ScoreMultiplier => 0.5;
    }

    public class VitaruModNightcore : ModNightcore
    {
        public override double ScoreMultiplier => 1.12;
    }

    public class VitaruModDoubleTrouble : ModDoubleTrouble
    {
        public override double ScoreMultiplier => 1.12;
    }

    public class VitaruModPerfect : ModPerfect
    {

    }

    public class VitaruModAutoplay : ModAutoplay<OsuHitObject>
    {
        protected override Score CreateReplayScore(Beatmap<OsuHitObject> beatmap) => new Score
        {
            Replay = new VitaruAutoReplay(beatmap)
        };
    }

    public class OsuModTarget : Mod
    {
        public override string Name => "Target";
        public override FontAwesome Icon => FontAwesome.fa_osu_mod_target;
        public override string Description => @"";
        public override double ScoreMultiplier => 1;
    }
}
