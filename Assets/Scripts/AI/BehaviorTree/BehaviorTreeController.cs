using System.Collections.Generic;

public class BehaviorTreeController
{
    private EnemyAI enemy;
    private Node root;
    private PatrolNode patrolNode;
    private SearchNode searchNode;
    private string previousAction = "None";

    public BehaviorTreeController(EnemyAI enemy)
    {
        this.enemy = enemy;

        BuildTree();
    }

    private void BuildTree()
    {
        patrolNode = new PatrolNode(enemy);
        searchNode = new SearchNode(enemy);

        Sequence attackSequence = new Sequence(
            new List<Node>
            {
                new ConditionNode(() => enemy.PlayerDetected),
                new ConditionNode(enemy.IsPlayerInAttackRange),
                new AttackNode(enemy)
            }
        );

        Sequence chaseSequence = new Sequence(
            new List<Node>
            {
                new ConditionNode(() => enemy.PlayerDetected),
                new ChaseNode(enemy)
            }
        );

        Sequence searchSequence = new Sequence(
            new List<Node>
            {
                new ConditionNode(() => enemy.HasLastKnownPosition),
                searchNode
            }
        );

        root = new Selector(
            new List<Node>
            {
                attackSequence,
                chaseSequence,
                searchSequence,
                patrolNode
            }
        );
    }

    public void Update()
    {
        root?.Evaluate();

        if (previousAction == "Search" &&
            enemy.CurrentBTAction != "Search")
        {
            searchNode?.ResetSearch();
        }

        previousAction = enemy.CurrentBTAction;
    }

    public void Reset()
    {
        searchNode?.ResetSearch();
        previousAction = "None";
    }

    public void ResetTransientState()
    {
        searchNode?.ResetSearch();
        previousAction = "None";
    }
}