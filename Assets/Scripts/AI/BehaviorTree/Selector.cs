using System.Collections.Generic;

public class Selector : Node
{
    private readonly List<Node> children;

    public Selector(List<Node> children)
    {
        this.children = children;
    }

    public override Status Evaluate()
    {
        foreach (Node child in children)
        {
            Status status = child.Evaluate();

            if (status == Status.Success)
                return Status.Success;

            if (status == Status.Running)
                return Status.Running;
        }

        return Status.Failure;
    }
}