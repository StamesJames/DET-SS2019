﻿using UnityEngine;

[System.Serializable]
public class FlatCaveMap : MapGenerator
{

    private Block.BlockType[,] flatMap;

    [SerializeField] private int randomFillPercent = 40;
    [SerializeField] private int smoothingItterations = 3;
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
        flatMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize];
        currentBlockMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        RandomFillMap();

        for (int i = 0; i < 1; i++) SmoothMap();

        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < yChunkCount * World.chunkSize; y++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    currentBlockMap[x, y, z] = flatMap[x, y];
                }
        CreaterBottomCeling();

        return currentBlockMap;
    }

    void RandomFillMap()
    {
        System.Random pseudoRandom = new System.Random();       
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < yChunkCount * World.chunkSize; y++)
            {
                if (x == 0 || x == xChunkCount * World.chunkSize - 1 || y == 0 || y == yChunkCount * World.chunkSize - 1)
                {
                    flatMap[x, y] = Block.BlockType.STONE;
                }
                else
                {
                    flatMap[x, y] = (pseudoRandom.Next(1,100) < randomFillPercent) ? Block.BlockType.STONE : Block.BlockType.AIR;
                }
            }       
    }

    void SmoothMap() {
        Block.BlockType[,] newFlatMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize];
		for (int x = 0; x < xChunkCount * World.chunkSize; x ++)
			for (int y = 0; y < yChunkCount * World.chunkSize; y ++)
            {
				int neighboreCount = CountSurroundingBlocks(x,y,Block.BlockType.STONE,flatMap,1);
                newFlatMap[x, y] = flatMap[x, y];
                if (flatMap[x,y] == Block.BlockType.STONE)
                {
                    foreach (Intervall deathIntervall in deathIntervalls)
                    {
                        if (deathIntervall.Contains(neighboreCount))
                        {
                            newFlatMap[x, y] = Block.BlockType.AIR;
                        }
                    }
                }
                else
                {
                    foreach (Intervall birthIntervall in birthIntervalls)
                    {
                        if (birthIntervall.Contains(neighboreCount))
                        {
                            newFlatMap[x, y] = Block.BlockType.STONE;
                        }
                    }
                }
			}
        flatMap = newFlatMap;
	}

    int CountSurroundingBlocks(int gridX, int gridY, Block.BlockType blockType, Block.BlockType[,] map, int distance) 
    {
        int blockCount = 0;
        for (int neighbourX = gridX - distance; neighbourX <= gridX + distance; neighbourX++)        
            for (int neighbourY = gridY - distance; neighbourY <= gridY + distance; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < xChunkCount * World.chunkSize && neighbourY >= 0 && neighbourY < yChunkCount * World.chunkSize)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        blockCount += flatMap[neighbourX,neighbourY] == blockType ? 1 : 0;
                    }
                }
                else
                {
                    blockCount++;
                }
            }
        return blockCount;
    }

    void CreaterBottomCeling(){
        for (int i = 0; i < xChunkCount * World.chunkSize; i++)
            for (int j = 0; j < yChunkCount * World.chunkSize; j++)
                for (int k = 0; k < zChunkCount * World.chunkSize; k++){
                    currentBlockMap[i, 1, j] = Block.BlockType.DIAMOND;
                    currentBlockMap[i, 2, j] = Block.BlockType.DIAMOND;

                }
            
        
        for (int i = xChunkCount * World.chunkSize / 2; i>1; i--)
            for (int j = 0; j < yChunkCount * World.chunkSize; j++)
                for (int k = 0; k < zChunkCount * World.chunkSize; k++){
                    if(currentBlockMap[j, i, k]==Block.BlockType.DIAMOND)
                    {
                        currentBlockMap[j,i-1,k] = Block.BlockType.DIRT;
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