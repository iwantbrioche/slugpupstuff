
namespace SlugpupStuff.Hooks
{
    public static class SlugpupHooks
    {
        public static void Patch()
        {
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

            IL.MoreSlugcats.SlugNPCAI.ctor += IL_SlugNPCAI_ctor;

        }
        private static void SlugNPCAI_ctor(On.MoreSlugcats.SlugNPCAI.orig_ctor orig, SlugNPCAI self, AbstractCreature creature, World world)
        {
            orig(self, creature, world);
            if (self.TryGetPupVariables(out var pupVariables))
            {
                pupVariables.pathingVisualizer = new(self, 5);
                pupVariables.labelManager = new(self.cat);
                //pupVariables.destinationVisualizer = new(world.game.abstractSpaceVisualizer, world, self.pathFinder, self.cat.ShortCutColor());
            }

        }
        private static void SlugNPCAI_Move(On.MoreSlugcats.SlugNPCAI.orig_Move orig, SlugNPCAI self)
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
                pupVariables.destinationVisualizer?.Update();
            }

        }
        private static void SlugNPCAI_Update(On.MoreSlugcats.SlugNPCAI.orig_Update orig, SlugNPCAI self)
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
                    foreach (var rep in self.itemTracker.items)
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

                pupVariables.pathingVisualizer?.Update();
                if (pupVariables.labelManager != null)
                {
                    pupVariables.labelManager.UpdateLabel("grabTarget", $"grabTarget: {(self.grabTarget != null ? self.grabTarget is Creature ? (self.grabTarget as Creature).abstractCreature.creatureTemplate.type : self.grabTarget.abstractPhysicalObject.type : "NULL")}", self.grabTarget != null);
                    pupVariables.labelManager.UpdateLabel("giftedItem", $"giftedItem: {(pupVariables.giftedItem != null ? pupVariables.giftedItem is AbstractCreature ? (pupVariables.giftedItem as AbstractCreature).creatureTemplate.type : pupVariables.giftedItem.type : "NULL")}", pupVariables.giftedItem != null);
                    pupVariables.labelManager.UpdateLabel("prey", $"hunting: {(self.AttackingPrey() ? self.preyTracker.MostAttractivePrey.representedCreature.realizedCreature : "NULL")}", self.AttackingPrey());
                    pupVariables.labelManager.Update(self.cat.mainBodyChunk.pos + new Vector2(35f, 30f));
                }
            }
        }
        private static void SlugNPCAI_DecideBehavior(On.MoreSlugcats.SlugNPCAI.orig_DecideBehavior orig, SlugNPCAI self)
        {
            orig(self);
            if (self.behaviorType == SlugNPCAI.BehaviorType.Attacking && self.cat.gourmandExhausted)
            {
                self.behaviorType = SlugNPCAI.BehaviorType.Fleeing;
            }
        }
        private static void SlugNPCAI_SocialEvent(On.MoreSlugcats.SlugNPCAI.orig_SocialEvent orig, SlugNPCAI self, SocialEventRecognizer.EventID ID, Creature subjectCrit, Creature objectCrit, PhysicalObject involvedItem)
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
        private static float SlugNPCAI_LethalWeaponScore(On.MoreSlugcats.SlugNPCAI.orig_LethalWeaponScore orig, SlugNPCAI self, PhysicalObject obj, Creature target)
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
        private static bool SlugNPCAI_TheoreticallyEatMeat(On.MoreSlugcats.SlugNPCAI.orig_TheoreticallyEatMeat orig, SlugNPCAI self, Creature crit, bool excludeCentipedes)
        {
            if (self.isTundrapup())
            {
                if (self.TryGetPupVariables(out var pupVariables))
                {
                    if (crit.abstractCreature == pupVariables.giftedItem && crit.dead)
                    {
                        return !excludeCentipedes;
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
        private static bool SlugNPCAI_WantsToEatThis(On.MoreSlugcats.SlugNPCAI.orig_WantsToEatThis orig, SlugNPCAI self, PhysicalObject obj)
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
        private static bool SlugNPCAI_HasEdible(On.MoreSlugcats.SlugNPCAI.orig_HasEdible orig, SlugNPCAI self)
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
        private static SlugNPCAI.Food SlugNPCAI_GetFoodType(On.MoreSlugcats.SlugNPCAI.orig_GetFoodType orig, SlugNPCAI self, PhysicalObject food)
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
        private static PathCost SlugNPCAI_TravelPreference(On.MoreSlugcats.SlugNPCAI.orig_TravelPreference orig, SlugNPCAI self, MovementConnection coord, PathCost cost)
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

        private static void IL_SlugNPCAI_ctor(ILContext il)
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
    }
}
