using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AutomatonUtilities
{
    public static int CountSurroundingBlocks(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true)
    {
        int blockCount = 0;
        for (int xd = -distance; xd <= distance; xd++)
            for (int yd = -distance; yd <= distance; yd++)
                for (int zd = -distance; zd <= distance; zd++)
                {
                    if (xd != 0 || yd != 0 || zd != 0)
                    {
                        int xi = x + xd;
                        int yj = y + yd;
                        int zk = z + zd;
                        if (xi < xChunkCount * World.chunkSize && xi >= 0 &&
                            yj < yChunkCount * World.chunkSize && yj >= 0 &&
                            zk < zChunkCount * World.chunkSize && zk >= 0)
                        {
                            blockCount += map[xi, yj, zk] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                        }
                        else if (countEdge)
                        {
                            blockCount++;
                        }
                    }
                }
        return blockCount;
    }

    public static int CountSurroundingBlocksDirect(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true)
    {
        int blockCount = 0;
        for (int d = -distance; d <= distance; d++)
        {
            if (d != 0)
            {
                int xi = x + d;
                int yj = y + d;
                int zk = z + d;
                if (xi < xChunkCount * World.chunkSize && xi >= 0)
                {
                    blockCount += map[xi, y, z] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge)
                {
                    blockCount++;
                }
                if (yj < yChunkCount * World.chunkSize && yj >= 0)
                {
                    blockCount += map[x, yj, z] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge)
                {
                    blockCount++;
                }
                if (zk < zChunkCount * World.chunkSize && zk >= 0)
                {
                    blockCount += map[x, y, zk] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge)
                {
                    blockCount++;
                }
            }
        }
        return blockCount;
    }

    public static int CountSurroundingBlocks(int x, int z, int xChunkCount, int zChunkCount, Block.BlockType[,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true)
    {
        int blockCount = 0;
        for (int xd = x - distance; xd <= x + distance; xd++)
            for (int zd = z - distance; zd <= z + distance; zd++)
            {
                if (xd >= 0 && xd < xChunkCount * World.chunkSize && zd >= 0 && zd < zChunkCount * World.chunkSize)
                {
                    if (xd != x || zd != z)
                    {
                        blockCount += map[xd, zd] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                    }
                }
                else if (countEdge)
                {
                    blockCount++;
                }
            }
        return blockCount;
    }

    public static int CountSurroundingBlocksDirect(int x, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true)
    {
        int blockCount = 0;
        for (int d = -distance; d <= distance; d++)
        {
            if (d != 0)
            {
                int xi = x + d;
                int zk = z + d;
                if (xi < xChunkCount * World.chunkSize && xi >= 0)
                {
                    blockCount += map[xi, z] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge)
                {
                    blockCount++;
                }
                if (zk < zChunkCount * World.chunkSize && zk >= 0)
                {
                    blockCount += map[x, zk] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge)
                {
                    blockCount++;
                }
            }
        }
        return blockCount;
    }

    public static Block.BlockType[,,] SpawnBlocksRandomly(Block.BlockType whatToSpawn, float spawnRate, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, 
        Func<int, int, int, bool> spawnPredicate)
    {
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        System.Random pseudoRandom = new System.Random();
        for (int x = 0; x < xChunkCount * World.chunkSize; x++)
            for (int y = 0; y < yChunkCount * World.chunkSize; y++)
                for (int z = 0; z < zChunkCount * World.chunkSize; z++)
                {
                    newMap[x, y, z] = map[x, y, z];
                    if (spawnPredicate(x,y,z))
                    {
                        float randomInt = pseudoRandom.Next(1, 100);
                        if ( randomInt <= spawnRate)
                        {
                            newMap[x, y, z] = whatToSpawn;
                        }
                    }
                }
        return newMap;
    }


}
