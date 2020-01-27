using System;
using UnityEngine;

[System.Serializable]
public class FlatCaveMap : MapGenerator
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
    // Bottom Celing
    [SerializeField] private NeighborType celingNeighbortype = NeighborType.CIRCLE;
    public NeighborType CelingNeighbortype { get => celingNeighbortype; set => celingNeighbortype = value; }
    [SerializeField] private int[] celingCurve = new int[] { 8,5,3,2,1,1 };
    public int[] CelingCurve { get => celingCurve; set => celingCurve = value; }
    [SerializeField] private NeighborType groundNeighbortype = NeighborType.CIRCLE;
    public NeighborType GroundNeighbortype { get => groundNeighbortype; set => groundNeighbortype = value; }
    [SerializeField] private int[] groundCurve = new int[] { 5,2,1 };
    public int[] GroundCurve { get => groundCurve; set => groundCurve = value; }


    private Block.BlockType[,] flatMap;

    public FlatCaveMap()
    {
        pseudoRandom = new System.Random();
        Debug.Log(diamondSpawnRate);
        XChunkCount = 3;
        YChunkCount = 3;
        ZChunkCount = 3;
        CurrentRuleset = new Ruleset(new Intervall[] { new Intervall(5, 8) }, new Intervall[] { new Intervall(0, 3) });
        GenerateMap();
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        // Random initialisierung
        if (UseRandomSeed) pseudoRandom = new System.Random();
        else pseudoRandom = new System.Random(seed.GetHashCode());  

        // Map initialisierung
        flatMap = new Block.BlockType[XChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
        currentBlockMap = new Block.BlockType[XChunkCount * World.chunkSize, YChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];

        // FlatMap erzeugung
        RandomFillMap();
        for (int i = 0; i < smoothingItterations; i++) SmoothMap();
        PutWood();

        // FlatMap Hochziehen
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int y = 0; y < YChunkCount * World.chunkSize; y++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = flatMap[x, z];
                }

        // Dach und Boden erzeugen
        PutGroundRoof();
        CreateCelingCurve(celingCurve);
        CreateGroundCurve(groundCurve);

        // Wände Aufrauen
        for (int i = 0; i < RoughnessIterations; i++) RoughenitUp();

        // Rescourcen Spawnen 
        SpawnRescources(Block.BlockType.DIAMOND, DiamondSpawnRate, DiamondOreDeepness, DiamondOreExpansionRate);

        // Die Wood Pilars verstreben
        if (spawnWoodEbenen) for (int i = 0; i < woodStrebeLänge; i++) PutWoodStrebenWithEbene();
        else PutWoodStreben();

        // Aufräumen
        RemoveFloatingBlocks();

        return currentBlockMap;
    }

    void PutGroundRoof(){
        for (int x = 0; x < World.chunkSize * xChunkCount; x++)
            for (int z = 0; z < World.chunkSize * zChunkCount; z++)
                {
                    currentBlockMap[x,0,z] = Block.BlockType.STONE;
                    currentBlockMap[x, World.chunkSize * yChunkCount - 1, z] = Block.BlockType.STONE;
                }
    }

    private void RemoveFloatingBlocks()
    {
        for (int x = 0; x < (XChunkCount * World.chunkSize); x++)
            for (int y = 0; y < (YChunkCount * World.chunkSize); y++)
                for (int z = 0; z < (ZChunkCount * World.chunkSize); z++) {
                    if(AutomatonUtilities.CountSurroundingBlocksDirect(x,y,z,xChunkCount,yChunkCount,zChunkCount,currentBlockMap,Block.BlockType.AIR) == 6)
                        currentBlockMap[x,y,z] = Block.BlockType.AIR;
                }   
    }

    private void RoughenitUp()
    {
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        for (int x = 0; x < (XChunkCount * World.chunkSize); x++)
            for (int y = 0; y < (YChunkCount * World.chunkSize); y++)
                for (int z = 0; z < (ZChunkCount * World.chunkSize); z++)
                {
                    newMap[x, y, z] = currentBlockMap[x, y, z];

                    if (x == 0 || y == 0 || z == 0 || x == XChunkCount * World.chunkSize - 1 || y == YChunkCount * World.chunkSize - 1 || z == ZChunkCount * World.chunkSize - 1)
                        continue;

                    if (currentBlockMap[x,y,z] == Block.BlockType.STONE && 
                        pseudoRandom.Next(1,100) <= roughness &&
                        AutomatonUtilities.HasSurroundingBlockDirect(x,y,z,xChunkCount,yChunkCount,zChunkCount,currentBlockMap,Block.BlockType.AIR,countEdge:false))
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
            AutomatonUtilities.CountSurroundingBlocksDirect(x, y, z, XChunkCount, YChunkCount, ZChunkCount, currentBlockMap, 
            Block.BlockType.AIR, countEdge: false) > 0, pseudoRandom);

        for (int i = 0; i < spawnDeepnes; i++) currentBlockMap = AutomatonUtilities.SpawnBlocksRandomly(whatToSpawn, spawnSpreadRate, XChunkCount, YChunkCount, ZChunkCount, CurrentBlockMap,
            (int x, int y, int z) => currentBlockMap[x,y,z] == Block.BlockType.STONE &&
            AutomatonUtilities.CountSurroundingBlocksDirect(x,y,z, XChunkCount, YChunkCount, ZChunkCount, 
            CurrentBlockMap, whatToSpawn, countEdge: false) > 0, pseudoRandom);
    }

    void PutWood()
    {
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
            {
                if (flatMap[x,z] != Block.BlockType.STONE && HasNeighbor(x,z,Block.BlockType.STONE,1) && pseudoRandom.Next(1,100) < WoodChance)
                {
                    flatMap[x,z] = Block.BlockType.WOOD;
                }
            }
    }

    void PutWoodStrebenWithEbene()
    {

        Block.BlockType[,,] newMap = new Block.BlockType[XChunkCount * World.chunkSize, YChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                for (int y = 0; y < YChunkCount * World.chunkSize; y++)
                {
                    newMap[x, y, z] = currentBlockMap[x, y, z];
                    if (y >= yChunkCount * World.chunkSize - 8 &&
                        (AutomatonUtilities.HasSurroundingBlockAllDirections(x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap,
                        Block.BlockType.WOOD, woodStrebeLänge, yDirection: false, xDirection: false, countEdge: false) ||
                        AutomatonUtilities.HasSurroundingBlockAllDirections(x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap,
                        Block.BlockType.WOOD, woodStrebeLänge, yDirection: false, zDirection: false, countEdge: false)))
                    {
                        newMap[x, y, z] = Block.BlockType.WOOD;
                    }

                }
        currentBlockMap = newMap;
    }

    void PutWoodStreben()
    {
        Block.BlockType[,,] newMap = new Block.BlockType[XChunkCount * World.chunkSize, YChunkCount * World.chunkSize, ZChunkCount * World.chunkSize];
        for (int y = 0; y < YChunkCount * World.chunkSize; y++)
            for (int x = 0; x < XChunkCount * World.chunkSize; x++)
                for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    newMap[x, y, z] = currentBlockMap[x, y, z];
                    if ( y >= yChunkCount * World.chunkSize - 8 &&
                        (AutomatonUtilities.IsBetweenBlock(x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap,
                        Block.BlockType.WOOD, woodStrebeLänge, yDirection: false, xDirection: false, countEdge: false) ||
                        AutomatonUtilities.IsBetweenBlock(x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap,
                        Block.BlockType.WOOD, woodStrebeLänge, yDirection: false, zDirection: false, countEdge: false)))
                    {
                        newMap[x, y, z] = Block.BlockType.WOOD;
                    }

                }
        currentBlockMap = newMap;
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
                    if (CurrentRuleset.checkDeath(neighboreCount))
                    {
                        newFlatMap[x, z] = Block.BlockType.AIR;
                    }
                }
                else
                {
                    if (CurrentRuleset.checkBirth(neighboreCount))
                    {
                        newFlatMap[x, z] = Block.BlockType.STONE;
                    }
                }
			}
        flatMap = newFlatMap;
	}

    void CreateCelingCurve(int[] distanceArray){
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize,yChunkCount * World.chunkSize,zChunkCount * World.chunkSize];
        for (int i = 0; i < distanceArray.Length; i++)
        {
            int y = World.chunkSize * YChunkCount - i - 2;
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    newMap[x,y,z] = AutomatonUtilities.HasSurroundingBlock(celingNeighbortype,x,y,z,xChunkCount,yChunkCount,zChunkCount,currentBlockMap,
                        Block.BlockType.STONE,distanceArray[i], yDirection:false) ?
                            Block.BlockType.STONE : currentBlockMap[x,y,z];
                }            
        }
        for (int y =  0; y < World.chunkSize * YChunkCount - distanceArray.Length - 1; y++)
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                    {
                        newMap[x,y,z] = currentBlockMap[x,y,z];
                    }
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    newMap[x,World.chunkSize * YChunkCount - 1,z] = currentBlockMap[x,World.chunkSize * YChunkCount -1,z];
                }
        currentBlockMap = newMap;
    }

    void CreateGroundCurve(int[] distanceArray)
    {
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        for (int i = 0; i < distanceArray.Length; i++)
        {
            int y = i + 1;
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    newMap[x, y, z] = AutomatonUtilities.HasSurroundingBlock(groundNeighbortype,x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap,
                        Block.BlockType.STONE, distanceArray[i], yDirection: false) ?
                            Block.BlockType.STONE : currentBlockMap[x, y, z];
                }
        }
        for (int y = distanceArray.Length + 1; y < World.chunkSize * YChunkCount; y++)
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    newMap[x, y, z] = currentBlockMap[x, y, z];
                }
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int z = 0; z < zChunkCount * World.chunkSize; z++)
            {
                newMap[x, 0, z] = currentBlockMap[x, 0, z];
            }
        currentBlockMap = newMap;
    }

    void CreateBottom(){
        for (int x = 0; x < XChunkCount * World.chunkSize; x++)
            for (int z = 0; z < ZChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, 0, z] = Block.BlockType.STONE;
                }
    }

    void SmoothBottom(){
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
                        neueEbene[x, z] = AutomatonUtilities.CountSurroundingBlocksDirect(x, y, z, XChunkCount, YChunkCount, ZChunkCount, currentBlockMap, Block.BlockType.AIR, 1, true) >= 2 ? 
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

