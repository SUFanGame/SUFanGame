using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.StrategyMap
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
        
        public IntVector3 Pos_ { get; private set; }
        public PathType PathType_ { get; private set; }

        List<MonoBehaviour> objects_ = null;
        /// <summary>
        /// List of any gameobjects in the node, for things like characters/chests/doors/etc
        /// </summary>
        public List<MonoBehaviour> Objects
        {
            get
            {
                return objects_;
            }
        }

        public void AddObject( MonoBehaviour obj )
        {
            if (objects_ == null)
                objects_ = new List<MonoBehaviour>();

            //Debug.LogFormat("Adding {0} at {1}", obj.name, Pos_);
            objects_.Add(obj);
        }

        public void RemoveObject( MonoBehaviour obj )
        {
            if (objects_ == null)
                return;

            objects_.Remove(obj);
        }

        public Node( IntVector3 pos, PathType pathType )
        {
            Pos_ = pos;
            PathType_ = pathType;
        }

        public void FormConnection( Node other )
        {
            if (neighbours_ == null)
            {
                neighbours_ = new List<Node>();
                neighboursReadOnly_ = neighbours_.AsReadOnly();
            }

            
            neighbours_.Add(other);

            //Debug.LogFormat("Forming connection from {0} to {1}", Pos_, other.Pos_);

            //if (other.neighbours_ == null)
            //{
            //    other.neighbours_ = new List<Node>();
            //    other.neighboursReadOnly_ = other.neighbours_.AsReadOnly();
            //}

            //other.neighbours_.Add(this);
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
            return 1;
        }

        public enum PathType
        {
            /// <summary>
            /// From a surface tiles you can only move to surface tiles at the same height or transitional ties
            /// at the same height or 1 lower.
            /// </summary>
            Surface,
            /// <summary>
            /// For transitional tiles you can only move to surface or transitional tile at the same height
            /// or 1 higher
            /// </summary>
            Transitional
        }

        public override string ToString()
        {
            return string.Format("{0}", Pos_);
        }
    }
}
