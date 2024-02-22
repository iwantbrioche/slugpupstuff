using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace SlugpupStuff
{
    public partial class SlugpupStuff   // Slugpup Variants, Variant Stats, and Variant Abilities
    {
        public class VariantName
        {
            public static SlugcatStats.Name Aquaticpup;
            public static SlugcatStats.Name Tundrapup;
            public static SlugcatStats.Name Hunterpup;
            public static SlugcatStats.Name Rotundpup;

            public static void RegisterValues()
            {
                Aquaticpup = new("Aquaticpup", true);
                Tundrapup = new("Tundrapup", true);
                Hunterpup = new("Hunterpup", true);
                Rotundpup = new("Rotundpup", true);
            }
            public static void UnregisterValues()
            {
                Aquaticpup?.Unregister();
                Aquaticpup = null;
                Tundrapup?.Unregister();
                Tundrapup = null;
                Hunterpup?.Unregister();
                Hunterpup = null;
                Rotundpup?.Unregister();
                Rotundpup = null;
            }
        }

        // Variant Methods
        public List<int> ID_PupIDExclude()
        {
            List<int> idlist =
            [
                1000,
                1001,
                2220
            ];
            return idlist;
        }
        public List<int> ID_AquaticPupID()
        {
            List<int> idlist = [];
            return idlist;
        }
        public List<int> ID_TundraPupID()
        {
            List<int> idlist = [];
            return idlist;

        }
        public List<int> ID_HunterPupID()
        {
            List<int> idlist = [];
            return idlist;
        }
        public List<int> ID_RotundPupID()
        {
            List<int> idlist = [];
            return idlist;
        }
        public SlugcatStats.Name GetSlugpupVariant(Player player)
        {
            if (pearlCat && PupsPlusModCompat.IsPearlpup(player))
            {
                return null;
            }
            if (SlugpupCWTs.pupAbstractCWT.TryGetValue(player.abstractCreature, out var pupAbstract))
            {
                if (pupAbstract.aquatic) return VariantName.Aquaticpup;
                if (pupAbstract.tundra) return VariantName.Tundrapup;
                if (pupAbstract.hunter) return VariantName.Hunterpup;
                if (pupAbstract.rotund) return VariantName.Rotundpup;
                if (pupAbstract.regular) return null;
            }
            if (player.abstractCreature.ID != null && !ID_PupIDExclude().Contains(player.abstractCreature.ID.RandomSeed))
            {
                Random.State state = Random.state;
                Random.InitState(player.abstractCreature.ID.RandomSeed);

                float variChance = Random.value;

                Random.state = state;
                float aquaticChance = (slugpupRemix.aquaticChance.Value - slugpupRemix.tundraChance.Value) / 100f;
                float tundraChance = ((slugpupRemix.tundraChance.Value - slugpupRemix.hunterChance.Value) / 100f) + aquaticChance;
                float hunterchance = ((slugpupRemix.hunterChance.Value - slugpupRemix.rotundChance.Value) / 100f) + tundraChance;
                float rotundChance = (slugpupRemix.rotundChance.Value / 100f) + hunterchance;

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
        public void SetSlugpupPersonality(Player self)
        {
            if (self.playerState is PlayerNPCState && SlugpupCWTs.pupStateCWT.TryGetValue(self.playerState as PlayerNPCState, out var pupNPCState))
            {
                Random.State state = Random.state;
                Random.InitState(self.abstractCreature.ID.RandomSeed);
                if (pupNPCState.Variant == VariantName.Aquaticpup)
                {
                    // Higher Energy Calculation
                    self.abstractCreature.personality.energy = Random.value;
                    self.abstractCreature.personality.energy = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.energy + 0.15f + 0.1f * (1.25f * self.abstractCreature.personality.energy), 0.4f), 0f, 1f);
                    // Base Personality Calculations
                    self.abstractCreature.personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(self.abstractCreature.personality.energy, 1f - self.abstractCreature.personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                    self.abstractCreature.personality.aggression = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery) / 2f * (1f - self.abstractCreature.personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                    self.abstractCreature.personality.dominance = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery + self.abstractCreature.personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                    self.abstractCreature.personality.nervous = Custom.PushFromHalf(self.abstractCreature.personality.nervous, 2.5f);
                    self.abstractCreature.personality.aggression = Custom.PushFromHalf(self.abstractCreature.personality.aggression, 2.5f);
                }
                if (pupNPCState.Variant == VariantName.Tundrapup)
                {
                    // Higher Sympathy Calculation
                    self.abstractCreature.personality.sympathy = Random.value;
                    self.abstractCreature.personality.sympathy = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.sympathy + 0.15f + 0.5f * (0.4f * self.abstractCreature.personality.sympathy), 2f), 0f, 1f);
                    // Base Nervousness Calculation
                    self.abstractCreature.personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(self.abstractCreature.personality.energy, 1f - self.abstractCreature.personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                    // Base Aggression & Dominance Calculations
                    self.abstractCreature.personality.aggression = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery) / 2f * (1f - self.abstractCreature.personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                    self.abstractCreature.personality.dominance = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery + self.abstractCreature.personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                    // Higher Nervousness & Less Aggression Calculations
                    self.abstractCreature.personality.nervous = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.nervous + 0.2f * 0.15f * (1.25f * self.abstractCreature.personality.nervous), 3f), 0f, 1f);
                    self.abstractCreature.personality.aggression = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.aggression / 1.75f, 0.485f), 0f, 1f);   
                }
                if (pupNPCState.Variant == VariantName.Hunterpup)
                {
                    // Higher Bravery & Less Sympathy Calculations
                    self.abstractCreature.personality.sympathy = Random.value;
                    self.abstractCreature.personality.bravery = Random.value;
                    self.abstractCreature.personality.sympathy = Mathf.Clamp(Custom.PushFromHalf((self.abstractCreature.personality.sympathy / 1.2f) - 0.15f, 0.6f), 0f, 1f);
                    self.abstractCreature.personality.bravery = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.bravery + 0.01f * (1.5f * self.abstractCreature.personality.bravery), 1.05f), 0f, 1f);
                    // Base Nervousness Calculation
                    self.abstractCreature.personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(self.abstractCreature.personality.energy, 1f - self.abstractCreature.personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                    // Base Aggression Calculation
                    self.abstractCreature.personality.aggression = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery) / 2f * (1f - self.abstractCreature.personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                    // Base Dominance & Nervousness Calculations
                    self.abstractCreature.personality.dominance = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery + self.abstractCreature.personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                    self.abstractCreature.personality.nervous = Custom.PushFromHalf(self.abstractCreature.personality.nervous, 2.5f);
                    // Higher Aggression Calculation
                    self.abstractCreature.personality.aggression = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.aggression + 0.1f + 0.05f * (0.25f * self.abstractCreature.personality.aggression), 1.35f), 0f, 1f);
                }
                if (pupNPCState.Variant == VariantName.Rotundpup)
                {
                    // Lower Energy Calculation
                    self.abstractCreature.personality.energy = Random.value;
                    self.abstractCreature.personality.energy = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.energy / 1.6f, 0.8f), 0f, 1f);
                    // Base Nervousness & Aggression Calculations
                    self.abstractCreature.personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(self.abstractCreature.personality.energy, 1f - self.abstractCreature.personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                    self.abstractCreature.personality.aggression = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery) / 2f * (1f - self.abstractCreature.personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                    // Higher Dominance Calculation
                    self.abstractCreature.personality.dominance = Mathf.Clamp(Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery + self.abstractCreature.personality.aggression) / 3f, Mathf.Pow(Random.value, .85f)) * 1.25f, 0f, 1f);
                    // Base Nervousness & Aggression Calculations
                    self.abstractCreature.personality.nervous = Custom.PushFromHalf(self.abstractCreature.personality.nervous, 2.5f);
                    self.abstractCreature.personality.aggression = Custom.PushFromHalf(self.abstractCreature.personality.aggression, 2.5f);
                }
                Random.state = state;
            }
        }

        // Hooks
        private void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
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
        private IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
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
        private bool SlugcatStats_HiddenOrUnplayableSlugcat(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
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
        private void IL_SlugcatStats_NourishmentOfObjectEaten(ILContext il)
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
        private void VariantMechanicsAquaticpup(Player self)
        {
            if (self.slugcatStats.name == VariantName.Aquaticpup)
            {
                self.buoyancy = 0.9f;
                if (self.grasps[0] != null && self.grasps[0].grabbed is WaterNut)
                {
                    (self.grasps[0].grabbed as WaterNut).swellCounter--;
                    if ((self.grasps[0].grabbed as WaterNut).swellCounter < 1)
                    {
                        (self.grasps[0].grabbed as WaterNut).Swell();
                    }
                }
                Player parent = null;
                if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player player)
                {
                    parent = player;
                }
                if (parent != null && parent.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    self.slowMovementStun = 5;
                }
            }
        }
        private void Player_VariantMechanicsTundrapup(On.Player.orig_ClassMechanicsSaint orig, Player self)
        {
            if (self.slugcatStats.name == VariantName.Tundrapup || (self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint && self.isNPC))
            {
                Player parent = null;
                if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player player)
                {
                    parent = player;
                }
                if (self.onBack != null && slugpupRemix.BackTundraGrapple.Value)
                {
                    parent = self.onBack.slugOnBack.owner;
                    while (parent.onBack != null)
                    {
                        parent = parent.onBack.slugOnBack.owner;
                    }
                }
                if (parent != null)
                {
                    if (parent.input[0].jmp && !parent.input[1].jmp && !parent.input[0].pckp && parent.canJump <= 0 && parent.bodyMode != Player.BodyModeIndex.Crawl && parent.animation != Player.AnimationIndex.ClimbOnBeam && parent.animation != Player.AnimationIndex.AntlerClimb && parent.animation != Player.AnimationIndex.HangFromBeam && self.SaintTongueCheck())
                    {
                        Vector2 vector = new(parent.flipDirection, 0.7f);
                        Vector2 normalized = vector.normalized;
                        if (parent.input[0].y > 0)
                        {
                            normalized = new Vector2(0f, 1f);
                        }
                        normalized = (normalized + self.mainBodyChunk.vel.normalized * 0.2f).normalized;
                        self.tongue.Shoot(normalized);
                    }
                }
                else
                {
                    if (self.input[0].jmp && !self.input[1].jmp && !self.input[0].pckp && self.canJump <= 0 && self.bodyMode != Player.BodyModeIndex.Crawl && self.animation != Player.AnimationIndex.ClimbOnBeam && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.HangFromBeam && self.SaintTongueCheck())
                    {
                        Vector2 vector = new(self.flipDirection, 0.7f);
                        Vector2 normalized = vector.normalized;
                        if (self.input[0].y > 0)
                        {
                            normalized = new Vector2(0f, 1f);
                        }
                        normalized = (normalized + self.mainBodyChunk.vel.normalized * 0.2f).normalized;
                        self.tongue.Shoot(normalized);
                    }
                }
            }
            else
            {
                orig(self);
            }
        }
        private void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (self.slugcatStats.name == VariantName.Rotundpup && !self.gourmandExhausted)
            {
                if (simpMovesetGourmand)
                {
                    self.gourmandExhausted = true;
                }
                if ((self.room != null && self.room.gravity == 0f) || Mathf.Abs(spear.firstChunk.vel.x) < 1f)
                {
                    spear.firstChunk.vel += spear.firstChunk.vel.normalized * 4.5f;
                }
                self.gourmandAttackNegateTime = 80;
            }
        }
        private bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
        {
            if ((self.slugcatStats.name == VariantName.Hunterpup || self.slugcatStats.name == VariantName.Rotundpup) && crit is not Player)
            {
                return true;
            }
            return orig(self, crit);
        }
        private bool Player_AllowGrabbingBatflys(On.Player.orig_AllowGrabbingBatflys orig, Player self)
        {
            if (self.isNPC && self.slugcatStats.name == VariantName.Tundrapup)
            {
                return false;
            }
            return orig(self);
        }
        private bool Player_SaintTongueCheck(On.Player.orig_SaintTongueCheck orig, Player self)
        {
            if (self.Consious && self.slugcatStats.name == VariantName.Tundrapup && self.tongue.mode == Player.Tongue.Mode.Retracted && self.bodyMode != Player.BodyModeIndex.CorridorClimb && !self.corridorDrop && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab)
            {
                return true;
            }
            return orig(self);
        }
        private void Player_TongueUpdate(On.Player.orig_TongueUpdate orig, Player self)
        {
            if (self.isNPC)
            {
                Player parent = null;
                if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player player)
                {
                    parent = player;
                }
                if (self.onBack != null)
                {
                    parent = self.onBack.slugOnBack.owner;
                    while (parent.onBack != null)
                    {
                        parent = parent.onBack.slugOnBack.owner;
                    }
                }
                if (parent != null)
                {
                    if (self.tongue == null || self.room == null) return;

                    if (parent.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer && parent.input[0].jmp && parent.input[1].pckp)
                    {
                        self.tongue.Release();
                    }

                    self.tongue.baseChunk = parent.bodyChunks[0];
                    if (self.tongue.Attached)
                    {
                        self.tongueAttachTime++;
                        if (self.Stunned)
                        {
                            self.tongue.Release();
                        }
                        else
                        {
                            if (parent.input[0].y > 0)
                            {
                                self.tongue.decreaseRopeLength(3f);
                            }

                            if (parent.input[0].y < 0)
                            {
                                self.tongue.increaseRopeLength(3f);
                            }

                            if (parent.input[0].jmp && !parent.input[1].jmp && self.tongueAttachTime >= 2)
                            {
                                self.tongue.Release();
                                if (!self.tongue.isZeroGMode())
                                {
                                    float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);
                                    if (self.onBack != null)
                                    {
                                        if (parent.grasps[0] != null && parent.HeavyCarry(parent.grasps[0].grabbed) && parent.grasps[0].grabbed is not Cicada)
                                        {
                                            num += Mathf.Min(Mathf.Max(0f, parent.grasps[0].grabbed.TotalMass - 0.2f) * 1.5f, 1.3f);
                                        }
                                    }
                                    parent.bodyChunks[0].vel.y = 6f * num;
                                    parent.bodyChunks[1].vel.y = 5f * num;
                                    parent.jumpBoost = 6.5f;
                                    if (self.grabbedBy.Count > 0)
                                    {
                                        self.bodyChunks[0].vel.y = 6f * num;
                                        self.bodyChunks[1].vel.y = 5f * num;
                                        self.jumpBoost = 6.5f;
                                    }
                                }

                                self.room.PlaySound(SoundID.Slugcat_Normal_Jump, self.mainBodyChunk, loop: false, 1f, 1f);
                            }
                        }
                    }
                    else
                    {
                        self.tongueAttachTime = 0;
                    }

                    self.tongue.Update();
                    if (self.tongue.rope.totalLength > self.tongue.totalRope * 2.5f)
                    {
                        self.tongue.Release();
                    }
                }
                else
                {
                    orig(self);
                }
            }
            else
            {
                orig(self);
            }
        }
        private void Tongue_Shoot(On.Player.Tongue.orig_Shoot orig, Player.Tongue self, Vector2 dir)
        {
            if (self.player.isNPC)
            {
                Player parent = null;
                if (self.player.grabbedBy.Count > 0 && self.player.grabbedBy[0].grabber is Player player)
                {
                    parent = player;
                }
                if (self.player.onBack != null)
                {
                    parent = self.player.onBack.slugOnBack.owner;
                    while (parent.onBack != null)
                    {
                        parent = parent.onBack.slugOnBack.owner;
                    }
                }
                self.resetRopeLength();
                if (self.Attached)
                {
                    self.Release();
                }
                else if (parent != null && !slugpupRemix.SaintTundraGrapple.Value && parent.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                {
                    return;
                }
                else if (self.mode == Player.Tongue.Mode.Retracted)
                {
                    self.mode = Player.Tongue.Mode.ShootingOut;
                    self.player.room.PlaySound(SoundID.Tube_Worm_Shoot_Tongue, self.baseChunk);
                    float num = parent != null ? parent.input[0].x : self.player.input[0].x;
                    float num2 = parent != null ? parent.input[0].y : self.player.input[0].y;
                    if (self.isZeroGMode() && (num != 0f || num2 != 0f))
                    {
                        dir = new Vector2(num, num2).normalized;
                        if (parent != null)
                        {
                            parent.bodyChunks[0].vel = new Vector2(self.GetTargetZeroGVelo(parent.bodyChunks[0].vel.x, dir.x), self.GetTargetZeroGVelo(parent.bodyChunks[0].vel.y, dir.y));
                            parent.bodyChunks[1].vel = new Vector2(self.GetTargetZeroGVelo(parent.bodyChunks[1].vel.x, dir.x), self.GetTargetZeroGVelo(parent.bodyChunks[1].vel.y, dir.y));
                        }
                        else
                        {
                            self.player.bodyChunks[0].vel = new Vector2(self.GetTargetZeroGVelo(self.player.bodyChunks[0].vel.x, dir.x), self.GetTargetZeroGVelo(self.player.bodyChunks[0].vel.y, dir.y));
                            self.player.bodyChunks[1].vel = new Vector2(self.GetTargetZeroGVelo(self.player.bodyChunks[1].vel.x, dir.x), self.GetTargetZeroGVelo(self.player.bodyChunks[1].vel.y, dir.y));
                        }
                    }
                    else
                    {
                        dir = self.AutoAim(dir);
                    }

                    self.pos = self.baseChunk.pos + dir * 5f;
                    self.elastic = 1f;
                    if (!self.player.Malnourished)
                    {
                        self.vel = dir * 70f;
                        self.requestedRopeLength = 140f;
                    }
                    else
                    {
                        self.vel = dir * 35f;
                        self.requestedRopeLength = 100f;
                    }
                    self.returning = false;
                }
            }
            else
            {
                orig(self, dir);
            }
        }
        private bool Player_SlugSlamConditions(On.Player.orig_SlugSlamConditions orig, Player self, PhysicalObject otherObject)
        {
            if (self.slugcatStats.name == VariantName.Rotundpup && otherObject is Player)
            {
                return false;
            }
            return orig(self, otherObject);
        }
        private void Tongue_decreaseRopeLength(On.Player.Tongue.orig_decreaseRopeLength orig, Player.Tongue self, float amount)
        {
            if (self.player.isNPC && self.player.Malnourished)
            {
                amount /= 2f;
            }
            orig(self, amount);
        }
        private void Tongue_increaseRopeLength(On.Player.Tongue.orig_increaseRopeLength orig, Player.Tongue self, float amount)
        {
            if (self.player.isNPC && self.player.Malnourished)
            {
                amount /= 2.5f;
            }
            orig(self, amount);
        }
        private float SlugNPCAI_LethalWeaponScore(On.MoreSlugcats.SlugNPCAI.orig_LethalWeaponScore orig, SlugNPCAI self, PhysicalObject obj, Creature target)
        {
            if (self.cat.slugcatStats.name == VariantName.Tundrapup)
            {
                if (obj is Spear)
                {
                    return 0.005f;
                }
            }
            return orig(self, obj, target);
        }
        private bool SlugNPCAI_TheoreticallyEatMeat(On.MoreSlugcats.SlugNPCAI.orig_TheoreticallyEatMeat orig, SlugNPCAI self, Creature crit, bool excludeCentipedes)
        {
            if (self.cat.slugcatStats.name == VariantName.Tundrapup)
            {
                if (crit.dead && crit.State.meatLeft > 0)
                {
                    if (self.friendTracker.giftOfferedToMe?.item != null && crit == self.friendTracker.giftOfferedToMe.item)
                    {
                        if ((crit.Template.type == CreatureTemplate.Type.SmallCentipede || crit.Template.type == CreatureTemplate.Type.VultureGrub || crit.Template.type == CreatureTemplate.Type.SmallNeedleWorm || crit.Template.type == CreatureTemplate.Type.Hazer || !excludeCentipedes) && crit is IPlayerEdible)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            if (self.cat.slugcatStats.name == VariantName.Hunterpup || self.cat.slugcatStats.name == VariantName.Rotundpup)
            {
                if (crit is IPlayerEdible || (crit.dead && crit.State.meatLeft > 0 && crit is not Player))
                {
                    return true;
                }
                return false;
            }
            return orig(self, crit, excludeCentipedes);
        }
        private bool SlugNPCAI_WantsToEatThis(On.MoreSlugcats.SlugNPCAI.orig_WantsToEatThis orig, SlugNPCAI self, PhysicalObject obj)
        {
            if (self.cat.slugcatStats.name == VariantName.Tundrapup)
            {
                if ((obj is IPlayerEdible && (obj as IPlayerEdible).Edible && obj is not Creature) || (obj is Creature && (self.TheoreticallyEatMeat(obj as Creature, excludeCentipedes: false) || self.friendTracker.giftOfferedToMe?.item != null && self.friendTracker.giftOfferedToMe.item == obj && (obj as Creature).dead)))
                {
                    return !self.IsFull;
                }
                return false;
            }
            if (self.cat.slugcatStats.name == VariantName.Aquaticpup)
            {
                if ((obj is IPlayerEdible && (obj as IPlayerEdible).Edible) || obj is WaterNut || (obj is Creature && self.TheoreticallyEatMeat(obj as Creature, excludeCentipedes: false) && (obj as Creature).dead))
                {
                    return !self.IsFull;
                }
                return false;
            }
            return orig(self, obj);

        }
        private bool SlugNPCAI_HasEdible(On.MoreSlugcats.SlugNPCAI.orig_HasEdible orig, SlugNPCAI self)
        {
            if (self.cat.slugcatStats.name == VariantName.Aquaticpup)
            {
                if (self.cat.grasps[0] != null && self.WantsToEatThis(self.cat.grasps[0].grabbed) && (self.cat.grasps[0].grabbed is not Creature || (self.cat.grasps[0].grabbed as Creature).dead || Random.value < Math.Pow(Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0.9f, 0.7f, self.creature.personality.sympathy)), 0.1)))
                {
                    if (self.cat.grasps[0].grabbed is WaterNut && (self.cat.grasps[0].grabbed as WaterNut) != null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return orig(self);
        }
        private SlugNPCAI.Food SlugNPCAI_GetFoodType(On.MoreSlugcats.SlugNPCAI.orig_GetFoodType orig, SlugNPCAI self, PhysicalObject food)
        {
            if (self.cat.slugcatStats.name == VariantName.Aquaticpup)
            {
                if (food is WaterNut)
                {
                    return SlugNPCAI.Food.WaterNut;
                }
            }
            return orig(self, food);
        }
        private void IL_Player_Jump(ILContext il)
        {
            ILCursor aquaCurs = new(il);

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchStfld<Player>(nameof(Player.jumpBoost)));
            /* GOTO
            else if (isSlugpup)
            {
                jumpBoost = 4f;
                >HERE<
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((Player self) =>   // If slugcatStats.name is Aquaticpup, set jumpBoost to 10f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    self.jumpBoost = 8f;
                }
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(2.25f));
            /* GOTO
            for (int i = 0; i < 2; i++)
            {
                base.bodyChunks[i].pos.y += (isSlugpup ? 2.25f >HERE< : 4.5f);
                base.bodyChunks[i].vel.y += (isSlugpup ? 1f : 2f);
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>   // If slugcatStats.name is Aquaticpup, return 3.5f, else return 2.25f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 3.25f;
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(1f));
            /* GOTO
            for (int i = 0; i < 2; i++)
            {
                base.bodyChunks[i].pos.y += (isSlugpup ? 2.25f : 4.5f);
                base.bodyChunks[i].vel.y += (isSlugpup ? 1f >HERE< : 2f);
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>   // If slugcatStats.Name is Aquaticpup, return 1.75f, else return 1f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 1.5f;
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(x => x.MatchLdcR4(4.5f));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchStfld<Vector2>(nameof(Vector2.x)));
            /* GOTO
            else if (isSlugpup)
            {
                base.bodyChunks[0].vel.y = 7f * num;
                base.bodyChunks[1].vel.y = 6f * num;
                base.bodyChunks[0].vel.x = 5f * (float)flipDirection * num;
                base.bodyChunks[1].vel.x = 4.5f * (float)flipDirection * num;
                >HERE<
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.Emit(OpCodes.Ldloc_0); // num
            aquaCurs.EmitDelegate((Player self, float num) =>   // If slugcatStats.name is Aquaticpup, set bodyChunks velocity
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    self.bodyChunks[0].vel.y = 8f * num;
                    self.bodyChunks[1].vel.y = 7f * num;
                    self.bodyChunks[0].vel.x = 6f * (float)self.flipDirection * num;
                    self.bodyChunks[1].vel.x = 5.5f * (float)self.flipDirection * num;
                }
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(0.65f));
            /* GOTO
            base.bodyChunks[0].vel = Custom.DegToVec((float)rollDirection * Mathf.Lerp(60f, 35f, t)) * Mathf.Lerp(9.5f, 13.1f, t) * num * (isSlugpup ? 0.65f >HERE<: 1f);
            base.bodyChunks[1].vel = Custom.DegToVec((float)rollDirection * Mathf.Lerp(60f, 35f, t)) * Mathf.Lerp(9.5f, 13.1f, t) * num * (isSlugpup ? 0.65f : 1f);
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>   // If slugcatStats.Name is Aquaticpup, return 0.8f, else return 0.65f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 0.8f;
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(0.65f));
            /* GOTO
            base.bodyChunks[0].vel = Custom.DegToVec((float)rollDirection * Mathf.Lerp(60f, 35f, t)) * Mathf.Lerp(9.5f, 13.1f, t) * num * (isSlugpup ? 0.65f : 1f);
            base.bodyChunks[1].vel = Custom.DegToVec((float)rollDirection * Mathf.Lerp(60f, 35f, t)) * Mathf.Lerp(9.5f, 13.1f, t) * num * (isSlugpup ? 0.65f >HERE< : 1f);
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>   // If slugcatStats.Name is Aquaticpup, return 0.8f, else return 0.65f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 0.8f;
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            /* GOTO
            if (isSlugpup)
            {
                num2 = 6f >HERE< ;
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>   // If slugcatStats.Name is Aquaticpup, return 10f, else return 6f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 9f;
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            /* GOTO
            if (isSlugpup)
            {
                y = 6f >HERE< ;
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>   // If slugcatStats.Name is Aquaticpup, return 10f, else return 6f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 7.5f;
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcI4(7));
            /* GOTO
            jumpBoost = (isSlugpup ? 7 >HERE< : 8);
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((int i, Player self) =>   // If slugcatStats.Name is Aquaticpup, return 8, else return 7
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 8;
                }
                return i;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchStfld<Player>(nameof(Player.jumpBoost)));
            /* GOTO
            if (isSlugpup)
            {
                jumpBoost = 3f;
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((Player self) =>   // If slugcatStats.name is Aquaticpup, set jumpBoost to 8f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    self.jumpBoost = 6.5f;
                }
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcI4(7));
            /* GOTO
            jumpBoost = (isSlugpup ? 7 >HERE< : 8);
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((int i, Player self) =>   // If slugcatStats.Name is Aquaticpup, return 8, else return 7
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 8;
                }
                return i;
            });

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            /* GOTO
            else if (isSlugpup)
            {
                num5 = 5.5f;
            }
            */
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>   // If slugcatStats.name is Aquaticpup, return 8f, else return 5.5f
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 7.5f;
                }
                return f;
            });
        } // fuck these two
        private void IL_Player_UpdateAnimation(ILContext il)
        {
            ILCursor aquaCurs = new(il);
            aquaCurs.GotoNext(x => x.MatchLdsfld<Player.AnimationIndex>(nameof(Player.AnimationIndex.DeepSwim)));
            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(x => x.MatchLdfld<Player>(nameof(Player.submerged)));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdarg(0));

            aquaCurs.Emit(OpCodes.Ldloc, 8);
            aquaCurs.EmitDelegate((Player self, bool flag3) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return self.submerged;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return self.submerged;
                        }
                    }
                }
                return flag3;
            });
            aquaCurs.Emit(OpCodes.Stloc, 8);
            aquaCurs.Emit(OpCodes.Ldarg_0);

            ILCursor aquaLblCurs = aquaCurs.Clone();
            aquaLblCurs.GotoNext(x => x.MatchLdarg(0), x => x.MatchLdfld<UpdatableAndDeletable>(nameof(UpdatableAndDeletable.room)));
            ILLabel isaquaSkiplabel = il.DefineLabel();
            aquaLblCurs.MarkLabel(isaquaSkiplabel);


            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(x => x.MatchLdcR4(6f));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdarg(0));
            aquaCurs.Emit(OpCodes.Ldloc, 11);
            aquaCurs.Emit(OpCodes.Ldloc, 10);
            aquaCurs.EmitDelegate((Player self, float num2, Vector2 vector2) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    self.airInLungs -= 0.025f * num2;
                    self.bodyChunks[0].vel += vector2 * ((vector2.y > 0.5f) ? 300f : 50f);
                    return true;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            self.bodyChunks[0].vel += vector2 * ((vector2.y > 0.5f) ? 300f : 50f);
                            self.airInLungs -= (ModManager.MMF ? 0.18f : 0.2f) * num2;
                            return true;
                        }
                    }
                }

                return false;
            });
            aquaCurs.Emit(OpCodes.Brtrue_S, isaquaSkiplabel);
            aquaCurs.Emit(OpCodes.Ldarg_0);

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcI4(20));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((int i, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 12;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 12;
                        }
                    }
                }
                return i;
            });

            aquaLblCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaLblCurs.GotoNext(x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)));
            ILLabel isaquaSkiplabel2 = il.DefineLabel();
            aquaLblCurs.MarkLabel(isaquaSkiplabel2);

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Artificer)));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchStloc(15));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.Emit(OpCodes.Ldloc, 15);
            aquaCurs.EmitDelegate((Player self, float num4) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 1f + Mathf.Log10(self.airInLungs + 0.2f);
                }
                return num4;
            });
            aquaCurs.Emit(OpCodes.Stloc, 15);

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(
                MoveType.After,
                x => x.MatchLdloc(7),
                x => x.Match(OpCodes.Call),
                x => x.Match(OpCodes.Call)
                );
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((Player self) =>
            {
                if (self.waterJumpDelay >= 5)
                {
                    if (self.slugcatStats.name == VariantName.Aquaticpup)
                    {
                        self.waterFriction = 0.99f;
                    }
                    if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                    {
                        foreach (var grasped in self.grasps)
                        {
                            if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                            {
                                self.waterFriction = 0.99f;
                            }
                        }
                    }
                }
            });

            aquaCurs.GotoNext(x => x.MatchLdsfld<Player.AnimationIndex>(nameof(Player.AnimationIndex.SurfaceSwim)));
            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(
                MoveType.After,
                x => x.MatchLdcR4(0.96f),
                x => x.Match(OpCodes.Call)
                );
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    self.waterFriction = 0.999f;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            self.waterFriction = 0.999f;
                        }
                    }
                }
            });

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(2.7f));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 5f;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 5f;
                        }
                    }
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(1f));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 1.5f;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 1.5f;
                        }
                    }
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 16f;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 16f;
                        }
                    }
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 16f;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 16f;
                        }
                    }
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(1f));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 3f;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 3f;
                        }
                    }
                }
                return f;
            });

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcI4(17));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((int i, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 9;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 9;
                        }
                    }
                }
                return i;
            });

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(x => x.MatchLdarg(0));

            aquaLblCurs.Goto(aquaCurs.Next);
            aquaLblCurs.GotoNext(MoveType.After, x => x.MatchStfld<Player>(nameof(Player.waterJumpDelay)));
            ILLabel isaquaSkipLabel3 = il.DefineLabel();
            aquaLblCurs.MarkLabel(isaquaSkipLabel3);

            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return true;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return true;
                        }
                    }
                }
                return false;
            });
            aquaCurs.Emit(OpCodes.Brtrue_S, isaquaSkipLabel3);

            aquaCurs.GotoNext(x => x.MatchCallOrCallvirt<Player>("get_isRivulet"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(4f));
            aquaCurs.Emit(OpCodes.Ldarg_0);
            aquaCurs.EmitDelegate((float f, Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Aquaticpup)
                {
                    return 10f;
                }
                if (self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    foreach (var grasped in self.grasps)
                    {
                        if (grasped?.grabbed is Player pupGrabbed && pupGrabbed.slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 10f;
                        }
                    }
                }
                return f;
            });
        } // find a better way
        private void IL_Player_Collide(ILContext il)
        {
            ILCursor rotundCurs = new(il);
            ILCursor rotundLabelCurs = new(il);

            ILLabel branchLabel = il.DefineLabel();

            rotundCurs.GotoNext(x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)));
            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)), x => x.Match(OpCodes.Brfalse));
            /* GOTO BEFORE
             * if (SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && animation == AnimationIndex.Roll && gourmandAttackNegateTime <= 0)
             */
            rotundLabelCurs = rotundCurs.Clone();

            rotundLabelCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse));
            /* GOTO 
             * if (SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand >HERE< && animation == AnimationIndex.Roll && gourmandAttackNegateTime <= 0)
             */
            rotundLabelCurs.MarkLabel(branchLabel); // Mark branchLabel after 'SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand'

            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((Player self) =>    // If self is Rotundpup or Rotundpup on back, branch to branchLabel
            {
                Player pupOnBack = null;
                if (self.slugOnBack?.slugcat != null)
                {
                    pupOnBack = self.slugOnBack.slugcat;
                    while (pupOnBack.slugOnBack?.slugcat != null)
                    {
                        pupOnBack = pupOnBack.slugOnBack.slugcat;
                    }
                }
                if (self.slugcatStats.name == VariantName.Rotundpup || (pupOnBack != null && pupOnBack.slugcatStats.name == VariantName.Rotundpup))
                {
                    return true;
                }
                return false;
            });
            rotundCurs.Emit(OpCodes.Brtrue_S, branchLabel);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(4f));
            /* GOTO
             * float num = 4f;
             */
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((float f, Player self) =>   // If extra Rotundpups on back or self is Gourmand and Rotundpup on back, increase f
            {
                Player pupOnBack = null;
                if (self.slugOnBack?.slugcat != null)
                {
                    pupOnBack = self.slugOnBack.slugcat;
                    while (pupOnBack.slugOnBack?.slugcat != null)
                    {
                        if (pupOnBack.slugcatStats.name == VariantName.Rotundpup)
                        {
                            f *= 1.25f;
                        }
                        pupOnBack = pupOnBack.slugOnBack.slugcat;
                    }
                }
                if (pupOnBack != null && pupOnBack.slugcatStats.name == VariantName.Rotundpup && self.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand)
                {
                    f *= 1.35f;
                }
                return f;
            });
        } // refactor
        private void IL_Player_SlugSlamConditions(ILContext il)
        {
            ILCursor rotundLabelCurs = new(il);
            rotundLabelCurs.GotoNext(x => x.MatchLdarg(1));
            ILLabel skipLable = il.DefineLabel();
            rotundLabelCurs.MarkLabel(skipLable);

            ILCursor rotundCurs = new(il);
            rotundCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse_S));
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((Player self) =>
            {
                Player pupOnBack = null;
                if (self.slugOnBack?.slugcat != null)
                {
                    pupOnBack = self.slugOnBack.slugcat;
                    while (pupOnBack.slugOnBack?.slugcat != null)
                    {
                        pupOnBack = pupOnBack.slugOnBack.slugcat;
                    }
                }
                if (self.slugcatStats.name == VariantName.Rotundpup || (pupOnBack != null && pupOnBack.slugcatStats.name == VariantName.Rotundpup))
                {
                    return true;
                }
                return false;
            });
            rotundCurs.Emit(OpCodes.Brtrue_S, skipLable);
        } // refactor
        private void IL_Player_ClassMechanicsGourmand(ILContext il)
        {
            ILCursor rotundCurs = new(il);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Gourmand)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_000b
             * 	IL_0001: ldfld class SlugcatStats/Name Player::SlugCatClass
	         *  IL_0006: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Gourmand
	         *  IL_000b: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
             */
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((Player self) =>   // If self is Rotundpup, return true
            {
                return self.slugcatStats.name == VariantName.Rotundpup;
            });
            rotundCurs.Emit(OpCodes.Or);
        }
        private void IL_Tundra_ThrowObject(ILContext il)
        {
            ILCursor pupthrowCurs = new(il);

            ILLabel spearLabel = il.DefineLabel();
            ILLabel rockLabel = il.DefineLabel();

            pupthrowCurs.GotoNext(x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Saint)));
            pupthrowCurs.GotoNext(x => x.MatchLdarg(0));
            /* GOTO AFTER
             * if (ModManager.MSC && base.grasps[grasp].grabbed is Spear && SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint && (!ModManager.Expedition || (ModManager.Expedition && !room.game.rainWorld.ExpeditionMode)))
             */
            pupthrowCurs.MarkLabel(spearLabel);

            pupthrowCurs.GotoPrev(MoveType.Before, x => x.MatchLdarg(0));
            /* GOTO
             * if (ModManager.MSC && base.grasps[grasp].grabbed is Spear >HERE< && SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint && (!ModManager.Expedition || (ModManager.Expedition && !room.game.rainWorld.ExpeditionMode)))
             */
            pupthrowCurs.Emit(OpCodes.Ldarg_0);
            pupthrowCurs.EmitDelegate((Player self) =>  // If self is Tundrapup, branch to spearLabel
            {
                if (self.slugcatStats.name == VariantName.Tundrapup)
                {
                    return true;
                }
                return false;
            });
            pupthrowCurs.Emit(OpCodes.Brtrue_S, spearLabel);

            pupthrowCurs.GotoNext(x => x.MatchLdelemRef());
            pupthrowCurs.GotoNext(x => x.Match(OpCodes.Brfalse_S));

            rockLabel = pupthrowCurs.Next.Operand as ILLabel;

            pupthrowCurs.GotoPrev(MoveType.After, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)), x => x.Match(OpCodes.Brfalse));

            pupthrowCurs.Emit(OpCodes.Ldarg_0);
            pupthrowCurs.EmitDelegate((Player self) =>  // If self is Tundrapup, branch to rockLabel
            {
                if (self.slugcatStats.name == VariantName.Tundrapup)
                {
                    return true;
                }
                return false;
            });
            pupthrowCurs.Emit(OpCodes.Brtrue_S, rockLabel);
        } // refactor
        private void IL_Rotund_ThrowObject(ILContext il)
        {
            ILCursor pupthrowCurs = new(il);

            ILLabel rotundLabel = il.DefineLabel();

            pupthrowCurs.GotoNext(x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Gourmand)));
            pupthrowCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse_S));
            /* GOTO
             * if (ModManager.MSC && SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand >HERE< && base.grasps[grasp].grabbed is Spear)
             */
            pupthrowCurs.MarkLabel(rotundLabel);

            pupthrowCurs.GotoPrev(MoveType.After, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)), x => x.Match(OpCodes.Brfalse_S));
            /* GOTO
             * if (ModManager.MSC >HERE< && SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && base.grasps[grasp].grabbed is Spear)
             */
            pupthrowCurs.Emit(OpCodes.Ldarg_0);
            pupthrowCurs.EmitDelegate((Player self) =>  // If self is Rotundpup, branch to rotundLabel
            {
                if (self.slugcatStats.name == VariantName.Rotundpup)
                {
                    return true;
                }
                return false;
            });
            pupthrowCurs.Emit(OpCodes.Brtrue_S, rotundLabel);
        } // refactor
        private void IL_Player_EatMeatUpdate(ILContext il)
        {
            ILCursor rotundCurs = new(il);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Gourmand)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_0896
             * 	IL_0891: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Gourmand
	         *  IL_0896: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
	         *  IL_089b: brfalse.s IL_08bf
             */
            rotundCurs.Emit(OpCodes.Ldarg_0); // self
            rotundCurs.EmitDelegate((Player self) =>   // If self is Rotundpup, return true
            {
                return self.slugcatStats.name == VariantName.Rotundpup;
            });
            rotundCurs.Emit(OpCodes.Or);
            Logger.LogDebug(il.ToString());
        }
        private void IL_Player_ObjectEaten(ILContext il)
        {
            ILCursor nameCurs = new(il);

            while(nameCurs.TryGotoNext(MoveType.After, x => x.MatchLdfld<Player>(nameof(Player.SlugCatClass))))
            {
                /* WHILE TRYGOTO AFTER ldfld class SlugcatStats/Name Player::SlugCatClass
                 * 	IL_****: ldarg.0
                 *  IL_****: ldfld class SlugcatStats/Name Player::SlugCatClass
                 */
                // ldfld class SlugcatStats/Name Player::SlugCatClass => slugcatClass
                nameCurs.Emit(OpCodes.Ldarg_0); // self
                nameCurs.EmitDelegate((SlugcatStats.Name slugcatClass, Player self) =>   // If self.isSlugpup, return slugcatStats.name, else return slugcatClass
                {
                    if (self.isSlugpup)
                    {
                        return self.slugcatStats.name;
                    }
                    return slugcatClass;
                });
            }
        }
        private void IL_Player_FoodInRoom(ILContext il)
        {
            ILCursor nameCurs = new(il);

            while (nameCurs.TryGotoNext(MoveType.After, x => x.MatchLdfld<Player>(nameof(Player.SlugCatClass))))
            {
                /* WHILE TRYGOTO AFTER ldfld class SlugcatStats/Name Player::SlugCatClass
                 * 	IL_****: ldarg.0
                 *  IL_****: ldfld class SlugcatStats/Name Player::SlugCatClass
                 */
                // ldfld class SlugcatStats/Name Player::SlugCatClass => slugcatClass
                nameCurs.Emit(OpCodes.Ldarg_0); // self
                nameCurs.EmitDelegate((SlugcatStats.Name slugcatClass, Player self) =>   // If self.isSlugpup, return slugcatStats.name, else return slugcatClass
                {
                    if (self.isSlugpup)
                    {
                        return self.slugcatStats.name;
                    }
                    return slugcatClass;
                });
            }
        }
        private void IL_NPCStats_ctor(ILContext il)
        {
            ILCursor statsCurs = new(il);

            statsCurs.GotoNext(x => x.MatchStloc(0));
            statsCurs.GotoNext(MoveType.After, x => x.MatchStloc(0));
            /* GOTO AFTER IL_0036
             * 	IL_002c: isinst MoreSlugcats.PlayerNPCState
	         *  IL_0031: ldfld bool MoreSlugcats.PlayerNPCState::Malnourished
	         *  IL_0036: stloc.0
             */
            statsCurs.Emit(OpCodes.Ldarg_1); // player
            statsCurs.EmitDelegate((Player player) =>
            {
                if (SlugpupCWTs.pupStateCWT.TryGetValue(player.playerState as PlayerNPCState, out var pupNPCState))
                {
                    if (player.abstractCreature.superSizeMe)
                    {
                        player.setPupStatus(false);
                    }
                    if (player.isSlugpup && player.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
                    {
                        pupNPCState.Variant ??= GetSlugpupVariant(player);
                    }
                }
            });

            statsCurs.GotoNext(MoveType.After, x => x.MatchStfld<Player.NPCStats>(nameof(Player.NPCStats.EyeColor)));
            /* GOTO AFTER IL_01d5
             * 	IL_01cf: sub
	         *  IL_01d0: call float32 [UnityEngine.CoreModule]UnityEngine.Mathf::Pow(float32, float32)
	         *  IL_01d5: stfld float32 Player/NPCStats::EyeColor
             */
            statsCurs.Emit(OpCodes.Ldarg_0); // self
            statsCurs.Emit(OpCodes.Ldarg_1); // player
            statsCurs.EmitDelegate((Player.NPCStats self, Player player) =>
            {
                if (player.playerState is PlayerNPCState playerNPCState && SlugpupCWTs.pupStateCWT.TryGetValue(playerNPCState, out var pupNPCState))
                {
                    if (pupNPCState.Variant != null)
                    {
                        if (pupNPCState.Variant == VariantName.Hunterpup && playerNPCState.isPup)
                        {
                            self.Size = Mathf.Pow(Random.Range(0.75f, 1.75f), 1.5f);
                            self.Wideness = Mathf.Pow(Random.Range(0.5f, 1.25f), 1.5f);
                        }
                    }
                }
            });

            statsCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup)));
            /* GOTO AFTER IL_0246
             * 	IL_0246: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Slugpup
	         *  IL_024b: ldloc.0
	         *  IL_024c: newobj instance void SlugcatStats::.ctor(class SlugcatStats/Name, bool)
             */
            // IL_0246 => slugpup
            statsCurs.Emit(OpCodes.Ldarg_1); // player
            statsCurs.EmitDelegate((SlugcatStats.Name slugpup, Player player) =>
            {
                if (player.playerState is PlayerNPCState playerNPCState && SlugpupCWTs.pupStateCWT.TryGetValue(playerNPCState, out var pupNPCState))
                {
                    if (pupNPCState.Variant != null)
                    {
                        return pupNPCState.Variant;
                    }
                }
                return slugpup;
            });
        }

    }
}