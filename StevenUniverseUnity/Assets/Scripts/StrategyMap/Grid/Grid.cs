using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.Util.Collections;
using StevenUniverse.FanGame.Overworld;
using System.Linq;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.Factions;
using UnityEngine.EventSystems;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.World;

namespace StevenUniverse.FanGame.StrategyMap
{
    // TODO: Cache and reuse collections in pathfinding funcs

        [ExecuteInEditMode]
    public class Grid : MonoBehaviour, IEnumerable<Node>
    {
        // Dictionary mapping nodes to their 3D position ( x, y, elevation )
        //[HideInInspector]
        Dictionary<IntVector3, Node> nodeDict_ = new Dictionary<IntVector3, Node>();

        // Dictionary mapping each 2D position to the highest walkable node in that position.
        //[HideInInspector]
        Dictionary<IntVector2, int> heightMap_ = new Dictionary<IntVector2, int>();

        // List of any objects added to the map.
        [SerializeField]
        //[HideInInspector]
        List<MonoBehaviour> objects_ = new List<MonoBehaviour>();

        /// <summary>
        /// The current size of the map in tiles.
        /// </summary>
        public IntVector3 Size { get; private set; }

        public int NodeCount_ { get { return nodeDict_.Count; } }

        public static Grid Instance { get; private set; }

        /// <summary>
        /// Callback for when the grid is done building from loaded chunks.
        /// </summary>
        public System.Action<Grid> OnGridBuilt_;

        public System.Action<Node> OnNodeClicked_;

        void Awake()
        {
            Instance = this;
            //selection_ = GetComponent<GridSelectionBehaviour>();
            // Register to receieve user clicks on the grid.
            //selection_.OnClicked_ += HandleClick;
        }

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
        /// Retrieve the highest pathable node at the given 2D position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Node GetHighestNode( IntVector2 pos )
        {
            int h = GetHeight(pos);
            if (h == int.MinValue)
                return null;
            return GetNode(new IntVector3(pos.x, pos.y, h));
        }

        /// <summary>
        /// Retrieve the node at the given position. Returns null if no node is present.
        /// </summary>
        public Node GetNode( IntVector3 pos )
        {
            Node n;
            nodeDict_.TryGetValue(pos, out n);
            return n;
        }

        /// <summary>
        /// Retrieve the highest walkable node position for the given cell. Returns int.MinValue if the given index is out of range.
        /// </summary>
        public int GetHeight( IntVector2 pos )
        {
            int height;
            if (!heightMap_.TryGetValue(pos, out height))
                return int.MinValue;
            return height;
        }


        /// <summary>
        /// Populate the given buffer with any objects of type t that are at the given position.
        /// </summary>
        /// <param name="buffer">Buffer of T, to be populated if any exist in at the given position.</param>
        /// <param name="predicate">Predicate against which discovered objects will be matched. If they
        /// return true from the predicate, then they will be added to the buffer.</param>
        public void GetObjects<T>(IntVector3 pos, List<T> buffer, System.Predicate<T> predicate = null ) where T : MonoBehaviour
        {
            var node = GetNode(pos);

            if (node == null)
                return;

            var list = node.Objects;

            if( list != null )
            {
                for( int i = 0; i < list.Count; ++i )
                {
                    var t = list[i] as T;

                    if (t == null)
                        continue;

                    // If a predicate was passed check the object against it
                    if (predicate != null && !predicate.Invoke(t))
                        continue;

                    buffer.Add(t);
                }
            }
        }

