﻿using OpenTK.Graphics;
using OpenTK;
using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Vitaru.Objects.Projectiles;
using osu.Game.Rulesets.Vitaru.Objects.Characters;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Framework.MathUtils;
using System.Collections.Generic;
using osu.Game.Rulesets.Vitaru.UI;

namespace osu.Game.Rulesets.Vitaru.Objects.Drawables
{
    public class DrawableVitaruEnemy : DrawableCharacter
    {
        private readonly Enemy enemy;
        public bool Shoot = false;
        private float playerPos;
        private Color4 enemyColor = Color4.Green;

        private readonly List<ISliderProgress> components = new List<ISliderProgress>();
        private int currentRepeat;

        public DrawableVitaruEnemy(Enemy enemy) : base(enemy)
        {
            this.enemy = enemy;
            AlwaysPresent = true;
            Origin = Anchor.Centre;
            Position = enemy.Position;
            CharacterType = HitObjectType.Enemy;
            CharacterHealth = 20;
            Team = 1;
            HitboxWidth = 24;
            HitboxColor = Color4.Cyan;
            Alpha = 1;
        }

        private bool hasShot = false;
        private bool sliderDone = false;

        protected override void Update()
        {
            enemy.EnemyPosition = enemy.Position;
            int bulletPattern = RNG.Next(1, 6); // could be remplaced by map seed, with stackleniency

            HitDetect();

            if (!enemy.IsSlider && !enemy.IsSpinner)
                hitcircle();

            if (enemy.IsSlider)
                slider();

            if (enemy.IsSpinner)
                spinner();
                
        }

        /// <summary>
        /// Generic Enemy stuff
        /// </summary>
        protected override void CheckJudgement(bool userTriggered)
        {
            if (CharacterHealth < 1)
            {
                Judgement.Result = HitResult.Hit;
            }
        }

        protected override void UpdateInitialState()
        {
            base.UpdateInitialState();
            Alpha = 0f;
            Scale = new Vector2(0.5f);
        }

        protected override void UpdatePreemptState()
        {
            base.UpdatePreemptState();

            FadeIn(Math.Min(TIME_FADEIN * 2, TIME_PREEMPT), EasingTypes.OutQuart);
            ScaleTo(1f, TIME_PREEMPT, EasingTypes.OutQuart);
        }

        protected override void UpdateState(ArmedState state)
        {
            base.UpdateState(state);

            double endTime = (HitObject as IHasEndTime)?.EndTime ?? HitObject.StartTime;
            double duration = endTime - HitObject.StartTime;



            Delay(HitObject.StartTime - Time.Current + Judgement.TimeOffset, true);

            switch (State)
            {
                case ArmedState.Idle:
                    Delay(duration + TIME_PREEMPT);
                    FadeOut(TIME_FADEOUT * 2);
                    Expire(true);
                    break;
                case ArmedState.Hit:
                    FadeOut(TIME_FADEOUT / 4);
                    Expire();
                    break;
            }

            Expire();
        }

        private void leave()
        {
            int r = RNG.Next(-100, 612);
            MoveTo(new Vector2(r, -300), 2000, EasingTypes.InCubic);
            FadeOut(2000, EasingTypes.InCubic);
            ScaleTo(new Vector2(0.75f), 2000, EasingTypes.InCubic);
        }

        /// <summary>
        /// All the hitcircle stuff
        /// </summary>
        private void hitcircle()
        {
            if (HitObject.StartTime < Time.Current && hasShot == false)
            {
                enemyShoot();
                leave();
                hasShot = true;
            }
            if (HitObject.StartTime < Time.Current && hasShot == true && Position.Y <= -300)
            {
                Dispose();
            }
        }

