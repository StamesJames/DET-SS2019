﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FlatMapHolder : MapGeneratorHolder
{
    [SerializeField] private TMP_InputField xChunkCountInput;
    [SerializeField] private TMP_InputField yChunkCountInput;
    [SerializeField] private TMP_InputField zChunkCountInput;
    [SerializeField] private Slider randomFillPercentSlider;
    [SerializeField] private TextMeshProUGUI randomFillPercentAnzeige;
    [SerializeField] private TMP_InputField itterationCountInput;
    [SerializeField] private TMP_Dropdown ruleDropDown;
    [SerializeField] private Slider diamondSpawnRateSlider;
    [SerializeField] private TextMeshProUGUI diamondSpawnRateAnzeige;
    [SerializeField] private Slider diamondExtentionRateSlider;
    [SerializeField] private TextMeshProUGUI diamondExtentionRateAnzeige;
    [SerializeField] private TMP_InputField diamondSpawnIterationsInput;
    [SerializeField] private Slider roughnessRateSlider;
    [SerializeField] private TextMeshProUGUI roughnessRateAnzeige;
    [SerializeField] private TMP_InputField roughnessIterationsInput;
    private Ruleset[] rules = new Ruleset[]{
        new Ruleset(new Intervall[] { new Intervall(5, 8) }, new Intervall[] { new Intervall(0, 3) }),
        new Ruleset(new Intervall[] { new Intervall(5, 8) }, new Intervall[] { new Intervall(0, 4) })};

    private FlatCaveMap generator = new FlatCaveMap();

    private void Start()
    {
        ruleDropDown.ClearOptions();
        List<string> ruleNames = new List<string>();
        for (int i = 0; i < rules.Length; i++)
        {
            ruleNames.Add(rules[i].ToString());
        }
        ruleDropDown.AddOptions(ruleNames);
        UpdateValues();
    }

    public override MapGenerator GetGenerator()
    {
        return generator;
    }

    public void UpdateChunkCountSize()
    {
        generator.XChunkCount = int.Parse(xChunkCountInput.text);
        generator.YChunkCount = int.Parse(yChunkCountInput.text);
        generator.ZChunkCount = int.Parse(zChunkCountInput.text);
    }

    public override void UpdateValues()
    {
        UpdateChunkCountSize();
        generator.RandomFillPercent = (int) randomFillPercentSlider.value;
        generator.SmoothingItterations = int.Parse(itterationCountInput.text);
        generator.CurrentRuleset = rules[ruleDropDown.value];
        generator.DiamondSpawnRate = (diamondSpawnRateSlider.value);
        generator.DiamondOreExpansionRate = diamondExtentionRateSlider.value;
        generator.DiamondOreDeepness = int.Parse(diamondSpawnIterationsInput.text);
        generator.Roughness = roughnessRateSlider.value;
        generator.RoughnessIterations = int.Parse(roughnessIterationsInput.text);
        randomFillPercentAnzeige.text = randomFillPercentSlider.value.ToString() + "%";
        diamondSpawnRateAnzeige.text = diamondSpawnRateSlider.value.ToString() + "%";
        diamondExtentionRateAnzeige.text = diamondExtentionRateSlider.value.ToString() + "%";
        roughnessRateAnzeige.text = roughnessRateSlider.value.ToString() + "%";
    }

}
