using System;
using UnityEngine;

public class CrowdNode
{
    public static event Action<CrowdNode, GameObject> OnNodeCreated;
    public event Action<CrowdNode, bool> OnActiveStateChanged;
    
    public virtual CrowdNode nextNode { get; private set; }
    
    public Vector3 position;

    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set {
        isActive = value;
        if (nextNode != null)
            nextNode.IsActive = isActive;
        
        OnActiveStateChanged?.Invoke(this, isActive); 
        }
    }

    private bool isStatic = false;

    public CrowdNode(Vector3 position, CrowdNode nextNode, bool isActive, GameObject sourceGO = null)
    {
        if (sourceGO != null) OnNodeCreated?.Invoke(this, sourceGO);
        
        this.position = position;
        this.nextNode = nextNode;
        
        IsActive = isActive;
    }

    public void CheckObstacles()
    {
        if (!IsActive) return;

        if (nextNode != null)
        {
            if (Physics.Linecast(this.position, nextNode.position, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Player.instance.SetCrowdToFollow(nextNode);
                }
            }
            nextNode.CheckObstacles();
        }
    }
    
    public void Connect()
    {
        if (nextNode != null)
            nextNode.IsActive = IsActive;
    }

    public void Disconnect()
    {
        if (nextNode != null)
            nextNode.IsActive = !IsActive;
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
    
    public SwitchCrowdNode(Vector3 position, CrowdNode nextNode, CrowdNode[] nextOriginNodes, bool isActive,  GameObject sourceGO = null) 
        : base(position, nextNode, isActive, sourceGO)
    {
        this.nextOriginNodes = nextOriginNodes;
    }
    
    public void Switch(int nbOfSwitches)
    {
        Disconnect();
        
        int totalStates = nextOriginNodes.Length + 1; //include 0 as null state
        
        // calcul index in range [0 à size]
        int virtualIndex = currentDirectionIndex + 1;
        
        virtualIndex = (virtualIndex + nbOfSwitches) % totalStates;
        
        // convert new index in range [-1 à size-1]
        currentDirectionIndex = virtualIndex - 1;
        
        Connect();
    }
}

public class DynamicCrowdNode : CrowdNode
{
    public DynamicCrowdNode(Vector3 position, CrowdNode nextNode, bool isActive) : base(position, nextNode, isActive)
    {
    }

    public void UpdatePosition()
    {
        
    }
}