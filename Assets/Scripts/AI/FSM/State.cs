public abstract class State
{
    protected EnemyAI enemy;

    public State(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public abstract void Update();
    public virtual void Exit() { }
}