        /// <summary>
        /// Gets all objects of type T in range of pos. A predicate can optionally be specified to filter results.
        /// </summary>
        /// <typeparam name="T">A MonoBehaviour being searched for.</typeparam>
        /// <param name="pos">The position to search from.</param>
        /// <param name="range">The range from the position to consider.</param>
        /// <param name="buffer">The buffer to be populated with results.</param>
        /// <param name="predicate">Optional predicate to filter results.</param>
        public void GetObjectsInArea<T>( IntVector2 pos, int range, List<T> buffer, System.Predicate<T> predicate = null ) where T : MonoBehaviour
        {
            // Could also search each cell in a diamond-shape starting from the position for low-range searches.
            // For any search beyond a range of 2 this is probably going to be faster.
            for( int i = 0; i < objects_.Count; ++i )
            {
                var t = objects_[i] as T;

                // Check if the object is one we're actually concerned with.
                if (t == null || (predicate != null && !predicate.Invoke(t) ) )
                    continue;
                
                // Check if the object is in range.
                IntVector2 objPos = (IntVector2)t.transform.position;
                if (IntVector2.ManhattanDistance(pos, objPos) <= range)
                    buffer.Add(t);
            }
        }

        public void AddObject( IntVector3 pos, MonoBehaviour obj )
        {
            var node = GetNode(pos);

            if (node == null)
                Debug.LogErrorFormat("Attempting to add object {0} to grid at {1}, but there's no node there", obj.name, pos);

            node.AddObject(obj);
            objects_.Add(obj);
        }

        public void RemoveObject( IntVector3 pos, MonoBehaviour obj )
        {
            var node = GetNode(pos);

            if (node == null)
                Debug.LogErrorFormat("Attempting to remove object {0} to grid at {1}, but there's no node there", obj.name, pos);

            node.RemoveObject(obj);
            objects_.Remove(obj);
        }

        public void MoveObject( IntVector3 oldPos, IntVector3 newPos, MonoBehaviour obj )
        {
            var oldNode = GetNode(oldPos);
            var newNode = GetNode(newPos);

            if (oldNode == null)
                Debug.LogErrorFormat("Attempting to remove object {0} to grid at {1}, but there's no node there", obj.name, oldPos );

            if (oldNode == null)
                Debug.LogErrorFormat("Attempting to remove object {0} to grid at {1}, but there's no node there", obj.name, newPos );

            oldNode.RemoveObject(obj);
            newNode.AddObject(obj);
        }

