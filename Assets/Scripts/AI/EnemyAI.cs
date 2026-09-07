using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public enum AIMode
{
    FSM,
    BehaviorTree
}
public class EnemyAI : MonoBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    [Header("Target")]
    public Transform player;
    [Header("Patrol")]
    public Transform[] patrolPoints;
    public int CurrentPatrolIndex { get; private set; } = 0;
    [Header("Detection")]
    public float detectionRange = 8f;
    [Range(0f, 360f)]
    public float fieldOfView = 90f;
    public LayerMask obstacleMask;
    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    [Header("Search")]
    public float searchDuration = 3f;
    [Header("AI Mode")]
    public AIMode aiMode = AIMode.FSM;
    private BehaviorTreeController behaviorTree;
    public Vector3 LastKnownPlayerPosition { get; private set; }
    public bool HasLastKnownPosition { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public PatrolState PatrolState { get; private set; }
    public ChaseState ChaseState { get; private set; }
    public AttackState AttackState { get; private set; }
    public SearchState SearchState { get; private set; }
    public string CurrentBTAction { get; set; } = "None";
    public bool PlayerDetected { get; private set; }
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        StateMachine = new StateMachine();
        PatrolState = new PatrolState(this);
        ChaseState = new ChaseState(this);
        AttackState = new AttackState(this);
        SearchState = new SearchState(this);
        behaviorTree = new BehaviorTreeController(this);
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Start()
    {
        StateMachine.ChangeState(PatrolState);
    }

    private void Update()
    {
        HandleModeInput();

        PlayerDetected = IsPlayerDetected();

        if (aiMode == AIMode.FSM)
        {
            StateMachine.Update();
        }
        else
        {
            behaviorTree.Update();
        }
    }

    private bool IsPlayerDetected()
    {
        if (IsPlayerDead())
            return false;

        if (player == null)
            return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle > fieldOfView * 0.5f)
            return false;

        Vector3 origin = transform.position + Vector3.up;

        Vector3 target = player.position + Vector3.up;

        Vector3 rayDirection = target - origin;

        if (Physics.Raycast(origin, rayDirection.normalized, rayDirection.magnitude, obstacleMask))
        {
            return false;
        }

        LastKnownPlayerPosition = player.position;
        HasLastKnownPosition = true;

        return true;
    }

    public bool IsPlayerInAttackRange()
    {
        if (IsPlayerDead())
            return false;

        if (player == null)
            return false;

        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }

    private void HandleModeInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SwitchAIMode(AIMode.FSM);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SwitchAIMode(AIMode.BehaviorTree);
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();

            if (health != null)
                health.ResetPlayer();

            ResetAI();
        }
    }

    private void SwitchAIMode(AIMode newMode)
    {
        if (aiMode == newMode)
            return;

        // 현재 이동 경로만 제거
        agent.ResetPath();

        // AI 모드 변경
        aiMode = newMode;

        // BT 표시 상태만 초기화
        CurrentBTAction = "None";

        if (aiMode == AIMode.FSM)
        {
            // 현재 상황에 맞는 FSM State로 바로 진입
            if (PlayerDetected)
            {
                if (IsPlayerInAttackRange())
                {
                    StateMachine.RestartState(AttackState);
                }
                else
                {
                    StateMachine.RestartState(ChaseState);
                }
            }
            else if (HasLastKnownPosition)
            {
                StateMachine.RestartState(SearchState);
            }
            else
            {
                StateMachine.RestartState(PatrolState);
            }
        }
        else
        {
            behaviorTree.ResetTransientState();
        }
    }

    public void ClearLastKnownPosition()
    {
        HasLastKnownPosition = false;
    }

    public void Attack()
    {
        if (player == null)
            return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health == null)
            return;

        health.TakeDamage(attackDamage);
    }

    public bool IsPlayerDead()
    {
        if (player == null)
            return true;

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        if (health == null)
            return false;

        return health.IsDead;
    }

    public void MoveToCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.SetDestination(
            patrolPoints[CurrentPatrolIndex].position
        );
    }

    public void MoveToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        CurrentPatrolIndex =
            (CurrentPatrolIndex + 1) % patrolPoints.Length;

        MoveToCurrentPatrolPoint();
    }

    public void ResetPatrolIndex()
    {
        CurrentPatrolIndex = 0;
    }

    public void ResetAI()
    {
        agent.ResetPath();

        ClearLastKnownPosition();
        CurrentBTAction = "None";

        ResetPatrolIndex();
        behaviorTree.Reset();

        agent.Warp(startPosition);
        transform.rotation = startRotation;

        StateMachine.RestartState(PatrolState);
    }

    private void OnDrawGizmos()
    {
        //Detection Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        //Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //FOV
        Gizmos.color = Color.cyan;

        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView * 0.5f, 0) * transform.forward;

        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView * 0.5f, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftBoundary * detectionRange);

        Gizmos.DrawRay(transform.position, rightBoundary * detectionRange);

        // Patrol Points
        if (patrolPoints == null)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i] == null)
                continue;

            Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

            int nextIndex = (i + 1) % patrolPoints.Length;

            if (patrolPoints[nextIndex] != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
            }
        }

        if (HasLastKnownPosition)
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawSphere(LastKnownPlayerPosition, 0.25f);
        }
    }
}