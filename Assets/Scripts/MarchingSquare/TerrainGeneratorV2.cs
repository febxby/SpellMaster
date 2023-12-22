using UnityEngine;

public class TerrainGeneratorV2 : MonoBehaviour
{
    // The size of the terrain in tiles
    public int mouseRadius = 10;
    public int width = 10;
    public int height = 10;

    // The size of each tile in pixels
    public int tileSize = 16;

    // The noise scale for generating the terrain shape
    public float noiseScale = 0.1f;

    // The threshold for determining if a pixel is solid or empty
    public float solidThreshold = 0.5f;

    // The prefab for the terrain tile
    public GameObject terrainTilePrefab;

    // The 2D array of terrain tiles
    private GameObject[,] tiles;

    // The 2D array of terrain data (0 for empty, 1 for solid)
    private int[,] data;

    // The 2D array of edge data (0 for no edge, 1 for edge)
    private int[,] edges;

    // The 2D array of corner data (0 for no corner, 1 for corner)
    private int[,] corners;

    // The 2D array of smoothed data (0 for empty, 1 for solid)
    private int[,] smoothed;

    // The texture for the terrain tile
    private Texture2D texture;

    // The color for the solid pixels
    public Color solidColor = Color.white;

    // The color for the empty pixels
    public Color emptyColor = Color.clear;

    // The color for the edge pixels
    public Color edgeColor = Color.black;

    // The color for the corner pixels
    public Color cornerColor = Color.red;

    // The color for the smoothed pixels
    public Color smoothedColor = Color.green;

    // The flag for showing the edge data
    public bool showEdges = false;

    // The flag for showing the corner data
    public bool showCorners = false;

    // The flag for showing the smoothed data
    public bool showSmoothed = false;

    // The flag for smoothing the terrain edges
    public bool smoothEdges = true;

    // The flag for smoothing the terrain corners
    public bool smoothCorners = true;

    // The flag for updating the terrain
    private bool updateTerrain = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the arrays
        tiles = new GameObject[width, height];
        data = new int[width * tileSize, height * tileSize];
        edges = new int[width * tileSize, height * tileSize];
        corners = new int[width * tileSize, height * tileSize];
        smoothed = new int[width * tileSize, height * tileSize];

        // Create the texture
        texture = new Texture2D(width * tileSize, height * tileSize);
        texture.filterMode = FilterMode.Point;

        // Generate the terrain data
        GenerateTerrainData();

        // Generate the terrain tiles
        GenerateTerrainTiles();

