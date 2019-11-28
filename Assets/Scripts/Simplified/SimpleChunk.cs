using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleChunk
{
    #region Member Fields
    private SimpleBlock[,,] _chunkData;
    private Material _blockMaterial;
    private Vector3 _position;
    private GameObject _gameObject;
    private int _chunkLength;
    private int _chunkHeight;
    private int _chunkWidth;
    #endregion

    #region Constructor
    public SimpleChunk(Vector3 position, int chunkLength, int chunkHeight, int chunkWidth, Material blockMaterial)
    {
        _blockMaterial = blockMaterial;
        _position = new Vector3(position.x * chunkLength,
                                position.y * chunkHeight,
                                position.z * chunkWidth);
        _chunkLength = chunkLength;
        _chunkHeight = chunkHeight;
        _chunkWidth = chunkWidth;

        // New GameObject
        _gameObject = new GameObject(World.BuildChunkName(_position));
        _gameObject.transform.position = _position;
        _chunkData = new SimpleBlock[_chunkLength, _chunkHeight, _chunkWidth];
        // Populate chunk data with blocks
        for (int z = 0; z < _chunkWidth; z++)
            for (int y = 0; y < _chunkHeight; y++)
                for (int x = 0; x < _chunkLength; x++)
                {
                    _chunkData[x, y, z] = new SimpleBlock(SimpleBlockType.DIRT, this, _gameObject, new Vector3(x,y,z));
                }
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Draws each block individually at first and then combines all Quads into one mesh in order to start rendering it.
    /// </summary>
    public void Draw()
    {
        for (int z = 0; z < _chunkWidth; z++)
            for (int y = 0; y < _chunkHeight; y++)
                for (int x = 0; x < _chunkLength; x++)
                {
                    DrawBlock(_chunkData[x, y, z]);
                }

        CombineQuads();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">Local position x of the to be returned block</param>
    /// <param name="y">Local position y of the to be returned block</param>
    /// <param name="z">Local position z of the to be returned block</param>
    /// <returns>Returns the requested block</returns>
    public SimpleBlock GetLocalBlock(int x, int y, int z)
    {
        return _chunkData[x, y, z];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="blockType"></param>
    public void SetLocalBlock(int x, int y, int z, SimpleBlockType blockType)
    {
        _chunkData[x, y, z].blockType = blockType;
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Creates Quads for the specified block.
    /// </summary>
    /// <param name="block">The to be drawn block by creating its Quads.</param>
    private void DrawBlock(SimpleBlock block)
    {
        // Create Quads
        // Don't draw Quads for blocks of type air
        if(block.blockType != SimpleBlockType.AIR)
        {
            // Convert current local block position to global by adding the chunk's position
            Vector3 globalBlockPosition = new Vector3(_position.x  + block.position.x,
                                          _position.y + block.position.y,
                                          _position.z + block.position.z);
            // Create Quads only in the case of having no neighoring block or no solid block
            if (!HasSolidNeighbor((int)globalBlockPosition.x, (int)globalBlockPosition.y, (int)globalBlockPosition.z + 1))
                CreateQuad(Cubeside.FRONT, block.position, block.blockType);
            if (!HasSolidNeighbor((int)globalBlockPosition.x, (int)globalBlockPosition.y, (int)globalBlockPosition.z - 1))
                CreateQuad(Cubeside.BACK, block.position, block.blockType);
            if (!HasSolidNeighbor((int)globalBlockPosition.x, (int)globalBlockPosition.y + 1, (int)globalBlockPosition.z))
                CreateQuad(Cubeside.TOP, block.position, block.blockType);
            if (!HasSolidNeighbor((int)globalBlockPosition.x, (int)globalBlockPosition.y - 1, (int)globalBlockPosition.z))
                CreateQuad(Cubeside.BOTTOM, block.position, block.blockType);
            if (!HasSolidNeighbor((int)globalBlockPosition.x + 1, (int)globalBlockPosition.y, (int)globalBlockPosition.z))
                CreateQuad(Cubeside.RIGHT, block.position, block.blockType);
            if (!HasSolidNeighbor((int)globalBlockPosition.x - 1, (int)globalBlockPosition.y, (int)globalBlockPosition.z))
                CreateQuad(Cubeside.LEFT, block.position, block.blockType);
        }
    }

    /// <summary>
    /// Searches for the specified block in the world and returns true if it is solid.
    /// </summary>
    /// <param name="x">Global x position of the to be checked block</param>
    /// <param name="y">Global y position of the to be checked block</param>
    /// <param name="z">Global z position of the to be checked block</param>
    /// <returns>Returns true if a block was found which is not of type air.</returns>
    private bool HasSolidNeighbor(int x, int y, int z)
    {
        try
        {
            SimpleBlock neighborBlock = SimpleWorld.Instance.GetBlock(x, y, z);
            if (neighborBlock != null)
            {
                return neighborBlock.blockType != SimpleBlockType.AIR;
            }
        }
        catch (System.IndexOutOfRangeException) { }
        return false;
    }

    /// <summary>
    /// Assembles one side of a cube's mesh by selecting the UVs, defining the vertices and calculating the normals.
    /// </summary>
    /// <param name="side">Quad to be created for this side</param>
    /// <param name="position">Local position of the block</param>
    /// <param name="blockType">Block type</param>
	private void CreateQuad(Cubeside side, Vector3 position, SimpleBlockType blockType)
    {
        Mesh mesh = new Mesh();
        mesh.name = "ScriptedMesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        List<Vector2> suvs = new List<Vector2>();
        int[] triangles = new int[6];

        // All possible UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (blockType == SimpleBlockType.GRASS && side == Cubeside.TOP)
        {
            uv00 = SimpleBlockUVs.blockUVs[0, 0];
            uv10 = SimpleBlockUVs.blockUVs[0, 1];
            uv01 = SimpleBlockUVs.blockUVs[0, 2];
            uv11 = SimpleBlockUVs.blockUVs[0, 3];
        }
        else if (blockType == SimpleBlockType.GRASS && side == Cubeside.BOTTOM)
        {
            uv00 = SimpleBlockUVs.blockUVs[(int)(SimpleBlockType.DIRT + 1), 0];
            uv10 = SimpleBlockUVs.blockUVs[(int)(SimpleBlockType.DIRT + 1), 1];
            uv01 = SimpleBlockUVs.blockUVs[(int)(SimpleBlockType.DIRT + 1), 2];
            uv11 = SimpleBlockUVs.blockUVs[(int)(SimpleBlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = SimpleBlockUVs.blockUVs[(int)(blockType + 1), 0];
            uv10 = SimpleBlockUVs.blockUVs[(int)(blockType + 1), 1];
            uv01 = SimpleBlockUVs.blockUVs[(int)(blockType + 1), 2];
            uv11 = SimpleBlockUVs.blockUVs[(int)(blockType + 1), 3];
        }

        // All possible vertices 
        // Top vertices
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        // Bottom vertices
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        switch (side)
        {
            case Cubeside.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] {Vector3.down, Vector3.down,
                                            Vector3.down, Vector3.down};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case Cubeside.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] {Vector3.up, Vector3.up,
                                            Vector3.up, Vector3.up};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case Cubeside.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] {Vector3.left, Vector3.left,
                                            Vector3.left, Vector3.left};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case Cubeside.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] {Vector3.right, Vector3.right,
                                            Vector3.right, Vector3.right};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case Cubeside.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] {Vector3.forward, Vector3.forward,
                                            Vector3.forward, Vector3.forward};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case Cubeside.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] {Vector3.back, Vector3.back,
                                            Vector3.back, Vector3.back};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.SetUVs(1, suvs);
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        quad.transform.position = position;
        quad.transform.parent = _gameObject.transform;

        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
    }

    /// <summary>
    /// Combines all Quads into one Mesh and creates all components to get started rendering.
    /// </summary>
    private void CombineQuads()
    {
        // 1. Combine all children meshes
        MeshFilter[] meshFilters = _gameObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        // 2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter)_gameObject.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        // 3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        // 4. Create a renderer for the parent
        MeshRenderer renderer = _gameObject.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = _blockMaterial;

        // 5. Delete all uncombined children
        foreach (Transform quad in _gameObject.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
    #endregion
}
