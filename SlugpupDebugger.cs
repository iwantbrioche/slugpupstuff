using MoreSlugcats;
using UnityEngine;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Emit;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;

namespace SlugpupStuff
{
    public class SlugpupDebugger
    {
        public class PathingVisualizer
        {
            public class ConnectionSprite : DebugSprite
            {
                public MovementConnection _connection;
                public MovementConnection connection
                {
                    get
                    {
                        return _connection;
                    }
                    set
                    {
                        _connection = value;
                        sprite.color = GetConnectionColor(_connection);
                    }
                }

                public ConnectionSprite(Vector2 ps, Room rm) : base(ps, new FSprite("pixel"), rm)
                {
                    room = rm;
                }

                private Color GetConnectionColor(MovementConnection connection)
                {
                    return connection.type switch
                    {
                        MovementConnection.MovementType.Standard => new Color(0f, 1f, 0f),
                        MovementConnection.MovementType.ReachOverGap => new Color(0f, 1f, 0.5f),
                        MovementConnection.MovementType.ReachUp => new Color(0.8f, 1f, 0.5f),
                        MovementConnection.MovementType.DoubleReachUp => new Color(0f, 1f, 1f),
                        MovementConnection.MovementType.ReachDown => new Color(0.8f, 1f, 0f),
                        MovementConnection.MovementType.SemiDiagonalReach => new Color(1f, 0.8f, 1f),
                        MovementConnection.MovementType.DropToFloor => new Color(1f, 0f, 0f),
                        MovementConnection.MovementType.DropToClimb => new Color(1f, 0.5f, 0f),
                        MovementConnection.MovementType.DropToWater => new Color(0f, 0f, 1f),
                        MovementConnection.MovementType.LizardTurn => new Color(0.5f, 0.5f, 0f),
                        MovementConnection.MovementType.OpenDiagonal => new Color(1f, 0.2f, 0.8f),
                        MovementConnection.MovementType.Slope => new Color(0.8f, 1f, 0f),
                        MovementConnection.MovementType.CeilingSlope => new Color(0.8f, 1f, 0f),
                        MovementConnection.MovementType.ShortCut => new Color(0.6f, 0.6f, 1f),
                        MovementConnection.MovementType.NPCTransportation => new Color(0.1f, 0.1f, 0.1f),
                        _ => Color.white
                    };
                }
            }

            public SlugNPCAI AI;
            public Creature crit;
            public ConnectionSprite[] conSprites;
            public ConnectionSprite[] savedConSprites;
            public DebugSprite[] destSprites;
            public PathingVisualizer(SlugNPCAI pup)
            {
                AI = pup;
                crit = pup.cat;
                conSprites = new ConnectionSprite[2];
                //if (AI.pathFinder is StandardPather standardPather)
                //{
                //    savedConSprites = new ConnectionSprite[standardPather.savedPastConnections];
                //}

                for (int u = 0; u < conSprites.Length; u++)
                {
                    conSprites[u] = new(default, crit.room);
                    conSprites[u].sprite.anchorY = 0f;
                    conSprites[u].sprite.scaleX = 2f;
                    crit.room.AddObject(conSprites[u]);
                }

                if (savedConSprites != null)
                {
                    for (int s = 0; s < savedConSprites.Length; s++)
                    {
                        savedConSprites[s] = new(default, crit.room);
                        savedConSprites[s].sprite.anchorY = 0f;
                        savedConSprites[s].sprite.scaleX = 2f;
                        crit.room.AddObject(savedConSprites[s]);
                    }
                }

                //destSprites = new DebugSprite[3];
                //for (int d = 0; d < destSprites.Length; d++)
                //{
                //    destSprites[d] = new(default, new FSprite("MonkA"), crit.Room.realizedRoom);
                //    destSprites[d].room = crit.Room.realizedRoom;
                //    destSprites[d].sprite.scale = 1f;
                //    crit.Room.realizedRoom.AddObject(destSprites[d]);
                //}
            }

