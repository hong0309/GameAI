public class PatrolNode : Node
{
    private EnemyAI enemy;

    public PatrolNode(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public override Status Evaluate()
    {
        enemy.CurrentBTAction = "Patrol";

        if (enemy.patrolPoints == null ||
            enemy.patrolPoints.Length == 0)
        {
            return Status.Failure;
        }

        // 현재 경로가 없는 경우 현재 Patrol Point로 이동
        if (!enemy.agent.hasPath)
        {
            enemy.MoveToCurrentPatrolPoint();
        }

        // Patrol Point 도착
        if (!enemy.agent.pathPending &&
            enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            enemy.MoveToNextPatrolPoint();
        }

        return Status.Running;
    }
}