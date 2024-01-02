﻿using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;

namespace SlugpupStuff
{
    public partial class SlugpupStuff
    {
        // unfinished and prone to bugs! specifically the fLabels!!
        public class SlugpupDebugViz
        {
            public class TrackedObjLabel
            {
                public PhysicalObject physicalObject;
                public FLabel label;
                public string text;
                public TrackedObjLabel(FLabel _label, PhysicalObject _physicalObject)
                {
                    text = _label.text;
                    label = _label;
                    physicalObject = _physicalObject;

                    Futile.stage.AddChild(label);
                }
                public TrackedObjLabel(FLabel _label)
                {
                    text = _label.text;
                    label = _label;
                    physicalObject = null;

                    Futile.stage.AddChild(label);
                }
                public void Update(string _text, PhysicalObject _physicalObject, Vector2 pos)
                {
                    physicalObject = _physicalObject;
                    text = _text;
                    label.text = text;
                    label.SetPosition(pos);
                    Enable();
                }
                public void Update(PhysicalObject _physicalObject, Vector2 pos)
                {
                    physicalObject = _physicalObject;
                    label.SetPosition(pos);
                    Enable();
                }
                public void Disable()
                {
                    label.isVisible = false;
                    physicalObject = null;
                }
                public void Enable()
                {
                    label.isVisible = true;
                }
                public void Destroy()
                {
                    label.RemoveFromContainer();
                    physicalObject = null;
                }
            }
            public class PupPathingSprite
            {
                public SlugNPCAI pup;
                public DebugSprite sprite;
                public Vector2 pos;
                public Color color;
                public MovementConnection connection;
                public Room room;
                public PupPathingSprite(SlugNPCAI _pup, DebugSprite _sprite)
                {
                    pup = _pup;
                    sprite = _sprite;
                    sprite.sprite.scale = 14f;
                    room = pup.cat.room;
                    room.AddObject(sprite);
                }
                public void Update(Color _color, Vector2 _pos, MovementConnection _connection)
                {
                    color = _color;
                    pos = _pos;
                    sprite.sprite.color = color;
                    sprite.pos = pos;
                    connection = _connection;
                    Enable();
                }
                public void Update(Color _color, Vector2 _pos)
                {
                    color = _color;
                    pos = _pos;
                    sprite.sprite.color = color;
                    sprite.pos = pos;
                    Enable();
                }
                public void Update(Color _color, Vector2 _pos, DebugSprite _sprite)
                {
                    color = _color;
                    pos = _pos;
                    sprite = _sprite;
                    sprite.sprite.color = color;
                    sprite.pos = pos;
                    Enable();
                }
                public void Disable()
                {
                    sprite.sprite.isVisible = false;
                    connection = null;
                }
                public void Enable()
                {
                    sprite.sprite.isVisible = true;
                }
                public void Destroy()
                {
                    sprite.Destroy();
                    connection = null;
                }
                public void ChangeRooms(Room newRoom)
                {
                    room.RemoveObject(sprite);
                    newRoom.AddObject(sprite);
                    room = newRoom;
                }
            }

            public SlugNPCAI pup;
            public World world;
            public Room room;
            public TrackedObjLabel[] debugItemLabels;
            public TrackedObjLabel[] debugInputLabels;
            public TrackedObjLabel[] debugCreatureLabels;
            public DebugDestinationVisualizer destinationVisualizer;
            public DebugTrackerVisualizer trackerVisualizer;
            public PupPathingSprite[] debugPathingSprites;
            public SlugpupDebugViz(SlugNPCAI _pup, World _world)
            {
                pup = _pup;
                world = _world;
                room = pup.cat.room;
            }

