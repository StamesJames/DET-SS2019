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
    [SerializeField] private int diamondSpawnRate = 5;
    public int DiamondPercent { get => diamondSpawnRate; set => diamondSpawnRate = value; }
    [SerializeField] private int diamondOreDeepness = 5;
    public int DiamondOreDeepness { get => diamondOreDeepness; set => diamondOreDeepness = value; }
    [SerializeField] private int diamondOreExpansionRate = 5;
    public int DiamondOreExpansionRate { get => diamondOreExpansionRate; set => diamondOreExpansionRate = value; }
    [SerializeField] private Intervall[] birthIntervalls;
    [SerializeField] private Intervall[] deathIntervalls;

    public ThreeDCave()
    {
        xChunkCount = 3;
        yChunkCount = 3;
        zChunkCount = 3;
        birthIntervalls = new Intervall[] { new Intervall(13, 14), new Intervall(17, 19) };
        deathIntervalls = new Intervall[] { new Intervall(0, 12),  new Intervall(27, 30) };
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        currentBlockMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];

        FillRandom();

        for (int i = 0; i < smoothingItterations; i++) Smooth();

        return currentBlockMap;
    }

    private void SpawnRescourcess(Block.BlockType rescource, int spawnRate)
    {
        
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
                    int neighborCount = AutomatonUtilities.CountSurroundingBlocks(x, y, z, xChunkCount, yChunkCount, zChunkCount,  currentBlockMap, Block.BlockType.AIR, 1, true);
                    newBlockMap[x, y, z] = currentBlockMap[x, y, z];
                    if (currentBlockMap[x,y, z] == Block.BlockType.STONE)
                    {
                        foreach (Intervall deathIntervall in deathIntervalls)
                        {
                            if (deathIntervall.Contains(neighborCount))
                            {
                                newBlockMap[x, y, z] = Block.BlockType.AIR;
                            }
                        }
                    }
                    else
                    {
                        foreach (Intervall birthIntervall in birthIntervalls)
                        {
                            if (birthIntervall.Contains(neighborCount))
                            {
                                newBlockMap[x, y, z] = Block.BlockType.STONE;
                            }
                        }
                    }
                }               
        currentBlockMap = newBlockMap;
    }



}
