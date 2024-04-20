using static SlugpupStuff.SlugpupStuff;

namespace SlugpupStuff
{
    public static class SlugpupCustom
    {
        public class GiftedItem
        {
            public AbstractPhysicalObject item;
            public int age;
            public float score;
            public GiftedItem(AbstractPhysicalObject gift, float s)
            {
                age = 0;
                item = gift;
                score = s;
            }
            public void Update()
            {
                age++;
            }
        }

        public static void PupSwallowObject(this Player self, int grabbedIndex)
        {
            Player parent = null;
            if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player player)
            {
                parent = player;
            }
            if (self.TryGetPupVariables(out var pupVariables))
            {
                if (self.objectInStomach == null && self.CanBeSwallowed(parent != null ? parent.grasps[grabbedIndex].grabbed : self.grasps[grabbedIndex].grabbed) && self.Consious)
                {
                    pupVariables.swallowing = true;
                    self.swallowAndRegurgitateCounter++;
                    self.AI.heldWiggle = 0;
                    if (self.swallowAndRegurgitateCounter > 90)
                    {
                        self.SwallowObject(grabbedIndex);
                        self.swallowAndRegurgitateCounter = 0;
                        (self.graphicsModule as PlayerGraphics).swallowing = 20;

                        pupVariables.swallowing = false;
                        pupVariables.wantsToSwallowObject = false;
                    }
                }
            }
        }
        public static void PupRegurgitate(this Player self)
        {
            if (self.TryGetPupVariables(out var pupVariables) && self.Consious)
            {
                pupVariables.regurgitating = true;
                self.swallowAndRegurgitateCounter++;

                bool spitUpObject = false;
                if (self.isRotundpup() && self.objectInStomach == null)
                {
                    spitUpObject = true;
                }
                self.AI.heldWiggle = 0;
                if (self.swallowAndRegurgitateCounter > 110)
                {
                    if (!spitUpObject || (spitUpObject && self.FoodInStomach > 0 && !self.Malnourished))
                    {
                        if (spitUpObject)
                        {
                            self.SubtractFood(1);
                        }
                        self.Regurgitate();
                    }
                    else
                    {
                        self.firstChunk.vel += new Vector2(Random.Range(-1f, 1f), 0f);
                        self.Stun(30);
                    }

                    self.swallowAndRegurgitateCounter = 0;
                    pupVariables.regurgitating = false;
                    pupVariables.wantsToRegurgitate = false;
                }
            }
        }
        public static void VariantMechanicsAquaticpup(this Player self)
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
        public static void VariantMechanicsRotundpup(this Player self)
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
        public static SlugcatStats.Name GetSlugpupVariant(this Player player)
        {

            if (SlugpupStuff.Pearlcat && PupsPlusModCompat.IsPearlpup(player)) return null;

            if (player.abstractCreature.TryGetPupAbstract(out var pupAbstract))
            {
                if (pupAbstract.aquatic) return VariantName.Aquaticpup;
                if (pupAbstract.tundra) return VariantName.Tundrapup;
                if (pupAbstract.hunter) return VariantName.Hunterpup;
                if (pupAbstract.rotund) return VariantName.Rotundpup;
                if (pupAbstract.regular) return null;
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
            return null;
        }
    }
}
