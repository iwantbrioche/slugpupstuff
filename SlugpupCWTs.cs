using MoreSlugcats;
using System.Runtime.CompilerServices;

namespace SlugpupStuff
{
    public static class SlugpupCWTs
    {
        public static readonly ConditionalWeakTable<PlayerGraphics, PupGraphics> pupGraphicsCWT = new();

        public static readonly ConditionalWeakTable<SlugNPCAI, PupVariables> pupCWT = new();
        public class PupVariables
        {
            public int wantCounter = 0;
            public int pointCounter = 0;
            public int grabbyCounter = 0;
            public int giftCounter = 0;
            public PhysicalObject wantedObject = null;
            public PhysicalObject giftedObject = null;

            public SlugpupStuff.SlugpupDebugViz debugViz;
        }

        public class PupGraphics
        {
            public int TongueSpriteIndex = 0;
        }

    }

}










