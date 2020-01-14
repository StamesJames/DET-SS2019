using System;
using UnityEngine;

[System.Serializable]
public class FlatCaveMap : MapGenerator
{

    private Block.BlockType[,] flatMap;

    [SerializeField] private float randomFillPercent = 40;
    public float RandomFillPercent { get => randomFillPercent; set => randomFillPercent = value; }
    [SerializeField] private int smoothingItterations = 3;
    public int SmoothingItterations { get => smoothingItterations; set => smoothingItterations = value; }
    [SerializeField] private Intervall[] birthIntervalls;
    [SerializeField] private Intervall[] deathIntervalls;
    [SerializeField] private int woodChance = 10;
    [SerializeField] private int celingNeighbors = 4;
    [SerializeField] private int bottomNeighbors = 4;
    [SerializeField] private float roughness = 1;
    [SerializeField] private int roughnessIterations = 5;

    [SerializeField] private float diamondSpawnRate = 1;
    public float DiamondSpawnRate { get => diamondSpawnRate; set => diamondSpawnRate = value; }
    [SerializeField] private int diamondOreDeepness = 5;
    public int DiamondOreDeepness { get => diamondOreDeepness; set => diamondOreDeepness = value; }
    [SerializeField] private float diamondOreExpansionRate = 5;
    public float DiamondOreExpansionRate { get => diamondOreExpansionRate; set => diamondOreExpansionRate = value; }

    public FlatCaveMap()
    {
        Debug.Log(diamondSpawnRate);
        XChunkCount = 3;
        YChunkCount = 3;
        ZChunkCount = 3;
        birthIntervalls = new Intervall[] { new Intervall(5, 8) };
        deathIntervalls = new Intervall[] { new Intervall(0, 3) };
        GenerateMap();
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        flatMap = new Block.BlockType[XChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
        currentBlockMap = new Block.BlockType[XChunkCount * World.chunkSize, YChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
        RandomFillMap();

        for (int i = 0; i < smoothingItterations; i++) SmoothMap();

        PutWood();

        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int y = 0; y < YChunkCount * World.chunkSize; y++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = flatMap[x, z];
                }

        CreaterCeling();
        CreateBottom();

        SmoothBottom();

        for (int i = 0; i < roughnessIterations; i++) RoughenitUp();

        SpawnRescources(Block.BlockType.DIAMOND, DiamondSpawnRate, DiamondOreDeepness, DiamondOreExpansionRate);

        return currentBlockMap;
    }

    private void RoughenitUp()
    {
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        System.Random pseudoRandom = new System.Random();
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int y = 0; y < YChunkCount * World.chunkSize; y++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    newMap[x, y, z] = currentBlockMap[x, y, z];

                    if (currentBlockMap[x,y,z] == Block.BlockType.STONE && 
                        pseudoRandom.Next(1,100) <= roughness &&
                        AutomatonUtilities.HasSurroundingBlocksDirect(x,y,z,xChunkCount,yChunkCount,zChunkCount,currentBlockMap,Block.BlockType.AIR))
                    {
                        newMap[x, y, z] = Block.BlockType.AIR;
                    }
                }
        currentBlockMap = newMap;
    }

    private void SpawnRescources(Block.BlockType whatToSpawn, float spawnRate, int spawnDeepnes, float spawnSpreadRate)
    {
        currentBlockMap = AutomatonUtilities.SpawnBlocksRandomly(whatToSpawn, spawnRate, XChunkCount, YChunkCount, ZChunkCount, CurrentBlockMap,
            (int x, int y, int z) => currentBlockMap[x, y, z] == Block.BlockType.STONE &&
                AutomatonUtilities.CountSurroundingBlocksDirect(x, y, z, XChunkCount, YChunkCount, ZChunkCount, currentBlockMap, Block.BlockType.AIR, countEdge: false) > 0);

        for (int i = 0; i < spawnDeepnes; i++) currentBlockMap = AutomatonUtilities.SpawnBlocksRandomly(whatToSpawn, spawnSpreadRate, XChunkCount, YChunkCount, ZChunkCount, CurrentBlockMap,
            (int x, int y, int z) => currentBlockMap[x,y,z] == Block.BlockType.STONE &&
                AutomatonUtilities.CountSurroundingBlocksDirect(x,y,z, XChunkCount, YChunkCount, ZChunkCount, CurrentBlockMap, whatToSpawn, countEdge: false) > 0);
    }

    void PutWood()
    {
        System.Random pseudoRandom = new System.Random();
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
            {
                if (flatMap[x,z] != Block.BlockType.STONE && HasNeighbor(x,z,Block.BlockType.STONE,1) && pseudoRandom.Next(1,100) < woodChance)
                {
                    flatMap[x,z] = Block.BlockType.WOOD;
                }
            }
    }

