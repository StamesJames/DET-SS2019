﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestMapHolder : MapGeneratorHolder
{
    [SerializeField] private TMP_InputField xChunkCountInput;
    [SerializeField] private TMP_InputField yChunkCountInput;
    [SerializeField] private TMP_InputField zChunkCountInput;

    private TestMap generator = new TestMap();

    private void Start()
    {
        UpdateChunkCountSize();
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
    }
}
