using BepInEx.Logging;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MoreSlugcats;
using RWCustom;
using SimplifiedMoveset;
using System;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

namespace SlugpupStuff
{
    public class SlugpupGraphics
    {
        private static ManualLogSource Logger;
        public static void Patch(ManualLogSource logSource)
        {
            Logger = logSource;

            // Graphics On Hooks
            On.PlayerGraphics.ctor += PlayerGraphics_ctor;
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            On.PlayerGraphics.SaintFaceCondition += PlayerGraphics_SaintFaceCondition;
            On.PlayerGraphics.MSCUpdate += PlayerGraphics_MSCUpdate;
            On.PlayerGraphics.ApplyPalette += PlayerGraphics_ApplyPalette;
            On.PlayerGraphics.DefaultBodyPartColorHex += PlayerGraphics_DefaultBodyPartColorHex;
            On.PlayerGraphics.ColoredBodyPartList += PlayerGraphics_ColoredBodyPartList;
            On.SlugcatHand.Update += SlugcatHand_Update;

            // Graphics IL Hooks
            IL.PlayerGraphics.DrawSprites += IL_PlayerGraphics_DrawSprites;
            IL.PlayerGraphics.InitiateSprites += IL_PlayerGraphics_InitiateSprites;
            IL.PlayerGraphics.Update += IL_PlayerGraphics_Update;
        }


