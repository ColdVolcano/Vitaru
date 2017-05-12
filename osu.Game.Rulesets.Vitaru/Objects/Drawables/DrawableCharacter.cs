using OpenTK;
using OpenTK.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using System;
using osu.Game.Rulesets.Vitaru.Objects.Projectiles;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Game.Rulesets.Vitaru.UI;

namespace osu.Game.Rulesets.Vitaru.Objects.Drawables
{
    public abstract class DrawableCharacter : DrawableVitaruHitObject
    {
        public HitObjectType CharacterType;

        protected Sprite CharacterSprite;
        protected Sprite CharacterKiaiSprite;
        protected SpellPiece Sign;

        public Vector2 Speed { get; set; } = Vector2.Zero;
        public float CharacterHealth { get; set; } = 100;
        public float Armor { get; internal set; } = 1; //All damage taken should be divided by this number. During kiai player will only take half damage so [2]
        public int Team { get; set; } = 0; // 0 = Player, 1 = Ememies + Boss(s) in Singleplayer
        public int ProjectileDamage { get; set; }
        public int BPM { get; set; } = (200);
        private SampleChannel sampleShoot;
        private SampleChannel sampleDeath;

        protected Hitbox Hitbox;

        public bool Shooting { get; set; } = false;

        private double timeSinceLastShoot;

        protected Color4 HitboxColor { get; set; }
        protected float HitboxWidth { get; set; }

        public Action OnDeath { get; set; }
        public Action CharacterShoot { get; set; }
        public bool Kiai { get; private set; }

        public DrawableCharacter(VitaruHitObject hitObject) : base(hitObject)
        {
        }
        
        /// <summary>
        /// The <see cref="Character"/> gets damaged, with a multiplier of <see cref="DamageMultiplier"/>
        /// </summary>
        /// <param name="damage">Damage without the Resistance applied</param>
        /// <returns>If the Character died</returns>
        public bool TakeDamage(int damage)
        {
            CharacterHealth -= (int)(damage * Armor);
            if (CharacterHealth <= 0)
            {
                Dispose();
                sampleDeath.Play();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Heals the <see cref="Character"/> by the specified amount
        /// </summary>
        /// <param name="healing">Amount of health to be healed</param>
        public void Heal(int healing)
        {
            CharacterHealth = 100 >= healing + CharacterHealth ? 100 : CharacterHealth + healing;
        }

        protected override void Update()
        {
            /*
            if(Kiai == true)
            {
                CharacterSprite.FadeOut();
                CharacterKiaiSprite.FadeIn();
            }
            if (Kiai == false)
            {
                CharacterSprite.FadeIn();
                CharacterKiaiSprite.FadeOut();
            }*/

            base.Update();

            HitDetect();

            if (Shooting)
            {
                timeSinceLastShoot += Clock.ElapsedFrameTime;
                if (timeSinceLastShoot / 1000.0 > 1 / BPM / 30.0)
                {
                    sampleShoot.Play();
                    CharacterShoot?.Invoke();
                    timeSinceLastShoot -= 1 / (BPM / 30.0) * 1000.0;
                }
            }
        }

        public void HitDetect()
        {
            if (VitaruPlayfield.vitaruPlayfield?.Children != null)
                foreach (Drawable draw in VitaruPlayfield.vitaruPlayfield.Children)
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
            if (VitaruPlayfield.vitaruPlayfield != null)
                foreach (Drawable draw in VitaruPlayfield.vitaruPlayfield.Children)
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
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio, TextureStore textures)
        {
            Anchor = Anchor.TopLeft;
            Origin = Anchor.Centre;
            Children = new Drawable[]
            {
                CharacterSprite = new Sprite()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 1,
                },
                CharacterKiaiSprite = new Sprite()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0,
                },
                Hitbox = new Hitbox()
                {
                    Alpha = 0,
                    HitboxWidth = HitboxWidth,
                    HitboxColor = HitboxColor,
                },
                Sign = new SpellPiece()
                {
                    Alpha = 0,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                }
            };

            string characterType = "null";
            switch(CharacterType)
            {
                case HitObjectType.Player:
                    characterType = "player";
                    break;
                case HitObjectType.Enemy:
                    characterType = "enemy";
                    break;
                case HitObjectType.Boss:
                    characterType = "boss";
                    break;
            }

            sampleDeath = audio.Sample.Get(@"Vitaru/deathSound");
            sampleShoot = audio.Sample.Get(@"Vitaru/shootSound");
            CharacterSprite.Texture = textures.Get(@"Play/Vitaru/" + characterType);
            CharacterKiaiSprite.Texture = textures.Get(@"Play/Vitaru/" + characterType + "Kiai");
        }
    }
}