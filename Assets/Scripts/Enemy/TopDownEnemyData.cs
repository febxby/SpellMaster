using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New TopDownEnemyData", menuName = "StateMachine/TopDownEnemyData")]

public class TopDownEnemyData : EnemyData
{
    public float PatrolRadius = 5f;
    public float IdleTime = 1.5f;
}
