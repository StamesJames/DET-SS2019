using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class ThreeDCaveHolder : MapGeneratorHolder
{
    // Rule
    [SerializeField] private TMP_Dropdown ruleDropDown;
    // Größen Einstellungen
    [SerializeField] private TMP_InputField xChunkCountInput;
    [SerializeField] private TMP_InputField yChunkCountInput;
    [SerializeField] private TMP_InputField zChunkCountInput;
    // Randomisation 
    [SerializeField] private Toggle useRandomSeedToggle;
    [SerializeField] private TMP_InputField seedInput;
    // Smoothing
    [SerializeField] private Slider randomFillPercentSlider;
    [SerializeField] private TextMeshProUGUI randomFillPercentAnzeige;
    [SerializeField] private TMP_InputField itterationCountInput;
    // Diamond Spawn
    [SerializeField] private Slider diamondSpawnRateSlider;
    [SerializeField] private TextMeshProUGUI diamondSpawnRateAnzeige;
    [SerializeField] private Slider diamondExtentionRateSlider;
    [SerializeField] private TextMeshProUGUI diamondExtentionRateAnzeige;
    [SerializeField] private TMP_InputField diamondSpawnIterationsInput;
    // Roughness
    [SerializeField] private Slider roughnessRateSlider;
    [SerializeField] private TextMeshProUGUI roughnessRateAnzeige;
    [SerializeField] private TMP_InputField roughnessIterationsInput;
    // Wood
    [SerializeField] private Toggle woodEbenenToggle;
    [SerializeField] private Slider woodSpawnRateSlider;
    [SerializeField] private TextMeshProUGUI woodSpawnRateAnzeige;
    [SerializeField] private TMP_InputField woodStrebenSize;

    private Ruleset[] rules = new Ruleset[]{
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(13, 14), new Intervall(17, 19) },
            pDeathRules: new Intervall[] { new Intervall(0, 12), new Intervall(27, 30) }
            , "Clouds 1"),
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(13, 14) },
            pDeathRules: new Intervall[] { new Intervall(0, 12), new Intervall(27, 30) }
            , "Clouds 2" ),
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(6, 8) },
            pDeathRules: new Intervall[] { new Intervall(0, 5), new Intervall(9, 30) }
            , "678 678" ),
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(5, 7), new Intervall(12, 13), new Intervall(15,15) },
            pDeathRules: new Intervall[] { new Intervall(0, 8), new Intervall(27, 30) }
            , "Amoeba" ),
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(4, 4), new Intervall(6,6), new Intervall(8,9) },
            pDeathRules: new Intervall[] { new Intervall(0, 1), new Intervall(3, 5), new Intervall(10, 30) }
            , "Builder" ),
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(1, 1), new Intervall(3,3) },
            pDeathRules: new Intervall[] { new Intervall(7, 30) }
            , "Crystal Growth 1" ),
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(6, 8) },
            pDeathRules: new Intervall[] { new Intervall(0, 3), new Intervall(8, 30) }
            , "Pyroclastic" ),
        new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(13, 26) },
            pDeathRules: new Intervall[] { new Intervall(0, 0), new Intervall(2, 3), new Intervall(5,7), new Intervall(9,10),new Intervall(12,12), new Intervall(27,30) }
            , "Slow Decay" ),
    };

    private ThreeDCave generator = new ThreeDCave();

    private void Start()
    {
        ruleDropDown.ClearOptions();
        List<string> ruleNames = new List<string>(rules.Select((rule) => rule.ToString()));
        ruleDropDown.AddOptions(ruleNames);

        // Random
        useRandomSeedToggle.onValueChanged.AddListener((isOn) => generator.UseRandomSeed = isOn);

        // Wood
        woodEbenenToggle.onValueChanged.AddListener((isOn) => generator.SpawnWoodEbenen = isOn);

        UpdateValues();
    }

    public override MapGenerator GetGenerator()
    {
        return generator;
    }

    public override void UpdateValues()
    {
        // Update Rules
        generator.CurrentRuleset = rules[ruleDropDown.value];
        // Randomisation
        generator.Seed = seedInput.text;
        // Update ChunkCount
        generator.XChunkCount = int.Parse(xChunkCountInput.text);
        generator.YChunkCount = int.Parse(yChunkCountInput.text);
        generator.ZChunkCount = int.Parse(zChunkCountInput.text);
        // Update Smoothing
        generator.RandomFillPercent = (int)randomFillPercentSlider.value;
        generator.SmoothingItterations = int.Parse(itterationCountInput.text);
        // Update Diamond Spawn
        generator.DiamondSpawnRate = (diamondSpawnRateSlider.value);
        generator.DiamondOreExpansionRate = diamondExtentionRateSlider.value;
        generator.DiamondOreDeepness = int.Parse(diamondSpawnIterationsInput.text);
        // Update Roughness
        generator.Roughness = roughnessRateSlider.value;
        generator.RoughnessIterations = int.Parse(roughnessIterationsInput.text);
        // Update Wood
        generator.WoodChance = woodSpawnRateSlider.value;
        generator.WoodStrebeLänge = int.Parse(woodStrebenSize.text);

        // Anzeige Änderungen
        randomFillPercentAnzeige.text = randomFillPercentSlider.value.ToString() + "%";
        diamondSpawnRateAnzeige.text = diamondSpawnRateSlider.value.ToString() + "%";
        diamondExtentionRateAnzeige.text = diamondExtentionRateSlider.value.ToString() + "%";
        roughnessRateAnzeige.text = roughnessRateSlider.value.ToString() + "%";
        woodSpawnRateAnzeige.text = woodSpawnRateSlider.value.ToString() + "%";
    }

}
