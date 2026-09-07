using System;

public class ConditionNode : Node
{
    private readonly Func<bool> condition;

    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;
    }

    public override Status Evaluate()
    {
        return condition()
            ? Status.Success
            : Status.Failure;
    }
}