    bool HasNeighbor(int x, int z, Block.BlockType block, int distance)
    {
        for (int xi = x - distance; xi <= x + distance; xi++)
            for (int zj = z - distance; zj <= z + distance; zj++)
            {
                if (xi >= 0 && xi < XChunkCount * World.chunkSize && zj >= 0 && zj < ZChunkCount * World.chunkSize)
                {
                    if ( (x != xi || z != zj ) && flatMap[xi,zj] == block)
                    {
                        return true;
                    }
                }
            }
        return false;
    }

    void RandomFillMap()
    {
        System.Random pseudoRandom = new System.Random();       
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
            {
                if (x == 0 || x == XChunkCount * World.chunkSize - 1 || z == 0 || z == ZChunkCount * World.chunkSize - 1)
                {
                    flatMap[x, z] = Block.BlockType.STONE;
                }
                else
                {
                    flatMap[x, z] = (pseudoRandom.Next(1,100) < randomFillPercent) ? Block.BlockType.STONE : Block.BlockType.AIR;
                }
            }       
    }

    void SmoothMap() {
        Block.BlockType[,] newFlatMap = new Block.BlockType[XChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
		for (int x = 0; x < XChunkCount * World.chunkSize; x ++)
			for (int z = 0; z < ZChunkCount * World.chunkSize; z ++)
            {
				int neighboreCount = AutomatonUtilities.CountSurroundingBlocks(x,z,XChunkCount, ZChunkCount, flatMap, Block.BlockType.AIR, 1,true);
                newFlatMap[x, z] = flatMap[x, z];
                if (flatMap[x,z] == Block.BlockType.STONE)
                {
                    foreach (Intervall deathIntervall in deathIntervalls)
                    {
                        if (deathIntervall.Contains(neighboreCount))
                        {
                            newFlatMap[x, z] = Block.BlockType.AIR;
                        }
                    }
                }
                else
                {
                    foreach (Intervall birthIntervall in birthIntervalls)
                    {
                        if (birthIntervall.Contains(neighboreCount))
                        {
                            newFlatMap[x, z] = Block.BlockType.STONE;
                        }
                    }
                }
			}
        flatMap = newFlatMap;
	}

    void CreaterCeling(){
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, YChunkCount * World.chunkSize - 1, z] = Block.BlockType.STONE;
                }
        Block.BlockType[,] neueEbene = new Block.BlockType[XChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
        for (int y = YChunkCount * World.chunkSize - 2; y > 0; y--)
        {
            for (int x = 0; x < XChunkCount * World.chunkSize; x++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    if (currentBlockMap[x,y,z] != Block.BlockType.AIR)
                    {
                        neueEbene[x, z] = currentBlockMap[x, y, z];
                    }
                    else
                    {
                        neueEbene[x, z] = AutomatonUtilities.CountSurroundingBlocks(x, y, z, XChunkCount,YChunkCount,ZChunkCount, currentBlockMap, Block.BlockType.AIR, 1, true) >= 14 ? 
                            Block.BlockType.STONE : Block.BlockType.AIR;
                    }
                }
            for (int x = 0; x < XChunkCount * World.chunkSize; x++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = neueEbene[x, z];
                }
        }
    }



    void CreateBottom(){
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, 0, z] = Block.BlockType.STONE;
                }
    }

    void SmoothBottom(){
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
            {
                currentBlockMap[x, YChunkCount * World.chunkSize - 1, z] = Block.BlockType.STONE;
            }
        Block.BlockType[,] neueEbene = new Block.BlockType[XChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
        for (int y = 0; y < YChunkCount; y++)
        {
            for (int x = 0; x < XChunkCount * World.chunkSize; x++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    if (currentBlockMap[x, y, z] != Block.BlockType.AIR)
                    {
                        neueEbene[x, z] = currentBlockMap[x, y, z];
                    }
                    else
                    {
                        neueEbene[x, z] = AutomatonUtilities.CountSurroundingBlocks(x, y, z, XChunkCount, YChunkCount, ZChunkCount, currentBlockMap, Block.BlockType.AIR, 1, true) >= 14 ? 
                            Block.BlockType.STONE : Block.BlockType.AIR;
                    }
                }
            for (int x = 0; x < XChunkCount * World.chunkSize; x++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = neueEbene[x, z];
                }
        }
    }

}

[System.Serializable]
public class Intervall
{
    public float von;
    public float bis;

    public Intervall(float pVon, float pBis)
    {
        von = pVon;
        bis = pBis;
    }

    public bool Contains(float x)
    {
        return (x >= von && x <= bis);
    }
}