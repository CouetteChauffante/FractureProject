using System.Collections.Generic;
using UnityEngine;

public class RailDebugDisplay : MonoBehaviour
{
    private Color nodeColor = Color.blue;
    private Color startNodeColor = Color.red;
    private Color switchNodeColor = Color.magenta;
    private float nodeSize = 0.2f;

    private void OnDrawGizmos()
    {
        DrawNewBranch(transform);
    }

    private void DrawNewBranch(Transform originNode)
    {
        Gizmos.color = startNodeColor;
        Gizmos.DrawSphere(originNode.position, nodeSize);
        
        if (originNode.childCount > 0) 
            DrawChildrenNodes(originNode);
    }

    private void DrawChildrenNodes(Transform parent)
    {
        Gizmos.color = nodeColor;
        
        if (parent.GetChild(0) != null) 
            Gizmos.DrawLine(parent.position, parent.GetChild(0).position);
        
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform currentPoint = parent.GetChild(i);

            if (currentPoint.childCount > 0)
            {
                for (int j = 0; j < currentPoint.childCount; j++)
                {
                    if(currentPoint.GetChild(j).gameObject.activeSelf == false) continue;
                    
                    DrawNewBranch(currentPoint.GetChild(j));
                    
                    Gizmos.color = switchNodeColor;
                    Gizmos.DrawLine(currentPoint.position, currentPoint.GetChild(j).position);
                }
                Gizmos.color = switchNodeColor;
                Gizmos.DrawSphere(currentPoint.position, nodeSize);
            }
            else
            {
                Gizmos.color = nodeColor;
                Gizmos.DrawSphere(currentPoint.position, nodeSize);
            }
            
            Transform nextPoint = (i + 1) < parent.childCount ? parent.GetChild(i + 1) : null;
            
            Gizmos.color = nodeColor;
            if (nextPoint != null) 
                Gizmos.DrawLine(currentPoint.position, nextPoint.position);
        }
    }
    
    private Dictionary<CrowdNode, GameObject> nodeMapping = new Dictionary<CrowdNode, GameObject>();
    
    private void Start()
    {
        Crowd crowd = GetComponent<Crowd>();
        if (crowd == null)
            Destroy(this);
        
        RegisterNodeRecursive(crowd.rootNode, transform);
    }
    
    private void RegisterNodeRecursive(CrowdNode node, Transform origin, int nodeIndex = 0)
    {
        if (node == null || nodeIndex >= origin.childCount) return;

        Transform nodeObject = origin.GetChild(nodeIndex);

        nodeMapping[node] = nodeObject.gameObject;
        node.OnStateChanged += HandleStateChange;
    
        Debug.Log($"Enregistré : {nodeObject.name}");

        if (node is SwitchCrowdNode switchNode && nodeObject.childCount > 0)
        {
            for (int i = 0; i < nodeObject.childCount; i++)
            {
                RegisterNodeRecursive(switchNode.nextOriginNodes[i], nodeObject.GetChild(i));
            }
        }
        
        if (node.nextNode != null)
        {
            RegisterNodeRecursive(node.nextNode, origin, nodeIndex + 1);
        }
    }
    
    private void HandleStateChange(CrowdNode node, bool newState)
    {
        
        Debug.Log("ca handle");
        if (nodeMapping.TryGetValue(node, out GameObject go))
        {
            go.SetActive(newState);
        }
    }

    private void OnDestroy()
    {
        foreach (var node in nodeMapping.Keys)
            node.OnStateChanged -= HandleStateChange;
    }
}
