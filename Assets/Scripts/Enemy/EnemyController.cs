using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : RuleFSM<TopDownEnemyData, EnemyController>, ICanPlayAnim, IDamageable
{
    [SerializeField] float maxHealth = 6;
    float currentHealth;
    [SerializeField] Wand wand;
    private int mFace = 1;
    public int FaceDir => mFace;
    public bool FaceEquals(float dir) => FaceDir == dir;
    public void Filp()
    {
        mFace = -mFace;
        transform.Rotate(0, 180, 0);
    }
    private Rigidbody2D mRig;
    [SerializeField]
    private Vector2 MoveVel;
    // 检测到的射线对象组
    private RaycastHit2D[] hits;
    private Collider2D[] collider2Ds;
    // 射线发射的原点 需要在AI下创建一个游戏物体用来调整射线发射位置
    private Transform mFaceDirGroundCheck;
    private Transform mGroundCheck;
    // 给状态类提供一个射线检测的公开方法 使用 RaycastNonAlloc 来进行检测 这个方法可以减少GC的产生
    public bool CheckFaceDirIsGround =>
        Physics2D.RaycastNonAlloc(mFaceDirGroundCheck.position, Vector2.down, hits, Data.GroundRayLength, Data.GroundLayer) > 0;
    public bool CheckObstacle =>
        Physics2D.OverlapBoxNonAlloc(mGroundCheck.position, Data.GroundCheckBoxSize, 0, collider2Ds, Data.GroundLayer) > 0;
    public bool ObserveCheck =>
        Physics2D.OverlapBoxNonAlloc(transform.position + Data.ObserveCheckOffset, Data.ObserveCheckBoxSize, 0, collider2Ds, Data.PlayerLayer) > 0;
    public bool PursuitCheck =>
        Physics2D.OverlapBoxNonAlloc(new Vector2(transform.position.x + mFace * Data.PursuitCheckOffset.x, transform.position.y + Data.PursuitCheckOffset.y),
            Data.PursuitCheckBoxSize, 0, collider2Ds, Data.PlayerLayer) > 0;
    public bool AttackCheck =>
        Physics2D.OverlapBoxNonAlloc(new Vector2(transform.position.x + mFace * Data.AttackCheckOffset.x, transform.position.y + Data.AttackCheckOffset.y),
            Data.AttackCheckBoxSize, 0, collider2Ds, Data.PlayerLayer) > 0;
    public bool CheckForwardObstacle =>
        Physics2D.RaycastNonAlloc(mFaceDirGroundCheck.position, mFace * Vector2.right, hits, Data.GroundRayLength, Data.GroundLayer) > 0;
    public bool IsDie => currentHealth <= 0;
    public Transform GetColliderTarget(string tag)
    {
        for (int i = 0; i < collider2Ds.Length; i++)
        {
            if (collider2Ds[i] == null) continue;
            if (!collider2Ds[i].CompareTag(tag)) continue;
            //Debug.Log("循环" + collider2Ds[i]);
            return collider2Ds[i].transform;
        }
        return null;
    }
    private void Start()
    {
        // 初始化可检测射线的个数 正常两个位置差不多
        hits = new RaycastHit2D[1];
        collider2Ds = new Collider2D[4];
        mRig = GetComponent<Rigidbody2D>();
        // 使用 transform.Find 找到射线检测发射点的变换组件对象
        mFaceDirGroundCheck = transform.Find("GroundCurDirCheck");
        mGroundCheck = transform.Find("GroundCheck");
        wand = GetComponentInChildren<Wand>();
        currentHealth = maxHealth;

    }
    private void OnDrawGizmos()
    {
        // 使用 OnDrawGizmos 函数 来渲染出 射线
        if (mFaceDirGroundCheck == null || mGroundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + mFace * Data.PursuitCheckOffset.x, transform.position.y + Data.PursuitCheckOffset.y), Data.PursuitCheckBoxSize);
        Gizmos.DrawWireCube(new Vector2(transform.position.x + mFace * Data.AttackCheckOffset.x, transform.position.y + Data.AttackCheckOffset.y), Data.AttackCheckBoxSize);
        Gizmos.DrawWireCube(transform.position + Data.ObserveCheckOffset, Data.ObserveCheckBoxSize);
        Gizmos.DrawWireCube(mGroundCheck.position, Data.GroundCheckBoxSize);
        Gizmos.DrawLine(mFaceDirGroundCheck.position, mFaceDirGroundCheck.position + Vector3.down * Data.GroundRayLength);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(mFaceDirGroundCheck.position, mFaceDirGroundCheck.position + Vector3.right * mFace * Data.GroundRayLength);
    }
    public float XSpeed
    {
        get => MoveVel.x;
        set
        {
            MoveVel.x = value;
        }
    }
    public float YSpeed
    {
        get => MoveVel.y;
        set
        {
            MoveVel.y = value;
        }
    }
    private void FixedUpdate()
    {
        mRig.velocity = MoveVel;
        // Debug.Log("更新" + Data?.GetType().Name);
    }

    public override string DataLabel => "TopDownEnemyData";
    protected override IStateBuilder<E_StateLife> Builder => new EnemyStateBuilder(this, Data);

    // 显式实现 播放动画接口
    void ICanPlayAnim.PlayAnim(int animHash) { }
    public void Cast(Vector2 pos)
    {
        wand.Cast(pos, tag);
    }
    public void Drop()
    {
        if (wand != null)
        {
            wand.gameObject.layer = LayerMask.NameToLayer("PickUpable");
            wand.transform.SetParent(null);
            wand.gameObject.SetActive(true);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
    }
    public void Die()
    {
        MEventSystem.Instance.Send(new EnemyDeath() { pos = transform.position });
        // Drop();
        Destroy(gameObject);
    }
}
public class EnemyStateBuilder : IStateBuilder<E_StateLife>
{
    private EnemyController fsm;
    private TopDownEnemyData data;
    public EnemyStateBuilder(EnemyController fsm, TopDownEnemyData data)
    {
        this.data = data;
        this.fsm = fsm;
        // Debug.Log(fsm?.GetType().Name + data?.GetType().Name);
    }
    Dictionary<string, State<E_StateLife>> IStateBuilder<E_StateLife>.Create(out string firstStateName)
    {
        firstStateName = StateName.Idle;


        return new Dictionary<string, State<E_StateLife>>()
            {
                { StateName.Idle ,new EnemyIdleState().SetAnim(StateName.GroundIdle)},
                { StateName.Observe ,new EnemyObserveState().SetAnim(StateName.GroundIdle) },
                { StateName.Patrol ,new EnemyPatrolState().SetAnim(StateName.GroundMove)},
                { StateName.Pursuit ,new EnemyPursuitState().SetAnim(StateName.GroundMove)},

                { StateName.Attack,new EnemyAttackState().SetAnim(StateName.Attack)},
                { StateName.Die,new EnemyDieState().SetAnim(StateName.Die)},
            };
    }
}

