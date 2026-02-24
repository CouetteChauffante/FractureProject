using Unity.VisualScripting;
using UnityEngine;

public class CrowdNode
{
    public virtual CrowdNode nextNode { get; private set; }
    
    public event System.Action<CrowdNode, bool> OnStateChanged;
    
    public Vector3 position;

    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set {
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
        if (!IsActive) return;

        if (nextNode != null)
        {
            if (Physics.Linecast(this.position, nextNode.position, out RaycastHit hit))
            {
                if (Player.instance.canMove && hit.collider.CompareTag("Player"))
                {
                    Player.instance.FollowCrowd(nextNode);
                }
            }
            nextNode.CheckObstacles();
        }
    }

    private void Split()
    {
        
    }

    private void Merge()
    {
        
    }
    
    public void Connect()
    {
        IsActive = true;
        
        if (nextNode != null) 
            nextNode.Connect();
    }

    public void Disconnect()
    {
        IsActive = false;
        
        if (nextNode != null) 
            nextNode.Disconnect();
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
    
    public SwitchCrowdNode(Vector3 position, CrowdNode nextNode, CrowdNode[] nextOriginNodes, bool isActive) : base(position, nextNode, isActive)
    {
        this.nextOriginNodes = nextOriginNodes;
    }
    
    public void Switch(int nbOfSwitches)
    {
        Debug.Log("ca switch dans node");
        
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