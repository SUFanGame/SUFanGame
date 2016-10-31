using UnityEngine;
using System.Collections;
using StevenUniverse.FanGame.StrategyMap;
using System.Collections.Generic;

public static class CharacterUtility
{
    
    public static IEnumerator MoveTo( MapCharacter mapChar, IntVector3 targetPos, GridPaths pathsCache )
    {
        var grid = Grid.Instance;

        grid.RemoveObject(mapChar.GridPosition, mapChar);
        //Debug.LogFormat("Moving {0} from {1} to {2}", name, current, pos);

        List<Node> path = new List<Node>();
        

        pathsCache.PathPosition(mapChar.GridPosition, targetPos, path, grid);

        //string pathString = string.Join("->", path.Select(p => p.Pos_.ToString()).ToArray());
        //Debug.LogFormat("Path to {0}: {1}", targetPos, pathString );

        for (int i = 0; i < path.Count; ++i)
        {
            var next = path[i];

            yield return mapChar.StartCoroutine(mapChar.MoveTo(next.Pos_));

            //var gridPos = mapChar.GridPosition;
            //gridPos.z = next.Pos_.z;
            //mapChar.GridPosition = gridPos;

            //mapChar.UpdateSortingOrder();

            //var dest = (Vector3)next.Pos_;

            //float t = 0;
            //while (t <= 1f)
            //{
            //    mapChar.transform.position = Vector3.Lerp(mapChar.transform.position, dest, t);
            //    t += Time.deltaTime * mapChar.tilesMovedPerSecond_;
            //    yield return null;
            //}
        }

        grid.AddObject(mapChar.GridPosition, mapChar);

        //Debug.LogFormat("Done moving");
        yield return null;
    }
}