//待机状态
public class EnemyIdleState : AnimState<EnemyController, TopDownEnemyData>
{

    private float idleStartTime;
    private float idleDuration;
    protected override string CheckCondition()
    {
        if (Time.time - idleStartTime >= idleDuration)
            return StateName.Patrol;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        switch (life)
        {
            case E_StateLife.Enter:
                Machine.XSpeed = 0;
                idleStartTime = Time.time;
                idleDuration = Data.IdleTime;
                break;
        }
    }
}

// 追击状态
public class EnemyPursuitState : AnimState<EnemyController, TopDownEnemyData>
{
    private Transform target;
    protected override string CheckCondition()
    {
        if (Machine.IsDie) return StateName.Die;
        else if (!Machine.PursuitCheck && !Machine.ObserveCheck) return StateName.Observe;
        else if (Machine.AttackCheck) return StateName.Attack;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        // 追击目标
        if (Machine.ObserveCheck || Machine.PursuitCheck)
        {
            Vector3 targetPosition = Machine.GetColliderTarget("Player").position;
            Vector3 direction = targetPosition - Machine.transform.position;
            int dir = direction.x > 0 ? 1 : -1;
            if (Mathf.Abs(direction.x) > 1 && !Machine.FaceEquals(dir)) Machine.Filp();
            direction.z = 0f; // Ignore vertical distance
            direction.Normalize();

            Machine.XSpeed = direction.x * Data.GroundPursuitSpeed;
            Machine.YSpeed = direction.y * Data.GroundPursuitSpeed;
        }
    }
}
// 观察状态
public class EnemyObserveState : AnimState<EnemyController, TopDownEnemyData>
{
    private float observeStartTime;
    private float observeDuration;
    protected override string CheckCondition()
    {
        if (Machine.IsDie) return StateName.Die;
        else
        if (Machine.ObserveCheck || Machine.PursuitCheck)
        {
            return StateName.Pursuit;
        }
        if (Time.time - observeStartTime >= observeDuration)
            return StateName.Patrol;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        switch (life)
        {
            case E_StateLife.Enter:
                Machine.XSpeed = 0;
                observeStartTime = Time.time;
                observeDuration = Data.ObserveWaitTime;
                break;
        }
    }
}
// 巡逻状态
public class EnemyPatrolState : AnimState<EnemyController, TopDownEnemyData>
{
    private Vector3 patrolCenter; // 巡逻中心点
    bool isArrive;
    Vector2 randomPosition;
    Vector3 patrolPosition;

