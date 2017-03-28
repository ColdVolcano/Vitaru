﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics.Containers;
using System;
using osu.Framework.Graphics;
using osu.Game.Modes.Vitaru.Objects.Projectiles;
using OpenTK;

namespace osu.Game.Modes.Vitaru.Objects.Characters
{
    public abstract class Character : VitaruHitObject
    {
        public float CharacterHealth { get; set; } = 100;
        public float Armor { get; internal set; } = 1; //All damage taken should be divided by this number. During kiai player will only take half damage so [2]
        public int Team { get; set; } = 0; // 0 = Player, 1 = Ememies + Boss(s) in Singleplayer
        public int ProjectileDamage { get; set; }
        public int BPM { get; set; } = 180;

        public Character(Container parent)
        {
            MainParent = parent;
        }

        protected override void Update()
        {
            base.Update();
            foreach (Drawable draw in MainParent.Children)
            {
                if (draw is Bullet)
                {
                    Bullet bullet = draw as Bullet;
                    if (bullet.Team != Team)
                    {
                        Vector2 bulletPos = bullet.ToSpaceOfOtherDrawable(Vector2.Zero, this);
                        float distance = (float)Math.Sqrt(Math.Pow(bulletPos.X, 2) + Math.Pow(bulletPos.Y, 2));
                        float minDist = Hitbox.HitboxWidth + bullet.BulletWidth;
                        if (distance < minDist)
                        {
                            bullet.DeleteBullet();
                            if (TakeDamage(bullet.BulletDamage))
                                break;
                        }
                    }
                }
            }
            if (Shooting)
            {
                timeSinceLastShoot += Clock.ElapsedFrameTime;
                if (timeSinceLastShoot / 1000.0 > 1 / BPM / 30.0)
                {
                    OnShoot?.Invoke();
                    timeSinceLastShoot -= 1 / (BPM / 30.0) * 1000.0;
                }
            }
        }
    }
}
