using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static SlugpupStuff.SlugpupCWTs;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace SlugpupStuff
{
    public partial class SlugpupStuff   // Slugpup Variants, Variant Stats, and Variant Abilities
    {
        public static class VariantName
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
        public static List<int> ID_AquaticPupID()
        {
            List<int> idlist = [];
            return idlist;
        }
        public static List<int> ID_TundraPupID()
        {
            List<int> idlist = [];
            return idlist;

        }
        public static List<int> ID_HunterPupID()
        {
            List<int> idlist =
            [
                1002
            ];
            return idlist;
        }
        public static List<int> ID_RotundPupID()
        {
            List<int> idlist = [];
            return idlist;
        }
        public static List<int> ID_PupIDExclude()
        {
            List<int> idlist =
            [
                1000,
                1001,
                2220,
                3118,
                4118,
                765
            ];
            return idlist;
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
        private void Player_ClassMechanicsSaint(On.Player.orig_ClassMechanicsSaint orig, Player self)
        {
            if (self.isTundrapup())
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
                if (self.SaintTongueCheck())
                {
                    if (parent != null)
                    {
                        if (parent.input[0].jmp && !parent.input[1].jmp && !parent.input[0].pckp && parent.canJump <= 0 && parent.bodyMode != Player.BodyModeIndex.Crawl && parent.animation != Player.AnimationIndex.ClimbOnBeam && parent.animation != Player.AnimationIndex.AntlerClimb && parent.animation != Player.AnimationIndex.HangFromBeam)
                        {
                            Vector2 vector = new(parent.flipDirection, 0.7f);
                            Vector2 normalized = vector.normalized;
                            if (parent.input[0].y > 0)
                            {
                                normalized = new Vector2(0f, 1f);
                            }
                            normalized = (normalized + parent.mainBodyChunk.vel.normalized * 0.2f).normalized;
                            self.tongue.Shoot(normalized);
                        }
                    }
                    else
                    {
                        if (self.input[0].jmp && !self.input[1].jmp && !self.input[0].pckp && self.canJump <= 0 && self.bodyMode != Player.BodyModeIndex.Crawl && self.animation != Player.AnimationIndex.ClimbOnBeam && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.HangFromBeam)
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
            }
            else
            {
                orig(self);
            }
        }
        private bool Player_SlugSlamConditions(On.Player.orig_SlugSlamConditions orig, Player self, PhysicalObject otherObject)
        {
            if (self.isNPC && otherObject is Player)
            {
                return false;
            }
            return orig(self, otherObject);
        }
        private void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (self.isTundrapup())
            {
                spear.spearDamageBonus = 0.2f + 0.3f * Mathf.Pow(Random.value, 6f);
            }
            if (self.isRotundpup() && !self.gourmandExhausted)
            {
                if (SimplifiedMovesetGourmand)
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
            if (self.isTundrapup())
            {
                return false;
            }
            if (self.isHunterpup() || self.isRotundpup())
            {
                if (!crit.dead)
                {
                    return false;
                }
                if (self.EatMeatOmnivoreGreenList(crit))
                {
                    return true;
                }
                if (crit is IPlayerEdible or Player)
                { 
                    return false;
                }
                return true;
            }
            return orig(self, crit);
        }
        private bool Player_AllowGrabbingBatflys(On.Player.orig_AllowGrabbingBatflys orig, Player self)
        {
            if (self.isNPC && self.isTundrapup())
            {
                return false;
            }
            return orig(self);
        }
        private bool Player_SaintTongueCheck(On.Player.orig_SaintTongueCheck orig, Player self)
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
                if (self.isTundrapup())
                {
                    if (self.Consious)
                    {
                        if (self.tongue.mode == Player.Tongue.Mode.Retracted && self.bodyMode != Player.BodyModeIndex.CorridorClimb && !self.corridorDrop && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab)
                        {
                            if (parent != null)
                            {
                                if (parent.bodyMode != Player.BodyModeIndex.CorridorClimb && !parent.corridorDrop && parent.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && parent.bodyMode != Player.BodyModeIndex.WallClimb && parent.bodyMode != Player.BodyModeIndex.Swimming && parent.animation != Player.AnimationIndex.VineGrab && parent.animation != Player.AnimationIndex.ZeroGPoleGrab)
                                {
                                    return true;
                                }
                                return false;
                            }
                            return true;
                        }
                    }
                    return false;
                }
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
                self.resetRopeLength();
                if (self.Attached)
                {
                    self.Release();
                }
                else if (self.mode == Player.Tongue.Mode.Retracted)
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
                    if (parent != null)
                    {
                        if (!slugpupRemix.SaintTundraGrapple.Value && parent.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Saint) return;

                        if (slugpupRemix.OneGrapple.Value)
                        {
                            if (self.player.onBack != null && self.player.onBack.slugOnBack.owner.onBack != null)
                            {
                                return;
                            }
                            if (self.player.grabbedBy.Count <= 0)
                            {
                                foreach (var grasped in parent.grasps)
                                {
                                    if (grasped?.grabbed is Player pup && pup.isNPC && pup.isTundrapup()) return;
                                }
                            }
                        }
                    }
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
            if (self.isTundrapup())
            {
                if (obj is Spear)
                {
                    if (slugpupRemix.TundraViolence.Value)
                    {
                        return 0.05f;
                    }
                    return 0f;
                }
            }
            return orig(self, obj, target);
        }
        private bool SlugNPCAI_TheoreticallyEatMeat(On.MoreSlugcats.SlugNPCAI.orig_TheoreticallyEatMeat orig, SlugNPCAI self, Creature crit, bool excludeCentipedes)
        {
            if (self.isTundrapup())
            {
                if (self.TryGetPupVariables(out var pupVariables))
                {
                    if (crit.abstractCreature == pupVariables.giftedItem && crit.dead)
                    {
                        return true;
                    }
                }
                return false;
            }
            if (self.isHunterpup() || self.isRotundpup())
            {
                if (crit is Player)
                {
                    return false;
                }
                if (crit.dead && crit.State.meatLeft > 0)
                {
                    return true;
                }
            }
            return orig(self, crit, excludeCentipedes);
        }
        private bool SlugNPCAI_WantsToEatThis(On.MoreSlugcats.SlugNPCAI.orig_WantsToEatThis orig, SlugNPCAI self, PhysicalObject obj)
        {
            if (self.isAquaticpup())
            {
                if (obj is WaterNut)
                {
                    return !self.IsFull;
                }
            }
            return orig(self, obj);

        }
        private bool SlugNPCAI_HasEdible(On.MoreSlugcats.SlugNPCAI.orig_HasEdible orig, SlugNPCAI self)
        {
            if (self.isTundrapup() && self.TryGetPupVariables(out var pupVariables))
            {
                if (orig(self) && self.cat.grasps[0].grabbed is Creature or JellyFish)
                {
                    if (self.cat.grasps[0].grabbed.abstractPhysicalObject == pupVariables.giftedItem)
                    {
                        return true;
                    }
                    return false;
                }
            }
            if (self.isAquaticpup())
            {
                if (orig(self) && self.cat.grasps[0].grabbed is WaterNut)
                {
                    return false;
                }
            }
            return orig(self);
        }
        private SlugNPCAI.Food SlugNPCAI_GetFoodType(On.MoreSlugcats.SlugNPCAI.orig_GetFoodType orig, SlugNPCAI self, PhysicalObject food)
        {
            if (self.isAquaticpup())
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

            aquaCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isSlugpup"));
            /* GOTO AFTER IL_01ad
             * 	IL_01ac: ldarg.0
	         *  IL_01ad: call instance bool Player::get_isSlugpup()
	         *  IL_01b2: brfalse.s IL_01c0
             */
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((Player self) =>   // If self is aquaticpup, return false
            {
                return !self.isAquaticpup();
            });
            aquaCurs.Emit(OpCodes.And);

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(2.25f));
            /* GOTO AFTER IL_02a0
             * 	IL_029e: br.s IL_02a5
		     *  IL_02a0: ldc.r4 2.25
		     *  IL_02a5: add
             */
            // ldc.r4 2.25 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 3.25f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 3.25f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(1f));
            /* GOTO AFTER IL_02ca
             * 	IL_02c8: br.s IL_02cf
		     *  IL_02ca: ldc.r4 1
		     *  IL_02cf: add
             */
            // ldc.r4 1 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 1.5f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 1.5f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isSlugpup"));
            /* GOTO AFTER IL_048e
             *	IL_048d: ldarg.0
	         *  IL_048e: call instance bool Player::get_isSlugpup()
	         *  IL_0493: brfalse.s IL_050b
             */
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((Player self) =>   // If self is aquaticpup, return false
            {
                return !self.isAquaticpup();
            });
            aquaCurs.Emit(OpCodes.And);

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(0.65f));
            /* GOTO AFTER IL_06fd
             *	IL_06f6: ldc.r4 1
	         *  IL_06fb: br.s IL_0702
	         *  IL_06fd: ldc.r4 0.65
             */
            // ldc.r4 0.65 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 0.8f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 0.8f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(0.65f));
            /* GOTO AFTER IL_069f
	         *  IL_0698: ldc.r4 1
	         *  IL_069d: br.s IL_06a4
	         *  IL_069f: ldc.r4 0.65
             */
            // ldc.r4 0.65 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 0.8f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 0.8f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(1f));
            /* GOTO AFTER IL_0728
	         *	IL_0726: brtrue.s IL_072f
	         *  IL_0728: ldc.r4 1
	         *  IL_072d: br.s IL_0734
             */
            // ldc.r4 1 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 1.2f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 1.2f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(1f));
            /* GOTO AFTER IL_0752
	         *	IL_0750: brtrue.s IL_0759
	         *  IL_0752: ldc.r4 1
	         *  IL_0757: br.s IL_075e
             */
            // ldc.r4 1 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 1.2f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 1.2f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            /* GOTO AFTER IL_0816
	         *	IL_0814: brfalse.s IL_081d
	         *  IL_0816: ldc.r4 6
	         *  IL_081b: stloc.s 4
             */
            // ldc.r4 6 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 9f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 9f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(6f));
            /* GOTO AFTER IL_0bf5
	         *	IL_0bf3: brfalse.s IL_0bfc
	         *  IL_0bf5: ldc.r4 6
	         *  IL_0bfa: stloc.s 9
             */
            // ldc.r4 6 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 9f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 9f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isSlugpup"));
            /* GOTO AFTER IL_0f2f
             *	IL_0f2e: ldarg.0
	         *  IL_0f2f: call instance bool Player::get_isSlugpup()
	         *  IL_0f34: brtrue.s IL_0f39
             */
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((Player self) =>   // If self is aquaticpup, return false
            {
                return !self.isAquaticpup();
            });
            aquaCurs.Emit(OpCodes.And);

            aquaCurs.GotoNext(x => x.MatchCall<Player>("get_isSlugpup"));
            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(3f));
            /* GOTO AFTER IL_140e
	         *	IL_140d: ldarg.0
	         *  IL_140e: ldc.r4 3
	         *  IL_1413: stfld float32 Player::jumpBoost
             */
            // ldc.r4 3 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 6.5f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 6.5f;
                }
                return f;
            });

            aquaCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isSlugpup"));
            /* GOTO AFTER IL_14df
             *	IL_14de: ldarg.0
	         *  IL_14df: call instance bool Player::get_isSlugpup()
	         *  IL_14e4: brtrue.s IL_14e9
             */
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((Player self) =>   // If self is aquaticpup, return false
            {
                return !self.isAquaticpup();
            });
            aquaCurs.Emit(OpCodes.And);

            aquaCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(5.5f));
            /* GOTO AFTER IL_15bf
	         *	IL_15bd: brfalse.s IL_15c6
	         *  IL_15bf: ldc.r4 5.5
	         *  IL_15c4: stloc.s 13
             */
            // ldc.r4 5.5 => f
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((float f, Player self) =>   // If self is aquaticpup, return 7.5f, else return f
            {
                if (self.isAquaticpup())
                {
                    return 7.5f;
                }
                return f;
            });

        }
        private void IL_Player_UpdateAnimation(ILContext il)
        {
            ILCursor swimCurs = new(il);

            swimCurs.GotoNext(x => x.MatchLdsfld<Player.AnimationIndex>(nameof(Player.AnimationIndex.DeepSwim)));
            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_24f4
             * 	IL_24f3: ldarg.0
	         *  IL_24f4: call instance bool Player::get_isRivulet()
	         *  IL_24f9: brfalse.s IL_2503
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_258e
             * 	IL_258d: ldarg.0
	         *  IL_258e: call instance bool Player::get_isRivulet()
	         *  IL_2593: brfalse.s IL_25e8
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup, return true
            {
                if (self.isAquaticpup())
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            ILLabel burstLabel = il.DefineLabel();
            swimCurs.GotoNext(x => x.Match(OpCodes.Br));
            swimCurs.GotoNext(MoveType.After, x => x.MatchBr(out burstLabel), x => x.MatchLdarg(0)); // Get out IL_2683 as burstLabel
            /* GOTO AFTER IL_263c
             * 	IL_2635: stfld valuetype [UnityEngine.CoreModule]UnityEngine.Vector2 BodyChunk::vel
	         *  IL_263a: br.s IL_2683
	         *  IL_263c: ldarg.0
             */
            // ldarg.0 => self
            swimCurs.Emit(OpCodes.Ldloc, 10); // vector
            swimCurs.Emit(OpCodes.Ldloc, 11); // num3
            swimCurs.EmitDelegate((Player self, Vector2 vector, float num3) =>   // If self is not Rivulet and holding aquaticpup, add burst velocity and branch to burstLabel
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (!self.isRivulet && pupGrabbed != null && pupGrabbed.isAquaticpup())
                {
                    self.bodyChunks[0].vel += vector * ((vector.y > 0.5f) ? 300f : 50f);
                    self.airInLungs -= 0.08f * num3;
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Brtrue_S, burstLabel);
            swimCurs.Emit(OpCodes.Ldarg_0);

            swimCurs.GotoNext(MoveType.After, x => x.MatchStfld<Player>(nameof(Player.waterJumpDelay)));
            /* GOTO AFTER IL_26d4
             * 	IL_26d0: br.s IL_26d4
	         *  IL_26d2: ldc.i4.s 10
	         *  IL_26d4: stfld int32 Player::waterJumpDelay
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    self.waterJumpDelay = 12;
                }
            });

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_294b
	         *  IL_294a: ldarg.0
	         *  IL_294b: call instance bool Player::get_isRivulet()
	         *  IL_2950: brfalse.s IL_296d
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_2d46
	         *  IL_2d45: ldarg.0
	         *  IL_2d46: call instance bool Player::get_isRivulet()
	         *  IL_2d4b: brfalse.s IL_2d63
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_2e27
	         *  IL_2e26: ldarg.0
	         *  IL_2e27: call instance bool Player::get_isRivulet()
	         *  IL_2e2c: brfalse.s IL_2e44
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_2e27
	         *  IL_3007: ldarg.0
	         *  IL_3008: call instance bool Player::get_isRivulet()
	         *  IL_300d: brtrue.s IL_3016
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_303d
	         *  IL_303c: ldarg.0
	         *  IL_303d: call instance bool Player::get_isRivulet()
	         *  IL_3042: brtrue.s IL_304b
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(18f));
            /* GOTO AFTER IL_3134
             * 	IL_3132: br.s IL_3139
	         *  IL_3134: ldc.r4 18
	         *  IL_3139: add
             */
            //ldc.r4 => f
            swimCurs.Emit(OpCodes.Ldarg_0);
            swimCurs.EmitDelegate((float f, Player self) =>
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return 16f;
                }
                return f;
            });

            swimCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(18f));
            /* GOTO AFTER IL_315e
	         *  IL_315c: br.s IL_3163
	         *  IL_315e: ldc.r4 18
	         *  IL_3163: add
             */
            //ldc.r4 => f
            swimCurs.Emit(OpCodes.Ldarg_0);
            swimCurs.EmitDelegate((float f, Player self) =>
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return 16f;
                }
                return f;
            });

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_303d
	         *  IL_316a: ldarg.0
	         *  IL_316b: call instance bool Player::get_isRivulet()
	         *  IL_3170: brtrue.s IL_3179
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchLdcI4(6));
            /* GOTO AFTER IL_3213
	         *  IL_3211: br.s IL_3214
	         *  IL_3213: ldc.i4.6
	         *  IL_3214: stfld int32 Player::waterJumpDelay
             */
            //ldc.i4 => i
            swimCurs.Emit(OpCodes.Ldarg_0);
            swimCurs.EmitDelegate((int i, Player self) =>
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return 9;
                }
                return i;
            });

            swimCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_3226
	         *  IL_3225: ldarg.0
	         *  IL_3226: call instance bool Player::get_isRivulet()
	         *  IL_322b: brtrue.s IL_3235
             */
            swimCurs.Emit(OpCodes.Ldarg_0); // self
            swimCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return true;
                }
                return false;
            });
            swimCurs.Emit(OpCodes.Or);

            swimCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(12f));
            /* GOTO AFTER IL_315e
	         *  IL_3255: br.s IL_325c
	         *  IL_3257: ldc.r4 12
	         *  IL_325c: newobj instance void [UnityEngine.CoreModule]UnityEngine.Vector2::.ctor(float32, float32)
             */
            //ldc.r4 => f
            swimCurs.Emit(OpCodes.Ldarg_0);
            swimCurs.EmitDelegate((float f, Player self) =>
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && (pupGrabbed != null && pupGrabbed.isAquaticpup()))
                {
                    return 10f;
                }
                return f;
            });
        }
        private void IL_Player_LungUpdate(ILContext il)
        {
            ILCursor aquaCurs = new(il);

            aquaCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isRivulet"));
            /* GOTO AFTER IL_0465
	         *  IL_0464: ldarg.0
	         *  IL_0465: call instance bool Player::get_isRivulet()
	         *  IL_046a: brfalse.s IL_046e
             */
            aquaCurs.Emit(OpCodes.Ldarg_0); // self
            aquaCurs.EmitDelegate((Player self) =>   // If self is aquaticpup or holding aquaticpup, return true
            {
                Player pupGrabbed = null;
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed is Player pup && pup.isNPC)
                    {
                        pupGrabbed = pup;
                        break;
                    }
                }
                if (self.isAquaticpup() || !self.isRivulet && pupGrabbed != null && pupGrabbed.isAquaticpup())
                {
                    return true;
                }
                return false;
            });
            aquaCurs.Emit(OpCodes.Or);
        }
        private void IL_Player_Collide(ILContext il)
        {
            ILCursor rotundCurs = new(il);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Gourmand)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_021b
             * 	IL_0216: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Gourmand
	         *  IL_021b: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
	         *  IL_0220: brfalse IL_0497
             */

            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((Player self) =>    // If self is Rotundpup or Rotundpup on back, return true
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
                if (self.isRotundpup() || (pupOnBack != null && pupOnBack.isRotundpup()))
                {
                    return true;
                }
                return false;
            });
            rotundCurs.Emit(OpCodes.Or);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdcR4(4f));
            /* GOTO AFTER IL_04a3
             * 	IL_049e: brfalse IL_0909
	         *  IL_04a3: ldc.r4 4
	         *  IL_04a8: stloc.s 7
             */
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((float f, Player self) =>   // If extra Rotundpups on back, increase f
            {
                Player pupOnBack = null;
                if (self.slugOnBack?.slugcat != null)
                {
                    pupOnBack = self.slugOnBack.slugcat;
                    while (pupOnBack.slugOnBack?.slugcat != null)
                    {
                        if (pupOnBack.isRotundpup())
                        {
                            f *= 0.8f + pupOnBack.mainBodyChunk.mass;
                        }
                        pupOnBack = pupOnBack.slugOnBack.slugcat;
                    }
                }
                return f;
            });
        }
        private void IL_Player_SlugSlamConditions(ILContext il)
        {
            ILCursor rotundCurs = new(il);

            ILLabel slamLabel = il.DefineLabel();
            rotundCurs.GotoNext(MoveType.After, x => x.MatchBrfalse(out slamLabel)); // Get out IL_0014 as slamLabel
            /* GOTO IL_0010
             * 	IL_0006: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Gourmand
	         *  IL_000b: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
	         *  IL_0010: brfalse IL_0014
             */
            rotundCurs.Emit(OpCodes.Ldarg_0); // self
            rotundCurs.EmitDelegate((Player self) =>    // If self is Rotundpup or Rotundpup on back, branch to slamLabel
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
                if (self.isRotundpup() || (pupOnBack != null && pupOnBack.isRotundpup()))
                {
                    return false;
                }
                return true;
            });
            rotundCurs.Emit(OpCodes.Brfalse_S, slamLabel);
        }
        private void IL_Player_ClassMechanicsGourmand(ILContext il)
        {
            if (!SimplifiedMovesetGourmand)
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
                    return self.isRotundpup();
                });
                rotundCurs.Emit(OpCodes.Or);
            }
        }
        private void IL_Player_ThrowObject(ILContext il)
        {
            ILCursor rotundCurs = new(il);
            ILCursor tundraCurs = new(il);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Gourmand)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_0031
	         *  IL_002c: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Gourmand
	         *  IL_0031: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
	         *  IL_0036: brfalse.s IL_0059
             */
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((Player self) =>    // If self is Rotundpup, return true
            {
                if (self.isRotundpup())
                {
                    return true;
                }
                return false;
            });
            rotundCurs.Emit(OpCodes.Or);

            if (!EmeraldsTweaks)
            {
                while(tundraCurs.TryGotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Saint)), x => x.Match(OpCodes.Call)))
                {
                    /* WHILE TRYGOTO AFTER call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
                     *  IL_****: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Gourmand
                     *  IL_****: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
                     *  IL_****: brfalse.s IL_****
                     */
                    tundraCurs.Emit(OpCodes.Ldarg_0);
                    tundraCurs.EmitDelegate((Player self) =>    // If self is Tundrapup, return true
                    {
                        if (self.isTundrapup())
                        {
                            return !slugpupRemix.TundraViolence.Value;
                        }
                        return false;
                    });
                    tundraCurs.Emit(OpCodes.Or);
                }
            }
        } 
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
                return self.isRotundpup();
            });
            rotundCurs.Emit(OpCodes.Or);
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
                // ldfld class SlugcatStats/Name Player::SlugCatClass => SlugCatClass
                nameCurs.Emit(OpCodes.Ldarg_0); // self
                nameCurs.EmitDelegate((SlugcatStats.Name SlugCatClass, Player self) =>   // If self.isSlugpup, return slugcatStats.name, else return SlugCatClass
                {
                    if (self.isSlugpup)
                    {
                        return self.slugcatStats.name;
                    }
                    return SlugCatClass;
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
                // ldfld class SlugcatStats/Name Player::SlugCatClass => SlugCatClass
                nameCurs.Emit(OpCodes.Ldarg_0); // self
                nameCurs.EmitDelegate((SlugcatStats.Name SlugCatClass, Player self) =>   // If self.isSlugpup, return slugcatStats.name, else return SlugCatClass
                {
                    if (self.isSlugpup)
                    {
                        return self.slugcatStats.name;
                    }
                    return SlugCatClass;
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
                if (player.playerState.TryGetPupState(out var pupNPCState))
                {
                    if (player.isSlugpup && player.abstractCreature.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
                    {
                        pupNPCState.Variant ??= player.GetSlugpupVariant();
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

                if (player.playerState.TryGetPupState(out var pupNPCState))
                {
                    if (player.abstractCreature.superSizeMe) player.playerState.forceFullGrown = true;

                    Random.State state = Random.state;
                    Random.InitState(player.abstractCreature.ID.RandomSeed);
                    if (pupNPCState.Variant != null)
                    {
                        AbstractCreature.Personality personality = player.abstractCreature.personality;
                        if (pupNPCState.Variant == VariantName.Aquaticpup)
                        {
                            self.Bal = Mathf.Lerp(Random.Range(0f, 0.2f), 1f, self.Bal);
                            self.Met = Mathf.Lerp(Random.Range(0.1f, 0.3f), 1f, self.Met);

                            // Higher Energy
                            //      increased by higher metabolism, and lower size
                            //      decreased by lower balance
                            personality.energy = Mathf.Clamp01(Mathf.Pow(personality.energy + Random.Range(0f, 0.25f), 0.5f + 0.1f * (1f - self.Met) + 0.15f * self.Size + 0.1f * (1f - self.Bal)));

                            // Base Personality Calculations
                            personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(personality.energy, 1f - personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                            personality.aggression = Mathf.Lerp(Random.value, (personality.energy + personality.bravery) / 2f * (1f - personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                            personality.dominance = Mathf.Lerp(Random.value, (personality.energy + personality.bravery + personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                            personality.nervous = Custom.PushFromHalf(personality.nervous, 2.5f);
                            personality.aggression = Custom.PushFromHalf(personality.aggression, 2.5f);
                        }
                        if (pupNPCState.Variant == VariantName.Tundrapup)
                        {
                            self.Size = Mathf.Lerp(0f, Random.Range(0.7f, 1f), self.Size);
                            self.Stealth = Mathf.Lerp(Random.Range(0f, 0.3f), 1f, self.Stealth);

                            // Higher Nervousness
                            //      increased by lower color luminosity, and lower size
                            //      decreased by higher stealth
                            personality.nervous = Mathf.Clamp01(Mathf.Pow(personality.nervous + Random.Range(0f, 0.3f), 0.4f + 0.15f * self.Size + 0.15f * self.L + 0.1f * self.Stealth));

                            // Higher Sympathy
                            //      increased by lower size
                            //      decreased by higher metabolism
                            personality.sympathy = Mathf.Clamp01(Mathf.Pow(personality.sympathy + Random.Range(0f, 0.2f), 0.5f + 0.15f * self.Size + 0.15f * self.Met));

                            // Lower Aggression
                            //      increased by higher size, and higher metabolism
                            //      decreased by higher stealth
                            personality.aggression = Mathf.Clamp01(Mathf.Pow(personality.aggression - Random.Range(0f, 0.15f), 1.35f + 0.15f * (1f - self.Size) + 0.1f * (1f - self.Met) + 0.15f * self.Stealth));
                            if (float.IsNaN(personality.aggression)) personality.aggression = 0f;

                            // Base Personality Calculations
                            personality.dominance = Mathf.Lerp(Random.value, (personality.energy + personality.bravery + personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                        }
                        if (pupNPCState.Variant == VariantName.Hunterpup && (player.playerState.isPup || !player.playerState.forceFullGrown))
                        {
                            self.Size = Mathf.Lerp(0.5f, Random.Range(1.5f, 1.8f), self.Size);
                            self.Wideness = Mathf.Lerp(0.35f, Random.Range(1f, 1.3f), self.Wideness);

                            // Higher Bravery
                            //      increased by higher stealth, and higher wideness
                            //      decreased by lower size
                            personality.bravery = Mathf.Clamp01(Mathf.Pow(personality.bravery + Random.Range(0f, 0.3f), 0.5f + 0.15f * (1f - self.Stealth) + 0.1f * (1f - self.Wideness) + 0.15f * (1f - self.Size)));

                            // Higher Aggression
                            //      increased by higher metabolism, and higher size
                            //      decreased by higher stealth
                            personality.aggression = Mathf.Clamp01(Mathf.Pow(personality.aggression + Random.Range(0f, 0.2f), 0.6f + 0.1f * (1f - self.Met) + 0.15f * (1f - self.Size) + 0.15f * self.Stealth));

                            // Base Personality Calculations
                            personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(personality.energy, 1f - personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                            personality.dominance = Mathf.Lerp(Random.value, (personality.energy + personality.bravery + personality.aggression) / 3f, Mathf.Pow(Random.value, 0.25f));
                            personality.nervous = Custom.PushFromHalf(personality.nervous, 2.5f);
                        }
                        if (pupNPCState.Variant == VariantName.Rotundpup)
                        {
                            self.Wideness = Mathf.Lerp(0.5f, Random.Range(1.65f, 1.8f), self.Wideness);
                            self.Met = Mathf.Lerp(0f, Random.Range(0.7f, 0.9f), self.Met);

                            // Lower Energy
                            //      increased by lower metabolism
                            //      decreased by lower balance, and higher wideness
                            personality.energy = Mathf.Clamp01(Mathf.Pow(personality.energy - Random.Range(0f, 0.1f), 1.2f + 0.1f * (1f - self.Met) + 0.1f * (1f - self.Bal) + 0.15f * self.Wideness));
                            if (float.IsNaN(personality.energy)) personality.energy = 0f;

                            // Higher Dominance
                            //      increased by higher size, high wideness
                            //      decreased by lower widness
                            personality.dominance = Mathf.Clamp01(Mathf.Pow(personality.dominance + Random.Range(0f, 0.25f), 0.4f + 0.1f * (1f - self.Size) + 0.2f * (1f - self.Wideness)));

                            // Base Personality Calculations
                            personality.nervous = Mathf.Lerp(Random.value, Mathf.Lerp(personality.energy, 1f - personality.bravery, 0.5f), Mathf.Pow(Random.value, 0.25f));
                            personality.aggression = Mathf.Lerp(Random.value, (personality.energy + personality.bravery) / 2f * (1f - personality.sympathy), Mathf.Pow(Random.value, 0.25f));
                            personality.nervous = Custom.PushFromHalf(personality.nervous, 2.5f);
                            personality.aggression = Custom.PushFromHalf(personality.aggression, 2.5f);
                        }
                        player.abstractCreature.personality = personality;
                    }
                    Random.state = state;
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
                if (player.playerState.TryGetPupState(out var pupNPCState))
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