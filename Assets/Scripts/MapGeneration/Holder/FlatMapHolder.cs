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
    [SerializeField] private Slider randomFillPercentInput;
    [SerializeField] private TMP_InputField itterationCountInput;
    [SerializeField] private TextMeshProUGUI percentAnzeige;
    [SerializeField] private TMP_Dropdown ruleDropDown;

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
        generator.RandomFillPercent = (int) randomFillPercentInput.value;
        generator.SmoothingItterations = int.Parse(itterationCountInput.text);
        percentAnzeige.text = randomFillPercentInput.value.ToString() + "%";
        generator.CurrentRuleset = rules[ruleDropDown.value];
    }

}
