using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Tilemap tilemap;
    public Tile tile;
    public TilemapRenderer tilemapRenderer;
    public float[,] grid = new float[46, 24];
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        GetTileInfo();
    }
    void Start()
    {

    }
    [ContextMenu("Generate1")]
    public void GetInfo()
    {
        var bound = tilemap.cellBounds;
        foreach (var pos in bound.allPositionsWithin)
        {
            tile = tilemap.GetTile<Tile>(pos);
            if (tile == null)
            {
                continue;
            }
            Rect textureRect = tile.sprite.textureRect;
            int sx = (int)textureRect.x;
            int sy = (int)textureRect.y;
            int width = (int)textureRect.width;
            int height = (int)textureRect.height;
            Color[] colors = tile.sprite.texture.GetPixels(sx, sy, width, height);
            Debug.Log(colors[10]);
        }
    }
    [ContextMenu("Generate")]
    public void GetTileInfo()
    {
        var bound = tilemap.cellBounds;
        foreach (var pos in bound.allPositionsWithin)
        {
            // var sprite = tilemap.GetSprite(pos);
            tile = tilemap.GetTile<Tile>(pos);

            if (tile == null)
            {
                continue;
            }
            Rect textureRect = tile.sprite.textureRect;
            int sx = (int)textureRect.x;
            int sy = (int)textureRect.y;
            int width = (int)textureRect.width;
            int height = (int)textureRect.height;
            Color[] colors = tile.sprite.texture.GetPixels(sx, sy, width, height);
            // Color[] colors = tile.sprite.texture.GetPixels();
            Debug.Log(colors.Length);
            //按照像素点的顺序，从左到右，从下到上，将colors数组中的颜色值赋值给grid数组
            for (int y = 0; y < 24; y++)
            {
                for (int x = 0; x < 46; x++)
                {
                    // if (colors[x + y * 46] == Color.white)
                    {
                        grid[x, y] = colors[x + y * 46].grayscale;
                    }
                    // else
                    {
                        // continue;
                    }
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
