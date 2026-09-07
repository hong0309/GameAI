public class ChaseState : State
{
    public ChaseState(EnemyAI enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
    }

    public override void Update()
    {
        if (!enemy.PlayerDetected)
        {
            enemy.StateMachine.ChangeState(enemy.SearchState);
            return;
        }

        if (enemy.IsPlayerInAttackRange())
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
            return;
        }

        enemy.agent.SetDestination(enemy.player.position);
    }

    public override void Exit()
    {
    }
}