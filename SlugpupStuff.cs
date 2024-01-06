using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using System;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

//       to do for variants:
// slug slam extra weight with gourm or other rotunds on back 
// rotund pup spitting up items when given disliked food or at random
// better rotund exhaust behavior
// better tundra pup ungrapple behavior
// starving stuff

//      to do for misc stuff:
// thumbnail for mod

//      to do for compat with mods:
// make dms work :hunterangy:
// emerald's tweaks 

//      remix config:
// manually set variant chances
// set pup id to specific variant

// oups..,......................,,,,,.........
// fo later:
// comment yer il hooks 


namespace SlugpupStuff
{
    [BepInPlugin(MOD_ID, "Slugpup Stuff", "1.0")]
    public partial class SlugpupStuff : BaseUnityPlugin
    {
        public const string MOD_ID = "iwantbread.slugpupstuff";
        private bool IsInit;
        private bool PostIsInit;
        private SlugpupRemix slugpupRemix;
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
            slugpupRemix = new SlugpupRemix(this, Logger);
        }
        public void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (IsInit) return;

                MachineConnector.SetRegisteredOI(MOD_ID, slugpupRemix);
                VariantName.RegisterValues();
                SlugpupGraphics.Patch();

                // Slugpup OnHooks
                On.MoreSlugcats.SlugNPCAI.ctor += SlugNPCAI_ctor;
                On.MoreSlugcats.SlugNPCAI.Update += SlugNPCAI_Update;
                On.MoreSlugcats.SlugNPCAI.TheoreticallyEatMeat += SlugNPCAI_TheoreticallyEatMeat;
                On.MoreSlugcats.SlugNPCAI.WantsToEatThis += SlugNPCAI_WantsToEatThis;
                On.MoreSlugcats.SlugNPCAI.HasEdible += SlugNPCAI_HasEdible;
                On.MoreSlugcats.SlugNPCAI.LethalWeaponScore += SlugNPCAI_LethalWeaponScore;
                On.MoreSlugcats.SlugNPCAI.GetFoodType += SlugNPCAI_GetFoodType;
                On.MoreSlugcats.SlugNPCAI.AteFood += SlugNPCAI_AteFood;
                On.MoreSlugcats.SlugNPCAI.Move += SlugNPCAI_Move;
                On.MoreSlugcats.SlugNPCAI.DecideBehavior += SlugNPCAI_DecideBehavior;


                // Slugpup ILHooks
                IL.MoreSlugcats.SlugNPCAI.ctor += IL_SlugNPCAI_ctor;
                IL.MoreSlugcats.SlugNPCAI.GetUpcoming += IL_SlugNPCAI_GetUpcoming;

                // Player OnHooks
                On.Player.Update += Player_Update;
                On.Player.AllowGrabbingBatflys += Player_AllowGrabbingBatflys;
                On.Player.CanEatMeat += Player_CanEatMeat;
                On.Player.SaintTongueCheck += Player_SaintTongueCheck;
                On.Player.ClassMechanicsSaint += Player_ClassMechanicsSaintOnBack;
                On.Player.ClassMechanicsSaint += Player_VariantMechanicsTundrapup;
                On.Player.GrabUpdate += Player_SlugpupStorageHook;
                On.Player.TongueUpdate += Player_TongueUpdate;

                // Player ILHooks
                IL.Player.ctor += IL_Player_ctor;
                IL.Player.SwallowObject += IL_Player_SwallowObject;
                IL.Player.Regurgitate += IL_Player_Regurgitate;
                IL.Player.GrabUpdate += IL_Player_GrabUpdate;
                IL.Player.Jump += IL_Player_Jump;
                IL.Player.UpdateAnimation += IL_Player_UpdateAnimation;
                IL.Player.SlugSlamConditions += IL_Player_SlugSlamConditions;
                IL.Player.Collide += IL_Player_Collide;
                IL.Player.ClassMechanicsGourmand += IL_Player_ClassMechanicsGourmand;
                IL.Player.ThrowObject += IL_Player_ThrowObject;
                IL.Player.ObjectEaten += IL_Player_ObjectEaten;
                IL.Player.FoodInRoom_Room_bool += IL_Player_FoodInRoom;
                IL.Player.StomachGlowLightColor += IL_Player_StomachGlowLightColor;
                IL.Player.NPCStats.ctor += IL_NPCStats_ctor;

