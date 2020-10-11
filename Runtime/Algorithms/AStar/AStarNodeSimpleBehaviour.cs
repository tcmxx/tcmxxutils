using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public class AStarNodeSimpleBehaviour : MonoBehaviour, IAStarNode<AStarNodeSimpleBehaviour>
    {
        [SerializeField]
        protected List<AStarNodeSimpleBehaviour> neighbors;
        List<AStarNodeDistPair<AStarNodeSimpleBehaviour>> neighborNodes;

        private void Awake()
        {
            neighborNodes = new List<AStarNodeDistPair<AStarNodeSimpleBehaviour>>();
            if (neighbors != null)
            {
                for (int i = 0; i < neighbors.Count; ++i)
                {
                    neighborNodes.Add(new AStarNodeDistPair<AStarNodeSimpleBehaviour>(neighbors[i], Vector3.Distance(transform.position, neighbors[i].transform.position)));
                }
            }
        }

        public List<AStarNodeSimpleBehaviour> FindPathTo(AStarNodeSimpleBehaviour goal)
        {
            List<IAStarNode<AStarNodeSimpleBehaviour>> result = AStarSolver<AStarNodeSimpleBehaviour>.SolveSimple(this, goal);

            return result.ConvertAll(o => (AStarNodeSimpleBehaviour)o);
        }

        public void FindPathTo(AStarNodeSimpleBehaviour goal, List<IAStarNode<AStarNodeSimpleBehaviour>> result)
        {
            AStarSolver<AStarNodeSimpleBehaviour>.SolveSimple(this, goal, result);
        }

        public List<AStarNodeDistPair<AStarNodeSimpleBehaviour>> GetNeigbors()
        {
            for (int i = 0; i < neighborNodes.Count; ++i)
            {
                var pair = neighborNodes[i];
                pair.distance = Vector3.Distance(transform.position, neighbors[i].transform.position);
                neighborNodes[i] = pair;
            }

            return neighborNodes;
        }
        public float GetHeuristicDistance(AStarNodeSimpleBehaviour other)
        {
            var casted = (AStarNodeSimpleBehaviour)other;
            return Vector3.Distance(transform.position, casted.transform.position);
        }

    }
}