﻿// Copyright (c) 2007-2018 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using OpenTK.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using System;

namespace osu.Game.Overlays.Changelog
{
    public class ChangelogContent : FillFlowContainer
    {
        private APIAccess api;
        private ChangelogContentGroup changelogContentGroup;

        public delegate void BuildSelectedEventHandler(APIChangelogBuild build, EventArgs args);

        public event BuildSelectedEventHandler BuildSelected;

        public ChangelogContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Padding = new MarginPadding{ Bottom = 100 };
        }

        public void ShowListing(APIChangelogBuild[] changelog)
        {
            DateTime currentDate = new DateTime();
            Clear();

            foreach (APIChangelogBuild build in changelog)
            {
                if (build.CreatedAt.Date != currentDate)
                {
                    if (Children.Count != 0)
                    {
                        Add(new Box
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 2,
                            Colour = new Color4(17, 17, 17, 255),
                            Margin = new MarginPadding { Top = 30 },
                        });
                    }
                    changelogContentGroup = new ChangelogContentGroup(build, true);
                    changelogContentGroup.BuildSelected += OnBuildSelected;
                    changelogContentGroup.GenerateText(build.ChangelogEntries);
                    Add(changelogContentGroup);
                    currentDate = build.CreatedAt.Date;
                }
                else
                {
                    changelogContentGroup.Add(new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 1,
                        Colour = new Color4(32, 24, 35, 255),
                        Margin = new MarginPadding { Top = 30 },
                    });

                    changelogContentGroup = new ChangelogContentGroup(build, false);
                    changelogContentGroup.BuildSelected += OnBuildSelected;
                    changelogContentGroup.GenerateText(build.ChangelogEntries);
                    Add(changelogContentGroup);
                }
            }
        }

        public void ShowBuild(APIChangelogBuild changelogBuild)
        {
            Child = changelogContentGroup = new ChangelogContentGroup(changelogBuild);
            changelogContentGroup.GenerateText(changelogBuild.ChangelogEntries);
            changelogContentGroup.UpdateChevronTooltips(changelogBuild.Versions.Previous?.DisplayVersion,
                changelogBuild.Versions.Next?.DisplayVersion);
            changelogContentGroup.BuildSelected += OnBuildSelected;
        }

        protected virtual void OnBuildSelected(APIChangelogBuild build, EventArgs args)
        {
            BuildSelected?.Invoke(build, EventArgs.Empty);
        }
    }
}
