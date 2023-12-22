using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public enum RoomType
{
    Shop,
    Health,
    Enhancement,
    Combat,
    Boss
}
public class Room : MonoBehaviour
{
    [SerializeField] int shopSpawnFrequency = 3;
    [SerializeField] int healthSpawnFrequency = 3;
    [SerializeField] int bossSpawnFrequency = 6;
    [SerializeField] GameObject spellPrefab;
    [SerializeField] GameObject healthPrefab;
    public GameObject player;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Transform playerSpawn;
    public Transform enemyParent;
    public Door[] door;
    [SerializeField] int roomLevel;
    RoomType roomType;
    [SerializeField] RoomType leftRoomType;
    [SerializeField] RoomType rightRoomType;
    int spawnEnemyCount;
    [SerializeField] int enemyCount;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>().gameObject;
        enemyCount = 1;
        MEventSystem.Instance.Register<EnemyDeath>((EnemyDeath death) =>
        {
            enemyCount--;
        });


    }
    public void Init(RoomType roomType, int enemyCount, int level)
    {
        this.spawnEnemyCount = enemyCount;
        this.enemyCount = enemyCount;
        this.roomType = roomType;
        roomLevel = level;
        player.transform.position = transform.TransformPoint(playerSpawn.position);
        switch (roomType)
        {
            case RoomType.Combat:
            case RoomType.Health:
                for (int i = 0; i < spawnEnemyCount; i++)
                {
                    int index = Random.Range(0, GameManger.Instance.allEnemies.Count);
                    Vector3 randomPosition = GetRandomPosition();
                    Instantiate(GameManger.Instance.allEnemies[index], randomPosition, Quaternion.identity, enemyParent);
                }
                break;
            case RoomType.Boss:
                for (int i = 0; i < spawnEnemyCount; i++)
                {
                    int index = roomLevel / bossSpawnFrequency;
                    Vector3 centerPosition = GetCenterPosition();
                    //TODO：测试
                    Instantiate(GameManger.Instance.allBoss[0], centerPosition, Quaternion.identity, enemyParent);
                }
                break;
        }
    }
    void SetNextRoomType()
    {
        if (roomLevel % bossSpawnFrequency == 0)
        {
            leftRoomType = RoomType.Boss;
            rightRoomType = RoomType.Boss;
        }
        else
        if (roomLevel % shopSpawnFrequency == 0)
        {
            int randomType = Random.Range(0, 2);
            if (randomType == 0)
            {
                leftRoomType = RoomType.Shop;
            }
            else
            {
                // leftRoomType = RoomType.Enhancement;
                leftRoomType = RoomType.Shop;

            }
            rightRoomType = RoomType.Combat;
        }
        else
        {
            leftRoomType = RoomType.Combat;
            rightRoomType = RoomType.Combat;
        }
    }
    private void OnEnable()
    {
        // roomLevel++;
        // player.transform.position = transform.TransformPoint(playerSpawn.position);
        // switch (roomType)
        // {
        //     case RoomType.Combat:
        //     case RoomType.Health:
        //         for (int i = 0; i < spawnEnemyCount; i++)
        //         {
        //             int index = Random.Range(0, GameManger.Instance.allEnemies.Count);
        //             Vector3 randomPosition = GetRandomPosition();
        //             Instantiate(GameManger.Instance.allEnemies[index], randomPosition, Quaternion.identity, enemyParent);
        //         }
        //         break;
        //     case RoomType.Boss:
        //         for (int i = 0; i < spawnEnemyCount; i++)
        //         {
        //             int index = Random.Range(0, GameManger.Instance.allBoss.Count);
        //             Vector3 centerPosition = GetCenterPosition();
        //             Instantiate(GameManger.Instance.allBoss[index], centerPosition, Quaternion.identity, enemyParent);
        //         }
        //         break;
        // }
        // enemyCount = spawnEnemyCount;
    }
    void CheckEnemyDeath()
    {
        if (leftRoomType == RoomType.Boss)
        {
            door[0].Init(leftRoomType, () => this.gameObject.SetActive(false)).gameObject.SetActive(true);
        }
        else
        {
            door[0].Init(leftRoomType, () => this.gameObject.SetActive(false)).gameObject.SetActive(true);
            door[1].Init(rightRoomType, () => this.gameObject.SetActive(false)).gameObject.SetActive(true);
        }
    }
    void DropItem()
    {
        if (roomType == RoomType.Combat)
        {
            //在中心位置随机掉落一个法术
            Instantiate(spellPrefab, GetCenterPosition(), Quaternion.identity);
        }
        else if (roomType == RoomType.Health)
        {
            //在中心位置随机掉落一个法术
            Instantiate(healthPrefab, GetCenterPosition(), Quaternion.identity);
        }
        else if (roomType == RoomType.Boss)
        {
            int index = Random.Range(0, GameManger.Instance.playerWands.Count);
            Instantiate(GameManger.Instance.playerWands[index], GetCenterPosition(), Quaternion.identity);
        }
    }
    private Vector3 GetCenterPosition()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        Vector3Int minPosition = bounds.min;
        Vector3Int maxPosition = bounds.max;
        Vector3Int centerPosition = new Vector3Int((minPosition.x + maxPosition.x) / 2, (minPosition.y + maxPosition.y) / 3, 0);
        return floorTilemap.CellToWorld(centerPosition);
    }
    private Vector3 GetRandomPosition()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        Vector3Int minPosition = bounds.min;
        Vector3Int maxPosition = bounds.max;
        Vector3Int randomPosition = new Vector3Int(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), 0);
        return floorTilemap.CellToWorld(randomPosition);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCount == 0)
        {
            enemyCount = -1;
            DropItem();
            SetNextRoomType();
            CheckEnemyDeath();
        }
    }
}