            public void Initiate()
            {
                destinationVisualizer = new DebugDestinationVisualizer(world.game.abstractSpaceVisualizer, world, pup.pathFinder, Color.red);
                trackerVisualizer = new DebugTrackerVisualizer(pup.tracker);
                pup.itemTracker.visualize = true;
                InitPathingViz(20);
                InitItemLabels();
                InitInputLabels();
            }
            public void Update()
            {
                if (debugViz)
                {
                    if (debugTracker)
                    {
                        trackerVisualizer.Update();
                        foreach (var ghosts in trackerVisualizer.spritesAndGhosts)
                        {
                            ghosts.sprite.sprite.isVisible = true;
                            ghosts.sprite2.sprite.isVisible = true;
                            if (!room.abstractRoom.creatures.Contains(ghosts.ghost.parent.representedCreature))
                            {
                                ghosts.sprite.sprite.isVisible = false;
                                ghosts.sprite2.sprite.isVisible = false;
                            }
                        }
                    }
                    else
                    {
                        foreach (var ghosts in trackerVisualizer.spritesAndGhosts)
                        {
                            ghosts.sprite.sprite.isVisible = false;
                            ghosts.sprite2.sprite.isVisible = false;
                        }
                    }
                    if (debugItems)
                    {
                        UpdateItemLabels();
                    }
                    else
                    {
                        foreach (var itemLabels in debugItemLabels)
                        {
                            itemLabels.Disable();
                        }
                        foreach (var itemDebug in pup.itemTracker.items)
                        {
                            itemDebug.dbSpr.sprite.isVisible = false;
                        }
                    }
                    if (debugInputs)
                    {
                        UpdateInputLabels();
                    }
                    else
                    {
                        foreach (var inputLabels in debugInputLabels)
                        {
                            inputLabels.Disable();
                        }
                    }
                    if (debugPath)
                    {
                        UpdatePathingViz();
                    }
                    else
                    {
                        destinationVisualizer.sprite1.sprite.isVisible = false;
                        destinationVisualizer.sprite2.sprite.isVisible = false;
                        destinationVisualizer.sprite3.sprite.isVisible = false;
                        destinationVisualizer.sprite4.sprite.isVisible = false;
                        foreach (var pathingDebug in debugPathingSprites)
                        {
                            pathingDebug.Disable();
                        }
                    }
                    if (!pup.cat.Consious)
                    {
                        DisableAll();
                    }
                }
                else
                {
                    DisableAll();
                }
            }
            public void InitPathingViz(int size)
            {
                debugPathingSprites = new PupPathingSprite[size + 1];
                for (int k = 0; k < debugPathingSprites.Length && k < size; k++)
                {
                    debugPathingSprites[k] = new PupPathingSprite(pup, new DebugSprite(default, new FSprite("pixel"), room));
                }
                debugPathingSprites[size + 1] = new PupPathingSprite(pup, new DebugSprite(default, new FSprite(""), room));  
            }
            public void UpdatePathingViz()
            {
                destinationVisualizer.Update();
                destinationVisualizer.sprite1.sprite.isVisible = true;
                destinationVisualizer.sprite2.sprite.isVisible = true;
                destinationVisualizer.sprite3.sprite.isVisible = true;
                destinationVisualizer.sprite4.sprite.isVisible = true;
                List<MovementConnection> upcoming = pup.GetUpcoming();
                if (upcoming != null)
                {
                    debugPathingSprites[0].Update(pup.cat.ShortCutColor(), room.MiddleOfTile(upcoming[0].StartTile), upcoming[0]);
                    for (int k = 1; k < debugPathingSprites.Length && k < upcoming.Count; k++)
                    {
                        Color color = GetMovementColor(upcoming[k]);
                        debugPathingSprites[k].Update(color, room.MiddleOfTile(upcoming[k].DestTile), upcoming[k]);
                        if (upcoming[k] == null)
                        {
                            debugPathingSprites[k].sprite.sprite.isVisible = false;
                        }
                    }
                    for (int ka = 0; ka < debugPathingSprites.Length; ka++)
                    {
                        if (!upcoming.Contains(debugPathingSprites[ka].connection))
                        {
                            debugPathingSprites[ka].sprite.sprite.isVisible = false;
                        }
                    }
                }
                else
                {
                    foreach (var pathingSprites in debugPathingSprites)
                    {
                        pathingSprites.Disable();
                    }
                }
            }
            public void InitItemLabels()
            {
                debugItemLabels = new TrackedObjLabel[3 + pup.itemTracker.maxTrackedItems * 2];

                debugItemLabels[0] = new TrackedObjLabel(new FLabel(Custom.GetFont(), pup.creature.ToString() + " grabTarget"));
                debugItemLabels[1] = new TrackedObjLabel(new FLabel(Custom.GetFont(), pup.creature.ToString() + " wantedObject"));
                debugItemLabels[2] = new TrackedObjLabel(new FLabel(Custom.GetFont(), pup.creature.ToString() + " giftedObject"));
                for (int i = 3; i < debugItemLabels.Length; i++)
                {
                    debugItemLabels[i] = new TrackedObjLabel(new FLabel(Custom.GetFont(), ""));
                }
            }
            public void UpdateItemLabels()
            {
                if (!SlugpupCWTs.pupCWT.TryGetValue(pup, out var pupVariables))
                {
                    return;
                }
                for (int i = 0; i < pup.itemTracker.ItemCount && pup.itemTracker.visualize; i++)
                {
                    ItemTracker.ItemRepresentation rep = pup.itemTracker.items[i];
                    rep.dbSpr.sprite.isVisible = true;

                    debugItemLabels[i + pup.itemTracker.maxTrackedItems + 3].Update(rep.representedItem.realizedObject.ToString(), rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos + new Vector2(0f, 12f));
                    debugItemLabels[i + 3].Update(pup.creature.ToString() + " priority: " + rep.priority.ToString(), rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos);

                    if (pup.grabTarget != null && pup.grabTarget == rep.representedItem.realizedObject)
                    {
                        rep.dbSpr.sprite.color = Color.green;
                        debugItemLabels[0].Update(rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos - new Vector2(0f, 12f));
                    }
                    else
                    {
                        debugItemLabels[0].Disable();
                    }
                    if (pupVariables.wantedObject != null && pupVariables.wantedObject == rep.representedItem.realizedObject)
                    {
                        rep.dbSpr.sprite.color = Color.blue;
                        debugItemLabels[1].Update(rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos - new Vector2(0f, 24f));
                    }
                    else
                    {
                        debugItemLabels[1].Disable();
                    }
                    if ((pup.friendTracker.giftOfferedToMe?.item != null && pup.friendTracker.giftOfferedToMe.item == rep.representedItem.realizedObject) || (pupVariables.giftedObject != null && pupVariables.giftedObject == rep.representedItem.realizedObject))
                    {
                        rep.dbSpr.sprite.color = new Color(1f, 0f, 1f);
                        debugItemLabels[2].Update(rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos - new Vector2(0f, 36f));
                    }
                    else
                    {
                        debugItemLabels[2].Disable();
                    }
                }
                foreach (var itemLabels in debugItemLabels)
                {
                    if (!pup.cat.room.updateList.Contains(itemLabels.physicalObject))
                    {
                        itemLabels.Disable();
                    }
                    if (itemLabels.physicalObject == null)
                    {
                        itemLabels.Disable();
                    }
                }
            }
            public void InitInputLabels()
            {
                debugInputLabels = new TrackedObjLabel[8];

                debugInputLabels[0] = new TrackedObjLabel(new FLabel(Custom.GetFont(), pup.creature.ToString()));
                for (int h = 1; h < debugInputLabels.Length; h++)
                {
                    debugInputLabels[h] = new TrackedObjLabel(new FLabel(Custom.GetFont(), ""));
                }
            }
            public void UpdateInputLabels()
            {
                debugInputLabels[0].Update(pup.cat, pup.cat.firstChunk.pos);
                debugInputLabels[1].Update("X: " + pup.cat.input[0].x, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f));
                debugInputLabels[2].Update("Y: " + pup.cat.input[0].y, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f * 2));
                debugInputLabels[3].Update("JMP: " + pup.cat.input[0].jmp, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f * 3));
                debugInputLabels[4].Update("PCKP: " + pup.cat.input[0].pckp, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f * 4));
                debugInputLabels[5].Update("THRW: " + pup.cat.input[0].thrw, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f * 5));
                debugInputLabels[6].Update("FOLLOW: " + Mathf.Round(pup.followCloseness * 100f) * 0.01f, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f) * 6);
                debugInputLabels[7].Update("BEHAVIOR: " + pup.behaviorType.ToString(), pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f) * 7);
            }
            public void InitCreatureLabels()
            {
                // show relationship to pup, what tracker is used, etc.
            }



            public void DisableAll()
            {
                destinationVisualizer.sprite1.sprite.isVisible = false;
                destinationVisualizer.sprite2.sprite.isVisible = false;
                destinationVisualizer.sprite3.sprite.isVisible = false;
                destinationVisualizer.sprite4.sprite.isVisible = false;

                foreach (var ghosts in trackerVisualizer.spritesAndGhosts)
                {
                    ghosts.sprite.sprite.isVisible = false;
                    ghosts.sprite2.sprite.isVisible = false;
                }
                foreach (var itemDebug in pup.itemTracker.items)
                {
                    itemDebug.dbSpr.sprite.isVisible = false;
                }
                foreach (var pathingSprites in debugPathingSprites)
                {
                    pathingSprites.Disable();
                }
                foreach (var itemLabels in debugItemLabels)
                {
                    itemLabels.Disable();
                }
                foreach (var inputLabels in debugInputLabels)
                {
                    inputLabels.Disable();
                }
            }

            public void DestroyAll()
            {
                destinationVisualizer.sprite1.Destroy();
                destinationVisualizer.sprite2.Destroy();
                destinationVisualizer.sprite3.Destroy();
                destinationVisualizer.sprite4.Destroy();

                trackerVisualizer.ClearSprites();
                pup.itemTracker.visualize = false;
                foreach (var pathingSprites in debugPathingSprites)
                {
                    pathingSprites.Destroy();
                }
                foreach (var itemLabels in debugItemLabels)
                {
                    itemLabels.Destroy();
                }
                foreach (var inputLabels in debugInputLabels)
                {
                    inputLabels.Destroy();
                }
            }
            public Color GetMovementColor(MovementConnection movementConnection)
            {
                Color color = movementConnection.type switch
                {
                    MovementConnection.MovementType.Standard => Color.green,
                    MovementConnection.MovementType.OpenDiagonal => new Color(0.7f, 1f, 0f),
                    MovementConnection.MovementType.ReachOverGap => Color.magenta,
                    MovementConnection.MovementType.ReachUp => new Color(0.5f, 0f, 1f),
                    MovementConnection.MovementType.ReachDown => new Color(0.5f, 0f, 1f),
                    MovementConnection.MovementType.SemiDiagonalReach => new Color(0.7f, 1f, 0f),
                    MovementConnection.MovementType.DropToFloor => Color.red,
                    MovementConnection.MovementType.DropToClimb => new Color(1f, 0.5f, 0f),
                    MovementConnection.MovementType.DropToWater => Color.blue,
                    MovementConnection.MovementType.ShortCut => Color.cyan,
                    MovementConnection.MovementType.BetweenRooms => Color.yellow,
                    MovementConnection.MovementType.Slope => new Color(0.5f, 0.5f, 0f),
                    MovementConnection.MovementType.NPCTransportation => Color.gray,
                    _ => Color.white,
                };
                Color.RGBToHSV(color, out float H, out float S, out float V);
                var cost = pup.pathFinder.CoordinateCost(movementConnection.destinationCoord);
                if (cost.resistance != 1f)
                {
                    S /= cost.resistance / 1.25f;
                }

                return Color.HSVToRGB(H, S, V);
            }

            public void ChangeRooms(Room newRoom)
            {
                destinationVisualizer.ChangeRooms(newRoom);
                foreach(var pathingSprites in debugPathingSprites)
                {
                    pathingSprites.ChangeRooms(newRoom);
                }
            }
        }


        public void SlugNPCAI_DebugSprites(On.MoreSlugcats.SlugNPCAI.orig_ctor orig, SlugNPCAI self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (SlugpupCWTs.pupCWT.TryGetValue(self, out var pupVariables))
            {
                pupVariables.debugViz = new SlugpupDebugViz(self, world);
                pupVariables.debugViz.Initiate();
            }
        }
        public void SlugNPCAI_DebugUpdate(On.MoreSlugcats.SlugNPCAI.orig_Update orig, SlugNPCAI self)
        {
            orig(self);
            if (SlugpupCWTs.pupCWT.TryGetValue(self, out var pupVariables))
            {
                pupVariables.debugViz.Update();
            }
        }

        public void Slugpup_Die(On.Player.orig_Die orig, Player self)
        {
            orig(self);
            if (self.isNPC && SlugpupCWTs.pupCWT.TryGetValue(self.AI, out var pupVariables))
            {
                pupVariables.debugViz.DisableAll();
            }
        }
        public void Slugpup_DestroyDebug(On.AbstractRoom.orig_RemoveEntity_AbstractWorldEntity orig, AbstractRoom self, AbstractWorldEntity ent)
        {
            if (ent is AbstractCreature && (ent as AbstractCreature).realizedCreature is Player && ((ent as AbstractCreature).realizedCreature as Player).isNPC)
            {
                SlugNPCAI pup = ((ent as AbstractCreature).realizedCreature as Player).AI;
                if (SlugpupCWTs.pupCWT.TryGetValue(pup, out var pupVariables))
                {
                    pupVariables.debugViz.DestroyAll();
                }
            }
            orig(self, ent);
        }
        private void Slugpup_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
        {
            if (self.isNPC && SlugpupCWTs.pupCWT.TryGetValue(self.AI, out var pupVariables))
            {
                pupVariables.debugViz.ChangeRooms(newRoom);
            }
            orig(self, newRoom);
        }
        public void Slugpup_DebugToggles(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            orig(self);
            if (!self.devToolsActive)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.LeftBracket) && debugViz)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("disable slugpup debug visuals");
                }
                debugViz = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.Quote) && debugViz && debugItems)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("disable slugpup item debug visuals");
                }
                debugItems = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.Backslash) && debugViz && debugPath)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("disable slugpup pathing debug visuals");
                }
                debugPath = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.RightBracket) && debugViz && debugTracker)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("disable slugpup tracking debug visuals");
                }
                debugTracker = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.Semicolon) && debugViz && debugCreatures)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("disable slugpup creature visuals");
                }
                debugCreatures = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.Equals) && debugViz && debugInputs)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("disable slugpup input visuals");
                }
                debugInputs = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("enable slugpup debug visuals");
                }
                debugViz = true;
            }
            if (Input.GetKeyDown(KeyCode.Quote) && debugViz)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("enable slugpup item debug visuals");
                }
                debugItems = true;
            }
            if (Input.GetKeyDown(KeyCode.Backslash) && debugViz)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("enable slugpup pathing debug visuals");
                }
                debugPath = true;
            }
            if (Input.GetKeyDown(KeyCode.RightBracket) && debugViz)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("enable slugpup tracking debug visuals");
                }
                debugTracker = true;
            }
            if (Input.GetKeyDown(KeyCode.Semicolon) && debugViz)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("enable slugpup creature visuals");
                }
                debugCreatures = true;
            }
            if (Input.GetKeyDown(KeyCode.Equals) && debugViz)
            {
                if (RainWorld.ShowLogs)
                {
                    Debug.Log("enable slugpup input visuals");
                }
                debugInputs = true;
            }
        }

        public static bool debugViz = false;
        public static bool debugItems = true;
        public static bool debugPath = true;
        public static bool debugTracker = true;
        public static bool debugCreatures = true;
        public static bool debugInputs = false;
    }
}
