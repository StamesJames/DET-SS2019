using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MapGenerator
{

    public TestMap()
    {
        xChunkCount = 1;
        yChunkCount = 1;
        zChunkCount = 1;
        GenerateMap();
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        currentBlockMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
        for (int i = 0; i < World.chunkSize * xChunkCount; i++)
            for (int j = 0; j < World.chunkSize * yChunkCount; j++)
                for (int k = 0; k < World.chunkSize * zChunkCount; k++)
                {
                    currentBlockMap[i, j, k] = Block.BlockType.WOOD;
                }
        return currentBlockMap;
    }
}