    protected override string CheckCondition()
    {
        if (Machine.IsDie) return StateName.Die;
        else
        if (Machine.PursuitCheck) return StateName.Pursuit;
        return null;
    }
    void SetPatrolPosition()
    {
        isArrive = false;
        randomPosition = Random.insideUnitCircle * Data.PatrolRadius;
        patrolPosition = Machine.transform.position + (Vector3)randomPosition;
        Vector3 direction = (patrolPosition - Machine.transform.position).normalized;
        Machine.XSpeed = direction.x * Data.GroundPatrolSpeed;
        Machine.YSpeed = direction.y * Data.GroundPatrolSpeed;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        switch (life)
        {
            case E_StateLife.Enter:
                isArrive = false;
                SetPatrolPosition();
                break;
            case E_StateLife.Update:
                if (Machine.CheckObstacle)
                {
                    SetPatrolPosition();
                    return;
                }
                if (isArrive)
                {
                    SetPatrolPosition();
                    return;
                }
                if (Vector2.Distance(Machine.transform.position, patrolPosition) < 0.2f)
                {
                    isArrive = true;
                    Machine.XSpeed = 0;
                    Machine.YSpeed = 0;
                    return;
                }

                break;
        }
    }
}
// 攻击状态
public class EnemyAttackState : AnimState<EnemyController, TopDownEnemyData>
{
    protected override string CheckCondition()
    {
        if (Machine.IsDie) return StateName.Die;
        else
        if (!Machine.AttackCheck) return StateName.Pursuit;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        switch (life)
        {
            case E_StateLife.Enter:
                Vector2 pos = Machine.GetColliderTarget("Player").position;
                Machine.Cast(pos);
                break;
            case E_StateLife.Update:
                pos = Machine.GetColliderTarget("Player").position;
                Machine.Cast(pos);
                break;
        }

    }
}
// 死亡状态
public class EnemyDieState : AnimState<EnemyController, TopDownEnemyData>
{
    protected override string CheckCondition()
    {
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        switch (life)
        {
            case E_StateLife.Enter:
                Machine.XSpeed = 0;
                Machine.Die();
                break;
        }
    }
}


