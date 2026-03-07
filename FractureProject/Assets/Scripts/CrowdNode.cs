using System;
using UnityEngine;

public class CrowdNode
{
    public virtual CrowdNode nextNode { get; private set; }
    
    public Vector3 position;

    public CrowdNode(Vector3 position, CrowdNode nextNode)
    {
        this.position = position;
        this.nextNode = nextNode;
    }
    
    public bool IsPathValid()
    {
        if (this is ExitCrowdNode) return true;

        if (nextNode == null) return false;

        return nextNode.IsPathValid();
    }

    public void CheckObstacles()
    {
        if (nextNode == null) return;

        if (Physics.Linecast(this.position, nextNode.position, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (IsPathValid())
                {
                    Player.instance.SetCrowdToFollow(nextNode);
                }
                else
                {
                    Player.instance.BlockByCrowd();
                }
            }
        }
        
        nextNode.CheckObstacles();
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
    
    public SwitchCrowdNode(Vector3 position, CrowdNode nextNode, CrowdNode[] nextOriginNodes) 
        : base(position, nextNode)
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
    public DynamicCrowdNode(Vector3 position, CrowdNode nextNode) : base(position, nextNode)
    {
    }

    public void UpdatePosition()
    {
        
    }
}

public class ExitCrowdNode : CrowdNode
{
    public ExitCrowdNode(Vector3 position, CrowdNode nextNode) 
        : base(position, nextNode) { }
}

public class StopCrowdNode : CrowdNode
{
    public override CrowdNode nextNode => isStopped ? null : base.nextNode;

    public bool isStopped = false;
    
    public StopCrowdNode(Vector3 position, CrowdNode nextNode) 
        : base(position, nextNode) { }
}