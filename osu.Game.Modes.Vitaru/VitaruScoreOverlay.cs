﻿using System;
using osu.Game.Graphics.UserInterface;
using osu.Game.Modes.UI;
using osu.Game.Screens.Play;

namespace osu.Game.Modes.Vitaru
{
    internal class VitaruHudOverlay : HudOverlay
    {
        protected override PercentageCounter CreateAccuracyCounter()
        {
            throw new NotImplementedException();
        }

        protected override ComboCounter CreateComboCounter()
        {
            throw new NotImplementedException();
        }

        protected override HealthDisplay CreateHealthDisplay()
        {
            throw new NotImplementedException();
        }

        protected override KeyCounterCollection CreateKeyCounter()
        {
            throw new NotImplementedException();
        }

        protected override ScoreCounter CreateScoreCounter()
        {
            throw new NotImplementedException();
        }
    }
}