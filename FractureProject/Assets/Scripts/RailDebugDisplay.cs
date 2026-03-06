using System.Collections.Generic;
using UnityEngine;

public class RailDebugDisplay : MonoBehaviour
{
    private Color nodeColor = Color.blue;
    private Color startNodeColor = Color.red;
    private Color switchNodeColor = Color.magenta;
    private float nodeSize = 0.2f;

    public List<Crowd> branchesOrigins = new List<Crowd>();

    private void OnDrawGizmos()
    {
        foreach (Crowd branchOrigin in branchesOrigins)
        {
            DrawNewBranch(branchOrigin.transform);
        }
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
            
            if(currentPoint.gameObject.activeSelf == false) break;

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
            
            Transform previousPoint = (i - 1) >= 0 ? parent.GetChild(i - 1) : null;
            
            Gizmos.color = nodeColor;
            if (previousPoint != null) 
                Gizmos.DrawLine(previousPoint.position, currentPoint.position);
        }
    }
    
    private Dictionary<CrowdNode, GameObject> nodeMapping = new Dictionary<CrowdNode, GameObject>();
    
    private void OnEnable()
    {
        CrowdNode.OnNodeCreated += RegisterNode;
    }
    
    
    public void RegisterNode(CrowdNode node, GameObject nodeObject)
    {
        nodeMapping[node] = nodeObject.gameObject;
        node.OnActiveStateChanged += HandleStateChange;
        
        Debug.Log(nodeObject.gameObject.name);
    }
    
    private void HandleStateChange(CrowdNode node, bool newState)
    {
        if (nodeMapping.TryGetValue(node, out GameObject go))
        {
            go.SetActive(newState);
        }
    }

    private void OnDestroy()
    {
        foreach (var node in nodeMapping.Keys)
            node.OnActiveStateChanged -= HandleStateChange;
    }
}
