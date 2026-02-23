using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    public CrowdNode rootNode { get; private set; }
    
    private void Awake()
    {
        rootNode = CreateNewBranch(gameObject.transform);
        if (debugMode) InitDebugger(rootNode);
    }

    private CrowdNode CreateNewBranch(Transform newBranchOrigin)
    {
        return new CrowdNode(
            newBranchOrigin.position,
            GenerateNodeByChildren(newBranchOrigin),
            newBranchOrigin.gameObject.activeSelf
        );
    }
    
    private CrowdNode GenerateNodeByChildren(Transform origin, int nodeIndex = 0)
    {
        if (nodeIndex >= origin.childCount) return null;
        
        Transform nodeObject = origin.GetChild(nodeIndex);
        bool isNodeActive = nodeObject.gameObject.activeSelf;

        if (nodeObject.childCount > 0)
        {
            CrowdNode[] nextOriginNodes = new CrowdNode[nodeObject.childCount];
            for (int i = 0; i < nodeObject.childCount; i++)
                nextOriginNodes[i] = CreateNewBranch(nodeObject.GetChild(i));
            
            return new SwitchCrowdNode(nodeObject.position, GenerateNodeByChildren(origin, nodeIndex+1), nextOriginNodes, isNodeActive);
        }
            
        return new CrowdNode(nodeObject.position, GenerateNodeByChildren(origin, nodeIndex+1), isNodeActive);
    }

    #region DebugManager

    [SerializeField] private bool debugMode;
    private List<CrowdNode> nodes = new List<CrowdNode>();

    private void InitDebugger(CrowdNode startingNode)
    {
        CrowdNode currentNode = startingNode;
        while (currentNode != null)
        {
            nodes.Add(currentNode);

            if (currentNode is SwitchCrowdNode switchCrowdNode)
            {
                foreach (CrowdNode node in switchCrowdNode.nextOriginNodes)
                {
                    InitDebugger(node);
                }
            }
            
            currentNode = currentNode.nextNode;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (CrowdNode node in nodes)
        {
            node.DebugNode();
            node.DebugSegment();
        }
    }

    #endregion
}
