using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneratorMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown generatorTMP;
    [SerializeField] private MapGeneratorHolder[] holders;
    [SerializeField] private World world;

    private MapGeneratorHolder currenHolder;


    private void Start()
    {
        generatorTMP.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < holders.Length; i++)
        {
            options.Add(holders[i].Holdername);
        }
        generatorTMP.AddOptions(options);
        world.MapGenerator = holders[0].GetGenerator();
        currenHolder = holders[0];
    }

    public void ChangeWorldGenerator(TMP_Dropdown change)
    {
        world.MapGenerator = holders[change.value].GetGenerator();
        currenHolder = holders[change.value];
        Debug.Log("Changed to " + holders[change.value].Holdername);
    }
}
