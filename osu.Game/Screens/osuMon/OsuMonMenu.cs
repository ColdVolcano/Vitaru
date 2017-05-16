// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Backgrounds;
using osu.Game.Screens.Tournament.Components;
using osu.Game.Screens.Tournament.Teams;
using OpenTK;
using OpenTK.Graphics;
using osu.Framework.IO.Stores;
using osu.Game.Screens.Tournament;
using osu.Game.Screens.Menu;

namespace osu.Game.Screens.osuMon
{
    public class OsuMonMenu : OsuScreen
    {
        private const string results_filename = "drawings_results.txt";

        internal override bool ShowOverlays => false;

        protected override BackgroundScreen CreateBackground() => new BackgroundScreenDefault();

        private readonly List<DrawingsTeam> allTeams = new List<DrawingsTeam>();

        private DrawingsConfigManager drawingsConfig;

        private Task writeOp;

        private Storage storage;
        private MenuVisualisation vis;

        protected override DependencyContainer CreateLocalDependencies(DependencyContainer parent) => new DependencyContainer(parent);

        [BackgroundDependencyLoader]
        private void load(TextureStore textures, Storage storage, DependencyContainer dependencies)
        {
            this.storage = storage;

            TextureStore flagStore = new TextureStore();
            // Local flag store
            flagStore.AddStore(new RawTextureLoaderStore(new NamespacedResourceStore<byte[]>(new StorageBackedResourceStore(storage), "Drawings")));
            // Default texture store
            flagStore.AddStore(textures);

            dependencies.Cache(flagStore);

            drawingsConfig = new DrawingsConfigManager(storage);

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(77, 77, 77, 255)
                },
                new Sprite
                {
                    FillMode = FillMode.Fill,
                    Texture = textures.Get(@"Backgrounds/Drawings/background.png")
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,

                    Children = new Drawable[]
                    {
                        // Main container
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.85f,

                            Children = new Drawable[]
                            {
                                // Visualiser
                                new VisualiserContainer
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,

                                    RelativeSizeAxes = Axes.X,
                                    Size = new Vector2(1, 10),

                                    Colour = new Color4(255, 204, 34, 255),

                                    Lines = 6
                                },
                            }
                        },
                        // Control panel container
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.15f,

                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = new Color4(54, 54, 54, 255)
                                },
                                vis = new MenuVisualisation
                                {
                                    Scale = Vector2.Zero,
                                    Position = new Vector2(0)
                                },
                                new OsuSpriteText
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,

                                    Text = "Welcome to OsuMon!",
                                    TextSize = 22f,
                                    Font = "Exo2.0-Bold"
                                },
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,

                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Width = 0.75f,

                                    Position = new Vector2(0, 35f),

                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 5f),

                                    Children = new Drawable[]
                                    {
                                        new OsuButton
                                        {
                                            RelativeSizeAxes = Axes.X,

                                            Text = "Find Match",
                                            //Action = openLink.OsuMonFind,
                                        },
                                        new OsuButton
                                        {
                                            RelativeSizeAxes = Axes.X,

                                            Text = "Start Match",
                                            //Action = openLink.OsuMonStartMatch,
                                        },
                                        new OsuButton
                                        {
                                            RelativeSizeAxes = Axes.X,

                                            Text = "Open Discord",
                                            //Action = openLink.OsuMonDiscord,
                                        },
                                    }
                                },
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.BottomCentre,

                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Width = 0.75f,

                                    Position = new Vector2(0, -5f),

                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 5f),

                                    Children = new Drawable[]
                                    {
                                        new OsuButton
                                        {
                                            RelativeSizeAxes = Axes.X,

                                            Text = "Join OsuMon!",
                                            //Action = opnlink.OsuMonJoin,
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            reset(true);
        }

        private void writeResults(string text)
        {
            Action writeAction = () =>
            {
                try
                {
                    // Write to drawings_results
                    using (Stream stream = storage.GetStream(results_filename, FileAccess.Write, FileMode.Create))
                    using (StreamWriter sw = new StreamWriter(stream))
                    {
                        sw.Write(text);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to write results.");
                }
            };

            writeOp = writeOp?.ContinueWith(t => { writeAction(); }) ?? Task.Run(writeAction);
        }

        private void reset(bool loadLastResults = false)
        {
            if (!storage.Exists(results_filename))
                return;

            if (loadLastResults)
            {
                try
                {
                    // Read from drawings_results
                    using (Stream stream = storage.GetStream(results_filename, FileAccess.Read, FileMode.Open))
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string line;
                        while ((line = sr.ReadLine()?.Trim()) != null)
                        {
                            if (string.IsNullOrEmpty(line))
                                continue;

                            if (line.ToUpper().StartsWith("GROUP"))
                                continue;

                            DrawingsTeam teamToAdd = allTeams.FirstOrDefault(t => t.FullName == line);

                            if (teamToAdd == null)
                                continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to read last drawings results.");
                }

            }
            else
            {
                writeResults(string.Empty);
            }
        }
    }
}
