// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Vitaru.Objects;
using System.Collections.Generic;
using osu.Game.Rulesets.Objects.Types;
using System;
using osu.Game.Rulesets.Vitaru.UI;
using osu.Game.Rulesets.Beatmaps;
using osu.Game.Rulesets.Vitaru.Objects.Characters;
using osu.Game.Rulesets.Vitaru.Objects.Drawables;

namespace osu.Game.Rulesets.Vitaru.Beatmaps
{
    internal class VitaruBeatmapConverter : BeatmapConverter<VitaruHitObject>
    {
        private bool playerLoaded = false;
        protected override IEnumerable<Type> ValidConversionTypes { get; } = new[] { typeof(IHasPosition) };

        protected override IEnumerable<VitaruHitObject> ConvertHitObject(HitObject original, Beatmap beatmap)
        {
            var curveData = original as IHasCurve;
            var endTimeData = original as IHasEndTime;
            var positionData = original as IHasPosition;
            var comboData = original as IHasCombo;
            
            if (playerLoaded == false)
            {
                playerLoaded = true;
                VitaruPlayer.PlayerPosition = new Vector2(256, 800);
                yield return new VitaruPlayer
                {
                    //This Position does nothing atm
                    Position = new Vector2(256, 800),
                    StartTime = 0f,
                };
            }
            if (curveData != null)
            {
                yield return new Enemy
                {
                    StartTime = original.StartTime,
                    Samples = original.Samples,
                    ControlPoints = curveData.ControlPoints,
                    CurveType = curveData.CurveType,
                    Distance = curveData.Distance,
                    //RepeatSamples = curveData.RepeatSamples,
                    RepeatCount = curveData.RepeatCount,
                    Position = positionData?.Position ?? Vector2.Zero,
                    NewCombo = comboData?.NewCombo ?? false,
                    IsSlider = true,
                };
            }
            else if (endTimeData != null)
            {
                yield return new Enemy
                {
                    StartTime = original.StartTime,
                    Samples = original.Samples,
                    //EndTime = endTimeData.EndTime,
                    //IsSpinner = true,

                    Position = positionData?.Position ?? VitaruPlayfield.BASE_SIZE / 2,
                };
            }
            else
            {
                yield return new Enemy
                {
                    StartTime = original.StartTime,
                    Samples = original.Samples,
                    Position = positionData?.Position ?? Vector2.Zero,
                    NewCombo = comboData?.NewCombo ?? false,
                };
            }
        }
    }
}
