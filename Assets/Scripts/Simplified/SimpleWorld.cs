using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;

public class SimpleWorld : Singleton<SimpleWorld>
{
    #region Member Fields
    [SerializeField]
    private int _seed = 0;
    [SerializeField]
    private int _worldLength = 3;
    [SerializeField]
    private int _worldHeight = 2;
    [SerializeField]
    private int _worldWidth = 3;
    [SerializeField]
    private int _chunkLength = 8;
    [SerializeField]
    private int _chunkHeight = 16;
    [SerializeField]
    private int _chunkWidth = 8;

    private SimpleChunk[,,] _chunks;
    [SerializeField]
    private Material _blockMaterial;
    #endregion

    #region Member Properties
    public int WorldLength
    {
        get { return _worldLength; }
    }

    public int WorldHeight
    {
        get { return _worldHeight; }
    }

    public int WorldWidth
    {
        get { return _worldWidth; }
    }

    public int ChunkLength
    {
        get { return _chunkLength; }
    }

    public int ChunkHeight
    {
        get { return _chunkHeight; }
    }

    public int ChunkWidth
    {
        get { return _chunkWidth; }
    }
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        // Create chunk data
        _chunks = new SimpleChunk[_worldLength, _worldHeight, _worldWidth];
        for (int z = 0; z < _worldWidth; z++)
            for (int y = 0; y < _worldHeight; y++)
                for (int x = 0; x < _worldLength; x++)
                {
                    _chunks[x, y, z] = new SimpleChunk(new Vector3(x,y,z), _chunkLength, _chunkHeight, _chunkWidth, _blockMaterial);
                }

        // Render chunk data
        for (int z = 0; z < _worldWidth; z++)
            for (int y = 0; y < _worldHeight; y++)
                for (int x = 0; x < _worldLength; x++)
                {
                    _chunks[x, y, z].Draw();
                }
    }
    #endregion

    #region Private Methods

    #endregion

    #region Public Methods
    public SimpleBlock GetBlock(int x, int y, int z)
    {
        int chunkX = x / _chunkLength;
        int localX = x % (_chunkLength);

        int chunkY = y / _chunkHeight;
        int localY = y % (_chunkHeight);

        int chunkZ = z / _chunkWidth;
        int localZ = z % (_chunkWidth);

        return _chunks[chunkX, chunkY, chunkZ].GetLocalBlock(localX, localY, localZ);
    }
    #endregion
}
