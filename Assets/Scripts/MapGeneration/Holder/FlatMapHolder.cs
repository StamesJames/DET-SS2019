using System.Collections;
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

    private FlatCaveMap generator = new FlatCaveMap();

    private void Start()
    {
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
    }

}
