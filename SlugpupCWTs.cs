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

        public static bool isAquaticpup(this Player self)
        {
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Aquaticpup;
            }
            return state || self.slugcatStats.name == SlugpupStuff.VariantName.Aquaticpup;
        }
        public static bool isAquaticpup(this SlugNPCAI self)
        {
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Aquaticpup;
            }
            return state || self.cat.slugcatStats.name == SlugpupStuff.VariantName.Aquaticpup;
        }

        public static bool isTundrapup(this Player self)
        {
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Tundrapup;
            }
            return state || self.slugcatStats.name == SlugpupStuff.VariantName.Tundrapup;
        }
        public static bool isTundrapup(this SlugNPCAI self)
        {
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Tundrapup;
            }
            return state || self.cat.slugcatStats.name == SlugpupStuff.VariantName.Tundrapup;
        }

        public static bool isHunterpup(this Player self)
        {
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Hunterpup;
            }
            return state || self.slugcatStats.name == SlugpupStuff.VariantName.Hunterpup;
        }
        public static bool isHunterpup(this SlugNPCAI self)
        {
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Hunterpup;
            }
            return state || self.cat.slugcatStats.name == SlugpupStuff.VariantName.Hunterpup;
        }

        public static bool isRotundpup(this Player self)
        {
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Rotundpup;
            }
            return state || self.slugcatStats.name == SlugpupStuff.VariantName.Rotundpup;
        }
        public static bool isRotundpup(this SlugNPCAI self)
        {
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Rotundpup;
            }
            return state || self.cat.slugcatStats.name == SlugpupStuff.VariantName.Rotundpup;
        }

        public static PupNPCState GetPupState(this PlayerNPCState self)
        {
            if (self != null)
            {
                return pupStateCWT.GetValue(self, _ => new PupNPCState());
            }
            return null;
        }
        public static bool TryGetPupState(this PlayerState self, out PupNPCState pupNPCState)
        {
            if (self != null && self is PlayerNPCState playerNPCState)
            {
                pupNPCState = playerNPCState.GetPupState();
            }
            else pupNPCState = null;

            return pupNPCState != null;
        }
        public static bool TryGetPupState(this PlayerNPCState self, out PupNPCState pupNPCState)
        {
            if (self != null)
            {
                pupNPCState = self.GetPupState();
            }
            else pupNPCState = null;

            return pupNPCState != null;
        }

        public static PupVariables GetPupVariables(this SlugNPCAI self)
        {
            if (self != null)
            {
                return pupCWT.GetValue(self, _ => new PupVariables());
            }
            return null;
        }
        public static bool TryGetPupVariables(this Player self, out PupVariables pupVariables)
        {
            if (self.AI != null)
            {
                pupVariables = self.AI.GetPupVariables();
            }
            else pupVariables = null;

            return pupVariables != null;
        }
        public static bool TryGetPupVariables(this SlugNPCAI self, out PupVariables pupVariables)
        {
            if (self != null)
            {
                pupVariables = self.GetPupVariables();
            }
            else pupVariables = null;

            return pupVariables != null;
        }

        public static PupGraphics GetPupGraphics(this PlayerGraphics self)
        {
            if (self != null)
            {
                return pupGraphicsCWT.GetValue(self, _ => new PupGraphics());
            }
            return null;
        }
        public static bool TryGetPupGraphics(this PlayerGraphics self, out PupGraphics pupGraphics)
        {
            if (self != null)
            {
                pupGraphics = self.GetPupGraphics();
            }
            else pupGraphics = null;

            return pupGraphics != null;
        }

        public static PupAbstract GetPupAbstract(this AbstractCreature self)
        {
            if (self != null)
            {
                return pupAbstractCWT.GetValue(self, _ => new PupAbstract());
            }
            return null;
        }
        public static bool TryGetPupAbstract(this AbstractCreature self, out PupAbstract pupAbstract)
        {
            if (self != null)
            {
                pupAbstract = self.GetPupAbstract();
            }
            else pupAbstract = null;

            return pupAbstract != null;
        }

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
            public bool aquatic;
            public bool tundra;
            public bool hunter;
            public bool rotund;
            public bool regular;
        }
    }

}