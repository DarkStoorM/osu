// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Difficulty.Skills;
using osuTK;

namespace osu.Game.Rulesets.Typing.Tests.Difficulty.Components
{
    public partial class GraphContainer : Container
    {
        private Vector2 lastSize;
        private readonly double[] strains;

        public GraphContainer(StrainSkill skill)
        {
            strains = skill.GetCurrentStrainPeaks().ToArray();

            RelativeSizeAxes = Axes.Both;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            buildGraph();
        }

        protected override void Update()
        {
            base.Update();

            if (lastSize == DrawSize)
                return;

            buildGraph();
        }

        private void buildGraph()
        {
            if (strains.Length < 2)
                return;

            float width = DrawWidth;
            float height = DrawHeight;

            lastSize = new Vector2(DrawWidth, DrawHeight);

            float max = Math.Max((float)strains.Max(), 1f);

            var grid = new Container
            {
                RelativeSizeAxes = Axes.Both,
            };

            for (int i = 0; i <= 5; i++)
            {
                float t = i / 5f;
                float y = height - t * height;

                grid.Add(new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    Y = y,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.White,
                            Alpha = 0.1f,
                        },
                        new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            X = 5,
                            Y = 0,
                            Text = (t * max).ToString("0.00"),
                            Font = OsuFont.Torus.With(size: 12),
                            Alpha = 0.8f
                        }
                    }
                });
            }

            var path = new Path
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                PathRadius = 1,
                Colour = Colour4.White,
            };

            float stepX = width / (strains.Length - 1);

            for (int i = 0; i < strains.Length; i++)
            {
                float x = i * stepX;

                float normalized = (float)(strains[i] / max);
                float y = height - normalized * height;

                path.AddVertex(new Vector2(x, y));
            }

            InternalChildren = new Drawable[]
            {
                grid,
                path
            };
        }
    }
}
