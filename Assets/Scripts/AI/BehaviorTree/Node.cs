public abstract class Node
{
    public enum Status
    {
        Success,
        Failure,
        Running
    }

    public abstract Status Evaluate();
}