using BepInEx.Logging;
using DevInterface;
using Menu;
using Menu.Remix.MixedUI;
using RWCustom;
using System;
using UnityEngine;

namespace SlugpupStuff
{
    internal class SlugpupRemix : OptionInterface
    {
        private readonly ManualLogSource logger;

        const float TITLEX = 20f;
        const float TITLEY = 540f;
        const float CHECKBOXX = 60f;
        const float CHECKBOXY = 500f;
        const float CHECKBOXYOFFSET = 37.5f;
        const float SLIDERX = 65f;
        const float SLIDERY = 260f;
        const float SLIDERYOFFSET = 50f;
        public SlugpupRemix(SlugpupStuff slugpupStuff, ManualLogSource logSource)
        {
            logger = logSource;

            SaintTundraGrapple = config.Bind("SlugpupStuff_SaintTundraGrapple", true);
            BackTundraGrapple = config.Bind("SlugpupStuff_BackTundraGrapple", true);
            ManualItemGen = config.Bind("SlugpupStuff_ManualItemGem", false);
            aquaticChance = config.Bind("SlugpupStuff_aquaticChance", 70);
            tundraChance = config.Bind("SlugpupStuff_tundraChance", 52);
            hunterChance = config.Bind("SlugpupStuff_hunterChance", 41);
            rotundChance = config.Bind("SlugpupStuff_rotundChance", 16);
        }

        public readonly Configurable<bool> SaintTundraGrapple;
        public readonly Configurable<bool> BackTundraGrapple;
        public readonly Configurable<bool> ManualItemGen;
        public readonly Configurable<int> aquaticChance;
        public readonly Configurable<int> tundraChance;
        public readonly Configurable<int> hunterChance;
        public readonly Configurable<int> rotundChance;
        public OpSlider OPaquaticSlider;
        public OpSlider OPtundraSlider;
        public OpSlider OPhunterSlider;
        public OpSlider OProtundSlider;
        public OpSimpleButton resetButton;
        public OpSimpleButton zeroButton;
        public OpLabel OPaquaticMax;
        public OpLabel OPtundraMax;
        public OpLabel OPhunterMax;
        public OpLabel OProtundMax;
        public OpLabel OPaquaticChance;
        public OpLabel OPtundraChance;
        public OpLabel OProtundChance;
        public OpLabel OPhunterChance;
        public OpLabel OPregularChance;


