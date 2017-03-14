﻿using osu.Framework.Graphics.Containers;
using OpenTK;
using OpenTK.Input;
using osu.Game.Modes.Vitaru.Objects.Drawables;
using osu.Framework.Graphics;
using osu.Framework.Input;
using System.Collections.Generic;
using System;
using osu.Game.Modes.Vitaru.Objects.Projectiles;
using OpenTK.Graphics;

namespace osu.Game.Modes.Vitaru.Objects
{
    public abstract class BulletPatterns : Container
    {
        public float patternAngle;
        public int Team;

        public void Pattern1(Container parent)
        {
            for (int i = 0; i < 6; i++)
            {
                Bullet bullet;
                parent.Add(bullet = new Bullet(Team)
                {
                    Depth = 1,
                    Anchor = Anchor.Centre,
                    BulletAngle = patternAngle,
                    BulletSpeed = i/10,
                    BulletColor = Color4.Green,
                });
                bullet.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), bullet));
            }
        }
    }
}
