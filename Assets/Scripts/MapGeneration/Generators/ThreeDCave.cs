using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDCave : MapGenerator
{
    // Randomisation
    private System.Random pseudoRandom = new System.Random();
    [SerializeField] private bool useRandomSeed = true;
    public bool UseRandomSeed { get => useRandomSeed; set => useRandomSeed = value; }
    [SerializeField] private string seed = "NOTRANDOM";
    public string Seed { get => seed; set => seed = value; }
    // Rules
    [SerializeField] private Ruleset currentRuleset;
    public Ruleset CurrentRuleset { get => currentRuleset; set => currentRuleset = value; }
    // Smoothing
    [SerializeField] private float randomFillPercent = 40;
    public float RandomFillPercent { get => randomFillPercent; set => randomFillPercent = value; }
    [SerializeField] private int smoothingItterations = 5;
    public int SmoothingItterations { get => smoothingItterations; set => smoothingItterations = value; }
    // Roughness
    [SerializeField] private float roughness = 1;
    public float Roughness { get => roughness; set => roughness = value; }
    [SerializeField] private int roughnessIterations = 5;
    public int RoughnessIterations { get => roughnessIterations; set => roughnessIterations = value; }
    // Diamonds
    [SerializeField] private float diamondSpawnRate = 1;
    public float DiamondSpawnRate { get => diamondSpawnRate; set => diamondSpawnRate = value; }
    [SerializeField] private int diamondOreDeepness = 5;
    public int DiamondOreDeepness { get => diamondOreDeepness; set => diamondOreDeepness = value; }
    [SerializeField] private float diamondOreExpansionRate = 5;
    public float DiamondOreExpansionRate { get => diamondOreExpansionRate; set => diamondOreExpansionRate = value; }
    // Wood
    [SerializeField] private float woodChance = 10;
    public float WoodChance { get => woodChance; set => woodChance = value; }
    [SerializeField] private int woodStrebeLänge = 10;
    public int WoodStrebeLänge { get => woodStrebeLänge; set => woodStrebeLänge = value; }
    [SerializeField] private bool spawnWoodEbenen = false;
    public bool SpawnWoodEbenen { get => spawnWoodEbenen; set => spawnWoodEbenen = value; }


    public ThreeDCave()
    {
        xChunkCount = 3;
        yChunkCount = 3;
        zChunkCount = 3;
        currentRuleset = new Ruleset(
            pBirthrules: new Intervall[] { new Intervall(13, 14), new Intervall(17, 19) },
            pDeathRules: new Intervall[] { new Intervall(0, 12), new Intervall(27, 30) }
            );
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        currentBlockMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];

        FillRandom();

        for (int i = 0; i < smoothingItterations; i++) Smooth();

        SpawnRescources(Block.BlockType.DIAMOND, diamondSpawnRate, DiamondOreDeepness, DiamondOreExpansionRate);

        for (int i = 0; i < RoughnessIterations; i++) RoughenItUp();

        //SpawnPillars(); 

        return currentBlockMap;
    }

    public void RoughenItUp()
    {
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        for (int x = 0; x < (XChunkCount * World.chunkSize); x++)
            for (int y = 0; y < (YChunkCount * World.chunkSize); y++)
                for (int z = 0; z < (ZChunkCount * World.chunkSize); z++)
                {
                    newMap[x, y, z] = currentBlockMap[x, y, z];

                    if (x == 0 || y == 0 || z == 0 || x == XChunkCount * World.chunkSize - 1 || y == YChunkCount * World.chunkSize - 1 || z == ZChunkCount * World.chunkSize - 1)
                        continue;

                    if (currentBlockMap[x, y, z] == Block.BlockType.STONE &&
                        pseudoRandom.Next(1, 100) <= roughness &&
                        AutomatonUtilities.HasSurroundingBlockDirect(x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap, Block.BlockType.AIR, countEdge: false))
                    {
                        newMap[x, y, z] = Block.BlockType.AIR;
                    }
                }
        currentBlockMap = newMap;
    }


    private void SpawnPillars() // funktioniert noch nciht wirklich gut
    {
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < YChunkCount * World.chunkSize; y++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    newMap[x, y, z] = currentBlockMap[x, y, z];
                    int count = AutomatonUtilities.CountSurroundingBlocksDirect(x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap, Block.BlockType.AIR, 5,
                        countEdge: false);
                    if (count >= 30){
                        newMap[x, y, z] = Block.BlockType.WOOD;
                    }
                }
        currentBlockMap = newMap;
    }

    private void SpawnRescources(Block.BlockType whatToSpawn, float spawnRate, int spawnDeepnes, float spawnSpreadRate)
    {
        currentBlockMap = AutomatonUtilities.SpawnBlocksRandomly(whatToSpawn, spawnRate, XChunkCount, YChunkCount, ZChunkCount, CurrentBlockMap,
            (int x, int y, int z) => currentBlockMap[x, y, z] == Block.BlockType.STONE &&
                AutomatonUtilities.CountSurroundingBlocksDirect(x, y, z, XChunkCount, YChunkCount, ZChunkCount, 
                currentBlockMap, Block.BlockType.AIR, countEdge: false) > 0, pseudoRandom);

        for (int i = 0; i < spawnDeepnes; i++) currentBlockMap = AutomatonUtilities.SpawnBlocksRandomly(whatToSpawn, spawnSpreadRate, XChunkCount, YChunkCount, ZChunkCount, CurrentBlockMap,
            (int x, int y, int z) => currentBlockMap[x, y, z] == Block.BlockType.STONE &&
                AutomatonUtilities.CountSurroundingBlocksDirect(x, y, z, XChunkCount, YChunkCount, ZChunkCount, 
                CurrentBlockMap, whatToSpawn, countEdge: false) > 0, pseudoRandom);
    }

    private void FillRandom()
    {
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
                        if (currentRuleset.checkDeath(neighborCount))
                        {
                            newBlockMap[x, y, z] = Block.BlockType.AIR;
                        }
                    }
                    else
                    {
                        if (currentRuleset.checkBirth(neighborCount))
                        {
                            newBlockMap[x, y, z] = Block.BlockType.STONE;
                        }
                    }
                }               
        currentBlockMap = newBlockMap;
    }



}
