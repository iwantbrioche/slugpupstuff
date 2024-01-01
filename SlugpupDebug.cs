using MoreSlugcats;
using RWCustom;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SlugpupStuff
{
    public partial class SlugpupStuff
    {
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
                    label.isVisible = true;
                }
                public void Update(PhysicalObject _physicalObject, Vector2 pos)
                {
                    physicalObject = _physicalObject;
                    label.SetPosition(pos);
                    label.isVisible = true;
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
                }
            }
            public class UpcomingPathDebug
            {
                public SlugNPCAI pup;
                public DebugSprite sprite;
                public Vector2 pos;
                public Color color;
                public MovementConnection connection;
                public UpcomingPathDebug(SlugNPCAI _pup, DebugSprite _sprite)
                {
                    pup = _pup;
                    sprite = _sprite;
                    color = sprite.sprite.color;
                    sprite.sprite.scale = 14f;
                    pup.creature.Room.realizedRoom.AddObject(sprite);
                }
                public void Update(Color _color, Vector2 _pos, MovementConnection _connection)
                {
                    color = _color;
                    pos = _pos;
                    sprite.sprite.color = color;
                    sprite.pos = pos;
                    sprite.sprite.isVisible = true;
                    connection = _connection;
                }
                public void Disable()
                {
                    sprite.sprite.isVisible = false;
                    connection = null;
                }
            }

            public SlugNPCAI pup;
            public World world;
            public TrackedObjLabel[] debugItemLabels;
            public TrackedObjLabel[] debugInputLabels;
            public TrackedObjLabel[] debugCreatureLabels;
            public DebugDestinationVisualizer destinationVisualizer;
            public DebugTrackerVisualizer trackerVisualizer;
            public UpcomingPathDebug[] debugPathingSprites;
            public SlugpupDebugViz(SlugNPCAI _pup, World _world)
            {
                pup = _pup;
                world = _world;
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
                            if (!pup.creature.Room.creatures.Contains(ghosts.ghost.parent.representedCreature))
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
                            pathingDebug.sprite.sprite.isVisible = false;
                        }
                    }
                }
                else
                {
                    DisableAll();
                }
            }
            public void InitPathingViz(int size)
            {
                debugPathingSprites = new UpcomingPathDebug[size];
                for (int k = 0; k < debugPathingSprites.Length; k++)
                {
                    debugPathingSprites[k] = new UpcomingPathDebug(pup, new DebugSprite(default, new FSprite("pixel"), pup.cat.room));
                }
            }
            public void InitItemLabels()
            {
                debugItemLabels = new TrackedObjLabel[23];

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

                    debugItemLabels[i + 13].Update(rep.representedItem.realizedObject.ToString(), rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos + new Vector2(0f, 12f));
                    debugItemLabels[i + 2].Update(pup.creature.ToString() + " priority: " + rep.priority.ToString(), rep.representedItem.realizedObject, rep.representedItem.realizedObject.firstChunk.pos);

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
                debugInputLabels[1].Update("X: " + pup.cat.input[0].x, pup.cat, pup.cat.firstChunk.pos);
                debugInputLabels[2].Update("Y: " + pup.cat.input[0].y, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f));
                debugInputLabels[3].Update("JMP: " + pup.cat.input[0].jmp, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f * 2));
                debugInputLabels[4].Update("PCKP: " + pup.cat.input[0].pckp, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f * 3));
                debugInputLabels[5].Update("THRW: " + pup.cat.input[0].thrw, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f * 4));
                debugInputLabels[6].Update("FOLLOW: " + Mathf.Round(pup.followCloseness * 100f) * 0.01f, pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f) * 5);
                debugInputLabels[7].Update("BEHAVIOR: " + pup.behaviorType.ToString(), pup.cat, pup.cat.firstChunk.pos - new Vector2(0f, 12f) * 6);
            }
            public void InitCreatureLabels()
            {
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
                    debugPathingSprites[0].Update(Color.blue, pup.creature.Room.realizedRoom.MiddleOfTile(upcoming[0].StartTile), upcoming[0]);
                    for (int k = 1; k < debugPathingSprites.Length && k < upcoming.Count; k++)
                    {
                        Color color = debugPathingSprites[k].color;
                        switch (upcoming[k].type)
                        {
                            case MovementConnection.MovementType.Standard:
                                {
                                    color = Color.green;
                                    break;
                                }
                            case MovementConnection.MovementType.OpenDiagonal:
                                {
                                    color = new Color(0.7f, 1f, 0f);
                                    break;
                                }
                            case MovementConnection.MovementType.ReachOverGap:
                                {
                                    color = Color.magenta;
                                    break;
                                }
                            case MovementConnection.MovementType.ReachUp:
                                {
                                    color = new Color(0.5f, 0f, 1f);
                                    break;
                                }
                            case MovementConnection.MovementType.ReachDown:
                                {
                                    color = new Color(0.5f, 0f, 1f);
                                    break;
                                }
                            case MovementConnection.MovementType.SemiDiagonalReach:
                                {
                                    color = new Color(0.7f, 1f, 0f);
                                    break;
                                }
                            case MovementConnection.MovementType.DropToFloor:
                                {
                                    color = Color.red;
                                    break;
                                }
                            case MovementConnection.MovementType.DropToClimb:
                                {
                                    color = new Color(1f, 0.5f, 0f);
                                    break;
                                }
                            case MovementConnection.MovementType.DropToWater:
                                {
                                    color = Color.blue;
                                    break;
                                }
                            case MovementConnection.MovementType.ShortCut:
                                {
                                    color = Color.cyan;
                                    break;
                                }
                            case MovementConnection.MovementType.BetweenRooms:
                                {
                                    color = Color.yellow;
                                    break;
                                }
                            case MovementConnection.MovementType.Slope:
                                {
                                    color = new Color(0.5f, 0.5f, 0f);
                                    break;
                                }
                            case MovementConnection.MovementType.NPCTransportation:
                                {
                                    color = Color.gray;
                                    break;
                                }
                        }
                        debugPathingSprites[k].Update(color, pup.creature.Room.realizedRoom.MiddleOfTile(upcoming[k].DestTile), upcoming[k]);
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
        }


        public void SlugNPCAI_DebugSprites(On.MoreSlugcats.SlugNPCAI.orig_ctor orig, SlugNPCAI self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (SlugpupCWTs.pupCWT.TryGetValue(self, out var pupVariables) && slugpupRemix.SlugpupDebugVisuals.Value)
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
