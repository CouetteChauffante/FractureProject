using System.Collections.Generic;
using UnityEngine;

public class RailDebugDisplay : MonoBehaviour
{
    [Header("Editor Colors (Transform)")]
    private Color nodeColor = Color.blue;
    private Color startNodeColor = Color.red;
    private Color switchNodeColor = Color.magenta;
    private float nodeSize = 0.2f;

    public List<Crowd> branchesOrigins = new List<Crowd>();

    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            foreach (Crowd crowd in branchesOrigins)
            {
                if (crowd != null && crowd.rootNode != null)
                {
                    DrawRuntimeNode(crowd.rootNode);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            foreach (Crowd crowd in branchesOrigins)
            {
                if (crowd != null && crowd.transform != null)
                {
                    DrawNewBranch(crowd.transform);
                }
            }
        }
    }

    private void DrawRuntimeNode(CrowdNode node)
    {
        if (node == null) return;

        Color currentColor = node.IsPathValid() ? Color.green : Color.red; 

        GizmosRuntimeCustom.color = currentColor;

        if (node is ExitCrowdNode)
        {
            GizmosRuntimeCustom.color = Color.yellow;
            GizmosRuntimeCustom.DrawCube(node.position, Vector3.one * nodeSize);
        }
        else if (node is SwitchCrowdNode)
        {
            GizmosRuntimeCustom.DrawWireCube(node.position, Vector3.one * (nodeSize * 1.5f));
        }
        else if (node is StopCrowdNode stopNode)
        {
            GizmosRuntimeCustom.DrawSphere(node.position, nodeSize * 1.2f);
        }
        else
        {
            GizmosRuntimeCustom.DrawSphere(node.position, nodeSize);
        }

        if (node.nextNode != null)
        {
            GizmosRuntimeCustom.color = currentColor;
            DrawLineWithArrow(node.position, node.nextNode.position);
            
            DrawRuntimeNode(node.nextNode);
        }
    }

    private void DrawLineWithArrow(Vector3 start, Vector3 end)
    {
        GizmosRuntimeCustom.DrawLine(start, end);
        Vector3 direction = (start - end).normalized;
        
        if (direction != Vector3.zero) 
        {
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            float arrowSize = 0.15f;
            GizmosRuntimeCustom.DrawLine(end, end + direction * arrowSize + right * arrowSize);
            GizmosRuntimeCustom.DrawLine(end, end + direction * arrowSize - right * arrowSize);
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
}