using UnityEngine;

public class AttackNode : Node
{
    private readonly EnemyAI enemy;
    private float attackTimer;

    public AttackNode(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public override Status Evaluate()
    {
        enemy.CurrentBTAction = "Attack";

        enemy.agent.ResetPath();

        Vector3 direction =
            enemy.player.position - enemy.transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            enemy.transform.rotation =
                Quaternion.LookRotation(direction);
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            enemy.Attack();
            attackTimer = enemy.attackCooldown;
        }

        return Status.Running;
    }
}