using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;

namespace SlugpupStuff
{
    public class SlugpupStuffRemix : OptionInterface
    {
        private const float TITLEX = 20f;
        private const float TITLEY = 540f;
        private const float CHECKBOXX = 60f;
        private const float CHECKBOXY = 500f;
        private const float CHECKBOXYOFFSET = 37.5f;
        private const float SLIDERX = 65f;
        private const float SLIDERY = 460f;
        private const float SLIDERYOFFSET = 50f;
        public SlugpupStuffRemix(SlugpupStuff slugpupStuff)
        {
            CosmeticMode = config.Bind("SlugpupStuff_CosmeticMode", false);
            SaintTundraGrapple = config.Bind("SlugpupStuff_SaintTundraGrapple", true);
            BackTundraGrapple = config.Bind("SlugpupStuff_BackTundraGrapple", true);
            OneGrapple = config.Bind("SlugpupStuff_OneGrapple", true);
            TundraViolence = config.Bind("SlugpupStuff_TundraViolence", false);
            ManualItemGen = config.Bind("SlugpupStuff_ManualItemGem", false);
            RotundBackExaustion = config.Bind("SlugpupStuff_RotundBackExhaust", true);
            aquaticChance = config.Bind("SlugpupStuff_aquaticChance", 70);
            tundraChance = config.Bind("SlugpupStuff_tundraChance", 52);
            hunterChance = config.Bind("SlugpupStuff_hunterChance", 41);
            boomChance = config.Bind("SlugpupStuff_boomChance", 24);
            rotundChance = config.Bind("SlugpupStuff_rotundChance", 16);
        }

        public readonly Configurable<bool> CosmeticMode;
        public readonly Configurable<bool> SaintTundraGrapple;
        public readonly Configurable<bool> BackTundraGrapple;
        public readonly Configurable<bool> OneGrapple;
        public readonly Configurable<bool> TundraViolence;
        public readonly Configurable<bool> ManualItemGen;
        public readonly Configurable<bool> RotundBackExaustion;
        public readonly Configurable<int> aquaticChance;
        public readonly Configurable<int> tundraChance;
        public readonly Configurable<int> hunterChance;
        public readonly Configurable<int> boomChance;
        public readonly Configurable<int> rotundChance;

        private OpCheckBox OPCosmeticMode;
        private OpCheckBox OPsaintTundraGrapple;
        private OpCheckBox OPbackTundraGrapple;
        private OpCheckBox OPoneGrapple;
        private OpCheckBox OPtundraViolence;
        private OpCheckBox OPmanualItemGen;
        private OpCheckBox OProtundBackExhaust;


        private OpSlider OPaquaticSlider;
        private OpSlider OPtundraSlider;
        private OpSlider OPhunterSlider;
        private OpSlider OPboomSlider;
        private OpSlider OProtundSlider;
        private OpSimpleButton resetButton;
        private OpSimpleButton zeroButton;
        private OpLabel OPaquaticMax;
        private OpLabel OPtundraMax;
        private OpLabel OPhunterMax;
        private OpLabel OPboomMax;
        private OpLabel OProtundMax;
        private OpLabel OPaquaticChance;
        private OpLabel OPtundraChance;
        private OpLabel OProtundChance;
        private OpLabel OPhunterChance;
        private OpLabel OPboomChance;
        private OpLabel OPregularChance;


