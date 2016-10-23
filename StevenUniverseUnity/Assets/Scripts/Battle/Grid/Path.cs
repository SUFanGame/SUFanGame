using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace StevenUniverse.FanGame.Battle
{

    public class Path
    {
        // A list of all nodes in the path. This list is not in any particular order.
        List<Node> nodes_ = new List<Node>();
        // Hashset to ensure we don't add a node more than once to our nodes list.
        HashSet<Node> nodeSet_ = new HashSet<Node>();

        /// <summary>
        /// A list of all the nodes in the path, not in any particular order.
        /// To retrieve the specific path to a node in this path call ToPosition()
        /// </summary>
        public IList<Node> NodesReadOnly_ = null;

        // Maps the previous position on the path to the node it leads to
        Dictionary<IntVector3, Node> cameFrom_ = new Dictionary<IntVector3, Node>();

        public Path()
        {
            NodesReadOnly_ = nodes_.AsReadOnly();
        }

        /// <summary>
        /// Add a node to the path, forming a connection from the given position to the given node.
        /// </summary>
        public void AddToPath(IntVector3 fromPos, Node toNode)
        {
            cameFrom_[fromPos] = toNode;
            AddToPath(toNode);
        }

        /// <summary>
        /// Add a single node to our path with no connection information
        /// </summary>
        /// <param name="node"></param>
        public void AddToPath( Node node )
        {
            if( !nodeSet_.Contains(node) )
            {
                nodeSet_.Add(node);
                nodes_.Add(node);
            }
        }

        /// <summary>
        /// Populate the given list of nodes with the ordered path required to reach the given target position.
        /// Note this will reverse the given list, it is assumed to be empty.
        /// </summary>
        /// <param name="path"></param>
        public void ToPosition(IntVector3 target, List<Node> path)
        {
            Node dest;
            cameFrom_.TryGetValue(target, out dest);

            if (dest == null)
                return;

            while( dest != null )
            {
                path.Add(dest);
                cameFrom_.TryGetValue(dest.Pos_, out dest);
            }

            path.Reverse();
        }

        /// <summary>
        /// Clear all current data from this path.
        /// </summary>
        public void Clear()
        {
            cameFrom_.Clear();
            nodes_.Clear();
            nodeSet_.Clear();
        }

    }
}
