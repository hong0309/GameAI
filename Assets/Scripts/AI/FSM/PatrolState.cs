public class PatrolState : State
{
    public PatrolState(EnemyAI enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.MoveToCurrentPatrolPoint();
    }

    public override void Update()
    {
        if (enemy.PlayerDetected)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        if (enemy.patrolPoints == null ||
            enemy.patrolPoints.Length == 0)
            return;

        if (!enemy.agent.pathPending &&
            enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            enemy.MoveToNextPatrolPoint();
        }
    }
}