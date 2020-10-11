using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public struct AStarNodeDistPair<T>
    {
        public AStarNodeDistPair(IAStarNode<T> node, float distance)
        {
            this.node = node;
            this.distance = distance;
        }
        public IAStarNode<T> node;
        public float distance;
    }

    public interface IAStarNode<T>
    {
        List<AStarNodeDistPair<T>> GetNeigbors();
        float GetHeuristicDistance(T other);
    }
}