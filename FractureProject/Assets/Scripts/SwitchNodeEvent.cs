using UnityEngine;

public class SwitchNodeEvent : MonoBehaviour
{
    private SwitchCrowdNode node;

    public void Bind(SwitchCrowdNode node)
    {
        this.node = node;
    }

    public void SwitchEvent(int amount)
    {
        Debug.Log("ca switch");
        if (node != null)
            node.Switch(amount);
    }
}