        // Update the terrain
        updateTerrain = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the terrain needs to be updated
        if (updateTerrain)
        {
            // Update the texture
            UpdateTexture();

            // Update the tiles
            UpdateTiles();

            // Reset the flag
            updateTerrain = false;
        }
    }
    // The radius of the mouse click in pixels

    // Called when the mouse button is pressed over the collider
    void OnMouseDown()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Convert the mouse position to data coordinates
        int mouseX = Mathf.RoundToInt(mousePosition.x);
        int mouseY = Mathf.RoundToInt(mousePosition.y);

        // Loop through the data array within the mouse radius
        for (int x = mouseX - mouseRadius; x <= mouseX + mouseRadius; x++)
        {
            for (int y = mouseY - mouseRadius; y <= mouseY + mouseRadius; y++)
            {
                // Check if the current position is within the data array bounds
                if (x >= 0 && x < width * tileSize && y >= 0 && y < height * tileSize)
                {
                    // Set the data value to zero (empty)
                    data[x, y] = 0;
                }
            }
        }

        // Set the update terrain flag to true
        updateTerrain = true;
    }

    // Generate the terrain data using Perlin noise
    void GenerateTerrainData()
    {
        // Loop through the data array
        for (int x = 0; x < width * tileSize; x++)
        {
            for (int y = 0; y < height * tileSize; y++)
            {
                // Get the noise value at the current position
                float noise = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);

                // Set the data value based on the threshold
                if (noise > solidThreshold)
                {
                    data[x, y] = 1;
                }
                else
                {
                    data[x, y] = 0;
                }
            }
        }
    }

    // Generate the terrain tiles from the data array
    void GenerateTerrainTiles()
    {
        // Loop through the tiles array
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Instantiate a new tile
                GameObject tile = Instantiate(terrainTilePrefab, transform);

                // Set the tile position
                tile.transform.position = new Vector3(i * tileSize, j * tileSize, 0);

                // Set the tile name
                tile.name = "Tile_" + i + "_" + j;

                // Add the tile to the array
                tiles[i, j] = tile;
            }
        }
    }

    // Update the texture from the data array
    void UpdateTexture()
    {
        // Loop through the data array
        for (int x = 0; x < width * tileSize; x++)
        {
            for (int y = 0; y < height * tileSize; y++)
            {
                // Get the data value at the current position
                int value = data[x, y];

                // Set the color based on the value
                Color color = emptyColor;
                if (value == 1)
                {
                    color = solidColor;
                }
                // Check if the show edges flag is true
                if (showEdges)
                {
                    // Get the edge value at the current position
                    int edge = edges[x, y];

                    // Set the color based on the edge value
                    if (edge == 1)
                    {
                        color = edgeColor;
                    }
                }
                // Check if the show corners flag is true
                if (showCorners)
                {
                    // Get the corner value at the current position
                    int corner = corners[x, y];

                    // Set the color based on the corner value
                    if (corner == 1)
                    {
                        color = cornerColor;
                    }
                }
                // Check if the show smoothed flag is true
                if (showSmoothed)
                {
                    // Get the smoothed value at the current position
                    int smooth = smoothed[x, y];

                    // Set the color based on the smoothed value
                    if (smooth == 1)
                    {
                        color = smoothedColor;
                    }
                }


                // Set the pixel color
                texture.SetPixel(x, y, color);
            }
        }

        // Apply the texture changes
        texture.Apply();
    }

    // Update the tiles from the texture
    void UpdateTiles()
    {
        // Loop through the tiles array
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Get the tile at the current position
                GameObject tile = tiles[i, j];

                // Get the sprite renderer component
                SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();

                // Get the polygon collider component
                PolygonCollider2D polygonCollider = tile.GetComponent<PolygonCollider2D>();

                // Create a new sprite from the texture
                Sprite sprite = Sprite.Create(texture, new Rect(i * tileSize, j * tileSize, tileSize, tileSize), new Vector2(0.5f, 0.5f), tileSize);

                // Set the sprite to the sprite renderer
                spriteRenderer.sprite = sprite;

                // Set the sprite to the polygon collider
                // Set the polygon collider from the sprite
                DestroyImmediate(polygonCollider);
                tile.AddComponent<PolygonCollider2D>();

            }
        }
    }

    // Detect the edges of the terrain from the data array
    void DetectEdges()
    {
        // Loop through the data array
        for (int x = 0; x < width * tileSize; x++)
        {
            for (int y = 0; y < height * tileSize; y++)
            {
                // Get the data value at the current position
                int value = data[x, y];

                // Set the edge value to zero by default
                edges[x, y] = 0;

                // Check if the value is solid
                if (value == 1)
                {
                    // Check the four neighbors of the current position
                    int left = x > 0 ? data[x - 1, y] : 0;
                    int right = x < width * tileSize - 1 ? data[x + 1, y] : 0;
                    int up = y < height * tileSize - 1 ? data[x, y + 1] : 0;
                    int down = y > 0 ? data[x, y - 1] : 0;

                    // If any of the neighbors is empty, then the current position is an edge
                    if (left == 0 || right == 0 || up == 0 || down == 0)
                    {
                        edges[x, y] = 1;
                    }
                }
            }
        }
    }

    // Detect the corners of the terrain from the edge array
    void DetectCorners()
    {
        // Loop through the edge array
        for (int x = 0; x < width * tileSize; x++)
        {
            for (int y = 0; y < height * tileSize; y++)
            {
                // Get the edge value at the current position
                int value = edges[x, y];

                // Set the corner value to zero by default
                corners[x, y] = 0;

                // Check if the value is an edge
                if (value == 1)
                {
                    // Check the four diagonal neighbors of the current position
                    int topLeft = x > 0 && y < height * tileSize - 1 ? edges[x - 1, y + 1] : 0;
                    int topRight = x < width * tileSize - 1 && y < height * tileSize - 1 ? edges[x + 1, y + 1] : 0;
                    int bottomLeft = x > 0 && y > 0 ? edges[x - 1, y - 1] : 0;
                    int bottomRight = x < width * tileSize - 1 && y > 0 ? edges[x + 1, y - 1] : 0;

                    // If any of the diagonal neighbors is an edge, then the current position is a corner
                    if (topLeft == 1 || topRight == 1 || bottomLeft == 1 || bottomRight == 1)
                    {
                        corners[x, y] = 1;
                    }
                }
            }
        }
    }

    // Smooth the edges of the terrain by interpolating the data array
    void SmoothEdges()
    {
        // Loop through the edge array
        for (int x = 0; x < width * tileSize; x++)
        {
            for (int y = 0; y < height * tileSize; y++)
            {
                // Get the edge value at the current position
                int value = edges[x, y];

                // Check if the value is an edge
                if (value == 1)
                {
                    // Get the four neighbors of the current position
                    int left = x > 0 ? data[x - 1, y] : 0;
                    int right = x < width * tileSize - 1 ? data[x + 1, y] : 0;
                    int up = y < height * tileSize - 1 ? data[x, y + 1] : 0;
                    int down = y > 0 ? data[x, y - 1] : 0;

                    // Calculate the average of the neighbors
                    float average = (left + right + up + down) / 4f;

                    // Set the smoothed value to the average
                    smoothed[x, y] = Mathf.RoundToInt(average);
                }
                else
                {
                    // Set the smoothed value to the original data value
                    smoothed[x, y] = data[x, y];
                }
            }
        }
    }

    // Smooth the corners of the terrain by interpolating the edge array
    void SmoothCorners()
    {
        // Loop through the corner array
        for (int x = 0; x < width * tileSize; x++)
        {
            for (int y = 0; y < height * tileSize; y++)
            {
                // Get the corner value at the current position
                int value = corners[x, y];

                // Check if the value is a corner
                if (value == 1)
                {
                    // Get the four diagonal neighbors of the current position
                    int topLeft = x > 0 && y < height * tileSize - 1 ? edges[x - 1, y + 1] : 0;
                    int topRight = x < width * tileSize - 1 && y < height * tileSize - 1 ? edges[x + 1, y + 1] : 0;
                    int bottomLeft = x > 0 && y > 0 ? edges[x - 1, y - 1] : 0;
                    int bottomRight = x < width * tileSize - 1 && y > 0 ? edges[x + 1, y - 1] : 0;

                    // Calculate the average of the diagonal neighbors
                    float average = (topLeft + topRight + bottomLeft + bottomRight) / 4f;

                    // Set the smoothed value to the average
                    smoothed[x, y] = Mathf.RoundToInt(average);
                }
            }
        }
    }
}