        public override void Initialize()
        {
            base.Initialize();

            var mainConfig = new OpTab(this, "Options");
            Tabs = [mainConfig];

            // CHECKBOXES
            mainConfig.AddItems(
                new OpLabel(TITLEX, TITLEY, "Toggles", true),
                new OpCheckBox(SaintTundraGrapple, new(CHECKBOXX, CHECKBOXY)),
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY, "Toggle using Tundrapup grapple as Saint"),
                new OpCheckBox(BackTundraGrapple, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET)),
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET, "Toggle using Tundrapup grapple while pup is on back"),
                new OpCheckBox(ManualItemGen, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET * 2f)),
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET * 2f, "Toggle regurgitating random items by using UP + GRAB on Rotundpups")
                );
            // SLIDERS
            OPaquaticSlider = new(aquaticChance, new(SLIDERX, SLIDERY), 100)
            {
                size = new(300f, 30f),
            };
            OPtundraSlider = new(tundraChance, new(SLIDERX, SLIDERY - SLIDERYOFFSET), 100)
            {
                size = new(300f, 30f),
            };
            OPhunterSlider = new(hunterChance, new(SLIDERX, SLIDERY - SLIDERYOFFSET * 2f), 100)
            {
                size = new(300f, 30f),
            };
            OProtundSlider = new(rotundChance, new(SLIDERX, SLIDERY - SLIDERYOFFSET * 3f), 100)
            {
                size = new(300f, 30f),
            };
            resetButton = new(new(SLIDERX + 275f, SLIDERY + 55f), new(45f, 10f), "reset")
            {
                description = "Reset all variant chances to default"
            };
            zeroButton = new(new(SLIDERX + 220f, SLIDERY + 55f), new(45f, 10f), "tare")
            {
                description = "Set all variant chances to zero"
            };
            OPaquaticMax = new(SLIDERX + 315f, SLIDERY + 6f, "");
            OPtundraMax = new(SLIDERX + 315f, SLIDERY - SLIDERYOFFSET + 6f, "");
            OPhunterMax = new(SLIDERX + 315f, SLIDERY - (SLIDERYOFFSET * 2f) + 6f, "");
            OProtundMax = new(SLIDERX + 315f, SLIDERY - (SLIDERYOFFSET * 3f) + 6f, "");
            OPaquaticChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 3.5f), "");
            OPtundraChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 3.5f) - 20f, "");
            OPhunterChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 3.5f) - 40f, "");
            OProtundChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 3.5f) - 60f, "");
            OPregularChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 3.5f) - 80f, "");
            mainConfig.AddItems(
                new OpLabel(TITLEX, SLIDERY + 80f, "Variant Chances", true),
                new OpLabel(SLIDERX, SLIDERY + 30f, "Aquaticpup Chance"),
                new OpLabel(SLIDERX, SLIDERY - SLIDERYOFFSET + 30f, "Tundrapup Chance"),
                new OpLabel(SLIDERX, SLIDERY - (SLIDERYOFFSET * 2f) + 30f, "Hunterpup Chance"),
                new OpLabel(SLIDERX, SLIDERY - (SLIDERYOFFSET * 3f) + 30f, "Rotundpup Chance"),
                OPaquaticSlider,
                OPtundraSlider,
                OProtundSlider,
                OPhunterSlider,
                resetButton,
                zeroButton,
                OPaquaticMax,
                OPtundraMax,
                OPhunterMax,
                OProtundMax,
                OPaquaticChance,
                OPtundraChance,
                OPhunterChance,
                OProtundChance,
                OPregularChance
                );
        }

        public override void Update()
        {
            int aquaticValue = int.Parse(OPaquaticSlider.value);
            int tundraValue = int.Parse(OPtundraSlider.value);
            int hunterValue = int.Parse(OPhunterSlider.value);
            int rotundValue = int.Parse(OProtundSlider.value);

            int hunterMax = Mathf.Clamp(100 - rotundValue, 0, 100);
            int tundraMax = Mathf.Clamp(100 - hunterValue, 0, 100);
            int aquaticMax = Mathf.Clamp(100 - tundraValue, 0, 100);

            if (hunterValue - rotundValue <= 0)
            {
                OPhunterSlider.value = rotundValue.ToString();
            }
            if (tundraValue - hunterValue <= 0)
            {
                OPtundraSlider.value = hunterValue.ToString();
            }
            if (aquaticValue - tundraValue <= 0)
            {
                OPaquaticSlider.value = tundraValue.ToString();
            }

            OProtundSlider._label.text = rotundValue + "%";
            OPhunterSlider._label.text = Mathf.Clamp(hunterValue - rotundValue, 0, hunterMax) + "%";
            OPtundraSlider._label.text = Mathf.Clamp(tundraValue - hunterValue, 0, tundraMax) + "%";
            OPaquaticSlider._label.text = Mathf.Clamp(aquaticValue - tundraValue, 0, aquaticMax) + "%";

            OProtundMax.text = "max 100%";
            OPhunterMax.text = "max " + hunterMax.ToString() + "%";
            OPtundraMax.text = "max " + tundraMax.ToString() + "%";
            OPaquaticMax.text = "max " + aquaticMax.ToString() + "%";

            OPaquaticChance.text = aquaticValue - tundraValue + "% chance to spawn as Aquaticpup";
            OPtundraChance.text = tundraValue - hunterValue + "% chance to spawn as Tundrapup";
            OPhunterChance.text = hunterValue - rotundValue + "% chance to spawn as Hunterpup";
            OProtundChance.text = rotundValue + "% chance to spawn as Rotundpup";
            OPregularChance.text = 100 - aquaticValue + "% chance to spawn as a regular pup";


            resetButton.OnClick += delegate
            {
                aquaticChance.Value = 70;
                OPaquaticSlider.value = aquaticChance.defaultValue;
                tundraChance.Value = 52;
                OPtundraSlider.value = tundraChance.defaultValue;
                hunterChance.Value = 41;
                OPhunterSlider.value = hunterChance.defaultValue;
                rotundChance.Value = 16;
                OProtundSlider.value = rotundChance.defaultValue;
            };

            zeroButton.OnClick += delegate
            {
                aquaticChance.Value = 0;
                OPaquaticSlider.value = 0.ToString();
                tundraChance.Value = 0;
                OPtundraSlider.value = 0.ToString();
                hunterChance.Value = 0;
                OPhunterSlider.value = 0.ToString();
                rotundChance.Value = 0;
                OProtundSlider.value = 0.ToString();
            };
            base.Update();
        }

    }
}