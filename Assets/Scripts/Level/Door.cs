using System;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] RoomType doorType;
    public GameObject room;
    Action action;
    public Door Init(RoomType type, Action action)
    {
        this.action = action;
        doorType = type;
        switch (type)
        {
            case RoomType.Shop:
                text.text = "商店";
                break;
            case RoomType.Health:
                text.text = "治疗";
                break;
            case RoomType.Enhancement:
                text.text = "强化";
                break;
            case RoomType.Combat:
                text.text = "战斗";
                break;
            case RoomType.Boss:
                text.text = "精英";
                break;
            default:
                break;
        }
        return this;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            action?.Invoke();
            MEventSystem.Instance.Send<ChangeRoom>(new ChangeRoom { roomType = doorType, roomLevel = 1 });
        }
    }
}