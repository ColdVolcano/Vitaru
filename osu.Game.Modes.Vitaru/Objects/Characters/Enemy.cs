﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics.Containers;
using OpenTK;
using osu.Game.Modes.Vitaru.Objects.Drawables;
using osu.Framework.Graphics;
using OpenTK.Graphics;
using osu.Game.Modes.Vitaru.Objects.Projectiles;
using System;
using System.Collections.Generic;
using osu.Framework.MathUtils;
using osu.Framework.Graphics.Transforms;
using osu.Game.Beatmaps.Samples;

namespace osu.Game.Modes.Vitaru.Objects.Characters
{
    public class Enemy : Character
    {
        public bool Shoot = false;
        public Vector2 EnemyPosition = new Vector2(0, -160);
        public Vector2 EnemySpeed { get; set; } = new Vector2(0.5f, 0.5f);
        public BulletPattern Pattern { get; set; }

        public Vector2 EnemyVelocity;
        public float EnemyAngle;
        public Action OnShoot;

        public static Vector2 EnemyPos;
        

        //Main Enemy Function
        public Enemy() : base() { }
        public override HitObjectType Type => HitObjectType.Enemy;
    }
}