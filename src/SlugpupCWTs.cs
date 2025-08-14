using System.Runtime.CompilerServices;

namespace SlugpupStuff
{
    public static class SlugpupCWTs
    {
        public static readonly ConditionalWeakTable<PlayerGraphics, PupGraphics> pupGraphicsCWT = new();

        public static readonly ConditionalWeakTable<Player, ParentVariables> parentVariablesCWT = new();

        public static readonly ConditionalWeakTable<SlugNPCAI, PupVariables> pupCWT = new();

        public static readonly ConditionalWeakTable<PlayerNPCState, PupNPCState> pupStateCWT = new();


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
        public static bool isBoompup(this Player self)
        {
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Boompup;
            }
            return state || self.slugcatStats.name == SlugpupStuff.VariantName.Boompup;
        }
        public static bool isBoompup(this SlugNPCAI self)
        {
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Boompup;
            }
            return state || self.cat.slugcatStats.name == SlugpupStuff.VariantName.Boompup;
        }

        public static bool TryGetPupState(this PlayerState self, out PupNPCState pupNPCState)
        {
            if (self != null && self is PlayerNPCState playerNPCState)
            {
                pupNPCState = pupStateCWT.GetOrCreateValue(playerNPCState);
            }
            else pupNPCState = null;

            return pupNPCState != null;
        }
        public static bool TryGetPupState(this PlayerNPCState self, out PupNPCState pupNPCState)
        {
            if (self != null)
            {
                pupNPCState = pupStateCWT.GetOrCreateValue(self);
            }
            else pupNPCState = null;

            return pupNPCState != null;
        }
        public static bool TryGetParentVariables(this Player self, out ParentVariables parentVariables)
        {
            if (self != null)
            {
                parentVariables = parentVariablesCWT.GetOrCreateValue(self);
            }
            else parentVariables = null;

            return parentVariables != null;
        }
        public static bool TryGetPupVariables(this Player self, out PupVariables pupVariables)
        {
            if (self.AI != null)
            {
                pupVariables = pupCWT.GetOrCreateValue(self.AI);
            }
            else pupVariables = null;

            return pupVariables != null;
        }
        public static bool TryGetPupVariables(this SlugNPCAI self, out PupVariables pupVariables)
        {
            if (self != null)
            {
                pupVariables = pupCWT.GetOrCreateValue(self);
            }
            else pupVariables = null;

            return pupVariables != null;
        }

        public static bool TryGetPupGraphics(this PlayerGraphics self, out PupGraphics pupGraphics)
        {
            if (self != null)
            {
                pupGraphics = pupGraphicsCWT.GetOrCreateValue(self);
            }
            else pupGraphics = null;

            return pupGraphics != null;
        }


        public class ParentVariables
        {
            public bool rotundPupExhaustion;
        }
        public class PupVariables
        {
            public bool regurgitating;
            public bool swallowing;
            public bool wantsToRegurgitate;
            public bool wantsToSwallowObject;

            public AbstractPhysicalObject giftedItem;

            public SlugpupDebugger.DebugLabelManager labelManager;
            public SlugpupDebugger.PathingVisualizer pathingVisualizer;
            public DebugDestinationVisualizer destinationVisualizer;

        }
        public class PupGraphics
        {
            public int TongueSpriteIndex;
            public int sLeaserLength;
        }

        public class PupNPCState // DONT CHANGE THIS FFS, BEASTMASTERPUPEXTRAS RELIES ON IT!!!!!
        {
            public SlugcatStats.Name Variant;
            public AbstractPhysicalObject PupsPlusStomachObject;
        }
    }

}