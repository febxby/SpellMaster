using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public struct EnemyDeath
{
    public Vector3 pos;
    public Room room;
};
public struct ChangeRoom
{
    public RoomType roomType;
    public int roomLevel;
};
public class LevelController : MonoBehaviour
{
    // Start is called before the first frame update
    RoomGenerator roomGenerator;
    public GameObject shopPrefab;
    public GameObject enhancementShopPrefab;
    public GameObject roomPrefab;
    public GameObject bossRoomPrefab;
    public Door[] door;
    [SerializeField] int shopSpawnFrequency = 3;
    [SerializeField] int healthSpawnFrequency = 5;
    [SerializeField] int bossSpawnFrequency = 6;
    [SerializeField] int Level;
    [SerializeField] int enemyIncreaseInterval = 6;
    [SerializeField] int initialEnemiesCount = 4;
    [SerializeField] int additionalEnemies = 2;
    List<GameObject> enemies = new();
    List<GameObject> bosses = new();
    Room room;
    private void Awake()
    {
        MEventSystem.Instance.Register<ChangeRoom>(e =>
        {
            Level++;
            switch (e.roomType)
            {
                case RoomType.Shop:
                    room = GameObjectPool.Instance.GetObject(shopPrefab).GetComponent<Room>();
                    break;
                case RoomType.Health:
                    room = GameObjectPool.Instance.GetObject(roomPrefab).GetComponent<Room>();
                    break;
                case RoomType.Combat:
                    room = GameObjectPool.Instance.GetObject(roomPrefab).GetComponent<Room>();
                    break;
                case RoomType.Boss:
                    room = GameObjectPool.Instance.GetObject(bossRoomPrefab).GetComponent<Room>();
                    break;
            }
            if (e.roomType == RoomType.Shop || e.roomType == RoomType.Enhancement)
                room.Init(e.roomType, 0, Level);
            else if (e.roomType == RoomType.Boss)
                room.Init(e.roomType, 1, Level);
            else
                room.Init(e.roomType, Level / enemyIncreaseInterval * additionalEnemies + initialEnemiesCount, Level);
        }).UnRegisterWhenGameObjectDestroy(gameObject);
        // Init(false);
        // roomGenerator.Init();
    }
    public void Init(bool isLoad)
    {
        if (!isLoad)
        {
            enemies = GameManger.Instance.allEnemies;
            bosses = GameManger.Instance.allBoss;
            room = GameObjectPool.Instance.GetObject(roomPrefab).GetComponent<Room>();
            room.Init(RoomType.Combat, 4, 1);
        }
        else
        {
            RoomData roomData = SaveSystem.LoadFromJson<RoomData>("Room");
            if (roomData != null)
            {
                Level = roomData.level;
                switch (roomData.roomType)
                {
                    case RoomType.Shop:
                        room = GameObjectPool.Instance.GetObject(shopPrefab).GetComponent<Room>();
                        break;
                    case RoomType.Health:
                        room = GameObjectPool.Instance.GetObject(roomPrefab).GetComponent<Room>();
                        break;
                    case RoomType.Combat:
                        room = GameObjectPool.Instance.GetObject(roomPrefab).GetComponent<Room>();
                        break;
                    case RoomType.Boss:
                        room = GameObjectPool.Instance.GetObject(bossRoomPrefab).GetComponent<Room>();
                        break;
                }
                room.Init(roomData.roomType, roomData.enemyCount, Level);
            }
            else
            {
                enemies = GameManger.Instance.allEnemies;
                bosses = GameManger.Instance.allBoss;
                room = GameObjectPool.Instance.GetObject(roomPrefab).GetComponent<Room>();
                room.Init(RoomType.Combat, 4, 1);
            }

        }
    }
    void Start()
    {

    }
    private void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
