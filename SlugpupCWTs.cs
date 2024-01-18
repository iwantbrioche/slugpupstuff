using MoreSlugcats;
using System.Runtime.CompilerServices;

namespace SlugpupStuff
{
    public static class SlugpupCWTs
    {
        public static readonly ConditionalWeakTable<PlayerGraphics, PupGraphics> pupGraphicsCWT = new();

        public static readonly ConditionalWeakTable<SlugNPCAI, PupVariables> pupCWT = new();

        public static readonly ConditionalWeakTable<PlayerNPCState, PupNPCState> pupStateCWT = new();

        public static readonly ConditionalWeakTable<AbstractCreature, PupAbstract> pupAbstractCWT = new();
        public class PupVariables
        {
            public bool regurgitating;
            public bool swallowing;
            public bool wantsToRegurgitate;
            public bool wantsToSwallowObject;
        }
        public class PupGraphics
        {
            public int TongueSpriteIndex;
        }

        public class PupNPCState
        {
            public SlugcatStats.Name Variant;
            public AbstractPhysicalObject PupsPlusStomachObject;
        }

        public class PupAbstract
        {
            public bool aquatic = false;
            public bool tundra = false;
            public bool hunter = false;
            public bool rotund = false;
            public bool regular = false;

        }
    }

}