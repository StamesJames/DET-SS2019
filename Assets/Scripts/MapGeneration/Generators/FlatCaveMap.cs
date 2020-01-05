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

        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < yChunkCount * World.chunkSize; y++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = flatMap[x, z];
                }

        CreateBottom();

        for(int i=0;i<8;i++){   //Hier serialized field für iterations vom boden smoothing einfügen (4 bei tiefe 1, 8 bei tiefe 2)
            SmoothBottom();
        }
        
        return currentBlockMap;
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
				int neighboreCount = CountSurroundingBlocks(x,z,Block.BlockType.STONE,flatMap,1);
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

    void CreaterBottomCeling(){
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
                    neueEbene[x, z] = currentBlockMap[x,y,z] == Block.BlockType.STONE ||
                        CountSurroundingBlocks(x, y, z, Block.BlockType.STONE, currentBlockMap, 1) >= 14 ? Block.BlockType.STONE : Block.BlockType.AIR;                        
                }

            for (int x = 0; x < xChunkCount * World.chunkSize; x++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = neueEbene[x, z];
                    Debug.Log( ( (currentBlockMap[x, y, z] == neueEbene[x, z]) ? "IST GLEICH" : "NICHT GLEICH" ));
                }
        }
    }

    int CountSurroundingBlocks(int gridX, int gridZ, Block.BlockType blockType, Block.BlockType[,] map, int distance, bool negative = false)
    {
        int blockCount = 0;
        for (int x = gridX - distance; x <= gridX + distance; x++)
            for (int z = gridZ - distance; z <= gridZ + distance; z++)
            {
                if (x >= 0 && x < xChunkCount * World.chunkSize && z >= 0 && z < zChunkCount * World.chunkSize)
                {
                    if (x != gridX || z != gridZ)
                    {
                        blockCount += map[x, z] == blockType ? ( negative ? 0 : 1) : (negative ? 1 : 0);
                    }
                }
                else
                {
                    blockCount++;
                }
            }
        return blockCount;
    }

    void CreateBottom(){
        for (int i = 0; i < xChunkCount * World.chunkSize; i++)
            for (int j = 0; j < yChunkCount * World.chunkSize; j++)
                {
                    currentBlockMap[i, j, zChunkCount*World.chunkSize-1] = Block.BlockType.STONE;
                }
    }

    void SmoothBottom(){
            
        Block.BlockType[,,] tmpBlockMap = currentBlockMap;

        for (int k = 1; k < zChunkCount*World.chunkSize-1 ; k++)
            for (int i = 1; i < xChunkCount * World.chunkSize-1; i++)
                for (int j = 1; j < yChunkCount * World.chunkSize-1; j++){
                    if(getNeighbours(i,j,k)>=12)     //anzahl neighbours evtl variabel machen?
                    {
                        tmpBlockMap[i,j,k] = Block.BlockType.STONE;
                    }
                }
        
        currentBlockMap=tmpBlockMap;
    }
  
    int CountSurroundingBlocks(int i,int j,int k, Block.BlockType blockType, Block.BlockType[,,] map, int distance, bool negative = false){
        int blockCount = 0;

        for (int x = -distance; x <= distance; x++)
            for (int y = -distance; y <= distance; y++)
                for (int z = -distance; z <= distance; z++)
                {
                    if ( x != 0 || y != 0 || z != 0 )
                    {
                        int xi = i + x;
                        int yj = j + y;
                        int zk = k + z;
                        if (xi < xChunkCount * World.chunkSize && xi >= 0 &&
                            yj < yChunkCount * World.chunkSize && yj >= 0 &&
                            zk < zChunkCount * World.chunkSize && zk >= 0)
                        {
                                blockCount += map[xi, yj, zk] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                        }
                        else
                        {
                            blockCount++;
                        }
                    }
                }
        return blockCount;
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