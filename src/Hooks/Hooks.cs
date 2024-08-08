
namespace SlugpupStuff.Hooks
{
    public static class Hooks
    {
        public static void PatchAllHooks()
        {
            MiscHooks.Patch();
            SlugpupHooks.Patch();
            PlayerHooks.Patch();
            PlayerGraphicsHooks.Patch();
            PlayerNPCStateHooks.Patch();
            SlugcatStatsHooks.Patch();

        }
    }
}
