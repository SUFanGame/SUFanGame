using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Overworld;
using System.Linq;

namespace StevenUniverse.FanGame.Battle
{

    public class Grid : MonoBehaviour
    {
        TileMap<ITile> tileMap_ = null;

        Dictionary<IntVector3, Node> dict_ = new Dictionary<IntVector3, Node>();

        public GameObject pfb_pathSprite_;

        /// <summary>
        /// Populate the given list with the neighbours of the given node.
        /// </summary>
        /// <param name="nodeBuffer"></param>
        public IList<Node> GetNeighbours( IntVector3 pos )
        {
            var current = GetNode(pos);

            if (current == null)
                return null;
            return current.Neighbours_;
        }

        /// <summary>
        /// Retrieve the node at the given position. Returns null if no node is present.
        /// </summary>
        public Node GetNode( IntVector3 pos )
        {
            Node n;
            dict_.TryGetValue(pos, out n);
            return n;
        }


        /// <summary>
        /// Populates the node buffer with all nodes in the given range starting from the given point with the given movement type.
        /// </summary>
        /// <param name="pos">The position to start the search.</param>
        /// <param name="range">The range of the search (in tiles).</param>
        /// <param name="nodeBuffer">All discovered nodes will be added to the buffer.</param>
        /// <param name="movementType">The movement type to be used during the search.</param>
        public void GetNodesInRange( IntVector3 pos, int range, Path path, MovementType movementType = MovementType.GROUNDED)
        {
            // Use dijkstra's to retrieve all nodes in range.
            var current = GetNode(pos);

            if (current == null)
                return;

            // Our frontier will be a priority queue. The cost to reach each node in our path will determine it's
            // priority in the queue.
            var frontier = new MinPriorityQueue<Node>();
            // Maps each node in our path to the node it came from
            var cameFrom = new Dictionary<Node, Node>();
            // Maps each node in our path to the total cost to reach that node from the starting point
            var costSoFar = new Dictionary<Node, int>();

            path.AddToPath(current);
            frontier.Add(current, 0);
            costSoFar[current] = 0;

            while( frontier.Count != 0 )
            {
                current = frontier.Remove();

                var adjNodes = current.Neighbours_;
                if (adjNodes == null)
                    continue;

                for( int i = 0; i < adjNodes.Count; ++i )
                {
                    var next = adjNodes[i];

                    // Get the total cost to move to this node from the start of our path
                    int newCost = costSoFar[current] + next.GetCost(movementType);

                    // Check if the new cost is within our range and if we've visited the tile yet.
                    // If we have visited the tile we only want to consider it if it's a lower cost
                    // alternative on our path.
                    if( newCost <= range && (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) )
                    {
                        // Update our path data
                        costSoFar[next] = newCost;
                        frontier.Add(next, newCost);
                        cameFrom[next] = current;
                        path.AddToPath(current.Pos_, next);
                    }
                }
                
            }
        }

        void OnDrawGizmosSelected()
        {
            if( tileMap_ != null )
            {
                var min = tileMap_.Min;
                var max = tileMap_.Max;

                for (int x = min.x; x <= max.x; ++x)
                {
                    for (int y = min.y; y <= max.y; ++y)
                    {
                        var tileStack = tileMap_.GetTileStack(x, y);
                        if (tileStack != null)
                            Gizmos.DrawWireSphere(new Vector3(x, y, 1f) + Vector3.one * .5f, .5f);
                    }
                }
            }
        }

        public void BuildGrid( TileMap<ITile> tiles )
        {
            tileMap_ = tiles;
            var min = tiles.Min;
            var max = tiles.Max;

            // Run through each cell of the map...
            for( int x = min.x; x < max.x + 1; ++x )
            {
                for( int y = min.y; y < max.y + 1; ++y )
                {
                    // Grab the stack of tiles at this cell
                    var tileStack = tiles.GetTileStack(x, y);

                    if (tileStack == null)
                        continue;

                    // Group each set of tiles by their elevation, so we can treat all tiles at
                    // the same elevation as a separate group
                    var query = tileStack.GroupBy(t => t.Elevation, t => t);

                    // Keep track of grounded tiles. If a tile is grounded it means the space directly below it
                    // (in terms of elevation) is not pathable.
                    bool wasGrounded = false;
                    foreach( var elevationGroup in query )
                    {
                        // The current elevation for this group of tiles in this node
                        int elevation = elevationGroup.Key;

                        // Whether or not this node is pathable.
                        bool pathable = false;
                        // Whether or not this node is collidable.
                        bool collidable = false;

                        // If the previously polled node was grounded, it means the node directly below it is automatically 
                        // unpathable - so skip it.
                        if( wasGrounded )
                        {
                            wasGrounded = false;
                            continue;
                        }

                        // All tiles in this group share the same elevation, and are in descending order (high to low)
                        foreach( var tile in elevationGroup )
                        {
                            var mode = tile.TileModeName;

                            // Ignore "normal" tiles
                            if (mode == "Normal")
                                continue;

                            // A tile is only pathable if previous tiles in this node are NOT collidable
                            if( !collidable && (mode == "Transitional" || mode == "Surface") )
                            {
                                pathable = true;
                            }

                            // If a tile is collidable it blocks any tiles below it in the same elevation from
                            // being pathable. This allows for things like rocks to sit on top of pathable
                            // tiles (at the same elevation), preventing pathability, while a bridge could sit on top of an unpathable
                            // cliff tile (at the same elevation) but still be pathable.
                            if ( mode == "Collidable" )
                            {
                                collidable = true;
                            }

                            // If a tile is grounded it prevents pathability on tiles beneath it ( elevation-wise )
                            if( tile.IsGrounded )
                            {
                                wasGrounded = true;
                            }
                        }

                        // If we reach this point and the node is pathable then this position can officially be walked on.
                        if(pathable)
                        {
                            AddNode(new IntVector3(x, y, elevation));
                        }
                    }
                }
            }

            foreach (var pair in dict_)
            {
                var pos = pair.Value.Pos_;

                var spriteGO = Instantiate(pfb_pathSprite_);
                spriteGO.transform.position = (Vector3)pos;

                var tilesAtPos = tileMap_.GetTilesAtElevation(pos.x, pos.y, pos.z);

                if (tilesAtPos != null && tilesAtPos.Count > 0)
                {
                    var sprite = spriteGO.GetComponentInChildren<SpriteRenderer>();
                    // Grab the topmost tile at this position and get it's sorting order.
                    var topTile = tilesAtPos[0];
                    
                    int sortingOrder = tilesAtPos[0].SortingOrder;
                    // Put our sprite above it on the same layer.
                    sprite.sortingOrder = topTile.Elevation * 100 + sortingOrder + 10;
                    sprite.sortingLayerID = SortingLayer.NameToID("Overworld");
                }
            }
        }
        
        void AddNode( IntVector3 pos )
        {
            dict_.Add(pos, new Node(pos));
        }

    }
}

/*
    Dust on transitioning between elevations:

    The logic for that is basically: if you are on a Surface tile at elevation n, 
    you can go to any adjacent Surface tile at elevation n or any Transitional tile at elevation n or n-1.
    And if you are on a Transitional tile at elevation n, 
    you can go to an adjacent Surface or Transitional tile at either n or n+1(edited)
    Which allows you to move up to non-transitonal tiles
    */
