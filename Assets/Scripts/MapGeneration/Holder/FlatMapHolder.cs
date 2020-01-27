using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class FlatMapHolder : MapGeneratorHolder
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
    // Bottom Celing
    [SerializeField] private TMP_Dropdown celingNeighborTypeDropdown;
    [SerializeField] private TMP_InputField celingCurveInput;
    [SerializeField] private TMP_Dropdown groundNeighborTypeDropdown;
    [SerializeField] private TMP_InputField groundCurveInput;



    private Ruleset[] rules = new Ruleset[]{
        new Ruleset(new Intervall[] { new Intervall(5, 8) }, new Intervall[] { new Intervall(0, 3) }, "Rounded Cave"),
        new Ruleset(new Intervall[] { new Intervall(5, 8) }, new Intervall[] { new Intervall(0, 4) }, "Straight Cave")};

    private FlatCaveMap generator = new FlatCaveMap();

    private void Start()
    {
        ruleDropDown.ClearOptions();
        List<string> ruleNames = new List<string>(rules.Select((rule) => rule.ToString()));
        ruleDropDown.AddOptions(ruleNames);

        celingNeighborTypeDropdown.ClearOptions();
        List<string> celingNeighborTypes = new List<string>(System.Enum.GetNames(typeof(NeighborType)));
        celingNeighborTypeDropdown.AddOptions(celingNeighborTypes);

        groundNeighborTypeDropdown.ClearOptions();
        List<String> groundNeighborTypes = Enum.GetNames(typeof(NeighborType)).ToList<string>();
        groundNeighborTypeDropdown.AddOptions(groundNeighborTypes);

        // Random
        useRandomSeedToggle.onValueChanged.AddListener((isOn) => generator.UseRandomSeed = isOn);

        // ground Celing
        celingNeighborTypeDropdown.onValueChanged.AddListener((index) => generator.CelingNeighbortype = (NeighborType) index);
        celingCurveInput.onValueChanged.AddListener((input) => generator.CelingCurve = AutomatonUtilities.CurveParser(input));
        groundNeighborTypeDropdown.onValueChanged.AddListener((index) => generator.GroundNeighbortype = (NeighborType)index);
        groundCurveInput.onValueChanged.AddListener((input) => generator.GroundCurve = AutomatonUtilities.CurveParser(input));


        // Wood
        woodEbenenToggle.onValueChanged.AddListener( (isOn) => generator.SpawnWoodEbenen = isOn);

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
        generator.RandomFillPercent = (int) randomFillPercentSlider.value;
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
