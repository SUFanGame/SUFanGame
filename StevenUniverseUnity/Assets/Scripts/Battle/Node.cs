using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace StevenUniverse.FanGame.Battle
{
    /// <summary>
    /// A node in the strategy graph.
    /// </summary>
    public class Node
    {

        List<Node> neighbours_ = null;
        // Read only list of neighbours.
        IList<Node> neighboursReadOnly_ = null;
        /// <summary>
        /// The neighbours of this node. This list is read only.
        /// </summary>
        public IList<Node> Neighbours_ { get { return neighboursReadOnly_; } }

        public Node( )
        {
        }

        public void FormConnection( Node other )
        {
            if (neighbours_ == null)
            {
                neighbours_ = new List<Node>();
                neighboursReadOnly_ = neighbours_.AsReadOnly();
            }

            neighbours_.Add(other);

            if (other.neighbours_ == null)
            {
                other.neighbours_ = new List<Node>();
                other.neighboursReadOnly_ = other.neighbours_.AsReadOnly();
            }

            other.neighbours_.Add(this);
        }

        public void SeverConnection(  Node other )
        {
            neighbours_.Remove(other);

            other.neighbours_.Remove(this);
        }

        /// <summary>
        /// Returns the cost to move into this node for the given movement type
        /// </summary>
        /// <param name="movementType"></param>
        /// <returns></returns>
        public int GetCost( MovementType movementType )
        {
            return 0;
        }

    }
}
