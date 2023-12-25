using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public enum Grid
    {
        Empty,
        Floor,
        Wall,
    }
    // Start is called before the first frame update
    public Grid[,] grid;
    public int width;
    public int height;
    public Tilemap floorTileMap;
    public Tilemap wallTileMap;
    public Tile floorTile;
    public Tile wallTile;
    public RuleTile floorRuleTile;
    public RuleTile wallRuleTile;
    public float removePercentage;
    public float continuousPercentage;

    int xLength => grid.GetLength(0) - 2;
    int yLength => grid.GetLength(1) - 2;

    void Start()
    {
        InitializeGrid();
    }

    [ContextMenu("InitializeGrid")]
    public void InitializeGrid()
    {
        grid = new Grid[width + 4, height + 4];
        Debug.Log(grid.GetLength(0));
        Debug.Log(grid.GetLength(1));
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (x == 0 || x == grid.GetLength(0) - 1 || y == 0 || y == grid.GetLength(1) - 1 ||
                    x == 1 || x == grid.GetLength(0) - 2 || y == 1 || y == grid.GetLength(1) - 2)
                {
                    grid[x, y] = Grid.Empty;
                }
                else
                {
                    grid[x, y] = Grid.Floor;
                }
            }
        }

        RandomizeGridEdges();
        GenerateWall();
        DrawGrid();
    }
    public void GenerateWall()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == Grid.Floor)
                {

                    if (grid[x + 1, y] == Grid.Empty)
                    {
                        grid[x + 1, y] = Grid.Wall;
                    }
                    if (grid[x - 1, y] == Grid.Empty)
                    {
                        grid[x - 1, y] = Grid.Wall;
                    }
                    if (grid[x, y + 1] == Grid.Empty)
                    {
                        grid[x, y + 1] = Grid.Wall;
                    }
                    if (grid[x, y - 1] == Grid.Empty)
                    {
                        grid[x, y - 1] = Grid.Wall;
                    }
                    if (grid[x + 1, y + 1] == Grid.Empty)
                    {
                        grid[x + 1, y + 1] = Grid.Wall;
                    }
                    if (grid[x - 1, y - 1] == Grid.Empty)
                    {
                        grid[x - 1, y - 1] = Grid.Wall;
                    }
                    if (grid[x + 1, y - 1] == Grid.Empty)
                    {
                        grid[x + 1, y - 1] = Grid.Wall;
                    }
                    if (grid[x - 1, y + 1] == Grid.Empty)
                    {
                        grid[x - 1, y + 1] = Grid.Wall;
                    }
                }
            }
        }

    }
    public void DrawGrid()
    {
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                switch (grid[x, y])
                {
                    case Grid.Floor:
                        if (floorTile == null)
                            floorTileMap.SetTile(new Vector3Int(x, y, 0), floorRuleTile);
                        else
                            floorTileMap.SetTile(new Vector3Int(x, y, 0), floorTile);
                        break;
                    case Grid.Wall:
                        if (wallTile == null)
                            wallTileMap.SetTile(new Vector3Int(x, y, 0), wallRuleTile);
                        else
                            wallTileMap.SetTile(new Vector3Int(x, y, 0), wallTile);
                        break;
                    case Grid.Empty:
                        break;
                }
            }
        }
    }
    private void RandomizeGridEdges()
    {
        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (x == 0 || x == xLength - 1 || y == 0 || y == yLength - 1)
                {
                    if (Random.value < removePercentage)
                    {
                        grid[x, y] = Grid.Empty;
                    }
                }
            }
        }
    }

    private void RandomizeContinuousGrid()
    {
        for (int x = 1; x < xLength - 1; x++)
        {
            for (int y = 1; y < yLength - 1; y++)
            {
                if (grid[x, y] == Grid.Floor && Random.value < continuousPercentage)
                {
                    grid[x, y] = Grid.Empty;
                }
            }
        }
    }
    void Update()
    {

    }
}
