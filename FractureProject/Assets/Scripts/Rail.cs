using UnityEngine;
using System.Collections.Generic;

public class Rail : MonoBehaviour
{
    private LayerMask obstacleLayer;
    public Rail nextRail;
    private Transform anchorA;
    private Transform anchorB;
    public GameObject entryNode;
    public GameObject exitNode;

    private void Awake() => obstacleLayer = LayerMask.GetMask("Obstacle");

    private void Update()
    {
        if (nextRail != null)
        {
            CheckForMerge();
        }
        else
        {
            CheckObstacles();
        }
    }

    private void CheckObstacles()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Transform nodeA = transform.GetChild(i);
            Transform nodeB = transform.GetChild(i + 1);

            if (nodeA.name.Contains("Dynamic") || nodeB.name.Contains("Dynamic")) continue;

            if (Physics.Linecast(nodeA.position, nodeB.position, out RaycastHit hitForward, obstacleLayer))
            {
                Physics.Linecast(nodeB.position, nodeA.position, out RaycastHit hitBackward, obstacleLayer);
                Split(i, hitForward.point, hitBackward.point, nodeA, nodeB);
                break; 
            }
        }
    }

    private void Split(int index, Vector3 entryPos, Vector3 exitPos, Transform nodeA, Transform nodeB)
    {
        anchorA = nodeA;
        anchorB = nodeB;

        GameObject newRailGO = new GameObject("SubRail_Split");
        newRailGO.transform.parent = transform.parent;
        nextRail = newRailGO.AddComponent<Rail>();
        newRailGO.AddComponent<RailDebugDisplay>();

        Transform exitT = CreatePoint("DynamicExit", exitPos, newRailGO.transform);
        exitNode = exitT.gameObject;
        nextRail.exitNode = exitNode; 
        
        var dynExit = exitNode.AddComponent<DynamicNode>();

        List<Transform> childrenToMove = new List<Transform>();
        for (int i = index + 1; i < transform.childCount; i++)
        {
            childrenToMove.Add(transform.GetChild(i));
        }

        foreach (Transform t in childrenToMove) t.parent = newRailGO.transform;

        Transform entryT = CreatePoint("DynamicEntry", entryPos, transform);
        entryNode = entryT.gameObject;
        var dynEntry = entryNode.AddComponent<DynamicNode>();

        dynEntry.refStart = anchorA; dynEntry.refEnd = anchorB;
        dynEntry.mask = obstacleLayer; dynEntry.isForward = true;

        dynExit.refStart = anchorA; dynExit.refEnd = anchorB;
        dynExit.mask = obstacleLayer; dynExit.isForward = false;
        
        nextRail.anchorB = anchorB; 
    }

    private void CheckForMerge()
    {
        if (!Physics.Linecast(anchorA.position, anchorB.position, obstacleLayer))
        {
            Merge();
        }
    }

    private void Merge()
    {
        GameObject targetExit = nextRail.exitNode;
        GameObject targetEntry = entryNode;

        List<Transform> pointsToRecover = new List<Transform>();
        foreach (Transform child in nextRail.transform)
        {
            if (child.gameObject != targetExit)
            {
                pointsToRecover.Add(child);
            }
        }

        foreach (Transform t in pointsToRecover)
        {
            t.parent = this.transform;
            t.SetAsLastSibling(); 
        }

        Rail futureRail = nextRail.nextRail;
        
        Destroy(targetExit);
        Destroy(targetEntry);
        Destroy(nextRail.gameObject);
        
        nextRail = futureRail;
    }

    private Transform CreatePoint(string n, Vector3 p, Transform parent)
    {
        GameObject go = new GameObject(n);
        go.transform.parent = parent;
        go.transform.position = p;
        return go.transform;
    }
}