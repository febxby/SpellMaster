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
    [SerializeField] GameObject coinPrefab;
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
        MEventSystem.Instance.Register<EnemyDeath>((EnemyDeath e) =>
        {
            if (roomType == RoomType.Boss)
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObjectPool.Instance.GetObject(coinPrefab).
                    SetPositionAndRotation(e.pos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity).
                    SetParent(transform);
                }
            }
            else if (roomType == RoomType.Health)
            {
                GameObjectPool.Instance.GetObject(healthPrefab)
                .SetPositionAndRotation(e.pos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity).
                SetParent(transform);
            }
            else
            {
                GameObjectPool.Instance.GetObject(coinPrefab).
                SetPositionAndRotation(e.pos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity).
                SetParent(transform);
            }

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

                    GameObjectPool.Instance.GetObject(GameManger.Instance.allEnemies[index]).
                    SetPositionAndRotation(randomPosition, Quaternion.identity).
                    SetParent(enemyParent);
                }
                break;
            case RoomType.Boss:
                for (int i = 0; i < spawnEnemyCount; i++)
                {
                    int index = roomLevel / bossSpawnFrequency;
                    Vector3 centerPosition = GetCenterPosition();
                    GameObjectPool.Instance.GetObject(GameManger.Instance.allBoss[index]).
                    SetPositionAndRotation(centerPosition, Quaternion.identity).
                    SetParent(enemyParent);
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
            door[0].Init(leftRoomType, () => GameObjectPool.Instance.PushObject(this.gameObject)).gameObject.SetActive(true);
        }
        else
        {
            door[0].Init(leftRoomType, () => GameObjectPool.Instance.PushObject(this.gameObject)).gameObject.SetActive(true);
            door[1].Init(rightRoomType, () => GameObjectPool.Instance.PushObject(this.gameObject)).gameObject.SetActive(true);
        }
    }
    void DropItem()
    {
        if (roomType == RoomType.Combat)
        {
            //在中心位置随机掉落一个法术
            GameObjectPool.Instance.GetObject(spellPrefab).
            SetPositionAndRotation(GetCenterPosition(), Quaternion.identity).
            SetParent(transform);
        }
        else if (roomType == RoomType.Health)
        {
            //在中心位置随机掉落一个法术
            GameObjectPool.Instance.GetObject(healthPrefab).
            SetPositionAndRotation(GetCenterPosition(), Quaternion.identity).
            SetParent(transform);
        }
        else if (roomType == RoomType.Boss)
        {
            int index = Random.Range(0, GameManger.Instance.playerWands.Count);
            GameObjectPool.Instance.GetObject(GameManger.Instance.playerWands[index]).
            SetPositionAndRotation(GetCenterPosition(), Quaternion.identity).
            SetParent(transform);
        }
    }
    private Vector3 GetCenterPosition()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        Vector3Int minPosition = bounds.min;
        Vector3Int maxPosition = bounds.max;
        Vector3Int centerPosition;
        centerPosition = new Vector3Int(minPosition.x + (maxPosition.x - minPosition.x) / 2, maxPosition.y - (maxPosition.y - minPosition.y) / 3, 0);
        return floorTilemap.CellToWorld(centerPosition);
    }
    private Vector3 GetRandomPosition()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        Vector3Int minPosition = bounds.min;
        Vector3Int maxPosition = bounds.max;
        Vector3Int randomPosition;
        do
        {
            randomPosition = new Vector3Int(
                Random.Range(minPosition.x, maxPosition.x),
                Random.Range(minPosition.y + (maxPosition.y - minPosition.y) / 2, maxPosition.y),
                0
            );
        } while (floorTilemap.GetTile(randomPosition) == null); // 如果这个位置没有瓦片，那么重新生成位置
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
    private void OnDisable()
    {
        // 将当前场景所有DropItem组件的游戏对象都放回对象池
        DropItem[] dropItems = FindObjectsOfType<DropItem>();
        foreach (DropItem dropItem in dropItems)
        {
            GameObjectPool.Instance.PushObject(dropItem.gameObject);
        }
    }
}
