using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



namespace StevenUniverse.FanGame.StrategyMap
{
    /// <summary>
    /// A list of nodes, where any node in the path can be passed in to retrieve a path to that node.
    /// Not sure about the name, though...
    /// </summary>
    public class GridPaths
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
        Dictionary<Node, Node> cameFrom_ = new Dictionary<Node, Node>();

        public GridPaths()
        {
            NodesReadOnly_ = nodes_.AsReadOnly();
        }

        /// <summary>
        /// Add a node to the path, forming a connection from the given position to the given node.
        /// </summary>
        public void AddToPath(Node fromNode, Node toNode)
        {
            //Debug.LogFormat("Updating path {0} leads to {1}", fromPos, toNode.Pos_);
            cameFrom_[toNode] = fromNode;
            AddToPath(toNode);
            AddToPath(fromNode);
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
        public void PathPosition(IntVector3 startPos, IntVector3 endPos, List<Node> path, Grid grid)
        {
            var start = grid.GetNode(startPos);
            var end = grid.GetNode(endPos);

            if( !nodeSet_.Contains( start ) )
            {
                Debug.LogFormat("Start pos {0} not found in paths", start);
            }

            if( !nodeSet_.Contains(end) )
            {
                Debug.LogFormat("End pos {0} not found in paths", end);
            }

            path.Add(end);

            Node dest;
            cameFrom_.TryGetValue(end , out dest);

            while( dest != null && dest != start )
            {
                path.Add(dest);
                //Debug.LogFormat("Adding {0} to path", dest.Pos_);
                cameFrom_.TryGetValue(dest, out dest);
            }

            //path.Add( end );

            string pathString = string.Join("->", path.Select(p=>p.Pos_.ToString()).ToArray() );
            //Debug.LogFormat("Path from {0} to {1}", start, end);
            //Debug.LogFormat(pathString);
            path.Reverse();
        }

        /// <summary>
        /// Returns whether or not the given position is a part of this set of paths
        /// </summary>
        public bool Contains( Node node )
        {
            return node != null && nodeSet_.Contains(node);
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
