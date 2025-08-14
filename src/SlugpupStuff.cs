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
global using Random = UnityEngine.Random;
global using Vector2 = UnityEngine.Vector2;
global using Color = UnityEngine.Color;
global using Custom = RWCustom.Custom;
global using SlugpupStuff.PupsPlusCustom;
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
            DevMode = false;
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
                //throw;
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
                    if (DevMode)
                    {
                        PupsPlusModCompat.RegisterPupsPlusDebugCommands();
                    }

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
            public static SlugcatStats.Name Regular;

            public static void RegisterValues()
            {
                Aquaticpup = new("Aquaticpup", true);
                Tundrapup = new("Tundrapup", true);
                Hunterpup = new("Hunterpup", true);
                Rotundpup = new("Rotundpup", true);
                Regular = new("Regular", true);
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
                Regular?.Unregister();
                Regular = null;
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
        public static bool DevMode;



    }
}