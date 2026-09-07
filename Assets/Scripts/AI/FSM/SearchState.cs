using UnityEngine;

public class SearchState : State
{
    private float searchTimer;
    private bool reachedLastPosition;

    private Quaternion startRotation;

    public SearchState(EnemyAI enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        searchTimer = 0f;
        reachedLastPosition = false;

        if (enemy.HasLastKnownPosition)
        {
            enemy.agent.SetDestination(
                enemy.LastKnownPlayerPosition
            );
        }
    }

    public override void Update()
    {
        // 탐색 중 플레이어를 다시 발견
        if (enemy.PlayerDetected)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // 마지막으로 본 위치까지 이동
        if (!reachedLastPosition)
        {
            if (enemy.agent.pathPending)
                return;

            if (enemy.agent.remainingDistance
                <= enemy.agent.stoppingDistance)
            {
                reachedLastPosition = true;

                enemy.agent.ResetPath();

                startRotation = enemy.transform.rotation;
                searchTimer = 0f;
            }

            return;
        }

        SearchAround();
    }

    private void SearchAround()
    {
        searchTimer += Time.deltaTime;

        float duration = enemy.searchDuration;

        float angle;

        // 전반부: 정면 → 왼쪽
        if (searchTimer < duration * 0.5f)
        {
            angle = Mathf.Lerp(
                0f,
                -60f,
                searchTimer / (duration * 0.5f)
            );
        }
        // 후반부: 왼쪽 → 오른쪽
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

            enemy.StateMachine.ChangeState(
                enemy.PatrolState
            );
        }
    }

    public override void Exit()
    {
    }
}