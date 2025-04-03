
namespace SlugpupStuff.PupsPlusCustom
{
    public static class VariantStuff
    {
        public static void VariantMechanicsAquaticpup(Player self)
        {
            if (self.isAquaticpup())
            {
                self.buoyancy = 0.9f;
                if (self.grasps[0] != null && self.grasps[0].grabbed is WaterNut waterNut)
                {
                    waterNut.swellCounter--;
                    if (waterNut.swellCounter < 1)
                    {
                        waterNut.Swell();
                    }
                }
                Player parent = null;
                if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player player)
                {
                    parent = player;
                }
                if (parent != null && parent.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    if (!self.submerged)
                    {
                        self.slowMovementStun = 5;
                    }
                    if (!parent.monkAscension)
                    {
                        parent.buoyancy = 0.9f;
                    }
                }
            }
        }
        public static void VariantMechanicsRotundpup(Player self)
        {
            Player pupOnBack = null;
            if (self.slugOnBack?.slugcat != null)
            {
                pupOnBack = self.slugOnBack.slugcat;
                while (pupOnBack.slugOnBack?.slugcat != null)
                {
                    if (pupOnBack.isRotundpup()) break;
                    pupOnBack = pupOnBack.slugOnBack.slugcat;
                }
            }
            if (self.TryGetParentVariables(out var parentVariables))
            {
                if (pupOnBack != null && pupOnBack.isRotundpup() && self.aerobicLevel >= 0.9f)
                {
                    parentVariables.rotundPupExhaustion = true;
                }
                if (self.aerobicLevel < 0.6f)
                {
                    parentVariables.rotundPupExhaustion = false;
                }
                if (parentVariables.rotundPupExhaustion)
                {
                    if (pupOnBack != null && pupOnBack.isRotundpup())
                    {
                        self.slowMovementStun = Mathf.Max(self.slowMovementStun, (int)Custom.LerpMap(self.aerobicLevel, 0.35f, 0.2f, 4f, 0f));
                    }
                    self.lungsExhausted = true;
                }
            }
        }
        public static SlugcatStats.Name GetSlugpupVariant(Player player)
        {

            if (SlugpupStuff.Pearlcat && PupsPlusModCompat.IsPearlpup(player)) return VariantName.Regular;

            if (player.playerState.TryGetPupState(out var pupState))
            {
                if (pupState.Variant != null)
                {
                    return pupState.Variant;
                }
            }

            if (!SlugpupStuff.ID_PupIDExclude().Contains(player.abstractCreature.ID.RandomSeed))
            {
                Random.State state = Random.state;
                Random.InitState(player.abstractCreature.ID.RandomSeed);

                float variChance = Random.value;

                Random.state = state;

                // setup variant chance
                if (variChance <= aquaticChance || ID_AquaticPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Aquaticpup;
                }
                else if (variChance <= tundraChance || ID_TundraPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Tundrapup;
                }
                else if (variChance <= hunterchance || ID_HunterPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Hunterpup;
                }
                else if (variChance <= rotundChance || ID_RotundPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Rotundpup;
                }
            }

            return VariantName.Regular;
        }

        public static void SetVariantFromAbstract(AbstractCreature abstractPup)
        {
            if (abstractPup.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
            {
                if (abstractPup.state is PlayerNPCState npcState && npcState.TryGetPupState(out var pupState))
                {
                    if (abstractPup.spawnData == null || abstractPup.spawnData[0] != '{')
                    {
                        pupState.Variant = null;
                        return;
                    }
                    string[] array = abstractPup.spawnData.Substring(1, abstractPup.spawnData.Length - 2).Split([',']);
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].Length > 0)
                        {
                            switch (array[i].Split([':'])[0])
                            {
                                case "Aquatic":
                                    pupState.Variant = VariantName.Aquaticpup;
                                    break;
                                case "Tundra":
                                    pupState.Variant = VariantName.Tundrapup;
                                    break;
                                case "Hunter":
                                    pupState.Variant = VariantName.Hunterpup;
                                    break;
                                case "Rotund":
                                    pupState.Variant = VariantName.Rotundpup;
                                    break;
                                case "Regular":
                                    pupState.Variant = VariantName.Regular;
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
