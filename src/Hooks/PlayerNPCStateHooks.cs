
namespace SlugpupStuff.Hooks
{
    public static class PlayerNPCStateHooks
    {
        public static void Patch()
        {
            On.MoreSlugcats.PlayerNPCState.ToString += PlayerNPCState_ToString;
            On.MoreSlugcats.PlayerNPCState.LoadFromString += PlayerNPCState_LoadFromString;

            IL.MoreSlugcats.PlayerNPCState.CycleTick += IL_PlayerNPCState_CycleTick;
        }

        private static string PlayerNPCState_ToString(On.MoreSlugcats.PlayerNPCState.orig_ToString orig, PlayerNPCState self)
        {
            string text = orig(self);
            if (self.player.realizedCreature is Player pup && pup.playerState.TryGetPupState(out var pupNPCState))
            {
                text += "Variant<cC>" + ((pup.slugcatStats.name != MoreSlugcatsEnums.SlugcatStatsName.Slugpup) ? pup.slugcatStats.name.value : "NULL") + "<cB>";
                text += "PupsPlusStomach<cC>" + ((pupNPCState.PupsPlusStomachObject != null) ? pupNPCState.PupsPlusStomachObject.ToString() : "NULL") + "<cB>";
            }
            return text;

        }
        private static void PlayerNPCState_LoadFromString(On.MoreSlugcats.PlayerNPCState.orig_LoadFromString orig, PlayerNPCState self, string[] s)
        {
            orig(self, s);
            if (self.TryGetPupState(out var pupNPCState))
            {
                for (int i = 0; i < s.Length - 1; i++)
                {
                    string[] array = Regex.Split(s[i], "<cC>");
                    switch (array[0])
                    {
                        case "Variant":
                            pupNPCState.Variant = array[1] switch
                            {
                                "Aquaticpup" => VariantName.Aquaticpup,
                                "Tundrapup" => VariantName.Tundrapup,
                                "Hunterpup" => VariantName.Hunterpup,
                                "Rotundpup" => VariantName.Rotundpup,
                                "NULL" => VariantName.Regular,
                                _ => null
                            };
                            break;
                        case "PupsPlusStomach":
                            if (array[1] != "NULL")
                            {
                                if (array[1].Contains("<oA>"))
                                {
                                    pupNPCState.PupsPlusStomachObject = SaveState.AbstractPhysicalObjectFromString(self.player.Room.world, array[1]);
                                }
                                else if (array[1].Contains("<cA>"))
                                {
                                    pupNPCState.PupsPlusStomachObject = SaveState.AbstractCreatureFromString(self.player.Room.world, array[1], onlyInCurrentRegion: false);
                                }
                            }
                            break;
                        // BeastMasterPupExtras Compat
                        case "SlugcatCharacter":
                            if (BeastMasterPupExtras && !array[1].Equals("Slugpup"))
                            {
                                pupNPCState.Variant = null;
                            }
                            break;
                    }
                }
            }
            self.unrecognizedSaveStrings.Remove("Variant");
            self.unrecognizedSaveStrings.Remove("PupsPlusStomach");
        }
        private static void IL_PlayerNPCState_CycleTick(ILContext il)
        {
            ILCursor foodCurs = new(il);

            while (foodCurs.TryGotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup))))
            {
                /* WHILE TRYGOTO AFTER ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Slugpup
                 * 	IL_****: ldarg.0
	             *  IL_****: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Slugpup
                 */
                foodCurs.Emit(OpCodes.Ldarg_0); // self
                foodCurs.EmitDelegate((SlugcatStats.Name slugpup, PlayerNPCState self) =>   // If pupNPCState.variant != Regular, return variant, else return slugpup
                {
                    if (self.TryGetPupState(out var pupNPCState))
                    {
                        SlugcatStats.Name variant = pupNPCState.Variant;
                        if (variant != VariantName.Regular)
                        {
                            return variant;
                        }
                    }
                    return slugpup;
                });
            }
        }
    }
}
