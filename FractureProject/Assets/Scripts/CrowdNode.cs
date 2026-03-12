using System;
using System.Collections.Generic;
using UnityEngine;


public enum CrowdState 
{ 
    Empty,
    Flowing,
    Stagnant
}

public class CrowdNode
{
    public virtual CrowdNode nextNode { get; private set; }
    public Vector3 position;
    
    public CrowdState state = CrowdState.Empty;
    public bool isConnectedToSource = false;

    public CrowdNode(Vector3 position, CrowdNode nextNode, HashSet<CrowdNode> track = null)
    {
        this.position = position;
        this.nextNode = nextNode;
        
        track?.Add(this);
    }
    
    public bool IsPathValid()
    {
        if (this is ExitCrowdNode) return true;
        if (nextNode == null) return false;
        return nextNode.IsPathValid();
    }
    
    public void CheckObstacles()
    {
        if (nextNode == null || state == CrowdState.Empty) return;

        if (Physics.Linecast(this.position, nextNode.position, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (state == CrowdState.Flowing)
                {
                    Player.instance.SetCrowdToFollow(nextNode);
                }
                else if (state == CrowdState.Stagnant)
                {
                    Player.instance.BlockByCrowd();
                }
            }
        }
    }
}

public class SwitchCrowdNode : CrowdNode
{
    public CrowdNode[] nextOriginNodes;
    public int currentDirectionIndex = -1;
    
    public override CrowdNode nextNode 
    {
        get 
        {
            if (currentDirectionIndex >= 0 && currentDirectionIndex < nextOriginNodes.Length)
                return nextOriginNodes[currentDirectionIndex];
            
            return base.nextNode;
        }
    }
    
    public SwitchCrowdNode(Vector3 position, CrowdNode nextNode, CrowdNode[] nextOriginNodes, HashSet<CrowdNode> track = null) 
        : base(position, nextNode, track)
    {
        this.nextOriginNodes = nextOriginNodes;
    }
    
    public void Switch(int nbOfSwitches)
    {
        int totalStates = nextOriginNodes.Length + 1; //include 0 as null state
        
        // calcul index in range [0 à size]
        int virtualIndex = currentDirectionIndex + 1;
        
        virtualIndex = (virtualIndex + nbOfSwitches) % totalStates;
        
        // convert new index in range [-1 à size-1]
        currentDirectionIndex = virtualIndex - 1;
    }
}

public class DynamicCrowdNode : CrowdNode
{
    public DynamicCrowdNode(Vector3 position, CrowdNode nextNode, HashSet<CrowdNode> track = null) 
        : base(position, nextNode, track)
    {
    }

    public void UpdatePosition()
    {
        
    }
}

public class ExitCrowdNode : CrowdNode
{
    public ExitCrowdNode(Vector3 position, CrowdNode nextNode, HashSet<CrowdNode> track = null) 
        : base(position, nextNode, track) { }
}

public class StopCrowdNode : CrowdNode
{
    public override CrowdNode nextNode => isStopped ? null : base.nextNode;

    public bool isStopped = false;
    
    public StopCrowdNode(Vector3 position, CrowdNode nextNode, HashSet<CrowdNode> track = null) 
        : base(position, nextNode, track) { }
}