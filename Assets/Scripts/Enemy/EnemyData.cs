using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New EnemyData", menuName = "StateMachine/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int MaxHealth = 100;
    public float GroundPatrolSpeed = 5f;
    public float GroundPursuitSpeed = 8f;
    public float GroundRayLength = 1f;
    public float ObserveWaitTime = 1f;
    public float JumpForce = 12;
    public float InAirMoveSpeed = 7;

    public Vector2 GroundCheckBoxSize;

    public Vector2 ObserveCheckBoxSize;
    public Vector3 ObserveCheckOffset;

    public Vector2 PursuitCheckBoxSize;
    public Vector2 PursuitCheckOffset;
    public Vector2 AttackCheckBoxSize;
    public Vector2 AttackCheckOffset;

    public Vector2 TriggerJumpRange;

    public LayerMask GroundLayer;
    public LayerMask PlayerLayer;
}