            public void Update()
            {
                for (int u = 0; u < conSprites.Length; u++)
                {
                    if (crit.room != conSprites[u].room)
                    {
                        conSprites[u].room.RemoveObject(conSprites[u]);
                        crit.room.AddObject(conSprites[u]);
                        conSprites[u].room = crit.room;
                    }
                }

                if (savedConSprites != null)
                {
                    for (int s = 0; s < savedConSprites.Length; s++)
                    {
                        if (crit.room != savedConSprites[s].room)
                        {
                            savedConSprites[s].room.RemoveObject(savedConSprites[s]);
                            crit.room.AddObject(savedConSprites[s]);
                            savedConSprites[s].room = crit.room;
                        }
                    }
                }

                //for (int d = 0; d < destSprites.Length; d++)
                //{
                //    if (crit.Room.realizedRoom != destSprites[d].room)
                //    {
                //        destSprites[d].room.RemoveObject(destSprites[d]);
                //        crit.Room.realizedRoom.AddObject(destSprites[d]);
                //        destSprites[d].room = crit.Room.realizedRoom;
                //    }
                //}

            }
            public void VisualizeConnections()
            {

                List<MovementConnection> upcoming = AI.GetUpcoming();
                if (upcoming != null)
                {
                    for (int u = 0; u < upcoming.Count && u < conSprites.Length; u++)
                    {
                        Vector2 vector = crit.room.MiddleOfTile(upcoming[u].startCoord);
                        Vector2 vector2 = crit.room.MiddleOfTile(upcoming[u].destinationCoord);
                        conSprites[u].pos = vector2;
                        conSprites[u].sprite.rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                        conSprites[u].sprite.scaleY = Vector2.Distance(vector2, vector);
                        conSprites[u].connection = upcoming[u];
                        conSprites[u].sprite.isVisible = true;
                    }
                    for (int ua = 0; ua < conSprites.Length; ua++)
                    {
                        if (!upcoming.Contains(conSprites[ua].connection))
                        {
                            conSprites[ua].sprite.isVisible = false;
                        }
                    }
                }
                else
                {
                    for (int ub = 0; ub < conSprites.Length; ub++)
                    {
                        conSprites[ub].sprite.isVisible = false;
                    }
                }

                if (savedConSprites != null)
                {
                    List<MovementConnection> saved = (AI.pathFinder as StandardPather).pastConnections;
                    if (saved != null)
                    {
                        for (int s = 0; s < saved.Count && s < savedConSprites.Length; s++)
                        {
                            Vector2 vector = crit.room.MiddleOfTile(saved[s].startCoord);
                            Vector2 vector2 = crit.room.MiddleOfTile(saved[s].destinationCoord);
                            savedConSprites[s].pos = vector2;
                            savedConSprites[s].sprite.rotation = Custom.AimFromOneVectorToAnother(vector2, vector);
                            savedConSprites[s].sprite.scaleY = Vector2.Distance(vector2, vector);
                            savedConSprites[s].connection = saved[s];
                            savedConSprites[s].sprite.color = Color.white;
                            savedConSprites[s].sprite.alpha = 0.5f;
                            savedConSprites[s].sprite.isVisible = true;
                        }
                        for (int sa = 0; sa < savedConSprites.Length; sa++)
                        {
                            if (!saved.Contains(savedConSprites[sa].connection))
                            {
                                savedConSprites[sa].sprite.isVisible = false;
                            }
                        }
                    }
                    else
                    {
                        for (int sb = 0; sb < savedConSprites.Length; sb++)
                        {
                            savedConSprites[sb].sprite.isVisible = false;
                        }
                    }
                }
                //if (AI.pathFinder.destination != null)
                //{
                //    destSprites[0].pos = crit.Room.realizedRoom.MiddleOfTile(AI.pathFinder.destination);
                //    destSprites[0].sprite.isVisible = true;
                //}
                //else destSprites[0].sprite.isVisible = false;
                //if (AI.pathFinder.nextDestination != null)
                //{
                //    destSprites[1].pos = crit.Room.realizedRoom.MiddleOfTile(AI.pathFinder.nextDestination.Value);
                //    destSprites[1].sprite.isVisible = true;
                //}
                //else destSprites[1].sprite.isVisible = false;
                //if (AI.pathFinder.currentlyFollowingDestination != null)
                //{
                //    destSprites[2].pos = crit.Room.realizedRoom.MiddleOfTile(AI.pathFinder.currentlyFollowingDestination);
                //    destSprites[2].sprite.isVisible = true;
                //}
                //else destSprites[2].sprite.isVisible = false;
            }
        }
        public class DebugLabelManager
        {
            public Creature creature;
            private Dictionary<string, DebugLabel> labelDict;
            private DebugSprite labelLine;
            public DebugLabelManager(Creature crit)
            {
                creature = crit;
                labelDict = new Dictionary<string, DebugLabel>();
                labelLine = new(default, new FSprite("pixel"), creature.room);
                labelLine.sprite.anchorY = 0f;
                labelLine.sprite.scaleX = 2f;
                creature.room.AddObject(labelLine);
            }

            public void Update(Vector2 pos)
            {
                int num = 0;
                Vector2 labelPos = default;
                foreach (var infoLabel in labelDict.Values)
                {
                    if (infoLabel.obj.room == creature.room.world.game.cameras[0].room)
                    {
                        if (!infoLabel.label.isVisible) continue;
                        labelPos = (pos - new Vector2(0f, 15f) * num);
                        infoLabel.label.x = labelPos.x - creature.room.world.game.cameras[0].pos.x;
                        infoLabel.label.y = labelPos.y - creature.room.world.game.cameras[0].pos.y;
                        num++;
                        infoLabel.label.isVisible = true;
                    }
                    else
                    {
                        infoLabel.label.isVisible = false;
                    }

                }
                if (creature.room != null && labelLine.room != creature.room)
                {
                    labelLine.room.RemoveObject(labelLine);
                    creature.room.AddObject(labelLine);
                }
                if (labelPos != default)
                {
                    labelLine.sprite.isVisible = true;
                    labelLine.pos = labelPos - new Vector2(0f, 15f);
                    labelLine.sprite.rotation = Custom.AimFromOneVectorToAnother(labelPos, creature.mainBodyChunk.pos);
                    labelLine.sprite.scaleY = Vector2.Distance(labelPos, creature.mainBodyChunk.pos);
                }
                else
                {
                    labelLine.sprite.isVisible = false;
                }
            }

            public void AddLabel(string name)
            {
                DebugLabel infoLabel = new(creature, default);
                infoLabel.label.color = Color.white;
                Futile.stage.AddChild(infoLabel.label);
                labelDict.Add(name, infoLabel);
            }

            public void UpdateLabel(string name, string label)
            {
                if (labelDict.TryGetValue(name, out var infoLabel))
                {
                    infoLabel.label.text = label;
                }
            }

            public void RemoveLabel(string name)
            {
                if (labelDict.TryGetValue(name, out var infolabel))
                {
                    infolabel.label.RemoveFromContainer();
                    labelDict.Remove(name);
                }
            }

            public void LabelToggle(string name, bool visible)
            {
                if (labelDict.TryGetValue(name, out var infoLabel))
                {
                    if (infoLabel.obj.room == creature.room.world.game.cameras[0].room)
                    {
                        infoLabel.label.isVisible = visible;
                    }

                }
            }
        }
    }


}
