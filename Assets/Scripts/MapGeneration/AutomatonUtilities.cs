using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutomatonUtilities
{
    public static int CountSurroundingBlocks(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType, int distance = 1, bool negative = false)
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
                        else
                        {
                            blockCount++;
                        }
                    }
                }
        return blockCount;
    }

    public static int CountSurroundingBlocks(int x, int z, int xChunkCount, int zChunkCount, Block.BlockType[,] map, Block.BlockType blockType, int distance = 1, bool negative = false)
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
                else
                {
                    blockCount++;
                }
            }
        return blockCount;
    }
}
