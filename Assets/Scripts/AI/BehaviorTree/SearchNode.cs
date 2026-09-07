using UnityEngine;

public class SearchNode : Node
{
    private readonly EnemyAI enemy;

    private bool reachedLastPosition = false;
    private float searchTimer = 0f;

    private Quaternion startRotation;

    public SearchNode(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public override Status Evaluate()
    {
        if (enemy == null)
            return Status.Failure;

        if (enemy.player == null)
            return Status.Failure;

        enemy.CurrentBTAction = "Search";
        if (!enemy.HasLastKnownPosition)
        {
            ResetSearch();
            return Status.Failure;
        }

        // 다시 발견하면 Search 종료
        if (enemy.PlayerDetected)
        {
            ResetSearch();
            return Status.Failure;
        }

        // 마지막 위치까지 이동
        if (!reachedLastPosition)
        {
            if (!enemy.agent.hasPath)
            {
                enemy.agent.SetDestination(
                    enemy.LastKnownPlayerPosition
                );
            }

            if (enemy.agent.pathPending)
                return Status.Running;

            if (enemy.agent.remainingDistance
                <= enemy.agent.stoppingDistance)
            {
                reachedLastPosition = true;

                enemy.agent.ResetPath();

                startRotation =
                    enemy.transform.rotation;

                searchTimer = 0f;
            }

            return Status.Running;
        }

        // 좌우 탐색
        searchTimer += Time.deltaTime;

        float duration = enemy.searchDuration;

        float angle;

        // 정면 → 왼쪽
        if (searchTimer < duration * 0.5f)
        {
            angle = Mathf.Lerp(
                0f,
                -60f,
                searchTimer / (duration * 0.5f)
            );
        }
        // 왼쪽 → 오른쪽
        else
        {
            angle = Mathf.Lerp(
                -60f,
                60f,
                (searchTimer - duration * 0.5f)
                / (duration * 0.5f)
            );
        }

        enemy.transform.rotation =
            startRotation *
            Quaternion.Euler(0f, angle, 0f);

        if (searchTimer >= duration)
        {
            enemy.ClearLastKnownPosition();

            ResetSearch();

            return Status.Success;
        }

        return Status.Running;
    }

    public void ResetSearch()
    {
        reachedLastPosition = false;
        searchTimer = 0f;
    }
}