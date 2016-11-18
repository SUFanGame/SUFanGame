using System.Collections;
using System.Collections.Generic;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame;
using StevenUniverse.FanGame.Util;

public static class CharacterUtility
{
    // TODO : Cache path lists.

    /// <summary>
    /// Move the given character from it's current position to the target position, taking pathfinding into account. 
    /// Uses given paths cache to speed up the pathfinding.
    /// </summary>
    /// <param name="mapChar">The character to move.</param>
    /// <param name="targetPos">The eventual position of the character.</param>
    /// <param name="pathsCache">Cached path values of paths in range. Speeds up pathfinding when there are
    /// a lot of possible destinations.</param>
    public static IEnumerator MoveTo( MapCharacter mapChar, IntVector3 targetPos, GridPaths pathsCache )
    {
        var grid = Grid.Instance;

        grid.RemoveObject(mapChar.GridPosition, mapChar);
        //Debug.LogFormat("Moving {0} from {1} to {2}", name, current, pos);

        List<Node> path = new List<Node>();

        pathsCache.PathPosition(mapChar.GridPosition, targetPos, path, grid);

        for (int i = 0; i < path.Count; ++i)
        {
            yield return mapChar.MoveTo(path[i].Pos_);
        }

        grid.AddObject(mapChar.GridPosition, mapChar);
    }

    /// <summary>
    /// Coroutine that plots a path from the given character's current position to 
    /// it's destination and moves the character there.
    /// </summary>
    /// <param name="mapChar">Character to move.</param>
    /// <param name="targetPos">Destination.</param>
    public static IEnumerator MoveTo( MapCharacter mapChar, IntVector3 targetPos )
    {
        List<Node> path = new List<Node>();

        var grid = Grid.Instance;

        grid.GetPath(mapChar.GridPosition, targetPos, path, MovementType.GROUNDED);

        // TODO: Might make more sense to just pass in the path in it's entirety and have the character do the movement logic.

        grid.RemoveObject(mapChar.GridPosition, mapChar);
        for ( int i = 0; i < path.Count; ++i )
        {
            yield return mapChar.MoveTo(path[i].Pos_);
        }
        grid.AddObject(mapChar.GridPosition, mapChar);
    }

    /// <summary>
    /// Scans for characters that are adjacent.
    /// </summary>
    /// <param name="actor">Character to conduct scan around.</param>
    /// <param name="filter">Optional predicate to apply filters.</param>
    public static List<MapCharacter> ScanForAdjacent(MapCharacter actor, System.Predicate<MapCharacter> filter = null)
    {
        List<MapCharacter> adjacentChars = new List<MapCharacter>();
        var grid = Grid.Instance;
        var pos = actor.GridPosition;
        var adjacent = Directions2D.Quadrilateral;
        
        for (int i = 0; i < adjacent.Length; ++i)
        {
            var adj = pos + adjacent[i];

            grid.GetObjects(adj, adjacentChars, filter);
        }

        return adjacentChars;
    }
}
