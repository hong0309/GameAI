using TMPro;
using UnityEngine;

public class AIStatsUI : MonoBehaviour
{
    public EnemyAI enemy;
    public TMP_Text stateText;

    private void Update()
    {
        PlayerHealth playerHealth = enemy.player.GetComponent<PlayerHealth>();

        string healthText = 
            playerHealth != null
                ? playerHealth.CurrentHealth.ToString()
                : "-";
        if (enemy == null || stateText == null)
            return;

        if (enemy.aiMode == AIMode.FSM)
        {
            string stateName =
                enemy.StateMachine.GetCurrentStateName()
                .Replace("State", "");

            stateText.text =
                "AI Mode : FSM\n" +
                "Current State : " + stateName + "\n" +
                "Player Detected : " + enemy.PlayerDetected + "\n" +
                "Player HP : " + healthText + "\n" +
                "\n" +
                "[1] FSM\n" +
                "[2] Behavior Tree\n" +
                "[R] Reset";
        }
        else
        {
            stateText.text =
                "AI Mode : Behavior Tree\n" +
                "Current Action : " +
                enemy.CurrentBTAction + "\n" +
                "Player Detected : " + enemy.PlayerDetected + "\n" +
                "Player HP : " + healthText + "\n" +
                "\n" +
                "[1] FSM\n" +
                "[2] Behavior Tree\n" +
                "[R] Reset";
        }
    }
}