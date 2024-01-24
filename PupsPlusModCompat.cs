using System;
using Debug = UnityEngine.Debug;
using DevConsole.Commands;
using MoreSlugcats;
using DevConsole;
using System.Linq;

namespace SlugpupStuff
{
    public class PupsPlusModCompat
    {
        public static void RegisterSpawnPupCommand()
        {
            string[] tags = ["Voidsea", "Winter", "Ignorecycle", "TentacleImmune", "Lavasafe", "AlternateForm", "PreCycle", "Night"];
            string[] variants = ["Aquatic", "Tundra", "Hunter", "Rotund", "Regular"];
            string[] tags2 = [.. tags, .. variants];
            string[] arguments = null;
            new CommandBuilder("spawn_pup")
                .RunGame((game, args) =>
                {
                    arguments = args;
                    try
                    {
                        EntityID? id = null;
                        if (args.Length != 0 && args[0].Contains('.'))
                        {
                            try
                            {
                                id = EntityID.FromString(args[0]);
                            }
                            catch
                            {
                                if (int.TryParse(args[0], out int idNum))
                                    id = new EntityID(0, idNum);
                            }
                        }

                        var abstractPup = new AbstractCreature(game.world,
                            StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC),
                            null,
                            GameConsole.TargetPos.Room.realizedRoom.GetWorldCoordinate(GameConsole.TargetPos.Pos),
                            id ?? game.GetNewID()
                        );

                        if (args.Length != 0)
                            abstractPup.spawnData = "{" + string.Join(",", args.Select((string tag) => tags2.FirstOrDefault((string testTag) => tag.Equals(testTag, StringComparison.OrdinalIgnoreCase)) ?? tag)) + "}";
                        abstractPup.setCustomFlags();

                        GameConsole.TargetPos.Room.AddEntity(abstractPup);

                        if (GameConsole.TargetPos.Room.realizedRoom != null)
                            abstractPup.RealizeInRoom();

                    }
                    catch(Exception ex)
                    {
                        GameConsole.WriteLine("Failed to spawn pup! See console log for more info.");
                        Debug.Log("pup failed:" + ex.ToString());
                    }
                })
                .Help("spawn_pup [ID?] [variant?] [args...]")
                .AutoComplete(arguments => 
                {
                    if (arguments.Length == 0) return variants;
                    else if (arguments.Length == 1 && arguments[0].Contains('.')) return variants;
                    else if (arguments.Length == 1 && !arguments[0].Contains('.') || arguments.Length == 2) return tags;
                    else return null;
                })
                .Register();
        }

        public static void SimpMovesetGourmandOn()
        {
            if (SimplifiedMoveset.MainModOptions.gourmand.Value)
            {
                SlugpupStuff.simpMovesetGourmand = true;
            }
        }

    }
}
