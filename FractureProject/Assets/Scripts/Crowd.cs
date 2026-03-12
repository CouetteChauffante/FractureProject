using System;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    public CrowdNode rootNode { get; private set; }

    public HashSet<CrowdNode> allNodes = new HashSet<CrowdNode>();
    
    private void Awake()
    {
        rootNode = CreateNewBranch(gameObject.transform);
    }

    private void Start()
    {
        RefreshCrowdStates();
    }

    private void Update()
    {
        foreach (var node in allNodes)
        {
            node.CheckObstacles();
        }
    }

    private CrowdNode CreateNewBranch(Transform newBranchOrigin)
    {
        if (newBranchOrigin.childCount == 0)
        {
            return new ExitCrowdNode(newBranchOrigin.position, null, allNodes);
        }
        
        return new CrowdNode(
            newBranchOrigin.position,
            GenerateNodeByChildren(newBranchOrigin),
            allNodes
        );
    }
    
    private CrowdNode GenerateNodeByChildren(Transform origin, int nodeIndex = 0)
    {
        if (nodeIndex >= origin.childCount) return null;
        
        Transform nodeObject = origin.GetChild(nodeIndex);

        if (nodeObject.childCount > 0)
        {
            CrowdNode[] nextOriginNodes = new CrowdNode[nodeObject.childCount];
            for (int i = 0; i < nodeObject.childCount; i++)
                nextOriginNodes[i] = CreateNewBranch(nodeObject.GetChild(i));
            
            SwitchCrowdNode newSwitchNode = 
                new SwitchCrowdNode(
                    nodeObject.position, 
                    GenerateNodeByChildren(origin, nodeIndex+1), 
                    nextOriginNodes,
                    allNodes
                    );
            
            SwitchNodeEvent eventLinked = nodeObject.GetComponent<SwitchNodeEvent>();
            if (eventLinked != null)
            {
                eventLinked.Bind(newSwitchNode, this);
            }
            
            return newSwitchNode;
        }
        
        if (nodeIndex == origin.childCount - 1) {
            return new ExitCrowdNode(nodeObject.position, null, allNodes);
        }
        
        StopNodeEvent stopEvent = nodeObject.GetComponent<StopNodeEvent>();
        if (stopEvent != null)
        {
            StopCrowdNode stopNode = new StopCrowdNode(
                nodeObject.position, 
                GenerateNodeByChildren(origin, nodeIndex + 1),
                allNodes
            );
        
            stopEvent.Bind(stopNode, this);
            return stopNode;
        }

        return new CrowdNode(nodeObject.position, GenerateNodeByChildren(origin, nodeIndex+1), allNodes);
    }
    
    
    public void RefreshCrowdStates()
    {
        foreach (var node in allNodes) node.isConnectedToSource = false;

        CrowdNode current = rootNode;
    
        while (current != null)
        {
            current.isConnectedToSource = true;
            current = current.nextNode;
        }

        foreach (var node in allNodes)
        {
            bool hasSource = node.isConnectedToSource;
            bool hasExit = node.IsPathValid();

            if (hasSource && hasExit) 
            {
                node.state = CrowdState.Flowing;
            }
            else if (hasSource && !hasExit) 
            {
                node.state = CrowdState.Stagnant;
            }
            else if (!hasSource && hasExit) 
            {
                node.state = CrowdState.Empty; 
            }
            else
            {
                if (node.state == CrowdState.Flowing || node.state == CrowdState.Stagnant)
                {
                    node.state = CrowdState.Stagnant; 
                }
            }
        }
    }
    
}
