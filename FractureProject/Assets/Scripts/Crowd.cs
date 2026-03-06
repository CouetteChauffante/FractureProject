using UnityEngine;

public class Crowd : MonoBehaviour
{
    public CrowdNode rootNode { get; private set; }
    
    private void Awake()
    {
        rootNode = CreateNewBranch(gameObject.transform, true);
    }

    private void Update()
    {
        rootNode.CheckObstacles();
    }

    private CrowdNode CreateNewBranch(Transform newBranchOrigin, bool isActiveBranch)
    {
        return new CrowdNode(
            newBranchOrigin.position,
            GenerateNodeByChildren(newBranchOrigin, isActiveBranch),
            isActiveBranch,
            
            newBranchOrigin.gameObject
        );
    }
    
    private CrowdNode GenerateNodeByChildren(Transform origin, bool isActiveBranch, int nodeIndex = 0)
    {
        if (nodeIndex >= origin.childCount) return null;
        
        Transform nodeObject = origin.GetChild(nodeIndex);

        if (nodeObject.childCount > 0)
        {
            CrowdNode[] nextOriginNodes = new CrowdNode[nodeObject.childCount];
            for (int i = 0; i < nodeObject.childCount; i++)
                nextOriginNodes[i] = CreateNewBranch(nodeObject.GetChild(i), false);
            
            SwitchCrowdNode newSwitchNode = 
                new SwitchCrowdNode(
                    nodeObject.position, 
                    GenerateNodeByChildren(origin, isActiveBranch, nodeIndex+1), 
                    nextOriginNodes,
                    isActiveBranch,
                    
                    nodeObject.gameObject
                    );
            
            SwitchNodeEvent eventLinked = nodeObject.GetComponent<SwitchNodeEvent>();
            if (eventLinked != null)
            {
                eventLinked.Bind(newSwitchNode);
            }
            
            return newSwitchNode;
        }

        return new CrowdNode(nodeObject.position, GenerateNodeByChildren(origin, isActiveBranch, nodeIndex+1), isActiveBranch,
            nodeObject.gameObject);
    }
    
}
