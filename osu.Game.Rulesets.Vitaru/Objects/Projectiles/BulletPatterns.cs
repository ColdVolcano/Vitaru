// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics.Containers;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Vitaru.Objects.Drawables;
using System;
using osu.Game.Rulesets.Vitaru.UI;
using osu.Game.Rulesets.Vitaru.Objects.Projectiles;

namespace osu.Game.Rulesets.Vitaru.Objects.Projectiles
{
    public abstract class BulletPattern : Container
    {
        public abstract int PatternID { get; }
        public float PatternSpeed { get; set; }
        public float PatternComplexity { get; set; } = 1;
        public float PatternAngleRadian { get; set; } = -10;
        public float PatternAngleDegree { get; set; } = 0;
        public float PatternBulletWidth { get; set; } = 2;
        public int Team { get; set; }

        public Color4 PatternColor { get; set; } = Color4.White;
        protected int bulletCount { get; set; } = 0;

        public BulletPattern()
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }

        protected override void Update()
        {
            base.Update();

            if (bulletCount < 1)
                Dispose();
        }
        protected void bulletAddRad(float speed, float angle)
        {
            bulletCount++;
            Bullet bullet;
            VitaruPlayfield.vitaruPlayfield.Add(bullet = new Bullet(Team)
            {
                Origin = Anchor.Centre,
                Depth = 5,
                BulletColor = PatternColor,
                BulletAngleRadian = angle,
                BulletSpeed = speed,
                BulletWidth = PatternBulletWidth,
            });
            bullet.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), bullet));
        }
    }
    public class Wave : BulletPattern
    {
        public override int PatternID => 0;

        Bullet b;
        public Wave(int team)
        {
            Team = team;
        }
        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (PatternAngleRadian == -10)
                PatternAngleRadian = MathHelper.DegreesToRadians(PatternAngleDegree - 90);

            float directionModifier = -0.1f * PatternComplexity;
            for (int i = 1; i <= (3 * PatternComplexity); i++)
            {
                bulletAddRad(PatternSpeed, PatternAngleRadian + directionModifier);
                directionModifier += 0.1f;
            }
        }
    }
    public class Line : BulletPattern
    {
        public override int PatternID => 1;

        Bullet b;
        public Line(int team)
        {
            Team = team;
        }
        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (PatternAngleRadian == -10)
                PatternAngleRadian = MathHelper.DegreesToRadians(PatternAngleDegree - 90);

            for (int i = 1; i <= 3 * PatternComplexity; i++)
            {
                bulletAddRad(0.12f + PatternSpeed, PatternAngleRadian);
                PatternSpeed += 0.02f;
            }
        }
    }
    public class Flower : BulletPattern
    {
        public override int PatternID => 2;

        Bullet b;
        public Flower(int team)
        {
            Team = team;
        }
        protected override void LoadComplete()
        {
            base.LoadComplete();

            double timeSaved = Time.Current;
            int a = 0;
            for (int j = 1; j <= 16 * PatternComplexity; j++)
            {
                a = a + 21;
                PatternAngleRadian = MathHelper.DegreesToRadians(a - 90);
                bulletAddRad(PatternSpeed, a);
            }
        }
    }
    public class DirectStrike : BulletPattern
    {
        public override int PatternID => 3;


    }
}


/*            switch (bulletPattern)
            {
                case 1: // Wave
                    directionModifier = -0.1f * patternDifficulty;
                    for (int i = 1; i <= (3 * patternDifficulty); i++)
                    {
                        bulletAddRad(0.15f, randomDirection + directionModifier);
                        directionModifier += 0.1f;
                    }
                    break;

                case 2: // Line
                    speedModifier = 0;
                    for (int i = 1; i <= 3 + patternDifficulty; i++)
                    {
                        bulletAddRad(0.12f + speedModifier, randomDirection);
                        speedModifier += 0.02f;
                    }
                    break;

                case 3: // Cool wave
                    speedModifier = 0.02f + 0.01f * (patternDifficulty - 1);
                    directionModifier = -0.15f - 0.075f * (patternDifficulty - 1);
                    for (int i = 1; i <= 3 + patternDifficulty * 2; i++)
                    {
                        bulletAddRad(
                            0.1f + Math.Abs(speedModifier),
                            directionModifier + randomDirection
                        );
                        speedModifier -= 0.01f;
                        directionModifier += 0.075f;
                    }
                    break;

                case 4: // Circle
                    directionModifier = (float)(90 / Math.Pow(2, patternDifficulty));
                    circleAngle = 0;
                    for (int j = 1; j <= Math.Pow(2, patternDifficulty + 2); j++)
                    {
                        bulletAddDeg(0.15f, circleAngle);
                        circleAngle += directionModifier;
                    }
                    break;

                case 5: // Fast shot !
                    bulletAddRad(0.30f, 0 + randomDirection);
                    break;
            }*/