        // Hooks
        private static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (self.player.isAquaticpup())
            {
                self.gills = new PlayerGraphics.AxolotlGills(self, 13);
            }
            if (self.player.isTundrapup())
            {
                self.ropeSegments = new PlayerGraphics.RopeSegment[20];
                for (int k = 0; k < self.ropeSegments.Length; k++)
                {
                    self.ropeSegments[k] = new PlayerGraphics.RopeSegment(k, self);
                }
            }
        }
        private static void SlugcatHand_Update(On.SlugcatHand.orig_Update orig, SlugcatHand self)
        {
            orig(self);
            Player pupGrabbed = null;
            int grabbedIndex = -1;
            foreach (var grasped in (self.owner.owner as Player).grasps)
            {
                if (grasped != null && grasped.grabbed is Player pup && pup.isNPC)
                {
                    pupGrabbed = pup;
                    break;
                }
            }
            if (pupGrabbed != null)
            {
                foreach (var grasped in (self.owner.owner as Player).grasps)
                {
                    if (grasped?.grabbed != null && grasped.grabbed != pupGrabbed)
                    {
                        grabbedIndex = grasped.graspUsed;
                        break;
                    }
                }
                if (grabbedIndex > -1)
                {
                    if (pupGrabbed.swallowAndRegurgitateCounter > 10 && pupGrabbed.objectInStomach == null && pupGrabbed.CanBeSwallowed((self.owner.owner as Player).grasps[grabbedIndex].grabbed) && pupGrabbed.Consious)
                    {
                        if (grabbedIndex == self.limbNumber)
                        {
                            self.mode = Limb.Mode.HuntRelativePosition;
                            float num5 = Mathf.InverseLerp(10f, 90f, pupGrabbed.swallowAndRegurgitateCounter);
                            if (num5 < 0.5f)
                            {
                                self.relativeHuntPos *= Mathf.Lerp(0.9f, 0.7f, num5 * 2f);
                                self.relativeHuntPos.y += Mathf.Lerp(2f, 4f, num5 * 2f);
                                self.relativeHuntPos.x *= Mathf.Lerp(1f, 1.2f, num5 * 2f);
                            }
                            else
                            {
                                self.mode = Limb.Mode.HuntAbsolutePosition;
                                self.absoluteHuntPos = (pupGrabbed.graphicsModule as PlayerGraphics).head.pos;
                                self.absoluteHuntPos += new Vector2(0f, -4f) + Custom.RNV() * 2f * Random.value * Mathf.InverseLerp(0.5f, 1f, num5);

                                (pupGrabbed.graphicsModule as PlayerGraphics).blink = 5;
                                (pupGrabbed.graphicsModule as PlayerGraphics).head.vel += Custom.RNV() * 2f * Random.value * Mathf.InverseLerp(0.5f, 1f, num5);
                                pupGrabbed.bodyChunks[0].vel += Custom.RNV() * 0.2f * Random.value * Mathf.InverseLerp(0.5f, 1f, num5);
                            }
                        }
                    }
                }
            }
        }
        private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self.player.isHunterpup())
            {
                if (self.RenderAsPup)
                {
                    sLeaser.sprites[0].scaleY = 0.7f;
                }
            }
        }
        private static void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig(self, sLeaser, rCam, newContatiner);
            if (self.player.isAquaticpup())
            {
                self.gills.AddToContainer(sLeaser, rCam, rCam.ReturnFContainer("Midground"));
            }
            if (SlugpupCWTs.pupGraphicsCWT.TryGetValue(self, out var pupGraphics))
            {
                if (self.player.isTundrapup())
                {
                    rCam.ReturnFContainer("Midground").AddChild(sLeaser.sprites[pupGraphics.TongueSpriteIndex]);
                }
            }
        }
        private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (self.player.room != null && self.player.isAquaticpup())
            {
                self.gills.DrawSprites(sLeaser, rCam, timeStacker, camPos);
            }
            if (self.TryGetPupGraphics(out var pupGraphics))
            {
                if (self.player.isTundrapup())
                {
                    string headElement = sLeaser.sprites[3]?.element?.name;
                    if (headElement != null)
                    {
                        headElement = "HeadB" + headElement.Remove(0, 5);
                        sLeaser.sprites[3].element = Futile.atlasManager.GetElementWithName(headElement);
                    }
                    if (self.player.room != null)
                    {
                        float b = Mathf.Lerp(self.lastStretch, self.stretch, timeStacker);
                        Vector2 vector2;
                        Vector2 vector = Vector2.Lerp(self.ropeSegments[0].lastPos, self.ropeSegments[0].pos, timeStacker);
                        vector += Custom.DirVec(Vector2.Lerp(self.ropeSegments[1].lastPos, self.ropeSegments[1].pos, timeStacker), vector) * 1f;
                        float num5 = 0f;
                        for (int k = 1; k < self.ropeSegments.Length; k++)
                        {
                            float num6 = k / self.ropeSegments.Length - 1;
                            if (k >= self.ropeSegments.Length - 2)
                            {
                                vector2 = new Vector2(sLeaser.sprites[9].x + camPos.x, sLeaser.sprites[9].y - 1 + camPos.y);
                            }
                            else
                            {
                                vector2 = Vector2.Lerp(self.ropeSegments[k].lastPos, self.ropeSegments[k].pos, timeStacker);
                            }
                            Vector2 a2 = Custom.PerpendicularVector((vector - vector2).normalized);
                            float d4 = 0.2f + 1.6f * Mathf.Lerp(1f, b, Mathf.Pow(Mathf.Sin(num6 * (float)Math.PI), 0.7f));
                            Vector2 vector4 = vector - a2 * d4;
                            Vector2 vector5 = vector2 + a2 * d4;
                            float num7 = Mathf.Sqrt(Mathf.Pow(vector4.x - vector5.x, 2f) + Mathf.Pow(vector4.y - vector5.y, 2f));
                            if (!float.IsNaN(num7))
                            {
                                num5 += num7;
                            }
                            (sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).MoveVertice((k - 1) * 4, vector4 - camPos);
                            (sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).MoveVertice((k - 1) * 4 + 1, vector + a2 * d4 - camPos);
                            (sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).MoveVertice((k - 1) * 4 + 2, vector2 - a2 * d4 - camPos);
                            (sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).MoveVertice((k - 1) * 4 + 3, vector5 - camPos);
                            vector = vector2;
                        }
                        if (self.player.tongue.Free || self.player.tongue.Attached)
                        {
                            sLeaser.sprites[pupGraphics.TongueSpriteIndex].isVisible = true;
                        }
                        else
                        {
                            sLeaser.sprites[pupGraphics.TongueSpriteIndex].isVisible = false;
                        }
                    }
                }
            }
        }
        private static bool PlayerGraphics_SaintFaceCondition(On.PlayerGraphics.orig_SaintFaceCondition orig, PlayerGraphics self)
        {
            if (self.player.room != null && self.player.isTundrapup())
            {
                return true;
            }
            return orig(self);
        }
        private static void PlayerGraphics_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            orig(self, sLeaser, rCam, palette);
            if (self.gills != null && self.player.isAquaticpup())
            {
                Random.State state = Random.state;
                Random.InitState(self.player.abstractCreature.ID.RandomSeed);

                Color baseCol = self.player.ShortCutColor();
                Color.RGBToHSV(baseCol, out float H, out float S, out float V);

                H *= Random.Range(1.35f, 1.7f);
                S *= Random.Range(1.15f, 2.4f);
                if (self.player.npcStats.Dark)
                {
                    V *= Random.Range(7.5f, 9f);
                }
                if (SlugpupStuff.RainbowPups)
                {
                    H = Random.Range(0f, 1f);
                    S = Random.Range(0f, 1f);
                    V = Random.Range(0f, 1f);
                }
                Random.state = state;

                Color effectCol = Color.HSVToRGB(Mathf.Clamp01(H), S, V);

                if (self.player.room != null && self.player.room.world.game.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat == MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel)
                {
                    effectCol = Color.red;
                }

                self.gills.effectColor = effectCol;
                self.gills.baseColor = baseCol;
                self.gills.ApplyPalette(sLeaser, rCam, palette);
            }
            if (self.TryGetPupGraphics(out var pupGraphics))
            {
                if (self.player.isTundrapup())
                {
                    for (int j = 0; j < (sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).verticeColors.Length; j++)
                    {
                        float num2 = Mathf.Clamp(Mathf.Sin(j / ((sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).verticeColors.Length - 1) * (float)Math.PI), 0f, 1f);
                        (sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).verticeColors[j] = Color.Lerp(palette.fogColor, Custom.HSL2RGB(Mathf.Lerp(0.95f, 1f, num2), 1f, Mathf.Lerp(0.75f, 0.9f, Mathf.Pow(num2, 0.15f))), 0.7f);

                        if (self.player.room != null && self.player.room.world.game.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat == MoreSlugcatsEnums.SlugcatStatsName.Sofanthiel)
                        {
                            (sLeaser.sprites[pupGraphics.TongueSpriteIndex] as TriangleMesh).verticeColors[j] = Color.red;
                        }
                    }
                }
            }
        }
        private static List<string> PlayerGraphics_DefaultBodyPartColorHex(On.PlayerGraphics.orig_DefaultBodyPartColorHex orig, SlugcatStats.Name slugcatID)
        {
            List<string> list = orig(slugcatID);
            if (slugcatID == SlugpupStuff.VariantName.Aquaticpup)
            {
                list.Add("FFFFFF");
                return list;
            }
            if (slugcatID == SlugpupStuff.VariantName.Tundrapup)
            {
                list.Add("FF80A6");
                return list;
            }
            return orig(slugcatID);
        }
        private static List<string> PlayerGraphics_ColoredBodyPartList(On.PlayerGraphics.orig_ColoredBodyPartList orig, SlugcatStats.Name slugcatID)
        {
            List<string> list = orig(slugcatID);
            if (slugcatID == SlugpupStuff.VariantName.Aquaticpup)
            {
                list.Add("Gills");
                return list;
            }
            if (slugcatID == SlugpupStuff.VariantName.Tundrapup)
            {
                list.Add("Tongue");
                return list;
            }
            return orig(slugcatID);
        }
        private static void PlayerGraphics_MSCUpdate(On.PlayerGraphics.orig_MSCUpdate orig, PlayerGraphics self)
        {
            orig(self);
            if (self.player.room != null && self.player.isAquaticpup())
            {
                self.gills.Update();
            }
            if (self.player.room != null && self.player.isTundrapup())
            {
                self.lastStretch = self.stretch;
                self.stretch = self.RopeStretchFac;
                List<Vector2> list = [];
                for (int j = self.player.tongue.rope.TotalPositions - 1; j > 0; j--)
                {
                    list.Add(self.player.tongue.rope.GetPosition(j));
                }
                list.Add(self.player.mainBodyChunk.pos);
                float num = 0f;
                for (int k = 1; k < list.Count; k++)
                {
                    num += Vector2.Distance(list[k - 1], list[k]);
                }
                float num2 = 0f;
                for (int l = 0; l < list.Count; l++)
                {
                    if (l > 0)
                    {
                        num2 += Vector2.Distance(list[l - 1], list[l]);
                    }
                    self.AlignRope(num2 / num, list[l]);
                }
                for (int m = 0; m < self.ropeSegments.Length; m++)
                {
                    self.ropeSegments[m].Update();
                }
                for (int n = 1; n < self.ropeSegments.Length; n++)
                {
                    self.ConnectRopeSegments(n, n - 1);
                }
                for (int num3 = 0; num3 < self.ropeSegments.Length; num3++)
                {
                    self.ropeSegments[num3].claimedForBend = false;
                }
            }
        }
        private static void IL_PlayerGraphics_InitiateSprites(ILContext il)
        {
            ILCursor initCurs = new(il);

            initCurs.GotoNext(MoveType.After, x => x.MatchCallvirt<PlayerGraphics.Gown>(nameof(PlayerGraphics.Gown.InitiateSprite)), x => x.MatchLdarg(0));
            /* GOTO AFTER IL_0662
             * 	IL_065b: ldarg.1
	         *  IL_065c: ldarg.2
	         *  IL_065d: callvirt instance void PlayerGraphics/Gown::InitiateSprite(int32, class RoomCamera/SpriteLeaser, class RoomCamera)
	         *  IL_0662: ldarg.0
             */
            // IL_0662: ldarg.0 => self
            initCurs.Emit(OpCodes.Ldarg_1); // sLeaser
            initCurs.Emit(OpCodes.Ldarg_2); // rCam
            initCurs.EmitDelegate((PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam) =>   // Resize sLeaser.sprites and initiate pup sprites
            {
                if (self.player.isAquaticpup())
                {
                    Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + self.gills.numberOfSprites);
                    self.gills.InitiateSprites(sLeaser, rCam);
                }
                if (self.TryGetPupGraphics(out var pupGraphics))
                {
                    if (self.player.isTundrapup())
                    {
                        Array.Resize(ref sLeaser.sprites, sLeaser.sprites.Length + 1);
                        pupGraphics.TongueSpriteIndex = sLeaser.sprites.Length - 1;

                        sLeaser.sprites[pupGraphics.TongueSpriteIndex] = TriangleMesh.MakeLongMesh(self.ropeSegments.Length - 1, false, true);
                    }
                }
            });
            initCurs.Emit(OpCodes.Ldarg_0); // re-emit ldarg.0
        } 
        private static void IL_PlayerGraphics_DrawSprites(ILContext il)
        {
            ILCursor drawCurs = new(il);

            drawCurs.GotoNext(x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup)));
            drawCurs.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt<FNode>("set_scaleX"));
            /* GOTO AFTER IL_03c8
             * 	IL_03c7: add
	         *  IL_03c8: callvirt instance void FNode::set_scaleX(float32)
	         *  IL_03cd: br.s IL_041a
             */
            drawCurs.Emit(OpCodes.Ldarg_0);
            drawCurs.Emit(OpCodes.Ldarg_1);
            drawCurs.Emit(OpCodes.Ldloc_0);
            drawCurs.Emit(OpCodes.Ldloc, 4);
            drawCurs.EmitDelegate((PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, float num, float num2) =>   // If self is Rotundpup, set sLeaser.sprites[1].scaleX to be wider
            {
                if (self.player.isRotundpup())
                {
                    sLeaser.sprites[1].scaleX = 1.2f + 0.2f * Mathf.Lerp(self.player.npcStats.Wideness, 0.5f, 0.5f) + Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(-0.05f, -0.2f, self.malnourished), 0.05f, num) * num2, 0.15f, self.player.sleepCurlUp);
                }
            });

            drawCurs.GotoNext(x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Slugpup)));
            drawCurs.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt<FNode>("set_scaleX"));
            /* GOTO AFTER IL_062b
             * 	IL_062a: sub
	         *  IL_062b: callvirt instance void FNode::set_scaleX(float32)
	         *  IL_0630: br.s IL_066b
             */
            drawCurs.Emit(OpCodes.Ldarg_0);
            drawCurs.Emit(OpCodes.Ldarg_1);
            drawCurs.Emit(OpCodes.Ldloc_0);
            drawCurs.EmitDelegate((PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, float num) =>   // If self is Rotundpup, set sLeaser.sprites[0].scaleX to be wider
            {
                if (self.player.isRotundpup())
                {
                    sLeaser.sprites[0].scaleX = 1f + 0.2f * Mathf.Lerp(self.player.npcStats.Wideness, 0.5f, 0.5f) + self.player.sleepCurlUp * 0.2f + 0.05f * num - 0.1f * self.malnourished;
                }
            });
        }
        private static void IL_PlayerGraphics_Update(ILContext il)
        {
            ILCursor rotundCurs = new(il);

            rotundCurs.GotoNext(MoveType.After, x => x.MatchLdsfld<MoreSlugcatsEnums.SlugcatStatsName>(nameof(MoreSlugcatsEnums.SlugcatStatsName.Gourmand)), x => x.Match(OpCodes.Call));
            /* GOTO AFTER IL_1dcc
             * 	IL_1dc7: ldsfld class SlugcatStats/Name MoreSlugcats.MoreSlugcatsEnums/SlugcatStatsName::Gourmand
	         *  IL_1dcc: call bool class ExtEnum`1<class SlugcatStats/Name>::op_Equality(class ExtEnum`1<!0>, class ExtEnum`1<!0>)
	         *  IL_1dd1: brtrue.s IL_1ded
             */
            rotundCurs.Emit(OpCodes.Ldarg_0);
            rotundCurs.EmitDelegate((PlayerGraphics self) =>   // If self is Rotundpup, return true
            {
                return self.player.isRotundpup();
            });
            rotundCurs.Emit(OpCodes.Or);
        }
    }
}