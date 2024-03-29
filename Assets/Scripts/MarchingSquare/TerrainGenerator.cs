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
    float baseTerrainHeight = 1 + 0.5f;
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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger" + other.name);

    }
    public float GetVoxelVolumeForSphere(Vector2Int voxelPosition, Vector2Int sphereOrigin, float sphereRadius)
    {
        // Vector3 vector3 = new Vector3(voxelPosition.x, voxelPosition.y, 0);
        float t = (voxelPosition - sphereOrigin).sqrMagnitude / (sphereRadius * sphereRadius);
        float value = Mathf.Lerp(1f, 0f, t);
        return Mathf.Clamp01(value);
    }
    public void LerpData(Vector2Int worldPos, float to, float time)
    {
        // Vector2Int currentGridPosition = GetGridPositionFromWorldPosition(worldPos);
        // NimbatusTerrainData? data = GetData(worldPos);
        // if (data.HasValue)
        float volume = grid[worldPos.x, worldPos.y];
        float value = Mathf.Lerp(volume, to, time);
        grid[worldPos.x, worldPos.y] = value;
        // }
    }
    private void ColliderCallback(Vector3 worldPosition, int radius)
    {
        // float time = 3;
        worldPosition.z = 0;
        worldPosition = transform.InverseTransformPoint(worldPosition);
        Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        bool canGenerate = false;
        // for (int y = gridPosition.y - radius; y <= gridPosition.y + radius; y++)
        // {
        //     for (int x = gridPosition.x - radius; x <= gridPosition.x + radius; x++)
        //     {
        //         Vector2Int currentGridPosition = new(x, y);
        //         if (!isValidGridPosition(currentGridPosition))
        //         {
        //             Debug.Log("Out of range");
        //             continue;
        //         }
        //         float distance = Vector2.Distance(gridPosition, currentGridPosition);
        //         float factor = brushStrength * Mathf.Exp(-distance * brushFallback / radius);
        //         grid[currentGridPosition.x, currentGridPosition.y] -= factor;
        //         canGenerate = true;
        //     }
        // }
        if (canGenerate)
            GenerateMesh();
    }
    // private void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log("collision" + collision.gameObject.name);
    //     TouchingCallback(collision.GetContact(0).point, brushStrength);
    // }

    public void TouchingCallback(Vector3 worldPosition, int radius, float strength)
    {
        worldPosition.z = 0;
        worldPosition = transform.InverseTransformPoint(worldPosition);
        Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        bool canGenerate = false;
        // for (int y = gridPosition.y; y <= gridPosition.y + radius; y++)
        {
            for (int x = gridPosition.x; x <= gridPosition.x + radius; x++)
            {
                Vector2Int currentGridPosition = new(x, gridPosition.y);
                if (!isValidGridPosition(currentGridPosition))
                {
                    // Debug.Log("Out of range");
                    continue;
                }
                // gridPosition = GetGridPositionFromWorldPosition(vector);
                // Vector2Int currentGridPosition = new(gridPosition.x + i, gridPosition.y + j);
                float volume = grid[currentGridPosition.x, currentGridPosition.y];
                float num = 1f - GetVoxelVolumeForSphere(currentGridPosition, gridPosition, (float)radius * 1.2f);

                float num2 = strength;
                num2 = strength / 10f;
                if (volume >= num)
                {
                    // if (num2 > 0f)
                    {
                        LerpData(currentGridPosition, Mathf.Clamp01(num), num2);
                    }
                    // else
                    {
                        // grid[currentGridPosition.x, currentGridPosition.y] = Mathf.Clamp01(num);

                    }
                    canGenerate = true;

                }

            }
        }
        // worldPosition.z = 0;
        // worldPosition = transform.InverseTransformPoint(worldPosition);
        // Vector2Int gridPosition = GetGridPositionFromWorldPosition(worldPosition);

        // bool canGenerate = false;
        // for (int y = gridPosition.y - brushRadius; y <= gridPosition.y + brushRadius; y++)
        // {
        //     for (int x = gridPosition.x - brushRadius; x <= gridPosition.x + brushRadius; x++)
        //     {
        //         Vector2Int currentGridPosition = new(x, y);
        //         if (!isValidGridPosition(currentGridPosition))
        //         {
        //             // Debug.Log("Out of range");
        //             continue;
        //         }
        //         float distance = Vector2.Distance(gridPosition, currentGridPosition);
        //         float factor = brushStrength * Mathf.Exp(-distance * brushFallback / brushRadius);
        //         grid[currentGridPosition.x, currentGridPosition.y] -= factor;
        //         canGenerate = true;
        //     }
        // }
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
