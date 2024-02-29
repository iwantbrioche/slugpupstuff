﻿using MoreSlugcats;
using System.Runtime.CompilerServices;

namespace SlugpupStuff
{
    public static class SlugpupCWTs
    {
        public static readonly ConditionalWeakTable<PlayerGraphics, PupData.PupGraphics> pupGraphicsCWT = new();

        public static readonly ConditionalWeakTable<SlugNPCAI, PupData.PupVariables> pupCWT = new();

        public static readonly ConditionalWeakTable<PlayerNPCState, PupData.PupNPCState> pupStateCWT = new();

        public static readonly ConditionalWeakTable<AbstractCreature, PupData.PupAbstract> pupAbstractCWT = new();

        public static bool isAquaticpup(this Player self)
        {
            bool name = self.slugcatStats.name == SlugpupStuff.VariantName.Aquaticpup;
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Aquaticpup;
            }
            return name || state;
        }
        public static bool isAquaticpup(this SlugNPCAI self)
        {
            bool name = self.cat.slugcatStats.name == SlugpupStuff.VariantName.Aquaticpup;
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Aquaticpup;
            }
            return name || state;
        }

        public static bool isTundrapup(this Player self)
        {
            bool name = self.slugcatStats.name == SlugpupStuff.VariantName.Tundrapup;
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Tundrapup;
            }
            return name || state;
        }
        public static bool isTundrapup(this SlugNPCAI self)
        {
            bool name = self.cat.slugcatStats.name == SlugpupStuff.VariantName.Tundrapup;
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Tundrapup;
            }
            return name || state;
        }

        public static bool isHunterpup(this Player self)
        {
            bool name = self.slugcatStats.name == SlugpupStuff.VariantName.Hunterpup;
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Hunterpup;
            }
            return name || state;
        }
        public static bool isHunterpup(this SlugNPCAI self)
        {
            bool name = self.cat.slugcatStats.name == SlugpupStuff.VariantName.Hunterpup;
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Hunterpup;
            }
            return name || state;
        }

        public static bool isRotundpup(this Player self)
        {
            bool name = self.slugcatStats.name == SlugpupStuff.VariantName.Rotundpup;
            bool state = false;
            if (self.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Rotundpup;
            }
            return name || state;
        }
        public static bool isRotundpup(this SlugNPCAI self)
        {
            bool name = self.cat.slugcatStats.name == SlugpupStuff.VariantName.Rotundpup;
            bool state = false;
            if (self.cat.playerState.TryGetPupState(out var pupNPCState))
            {
                state = pupNPCState.Variant == SlugpupStuff.VariantName.Rotundpup;
            }
            return name || state;
        }

        public static PupData.PupNPCState GetPupState(this PlayerNPCState self)
        {
            if (self != null)
            {
                return pupStateCWT.GetValue(self, _ => new PupData.PupNPCState());
            }
            return null;
        }
        public static bool TryGetPupState(this PlayerState self, out PupData.PupNPCState pupNPCState)
        {
            if (self != null && self is PlayerNPCState playerNPCState)
            {
                pupNPCState = playerNPCState.GetPupState();
            }
            else pupNPCState = null;

            return pupNPCState != null;
        }
        public static bool TryGetPupState(this PlayerNPCState self, out PupData.PupNPCState pupNPCState)
        {
            if (self != null)
            {
                pupNPCState = self.GetPupState();
            }
            else pupNPCState = null;

            return pupNPCState != null;
        }

        public static PupData.PupVariables GetPupVariables(this SlugNPCAI self)
        {
            if (self != null)
            {
                return pupCWT.GetValue(self, _ => new PupData.PupVariables());
            }
            return null;
        }
        public static bool TryGetPupVariables(this Player self, out PupData.PupVariables pupVariables)
        {
            if (self.AI != null)
            {
                pupVariables = self.AI.GetPupVariables();
            }
            else pupVariables = null;

            return pupVariables != null;
        }
        public static bool TryGetPupVariables(this SlugNPCAI self, out PupData.PupVariables pupVariables)
        {
            if (self != null)
            {
                pupVariables = self.GetPupVariables();
            }
            else pupVariables = null;

            return pupVariables != null;
        }

        public static PupData.PupGraphics GetPupGraphics(this PlayerGraphics self)
        {
            if (self != null)
            {
                return pupGraphicsCWT.GetValue(self, _ => new PupData.PupGraphics());
            }
            return null;
        }
        public static bool TryGetPupGraphics(this PlayerGraphics self, out PupData.PupGraphics pupGraphics)
        {
            if (self != null)
            {
                pupGraphics = self.GetPupGraphics();
            }
            else pupGraphics = null;

            return pupGraphics != null;
        }

        public static PupData.PupAbstract GetPupAbstract(this AbstractCreature self)
        {
            if (self != null)
            {
                return pupAbstractCWT.GetValue(self, _ => new PupData.PupAbstract());
            }
            return null;
        }
        public static bool TryGetPupAbstract(this AbstractCreature self, out PupData.PupAbstract pupAbstract)
        {
            if (self != null)
            {
                pupAbstract = self.GetPupAbstract();
            }
            else pupAbstract = null;

            return pupAbstract != null;
        }

        public class PupData
        {
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

}