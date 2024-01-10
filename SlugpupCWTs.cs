using MoreSlugcats;
using System.Runtime.CompilerServices;

namespace SlugpupStuff
{
    public static class SlugpupCWTs
    {
        public static readonly ConditionalWeakTable<PlayerGraphics, PupGraphics> pupGraphicsCWT = new();

        public static readonly ConditionalWeakTable<SlugNPCAI, PupVariables> pupCWT = new();

        public static readonly ConditionalWeakTable<PlayerNPCState, PupNPCState> pupStateCWT = new();
        public class PupVariables
        {
        }

        public class PupGraphics
        {
            public int TongueSpriteIndex = 0;
        }

        public class PupNPCState
        {
            public SlugcatStats.Name Variant = null;
        }
    }

}