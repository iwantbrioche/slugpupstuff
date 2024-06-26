﻿using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
using Custom = RWCustom.Custom;
using static SlugpupStuff.SlugpupCustom;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.Drawing;
using HUD;


namespace SlugpupStuff
{
    [BepInPlugin(MOD_ID, "Slugpup Stuff", "1.2.15")]
    public partial class SlugpupStuff : BaseUnityPlugin
    {
        public const string MOD_ID = "iwantbread.slugpupstuff";
        private bool IsInit;
        private bool PostIsInit;
        public static SlugpupStuffRemix slugpupRemix;
        public static float aquaticChance => (slugpupRemix.aquaticChance.Value - slugpupRemix.tundraChance.Value) / 100f;
        public static float tundraChance => ((slugpupRemix.tundraChance.Value - slugpupRemix.hunterChance.Value) / 100f) + aquaticChance;
        public static float hunterchance => ((slugpupRemix.hunterChance.Value - slugpupRemix.rotundChance.Value) / 100f) + tundraChance;
        public static float rotundChance => (slugpupRemix.rotundChance.Value / 100f) + hunterchance;
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
            slugpupRemix = new SlugpupStuffRemix(this, Logger);
        }
        public void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;

                if (ModManager.ActiveMods.Any(mod => mod.id == "emeralds_features"))
                {
                    EmeraldsTweaks = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "SimplifiedMoveset"))
                {
                    SimplifiedMovesetGourmand = PupsPlusModCompat.SimplifiedMovesetGourmand();
                }

                MachineConnector.SetRegisteredOI(MOD_ID, slugpupRemix);
                VariantName.RegisterValues();
                SlugpupGraphics.Patch(Logger);

                // Slugpup OnHooks
                On.MoreSlugcats.SlugNPCAI.ctor += SlugNPCAI_ctor;
                On.MoreSlugcats.SlugNPCAI.Update += SlugNPCAI_Update;
                On.MoreSlugcats.SlugNPCAI.TheoreticallyEatMeat += SlugNPCAI_TheoreticallyEatMeat;
                On.MoreSlugcats.SlugNPCAI.WantsToEatThis += SlugNPCAI_WantsToEatThis;
                On.MoreSlugcats.SlugNPCAI.HasEdible += SlugNPCAI_HasEdible;
                On.MoreSlugcats.SlugNPCAI.LethalWeaponScore += SlugNPCAI_LethalWeaponScore;
                On.MoreSlugcats.SlugNPCAI.GetFoodType += SlugNPCAI_GetFoodType;
                On.MoreSlugcats.SlugNPCAI.Move += SlugNPCAI_Move;
                On.MoreSlugcats.SlugNPCAI.DecideBehavior += SlugNPCAI_DecideBehavior;
                On.MoreSlugcats.SlugNPCAI.SocialEvent += SlugNPCAI_SocialEvent;
                On.MoreSlugcats.SlugNPCAI.TravelPreference += SlugNPCAI_TravelPreference;

                // Slugpup ILHooks
                IL.MoreSlugcats.SlugNPCAI.ctor += IL_SlugNPCAI_ctor;

                // Player OnHooks
                On.Player.ctor += Player_ctor;
                On.Player.UpdateMSC += Player_UpdateMSC;
                On.Player.AllowGrabbingBatflys += Player_AllowGrabbingBatflys;
                On.Player.CanEatMeat += Player_CanEatMeat;
                On.Player.SaintTongueCheck += Player_SaintTongueCheck;
                On.Player.ClassMechanicsSaint += Player_ClassMechanicsSaint;
                On.Player.SlugSlamConditions += Player_SlugSlamConditions;
                On.Player.GrabUpdate += Player_GrabUpdate;
                On.Player.TongueUpdate += Player_TongueUpdate;
                On.Player.Tongue.increaseRopeLength += Tongue_increaseRopeLength;
                On.Player.Tongue.decreaseRopeLength += Tongue_decreaseRopeLength;
                On.Player.ThrownSpear += Player_ThrownSpear;

                // Player ILHooks
                IL.Player.ctor += IL_Player_ctor;
                IL.Player.Update += IL_Player_Update;
                IL.Player.SwallowObject += IL_Player_SwallowObject;
                IL.Player.Regurgitate += IL_Player_Regurgitate;
                IL.Player.GrabUpdate += IL_Player_GrabUpdate;
                IL.Player.Jump += IL_Player_Jump;
                IL.Player.UpdateAnimation += IL_Player_UpdateAnimation;
                IL.Player.LungUpdate += IL_Player_LungUpdate;
                IL.Player.SlugSlamConditions += IL_Player_SlugSlamConditions;
                IL.Player.Collide += IL_Player_Collide;
                IL.Player.ClassMechanicsGourmand += IL_Player_ClassMechanicsGourmand;
                IL.Player.ThrowObject += IL_Player_ThrowObject;
                IL.Player.StomachGlowLightColor += IL_Player_StomachGlowLightColor;
                IL.Player.EatMeatUpdate += IL_Player_EatMeatUpdate;
                IL.Player.ObjectEaten += IL_Player_ObjectEaten;
                IL.Player.FoodInRoom_Room_bool += IL_Player_FoodInRoom;
                IL.Player.SetMalnourished += IL_Player_SetMalnourished;
                IL.Player.NPCStats.ctor += IL_NPCStats_ctor;

                // Other OnHooks
                On.SlugcatStats.ctor += SlugcatStats_ctor;
                On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
                On.SlugcatStats.HiddenOrUnplayableSlugcat += SlugcatStats_HiddenOrUnplayableSlugcat;
                On.Player.Tongue.Shoot += Tongue_Shoot;
                On.MoreSlugcats.PlayerNPCState.ToString += PlayerNPCState_ToString;
                On.MoreSlugcats.PlayerNPCState.LoadFromString += PlayerNPCState_LoadFromString;
                On.AbstractCreature.setCustomFlags += AbstractCreature_setCustomFlags;

                // Other ILHooks
                IL.Snail.Click += IL_Snail_Click;
                IL.SlugcatStats.NourishmentOfObjectEaten += IL_SlugcatStats_NourishmentOfObjectEaten;
                IL.RegionState.AdaptRegionStateToWorld += IL_RegionState_AdaptRegionStateToWorld;
                IL.MoreSlugcats.PlayerNPCState.CycleTick += IL_PlayerNPCState_CycleTick;

                IsInit = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Pups+ failed to load!");
                Logger.LogError(ex);
                throw;
            }
        }



        public void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (PostIsInit) return;

                if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
                {
                    PupsPlusModCompat.SetupDMSSprites();
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "yeliah.slugpupFieldtrip"))
                {
                    SlugpupSafari = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "rgbpups"))
                {
                    RainbowPups = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "pearlcat"))
                {
                    Pearlcat = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "NoirCatto.BeastMasterPupExtras"))
                {
                    BeastMasterPupExtras = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "slime-cubed.devconsole"))
                {
                    PupsPlusModCompat.RegisterSpawnPupCommand();
                    Logger.LogInfo("spawn_pup command registered");
                    //PupsPlusModCompat.RegisterPupsPlusDebugCommands();
                }

                PostIsInit = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Pups+ PostModsInit failed to load!");
                Logger.LogError(ex);
                throw;
            }
        }

        // Hooks

        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.isNPC && self.isSlugpup && self.playerState.TryGetPupState(out var pupNPCState))
            {
                if (pupNPCState.Variant != null)
                {
                    Custom.Log($"{self} variant set to: {pupNPCState.Variant}");
                }
            }
            if (self.isTundrapup())
            {
                self.tongue = new Player.Tongue(self, 0);
            }
        }
        private void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            Player pupGrabbed = null;
            int grabbedIndex = -1;
            foreach (var grasped in self.grasps)
            {
                if (grasped?.grabbed is Player pup && pup.isNPC)
                {
                    pupGrabbed = pup;
                    break;
                }
            }
            if (pupGrabbed != null)
            {
                foreach (var grasped in self.grasps)
                {
                    if (grasped?.grabbed != null && grasped.grabbed != pupGrabbed)
                    {
                        grabbedIndex = grasped.graspUsed;
                        break;
                    }
                }
                if (self.input[0].pckp && self.input[0].y == 1)
                {
                    if (pupGrabbed.grasps[0] != null && SlugpupSafari)
                    {
                        orig(self, eu);
                        return;
                    }
                    if (self.slugOnBack != null) self.slugOnBack.increment = false;
                    if (self.spearOnBack != null) self.spearOnBack.increment = false;

                    if (grabbedIndex > -1 && self.grasps[grabbedIndex].grabbed != null)
                    {
                        pupGrabbed.PupSwallowObject(grabbedIndex);
                    }
                    else if (pupGrabbed.objectInStomach != null || pupGrabbed.isRotundpup() && slugpupRemix.ManualItemGen.Value)
                    {
                        pupGrabbed.PupRegurgitate();
                    }
                    else
                    {
                        orig(self, eu);
                    }
                }
                else
                {
                    if (pupGrabbed.swallowAndRegurgitateCounter > 0)
                    {
                        pupGrabbed.swallowAndRegurgitateCounter--;
                    }
                    orig(self, eu);
                }
            }
            else
            {
                orig(self, eu);
            }
        }
        private void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
        {
            orig(self);
            self.VariantMechanicsAquaticpup();
            if (slugpupRemix.RotundBackExaustion.Value) self.VariantMechanicsRotundpup();
        }
        private void IL_Player_ctor(ILContext il)
        {
            ILCursor pupCurs = new(il);

            pupCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>(nameof(Player.SetMalnourished)));
            /* GOTO AFTER IL_0a7f
	         *  IL_0a7c: br.s IL_0a7f
	         *  IL_0a7e: ldc.i4.1
	         *  IL_0a7f: call instance void Player::SetMalnourished(bool)
             */
            pupCurs.Emit(OpCodes.Ldarg_0);
            pupCurs.Emit(OpCodes.Ldarg_1);
            pupCurs.EmitDelegate((Player self, AbstractCreature abstractCreature) =>   // If game session is StoryGameSession and PupsPlusStomachObject is not null, set objectInStomach to PupsPlusStomachObject
            {
                if (self.playerState.TryGetPupState(out var pupNPCState))
                {
                    if (abstractCreature.Room?.world?.game?.session != null && abstractCreature.Room.world.game.session is StoryGameSession)
                    {
                        if (pupNPCState?.PupsPlusStomachObject != null)
                        {
                            self.objectInStomach = pupNPCState.PupsPlusStomachObject;
                            self.objectInStomach.pos = abstractCreature.pos;
                        }
                        else
                        {
                            self.objectInStomach = null;
                        }
                    }
                }
            });
        }
        private void IL_Player_Update(ILContext il)
        {
            ILCursor exhaustCurs = new(il);

            exhaustCurs.GotoNext(MoveType.After, x => x.MatchLdfld<Player>(nameof(Player.gourmandExhausted)));
            /* GOTO AFTER ldfld bool Player::gourmandExhausted
             * 	ldarg.0
	         *  ldfld bool Player::gourmandExhausted
	         *  brtrue.s IL_0c80
             */
            exhaustCurs.Emit(OpCodes.Ldarg_0);
            exhaustCurs.EmitDelegate((Player self) =>   // If pup is on back and pup is Rotundpup, return true
            {
                if (self.TryGetParentVariables(out var parentVariables))
                {
                    return parentVariables.rotundPupExhaustion;
                }
                return false;
            });
            exhaustCurs.Emit(OpCodes.Or);

            exhaustCurs.GotoNext(MoveType.After, x => x.MatchLdfld<Player>(nameof(Player.gourmandExhausted)));
            /* GOTO AFTER ldfld bool Player::gourmandExhausted
             * 	ldarg.0
	         *  ldfld bool Player::gourmandExhausted
	         *  brtrue.s IL_0d4e
             */
            exhaustCurs.Emit(OpCodes.Ldarg_0);
            exhaustCurs.EmitDelegate((Player self) =>   // If pup is on back and pup is Rotundpup, return true
            {
                if (self.TryGetParentVariables(out var parentVariables))
                {
                    return parentVariables.rotundPupExhaustion;
                }
                return false;
            });
            exhaustCurs.Emit(OpCodes.Or);
        }
        private void IL_Player_SwallowObject(ILContext il)
        {
            ILCursor parentCurs = new(il);
            ILCursor graspedCurs = new(il);

            parentCurs.GotoNext(MoveType.After, x => x.MatchStloc(0));
            /* GOTO AFTER IL_0021
	            IL_0016: ldelem.ref
	            IL_0017: ldfld class PhysicalObject Creature/Grasp::grabbed
	            IL_001c: ldfld class AbstractPhysicalObject PhysicalObject::abstractPhysicalObject
	            IL_0021: stloc.0
            */
            ILLabel graspedLabel = parentCurs.MarkLabel();

            parentCurs.Emit(OpCodes.Ldloc_0); // abstractPhysicalObject
            parentCurs.Emit(OpCodes.Ldarg_0); // self
            parentCurs.Emit(OpCodes.Ldarg_1); // grasp
            parentCurs.EmitDelegate((AbstractPhysicalObject abstractPhysicalObject, Player self, int grasp) =>  // If grabbed by a player, set abstractPhysicalObject to object in the player's grasps[grasp]
            {
                if (self.isNPC && self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player parent)
                {
                    return parent.grasps[grasp].grabbed.abstractPhysicalObject;
                }
                return abstractPhysicalObject;
            });
            parentCurs.Emit(OpCodes.Stloc_0);

            // graspedCurs is at IL_0000
            graspedCurs.Emit(OpCodes.Ldarg_0); // self
            graspedCurs.EmitDelegate((Player self) =>   // If grabbed by a player, branch to graspedLabel
            {
                if (self.isNPC && self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player parent)
                {
                    return true;
                }
                return false;
            });
            graspedCurs.Emit(OpCodes.Brtrue_S, graspedLabel);

            graspedCurs.GotoNext(MoveType.After, x => x.MatchCallvirt<Creature>(nameof(Creature.ReleaseGrasp)));
            /* GOTO AFTER IL_007d
                IL_007b: ldarg.0
                IL_007c: ldarg.1
                IL_007d: callvirt instance void Creature::ReleaseGrasp(int32)
            */
            ILLabel releaseLabel = graspedCurs.MarkLabel();

            parentCurs.GotoNext(MoveType.After, x => x.MatchCallvirt<StoryGameSession>(nameof(StoryGameSession.RemovePersistentTracker)), x => x.MatchLdarg(0));
            /* GOTO AFTER IL_007b
	            IL_0071: ldfld class AbstractPhysicalObject Player::objectInStomach
	            IL_0076: callvirt instance void StoryGameSession::RemovePersistentTracker(class AbstractPhysicalObject)
	            IL_007b: ldarg.0
            */
            parentCurs.Emit(OpCodes.Ldarg_1); 
            parentCurs.EmitDelegate((Player self, int grasp) =>   // If grabbed by a player, make the player drop the object in their grasps[grasp]
            {
                if (self.isNPC && self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player parent)
                {
                    parent.ReleaseGrasp(grasp);
                    return true;
                }
                return false;
            });
            parentCurs.Emit(OpCodes.Brtrue_S, releaseLabel);
            parentCurs.Emit(OpCodes.Ldarg_0);
        }
        private void IL_Player_Regurgitate(ILContext il)
        {
            ILCursor parentCurs = new(il);
            ILCursor rotundCurs = new(il);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isGourmand"));
            /* GOTO AFTER IL_0062
             * 	IL_0061: ldarg.0
	         *  IL_0062: call instance bool Player::get_isGourmand()
	         *  IL_0067: brtrue.s IL_006a
             */
            rotundCurs.Emit(OpCodes.Ldarg_0); // self
            rotundCurs.EmitDelegate((Player self) =>   // If self is Rotundpup, return true
            {
                return self.isRotundpup();
            });
            rotundCurs.Emit(OpCodes.Or);

            ILLabel handLabel = il.DefineLabel();
            parentCurs.GotoNext(MoveType.After, x => x.MatchLdloc(2), x => x.MatchBrfalse(out handLabel)); // Get out IL_04c2 as handLabel
            /* GOTO AFTER IL_0428
             * 	IL_0427: ldloc.2
	         *  IL_0428: brfalse IL_04c2
             */
            parentCurs.Emit(OpCodes.Ldarg_0);
            parentCurs.EmitDelegate((Player self) =>   // If grabbed by a player, make player grab regurgitated object and branch to handLabel
            {
                if (self.isNPC && self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player parent)
                {
                    if (parent != null && parent.FreeHand() > -1)
                    {
                        parent.SlugcatGrab(self.objectInStomach.realizedObject, parent.FreeHand());
                        return true;
                    }
                }
                return false;
            });
            parentCurs.Emit(OpCodes.Brtrue_S, handLabel);
        }
        private void IL_Player_GrabUpdate(ILContext il)
        {
            ILCursor counterCurs = new(il);

            ILLabel branchLabel = il.DefineLabel();

            counterCurs.GotoNext(x => x.MatchStfld<PlayerGraphics>(nameof(PlayerGraphics.swallowing)));
            counterCurs.GotoNext(x => x.MatchLdcI4(0), x => x.MatchStfld<Player>(nameof(Player.swallowAndRegurgitateCounter)));
            counterCurs.GotoPrev(x => x.MatchBr(out branchLabel)); // Get IL_1aeb as branchlabel
            /* GOTO IL_1ae2
             * 	IL_1ae2: br.s IL_1aeb
	         *  IL_1ae4: ldarg.0
	         *  IL_1ae5: ldc.i4.0
             */
            counterCurs.GotoNext(MoveType.After, x => x.MatchLdarg(0));
            /* GOTO AFTER IL_1ae4
             * 	IL_1ae4: ldarg.0
	         *  IL_1ae5: ldc.i4.0
	         *  IL_1ae6: stfld int32 Player::swallowAndRegurgitateCounter
             */
            counterCurs.EmitDelegate((Player self) =>   // If pupVariables.regurgitating or pupVariables.swallowing, branch to branchLabel
            {
                if (self.TryGetPupVariables(out var pupVariables))
                {
                    return pupVariables.regurgitating || pupVariables.swallowing;
                }
                return false;
            });
            counterCurs.Emit(OpCodes.Brtrue_S, branchLabel);
            counterCurs.Emit(OpCodes.Ldarg_0);
        }
        private void IL_Player_StomachGlowLightColor(ILContext il)
        {
            ILCursor glowCurs = new(il);

            glowCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_0001
             * 	IL_0000: ldarg.0
	         *  IL_0001: call instance class MoreSlugcats.SlugNPCAI Player::get_AI()
	         *  IL_0006: brtrue.s IL_0011 
             */
            glowCurs.Emit(OpCodes.Pop);   // Pop IL_0001 off the stack
            glowCurs.Emit(OpCodes.Ldc_I4_0);   // Emit false into IL_0011 branch
        }
        private void IL_Player_SetMalnourished(ILContext il)
        {
            ILCursor variCurs = new(il);

            variCurs.GotoNext(MoveType.After, x => x.MatchLdfld<Player>(nameof(Player.SlugCatClass)));
            /* GOTO AFTER ldfld class SlugcatStats/Name Player::SlugCatClass
             * 	ldarg.0
	         *  ldfld class SlugcatStats/Name Player::SlugCatClass
	         *  ldarg.1
             */
            //ldfld class SlugcatStats/Name Player::SlugCatClass => slugpup
            variCurs.Emit(OpCodes.Ldarg_0); // self
            variCurs.EmitDelegate((SlugcatStats.Name slugpup, Player self) =>
            {
                if (self.playerState.TryGetPupState(out var pupNPCState))
                {
                    if (pupNPCState.Variant != null)
                    {
                        return pupNPCState.Variant;
                    }
                }
                return slugpup;
            });
        }
        private void SlugNPCAI_ctor(On.MoreSlugcats.SlugNPCAI.orig_ctor orig, SlugNPCAI self, AbstractCreature creature, World world)
        {
            orig(self, creature, world);
            if (self.TryGetPupVariables(out var pupVariables))
            {
                //pupVariables.pathingVisualizer = new(self, 5);
                //pupVariables.labelManager = new(self.cat);
            }
  
        }
        private void SlugNPCAI_Move(On.MoreSlugcats.SlugNPCAI.orig_Move orig, SlugNPCAI self)
        {
            orig(self);
            if (self.cat.gourmandExhausted)
            {
                if (!self.OnAnyBeam())
                {
                    self.cat.input[0].jmp = false;
                }
                if (self.cat.input[0].x == 0 && self.cat.input[0].y == 0 && !self.cat.input[0].pckp && self.FunStuff)
                {
                    if (Random.value < Mathf.Lerp(0f, 0.3f, Mathf.InverseLerp(0.1f, 0f, self.creature.personality.energy)))
                    {
                        self.cat.standing = false;
                    }
                }
            }
            if (self.isTundrapup())
            {
                if (self.stuckTracker.Utility() > 0.1f)
                {
                    if (self.cat.tongue.Attached && Random.Range(0f, 1f) < Random.Range(0f, 0.6f * self.stuckTracker.Utility()))
                    {
                        self.cat.input[0].jmp = Random.Range(0, 2) == 0;
                    }
                }
            }
            if (self.TryGetPupVariables(out var pupVariables))
            {
                pupVariables.pathingVisualizer?.VisualizeConnections();
            }

        }
        private void SlugNPCAI_Update(On.MoreSlugcats.SlugNPCAI.orig_Update orig, SlugNPCAI self)
        {
            orig(self);
            if (self.TryGetPupVariables(out var pupVariables))
            {
                if (self.nap)
                {
                    if (Mathf.Clamp01(0.06f / self.creature.personality.energy) > Random.Range(0.35f, 1f) || self.cat.emoteSleepCounter > 1.4f)
                    {
                        self.cat.emoteSleepCounter += Mathf.Clamp(0.0008f / self.creature.personality.energy, 0.0008f, 0.05f);
                        if (self.cat.emoteSleepCounter > 1.4f)
                        {
                            if (self.cat.graphicsModule != null)
                            {
                                (self.cat.graphicsModule as PlayerGraphics).blink = 5;
                            }
                            self.cat.sleepCurlUp = Mathf.SmoothStep(self.cat.sleepCurlUp, 1f, self.cat.emoteSleepCounter - 1.4f);
                        }
                        else
                        {
                            self.cat.sleepCurlUp = Mathf.Max(0f, self.cat.sleepCurlUp - 0.1f);
                        }
                    }
                }
                else
                {
                    self.cat.emoteSleepCounter = 0f;
                }
                if (self.isRotundpup())
                {
                    if (self.foodReaction < -110 && self.FunStuff && self.cat.objectInStomach == null && !pupVariables.regurgitating)
                    {
                        pupVariables.wantsToRegurgitate = true;
                    }
                }
                if (pupVariables.wantsToRegurgitate)
                {
                    self.cat.PupRegurgitate();
                }
                if (pupVariables.wantsToSwallowObject && self.cat.grasps[0]?.grabbed != null)
                {
                    self.cat.PupSwallowObject(0);
                }
                if (pupVariables.giftedItem != null)
                {
                    bool giftedTracked = false;
                    foreach(var rep in self.itemTracker.items)
                    {
                        if (rep.representedItem != pupVariables.giftedItem) continue;
                        giftedTracked = true;
                    }
                    if (pupVariables.giftedItem.realizedObject == null || !giftedTracked)
                    {
                        pupVariables.giftedItem = null;
                    }
                }
                if (self.behaviorType == SlugNPCAI.BehaviorType.OnHead || self.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    if (self.cat.grasps[0]?.grabbed != null)
                    {
                        if (self.cat.Grabability(self.cat.grasps[0].grabbed) > Player.ObjectGrabability.TwoHands)
                        {
                            self.cat.ReleaseGrasp(0);
                        }
                    }
                }

                //pupVariables.pathingVisualizer?.Update();
                //if (pupVariables.labelManager != null)
                //{
                //    pupVariables.labelManager.UpdateLabel("grabTarget", $"grabTarget: {(self.grabTarget != null ? self.grabTarget is Creature ? (self.grabTarget as Creature).abstractCreature.creatureTemplate.type : self.grabTarget.abstractPhysicalObject.type : "NULL")}", self.grabTarget != null);
                //    pupVariables.labelManager.UpdateLabel("giftedItem", $"giftedItem: {(pupVariables.giftedItem != null ? pupVariables.giftedItem is AbstractCreature ? (pupVariables.giftedItem as AbstractCreature).creatureTemplate.type : pupVariables.giftedItem.type : "NULL")}", pupVariables.giftedItem != null);
                //    pupVariables.labelManager.Update(self.cat.mainBodyChunk.pos + new Vector2(35f, 30f));
                //}
            }
        }
        private void SlugNPCAI_DecideBehavior(On.MoreSlugcats.SlugNPCAI.orig_DecideBehavior orig, SlugNPCAI self)
        {
            orig(self);
            if (self.behaviorType == SlugNPCAI.BehaviorType.Attacking && self.cat.gourmandExhausted)
            {
                self.behaviorType = SlugNPCAI.BehaviorType.Fleeing;
            }
        }
        private void SlugNPCAI_SocialEvent(On.MoreSlugcats.SlugNPCAI.orig_SocialEvent orig, SlugNPCAI self, SocialEventRecognizer.EventID ID, Creature subjectCrit, Creature objectCrit, PhysicalObject involvedItem)
        {
            if (ID == SocialEventRecognizer.EventID.ItemTransaction && objectCrit == self.cat)
            {
                if (self.TryGetPupVariables(out var pupVariables))
                {
                    pupVariables.giftedItem = involvedItem.abstractPhysicalObject;
                }
            }
                orig(self, ID, subjectCrit, objectCrit, involvedItem);
        }
        private void IL_SlugNPCAI_ctor(ILContext il)
        {
            ILCursor itemTrackerCurs = new(il);
            itemTrackerCurs.GotoNext(MoveType.Before, x => x.MatchNewobj<ItemTracker>());
            /* GOTO BEFORE IL_007d
             *	IL_007b: ldc.i4.m1
	         *  IL_007c: ldc.i4.1
	         *  IL_007d: newobj instance void ItemTracker::.ctor(class ArtificialIntelligence, int32, int32, int32, int32, bool)
	         *  IL_0082: call instance void ArtificialIntelligence::AddModule(class AIModule)
             */
            itemTrackerCurs.Emit(OpCodes.Pop);
            itemTrackerCurs.Emit(OpCodes.Ldc_I4_0); // Switch stopTrackingCarried to false
        }
        private PathCost SlugNPCAI_TravelPreference(On.MoreSlugcats.SlugNPCAI.orig_TravelPreference orig, SlugNPCAI self, MovementConnection coord, PathCost cost)
        {
            PathCost origCost = orig(self, coord, cost);
            if (self.behaviorType != SlugNPCAI.BehaviorType.Fleeing)
            {
                origCost = cost;
                if (self.cat.gourmandExhausted)
                {
                    origCost += new PathCost(50f, PathCost.Legality.Unallowed);
                }
            }
            return origCost;
        }
        private string PlayerNPCState_ToString(On.MoreSlugcats.PlayerNPCState.orig_ToString orig, PlayerNPCState self)
        {
            string text = orig(self);
            if (self.player.realizedCreature is Player pup && pup.playerState.TryGetPupState(out var pupNPCState))
            {
                text += "Variant<cC>" + ((pup.slugcatStats.name != MoreSlugcatsEnums.SlugcatStatsName.Slugpup) ? pup.slugcatStats.name.value : "NULL") + "<cB>";
                text += "PupsPlusStomach<cC>" + ((pupNPCState.PupsPlusStomachObject != null) ? pupNPCState.PupsPlusStomachObject.ToString() : "NULL") + "<cB>";
            }
            return text;

        }
        private void PlayerNPCState_LoadFromString(On.MoreSlugcats.PlayerNPCState.orig_LoadFromString orig, PlayerNPCState self, string[] s)
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
                            if (BeastMasterPupExtras && !array[1].Equals("Slugpup") && self.player.TryGetPupAbstract(out var pupAbstract))
                            {
                                pupAbstract.regular = true;
                            }
                            break;
                    }
                }
            }
            self.unrecognizedSaveStrings.Remove("Variant");
            self.unrecognizedSaveStrings.Remove("PupsPlusStomach");
        }
        private void IL_PlayerNPCState_CycleTick(ILContext il)
        {
            ILCursor foodCurs = new(il);

            while (foodCurs.TryGotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup))))
            {
                /* WHILE TRYGOTO AFTER ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Slugpup
                 * 	IL_****: ldarg.0
	             *  IL_****: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Slugpup
                 */
                foodCurs.Emit(OpCodes.Ldarg_0); // self
                foodCurs.EmitDelegate((SlugcatStats.Name slugpup, PlayerNPCState self) =>   // If pupNPCState.variant != null, return variant, else return slugpup
                {
                    if (self.TryGetPupState(out var pupNPCState))
                    {
                        SlugcatStats.Name variant = pupNPCState.Variant;
                        if (variant != null)
                        {
                            return variant;
                        }
                    }
                    return slugpup;
                });
            }
        }
        private void AbstractCreature_setCustomFlags(On.AbstractCreature.orig_setCustomFlags orig, AbstractCreature self)
        {
            if (self.creatureTemplate.type == MoreSlugcatsEnums.CreatureTemplateType.SlugNPC)
            {
                if (self.TryGetPupAbstract(out var pupAbstract))
                {
                    if (self.spawnData == null || self.spawnData[0] != '{')
                    {
                        orig(self);
                        return;
                    }
                    string[] array = self.spawnData.Substring(1, self.spawnData.Length - 2).Split([',']);
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].Length > 0)
                        {
                            switch (array[i].Split([':'])[0])
                            {
                                case "Aquatic":
                                    pupAbstract.aquatic = true;
                                    break;
                                case "Tundra":
                                    pupAbstract.tundra = true;
                                    break;
                                case "Hunter":
                                    pupAbstract.hunter = true;
                                    break;
                                case "Rotund":
                                    pupAbstract.rotund = true;
                                    break;
                                case "Regular":
                                    pupAbstract.regular = true;
                                    break;
                            }
                        }
                    }
                }
            }
            orig(self);
        }
        private void IL_RegionState_AdaptRegionStateToWorld(ILContext il)
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
        private void IL_Snail_Click(ILContext il)
        {
            ILCursor staggerCurs = new(il);

            staggerCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Saint)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_0660
             * 	IL_065b: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Saint
			 *  IL_0660: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
			 *  IL_0665: brfalse.s IL_067a
             */
            staggerCurs.Emit(OpCodes.Ldloc, 10); // item
            staggerCurs.EmitDelegate((PhysicalObject item) =>   // If item is Player and Player is Tundrapup, return true
            {
                if (item is Player player && player.isTundrapup())
                {
                    return true;
                }
                return false;
            });
            staggerCurs.Emit(OpCodes.Or);
        }







        public static bool SlugpupSafari;
        public static bool RainbowPups;
        public static bool Pearlcat;
        public static bool SimplifiedMovesetGourmand;
        public static bool EmeraldsTweaks;
        public static bool BeastMasterPupExtras;


    }
}