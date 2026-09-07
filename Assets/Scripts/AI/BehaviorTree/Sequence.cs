using System.Collections.Generic;

public class Sequence : Node
{
    private readonly List<Node> children;

    public Sequence(List<Node> children)
    {
        this.children = children;
    }

    public override Status Evaluate()
    {
        foreach (Node child in children)
        {
            Status status = child.Evaluate();

            if (status == Status.Failure)
                return Status.Failure;

            if (status == Status.Running)
                return Status.Running;
        }

        return Status.Success;
    }
}