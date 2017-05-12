using OpenTK.Graphics;
using OpenTK;
using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Vitaru.Objects.Projectiles;
using osu.Game.Rulesets.Vitaru.Objects.Drawables;
using osu.Game.Rulesets.Vitaru.Objects;
using osu.Game.Rulesets.Vitaru.Objects.Characters;
using osu.Framework.Audio.Sample;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Vitaru.Judgements;
using osu.Framework.MathUtils;
using System.Collections.Generic;
using osu.Game.Rulesets.Vitaru.UI;
using osu.Game.Rulesets.Vitaru.Beatmaps;

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

        /// <summary>
        /// All the hitcircle stuff
        /// </summary>
        private void hitcircle()
        {
            if (HitObject.StartTime < Time.Current && hasShot == false)
            {
                enemyShoot();
                FadeOut(Math.Min(TIME_FADEOUT * 2, TIME_PREEMPT));
                hasShot = true;
            }
            if (HitObject.StartTime < Time.Current && hasShot == true && Alpha < 0.1f)
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
                FadeOut(Math.Min(TIME_FADEOUT * 2, TIME_PREEMPT));
                sliderDone = true;
            }

            if (enemy.EndTime < Time.Current && hasShot == true && Alpha < 0.1f)
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
            UpdateProgress(progress, repeat);
        }

        public void UpdateProgress(double progress, int repeat)
        {
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
            playerRelativePositionAngle();
            PlaySamples();
            Wave w;
            VitaruPlayfield.vitaruPlayfield.Add(w = new Wave(1)
            {
                Origin = Anchor.Centre,
                Depth = 6,
                PatternColor = Color4.Green,
                PatternAngleRadian = playerPos,
                PatternSpeed = 0.2f,
                PatternBulletWidth = 8,
                PatternComplexity = 1,
            });
            w.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), w));
        }

        public float playerRelativePositionAngle()
        {
            //Returns a Radian
            playerPos = (float)Math.Atan2((VitaruPlayer.PlayerPosition.Y - Position.Y),(VitaruPlayer.PlayerPosition.X - Position.X));
            return playerPos;
        }
    }
}