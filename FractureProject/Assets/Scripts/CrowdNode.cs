using Unity.VisualScripting;
using UnityEngine;

public class CrowdNode
{
    public virtual CrowdNode nextNode { get; private set; }
    
    public event System.Action<CrowdNode, bool> OnStateChanged;
    
    
    
    public Vector3 position;
    public bool isActive 
    { 
        get => isActive;
        private set {
        if (isActive == value) return;
        isActive = value;
        OnStateChanged?.Invoke(this, isActive); 
        }
    }
    
    public CrowdNode(Vector3 position, CrowdNode nextNode, bool isActive)
    {
        this.position = position;
        this.isActive = isActive;
        this.nextNode = nextNode;
    }

    public void CheckObstacles()
    {
        
    }

    private void Split()
    {
        
    }

    private void Merge()
    {
        
    }
    
    public void Connect()
    {
        isActive = true;
        nextNode.Connect();
    }

    public void Disconnect()
    {
        isActive = false;
        nextNode.Disconnect();
    }
    
}

public class SwitchCrowdNode : CrowdNode
{
    public CrowdNode[] nextOriginNodes;
    public int currentDirectionIndex;
    
    public override CrowdNode nextNode 
    {
        get 
        {
            if (currentDirectionIndex >= 0 && currentDirectionIndex < nextOriginNodes.Length)
                return nextOriginNodes[currentDirectionIndex];
            
            return base.nextNode;
        }
    }
    
    public SwitchCrowdNode(Vector3 position, CrowdNode nextNode, CrowdNode[] nextOriginNodes, bool isActive) : base(position, nextNode, isActive)
    {
        this.nextOriginNodes = nextOriginNodes;
    }
    
    public void Switch(int nbOfSwitches)
    {
        nextNode.Disconnect();
        
        int totalStates = nextOriginNodes.Length + 1; //include 0 as null state
        
        // calcul index in range [0 à size]
        int virtualIndex = currentDirectionIndex + 1;
        
        virtualIndex = (virtualIndex + nbOfSwitches) % totalStates;
        
        // convert new index in range [-1 à size-1]
        currentDirectionIndex = virtualIndex - 1;
        
        nextNode.Connect();
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