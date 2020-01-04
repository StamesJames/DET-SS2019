using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGeneratorHolder : MonoBehaviour
{
    [SerializeField] private string holdername = "unnamed";
    public string Holdername { get => holdername;}

    public abstract MapGenerator GetGenerator();

    public abstract void UpdateValues();
}
