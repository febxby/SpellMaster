using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public struct EnemyDeath { };
public struct ChangeRoom
{
    public RoomType roomType;
    public int roomLevel;
};
public class LevelController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject shopPrefab;
    public GameObject enhancementShopPrefab;
    public GameObject roomPrefab;
    public GameObject bossRoomPrefab;
    public Door[] door;

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
                    room = Instantiate(shopPrefab).GetComponent<Room>();
                    break;
                case RoomType.Enhancement:
                    room = Instantiate(enhancementShopPrefab).GetComponent<Room>();
                    break;
                case RoomType.Combat:
                    room = Instantiate(roomPrefab).GetComponent<Room>();
                    break;
                case RoomType.Boss:
                    room = Instantiate(bossRoomPrefab).GetComponent<Room>();
                    break;
            }
            if (e.roomType == RoomType.Shop || e.roomType == RoomType.Enhancement)
                room.Init(e.roomType, 0, Level);
            else if (e.roomType == RoomType.Boss)
                room.Init(e.roomType, 1, Level);
            else
                room.Init(e.roomType, Level / enemyIncreaseInterval * additionalEnemies + initialEnemiesCount, Level);
        });
        enemies = GameManger.Instance.allEnemies;
        bosses = GameManger.Instance.allBoss;
        room = Instantiate(roomPrefab).GetComponent<Room>();
        room.Init(RoomType.Combat, 4, 1);

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
