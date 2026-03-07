using UnityEngine;

public class Crowd : MonoBehaviour
{
    public CrowdNode rootNode { get; private set; }
    
    private void Awake()
    {
        rootNode = CreateNewBranch(gameObject.transform);
    }

    private void Update()
    {
        rootNode.CheckObstacles();
    }

    private CrowdNode CreateNewBranch(Transform newBranchOrigin)
    {
        if (newBranchOrigin.childCount == 0)
        {
            return new ExitCrowdNode(newBranchOrigin.position, null);
        }
        
        return new CrowdNode(
            newBranchOrigin.position,
            GenerateNodeByChildren(newBranchOrigin)
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
                    nextOriginNodes
                    );
            
            SwitchNodeEvent eventLinked = nodeObject.GetComponent<SwitchNodeEvent>();
            if (eventLinked != null)
            {
                eventLinked.Bind(newSwitchNode);
            }
            
            return newSwitchNode;
        }
        
        if (nodeIndex == origin.childCount - 1) {
            return new ExitCrowdNode(nodeObject.position, null);
        }
        
        StopNodeEvent stopEvent = nodeObject.GetComponent<StopNodeEvent>();
        if (stopEvent != null)
        {
            StopCrowdNode stopNode = new StopCrowdNode(
                nodeObject.position, 
                GenerateNodeByChildren(origin, nodeIndex + 1)
            );
        
            stopEvent.Bind(stopNode);
            return stopNode;
        }

        return new CrowdNode(nodeObject.position, GenerateNodeByChildren(origin, nodeIndex+1));
    }
    
}