        public override void Initialize()
        {
            base.Initialize();

            var toggleConfig = new OpTab(this, "Options");
            var variantConfig = new OpTab(this, "Variant Chances");
            Tabs = [toggleConfig, variantConfig];

            // CHECKBOXES
            OPCosmeticMode = new OpCheckBox(CosmeticMode, new(CHECKBOXX, CHECKBOXY));
            OPsaintTundraGrapple = new OpCheckBox(SaintTundraGrapple, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET));
            OPbackTundraGrapple = new OpCheckBox(BackTundraGrapple, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET * 2));
            OPoneGrapple = new OpCheckBox(OneGrapple, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET * 3f));
            OPtundraViolence = new OpCheckBox(TundraViolence, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET * 4f));
            OPmanualItemGen = new OpCheckBox(ManualItemGen, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET * 5f));
            OProtundBackExhaust = new OpCheckBox(RotundBackExaustion, new(CHECKBOXX, CHECKBOXY - CHECKBOXYOFFSET * 6f));

            toggleConfig.AddItems(
                new OpLabel(TITLEX, TITLEY, "Toggles", true),
                OPCosmeticMode,
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY, "Toggle Cosmetic Mode, Pups won't have differing food needs or abilities,\n just their appearance will change"),
                OPsaintTundraGrapple,
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET, "Toggle using Tundrapup grapple as Saint"),
                OPbackTundraGrapple,
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET * 2f, "Toggle using Tundrapup grapple while pup is on back"),
                OPoneGrapple,
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET * 3f, "Toggle using only one Tundrapup grapple if multiple held/on back"),
                OPtundraViolence,
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET * 4f, "Toggle Tundrapups being able to throw spears at reduced damage"),
                OPmanualItemGen,
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET * 5f, "Toggle regurgitating random items by using UP + GRAB on Rotundpups"),
                OProtundBackExhaust,
                new OpLabel(CHECKBOXX + 40f, CHECKBOXY - CHECKBOXYOFFSET * 6f, "Toggle exhaustion while having a Rotundpup on your back")
                );

            // SLIDERS
            OPaquaticSlider = new(aquaticChance, new(SLIDERX, SLIDERY), 100) { size = new(300f, 30f) };
            OPtundraSlider = new(tundraChance, new(SLIDERX, SLIDERY - SLIDERYOFFSET), 100) { size = new(300f, 30f) };
            OPhunterSlider = new(hunterChance, new(SLIDERX, SLIDERY - SLIDERYOFFSET * 2f), 100) { size = new(300f, 30f) };
            OPboomSlider = new(boomChance, new(SLIDERX, SLIDERY - SLIDERYOFFSET * 3f), 100) { size = new(300f, 30f) };
            OProtundSlider = new(rotundChance, new(SLIDERX, SLIDERY - SLIDERYOFFSET * 4f), 100) { size = new(300f, 30f) };
            resetButton = new(new(SLIDERX + 275f, SLIDERY + 55f), new(45f, 10f), "reset") { description = "Reset all variant chances to default" };
            zeroButton = new(new(SLIDERX + 220f, SLIDERY + 55f), new(45f, 10f), "tare") { description = "Set all variant chances to zero" };

            OPaquaticMax = new(SLIDERX + 315f, SLIDERY + 6f, "");
            OPtundraMax = new(SLIDERX + 315f, SLIDERY - SLIDERYOFFSET + 6f, "");
            OPhunterMax = new(SLIDERX + 315f, SLIDERY - (SLIDERYOFFSET * 2f) + 6f, "");
            OPboomMax = new(SLIDERX + 315f, SLIDERY - (SLIDERYOFFSET * 3f) + 6f, "");
            OProtundMax = new(SLIDERX + 315f, SLIDERY - (SLIDERYOFFSET * 4f) + 6f, "");
            OPaquaticChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 4.5f), "");
            OPtundraChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 4.5f) - 20f, "");
            OPhunterChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 4.5f) - 40f, "");
            OPboomChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 4.5f) - 60f, "");
            OProtundChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 4.5f) - 80f, "");
            OPregularChance = new(SLIDERX, SLIDERY - (SLIDERYOFFSET * 4.5f) - 100f, "");

            variantConfig.AddItems(
                new OpLabel(TITLEX, TITLEY, "Variant Chances", true),
                new OpLabel(SLIDERX, SLIDERY + 30f, "Aquaticpup Chance"),
                new OpLabel(SLIDERX, SLIDERY - SLIDERYOFFSET + 30f, "Tundrapup Chance"),
                new OpLabel(SLIDERX, SLIDERY - (SLIDERYOFFSET * 2f) + 30f, "Hunterpup Chance"),
                new OpLabel(SLIDERX, SLIDERY - (SLIDERYOFFSET * 3f) + 30f, "Boompup Chance"),
                new OpLabel(SLIDERX, SLIDERY - (SLIDERYOFFSET * 4f) + 30f, "Rotundpup Chance"),
                OPaquaticSlider,
                OPtundraSlider,
                OPhunterSlider,
                OPboomSlider,
                OProtundSlider,
                resetButton,
                zeroButton,
                OPaquaticMax,
                OPtundraMax,
                OPhunterMax,
                OPboomMax,
                OProtundMax,
                OPaquaticChance,
                OPtundraChance,
                OPhunterChance,
                OPboomChance,
                OProtundChance,
                OPregularChance
                );
        }
        public override void Update()
        {
            if (!bool.Parse(OPbackTundraGrapple.value))
            {
                OPoneGrapple.greyedOut = true;
                OPoneGrapple.SetValueBool(false);
            }
            else
            {
                OPoneGrapple.greyedOut = false;
            }
            int aquaticValue = OPaquaticSlider.GetValueInt();
            int tundraValue = OPtundraSlider.GetValueInt();
            int hunterValue = OPhunterSlider.GetValueInt();
            int boomValue = OPboomSlider.GetValueInt();
            int rotundValue = OProtundSlider.GetValueInt();

            if (boomValue - rotundValue < 0)
            {
                OPboomSlider.ForceValue(OProtundSlider.value);
                OPboomSlider.ClampValue(rotundValue);
                boomValue = OProtundSlider.GetValueInt();
            }
            if (hunterValue - boomValue < 0) 
            { 
                OPhunterSlider.ForceValue(OPboomSlider.value);
                OPhunterSlider.ClampValue(boomValue);
                hunterValue = OPboomSlider.GetValueInt();
            }
            if (tundraValue - hunterValue < 0)
            {
                OPtundraSlider.ForceValue(OPhunterSlider.value);
                OPtundraSlider.ClampValue(hunterValue);
                tundraValue = OPhunterSlider.GetValueInt();
            }
            if (aquaticValue - tundraValue < 0)
            {
                OPaquaticSlider.ForceValue(OPtundraSlider.value);
                OPaquaticSlider.ClampValue(tundraValue);
                aquaticValue = OPtundraSlider.GetValueInt();
            }

            int boomMax = Mathf.Clamp(100 - rotundValue, 0, 100);
            int hunterMax = Mathf.Clamp(100 - boomValue, 0, 100);
            int tundraMax = Mathf.Clamp(100 - hunterValue, 0, 100);
            int aquaticMax = Mathf.Clamp(100 - tundraValue, 0, 100);

            OProtundSlider._label.text = rotundValue + "%";
            OPboomSlider._label.text = Mathf.Clamp(boomValue - rotundValue, 0, boomMax) + "%";
            OPhunterSlider._label.text = Mathf.Clamp(hunterValue - boomValue, 0, hunterMax) + "%";
            OPtundraSlider._label.text = Mathf.Clamp(tundraValue - hunterValue, 0, tundraMax) + "%";
            OPaquaticSlider._label.text = Mathf.Clamp(aquaticValue - tundraValue, 0, aquaticMax) + "%";

            OProtundMax.text = "max 100%";
            OPboomMax.text = "max " + boomMax.ToString() + "%";
            OPhunterMax.text = "max " + hunterMax.ToString() + "%";
            OPtundraMax.text = "max " + tundraMax.ToString() + "%";
            OPaquaticMax.text = "max " + aquaticMax.ToString() + "%";

            OPaquaticChance.text = aquaticValue - tundraValue + "% chance to spawn as Aquaticpup";
            OPtundraChance.text = tundraValue - hunterValue + "% chance to spawn as Tundrapup";
            OPhunterChance.text = hunterValue - boomValue + "% chance to spawn as Hunterpup";
            OPboomChance.text = boomValue - rotundValue + "% chance to spawn as Boompup";
            OProtundChance.text = rotundValue + "% chance to spawn as Rotundpup";
            OPregularChance.text = 100 - aquaticValue + "% chance to spawn as a regular pup";


            resetButton.OnClick += delegate
            {
                aquaticChance.Value = 70;
                OPaquaticSlider.ForceValue(aquaticChance.defaultValue);
                tundraChance.Value = 52;
                OPtundraSlider.ForceValue(tundraChance.defaultValue);
                hunterChance.Value = 41;
                OPhunterSlider.ForceValue(hunterChance.defaultValue);
                boomChance.Value = 24;
                OPboomSlider.ForceValue(boomChance.defaultValue);
                rotundChance.Value = 16;
                OProtundSlider.ForceValue(rotundChance.defaultValue);
            };

            zeroButton.OnClick += delegate
            {
                aquaticChance.Value = 0;
                OPaquaticSlider.SetValueInt(0);
                tundraChance.Value = 0;
                OPtundraSlider.SetValueInt(0);
                hunterChance.Value = 0;
                OPhunterSlider.SetValueInt(0);
                boomChance.Value = 0;
                OPboomSlider.SetValueInt(0);
                rotundChance.Value = 0;
                OProtundSlider.SetValueInt(0);
            };
            base.Update();
        }

    }
}