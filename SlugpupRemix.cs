using BepInEx.Logging;
using Menu.Remix.MixedUI;

namespace SlugpupStuff
{
    internal class SlugpupRemix : OptionInterface
    {
        private readonly ManualLogSource logger;

        public SlugpupRemix(SlugpupStuff slugpupStuff, ManualLogSource logSource)
        {
            logger = logSource;

            SaintTundraGrapple = config.Bind("SlugpupStuff_SaintTundraGrapple", true, new ConfigurableInfo("Toggle using Tundrapup grapple as Saint"));
        }

        public readonly Configurable<bool> SaintTundraGrapple;

        public override void Initialize()
        {
            base.Initialize();

            var opTab = new OpTab(this, "Options");
            Tabs = [opTab];

            opTab.AddItems(
                new OpLabel(10f, 530f, "Toggles", true),
                new OpCheckBox(SaintTundraGrapple, new(10f, 500f)),
                new OpLabel(40f, 507.5f, "Toggle using Tundrapup grapple as Saint")
                );
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
