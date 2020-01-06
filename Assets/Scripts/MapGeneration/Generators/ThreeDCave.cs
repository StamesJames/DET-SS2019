using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDCave : MapGenerator
{
    [SerializeField] private int randomFillPercent = 40;
    public int RandomFillPercent { get => randomFillPercent; set => randomFillPercent = value; }
    [SerializeField] private int smoothingItterations = 3;
    public int SmoothingItterations { get => smoothingItterations; set => smoothingItterations = value; }
    [SerializeField] private Intervall[] birthIntervalls;
    [SerializeField] private Intervall[] deathIntervalls;

    public override Block.BlockType[,,] GenerateMap()
    {
        currentBlockMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];

        FillRandom();

        return currentBlockMap;
    }

    private void FillRandom()
    {
        System.Random pseudoRandom = new System.Random();
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < YChunkCount * World.chunkSize; y++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    float randomFloat = pseudoRandom.Next(1,100);
                    if (randomFloat < randomFillPercent)
                    {
                        currentBlockMap[x, y, z] = Block.BlockType.STONE;
                    }
                    else
                    {
                        currentBlockMap[x, y, z] = Block.BlockType.AIR;
                    }
                }
    }

    void Smooth()
    {
        Block.BlockType[,,] newBlockMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize ];
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < YChunkCount * World.chunkSize; y++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    newBlockMap[x, y, z] = GetNewState(x, y, z);
                }
            
        
        currentBlockMap = newBlockMap;
    }

}
