using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateName
{
    public const string Patrol = "Patrol";
    public const string Pursuit = "Pursuit";
    public const string Observe = "Observe";

    public const string GroundMove = "GroundMove";
    public const string GroundIdle = "GroundIdle";

    public const string AirIdle = "AirIdle";
    public const string AirMove = "AirMove";

    public const string Jump = "Jump";
    public const string InAir = "InAir";

    public const string Attack = "Attack";
    public const string Die = "Die";
}
// 使用状态机实现平台类AI
public class AIFsm : RuleFSM<EnemyData, AIFsm>, ICanPlayAnim, IDamageable
{
    [SerializeField] int currentHealth;
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
    public bool CheckGround =>
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
        currentHealth = Data.MaxHealth;

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
            MoveVel.y = mRig.velocity.y;
        }
    }
    public float YSpeed
    {
        get => MoveVel.y;
        set
        {
            MoveVel.x = mRig.velocity.x;
            MoveVel.y = value;
        }
    }
    private void FixedUpdate()
    {
        mRig.velocity = MoveVel;
        // Debug.Log("更新" + Data?.GetType().Name);
    }

    public override string DataLabel => "EnemyData";
    protected override IStateBuilder<E_StateLife> Builder => new AIStateBuilder(this, Data);

    // 显式实现 播放动画接口
    void ICanPlayAnim.PlayAnim(int animHash) { }
    public void Cast(Vector2 pos)
    {
        wand.Cast(pos);
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
        Drop();
        Destroy(gameObject);
    }
}
public class AIStateBuilder : IStateBuilder<E_StateLife>
{
    private AIFsm fsm;
    private EnemyData data;
    public AIStateBuilder(AIFsm fsm, EnemyData data)
    {
        this.data = data;
        this.fsm = fsm;
        // Debug.Log(fsm?.GetType().Name + data?.GetType().Name);
    }
    Dictionary<string, State<E_StateLife>> IStateBuilder<E_StateLife>.Create(out string firstStateName)
    {
        firstStateName = StateName.AirIdle;

        var ground = new GroundState().Init(fsm, data);
        var inAir = new InAirState().Init(fsm, data);

        return new Dictionary<string, State<E_StateLife>>()
            {
                { StateName.Jump ,new JumpState().SetAnim(StateName.InAir) },

                { StateName.AirIdle ,new InAirIdleState().SetAnim(StateName.InAir).SetFather(inAir) },
                { StateName.AirMove ,new InAirMoveState().SetAnim(StateName.InAir).SetFather(inAir) },

                { StateName.Observe ,new ObserveState().SetAnim(StateName.GroundIdle).SetFather(ground) },
                { StateName.Patrol ,new PatrolState().SetAnim(StateName.GroundMove).SetFather(ground) },
                { StateName.Pursuit ,new PursuitState().SetAnim(StateName.GroundMove).SetFather(ground) },

                { StateName.Attack,new AttackState().SetAnim(StateName.Attack)},
                { StateName.Die,new DieState().SetAnim(StateName.Die)},
            };
    }
}

public class GroundState : BaseState<AIFsm, EnemyData>
{
    private Transform target;
    protected override string CheckCondition()
    {
        if (Machine.CheckGround)
        {
            // 如果检测到玩家并且敌人在地面上 
            if (target != null)
            {
                Vector2 dis = target.position - Machine.transform.position;
                // 如果在头顶 就跳一下
                // Debug.Log(Data);
                if (dis.y > Data.TriggerJumpRange.y && Mathf.Abs(dis.x) < Data.TriggerJumpRange.x) return StateName.Jump;
            }
        }
        else if (Machine.IsDie) return StateName.Die;
        else return StateName.AirIdle;
        return null;
    }

    protected override void Execute(E_StateLife life)
    {
        if (life != E_StateLife.Enter) return;
        if (Machine.ObserveCheck)
        {
            target = Machine.GetColliderTarget("Player");
        }
    }
}
public class JumpState : AnimState<AIFsm, EnemyData>
{
    protected override string CheckCondition() => StateName.AirMove;

    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);

        if (life == E_StateLife.Enter)
        {
            Machine.YSpeed = Data.JumpForce;
        }
    }
}
public class InAirState : BaseState<AIFsm, EnemyData>
{
    protected override string CheckCondition()
    {
        if (Machine.CheckGround) return StateName.Observe;
        else if (Machine.IsDie) return StateName.Die;
        return null;
    }

    protected override void Execute(E_StateLife life)
    {

    }
}
public class InAirMoveState : AnimState<AIFsm, EnemyData>
{
    protected override string CheckCondition()
    {
        if (!Machine.PursuitCheck && !Machine.ObserveCheck) return StateName.AirIdle;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        // 追击目标
        if (Machine.ObserveCheck || Machine.PursuitCheck)
        {
            float dis = Machine.GetColliderTarget("Player").position.x - Machine.transform.position.x;
            int dir = dis > 0 ? 1 : -1;
            if (Mathf.Abs(dis) > 1 && !Machine.FaceEquals(dir)) Machine.Filp();
            Machine.XSpeed = Machine.FaceDir * Data.GroundPursuitSpeed;
        }
    }
}
public class InAirIdleState : AnimState<AIFsm, EnemyData>
{
    protected override string CheckCondition()
    {
        // 如果检测到玩家 就往玩家方向移动
        if (Machine.PursuitCheck || Machine.ObserveCheck) return StateName.AirMove;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        Machine.XSpeed = 0;
        // Debug.Log("空中闲置");
    }
}
// 追击状态
public class PursuitState : AnimState<AIFsm, EnemyData>
{
    private Transform target;
    protected override string CheckCondition()
    {
        if (!Machine.CheckFaceDirIsGround || (!Machine.PursuitCheck && !Machine.ObserveCheck)) return StateName.Observe;
        else if (Machine.AttackCheck) return StateName.Attack;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);
        // 追击目标
        if (Machine.ObserveCheck || Machine.PursuitCheck)
        {
            float dis = Machine.GetColliderTarget("Player").position.x - Machine.transform.position.x;
            int dir = dis > 0 ? 1 : -1;
            if (Mathf.Abs(dis) > 1 && !Machine.FaceEquals(dir)) Machine.Filp();
            Machine.XSpeed = Machine.FaceDir * Data.GroundPursuitSpeed;
        }
    }
}
// 观察状态
public class ObserveState : AnimState<AIFsm, EnemyData>
{
    private float observeStartTime;
    private float observeDuration;
    protected override string CheckCondition()
    {
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
public class PatrolState : AnimState<AIFsm, EnemyData>
{
    protected override string CheckCondition()
    {
        if (Machine.PursuitCheck) return StateName.Pursuit;
        else if (Machine.IsDie) return StateName.Die;
        return null;
    }
    protected override void Execute(E_StateLife life)
    {
        base.Execute(life);

        // 如果前方还可以走 继续移动
        if (Machine.CheckForwardObstacle)
        {
            Machine.Filp();
        }
        else if (Machine.CheckFaceDirIsGround)
        {
            Machine.XSpeed = Data.GroundPatrolSpeed * Machine.FaceDir;
        }
        // 如果前方无路可走 但是在地面上 就转头向后移动
        else if (Machine.CheckGround)
        {
            Machine.Filp();
        }
    }
}
// 攻击状态
public class AttackState : AnimState<AIFsm, EnemyData>
{
    protected override string CheckCondition()
    {
        if (!Machine.AttackCheck) return StateName.Pursuit;
        else if (Machine.IsDie) return StateName.Die;
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
public class DieState : AnimState<AIFsm, EnemyData>
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

