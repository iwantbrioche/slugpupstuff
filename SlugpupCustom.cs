using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

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
        public static bool WantsItem(this SlugNPCAI self, PhysicalObject obj)
        {
            if (!(Random.value < Mathf.Lerp(0f, 0.9f, Mathf.InverseLerp(0.4f, 1f, self.cat.abstractCreature.personality.bravery))))
            {
                if ((obj is Spear || obj is ScavengerBomb || obj is SingularityBomb) && Random.value < Mathf.Lerp(0f, 0.05f, Mathf.InverseLerp(0.4f, 1f, self.cat.abstractCreature.personality.aggression)))
                {
                    return true;
                }
                if (Random.value < Mathf.Lerp(0f, 0.05f * ((obj is DataPearl || obj is OverseerCarcass || obj is NSHSwarmer || obj is VultureMask) ? 3f : 1f), Mathf.InverseLerp(0.4f, 1f, self.cat.abstractCreature.personality.energy)))
                {
                    return true;
                }
                if ((obj is FirecrackerPlant || obj is FlyLure || obj is PuffBall || obj is FlareBomb || obj is GooieDuck || obj is BubbleGrass || obj is NeedleEgg) && Random.value < Mathf.Lerp(0f, 0.05f, Mathf.InverseLerp(0.3f, 1f, self.cat.abstractCreature.personality.dominance)))
                {
                    return true;
                }
                if ((obj is Rock || obj is PuffBall || obj is FlareBomb || obj is FirecrackerPlant || obj is LillyPuck || obj is JellyFish) && Random.value < Mathf.Lerp(0f, 0.05f, Mathf.InverseLerp(0.3f, 1f, self.cat.abstractCreature.personality.nervous)))
                {
                    return true;
                }
            }
            return false;
        }
        public static float LerpModifier(float a, float b, float t, float mod)
        {
            return Mathf.Clamp01(Mathf.Lerp(mod < 1f ? a : a * mod, mod > 1f ? b : b * mod, t));
        }
    }
}
