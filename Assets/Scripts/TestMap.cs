using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMap : MapGenerator
{

    public TestMap()
    {
        xChunkCount = 3;
        yChunkCount = 3;
        zChunkCount = 3;
        currentBlockMap = new Block.BlockType[World.chunkSize * 3, World.chunkSize * 3, World.chunkSize * 3];
        for (int i = 0; i < World.chunkSize * 3; i++)
            for (int j = 0; j < World.chunkSize * 3; j++)
                for (int k = 0; k < World.chunkSize * 3; k++)
                {
                    currentBlockMap[i, j, k] = Block.BlockType.DIAMOND;
                }
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        return currentBlockMap;
    }
}
