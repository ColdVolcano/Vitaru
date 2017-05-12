// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics;
using OpenTK;
using System;
using OpenTK.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Extensions.Color4Extensions;

namespace osu.Game.Rulesets.Vitaru.Objects.Projectiles
{
    public class Bullet : Projectile
    {
        //Different stats for Bullet that should always be changed
        public int BulletDamage { get; set; } = 10;
        public Color4 BulletColor { get; set; } = Color4.White;
        public float BulletSpeed { get; set; } = 1f;
        public float BulletWidth { get; set; } = 12f;
        public float BulletAngleRadian { get; set; } = -10;
        public bool UpdateVelocity { get; set; } = false;

        //This is an extra 10 outside of playerbounds intentionally. There is No escape.
        private Vector4 BulletBounds = new Vector4(-10, -10, 522, 830);

        public static int BulletCount = 0;

        //Result of bulletSpeed + bulletAngle math, should never be modified outside of this class
        private Vector2 bulletVelocity;

        private Container bulletRing;
        private CircularContainer bulletCircle;


        public Bullet(int team)
        {
            Team = team;
        }

        protected override void LoadComplete()
        {
            BulletCount++;
            GetBulletVelocity();
            Children = new Drawable[]
            {
                bulletRing = new Container
                {
                    Masking = true,
                    AutoSizeAxes = Axes.Both,
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    BorderThickness = 3,
                    Depth = 5,
                    AlwaysPresent = true,
                    BorderColour = BulletColor,
                    Alpha = 1f,
                    CornerRadius = BulletWidth,
                    Children = new[]
                    {
                        new Box
                        {
                            Colour = Color4.White,
                            Alpha = 1,
                            Width = BulletWidth * 2,
                            Height = BulletWidth * 2,
                            Depth = 5,
                        },
                    },
                },
                bulletCircle = new CircularContainer
                {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Scale = new Vector2(BulletWidth * 2),
                        Depth = 6,
                        AlwaysPresent = true,
                        Masking = true,
                        EdgeEffect = new EdgeEffect
                        {
                            Type = EdgeEffectType.Shadow,
                            Colour = (BulletColor).Opacity(0.5f),
                            Radius = 2f,
                        }
                }
            };
        }

        public Vector2 GetBulletVelocity()
        {
            bulletVelocity.X = BulletSpeed * (((float)Math.Cos(BulletAngleRadian)));
            bulletVelocity.Y = BulletSpeed * ((float)Math.Sin(BulletAngleRadian));
            return bulletVelocity;
        }

        protected override void Update()
        {
            base.Update();
            MoveToOffset(new Vector2(bulletVelocity.X * (float)Clock.ElapsedFrameTime, bulletVelocity.Y * (float)Clock.ElapsedFrameTime));

            //Will be useful for makin bullets stop, like if a certain character / boss could freeze time.
            if (UpdateVelocity)
                GetBulletVelocity();

            if (Alpha < 0.05)
                DeleteBullet();

            if (Position.Y < BulletBounds.Y | Position.X < BulletBounds.X | Position.Y > BulletBounds.W | Position.X > BulletBounds.Z)   
                fadeOut();
        }

        private void fadeOut()
        {
            if(Alpha == 1)
                FadeOut(500);
        }

        internal void DeleteBullet()
        {
            Dispose();
        }
    }
}