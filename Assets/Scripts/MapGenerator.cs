﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator
{
    protected Block.BlockType[,,] currentBlockMap;
    public Block.BlockType[,,] CurrentBlockMap { get => currentBlockMap; }

    protected int xChunkCount, yChunkCount, zChunkCount;
    public int XChunkCount { get => xChunkCount; set => xChunkCount = value; }
    public int YChunkCount { get => yChunkCount; set => yChunkCount = value; }
    public int ZChunkCount { get => zChunkCount; set => zChunkCount = value; }

    abstract public Block.BlockType[,,] GenerateMap();
}
