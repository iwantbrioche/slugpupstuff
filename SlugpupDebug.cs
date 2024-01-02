using MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;

namespace SlugpupStuff
{
    public partial class SlugpupStuff
    {
        // unfinished and prone to bugs! specifically the fLabels!!
        // todo:
        // fix room changes
        // creature labels
        public class SlugpupDebugViz
        {
            public class TrackedObjLabel
            {
                public SlugNPCAI pup;
                public PhysicalObject physicalObject;
                public FLabel label;
                public string text;
                public TrackedObjLabel(SlugNPCAI _pup, FLabel _label)
                {
                    pup = _pup;
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
                    if (pup != null)
                    {
                        label.isVisible = true;
                    }
                    else Destroy();
                }
                public void Disable()
                {
                    label.isVisible = false;
                    physicalObject = null;
                }
                public void Destroy()
                {
                    label.RemoveFromContainer();
                    physicalObject = null;
                    pup = null;
                    
                }
            }
            public class PupDebugSprite
            {
                public SlugNPCAI pup;
                public DebugSprite sprite;
                public Vector2 pos;
                public float scale;
                public Color color;
                public MovementConnection connection;
                public Room room;
                public PupDebugSprite(SlugNPCAI _pup, DebugSprite _sprite, float _scale)
                {
                    pup = _pup;
                    sprite = _sprite;
                    scale = _scale;
                    sprite.sprite.scale = scale;
                    room = pup.cat.room;
                    room.AddObject(sprite);
                }
                public void Update(Color _color, Vector2 _pos, MovementConnection _connection)
                {
                    color = _color;
                    pos = _pos;
                    connection = _connection;
                    sprite.sprite.scale = scale;
                    sprite.sprite.color = color;
                    sprite.pos = pos;
                    if (pup != null)
                    {
                        sprite.sprite.isVisible = true;
                    }
                    else Destroy();
                }
                public void Update(Color _color, Vector2 _pos)
                {
                    color = _color;
                    pos = _pos;
                    sprite.sprite.scale = scale;
                    sprite.sprite.color = color;
                    sprite.pos = pos;
                    if (pup != null)
                    {
                        sprite.sprite.isVisible = true;
                    }
                    else Destroy();
                }
                public void Disable()
                {
                    sprite.sprite.isVisible = false;
                    connection = null;
                }
                public void Destroy()
                {
                    sprite.Destroy();
                    connection = null;
                    pup = null;
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
            public RoomCamera rCam;
            public TrackedObjLabel[] debugItemLabels;
            public TrackedObjLabel[] debugInputLabels;
            public TrackedObjLabel[] debugTrackerLabels;
            public DebugDestinationVisualizer destinationVisualizer;
            public DebugTrackerVisualizer trackerVisualizer;
            public PupDebugSprite[] debugPathingSprites;
            public PupDebugSprite[] idlePosSprites;
            public SlugpupDebugViz(SlugNPCAI _pup, World _world)
            {
                pup = _pup;
                world = _world;
                room = pup.cat.room;
                rCam = room.world.game.cameras.FirstOrDefault((cam) => cam.room == room);
            }

            public void Initiate()
            {
                destinationVisualizer = new DebugDestinationVisualizer(world.game.abstractSpaceVisualizer, world, pup.pathFinder, Color.red);
                trackerVisualizer = new DebugTrackerVisualizer(pup.tracker);
                pup.itemTracker.visualize = true;
                InitPathingDebug(20);
                InitItemDebug();
                InitInputDebug();
            }
            public void Update()
            {
                rCam = world.game.cameras.FirstOrDefault((cam) => cam.room == room);
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
                        UpdateItemDebug();
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
                        UpdateInputDebug();
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
                        UpdatePathingDebug();
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
                        foreach (var idleDebug in idlePosSprites)
                        {
                            idleDebug.Disable();
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
            public void InitPathingDebug(int size)
            {
                debugPathingSprites = new PupDebugSprite[size];
                idlePosSprites = new PupDebugSprite[2];
                for (int k = 0; k < debugPathingSprites.Length; k++)
                {
                    debugPathingSprites[k] = new PupDebugSprite(pup, new DebugSprite(default, new FSprite("pixel"), room), 14f);
                }
                for (int i = 0; i < idlePosSprites.Length; i++)
                {
                    idlePosSprites[i] = new PupDebugSprite(pup, new DebugSprite(default, new FSprite("MonkA"), room), 1f);
                }
            }
            public void UpdatePathingDebug()
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
                        rCam.ReturnFContainer("Foreground").AddChild(debugPathingSprites[ka].sprite.sprite);
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
                if (pup.testIdlePos != null)
                {
                    idlePosSprites[0].Update(Color.cyan, room.MiddleOfTile(pup.testIdlePos));
                    rCam.ReturnFContainer("ForegroundLights").AddChild(idlePosSprites[0].sprite.sprite);
                }
                else idlePosSprites[0].Disable();

                if (pup.lastIdleSpot != null && pup.lastIdleSpot.HasValue)
                {
                    idlePosSprites[1].Update(Color.black, room.MiddleOfTile(pup.lastIdleSpot.Value));
                    rCam.ReturnFContainer("ForegroundLights").AddChild(idlePosSprites[1].sprite.sprite);
                }
                else idlePosSprites[1].Disable();
            }
            public void InitItemDebug()
            {
                debugItemLabels = new TrackedObjLabel[pup.itemTracker.maxTrackedItems];

                for (int i = 0; i < debugItemLabels.Length; i++)
                {
                    debugItemLabels[i] = new TrackedObjLabel(pup, new FLabel(Custom.GetFont(), ""));
                }
            }
            public void UpdateItemDebug()
            {
                if (!SlugpupCWTs.pupCWT.TryGetValue(pup, out var pupVariables))
                {
                    return;
                }
                for (int i = 0; i < pup.itemTracker.ItemCount && pup.itemTracker.visualize; i++)
                {
                    ItemTracker.ItemRepresentation rep = pup.itemTracker.items[i];
                    rep.dbSpr.sprite.isVisible = true;

                    string text = rep.representedItem.realizedObject.ToString() + "\n" +
                        pup.creature.ToString() + " priority: " + rep.priority.ToString() + "\n";
                    string grab = pup.creature.ToString() + " grabTarget\n";
                    string wanted = pup.creature.ToString() + " wantedObject\n";
                    string gifted = pup.creature.ToString() + " giftedObject\n";
                    if (pup.grabTarget != null && pup.grabTarget == rep.representedItem.realizedObject)
                    {
                        rep.dbSpr.sprite.color = Color.green;
                        text += grab;
                    }

                    if (pupVariables.wantedObject != null && pupVariables.wantedObject == rep.representedItem.realizedObject)
                    {
                        rep.dbSpr.sprite.color = Color.blue;
                        text += wanted;
                    }

                    if ((pup.friendTracker.giftOfferedToMe?.item != null && pup.friendTracker.giftOfferedToMe.item == rep.representedItem.realizedObject) || (pupVariables.giftedObject != null && pupVariables.giftedObject == rep.representedItem.realizedObject))
                    {
                        rep.dbSpr.sprite.color = new Color(1f, 0f, 1f);
                        text += gifted;
                    }
                    debugItemLabels[i].Update(text, rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos + new Vector2(-20f, 50f));
                }
                foreach (var itemLabels in debugItemLabels)
                {
                    if (!pup.cat.room.updateList.Contains(itemLabels.physicalObject) || itemLabels.physicalObject == null)
                    {
                        itemLabels.Disable();
                    }
                }
            }
            public void InitInputDebug()
            {
                debugInputLabels = new TrackedObjLabel[1];
                debugInputLabels[0] = new TrackedObjLabel(pup, new FLabel(Custom.GetFont(), ""));
            }
            public void UpdateInputDebug()
            {
                string text = pup.cat.ToString() + "\n" +
                    "X: " + pup.cat.input[0].x + "\n" +
                    "Y: " + pup.cat.input[0].y + "\n" +
                    "JMP: " + pup.cat.input[0].jmp + "\n" +
                    "PCKP: " + pup.cat.input[0].pckp + "\n" +
                    "THRW: " + pup.cat.input[0].thrw + "\n" +
                    "FOLLOW: " + Mathf.Round(pup.followCloseness * 100f) * 0.01f + "\n" +
                    "BEHAVIOR: " + pup.behaviorType.ToString();

                debugInputLabels[0].Update(text, pup.cat, pup.cat.firstChunk.pos + new Vector2(-20f, 85f));
            }
            public void InitCreatureDebug()
            {
                // show relationship to pup, what tracker is used, etc.
                debugTrackerLabels = new TrackedObjLabel[pup.tracker.maxTrackedCreatures];
                for (int t = 0; t < debugTrackerLabels.Length; t++)
                {
                    debugTrackerLabels[t] = new TrackedObjLabel(pup, new FLabel(Custom.GetFont(), ""));
                }
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
        public void Slugpup_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
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