        /// <summary>
        /// All The Slider Stuff
        /// </summary>
        private void slider()
        {
            if (HitObject.StartTime < Time.Current && hasShot == false)
            {
                enemyShoot();
                hasShot = true;
            }

            if (enemy.EndTime < Time.Current && hasShot == true && sliderDone == false)
            {
                enemyShoot();
                leave();
                sliderDone = true;
            }

            if (enemy.EndTime < Time.Current && hasShot == true && Position.Y <= -300)
            {
                Dispose();
            }

            double progress = MathHelper.Clamp((Time.Current - enemy.StartTime) / enemy.Duration, 0, 1);

            int repeat = enemy.RepeatAt(progress);
            progress = enemy.ProgressAt(progress);

            if (repeat > currentRepeat)
            {
                if (repeat < enemy.RepeatCount)
                {
                    enemyShoot();
                }
                currentRepeat = repeat;
            }
            if(!sliderDone)
                UpdateProgress(progress, repeat);
        }

        public void UpdateProgress(double progress, int repeat)
        {
            if(!sliderDone)
                Position = enemy.Curve.PositionAt(progress);
        }

        internal interface ISliderProgress
        {
            void UpdateProgress(double progress, int repeat);
        }

        /// <summary>
        /// all the spinner stuff
        /// </summary>
        private void spinner()
        {

        }

        /// <summary>
        /// All the shooting stuff
        /// </summary>

        private void enemyShoot()
        {
            int pattern = RNG.Next(1, 6);
            playerRelativePositionAngle();
            PlaySamples();
            switch (pattern)
            {
                case 1: // Wave
                    Wave w;
                    VitaruPlayfield.vitaruPlayfield.Add(w = new Wave(Team)
                    {
                        Origin = Anchor.Centre,
                        Depth = 6,
                        PatternColor = Color4.Green,
                        PatternAngleRadian = playerPos,
                        PatternSpeed = 0.2f,
                        PatternBulletWidth = 8,
                        PatternComplexity = 2f,
                    });
                    w.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), w));
                    break;

                case 2: // Line
                    Line l;
                    VitaruPlayfield.vitaruPlayfield.Add(l = new Line(Team)
                    {
                        Origin = Anchor.Centre,
                        Depth = 6,
                        PatternColor = Color4.Green,
                        PatternAngleRadian = playerPos,
                        PatternSpeed = 0.2f,
                        PatternBulletWidth = 8,
                        PatternComplexity = 2f,
                    });
                    l.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), l));
                    break;

                case 3: // Cool wave
                    CoolWave cw;
                    VitaruPlayfield.vitaruPlayfield.Add(cw = new CoolWave(Team)
                    {
                        Origin = Anchor.Centre,
                        Depth = 6,
                        PatternColor = Color4.Green,
                        PatternAngleRadian = playerPos,
                        PatternSpeed = 0.3f,
                        PatternBulletWidth = 8,
                        PatternComplexity = 4f,
                    });
                    cw.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), cw));
                    break;

                case 4: // Circle
                    Circle c;
                    VitaruPlayfield.vitaruPlayfield.Add(c = new Circle(Team)
                    {
                        Origin = Anchor.Centre,
                        Depth = 6,
                        PatternColor = Color4.Cyan,
                        PatternAngleRadian = playerPos,
                        PatternSpeed = 0.2f,
                        PatternBulletWidth = 10,
                        PatternComplexity = 2f,
                    });
                    c.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), c));
                    break;

                case 5: // Snipe!
                    Wave f;
                    VitaruPlayfield.vitaruPlayfield.Add(f = new Wave(Team)
                    {
                        Origin = Anchor.Centre,
                        Depth = 6,
                        PatternColor = Color4.Green,
                        PatternAngleRadian = playerPos,
                        PatternSpeed = 0.5f,
                        PatternBulletWidth = 8,
                        PatternComplexity = 0.4f,
                    });
                    f.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), f));
                    break;
            }
        }

        public float playerRelativePositionAngle()
        {
            //Returns a Radian
            playerPos = (float)Math.Atan2((VitaruPlayer.PlayerPosition.Y - Position.Y),(VitaruPlayer.PlayerPosition.X - Position.X));
            return playerPos;
        }
    }
}