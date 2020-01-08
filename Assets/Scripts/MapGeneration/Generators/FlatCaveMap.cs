using UnityEngine;

[System.Serializable]
public class FlatCaveMap : MapGenerator
{

    private Block.BlockType[,] flatMap;

    [SerializeField] private int randomFillPercent = 40;
    public int RandomFillPercent { get => randomFillPercent; set => randomFillPercent = value; }
    [SerializeField] private int smoothingItterations = 3;
    public int SmoothingItterations { get => smoothingItterations; set => smoothingItterations = value; }
    [SerializeField] private Intervall[] birthIntervalls;
    [SerializeField] private Intervall[] deathIntervalls;
    [SerializeField] private int woodChance = 10;
    [SerializeField] private int celingNeighbors = 4;
    [SerializeField] private int bottomNeighbors = 4;

    public FlatCaveMap()
    {
        xChunkCount = 3;
        yChunkCount = 3;
        zChunkCount = 3;
        birthIntervalls = new Intervall[] { new Intervall(5, 8) };
        deathIntervalls = new Intervall[] { new Intervall(0, 3) };
        GenerateMap();
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        flatMap = new Block.BlockType[xChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        currentBlockMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        RandomFillMap();

        for (int i = 0; i < smoothingItterations; i++) SmoothMap();

        PutWood();

        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < yChunkCount * World.chunkSize; y++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = flatMap[x, z];
                }

        CreaterCeling();
        CreateBottom();

        SmoothBottom();
        
        return currentBlockMap;
    }

    void PutWood()
    {
        System.Random pseudoRandom = new System.Random();
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int z = 0; z < zChunkCount * World.chunkSize; z++)
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
                if (xi >= 0 && xi < xChunkCount * World.chunkSize && zj >= 0 && zj < zChunkCount * World.chunkSize)
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
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int z = 0; z < zChunkCount * World.chunkSize; z++)
            {
                if (x == 0 || x == xChunkCount * World.chunkSize - 1 || z == 0 || z == zChunkCount * World.chunkSize - 1)
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
        Block.BlockType[,] newFlatMap = new Block.BlockType[xChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
		for (int x = 0; x < xChunkCount * World.chunkSize; x ++)
			for (int z = 0; z < zChunkCount * World.chunkSize; z ++)
            {
				int neighboreCount = AutomatonUtilities.CountSurroundingBlocks(x,z,xChunkCount, zChunkCount, flatMap, Block.BlockType.AIR, 1,true);
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
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, yChunkCount * World.chunkSize - 1, z] = Block.BlockType.STONE;
                }
        Block.BlockType[,] neueEbene = new Block.BlockType[xChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        for (int y = yChunkCount * World.chunkSize - 2; y > 0; y--)
        {
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    if (currentBlockMap[x,y,z] != Block.BlockType.AIR)
                    {
                        neueEbene[x, z] = currentBlockMap[x, y, z];
                    }
                    else
                    {
                        neueEbene[x, z] = AutomatonUtilities.CountSurroundingBlocks(x, y, z, xChunkCount,yChunkCount,zChunkCount, currentBlockMap, Block.BlockType.AIR, 1, true) >= 14 ? 
                            Block.BlockType.STONE : Block.BlockType.AIR;
                    }
                }
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = neueEbene[x, z];
                }
        }
    }



    void CreateBottom(){
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, 0, z] = Block.BlockType.STONE;
                }
    }

    void SmoothBottom(){
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int z = 0; z < zChunkCount * World.chunkSize; z++)
            {
                currentBlockMap[x, yChunkCount * World.chunkSize - 1, z] = Block.BlockType.STONE;
            }
        Block.BlockType[,] neueEbene = new Block.BlockType[xChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        for (int y = 0; y < yChunkCount; y++)
        {
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    if (currentBlockMap[x, y, z] != Block.BlockType.AIR)
                    {
                        neueEbene[x, z] = currentBlockMap[x, y, z];
                    }
                    else
                    {
                        neueEbene[x, z] = AutomatonUtilities.CountSurroundingBlocks(x, y, z, xChunkCount, yChunkCount, zChunkCount, currentBlockMap, Block.BlockType.AIR, 1, true) >= 14 ? 
                            Block.BlockType.STONE : Block.BlockType.AIR;
                    }
                }
            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
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