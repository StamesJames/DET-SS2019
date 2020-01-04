using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlatCaveMap : MapGenerator
{

    public int[,,] layout = new int[World.chunkSize * 3, World.chunkSize * 3, World.chunkSize * 3];
    public int randomFillPercent = 40;
    int width = World.chunkSize*3;
    int height = World.chunkSize*3;

    int länge = World.chunkSize*3;
    

    public FlatCaveMap()
    {
        xChunkCount = 3;
        yChunkCount = 3;
        zChunkCount = 3;
        currentBlockMap = new Block.BlockType[länge, länge, länge];

        RandomFillMap();

        for(int i =0;i<1;i++) SmoothMap();

        for (int i = 0; i < länge; i++)
            for (int j = 0; j < länge; j++)
                for (int k = 0; k < länge; k++)
                {
                    if(layout[i,j,3]==1)
                    {
                        currentBlockMap[i, k, j] = Block.BlockType.DIAMOND;
                    }else{
                        currentBlockMap[i, k, j] = Block.BlockType.AIR;
                    }
                }

        CreaterBottomCeling();
    }

    public override Block.BlockType[,,] GenerateMap()
    {
        return currentBlockMap;
    }





    void RandomFillMap()
    {

        System.Random pseudoRandom = new System.Random();
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    layout[x, y,3] = 1;
                }
                else
                {
                    layout[x, y,3] = (pseudoRandom.Next(1,100) < randomFillPercent) ? 1 : 0;
                    Debug.Log(pseudoRandom.Next(1,100));
                }
            }
        }
    }

    void SmoothMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y,3);

				if (neighbourWallTiles > 4)
					layout[x,y,3] = 1;
				else if (neighbourWallTiles < 4)
					layout[x,y,3] = 0;

			}
		}
	}

    int GetSurroundingWallCount(int gridX, int gridY, int gridZ) 
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += layout[neighbourX, neighbourY,3];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    void CreaterBottomCeling(){
        for (int i = 0; i < länge; i++){
            for (int j = 0; j < länge; j++){
                for (int k = 0; k < länge; k++){
                    currentBlockMap[i, 1, j] = Block.BlockType.DIAMOND;
                    currentBlockMap[i, 2, j] = Block.BlockType.DIAMOND;

                }
            }
        }
        for (int i = länge/2; i>1; i--){
            for (int j = 0; j < länge; j++){
                for (int k = 0; k < länge; k++){
                    if(currentBlockMap[j, i, k]==Block.BlockType.DIAMOND)
                    {
                        currentBlockMap[j,i-1,k] = Block.BlockType.DIRT;
                    }
                }
            }
        }
    }





}
