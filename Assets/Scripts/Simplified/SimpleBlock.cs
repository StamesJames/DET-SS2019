using UnityEngine;

public enum SimpleBlockType
{
    GRASS, DIRT, STONE, SAND, BEDROCK, AIR
};

public static class SimpleBlockUVs
{
    public static Vector2[,] blockUVs = 
    { 
		/*GRASS TOP*/		{new Vector2( 0.125f, 0.375f ), new Vector2( 0.1875f, 0.375f),
                                new Vector2( 0.125f, 0.4375f ),new Vector2( 0.1875f, 0.4375f )},
		/*GRASS SIDE*/		{new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                                new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
		/*DIRT*/			{new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                                new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
		/*STONE*/			{new Vector2( 0, 0.875f ), new Vector2( 0.0625f, 0.875f),
                                new Vector2( 0, 0.9375f ),new Vector2( 0.0625f, 0.9375f )},	    
		/*SAND*/			{ new Vector2(0.125f,0.875f),  new Vector2(0.1875f,0.875f),
                                 new Vector2(0.125f,0.9375f), new Vector2(0.1875f,0.9375f)},
		/*BEDROCK*/			{new Vector2( 0.3125f, 0.8125f ), new Vector2( 0.375f, 0.8125f),
                                new Vector2( 0.3125f, 0.875f ),new Vector2( 0.375f, 0.875f )}
    };
}

public struct SimpleBlock
{
    #region Member Fields
    public SimpleBlockType blockType;
    public SimpleChunk owner;
    public GameObject gameObject;
    public Vector3 position;
    #endregion

    #region Constructor
    public SimpleBlock(SimpleBlockType blockType, SimpleChunk owner, GameObject gameObject, Vector3 position)
    {
        this.blockType = blockType;
        this.owner = owner;
        this.gameObject = gameObject;
        this.position = position;
    }
    #endregion
}
