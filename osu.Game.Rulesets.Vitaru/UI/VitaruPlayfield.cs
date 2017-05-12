// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics;
using osu.Game.Rulesets.Vitaru.Objects;
using osu.Game.Rulesets.Vitaru.Objects.Drawables;
using osu.Game.Rulesets.UI;
using OpenTK;
using osu.Game.Rulesets.Vitaru.Judgements;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects.Drawables;
using OpenTK.Graphics;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics.Sprites;

namespace osu.Game.Rulesets.Vitaru.UI
{
    public class VitaruPlayfield : Playfield<VitaruHitObject, VitaruJudgement>
    {
        public static Container vitaruPlayfield;
        private Container visablePlayfield;
        private Box cover;

        public override bool ProvidingUserCursor => true;

        public static readonly Vector2 BASE_SIZE = new Vector2(512, 820);

        public override Vector2 Size
        {
            get
            {
                var parentSize = Parent.DrawSize;
                var aspectSize = parentSize.X * 0.75f < parentSize.Y ? new Vector2(parentSize.X, parentSize.X * 0.75f) : new Vector2(parentSize.Y * 5f / 8f, parentSize.Y);

                return new Vector2(aspectSize.X / parentSize.X, aspectSize.Y / parentSize.Y) * base.Size;
            }
        }

        public VitaruPlayfield() : base(BASE_SIZE.X)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            Add(new Drawable[]
            {
                vitaruPlayfield = new Container
                {
                    Masking = false,
                    RelativeSizeAxes = Axes.Both,
                    Depth = 2,
                },
                visablePlayfield = new Container
                {
                    Masking = true,
                    Position = new Vector2(-10),
                    Origin = Anchor.TopLeft,
                    Anchor = Anchor.TopLeft,
                    Size = new Vector2(1.48f , 1.46f),
                    RelativeSizeAxes = Axes.Both,
                    Depth = 1,
                    BorderColour = Color4.Red,
                    BorderThickness = 10,
                    Children = new Drawable[]
                    {
                        cover = new Box
                        {
                            Colour = Color4.Black,
                            AlwaysPresent = true,
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0f,
                            Depth = 0,
                        }
                    }
                },
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
        }

        public override void Add(DrawableHitObject<VitaruHitObject, VitaruJudgement> h)
        {
            h.Depth = (float)h.HitObject.StartTime;

            IDrawableHitObjectWithProxiedApproach c = h as IDrawableHitObjectWithProxiedApproach;
            if (c != null)
                vitaruPlayfield.Add(c.ProxiedLayer.CreateProxy());

            base.Add(h);
        }
    }
}
