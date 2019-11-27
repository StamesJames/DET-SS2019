using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWorld : MonoBehaviour
{
    #region Member Fields
    [SerializeField]
    private int _seed = 0;
    [SerializeField]
    private int _worldLength = 12;
    [SerializeField]
    private int _worldHeight = 4;
    [SerializeField]
    private int _worldWidth = 12;
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

    #endregion
}
