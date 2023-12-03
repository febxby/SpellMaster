using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] MeshFilter filter;
    [Header("Brush Settings")]
    [SerializeField] int brushRadius;
    [SerializeField] float brushStrength;
    [SerializeField] float brushFallback;
    [Header("Data")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float gridScale;
    [SerializeField] float isoValue;
    private SquareGrid squareGrid;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    float[,] grid;
    Mesh mesh;
    public bool canTouch;
    public TileMapTest tileMapTest;
    private void Awake()
    {
        if (canTouch)
            InputManger.OnTouching += TouchingCallback;
        // Projectile.OnCollider += ColliderCallback;
    }



    void Start()
    {
        // Application.targetFrameRate = 60;
        // grid = tileMapTest.grid;

        grid = new float[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = isoValue + 0.1f;
                // grid[x, y] = UnityEngine.Random.Range(0, 1f);
            }
        }
        squareGrid = new SquareGrid(width - 1, height - 1, gridScale, isoValue);
        // squareGrid = new SquareGrid(grid.GetLength(0) - 1, grid.GetLength(1) - 1, gridScale, isoValue);

        GenerateMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("collider" + other.gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger" + other.name);

    }
    private void ColliderCallback(Vector3 worldPosition, int radius)
    {
        worldPosition.z = 0;
        worldPosition = transform.InverseTransformPoint(worldPosition);
        Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        bool canGenerate = false;
        for (int y = gridPosition.y - radius; y <= gridPosition.y + radius; y++)
        {
            for (int x = gridPosition.x - radius; x <= gridPosition.x + radius; x++)
            {
                Vector2Int currentGridPosition = new(x, y);
                if (!isValidGridPosition(currentGridPosition))
                {
                    Debug.Log("Out of range");
                    continue;
                }
                float distance = Vector2.Distance(gridPosition, currentGridPosition);
                float factor = brushStrength * Mathf.Exp(-distance * brushFallback / radius);
                grid[currentGridPosition.x, currentGridPosition.y] -= factor;
                canGenerate = true;
            }
        }
        if (canGenerate)
            GenerateMesh();
    }
    public void TouchingCallback(Vector3 worldPosition)
    {
        worldPosition.z = 0;
        worldPosition = transform.InverseTransformPoint(worldPosition);
        Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        bool canGenerate = false;
        for (int y = gridPosition.y - brushRadius; y <= gridPosition.y + brushRadius; y++)
        {
            for (int x = gridPosition.x - brushRadius; x <= gridPosition.x + brushRadius; x++)
            {
                Vector2Int currentGridPosition = new(x, y);
                if (!isValidGridPosition(currentGridPosition))
                {
                    // Debug.Log("Out of range");
                    continue;
                }
                float distance = Vector2.Distance(gridPosition, currentGridPosition);
                float factor = brushStrength * Mathf.Exp(-distance * brushFallback / brushRadius);
                grid[currentGridPosition.x, currentGridPosition.y] -= factor;
                canGenerate = true;
            }
        }
        if (canGenerate)
            GenerateMesh();
    }
    private void GenerateMesh()
    {
        mesh = new Mesh();
        vertices.Clear();
        triangles.Clear();
        squareGrid.Update(grid);
        mesh.vertices = squareGrid.GetVertices();
        mesh.triangles = squareGrid.GetTriangles();

        filter.mesh = mesh;
        GenerateCollider();
    }

    private void GenerateCollider()
    {
        if (filter.TryGetComponent(out MeshCollider meshCollider))
        {
            meshCollider.sharedMesh = mesh;
        }
        else
        {
            filter.gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
        }
    }
    private bool isValidGridPosition(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height;
    }
    private Vector2Int GetGridPositionFromWorldPosition(Vector2 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int
        {
            x = Mathf.FloorToInt(worldPosition.x / gridScale + width / 2 - gridScale / 2),
            y = Mathf.FloorToInt(worldPosition.y / gridScale + height / 2 - gridScale / 2)
        };
        return gridPosition;

    }
    private Vector2 GetWorldPositionFromGridPosition(int x, int y)
    {
        Vector2 worldPosition = new Vector2(x, y) * gridScale;
        worldPosition.x -= (width - 1) * gridScale / 2;
        worldPosition.y -= (height - 1) * gridScale / 2;
        return worldPosition;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying)
            return;
        Gizmos.color = Color.red;
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                Vector2 worldPosition = GetWorldPositionFromGridPosition(x, y);
                Gizmos.DrawSphere(worldPosition, gridScale / 4);
                Handles.Label(worldPosition + Vector2.up * gridScale / 2, grid[x, y].ToString());
            }
        }
    }
#endif
}
