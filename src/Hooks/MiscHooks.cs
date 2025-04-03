
namespace SlugpupStuff.Hooks
{
    public static class MiscHooks
    {
        public static void Patch()
        {
            IL.Snail.Click += IL_Snail_Click;
            IL.Centipede.Update += IL_Centipede_Update;
            IL.RegionState.AdaptRegionStateToWorld += IL_RegionState_AdaptRegionStateToWorld;
        }
        private static void IL_RegionState_AdaptRegionStateToWorld(ILContext il)
        {
            ILCursor stomachObjCurs = new(il);

            stomachObjCurs.GotoNext(x => x.MatchLdstr("Add pup to pendingFriendSpawns {0}"));
            stomachObjCurs.GotoNext(MoveType.Before, x => x.MatchLdarg(0));
            /* GOTO BEFORE IL_02ab
             * 	IL_02b0: ldarg.0
			 *  IL_02b1: ldfld class SaveState RegionState::saveState
			 *  IL_02b6: ldfld class [mscorlib]System.Collections.Generic.List`1<string> SaveState::pendingFriendCreatures
             */
            stomachObjCurs.Emit(OpCodes.Ldloc, 6); // abstractCreature
            stomachObjCurs.EmitDelegate((AbstractCreature abstractCreature) =>   // If abstractCreature is player and player playerState is PlayerNPCState, set PupsPlusStomachObject to objectInStomach
            {
                if (abstractCreature.realizedCreature is Player player && player.isNPC)
                {
                    if (player.playerState.TryGetPupState(out var pupNPCState))
                    {
                        if (player.objectInStomach != null)
                        {
                            pupNPCState.PupsPlusStomachObject = player.objectInStomach;
                        }
                        else pupNPCState.PupsPlusStomachObject = null;
                    }
                }
            });
        }
        private static void IL_Snail_Click(ILContext il)
        {
            ILCursor staggerCurs = new(il);

            staggerCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Saint)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
             * 	ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Saint
			 *  call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
			 *  brfalse.s IL_067a
             */
            staggerCurs.Emit(OpCodes.Ldloc, 10); // item
            staggerCurs.EmitDelegate((PhysicalObject item) =>   // If item is Player and Player is Tundrapup, return true
            {
                return item is Player player && player.isTundrapup();
            });
            staggerCurs.Emit(OpCodes.Or);
        }
        private static void IL_Centipede_Update(ILContext il)
        {
            ILCursor staggerCurs = new(il);

            staggerCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Saint)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
             * 	ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Saint
			 *  call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
			 *  br.s IL_02ca
             */
            staggerCurs.Emit(OpCodes.Ldarg_0); // self
            staggerCurs.EmitDelegate((Centipede self) =>   // If grabber is Player and Player is Tundrapup, return true
            {
                return self.grabbedBy[0].grabber is Player player && player.isTundrapup();
            });
            staggerCurs.Emit(OpCodes.Or);
        }
    }
}
