using Expedition;
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
        public SlugcatStats.Name GetSlugpupVariant(EntityID entityID)
        {
            if (entityID != null && !ID_PupIDExclude().Contains(entityID.RandomSeed))
            {
                Random.InitState(entityID.RandomSeed);
                float variChance = Random.value;
                float aquaticChance = (slugpupRemix.aquaticChance.Value - slugpupRemix.tundraChance.Value) / 100f;
                float tundraChance = ((slugpupRemix.tundraChance.Value - slugpupRemix.hunterChance.Value) / 100f) + aquaticChance;
                float hunterchance = ((slugpupRemix.hunterChance.Value - slugpupRemix.rotundChance.Value) / 100f) + tundraChance;
                float rotundChance = (slugpupRemix.rotundChance.Value / 100f) + hunterchance;

                // setup variant chance
                if (variChance <= aquaticChance || ID_AquaticPupID().Contains(entityID.RandomSeed))
                {
                    return VariantName.Aquaticpup;
                }
                else if (variChance <= tundraChance || ID_TundraPupID().Contains(entityID.RandomSeed))
                {
                    return VariantName.Tundrapup;
                }
                else if (variChance <= hunterchance || ID_HunterPupID().Contains(entityID.RandomSeed))
                {
                    return VariantName.Hunterpup;
                }
                else if (variChance <= rotundChance || ID_RotundPupID().Contains(entityID.RandomSeed))
                {
                    return VariantName.Rotundpup;
                }
            }
            return null;
        }
        public void SetSlugpupPersonality(Player self)
        {
            Random.State state = Random.state;
            Random.InitState(self.abstractCreature.ID.RandomSeed);
            Random.state = state;
            self.npcStats = new(self);
            if (self.slugcatStats.name == VariantName.Aquaticpup)
            {
                // Higher Energy Calculation
                self.abstractCreature.personality.energy = Random.value;
                self.abstractCreature.personality.energy = Mathf.Clamp(Custom.PushFromHalf(self.abstractCreature.personality.energy + 0.15f + 0.05f * (1.25f * self.abstractCreature.personality.energy), 1.65f), 0f, 1f);
                // Base Personality Calculations
                self.abstractCreature.personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(self.abstractCreature.personality.energy, 1f - self.abstractCreature.personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                self.abstractCreature.personality.aggression = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery) / 2f * (1f - self.abstractCreature.personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                self.abstractCreature.personality.dominance = Mathf.Lerp(Random.value, (self.abstractCreature.personality.energy + self.abstractCreature.personality.bravery + self.abstractCreature.personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                self.abstractCreature.personality.nervous = Custom.PushFromHalf(self.abstractCreature.personality.nervous, 2.5f);
                self.abstractCreature.personality.aggression = Custom.PushFromHalf(self.abstractCreature.personality.aggression, 2.5f);
            }
            if (self.slugcatStats.name == VariantName.Tundrapup)
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

                self.tongue = new Player.Tongue(self, 0);
            }
            if (self.slugcatStats.name == VariantName.Hunterpup)
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
            if (self.slugcatStats.name == VariantName.Rotundpup)
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
        }
        public void SlugcatStats_ctor(On.SlugcatStats.orig_ctor orig, SlugcatStats self, SlugcatStats.Name slugcat, bool malnourished)
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
        public IntVector2 SlugcatStats_SlugcatFoodMeter(On.SlugcatStats.orig_SlugcatFoodMeter orig, SlugcatStats.Name slugcat)
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
        public bool SlugcatStats_HiddenOrUnplayableSlugcat(On.SlugcatStats.orig_HiddenOrUnplayableSlugcat orig, SlugcatStats.Name i)
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
        public void IL_SlugcatStats_NourishmentOfObjectEaten(ILContext il)
        {
            ILCursor variCurs = new(il);
            ILCursor tundraCurs = new(il);

            ILLabel stunLabel = il.DefineLabel();
            ILLabel critLabel = il.DefineLabel();

            variCurs.GotoNext(MoveType.Before, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC))); // Goto before 'if ModManager.MSC'

            tundraCurs.GotoNext(MoveType.Before, x => x.MatchLdarg(1)); // Goto 'eatenobject is JellyFish' and mark stunLabel at tundraCurs
            tundraCurs.MarkLabel(stunLabel);

            variCurs.Emit(OpCodes.Ldarg_0);
            variCurs.EmitDelegate((SlugcatStats.Name slugcatIndex) =>   // If slugcatIndex is Slugpup and variantName.variant is Tundrapup, branch to stunLabel
            {
                if (slugcatIndex == VariantName.Tundrapup)
                {
                    return true;
                }
                return false;
            });
            variCurs.Emit(OpCodes.Brtrue_S, stunLabel);

            variCurs.GotoNext(MoveType.Before, x => x.MatchLdarg(0), x => x.MatchLdsfld<SlugcatStats.Name>(nameof(SlugcatStats.Name.Red))); // Goto before 'if slugcatIndex == Name.Red'

            ILCursor hunterCurs = variCurs.Clone();
            hunterCurs.GotoNext(MoveType.Before, x => x.MatchLdcI4(1)); // Goto 'eatenobject is Centipede' and mark critLabel at hunterCurs
            hunterCurs.MarkLabel(critLabel);

            variCurs.Emit(OpCodes.Ldarg_0);
            variCurs.EmitDelegate((SlugcatStats.Name slugcatIndex) =>   // If slugcatIndex is Slugpup and variantname.variant is Hunterpup, branch to critLabel
            {
                if (slugcatIndex == VariantName.Hunterpup)
                {
                    return true;
                }
                return false;
            });
            variCurs.Emit(OpCodes.Brtrue_S, critLabel);
        }
        public void IL_NPCStats_ctor(ILContext il)
        {
            ILCursor statsCurs = new(il);
            statsCurs.GotoNext(MoveType.After, x => x.MatchStloc(1));
            /* GOTO AFTER
             * SlugcatStats slugcatStats = new SlugcatStats(MoreSlugcatsEnums.SlugcatStatsName.Slugpup, malnourished);
             */
            statsCurs.Emit(OpCodes.Ldarg_0);
            statsCurs.Emit(OpCodes.Ldarg_1); // player
            statsCurs.Emit(OpCodes.Ldloc_0); // malnourished
            statsCurs.Emit(OpCodes.Ldloc_1); // slugcatStats
            statsCurs.EmitDelegate((Player.NPCStats self, Player player, bool malnourished, SlugcatStats slugcatStats) =>   // Set the pup's variant and return new SlugcatStats to slugcatStats
            {
                if (pearlCat)
                {
                    if (IsPearlpup(player))
                    {
                        return slugcatStats;
                    }
                }
                SlugcatStats.Name variant = GetSlugpupVariant(player.abstractCreature.ID);
                if (SlugpupCWTs.pupStateCWT.TryGetValue(player.playerState as PlayerNPCState, out var pupNPCState))
                {
                    if (pupNPCState.Variant != null)
                    {
                        variant = pupNPCState.Variant;
                    }
                }
                if (variant != null)
                {
                    Random.State state = Random.state;
                    Random.InitState(player.abstractCreature.ID.RandomSeed);
                    Random.state = state;
                    if (variant == VariantName.Hunterpup)
                    {
                        self.Size = Mathf.Pow(Random.Range(0.75f, 1.75f), 1.5f);
                        self.Wideness = Mathf.Pow(Random.Range(0.5f, 1.25f), 1.5f);
                    }
                    return new SlugcatStats(variant, malnourished);
                }
                return slugcatStats;
            });
            statsCurs.Emit(OpCodes.Stloc_1);
        }
        public void IL_Player_ClassMechanicsGourmand(ILContext il)
        {
            ILCursor rotundCurs = new(il);
            ILCursor labelCurs = new(il);
            
            ILLabel pupLabel = il.DefineLabel();

            labelCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse_S));
            /* GOTO
             * 	if (SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Gourmand && >HERE< (double)aerobicLevel >= 0.95)
             */
            labelCurs.MarkLabel(pupLabel); // Mark pupLabel at (double)aerobicLevel >= 0.95

            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((Player self) =>   // If self.slugcatStats.name is Rotundpup or Rotundpup is on back, branch to pupLabel
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
            rotundCurs.Emit(OpCodes.Brtrue_S, pupLabel);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            /* GOTO
             * slowMovementStun = Math.Max(slowMovementStun, (int)Custom.LerpMap(aerobicLevel, 0.7f, 0.4f, >HERE< 6f, 0f));
             */
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((float f, Player self) =>   // If gourmandExhausted and no Rotundpup on back, return 3f, else return 6f
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
                if ((pupOnBack == null || pupOnBack != null && pupOnBack.slugcatStats.name != VariantName.Rotundpup) && self.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Gourmand || self.slugcatStats.name != VariantName.Rotundpup)
                {
                    return 3f;
                }
                return f;
            });
        }
        public void VariantMechanicsAquaticpup(Player self)
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
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    parent = self.grabbedBy[0].grabber as Player;
                }
                if (parent != null && parent.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    self.slowMovementStun = 5;
                }
            }
        }
        public void Player_VariantMechanicsTundrapup(On.Player.orig_ClassMechanicsSaint orig, Player self)
        {
            Player parent = null;
            if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
            {
                parent = self.grabbedBy[0].grabber as Player;
            }
            if (self.onBack != null && slugpupRemix.BackTundraGrapple.Value)
            {
                parent = self.onBack.slugOnBack.owner;
                while (parent.onBack != null)
                {
                    parent = parent.onBack.slugOnBack.owner;
                }
            }
            if (self.slugcatStats.name == VariantName.Tundrapup && parent == null)
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
            else if (parent != null && (self.slugcatStats.name == VariantName.Tundrapup || (self.slugcatStats.name == MoreSlugcatsEnums.SlugcatStatsName.Saint && self.playerState.isPup && self.onBack != null)))
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
                orig(self);
            }
        }
        public bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
        {
            if ((self.slugcatStats.name == VariantName.Hunterpup || self.slugcatStats.name == VariantName.Rotundpup) && crit is not Player)
            {
                return true;
            }
            return orig(self, crit);
        }
        public bool Player_AllowGrabbingBatflys(On.Player.orig_AllowGrabbingBatflys orig, Player self)
        {
            if (self.isNPC)
            {
                if (self.slugcatStats.name == VariantName.Tundrapup)
                {
                    return false;
                }
            }
            return orig(self);
        }
        public bool Player_SaintTongueCheck(On.Player.orig_SaintTongueCheck orig, Player self)
        {
            if (self.Consious && self.slugcatStats.name == VariantName.Tundrapup && self.tongue.mode == Player.Tongue.Mode.Retracted && self.bodyMode != Player.BodyModeIndex.CorridorClimb && !self.corridorDrop && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab)
            {
                return true;
            }
            return orig(self);
        }
        public void Player_TongueUpdate(On.Player.orig_TongueUpdate orig, Player self)
        {
            Player parent = null;
            if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
            {
                parent = self.grabbedBy[0].grabber as Player;
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
                if (parent.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Artificer && parent.input[0].jmp && parent.input[1].pckp) self.tongue.Release();
                self.tongue.baseChunk = parent.bodyChunks[0];
                if (self.tongue.Attached)
                {
                    self.tongueAttachTime++;
                    if (self.Stunned)
                    {
                        if (RainWorld.ShowLogs)
                        {
                            Debug.Log("Tongue stun detatch?");
                        }

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
                                if (self.AI?.behaviorType != SlugNPCAI.BehaviorType.BeingHeld)
                                {
                                    if (parent.grasps[0] != null && parent.HeavyCarry(parent.grasps[0].grabbed) && !(parent.grasps[0].grabbed is Cicada))
                                    {
                                        num += Mathf.Min(Mathf.Max(0f, self.onBack.slugOnBack.owner.grasps[0].grabbed.TotalMass - 0.2f) * 1.5f, 1.3f);
                                    }
                                }
                                parent.bodyChunks[0].vel.y = 6f * num;
                                parent.bodyChunks[1].vel.y = 5f * num;
                                parent.jumpBoost = 6.5f;
                                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
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
        public void Tongue_Shoot(On.Player.Tongue.orig_Shoot orig, Player.Tongue self, Vector2 dir)
        {
            Player parent = null;
            if (self.player.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
            {
                parent = self.player.grabbedBy[0].grabber as Player;
            }
            if (self.player.onBack != null)
            {
                parent = self.player.onBack.slugOnBack.owner;
                while (parent.onBack != null)
                {
                    parent = parent.onBack.slugOnBack.owner;
                }
            }
            if (parent != null)
            {
                self.resetRopeLength();
                if (self.Attached)
                {
                    self.Release();
                }
                else if (!slugpupRemix.SaintTundraGrapple.Value && parent.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint)
                {
                    return;
                }
                else if (self.mode == Player.Tongue.Mode.Retracted)
                {
                    self.mode = Player.Tongue.Mode.ShootingOut;
                    self.player.room.PlaySound(SoundID.Tube_Worm_Shoot_Tongue, self.baseChunk);
                    float num = parent.input[0].x;
                    float num2 = parent.input[0].y;
                    if (self.isZeroGMode() && (num != 0f || num2 != 0f))
                    {
                        dir = new Vector2(num, num2).normalized;
                        parent.bodyChunks[0].vel = new Vector2(self.GetTargetZeroGVelo(parent.bodyChunks[0].vel.x, dir.x), self.GetTargetZeroGVelo(parent.bodyChunks[0].vel.y, dir.y));
                        parent.bodyChunks[1].vel = new Vector2(self.GetTargetZeroGVelo(parent.bodyChunks[1].vel.x, dir.x), self.GetTargetZeroGVelo(parent.bodyChunks[1].vel.y, dir.y));
                    }
                    else
                    {
                        dir = self.AutoAim(dir);
                    }

                    self.pos = self.baseChunk.pos + dir * 5f;
                    self.vel = dir * 70f;
                    self.elastic = 1f;
                    self.requestedRopeLength = 140f;
                    self.returning = false;
                }
            }
            else
            {
                orig(self, dir);
            }
        }
        public SlugNPCAI.Food SlugNPCAI_GetFoodType(On.MoreSlugcats.SlugNPCAI.orig_GetFoodType orig, SlugNPCAI self, PhysicalObject food)
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
        public float SlugNPCAI_LethalWeaponScore(On.MoreSlugcats.SlugNPCAI.orig_LethalWeaponScore orig, SlugNPCAI self, PhysicalObject obj, Creature target)
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
        public bool SlugNPCAI_TheoreticallyEatMeat(On.MoreSlugcats.SlugNPCAI.orig_TheoreticallyEatMeat orig, SlugNPCAI self, Creature crit, bool excludeCentipedes)
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
        public bool SlugNPCAI_WantsToEatThis(On.MoreSlugcats.SlugNPCAI.orig_WantsToEatThis orig, SlugNPCAI self, PhysicalObject obj)
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
        public bool SlugNPCAI_HasEdible(On.MoreSlugcats.SlugNPCAI.orig_HasEdible orig, SlugNPCAI self)
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
        public void IL_Player_Jump(ILContext il)
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
        }
        public void IL_Player_UpdateAnimation(ILContext il)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                            if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
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
                        if (grasped?.grabbed is Player && (grasped.grabbed as Player).slugcatStats.name == VariantName.Aquaticpup)
                        {
                            return 10f;
                        }
                    }
                }
                return f;
            });
        }
        public void IL_Player_Collide(ILContext il)
        {
            ILCursor rotundCurs = new(il);
            rotundCurs.GotoNext(x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)));
            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)), x => x.Match(OpCodes.Brfalse));

            ILCursor rotundLabelCurs = rotundCurs.Clone();
            rotundLabelCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse));
            ILLabel skipLabel = il.DefineLabel();
            rotundLabelCurs.MarkLabel(skipLabel);

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
            rotundCurs.Emit(OpCodes.Brtrue_S, skipLabel);
        }
        public void IL_Player_SlugSlamConditions(ILContext il)
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
        }
        public void IL_Player_ThrowObject(ILContext il)
        {
            ILCursor pupthrowCurs = new(il);
            pupthrowCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)), x => x.Match(OpCodes.Brfalse_S));

            ILCursor rotundLabelCurs = pupthrowCurs.Clone();
            rotundLabelCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse_S));
            ILLabel skipLabel = il.DefineLabel();
            rotundLabelCurs.MarkLabel(skipLabel);

            pupthrowCurs.Emit(OpCodes.Ldarg_0);
            pupthrowCurs.EmitDelegate((Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Rotundpup)
                {
                    return true;
                }
                return false;
            });
            pupthrowCurs.Emit(OpCodes.Brtrue_S, skipLabel);

            pupthrowCurs.GotoNext(x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)));
            pupthrowCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)), x => x.Match(OpCodes.Brfalse_S));
            pupthrowCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse_S));

            ILCursor tundraLabelCurs = pupthrowCurs.Clone();
            tundraLabelCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse_S));
            ILLabel skipLabel2 = il.DefineLabel();
            tundraLabelCurs.MarkLabel(skipLabel2);

            pupthrowCurs.Emit(OpCodes.Ldarg_0);
            pupthrowCurs.EmitDelegate((Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Tundrapup)
                {
                    return true;
                }
                return false;
            });
            pupthrowCurs.Emit(OpCodes.Brtrue_S, skipLabel2);

            pupthrowCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<ModManager>(nameof(ModManager.MSC)), x => x.Match(OpCodes.Brfalse));

            tundraLabelCurs = pupthrowCurs.Clone();
            tundraLabelCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brfalse_S));
            ILLabel skipLabel3 = il.DefineLabel();
            tundraLabelCurs.MarkLabel(skipLabel3);

            pupthrowCurs.Emit(OpCodes.Ldarg_0);
            pupthrowCurs.EmitDelegate((Player self) =>
            {
                if (self.slugcatStats.name == VariantName.Tundrapup)
                {
                    return true;
                }
                return false;
            });
            pupthrowCurs.Emit(OpCodes.Brtrue_S, skipLabel3);
        }
        public void IL_Player_ObjectEaten(ILContext il)
        {
            ILCursor nameCurs = new(il);

            while(nameCurs.TryGotoNext(MoveType.After, x => x.MatchLdfld<Player>(nameof(Player.SlugCatClass))))
            {
                nameCurs.Emit(OpCodes.Ldarg_0);
                nameCurs.EmitDelegate((SlugcatStats.Name SlugCatClass, Player self) =>
                {
                    if (self.isSlugpup)
                    {
                        return self.slugcatStats.name;
                    }
                    return SlugCatClass;
                });
            }
        }
        public void IL_Player_FoodInRoom(ILContext il)
        {
            ILCursor nameCurs = new(il);

            while (nameCurs.TryGotoNext(MoveType.After, x => x.MatchLdfld<Player>(nameof(Player.SlugCatClass))))
            {
                nameCurs.Emit(OpCodes.Ldarg_0);
                nameCurs.EmitDelegate((SlugcatStats.Name SlugCatClass, Player self) =>
                {
                    if (self.isSlugpup)
                    {
                        return self.slugcatStats.name;
                    }
                    return SlugCatClass;
                });
            }
        }
        public void PlayerNPCState_ctor(On.MoreSlugcats.PlayerNPCState.orig_ctor orig, PlayerNPCState self, AbstractCreature abstractCreature, int playerNumber)
        {
            orig(self, abstractCreature, playerNumber);
            if (!SlugpupCWTs.pupStateCWT.TryGetValue(self, out _))
            {
                SlugpupCWTs.pupStateCWT.Add(self, _ = new SlugpupCWTs.PupNPCState());
            }
        }
        public string PlayerNPCState_ToString(On.MoreSlugcats.PlayerNPCState.orig_ToString orig, PlayerNPCState self)
        {
            string text = orig(self);
            if (self.player.realizedCreature is Player pup && SlugpupCWTs.pupStateCWT.TryGetValue(self, out var pupNPCState))
            {
                text += "Variant<cC>" + ((pup.slugcatStats.name != MoreSlugcatsEnums.SlugcatStatsName.Slugpup) ? pup.slugcatStats.name.value : "NULL") + "<cB>";
                text += "PupsPlusStomach<cC>" + ((pupNPCState.PupsPlusStomachObject != null) ? pupNPCState.PupsPlusStomachObject.ToString() : "NULL") + "<cB>";
            }
            return text;

        }
        public void PlayerNPCState_LoadFromString(On.MoreSlugcats.PlayerNPCState.orig_LoadFromString orig, PlayerNPCState self, string[] s)
        {
            orig(self, s);
            if (SlugpupCWTs.pupStateCWT.TryGetValue(self, out var pupNPCState))
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
                                _ => null
                            };
                            break;
                        case "PupsPlusStomach":
                            string text = array[1];
                            if (text != "NULL")
                            {
                                if (text.Contains("<oA>"))
                                {
                                    pupNPCState.PupsPlusStomachObject = SaveState.AbstractPhysicalObjectFromString(self.player.Room.world, text); ;
                                }
                                else if (text.Contains("<cA>"))
                                {
                                    pupNPCState.PupsPlusStomachObject = SaveState.AbstractCreatureFromString(self.player.Room.world, text, onlyInCurrentRegion: false);
                                }
                            }
                            break;
                    }
                }
            }
            self.unrecognizedSaveStrings.Remove("Variant");
            self.unrecognizedSaveStrings.Remove("PupsPlusStomach");
        }
        public void IL_PlayerNPCState_CycleTick(ILContext il)
        {
            ILCursor foodCurs = new(il);

            while (foodCurs.TryGotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup))))
            {
                foodCurs.Emit(OpCodes.Ldarg_0);
                foodCurs.EmitDelegate((SlugcatStats.Name slugcat, PlayerNPCState self) =>
                {
                    if (SlugpupCWTs.pupStateCWT.TryGetValue(self, out var pupNPCState))
                    {
                        SlugcatStats.Name variant = pupNPCState.Variant;
                        if (variant != null)
                        {
                            return variant;
                        }
                    }
                    return slugcat;
                });
            }
        }




        public static bool IsPearlpup(Player player) => Pearlcat.Hooks.IsPearlpup(player);
    }
}