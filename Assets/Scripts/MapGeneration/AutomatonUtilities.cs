using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class AutomatonUtilities
{
    public static int CountSurroundingBlocks(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true, 
        bool xDirection = true, bool yDirection = true, bool zDirection = true)
    {
        int blockCount = 0;
        for (int xd = ( xDirection ? -distance : 0 ); xd <= ( xDirection ? distance : 0 ); xd++)
            for (int yd = (yDirection ? -distance : 0); yd <= (yDirection ? distance : 0); yd++)
                for (int zd = (zDirection ? -distance : 0); zd <= (zDirection ? distance : 0); zd++)
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
        int distance = 1, bool negative = false, bool countEdge = true,
        bool xDirection = true, bool yDirection = true, bool zDirection = true)
    {
        int blockCount = 0;
        for (int d = -distance; d <= distance; d++)
        {
            if (d != 0)
            {
                int xi = x + (xDirection ? d : 0);
                int yj = y + (yDirection ? d : 0);
                int zk = z + (zDirection ? d : 0);
                if (xi < xChunkCount * World.chunkSize && xi >= 0 && xDirection)
                {
                    blockCount += map[xi, y, z] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge && xDirection)
                {
                    blockCount++;
                }
                if (yj < yChunkCount * World.chunkSize && yj >= 0 && yDirection)
                {
                    blockCount += map[x, yj, z] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge && yDirection)
                {
                    blockCount++;
                }
                if (zk < zChunkCount * World.chunkSize && zk >= 0 && zDirection)
                {
                    blockCount += map[x, y, zk] == blockType ? (negative ? 0 : 1) : (negative ? 1 : 0);
                }
                else if (countEdge && zDirection)
                {
                    blockCount++;
                }
            }
        }
        return blockCount;
    }

    public static bool HasSurroundingBlockAllDirections(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType,
    int distance = 1, bool negative = false, bool countEdge = true,
    bool xDirection = true, bool yDirection = true, bool zDirection = true)
    {
        bool inXpos = !xDirection;
        bool inYpos = !yDirection;
        bool inZpos = !zDirection;
        bool inXneg = !xDirection;
        bool inYneg = !yDirection;
        bool inZneg = !zDirection;
        for (int d = -distance; d <= distance; d++)
        {
            if (d != 0)
            {
                int xi = x + (xDirection ? d : 0);
                int yj = y + (yDirection ? d : 0);
                int zk = z + (zDirection ? d : 0);
                if (xi < xChunkCount * World.chunkSize && xi >= 0 && xDirection)
                {
                    if (map[xi, y, z] == blockType)
                    {
                        if (d > 0) inXpos = true;
                        else inXneg = true;

                    }
                }
                else if (countEdge && xDirection)
                {
                    if (d > 0) inXpos = true;
                    else inXneg = true;
                }
                if (yj < yChunkCount * World.chunkSize && yj >= 0 && yDirection)
                {
                    if (map[x, yj, z] == blockType)
                    {
                        if (d > 0) inYpos = true;
                        else inYneg = true;
                    }
                }
                else if (countEdge && yDirection)
                {
                    if (d > 0) inYpos = true;
                    else inYneg = true;
                }
                if (zk < zChunkCount * World.chunkSize && zk >= 0 && zDirection)
                {
                    if (map[x, y, zk] == blockType)
                    {
                        if (d > 0) inZpos = true;
                        else inZneg = true;
                    }
                }
                else if (countEdge && zDirection)
                {
                    if (d > 0) inZpos = true;
                    else inZneg = true;
                }
            }
        }
        return inXpos && inYpos && inZpos && inXneg && inYneg && inZneg;
    }

    public static bool IsBetweenBlock(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType,
        int distance = 1, bool negative = false, bool countEdge = true,
        bool xDirection = true, bool yDirection = true, bool zDirection = true)
    {
        int xpos = xDirection ? distance + 10 : 0;
        int ypos = yDirection ? distance + 10 : 0;
        int zpos = zDirection ? distance + 10 : 0;
        int xneg = xDirection ? distance + 10 : 0;
        int yneg = yDirection ? distance + 10 : 0;
        int zneg = zDirection ? distance + 10 : 0;
        for (int d = 1; d < distance; d++)
        {
            if ( x + d < xChunkCount * World.chunkSize && xpos > distance && map[x + d, y, z] == blockType) xpos = d;
            if ( x - d >= 0 && xneg > distance && map[x - d, y, z] == blockType) xneg = d;
            if ( y + d < yChunkCount * World.chunkSize && ypos > distance && map[x, y + d, z] == blockType) ypos = d;
            if ( y - d >= 0 && yneg > distance && map[x, y - d, z] == blockType) yneg = d;
            if ( z + d < zChunkCount * World.chunkSize && zpos > distance && map[x, y, z + d] == blockType) zpos = d;
            if ( z - d >= 0 && zneg > distance && map[x, y, z - d] == blockType) zneg = d;
        }


        return (xpos + xneg <= distance) && (ypos + yneg <= distance) && (zpos + zneg <= distance);
    }

    public static bool HasSurroundingBlockDirect(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType,
        int distance = 1, bool negative = false, bool countEdge = true,
        bool xDirection = true, bool yDirection = true, bool zDirection = true)
    {
        for (int d = -distance; d <= distance; d++)
        {
            if (d != 0)
            {
                int xi = x + (xDirection ? d : 0);
                int yj = y + (yDirection ? d : 0);
                int zk = z + (zDirection ? d : 0);
                if (xi < xChunkCount * World.chunkSize && xi >= 0 && xDirection)
                {
                    if (map[xi, y, z] == blockType)
                    {
                        return true;
                    }
                }
                else if (countEdge && xDirection)
                {
                    return true;
                }
                if (yj < yChunkCount * World.chunkSize && yj >= 0 && yDirection)
                {
                    if (map[x, yj, z] == blockType)
                    {
                        return true;
                    }
                }
                else if (countEdge && yDirection)
                {
                    return true;
                }
                if (zk < zChunkCount * World.chunkSize && zk >= 0 && zDirection)
                {
                    if (map[x, y, zk] == blockType)
                    {
                        return true;
                    }
                }
                else if (countEdge && zDirection)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool HasSurroundingBlockCircle(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true, 
        bool xDirection = true, bool yDirection = true, bool zDirection = true)
    {
        for (int xd = ( xDirection ? -distance : 0 ); xd <= ( xDirection ? distance : 0 ); xd++)
            for (int yd = (yDirection ? -distance : 0); yd <= (yDirection ? distance : 0); yd++)
                for (int zd = (zDirection ? -distance : 0); zd <= (zDirection ? distance : 0); zd++)
                {
                    if (xd != 0 || yd != 0 || zd != 0)
                    {
                        int xi = x + xd;
                        int yj = y + yd;
                        int zk = z + zd;
                        if (xi < xChunkCount * World.chunkSize && xi >= 0 &&
                            yj < yChunkCount * World.chunkSize && yj >= 0 &&
                            zk < zChunkCount * World.chunkSize && zk >= 0 )
                        {
                            if (Mathf.Sqrt(Mathf.Pow(xDirection ? xd : 0,2) + Mathf.Pow(yDirection ? yd : 0,2) + Mathf.Pow(zDirection ? zd : 0,2)) <= distance)
                            {
                                if (map[xi,yj,zk] == blockType)
                                {
                                    return true;
                                }
                            } 
                        }
                        else if (countEdge)
                        {
                            return true;;
                        }
                    }
                }
        return false;
    }

    public static int CountSurroundingBlocksCircle(int x, int y, int z, int xChunkCount, int yChunkCount, int zChunkCount, Block.BlockType[,,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true, 
        bool xDirection = true, bool yDirection = true, bool zDirection = true)
    {
        int blockCount = 0;
        for (int xd = ( xDirection ? -distance : 0 ); xd <= ( xDirection ? distance : 0 ); xd++)
            for (int yd = (yDirection ? -distance : 0); yd <= (yDirection ? distance : 0); yd++)
                for (int zd = (zDirection ? -distance : 0); zd <= (zDirection ? distance : 0); zd++)
                {
                    if (xd != 0 || yd != 0 || zd != 0)
                    {
                        int xi = x + xd;
                        int yj = y + yd;
                        int zk = z + zd;
                        if (xi < xChunkCount * World.chunkSize && xi >= 0 &&
                            yj < yChunkCount * World.chunkSize && yj >= 0 &&
                            zk < zChunkCount * World.chunkSize && zk >= 0 && 
                            Mathf.Sqrt(Mathf.Pow(xi,2) + Mathf.Pow(yj,2) + Mathf.Pow(zk,2)) <= distance)
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

    public static int CountSurroundingBlocks(int x, int z, int xChunkCount, int zChunkCount, Block.BlockType[,] map, Block.BlockType blockType, 
        int distance = 1, bool negative = false, bool countEdge = true,
        bool xDirection = true, bool zDirection = true)
    {
        int blockCount = 0;
        for (int xd = x - ( xDirection ? distance : 0 ); xd <= x + (xDirection ? distance : 0); xd++)
            for (int zd = z - (zDirection ? distance : 0); zd <= z + (zDirection ? distance : 0); zd++)
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
        int distance = 1, bool negative = false, bool countEdge = true,
        bool xDirection = true, bool zDirection = true)
    {
        int blockCount = 0;
        for (int d = -distance; d <= distance; d++)
        {
            if (d != 0)
            {
                int xi = x + (xDirection ? distance : 0);
                int zk = z + (zDirection ? distance : 0);
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
        Func<int, int, int, bool> spawnPredicate, System.Random pseudoRandom)
    {
        Block.BlockType[,,] newMap = new Block.BlockType[xChunkCount * World.chunkSize, yChunkCount * World.chunkSize, zChunkCount * World.chunkSize];
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

    public override string ToString()
    {
        return (von + "-" + bis);
    }

}

[System.Serializable]
public class Ruleset
{
    Intervall[] birthRules;
    Intervall[] deathRules;

    public Ruleset(Intervall[] pBirthrules, Intervall[] pDeathRules)
    {
        this.birthRules = pBirthrules;
        this.deathRules = pDeathRules;
    }

    public override string ToString()
    {
        string name = "";
        string birthRulesString = "";
        string deathRulesString = "";
        for (int i = 0; i < birthRules.Length; i++)
        {
            birthRulesString += birthRules[i].ToString();
            if (i != birthRules.Length -1)
            {
                birthRulesString += ",";
            }
        }

        for (int i = 0; i < deathRules.Length; i++)
        {
            deathRulesString += deathRules[i].ToString();
            if (i != deathRules.Length - 1)
            {
                deathRulesString += ",";
            }
        }

        name = birthRulesString + "/" + deathRulesString;

        return name;
    }

    public bool checkBirth(int count)
    {
        foreach (Intervall birthRule in birthRules)
        {
            if (birthRule.Contains(count))
            {
                return true;
            }
        }
        return false;
    }

    public bool checkDeath(int count)
    {
        foreach (Intervall deathRule in deathRules)
        {
            if (deathRule.Contains(count))
            {
                return true;
            }
        }
        return false;
    }
}
