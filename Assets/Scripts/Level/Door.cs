using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] RoomType doorType;
    public GameObject room;
    Action action;
    public Door Init(RoomType type, Action action)
    {
        this.action = action;
        doorType = type;
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