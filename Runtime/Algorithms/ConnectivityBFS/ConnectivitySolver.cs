using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectivitySolver
{


    protected Dictionary<IConnectivityNode, int> allNodes;  //node->group dictionary. negative group means no group now


    protected Dictionary<int, List<IConnectivityNode>> groupDict;  //group-> dictionary.
    protected Dictionary<int, uint> groupPropertyDict;  //group-> dictionary.
    protected int maxGroupNum = 0;
    public int GroupCount { get { return maxGroupNum + 1; } }

    public uint AndPropertyBitsDefaultGroup { get; private set; }

    public List<IConnectivityNode> GetNodes(int groupIndex)
    {
        return groupDict[groupIndex];
    }

    public ConnectivitySolver(List<IConnectivityNode> initialNodes, int initialGroup = 0)
    {
        Debug.Assert(initialGroup >= 0);
        allNodes = new Dictionary<IConnectivityNode, int>();
        groupPropertyDict = new Dictionary<int, uint>();
        groupDict = new Dictionary<int, List<IConnectivityNode>>();
        groupDict[initialGroup] = new List<IConnectivityNode>();
        for (int i = 0; i < initialNodes.Count; ++i)
        {
            allNodes[initialNodes[i]] = initialGroup;
            groupDict[initialGroup].Add(initialNodes[i]);
        }
        AndPropertyBitsDefaultGroup = uint.MaxValue;
    }



    public List<ConnectivitySolver> Split()
    {
        List<ConnectivitySolver> result = new List<ConnectivitySolver>();
        for (int i = 0; i < GroupCount; ++i)
        {
            var newGroup = new ConnectivitySolver(groupDict[i], 0);
            newGroup.AndPropertyBitsDefaultGroup = groupPropertyDict[i];
            result.Add(newGroup);
        }
        return result;
    }

    public void AddWithoutResolving(IConnectivityNode newNode)
    {
        if (!allNodes.ContainsKey(newNode))
            allNodes[newNode] = -1;
    }

    public void Resolve()
    {
        groupDict.Clear();
        List<IConnectivityNode> nodes = new List<IConnectivityNode>(allNodes.Keys);
        Queue<IConnectivityNode> queue = new Queue<IConnectivityNode>();

        for (int i = 0; i < nodes.Count; ++i)
        {
            allNodes[nodes[i]] = -1;    //clear the group data
        }

        maxGroupNum = -1;
        for (int i = 0; i < nodes.Count; ++i)
        {
            if (!nodes[i].IsAvailable())
            {
                allNodes.Remove(nodes[i]);
            }
            else if (allNodes[nodes[i]] < 0)
            {
                maxGroupNum++;
                groupDict[maxGroupNum] = new List<IConnectivityNode>();
                groupPropertyDict[maxGroupNum] = nodes[i].GetAndPropertyBits();

                queue.Clear();
                queue.Enqueue(nodes[i]);
                allNodes[nodes[i]] = maxGroupNum;
                groupDict[maxGroupNum].Add(nodes[i]);
                while (queue.Count > 0)
                {
                    var nodeToExpand = queue.Dequeue();
                    var neibors = nodeToExpand.GetNeighbors();
                    for (int j = 0; j < neibors.Count; ++j)
                    {
                        if (!neibors[j].IsAvailable()) break;
                        int neighborGroup;
                        bool hasData = allNodes.TryGetValue(neibors[j], out neighborGroup);

                        if (!hasData)
                        {
                            //the neibor is not added to the solver before
                            allNodes[neibors[j]] = -1;
                            nodes.Add(neibors[j]);
                            neighborGroup = -1;
                        }

                        Debug.Assert(neighborGroup < 0 || neighborGroup == maxGroupNum, "Wrong assignment of neighbor's group detected: current maxGroupNum:" + maxGroupNum + ", this neighbor's group:" + neighborGroup);

                        if (neighborGroup != maxGroupNum)
                        {
                            //if the group of the neibor has not been assigned yet
                            queue.Enqueue(neibors[j]);
                            allNodes[neibors[j]] = maxGroupNum;
                            groupDict[maxGroupNum].Add(neibors[j]);
                            groupPropertyDict[maxGroupNum] = groupPropertyDict[maxGroupNum] & neibors[j].GetAndPropertyBits();
                        }
                    }
                }

            }
        }
    }
}
