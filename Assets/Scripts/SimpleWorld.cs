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
    private Vector3 _worldDimensions = new Vector3(4,2,4);
    [SerializeField]
    private Vector3 _chunkDimensions = new Vector3(8,8,8);
    [SerializeField]
    private int _surfaceHeight = 13;

    private SimpleChunk[,,] _chunks;
    [SerializeField]
    private Material _blockMaterial;
    #endregion

    #region Member Properties
    /// <summary>
    /// Dimensions of the world.
    /// </summary>
    public Vector3 WoldDim
    {
        get { return _worldDimensions; }
    }

    /// <summary>
    /// Dimensions of a chunk.
    /// </summary>
    public Vector3 ChunkDim
    {
        get { return _chunkDimensions; }
    }

    /// <summary>
    /// The general surface height to determine whether to spawn grass or air.
    /// </summary>
    public int SurfaceHeight
    {
        get { return _surfaceHeight; }
    }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Create and render chunk data.
    /// </summary>
    private void Start()
    {
        // Create chunk data
        _chunks = new SimpleChunk[(int)_worldDimensions.x, (int)_worldDimensions.y, (int)_worldDimensions.z];
        for (int z = 0; z < (int)_worldDimensions.x; z++)
            for (int y = 0; y < (int)_worldDimensions.y; y++)
                for (int x = 0; x < (int)_worldDimensions.z; x++)
                {
                    _chunks[x, y, z] = new SimpleChunk(new Vector3(x,y,z), _blockMaterial);
                }

        // Render chunk data
        for (int z = 0; z < (int)_worldDimensions.x; z++)
            for (int y = 0; y < (int)_worldDimensions.y; y++)
                for (int x = 0; x < (int)_worldDimensions.z; x++)
                {
                    _chunks[x, y, z].Draw();
                }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Takes a global block position, converts it to a local position in a specific chunk.
    /// The information is then used to acess the desired block.
    /// </summary>
    /// <param name="x">Global x position of the to be returned block</param>
    /// <param name="y">Global y position of the to be returned block</param>
    /// <param name="z">Global z position of the to be returned block</param>
    /// <returns>Returns the reference to the reqeusted block</returns>
    public SimpleBlock GetBlock(int x, int y, int z)
    {
        // Determine the requested chunk and the local position of the blocks
        int chunkX = x / (int)_chunkDimensions.x;
        int localX = x % ((int)_chunkDimensions.x);

        int chunkY = y / (int)_chunkDimensions.y;
        int localY = y % ((int)_chunkDimensions.y);

        int chunkZ = z / (int)_chunkDimensions.z;
        int localZ = z % ((int)_chunkDimensions.z);

        return _chunks[chunkX, chunkY, chunkZ].GetLocalBlock(localX, localY, localZ);
    }
    #endregion
}
