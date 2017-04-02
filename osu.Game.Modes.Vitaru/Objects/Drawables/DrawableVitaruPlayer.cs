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

namespace osu.Game.Modes.Vitaru.Objects.Drawables
{
    public class DrawableVitaruPlayer : DrawableVitaruCharacter
    {
        private Dictionary<Key, bool> keys = new Dictionary<Key, bool>();

        public static Vector2 PlayerPosition = new Vector2(0, 160);

        public DrawableVitaruPlayer(VitaruHitObject hitObject) : base(hitObject)
        {
            keys[Key.Up] = false;
            keys[Key.Right] = false;
            keys[Key.Down] = false;
            keys[Key.Left] = false;
            keys[Key.Z] = false;
            keys[Key.X] = false;
            keys[Key.LShift] = false;
            keys[Key.RShift] = false;
            Origin = Anchor.Centre;
            Position = PlayerPosition;
            CharacterType = HitObjectType.Player;
            CharacterHealth = 100;
            Team = 0;
            HitboxColor = Color4.Cyan;
            HitboxWidth = 8;

        }

        private const float playerSpeed = 0.5f;
        private Vector2 positionChange = Vector2.Zero;
        private bool isHalfSpeed = false;

        protected override void Update()
        {
            base.Update();

            //Handles Player Speed
            var pos = Position;
            float ySpeed = 0.5f * (float)(Clock.ElapsedFrameTime);
            float xSpeed = 0.5f * (float)(Clock.ElapsedFrameTime);

            //All these handle keys and when they are or aren't pressed
            if (keys[Key.LShift] | keys[Key.RShift])
            {
                xSpeed /= 2;
                ySpeed /= 2;
            }
            if (keys[Key.Z])
            {
                Shooting = true;
            }
            if (keys[Key.Z] == false)
            {
                Shooting = false;
            }
            if (keys[Key.X])
            {
                //Bomb();
            }
            if (keys[Key.Up])
            {
                pos.Y -= ySpeed;
            }
            if (keys[Key.Left])
            {
                pos.X -= xSpeed;
            }
            if (keys[Key.Down])
            {
                pos.Y += ySpeed;
            }
            if (keys[Key.Right])
            {
                pos.X += xSpeed;
            }

            Position = pos;
            PlayerPosition = pos;
        }

        private void shoot()
        {
            Bullet bullet;
            MainParent.Add(bullet = new Bullet(Team)
            {
                Depth = 1,
                Anchor = Anchor.Centre,
                BulletAngleDegree = 0f,
                BulletSpeed = 1f,
                BulletColor = Color4.Green,
            });
            bullet.MoveTo(ToSpaceOfOtherDrawable(new Vector2(0, 0), bullet));
        }

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            keys[args.Key] = true;
            if (args.Key == Key.LShift || args.Key == Key.RShift)
                Hitbox.Alpha = 1;
            return base.OnKeyDown(state, args);
        }
        protected override bool OnKeyUp(InputState state, KeyUpEventArgs args)
        {
            keys[args.Key] = false;
            if (args.Key == Key.LShift || args.Key == Key.RShift)
                Hitbox.Alpha = 0;
            return base.OnKeyUp(state, args);
        }
    }
}
