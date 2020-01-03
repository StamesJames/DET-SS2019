using System.Collections;
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

    public override void UpdateChunkCountSize()
    {
        Debug.Log("Updatet Chunk Size: X " + xChunkCountInput.text + " Y " + yChunkCountInput.text + " Z " + zChunkCountInput.text);
        generator.XChunkCount = int.Parse(xChunkCountInput.text);
        generator.YChunkCount = int.Parse(yChunkCountInput.text);
        generator.ZChunkCount = int.Parse(zChunkCountInput.text);
        Debug.Log("new Chunk Size: X " + generator.XChunkCount + " Y " + generator.YChunkCount + " Z " + generator.ZChunkCount);

    }
}
