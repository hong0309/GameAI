public class ChaseNode : Node
{
    private readonly EnemyAI enemy;

    public ChaseNode(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public override Status Evaluate()
    {
        enemy.CurrentBTAction = "Chase";

        if (enemy.player == null)
            return Status.Failure;

        enemy.agent.SetDestination(enemy.player.position);

        return Status.Running;
    }
}