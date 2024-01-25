using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;
[Serializable]
public class RoomData
{
    public RoomType roomType;
    public RoomType leftRoomType;
    public RoomType rightRoomType;
    public int enemyCount;
    public int level;
    public List<PropData> propDatas;
    public List<GameObject> dropItems;
}
[Serializable]
public struct PropData
{
    public bool isActive;
    public Spell spell;
}
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
    [SerializeField] GameObject itemPrefab;
    public GameObject player;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Transform playerSpawn;
    public Transform enemyParent;
    public Transform dropItemParent;
    public Door[] door;
    [SerializeField] int roomLevel;
    [SerializeField] RoomType roomType;
    [SerializeField] RoomType leftRoomType;
    [SerializeField] RoomType rightRoomType;
    int spawnEnemyCount;
    [SerializeField] int enemyCount;
    [SerializeField] Transform spellParent;
    List<PropData> propDatas = new List<PropData>();
    List<GameObject> dropItems = new List<GameObject>();
    public bool isEndRoom;
    public GameObject doorLeft, doorRight, doorUp, doorDown;
    public bool roomLeft, roomRight, roomUp, roomDown;
    public int stepToStart;

    public int doorNumber;
    public Text text;
    public bool isStartRoom;
    bool isClosed;
    void Start()
    {
        // doorLeft.SetActive(roomLeft);
        // doorRight.SetActive(roomRight);
        // doorUp.SetActive(roomUp);
        // doorDown.SetActive(roomDown);
        OpenDoor();
    }

    public void UpdateRoom(float xOffset, float yOffset)
    {
        stepToStart = (int)(Mathf.Abs(transform.position.x / xOffset) + Mathf.Abs(transform.position.y / yOffset));
        text.text = stepToStart.ToString();

        if (roomUp)
            doorNumber++;
        if (roomDown)
            doorNumber++;
        if (roomLeft)
            doorNumber++;
        if (roomRight)
            doorNumber++;
    }
    public void CloseDoor()
    {
        isClosed = true;
        doorLeft.SetActive(roomLeft);
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
    }
    public void OpenDoor()
    {
        doorLeft.SetActive(false);
        doorRight.SetActive(false);
        doorUp.SetActive(false);
        doorDown.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (!isClosed)
                CloseDoor();
            foreach (Transform child in enemyParent)
            {
                StartCoroutine(child.GetComponent<EnemyController>().ActiveEnemy());
            }
            CameraController.instance.ChangeTarget(transform);
        }
    }
    private void Awake()
    {
        MEventSystem.Instance.Register<PlayerDeath>((e) =>
        {
            foreach (Transform child in enemyParent)
            {
                Destroy(child.gameObject);
            }
            DisableCallBack();
        }).UnRegisterWhenGameObjectDestroy(gameObject);
        MEventSystem.Instance.Register<SaveData>((e) =>
        {
            if (gameObject.activeSelf)
            {
                if (spellParent != null)
                    for (int i = 0; i < spellParent.childCount; i++)
                    {
                        Transform child = spellParent.GetChild(i);
                        this.propDatas.Add(new PropData()
                        {
                            isActive = child.gameObject.activeSelf,
                            spell = child.GetComponent<DropItem>().spell
                        });
                    };
                if (dropItemParent != null)
                {
                    for (int i = 0; i < dropItemParent.childCount; i++)
                    {
                        dropItems.Add(dropItemParent.GetChild(i).gameObject);
                    }
                }
                SaveSystem.SaveByJson("Room", new RoomData()
                {
                    roomType = roomType,
                    leftRoomType = this.leftRoomType,
                    rightRoomType = this.rightRoomType,
                    enemyCount = this.enemyCount,
                    level = this.roomLevel,
                    propDatas = this.propDatas,
                    dropItems = this.dropItems
                });
            }

        }).UnRegisterWhenGameObjectDestroy(gameObject);
        player = FindFirstObjectByType<PlayerController>().gameObject;
        enemyCount = 1;
        MEventSystem.Instance.Register<EnemyDeath>((EnemyDeath e) =>
        {
            //BUG:敌人死亡消息会被多次接收
            if (gameObject.activeSelf && e.room == this)
            {
                enemyCount--;
                if (roomType == RoomType.Boss)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        GameObjectPool.Instance.GetObject(coinPrefab, true).
                        SetPositionAndRotation(e.pos + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0), Quaternion.identity);
                    }
                }
                else if (roomType == RoomType.Health)
                {
                    GameObjectPool.Instance.GetObject(coinPrefab, true)
                    .SetPositionAndRotation(e.pos + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0), Quaternion.identity);
                }
                else
                {
                    GameObjectPool.Instance.GetObject(coinPrefab, true).
                    SetPositionAndRotation(e.pos + new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0), Quaternion.identity);
                }
            }
        }).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    public void LoadData()
    {
        RoomData room = SaveSystem.LoadFromJson<RoomData>("Room");
        if (room != null)
        {
            roomType = room.roomType;
            leftRoomType = room.leftRoomType;
            rightRoomType = room.rightRoomType;
            enemyCount = room.enemyCount;
            roomLevel = room.level;
            propDatas = room.propDatas;
            for (int i = 0; i < spellParent.childCount; i++)
            {
                spellParent.GetChild(i).gameObject.SetActive(propDatas[i].isActive);
                spellParent.GetChild(i).GetComponent<DropItem>().Init(propDatas[i].spell);
            }

        }
    }
    public void Init(RoomType roomType, int enemyCount, int level, bool isEndRoom = false, bool isStartRoom = false, List<PropData> propDatas = null)
    {
        this.isEndRoom = isEndRoom;
        this.isStartRoom = isStartRoom;
        this.spawnEnemyCount = enemyCount;
        this.enemyCount = enemyCount;
        this.roomType = roomType;
        roomLevel = level;
        if (isStartRoom)
        {
            player = FindFirstObjectByType<PlayerController>().gameObject;
            player.transform.position = transform.TransformPoint(playerSpawn.position);
        }
        switch (roomType)
        {
            case RoomType.Shop:
                if (propDatas != null)
                    for (int i = 0; i < spellParent.childCount; i++)
                    {
                        spellParent.GetChild(i).gameObject.SetActive(propDatas[i].isActive);
                        spellParent.GetChild(i).GetComponent<DropItem>().Init(propDatas[i].spell);
                    }
                else
                {
                    // for (int i = 0; i < spellParent.childCount; i++)
                    {
                        spellParent.gameObject.SetActive(true);
                    }
                }
                break;
            case RoomType.Combat:
            case RoomType.Health:
                spellParent.gameObject.SetActive(false);
                for (int i = 0; i < spawnEnemyCount; i++)
                {
                    int index = UnityEngine.Random.Range(0, GameManger.Instance.allEnemies.Count);
                    Vector3 randomPosition = GetRandomPosition();

                    GameObjectPool.Instance.GetObject(GameManger.Instance.allEnemies[index]).
                    SetPositionAndRotation(randomPosition, Quaternion.identity).
                    SetParent(enemyParent);
                }
                break;
            case RoomType.Boss:
                spellParent.gameObject.SetActive(false);
                for (int i = 0; i < spawnEnemyCount; i++)
                {
                    int index = UnityEngine.Random.Range(0, GameManger.Instance.allBoss.Count);
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
            int randomType = UnityEngine.Random.Range(0, 2);
            if (randomType == 0)
            {
                leftRoomType = RoomType.Shop;
            }
            else
            {
                leftRoomType = RoomType.Shop;
            }
            rightRoomType = RoomType.Combat;
        }
        else
        if (roomLevel % healthSpawnFrequency == 0)
        {
            leftRoomType = RoomType.Health;
            rightRoomType = RoomType.Combat;
        }
        else
        {
            leftRoomType = RoomType.Combat;
            rightRoomType = RoomType.Combat;
        }
    }
    private void Update()
    {
        if (enemyCount == 0)
        {
            OpenDoor();
            enemyCount = -1;
            if (!isStartRoom)
                DropItem();
            if (isEndRoom)
            {
                StartCoroutine(GameObjectPool.Instance.RecycleAllCoroutine());
                SetNextRoomType();
                CheckEnemyDeath();
            }
        }
        else if (enemyCount == -1)
        {
            OpenDoor();
            if (isEndRoom)
                CheckEnemyDeath();

        }
    }
    private void OnEnable()
    {
        transform.SetParent(null);
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
            door[0].Init(leftRoomType, () => DisableCallBack()).gameObject.SetActive(true);
        }
        else
        {
            door[0].Init(leftRoomType, () => DisableCallBack()).gameObject.SetActive(true);
            door[1].Init(rightRoomType, () => DisableCallBack()).gameObject.SetActive(true);
        }
    }
    void DisableCallBack()
    {
        if (this.gameObject.activeSelf)
            StartCoroutine(nameof(Recycle));
    }
    void DropItem()
    {

        if (roomType == RoomType.Combat)
        {
            //在中心位置随机掉落一个法术
            int index = UnityEngine.Random.Range(0, GameManger.Instance.spellCount);
            GameObjectPool.Instance.GetObject(spellPrefab).
            SetPositionAndRotation(GetCenterPosition(), Quaternion.identity)
            .GetComponent<DropItem>().Init(GameManger.Instance.Get<Spell>(index));
        }
        else if (roomType == RoomType.Health)
        {
            //在中心位置随机掉落一个法术
            GameObjectPool.Instance.GetObject(healthPrefab, true).
            SetPositionAndRotation(GetCenterPosition(), Quaternion.identity);
            // SetParent(dropItemParent);
        }
        else if (roomType == RoomType.Boss)
        {
            int index = UnityEngine.Random.Range(0, GameManger.Instance.playerWands.Count);
            GameObjectPool.Instance.GetObject(GameManger.Instance.playerWands[index]).
            SetPositionAndRotation(GetCenterPosition(), Quaternion.identity).
            SetParent(dropItemParent);
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
        Vector3Int minPosition = bounds.min + Vector3Int.one * 2;
        Vector3Int maxPosition = bounds.max - Vector3Int.one * 2;
        Vector3Int randomPosition;
        do
        {
            randomPosition = new Vector3Int(
                UnityEngine.Random.Range(minPosition.x, maxPosition.x),
                // UnityEngine.Random.Range(minPosition.y + (maxPosition.y - minPosition.y) / 2, maxPosition.y),
                UnityEngine.Random.Range(minPosition.y, maxPosition.y),
                0
            );
        } while (floorTilemap.GetTile(randomPosition) == null); // 如果这个位置没有瓦片，那么重新生成位置
        return floorTilemap.CellToWorld(randomPosition);
    }
    private void OnDisable()
    {
        // 将当前场景所有DropItem组件的游戏对象都放回对象池
        door[0].gameObject.SetActive(false);
        door[1].gameObject.SetActive(false);
        // DropItem[] dropItems = FindObjectsOfType<DropItem>();
        // foreach (DropItem dropItem in dropItems)
        // {
        //     GameObjectPool.Instance.PushObject(dropItem.gameObject);
        // }
        // GameManger.Instance.ExitLoading();
        Time.timeScale = 1;

        //TODO:房间关闭后将所有对象放入对象池

        // await Recycle();
        //TODO：统一管理掉落物品
    }
    IEnumerator Recycle()
    {
        Time.timeScale = 0;
        //遍历dropItemParent的子物体
        foreach (Transform child in dropItemParent)
        {
            GameObjectPool.Instance.PushObject(child.gameObject);
        }
        // for (int i = 0; i < dropItemParent.childCount; i++)
        // {
        //     GameObjectPool.Instance.PushObject(dropItemParent.GetChild(i).gameObject);
        // }
        // GameManger.Instance.EnterLoading();
        yield return StartCoroutine(GameObjectPool.Instance.RecycleAllCoroutine());
        // GameManger.Instance.ExitLoading();
        Time.timeScale = 1;
        GameObjectPool.Instance.PushObject(this.gameObject);
    }
    // async Task Recycle()
    // {
    //     Time.timeScale = 0;
    //     await Task.Run(() => GameObjectPool.Instance.RecycleAll());
    //     Time.timeScale = 1;
    // }
}
