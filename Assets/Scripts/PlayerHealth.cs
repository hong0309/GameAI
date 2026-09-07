using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
        }
    }

    public void ResetPlayer()
    {
        CurrentHealth = maxHealth;

        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}