using UnityEngine;

public class StopNodeEvent : MonoBehaviour
{
    private StopCrowdNode node;

    public void Bind(StopCrowdNode node)
    {
        this.node = node;
    }

    public void SetStop(bool stop)
    {
        if (node != null)
            node.isStopped = stop;
    }
}