        /// <summary>
        /// Populates the path buffer with all nodes in the given range starting from the given point with the given movement type.
        /// The path object can then be used to retrieve paths to specific nodes.
        /// </summary>
        /// <param name="pos">The position to start the search.</param>
        /// <param name="range">The range of the search (in tiles).</param>
        /// <param name="nodeBuffer">All discovered nodes will be added to the buffer.</param>
        /// <param name="movementType">Movement type determines which nodes are reachable, IE: A node might cost more to
        /// move into for a ground unit as opposed to a floating unit.</param>
        /// <param name="predicate">Optional predicate to filter nodes based on some condition.
        /// IE: Could be used to ignore nodes with enemy characters in them.</param>
        public void GetNodesInRange( 
            IntVector3 pos, 
            int range, 
            GridPaths path,
            MovementType movementType,
            System.Predicate<Node> predicate = null )
        {
            // Use dijkstra's to retrieve all nodes in range.
            var current = GetNode(pos);

            if (current == null || (predicate != null && !predicate(current)) )
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

                    // Ignore the node if our predicate returns false
                    if (predicate != null && !predicate(next))
                        continue;

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
                        path.AddToPath(current, next);
                    }
                }
                
            }
        }

        /// <summary>
        /// Estimate of the cost to reach b from a.
        /// </summary>
        int Hueristic( IntVector2 a, IntVector2 b )
        {
            // Note this is a really shitty hueristic for 3D space,
            // but probably fine for our uses.
            return IntVector2.ManhattanDistance(a, b);
        }

        /// <summary>
        /// Gets the list of nodes along the path from a to b and populates the given buffer with them.
        /// </summary>
        /// <param name="start">Start position.</param>
        /// <param name="end">End position.</param>
        /// <param name="buffer">Buffer to hold the list of nodse. Note this list will be reversed,
        /// it's assumed to be empty.</param>
        /// <param name="predicate">Optional predicate to filter nodes based on some condition.
        /// IE: Could be used to ignore nodes with enemy characters in them.</param>
        public void GetPath( 
            IntVector3 start, 
            IntVector3 end, 
            List<Node> buffer, 
            MovementType movementType,
            System.Predicate<Node> predicate = null )
        {
            var startNode = GetNode(start);
            var endNode = GetNode(end);

            if (startNode == null )
                Debug.LogErrorFormat("Attempting to find path from {0} to {1}, {0} is not a valid node.", start, end );
            if (endNode == null)
                Debug.LogErrorFormat("Attempting to find a path from {0} to {1}, {1} is not a valid node", start, end);

            var frontier = new MinPriorityQueue<Node>();
            var cameFrom = new Dictionary<Node, Node>();
            var costSoFar = new Dictionary<Node, int>();

            cameFrom[startNode] = null;
            costSoFar[startNode] = 0;

            frontier.Add(startNode);

            while( frontier.Count != 0 )
            {
                var current = frontier.Remove();

                var adjNodes = current.Neighbours_;

                if (adjNodes == null)
                    continue;
                
                for( int i = 0; i < adjNodes.Count; ++i )
                {
                    var next = adjNodes[i];
                    int newCost = costSoFar[current] + next.GetCost(movementType);
                    // Check if our next node has already been visited - if so we
                    // check if it's a cheaper alternative on our path
                    if( !costSoFar.ContainsKey(next) || newCost < costSoFar[next] )
                    {
                        costSoFar[next] = newCost;
                        frontier.Add(next, newCost + Hueristic((IntVector2)next.Pos_, (IntVector2)end));
                        cameFrom[next] = current;
                    }
                }
                
            }

            var pNode = endNode;

            while( pNode != null )
            {
                buffer.Add(pNode);
                cameFrom.TryGetValue(pNode, out pNode);
            }

            buffer.Reverse();
        }

        /// <summary>
        /// Builds the grid from the given tile map and forms node connections for ground movement.
        /// </summary>
        public IEnumerator BuildFromTileMap( TileMap<ITile> tiles )
        {
            //tileMap_ = tiles;
            var min = tiles.Min;
            var max = tiles.Max;

            // How long should the function work before yielding if it hasn't yet finished?
            float workTime = .1f;
            float startTime = Time.realtimeSinceStartup;

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
                        // Whether or not this node is transitional
                        bool transitional = false;

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
                            TileTemplate.Mode mode = tile.TileMode;

                            // Ignore "normal" tiles
                            if (mode == TileTemplate.Mode.Normal)
                                continue;

                            // A tile is only pathable if previous tiles in this node are NOT collidable
                            if( !collidable && (mode == TileTemplate.Mode.Transitional || mode == TileTemplate.Mode.Surface) )
                            {
                                if (mode == TileTemplate.Mode.Transitional)
                                    transitional = true;
                                pathable = true;
                            }

                            // If a tile is collidable it blocks any tiles below it in the same elevation from
                            // being pathable. This allows for things like rocks to sit on top of pathable
                            // tiles (at the same elevation), preventing pathability, while a bridge could sit on top of an unpathable
                            // cliff tile (at the same elevation) but still be pathable.
                            if ( mode == TileTemplate.Mode.Collidable)
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
                            var pos = new IntVector3(x, y, elevation);
                            Node.PathType pathType = transitional ? Node.PathType.Transitional : Node.PathType.Surface;
                            nodeDict_.Add(pos, new Node(pos, pathType));

                            IntVector2 pos2D = new IntVector2(x, y);

                            // Populate our height map
                            int existingHeight;
                            if (!heightMap_.TryGetValue(pos2D, out existingHeight))
                                heightMap_[pos2D] = elevation;
                            else
                            {
                                heightMap_[pos2D] = Mathf.Max(elevation, existingHeight);
                            }
                        }

                        // Yield if we've exceeded our work time.
                        if( Time.realtimeSinceStartup - startTime >= workTime )
                        {
                            startTime = Time.realtimeSinceStartup;
                            yield return null;
                        }
                    }

                }
            }
            yield return null;

            FormConnections();

            Size = tiles.Size;

            if (OnGridBuilt_ != null)
                OnGridBuilt_(this);

        }


        // Notes about my previous take on building the grid from Dust's tile maps.
        //It iterates through each 2D (so no z value in iteration) position and grabs the entire stack of tiles
        // at that position. The stacks are pre-sorted such that tiles are grouped by elevation first, then by "Sorting Order" (Sorting Layer index) (Higher first in both cases)

        // The stacks are iterated highest-first. If the previous (one elevation higher) tile in the stack was found to be "Grounded" then all tiles at the current
        // elevation are skipped since it's an unwalkable tile.

        // Then tiles are iterated with a single cell (Iterating through the sorting layers). It's important to iterate from the top down since
        // pathability is dependant on the topmost tile. At this point the tile's "Mode" is examined. "Normal" tiles are skipped,
        // Top "Collidable" tiles prevent pathability in that cell. Top "Surface" or "Transitional" mean
        // the cell is pathable. "Grounded" tiles can be below other pathable or collidable tiles so  all tiles must be iterated though.

        // Figure out if this will work? Do we need to iterate through each stack of tiles on in each chunk in the stack? If so that's not too hard via chunkstacks
        public void BuildFromMap( Map map )
        {

            nodeDict_.Clear();
            heightMap_.Clear();
            //Debug.Log("BuildingMap");
            // We'll iterate through each chunkstack ( which are divided by chunk index )
            // Chunkstacks are ordered from highest to lowest.
            var chunkStackEnumerator = map.GetChunkStackEnumerator();
            while( chunkStackEnumerator.MoveNext() )
            {
                // Within each chunk in the stack we'll iterate through all tile stacks
                // until we find one we can process. If we build a node at any cell then
                // we'll consider that cell finished? <-- Doesn't work since we need to account for tiles "behind" other tiles
                foreach ( var chunk in chunkStackEnumerator.Current.Value )
                {
                    var tileStackEnumerator = chunk.GetTileStackEnumerator();
                   
                    while( tileStackEnumerator.MoveNext() )
                    {
                        var localPos = tileStackEnumerator.Current.Key;
                        var worldPos = (IntVector3)localPos + chunk.GridPosition_;
                        var tileStack = tileStackEnumerator.Current.Value;
                        //var tileStack = chunk.GetTileStackLocal(localPos);

                        bool pathable = false;
                        bool collidable = false;
                        bool grounded = false;
                        bool transitional = false;

                        //if (tileStack == null)
                        //    continue;

                        //var stack = chunk.GetTileStackLocal(localPos);


                        //if (stack == null)
                        //{
                        //    Debug.Log("Unable to retrieve stack explicitly locally");
                        //}
                        ////else
                        ////{
                        ////    Debug.Log("Was able to retrieve stack explicitly");
                        ////}

                        //if (tileStack == null)
                        //{
                        //    Debug.LogFormat("Tile stack at worldpos {0}, localPos {1} in chunk {2} was null...", worldPos, localPos, chunk.name);
                        //}



                        //foreach ( var tile in tileStack )
                        for (int i = tileStack.Count - 1; i >= 0; --i)
                        {
                            var tile = tileStack[i];
                            // Ignore "normal" tiles
                            if (tile == null || tile.Mode_ == Tile.Mode.Normal)
                                continue;

                            // If this node is still not collidable and this tile is pathable it means this node is pathable unless
                            // a node above us is grounded.
                            if (!collidable && (tile.Mode_ == Tile.Mode.Surface || tile.Mode_ == Tile.Mode.Transitional))
                            {
                                var posAbove = worldPos + new IntVector3(0, 0, 1);
                                // Check if the tile node above us is "Grounded"
                                Node nodeAbove;
                                if (nodeDict_.TryGetValue(posAbove, out nodeAbove))
                                {
                                    // If the node above this node is grounded it means this one is unpathable
                                    if (nodeAbove.IsGrounded_)
                                    {
                                        collidable = true;
                                    }
                                }

                                if (!collidable)
                                {
                                    transitional = tile.Mode_ == Tile.Mode.Transitional;
                                    pathable = true;
                                }
                            }

                            if (tile.Mode_ == Tile.Mode.Collidable)
                            {
                                collidable = true;
                            }

                            if (tile.IsGrounded_)
                                grounded = true;
                        }

                        if (!pathable)
                            continue;

                        var node = new Node(worldPos, transitional ? Node.PathType.Transitional : Node.PathType.Surface, grounded);
                        nodeDict_.Add(worldPos, node);

                        IntVector2 pos2D = (IntVector2)worldPos;

                        // Populate our height map
                        int existingHeight;
                        if (!heightMap_.TryGetValue(pos2D, out existingHeight))
                            heightMap_[pos2D] = chunk.Height_;
                        else
                        {
                            heightMap_[pos2D] = Mathf.Max(chunk.Height_, existingHeight);
                        }
                    }

 
                }
            }
        }

        // Form connections between adjacent nodes. Right now this only accounts
        // for ground units. If units can fly we'll need to modify things a bit - push
        // connection forming out to when pathfinding is actually called or model a second set of node adjacency data
        // to account for flying units
        void FormConnections()
        {

            foreach (var pair in nodeDict_)
            {
                var node = pair.Value;
                var pos = node.Pos_;

                foreach (var dir in Directions2D.Quadrilateral)
                {
                    var adj = pos + dir;

                    /// <summary>
                    /// From a surface tiles you can only move to surface tiles at the same height or transitional ties
                    /// at the same height or 1 lower.
                    /// </summary>
                    if (node.PathType_ == Node.PathType.Surface)
                    {
                        // Check for adjacent node at the same height.
                        var adjNode = GetNode(adj);
                        if (adjNode != null)
                            node.FormConnection(adjNode);

                        // Check for adjacent transitional node one below
                        adjNode = GetNode(adj + new IntVector3(0, 0, -1));
                        if (adjNode != null && adjNode.PathType_ == Node.PathType.Transitional)
                        {
                            node.FormConnection(adjNode);
                        }
                        adjNode = GetNode(adj + new IntVector3(0, 0, 1));
                        if (adjNode != null && adjNode.PathType_ == Node.PathType.Transitional)
                        {
                            node.FormConnection(adjNode);
                        }
                    }

                    /// <summary>
                    /// For transitional tiles you can only move to surface or transitional tile at the same height
                    /// or 1 higher
                    /// </summary>
                    if (node.PathType_ == Node.PathType.Transitional)
                    {
                        // Check for adjacent node at the same height.
                        var adjNode = GetNode(adj);
                        if (adjNode != null)
                            node.FormConnection(adjNode);

                        // Check for adjacent node one above
                        adjNode = GetNode(adj + new IntVector3(0, 0, 1));
                        if (adjNode != null)
                            node.FormConnection(adjNode);
                        //// Check for adjacent node one below
                        adjNode = GetNode(adj + new IntVector3(0, 0, -1));
                        if (adjNode != null)
                            node.FormConnection(adjNode);
                    }
                }
            }

        }

        /// <summary>
        /// Retrieve nodes from user clicks and forward click event to listeners.
        /// </summary>
        void HandleClick( PointerEventData data )
        {
            if (OnNodeClicked_ == null)
                return;

            var pos = (IntVector2)data.pointerCurrentRaycast.worldPosition;

            var node = GetHighestNode(pos);

            if( node != null )
                OnNodeClicked_.Invoke(node);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            var enumerator = nodeDict_.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddNodeTest( Node node )
        {
            nodeDict_.Add(node.Pos_, node);
        }
    }
}
