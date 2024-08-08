
namespace SlugpupStuff.Hooks
{
    public static class SlugcatStatsHooks
    {
        public static void Patch()
        {
            On.SlugcatStats.ctor += SlugcatStats_ctor;
            On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
            On.SlugcatStats.HiddenOrUnplayableSlugcat += SlugcatStats_HiddenOrUnplayableSlugcat;

            IL.SlugcatStats.NourishmentOfObjectEaten += IL_SlugcatStats_NourishmentOfObjectEaten;
        }

        private static void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
        {
            orig(self, slugcat, malnourished);
            if (slugcat == VariantName.Aquaticpup)
            {
                self.bodyWeightFac = 0.65f;
                self.generalVisibilityBonus = -0.2f;
                self.visualStealthInSneakMode = 0.6f;
                self.loudnessFac = 0.5f;
                self.lungsFac = 0.2f;
                self.throwingSkill = 0;
                self.poleClimbSpeedFac = 1.4f;
                self.corridorClimbSpeedFac = 1.35f;
                self.runspeedFac = 1.35f;
                if (malnourished)
                {
                    self.runspeedFac = 1f;
                    self.poleClimbSpeedFac = 0.9f;
                    self.corridorClimbSpeedFac = 0.9f;
                }
            }
            else if (slugcat == VariantName.Tundrapup)
            {
                self.bodyWeightFac = 0.65f;
                self.generalVisibilityBonus = -0.2f;
                self.visualStealthInSneakMode = 0.6f;
                self.loudnessFac = 0.5f;
                self.lungsFac = 0.8f;
                self.throwingSkill = 0;
                self.poleClimbSpeedFac = 0.8f;
                self.corridorClimbSpeedFac = 0.8f;
                self.runspeedFac = 0.8f;
                if (malnourished)
                {
                    self.runspeedFac = 0.6f;
                    self.poleClimbSpeedFac = 0.6f;
                    self.corridorClimbSpeedFac = 0.6f;
                }
            }
            else if (slugcat == VariantName.Hunterpup)
            {
                self.bodyWeightFac = 0.8f;
                self.generalVisibilityBonus = 0f;
                self.visualStealthInSneakMode = 0.35f;
                self.loudnessFac = 0.75f;
                self.lungsFac = 0.8f;
                self.throwingSkill = 1;
                self.poleClimbSpeedFac = 1f;
                self.corridorClimbSpeedFac = 0.95f;
                self.runspeedFac = 1f;
                if (malnourished)
                {
                    self.runspeedFac = 0.8f;
                    self.poleClimbSpeedFac = 0.8f;
                    self.corridorClimbSpeedFac = 0.8f;
                }
            }
            else if (slugcat == VariantName.Rotundpup)
            {
                self.generalVisibilityBonus = -0.2f;
                self.visualStealthInSneakMode = 0.6f;
                self.loudnessFac = 1f;
                self.lungsFac = 0.8f;
                self.throwingSkill = 2;
                self.poleClimbSpeedFac = 0.65f;
                self.corridorClimbSpeedFac = 0.65f;
                self.runspeedFac = 0.9f;
                if (malnourished)
                {
                    self.bodyWeightFac = 0.9f;
                    self.runspeedFac = 0.75f;
                    self.poleClimbSpeedFac = 0.5f;
                    self.corridorClimbSpeedFac = 0.5f;
                }
            }
        }
        private static IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
        {
            if (slugcat == VariantName.Aquaticpup)
            {
                return new IntVector2(4, 3);
            }
            else if (slugcat == VariantName.Tundrapup)
            {
                return new IntVector2(2, 2);
            }
            else if (slugcat == VariantName.Hunterpup)
            {
                return new IntVector2(5, 3);
            }
            else if (slugcat == VariantName.Rotundpup)
            {
                return new IntVector2(7, 4);
            }
            return orig(slugcat);
        }
        private static bool SlugcatStats_HiddenOrUnplayableSlugcat(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
        {
            if (i == VariantName.Aquaticpup)
            {
                return true;
            }
            if (i == VariantName.Tundrapup)
            {
                return true;
            }
            if (i == VariantName.Hunterpup)
            {
                return true;
            }
            if (i == VariantName.Rotundpup)
            {
                return true;
            }
            return orig(i);
        }
        private static void IL_SlugcatStats_NourishmentOfObjectEaten(ILContext il)
        {
            ILCursor variCurs = new(il);

            variCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Saint)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_000d
             * 	IL_0008: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Saint
	         *  IL_000d: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
	         *  IL_0012: brfalse.s IL_0046
             */
            variCurs.Emit(OpCodes.Ldarg_0); // slugcatIndex
            variCurs.EmitDelegate((SlugcatStats.Name slugcatIndex) =>   // If slugcatIndex is Tundrapup, return true
            {
                return slugcatIndex == VariantName.Tundrapup;
            });
            variCurs.Emit(OpCodes.Or);

            variCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Artificer)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_0062
             * 	IL_005d: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Artificer
	         *  IL_0062: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
	         *  IL_0067: brfalse.s IL_00b8
             */
            variCurs.Emit(OpCodes.Ldarg_0);
            variCurs.EmitDelegate((SlugcatStats.Name slugcatIndex) =>   // If slugcatIndex is Hunterpup, return true
            {
                return slugcatIndex == VariantName.Hunterpup;
            });
            variCurs.Emit(OpCodes.Or);

        }
    }
}
