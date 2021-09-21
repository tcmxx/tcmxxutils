using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using TCUtils;

public class AStarSolver<T> {

    /// <summary>
    /// Solve a Astart search
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="goal"></param>
    /// <param name="resultBuffer">if there is a result buffer, it will not allocate new list but store the result in the result buffer</param>
    /// <returns></returns>
    public static List<IAStarNode<T>> SolveSimple(IAStarNode<T> startNode, IAStarNode<T> goal, List<IAStarNode<T>> resultBuffer = null) {
        Dictionary<IAStarNode<T>, float> heuristics = StaticDictionaryPool<IAStarNode<T>, float>.Get();
        Dictionary<IAStarNode<T>, float> minDist = StaticDictionaryPool<IAStarNode<T>, float>.Get();
        Dictionary<IAStarNode<T>, IAStarNode<T>> prevNode = StaticDictionaryPool<IAStarNode<T>, IAStarNode<T>>.Get();
        SimplePriorityQueue<IAStarNode<T>> queue = StaticSimplePriorityQueuePool<IAStarNode<T>>.Get();

        minDist[startNode] = 0;
        heuristics[startNode] = startNode.GetHeuristicDistance((T)goal);
        bool found = false;
        queue.Enqueue(startNode, heuristics[startNode]);

        while (queue.Count > 0) {
            var node = queue.Dequeue();
            if (node == goal) {
                found = true;
                break;
            }

            var neighbors = node.GetNeigbors();
            for (int i = 0; i < neighbors.Count; ++i) {
                IAStarNode<T> next = neighbors[i].node;
                float newDist = neighbors[i].distance + minDist[node];
                float oldMinDist;
                bool hasMinDist = minDist.TryGetValue(next, out oldMinDist);
                if (!hasMinDist) {
                    oldMinDist = float.MaxValue;
                    minDist[next] = oldMinDist;
                }

                if (newDist < oldMinDist) {
                    minDist[next] = newDist;

                    //get the heuristic of the next node to goal
                    float heur;
                    bool hasHeur = heuristics.TryGetValue(next, out heur);
                    if (!hasHeur) {
                        heur = next.GetHeuristicDistance((T)goal);
                        heuristics[next] = heur;
                    }

                    //enque
                    queue.Enqueue(next, newDist + heur);
                    prevNode[next] = node;
                }
            }

        }

        StaticSimplePriorityQueuePool<IAStarNode<T>>.Release(queue);
        StaticDictionaryPool<IAStarNode<T>, float>.Release(minDist);
        StaticDictionaryPool<IAStarNode<T>, float>.Release(heuristics);
        if (found) {
            List<IAStarNode<T>> result;
            if (resultBuffer != null)
                result = resultBuffer;
            else
                result = new List<IAStarNode<T>>();
            result.Clear();

            result.Add(goal);
            IAStarNode<T> lastAdded = goal;
            while (lastAdded != startNode) {
                lastAdded = prevNode[lastAdded];
                result.Add(lastAdded);
            }

            result.Reverse();
            StaticDictionaryPool<IAStarNode<T>, IAStarNode<T>>.Release(prevNode);
            return result;
        } else {
            StaticDictionaryPool<IAStarNode<T>, IAStarNode<T>>.Release(prevNode);
            return null;
        }
    }
}