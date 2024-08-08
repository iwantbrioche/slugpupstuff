global using System;
global using System.Linq;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Reflection;
global using BepInEx;
global using MoreSlugcats;
global using Mono.Cecil.Cil;
global using MonoMod.Cil;
global using UnityEngine;
global using RWCustom;
global using Debug = UnityEngine.Debug;
global using Random = UnityEngine.Random;
global using Vector2 = UnityEngine.Vector2;
global using Color = UnityEngine.Color;
global using Custom = RWCustom.Custom;
global using static SlugpupStuff.SlugpupCustom;
global using static SlugpupStuff.SlugpupStuff;
using System.Security;
using System.Security.Permissions;
using BepInEx.Logging;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]


namespace SlugpupStuff
{
    [BepInPlugin(MOD_ID, "Slugpup Stuff", "1.3")]
    public class SlugpupStuff : BaseUnityPlugin
    {
        public const string MOD_ID = "iwantbread.slugpupstuff";
        private bool IsInit;
        private bool PostIsInit;
        public static new ManualLogSource Logger { get; private set; }
        public static SlugpupStuffRemix slugpupRemix;
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.PostModsInit += RainWorld_PostModsInit;
            slugpupRemix = new SlugpupStuffRemix(this);
            Logger = base.Logger;
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
                Hooks.Hooks.PatchAllHooks();

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
                    PupsPlusModCompat.RegisterPupsPlusDebugCommands();
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

        public static class VariantName
        {
            public static SlugcatStats.Name Aquaticpup;
            public static SlugcatStats.Name Tundrapup;
            public static SlugcatStats.Name Hunterpup;
            public static SlugcatStats.Name Rotundpup;

            public static void RegisterValues()
            {
                Aquaticpup = new("Aquaticpup", true);
                Tundrapup = new("Tundrapup", true);
                Hunterpup = new("Hunterpup", true);
                Rotundpup = new("Rotundpup", true);
            }
            public static void UnregisterValues()
            {
                Aquaticpup?.Unregister();
                Aquaticpup = null;
                Tundrapup?.Unregister();
                Tundrapup = null;
                Hunterpup?.Unregister();
                Hunterpup = null;
                Rotundpup?.Unregister();
                Rotundpup = null;
            }
        }

        public static List<int> ID_AquaticPupID()
        {
            List<int> idlist = [];
            return idlist;
        }
        public static List<int> ID_TundraPupID()
        {
            List<int> idlist = [];
            return idlist;

        }
        public static List<int> ID_HunterPupID()
        {
            List<int> idlist =
            [
                1002
            ];
            return idlist;
        }
        public static List<int> ID_RotundPupID()
        {
            List<int> idlist = [];
            return idlist;
        }
        public static List<int> ID_PupIDExclude()
        {
            List<int> idlist =
            [
                1000,
                1001,
                2220,
                3118,
                4118,
                765
            ];
            return idlist;
        }
        public static SlugcatStats.Name GetSlugpupVariant(Player player)
        {

            if (SlugpupStuff.Pearlcat && PupsPlusModCompat.IsPearlpup(player)) return null;

            if (player.abstractCreature.TryGetPupAbstract(out var pupAbstract))
            {
                if (pupAbstract.aquatic) return VariantName.Aquaticpup;
                if (pupAbstract.tundra) return VariantName.Tundrapup;
                if (pupAbstract.hunter) return VariantName.Hunterpup;
                if (pupAbstract.rotund) return VariantName.Rotundpup;
                if (pupAbstract.regular) return null;
            }

            if (!SlugpupStuff.ID_PupIDExclude().Contains(player.abstractCreature.ID.RandomSeed))
            {
                Random.State state = Random.state;
                Random.InitState(player.abstractCreature.ID.RandomSeed);

                float variChance = Random.value;

                Random.state = state;

                // setup variant chance
                if (variChance <= aquaticChance || ID_AquaticPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Aquaticpup;
                }
                else if (variChance <= tundraChance || ID_TundraPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Tundrapup;
                }
                else if (variChance <= hunterchance || ID_HunterPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Hunterpup;
                }
                else if (variChance <= rotundChance || ID_RotundPupID().Contains(player.abstractCreature.ID.RandomSeed))
                {
                    return VariantName.Rotundpup;
                }
            }
            return null;
        }
        public static void VariantMechanicsAquaticpup(Player self)
        {
            if (self.isAquaticpup())
            {
                self.buoyancy = 0.9f;
                if (self.grasps[0] != null && self.grasps[0].grabbed is WaterNut waterNut)
                {
                    waterNut.swellCounter--;
                    if (waterNut.swellCounter < 1)
                    {
                        waterNut.Swell();
                    }
                }
                Player parent = null;
                if (self.grabbedBy.Count > 0 && self.grabbedBy[0].grabber is Player player)
                {
                    parent = player;
                }
                if (parent != null && parent.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Rivulet)
                {
                    if (!self.submerged)
                    {
                        self.slowMovementStun = 5;
                    }
                    if (!parent.monkAscension)
                    {
                        parent.buoyancy = 0.9f;
                    }
                }
            }
        }
        public static void VariantMechanicsRotundpup(Player self)
        {
            Player pupOnBack = null;
            if (self.slugOnBack?.slugcat != null)
            {
                pupOnBack = self.slugOnBack.slugcat;
                while (pupOnBack.slugOnBack?.slugcat != null)
                {
                    if (pupOnBack.isRotundpup()) break;
                    pupOnBack = pupOnBack.slugOnBack.slugcat;
                }
            }
            if (self.TryGetParentVariables(out var parentVariables))
            {
                if (pupOnBack != null && pupOnBack.isRotundpup() && self.aerobicLevel >= 0.9f)
                {
                    parentVariables.rotundPupExhaustion = true;
                }
                if (self.aerobicLevel < 0.6f)
                {
                    parentVariables.rotundPupExhaustion = false;
                }
                if (parentVariables.rotundPupExhaustion)
                {
                    if (pupOnBack != null && pupOnBack.isRotundpup())
                    {
                        self.slowMovementStun = Mathf.Max(self.slowMovementStun, (int)Custom.LerpMap(self.aerobicLevel, 0.35f, 0.2f, 4f, 0f));
                    }
                    self.lungsExhausted = true;
                }
            }
        }

        public static float aquaticChance => (slugpupRemix.aquaticChance.Value - slugpupRemix.tundraChance.Value) / 100f;
        public static float tundraChance => ((slugpupRemix.tundraChance.Value - slugpupRemix.hunterChance.Value) / 100f) + aquaticChance;
        public static float hunterchance => ((slugpupRemix.hunterChance.Value - slugpupRemix.rotundChance.Value) / 100f) + tundraChance;
        public static float rotundChance => (slugpupRemix.rotundChance.Value / 100f) + hunterchance;

        public static bool SlugpupSafari;
        public static bool RainbowPups;
        public static bool Pearlcat;
        public static bool SimplifiedMovesetGourmand;
        public static bool EmeraldsTweaks;
        public static bool BeastMasterPupExtras;



    }
}