                // Other OnHooks
                On.SlugcatStats.ctor += SlugcatStats_ctor;
                On.SlugcatStats.SlugcatFoodMeter += SlugcatStats_SlugcatFoodMeter;
                On.SlugcatStats.HiddenOrUnplayableSlugcat += SlugcatStats_HiddenOrUnplayableSlugcat;
                On.Player.Tongue.Shoot += Tongue_Shoot;

                // Other ILHooks
                IL.Snail.Click += IL_Snail_Click;
                IL.SlugcatStats.NourishmentOfObjectEaten += IL_SlugcatStats_NourishmentOfObjectEaten;
                IL.MoreSlugcats.PlayerNPCState.CycleTick += IL_PlayerNPCState_CycleTick;
                IL.RegionState.AdaptRegionStateToWorld += IL_RegionState_AdaptRegionStateToWorld;



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
                    SlugpupGraphics.SetupDMSSprites();
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "yeliah.slugpupFieldtrip"))
                {
                    slugpupSafari = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "rgbpups"))
                {
                    rainbowPups = true;
                }
                if (ModManager.ActiveMods.Any(mod => mod.id == "pearlcat"))
                {
                    pearlCat = true;
                }

                PostIsInit = true;
            }
            catch (Exception ex)
            {
                Logger.LogError("Pups+ PostModsInit failed!");
                Logger.LogError(ex);
                throw;
            }
        }



        public void SlugNPCAI_ctor(On.MoreSlugcats.SlugNPCAI.orig_ctor orig, SlugNPCAI self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (self.cat.SlugCatClass == MoreSlugcatsEnums.SlugcatStatsName.Slugpup)
            {
                if (!SlugpupCWTs.pupCWT.TryGetValue(self, out _))
                {
                    SlugpupCWTs.pupCWT.Add(self, _ = new SlugpupCWTs.PupVariables());
                }
                SetSlugpupPersonality(self.cat);
                if (RainWorld.ShowLogs)
                {
                    Debug.Log($"slugcat name set to {self.cat.slugcatStats.name}");
                    Debug.Log($"slugcat class set to {self.cat.SlugCatClass}");
                }

            }
        }
        public void IL_SlugNPCAI_ctor(ILContext il)
        {
            ILCursor itemTrackerCurs = new(il);
            itemTrackerCurs.GotoNext(MoveType.Before, x => x.MatchNewobj<ItemTracker>());   // Goto 'AddModule(new ItemTracker(this, 10, 10, -1, -1, stopTrackingCarried: true));'
            itemTrackerCurs.Prev.OpCode = OpCodes.Ldc_I4_0; // Switch stopTrackingCarried to false
        }
        public void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            VariantMechanicsAquaticpup(self);
        }
        public void SlugNPCAI_Move(On.MoreSlugcats.SlugNPCAI.orig_Move orig, SlugNPCAI self)
        {
            orig(self);
            if (self.cat.slugcatStats.name == VariantName.Tundrapup)
            {
                if (self.stuckTracker.Utility() > 0.1f)
                {
                    if (self.cat.tongue.Attached && Random.Range(0f, 1f) < Random.Range(0f, 0.6f * self.stuckTracker.Utility()))
                    {
                        self.cat.input[0].jmp = Random.Range(0, 2) == 0;
                    }
                }
            }

        }
        public void SlugNPCAI_Update(On.MoreSlugcats.SlugNPCAI.orig_Update orig, SlugNPCAI self)
        {
            orig(self);

            if (self.nap)
            {
                self.cat.emoteSleepCounter += Mathf.Clamp(0.0008f / self.cat.abstractCreature.personality.energy, 0.0008f, 0.005f);
                if (self.cat.emoteSleepCounter > 1.4f)
                {
                    if (self.cat.graphicsModule != null)
                    {
                        (self.cat.graphicsModule as PlayerGraphics).blink = 5;
                    }
                    self.cat.sleepCurlUp = Mathf.SmoothStep(self.cat.sleepCurlUp, 1f, self.cat.emoteSleepCounter - 1.4f);
                    return;
                }
                self.cat.sleepCurlUp = Mathf.Max(0f, self.cat.sleepCurlUp - 0.1f);
            }
            else
            {
                self.cat.emoteSleepCounter = 0f;
            }

            if (self.cat.grasps[0] != null && self.cat.HeavyCarry(self.cat.grasps[0].grabbed))
            {
                if (self.behaviorType == SlugNPCAI.BehaviorType.BeingHeld || self.behaviorType == SlugNPCAI.BehaviorType.OnHead)
                {
                    self.cat.ReleaseGrasp(0);
                }
            }
        }
        public void SlugNPCAI_AteFood(On.MoreSlugcats.SlugNPCAI.orig_AteFood orig, SlugNPCAI self, PhysicalObject food)
        {
            orig(self, food);
        }
        public void SlugNPCAI_DecideBehavior(On.MoreSlugcats.SlugNPCAI.orig_DecideBehavior orig, SlugNPCAI self)
        {
            orig(self);
            if (self.cat.gourmandExhausted)
            {
                if (self.threatTracker.TotalTrackedThreats > 0)
                {
                    self.behaviorType = SlugNPCAI.BehaviorType.Fleeing;
                }
                else
                {
                    self.behaviorType = SlugNPCAI.BehaviorType.Idle;
                }
            }
        }
        private void IL_SlugNPCAI_GetUpcoming(ILContext il)
        {
            ILCursor upcomingCurs = new(il);

            upcomingCurs.GotoNext(x => x.MatchLdcI4(5));

            upcomingCurs.Next.OpCode = OpCodes.Ldc_I4;
            upcomingCurs.Next.Operand = 20;
        }
        public void Player_SlugpupStorageHook(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            Player pupGrabbed = null;
            int grabbedIndex = -1;
            PhysicalObject grabbedObj = null;
            foreach (var grasped in self.grasps)
            {
                if (grasped?.grabbed is Player && (grasped.grabbed as Player).isNPC)
                {
                    pupGrabbed = grasped.grabbed as Player;
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
                        grabbedObj = grasped.grabbed;
                        break;
                    }
                }
                if (self.input[0].pckp && self.input[0].y == 1)
                {
                    if (pupGrabbed.grasps[0] != null && slugpupSafari)
                    {
                        orig(self, eu);
                        return;
                    }
                    if (self.slugOnBack != null)
                    {
                        self.slugOnBack.increment = false;
                    }
                    if (self.spearOnBack != null)
                    {
                        self.spearOnBack.increment = false;
                    }
                    if (grabbedIndex > -1 && grabbedObj != null)
                    {
                        if (pupGrabbed.objectInStomach == null && pupGrabbed.CanBeSwallowed(grabbedObj) && pupGrabbed.Consious)
                        {
                            pupGrabbed.swallowAndRegurgitateCounter++;
                            pupGrabbed.AI.heldWiggle = 0;
                            if (pupGrabbed.swallowAndRegurgitateCounter > 90)
                            {
                                if (RainWorld.ShowLogs)
                                {
                                    Debug.Log($"yummy {grabbedObj}");
                                }
                                pupGrabbed.SwallowObject(grabbedIndex);
                                pupGrabbed.swallowAndRegurgitateCounter = 0;
                                (pupGrabbed.graphicsModule as PlayerGraphics).swallowing = 20;
                            }
                        }
                    }
                    if ((pupGrabbed.objectInStomach != null || pupGrabbed.slugcatStats.name == VariantName.Rotundpup) && pupGrabbed.Consious)
                    {
                        pupGrabbed.swallowAndRegurgitateCounter++;
                        bool spitUpObject = false;
                        if (pupGrabbed.slugcatStats.name == VariantName.Rotundpup && pupGrabbed.objectInStomach == null && grabbedIndex == -1 && grabbedObj == null)
                        {
                            spitUpObject = true;
                        }
                        pupGrabbed.AI.heldWiggle = 0;
                        if (pupGrabbed.swallowAndRegurgitateCounter > 110)
                        {
                            if (RainWorld.ShowLogs)
                            {
                                Debug.Log($"spit that {pupGrabbed.objectInStomach.realizedObject} out rn");
                            }
                            if (!spitUpObject || (spitUpObject && pupGrabbed.FoodInStomach >= 1))
                            {
                                if (spitUpObject)
                                {
                                    pupGrabbed.SubtractFood(1);
                                }
                                pupGrabbed.Regurgitate();
                            }
                            else
                            {
                                pupGrabbed.firstChunk.vel += new Vector2(Random.Range(-1f, 1f), 0f);
                                pupGrabbed.Stun(30);
                            }

                            pupGrabbed.swallowAndRegurgitateCounter = 0;
                        }
                    }
                    else
                    {
                        orig(self, eu);
                        return;
                    }
                }
                else
                {
                    if (pupGrabbed.swallowAndRegurgitateCounter > 0)
                    {
                        pupGrabbed.swallowAndRegurgitateCounter--;
                    }
                    orig(self, eu);
                    return;
                }
            }
            else
            {
                orig(self, eu);
                return;
            }
        }
        public void IL_Player_ctor(ILContext il)
        {
            ILCursor stomachObjCurs = new(il);

            stomachObjCurs.GotoNext(x => x.MatchLdfld<PlayerNPCState>(nameof(PlayerNPCState.Glowing)));
            stomachObjCurs.GotoNext(x => x.MatchLdfld<PlayerNPCState>(nameof(PlayerNPCState.Glowing)));
            stomachObjCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>(nameof(Player.SetMalnourished)));
            /* GOTO
             *  if (isSlugpup && playerState is PlayerNPCState)
             *  {
             *      glowing = (playerState as PlayerNPCState).Glowing;
             *      SetMalnourished((playerState as PlayerNPCState).Malnourished || base.dead);
             *      >HERE<
             *  }
             */
            stomachObjCurs.Emit(OpCodes.Ldarg_0);
            stomachObjCurs.Emit(OpCodes.Ldarg_1);
            stomachObjCurs.EmitDelegate((Player self, AbstractCreature abstractCreature) =>   // If game session is a Story Session and StomachObject is not null, set objectInStomach to StomachObject
            {
                if (self.room.game.session is StoryGameSession && (self.playerState as PlayerNPCState).StomachObject != null)
                {
                    self.objectInStomach = (self.playerState as PlayerNPCState).StomachObject;
                    self.objectInStomach.pos = abstractCreature.pos;
                }
            });

        }
        public void IL_Player_SwallowObject(ILContext il)
        {
            ILCursor parentCurs = new(il);
            ILCursor graspedCurs = new(il);

            parentCurs.GotoNext(MoveType.After, x => x.MatchStloc(0)); // Goto after 'AbstractPhysicalObject abstractPhysicalObject = base.grasps[grasp].grabbed.abstractPhysicalObject;'

            ILLabel abstObjLabel = il.DefineLabel();
            parentCurs.MarkLabel(abstObjLabel); // Mark abstObjLabel at the emiited ldarg.0
            parentCurs.Emit(OpCodes.Ldarg_0);
            parentCurs.Emit(OpCodes.Ldarg_1); // grasp
            parentCurs.Emit(OpCodes.Ldloc_0); // abstractPhysicalObject
            parentCurs.EmitDelegate((Player self, int grasp, AbstractPhysicalObject abstractPhysicalObject) =>  // If grabbed by a player, set abstractPhysicalObject to object in the player's grasps[grasp]
            {
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    return (self.grabbedBy[0].grabber as Player).grasps[grasp].grabbed.abstractPhysicalObject;
                }
                return abstractPhysicalObject;
            });
            parentCurs.Emit(OpCodes.Stloc_0); // Store abstractPhysicalObject at stloc.0

            graspedCurs.GotoNext(MoveType.Before, x => x.MatchLdarg(1)); // Goto before 'if (grasp < 0 || base.grasps[grasp] == null)'

            graspedCurs.Emit(OpCodes.Ldarg_0);
            graspedCurs.EmitDelegate((Player self) =>   // If grabbed by a player, branch to abstObjLabel
            {
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    return true;
                }
                return false;
            });
            graspedCurs.Emit(OpCodes.Brtrue_S, abstObjLabel);

            parentCurs.GotoNext(MoveType.After, x => x.MatchCallvirt<Creature>(nameof(Creature.ReleaseGrasp))); // Goto after 'ReleaseGrasp(grasp);'

            ILLabel releaseLabel = il.DefineLabel();
            parentCurs.MarkLabel(releaseLabel); // Mark releaseLabel at the emitted ldarg.0
            parentCurs.Emit(OpCodes.Ldarg_0);
            parentCurs.Emit(OpCodes.Ldarg_1); // grasp
            parentCurs.EmitDelegate((Player self, int grasp) =>   // If grabbed by a player, make the player drop the object in their grasps[grasp]
            {
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    (self.grabbedBy[0].grabber as Player).ReleaseGrasp(grasp);
                }
            });

            graspedCurs.GotoNext(MoveType.After, x => x.MatchCallvirt<StoryGameSession>(nameof(StoryGameSession.RemovePersistentTracker))); // Goto after '(room.game.session as StoryGameSession).RemovePersistentTracker(objectInStomach);'
            graspedCurs.Emit(OpCodes.Ldarg_0);
            graspedCurs.EmitDelegate((Player self) =>   // If grabbed by a player, branch to releaseLabel
            {
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    return true;
                }
                return false;
            });
            graspedCurs.Emit(OpCodes.Brtrue_S, releaseLabel);
        }
        public void IL_Player_Regurgitate(ILContext il)
        {
            ILCursor parentCurs = new(il);
            ILCursor graspedCurs = new(il);
            ILCursor rotundCurs = new(il);

            ILLabel rotundLabel = il.DefineLabel();
            ILLabel branchLabel = il.DefineLabel();
            ILLabel cursLabel = il.DefineLabel();

            rotundCurs.GotoNext(MoveType.After, x => x.MatchCall<Player>("get_isGourmand"));
            /* GOTO
             * if (!isGourmand)
             */
            rotundLabel = rotundCurs.Next.Operand as ILLabel; // Mark rotundLabel at 'objectInStomach = GourmandCombos.RandomStomachItem(this);'

            rotundCurs.GotoPrev(MoveType.Before, x => x.MatchLdarg(0));
            /* GOTO BEFORE
             * if (!isGourmand)
             */
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((Player self) =>   // If self is Rotundpup, branch to rotundLabel
            {
                if (self.slugcatStats.name == VariantName.Rotundpup)
                {
                    return true;
                }
                return false;
            });
            rotundCurs.Emit(OpCodes.Brtrue_S, rotundLabel);

            parentCurs.GotoNext(MoveType.After, x => x.MatchLdloc(2), x => x.Match(OpCodes.Brfalse));
            /* GOTO BEFORE
             * if (flag && FreeHand() > -1)
             */
            graspedCurs.GotoNext(x => x.MatchLdarg(0), x => x.MatchLdnull());
            /* GOTO
             * objectInStomach = null; 
             */

            graspedCurs.MarkLabel(branchLabel); // Mark branchLabel at 'objectInStomach = null;'

            parentCurs.MarkLabel(cursLabel); // Mark cursLabel before the emitted ldarg.0 to goto it later

            parentCurs.Emit(OpCodes.Ldarg_0);
            parentCurs.EmitDelegate((Player self) =>   // If grabbed by a player, make player grab regurgitated object and branch to branchLabel
            {
                Player parent = null;
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    parent = self.grabbedBy[0].grabber as Player;
                }
                if (parent != null && parent.FreeHand() > -1)
                {
                    if (ModManager.MMF && ((parent.grasps[0] != null) ^ (parent.grasps[1] != null)) && parent.Grabability(self.objectInStomach.realizedObject) == Player.ObjectGrabability.BigOneHand)
                    {
                        int num3 = 0;
                        if (parent.FreeHand() == 0)
                        {
                            num3 = 1;
                        }

                        if (parent.Grabability(parent.grasps[num3].grabbed) != Player.ObjectGrabability.BigOneHand)
                        {
                            parent.SlugcatGrab(self.objectInStomach.realizedObject, parent.FreeHand());
                        }
                    }
                    else
                    {
                        parent.SlugcatGrab(self.objectInStomach.realizedObject, parent.FreeHand());
                    }
                }
            });
            parentCurs.Emit(OpCodes.Br_S, branchLabel);

            ILLabel graspedlabel = il.DefineLabel();
            parentCurs.MarkLabel(graspedlabel); // Mark graspedLabel at 'if (flag && FreeHand() > -1)'

            parentCurs.GotoLabel(cursLabel, MoveType.Before); // Goto cursLabel

            parentCurs.Emit(OpCodes.Ldarg_0);
            parentCurs.EmitDelegate((Player self) =>   // If not grabbed by a player, branch to graspedLabel
            {
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    return true;
                }
                return false;
            });
            parentCurs.Emit(OpCodes.Brfalse_S, graspedlabel);
        }
        public void IL_Player_GrabUpdate(ILContext il)
        {
            ILCursor counterCurs = new(il);

            while (counterCurs.TryGotoNext(x => x.MatchLdfld<Player>(nameof(Player.eatCounter)))) { } // Goto last 'eatCounter'
            counterCurs.GotoNext(MoveType.After, x => x.MatchLdarg(0)); // Goto after ldarg.0 in 'swallowAndRegurgitateCounter = 0;'

            ILCursor graspedCurs = counterCurs.Clone();

            graspedCurs.GotoNext(MoveType.Before, x => x.MatchLdcI4(0), x => x.MatchStloc(34)); // Goto before 'for (int num15 = 0; num15 < base.grasps.Length; num15++)'
            ILLabel graspedLabel = il.DefineLabel();
            graspedCurs.MarkLabel(graspedLabel); // Mark graspedLabel at ldc.i4.0

            counterCurs.EmitDelegate((Player self) =>   // If grabbed by a player and player is holding pckp + up, branch to graspedLabel
            {
                if (self.AI?.behaviorType == SlugNPCAI.BehaviorType.BeingHeld)
                {
                    if ((self.grabbedBy[0].grabber as Player).input[0].pckp && (self.grabbedBy[0].grabber as Player).input[0].y == 1)
                    {
                        return true;
                    }
                }
                return false;
            });
            counterCurs.Emit(OpCodes.Brtrue_S, graspedLabel);
            counterCurs.Emit(OpCodes.Ldarg_0); // Push ldarg.0 back onto stack
        }
        public void IL_Player_StomachGlowLightColor(ILContext il)
        {
            ILCursor pupCurs = new(il);

            pupCurs.GotoNext(MoveType.After, x => x.Match(OpCodes.Brtrue_S));
            /* GOTO
             * AbstractPhysicalObject abstractPhysicalObject = ((AI != null) ? (base.State as PlayerNPCState).StomachObject : >HERE< objectInStomach);
             */

            ILLabel skipLabel = il.DefineLabel();
            pupCurs.MarkLabel(skipLabel); // Mark skipLabel at objectInStomach

            pupCurs = new(il);
            pupCurs.Emit(OpCodes.Br_S, skipLabel); // Branch over ((AI != null) ? (base.State as PlayerNPCState).StomachObject because it's bad and redundant
        }
        public void IL_RegionState_AdaptRegionStateToWorld(ILContext il)
        {
            ILCursor stomachObjCurs = new(il);

            stomachObjCurs.GotoNext(x => x.MatchLdstr("Add pup to pendingFriendSpawns "));
            stomachObjCurs.GotoNext(MoveType.Before, x => x.MatchLdarg(0));
            /* GOTO BEFORE
             * saveState.pendingFriendCreatures.Add(SaveState.AbstractCreatureToStringStoryWorld(abstractCreature));
             */
            stomachObjCurs.Emit(OpCodes.Ldloc, 5);
            stomachObjCurs.EmitDelegate((AbstractCreature abstractCreature) =>   // If abstractCreature is Player and it's playerState is PlayerNPCState, set StomachObject to objectInStomach
            {
                if (abstractCreature.realizedCreature is Player { objectInStomach: not null } pup)
                {
                    if (pup.playerState is PlayerNPCState)
                    {
                        (pup.playerState as PlayerNPCState).StomachObject = pup.objectInStomach;
                    }
                }
            });
        }
        public void IL_Snail_Click(ILContext il)
        {
            ILCursor staggerCursor = new(il);

            staggerCursor.GotoNext(x => x.MatchCallvirt<Player>(nameof(Player.SaintStagger))); // Goto '(item as Player).SaintStagger(1500);'
            staggerCursor.GotoNext(MoveType.After, x => x.MatchLdloc(10)); // Goto before ldloc.s 10 in 'else if (item is Player)'

            staggerCursor.EmitDelegate((PhysicalObject physicalObject) =>   // If physicalObject is Tundrapup, do SaintStagger
            {
                if (physicalObject is Player && (physicalObject as Player).slugcatStats.name == VariantName.Tundrapup)
                {
                    (physicalObject as Player).SaintStagger(1500);
                }
            });
            staggerCursor.Emit(OpCodes.Ldloc, 10); // Push ldloc.s 10 back onto stack
        }




        public static bool slugpupSafari = false;
        public static bool rainbowPups = false;
        public static bool pearlCat = false;


    }
}