using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class RoomGenerator : MonoBehaviour
{
    [SerializeField] int shopSpawnFrequency = 3;
    [SerializeField] int healthSpawnFrequency = 5;
    [SerializeField] int bossSpawnFrequency = 6;
    [SerializeField] int Level;
    [SerializeField] int enemyIncreaseInterval = 6;
    [SerializeField] int initialEnemiesCount = 4;
    [SerializeField] int additionalEnemies = 2;

    public enum Direction { up, down, left, right };

    public Direction direction;
    [Header("房间信息")]
    public GameObject roomPrefab;
    public GameObject shopPrefab;
    public int roomNumber;
    public Color startColor, endColor;
    private GameObject endRoom;

    [Header("位置控制")]
    public Transform generatorPoint;
    public float xOffset;
    public float yOffset;
    public LayerMask roomLayer;
    public int maxStep;
    public List<Room> rooms = new List<Room>();


    public List<GameObject> farRooms = new List<GameObject>();
    public List<GameObject> lessFarRooms = new List<GameObject>();
    public List<GameObject> oneWayRooms = new List<GameObject>();
    List<GameObject> walls = new List<GameObject>();
    public WallType wallType;
    private void Awake()
    {
        MEventSystem.Instance.Register<ChangeRoom>(e =>
        {
            Level++;
            // PlayerPrefs.SetInt("Level", Level);
            // PlayerPrefs.SetInt("RoomType", (int)e.roomType);
            // DestroyMap();
            StartCoroutine(Init());
            rooms[0].Init(RoomType.Combat, 0, 1, false, true);
            for (int i = 1; i < rooms.Count; i++)
            {
                if (rooms[i].gameObject == endRoom)
                {
                    if (e.roomType == RoomType.Shop || e.roomType == RoomType.Enhancement)
                        endRoom.GetComponent<Room>().Init(e.roomType, 0, Level, true);
                    else if (e.roomType == RoomType.Boss)
                        endRoom.GetComponent<Room>().Init(e.roomType, 1, Level, true);
                    else
                        endRoom.GetComponent<Room>().Init(e.roomType, Level / enemyIncreaseInterval * additionalEnemies + initialEnemiesCount, Level, true);
                    continue;
                }
                rooms[i].Init(RoomType.Combat, Level / enemyIncreaseInterval * additionalEnemies + initialEnemiesCount, Level);
            }
            // SceneManager.LoadScene("Demo");
            // StartCoroutine(Init());
            // for (int i = 0; i < rooms.Count; i++)
            // {
            //     if (rooms[i] == endRoom)
            //     {
            //         if (e.roomType == RoomType.Shop || e.roomType == RoomType.Enhancement)
            //             endRoom.GetComponent<Room>().Init(e.roomType, 0, Level, true);
            //         else if (e.roomType == RoomType.Boss)
            //             endRoom.GetComponent<Room>().Init(e.roomType, 1, Level, true);
            //         else
            //             endRoom.GetComponent<Room>().Init(e.roomType, Level / enemyIncreaseInterval * additionalEnemies + initialEnemiesCount, Level, true);
            //         continue;
            //     }
            //     rooms[i].Init(RoomType.Combat, Level / enemyIncreaseInterval * additionalEnemies + initialEnemiesCount, Level);
            // }

        }).UnRegisterWhenGameObjectDestroy(gameObject);
    }
    void Start()
    {
        StartCoroutine(Init());
        rooms[0].Init(RoomType.Combat, 0, 1, false, true);
        for (int i = 1; i < rooms.Count; i++)
        {
            if (rooms[i].gameObject == endRoom)
            {
                rooms[i].Init(RoomType.Combat, 4, 1, true);
                continue;
            }
            rooms[i].Init(RoomType.Combat, 4, 1);
        }
    }
    public void DestroyMap()
    {
        maxStep = 0;
        if (endRoom != null)
            generatorPoint.position = Vector3.zero;
        foreach (var room in rooms)
        {
            Destroy(room.gameObject);
        }
        foreach (var wall in walls)
        {
            Destroy(wall);
        }
    }

    public IEnumerator Init()
    {
        yield return StartCoroutine(InitMap());
    }
    IEnumerator InitMap()
    {
        DestroyMap();
        rooms.Clear();
        farRooms.Clear();
        lessFarRooms.Clear();
        oneWayRooms.Clear();
        walls.Clear();
        for (int i = 0; i < roomNumber; i++)
        {
            //生成地图
            rooms.Add(Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity).GetComponent<Room>());
            //修改Point位置
            ChangePointPos();
        }
        //


        rooms[0].GetComponent<SpriteRenderer>().color = startColor;

        endRoom = rooms[0].gameObject;

        foreach (var room in rooms)
        {
            //if (room.transform.position.sqrMagnitude > endRoom.transform.position.sqrMagnitude)
            //{
            //    endRoom = room.gameObject;
            //}
            SetupRoom(room, room.transform.position);
        }
        FindEndRoom();
        endRoom.GetComponent<SpriteRenderer>().color = endColor;
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void ChangePointPos()
    {
        do
        {


            direction = (Direction)Random.Range(0, 4);
            switch (direction)
            {
                case Direction.up:
                    generatorPoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.down:
                    generatorPoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.left:
                    generatorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.right:
                    generatorPoint.position += new Vector3(xOffset, 0, 0);
                    break;

            }
        } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f, roomLayer));
    }
    public void SetupRoom(Room newRoom, Vector3 roomPosition)
    {
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);

        newRoom.UpdateRoom(xOffset, yOffset);

        switch (newRoom.doorNumber)
        {
            case 1:
                if (newRoom.roomUp)
                    walls.Add(Instantiate(wallType.singleUp, roomPosition, Quaternion.identity));
                if (newRoom.roomLeft)
                    walls.Add(Instantiate(wallType.singleLeft, roomPosition, Quaternion.identity));
                if (newRoom.roomRight)
                    walls.Add(Instantiate(wallType.singleRight, roomPosition, Quaternion.identity));
                if (newRoom.roomDown)
                    walls.Add(Instantiate(wallType.singleBottom, roomPosition, Quaternion.identity));
                break;
            case 2:
                if (newRoom.roomLeft && newRoom.roomDown)
                    walls.Add(Instantiate(wallType.doubleLB, roomPosition, Quaternion.identity));
                if (newRoom.roomLeft && newRoom.roomRight)
                    walls.Add(Instantiate(wallType.doubleLR, roomPosition, Quaternion.identity));
                if (newRoom.roomLeft && newRoom.roomUp)
                    walls.Add(Instantiate(wallType.doubleLU, roomPosition, Quaternion.identity));
                if (newRoom.roomRight && newRoom.roomDown)
                    walls.Add(Instantiate(wallType.doubleRB, roomPosition, Quaternion.identity));
                if (newRoom.roomRight && newRoom.roomUp)
                    walls.Add(Instantiate(wallType.doubleRU, roomPosition, Quaternion.identity));
                if (newRoom.roomUp && newRoom.roomDown)
                    walls.Add(Instantiate(wallType.doubleUB, roomPosition, Quaternion.identity));
                break;
            case 3:
                if (newRoom.roomLeft && newRoom.roomDown && newRoom.roomRight)
                    walls.Add(Instantiate(wallType.tripleLBR, roomPosition, Quaternion.identity));
                if (newRoom.roomLeft && newRoom.roomUp && newRoom.roomRight)
                    walls.Add(Instantiate(wallType.tripleLUR, roomPosition, Quaternion.identity));
                if (newRoom.roomLeft && newRoom.roomDown && newRoom.roomUp)
                    walls.Add(Instantiate(wallType.tripleLUB, roomPosition, Quaternion.identity));
                if (newRoom.roomUp && newRoom.roomDown && newRoom.roomRight)
                    walls.Add(Instantiate(wallType.tripleRUB, roomPosition, Quaternion.identity));
                break;
            case 4:
                walls.Add(Instantiate(wallType.fourDoors, roomPosition, Quaternion.identity));
                break;
        }

    }
    public void FindEndRoom()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].stepToStart > maxStep)
            {
                maxStep = rooms[i].stepToStart;
            }
        }
        foreach (var room in rooms)
        {
            if (room.stepToStart == 0)
            {
                continue;
            }
            //最远距离的所有房间集合
            if (room.stepToStart == maxStep)
                farRooms.Add(room.gameObject);
            //比最远房间少一步的 以防最远房间进入后还有其他房间进不去，最远房间就是boss房间
            if (room.stepToStart == maxStep - 1)
                lessFarRooms.Add(room.gameObject);

        }
        for (int i = 0; i < farRooms.Count; i++)
        {
            if (farRooms[i].GetComponent<Room>().doorNumber == 1)
            {
                oneWayRooms.Add(farRooms[i]);
            }
        }
        for (int i = 0; i < lessFarRooms.Count; i++)
        {
            if (lessFarRooms[i].GetComponent<Room>().doorNumber == 1)
            {
                oneWayRooms.Add(lessFarRooms[i]);
            }
        }
        if (oneWayRooms.Count != 0)
        {
            endRoom = oneWayRooms[Random.Range(0, oneWayRooms.Count)];
        }
        else
        {
            endRoom = farRooms[Random.Range(0, farRooms.Count)];
        }
    }

}
[System.Serializable]
public class WallType
{
    public GameObject singleLeft, singleRight, singleUp, singleBottom,
                      doubleLR, doubleLU, doubleLB, doubleRU, doubleRB, doubleUB,
                      tripleLUR, tripleLBR, tripleRUB, tripleLUB,
                      fourDoors;
}