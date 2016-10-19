using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StevenUniverse.FanGame.Util;
using UnityEngine.SceneManagement;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Templates;
using System.Linq;
using StevenUniverse.FanGame.Battle;
using StevenUniverse.FanGame.Overworld.Instances;
using System;

public class LoadSceneChunks : MonoBehaviour 
{
    Chunk currentChunk_ = null;

    Grid grid_ = null;

    TileMap<TileInstance> tileMap_ = new TileMap<TileInstance>();

    IntVector3 lastCursorPosition_ = IntVector3.zero;
    public SpriteRenderer cursorSprite_;

    bool walkable_ = false;

    public GameObject pfb_WalkableTileSprite_;

    IEnumerator Start()
    {
        grid_ = GameObject.FindObjectOfType<Grid>();

        var chunks = GetChunks(4, 7);
        //var chunks = GetAllChunks();

        yield return StartCoroutine(LoadChunks(chunks));

        BuildGrid();
    }

    List<Chunk> GetChunks( params int[] indices  )
    {
        List<Chunk> chunks = new List<Chunk>();
        var appData = Utilities.ExternalDataPath;
        var currentSceneName = SceneManager.GetActiveScene().name;

        var chunkDirNames = Directory.GetDirectories(appData + "/Chunks/" + currentSceneName);

        chunkDirNames = chunkDirNames.Select(d => System.IO.Path.GetFileName(d)).ToArray();

        foreach( var i in indices )
        {
            string dir = chunkDirNames[i];
            string dataPath = string.Format("Chunks/{0}/{1}/{1}", currentSceneName, dir);

            chunks.Add(Chunk.GetChunk(dataPath));
        }

        return chunks;
    }

    List<Chunk> GetAllChunks()
    {
        List<Chunk> chunks = new List<Chunk>();
        var appData = Utilities.ExternalDataPath;
        var currentSceneName = SceneManager.GetActiveScene().name;

        var chunkDirNames = Directory.GetDirectories(appData + "/Chunks/" + currentSceneName);

        chunkDirNames = chunkDirNames.Select(d => System.IO.Path.GetFileName(d)).ToArray();

        for( int i = 0; i < chunkDirNames.Length; ++i )
        {
            string dir = chunkDirNames[i];
            string dataPath = string.Format("Chunks/{0}/{1}/{1}", currentSceneName, dir);

            chunks.Add(Chunk.GetChunk(dataPath));
        }

        return chunks;
    }

    IEnumerator LoadChunks( List<Chunk> chunks )
    {
        foreach (var chunk in chunks)
        {
            if (chunk == null)
                Debug.LogFormat("CHUNK IS NULL");

            currentChunk_ = chunk;
            ChunkRenderer.AddChunkRenderer(new GameObject(chunk.Name), true, chunk);

            yield return null;
        }

        currentChunk_ = null;

        yield return null;
    }

    void OnGUI()
    {
        if( currentChunk_ != null )
        {
            GUILayout.Label("Loading chunks...");
        }
    }

    public void BuildGrid( )
    {

        //List<Instance> instances = new List<Instance>();

        var renderers = GameObject.FindObjectsOfType<ChunkRenderer>();

        var chunks = renderers.Select(r => r.SourceChunk).ToArray();

        foreach( var renderer in renderers )
        {
            var chunk = renderer.SourceChunk;

            var instances = chunk.AllInstances;

            // Note this is includes any group instances sticking out the edges of the chunk
            var minX = chunk.MinX;
            var minY = chunk.MinY;
            var maxX = chunk.MaxX;
            var maxY = chunk.MaxY;
            
            int sizeX = maxX - minX;
            int sizeY = maxY - minY;
            if (sizeX > 0)
                ++sizeX;
            if (sizeY > 0)
                ++sizeY;

            // Run through each cell of our chunk and determine pathability.
            for( int x = 0; x < sizeX; ++x )
            {
                for( int y = 0; y < sizeY; ++y )
                {
                    //// Local positions...
                    //int xPos = x;// + (int)chunk.Position.x;
                    //int yPos = y;// + (int)chunk.Position.y;


                    // World positions...
                    int xPos = x + (int)chunk.Position.x;
                    int yPos = y + (int)chunk.Position.y;
                    Vector2 pos = new Vector2(xPos, yPos);

                    var tilesAtPos = chunk.AllInstancesFlattenedCoordinated.Get(xPos, yPos);
                    if (tilesAtPos.Length == 0)
                    {
                        //Debug.LogFormat("No tiles found at {0},{1}", xPos, yPos);
                        continue;
                    }

                    if( IsWalkable( tilesAtPos ) )
                    {
                        //Debug.LogFormat("{0} is walkable!", pos );
                        // Spawn a walkable tile marker on any walkable tiles
                        Instantiate(pfb_WalkableTileSprite_, new Vector3(xPos, yPos, 1f), Quaternion.identity, renderer.transform);
                    }
                }
            }
        }
        
    }
    
    // A tile is considered walkable if it's tile mode is "Surface" or "Transitional" 
    // and if it's not sharing a space with a tile whose tile mode is "Collidable" and if the tile above is it not grounded
    bool IsWalkable( TileInstance[] tileStack )
    {
        bool walkable = false;
        int walkableElevation = int.MinValue;
        bool collision = false;
        // Group our tiles according to their elevation
        var query = tileStack.GroupBy(t => t.Elevation, t => t);

        // Iterate through our groups starting from the lowest elevation to the highest
        foreach( var tileGroups in query )
        {
            foreach ( var tile in tileGroups )
            {
                var mode = tile.TileTemplate.TileModeName;

                // Ignore "normal" tiles, they are decorative and should not affect collision
                if (mode == "Normal")
                    continue;

                // If any tiles at this elevation are collidable, it's not walkable.
                if( mode == "Collidable" || collision )
                {
                    walkable = false;
                    collision = true;
                    continue;
                }

                // If this tile is grounded and is above any tile that was walkable, walkability is nulled
                if (walkable && tile.TileTemplate.IsGrounded && tile.Elevation == walkableElevation + 1)
                {
                    walkable = false;
                    walkableElevation = int.MinValue;
                }

                // If the current tile is a surface or transitional tile then this is potentially a walkable cell.
                if (mode == "Surface" || mode == "Transitional")
                {
                    walkable = true;
                    walkableElevation = tile.Elevation;
                }
            }
        }

        return walkable;


        //// Sort our tile stack into separate lists of tiles based on their elevation.
   

        //bool walkable = false;
        //int walkableHeight = int.MinValue;
        //bool collidable = false;
        //int collidableHeight = int.MinValue;

        //for( int i = 0; i < tileStack.Length; ++i )
        //{
        //    var tile = tileStack[i];
        //    var mode = tile.TileTemplate.TileModeName;

        //    // "Normal" tiles don't factor into walkability
        //    if (mode == "Normal")
        //        continue;


        //    //Debug.LogFormat("Pos:{0}, Elevation:{1}", tile.Position, tile.Elevation);

        //    // If a previous tile in the stack was found to be walkable...
        //    if ( walkable )
        //    {
        //        // Ensure the tile above it is not grounded. If it is, then the previous walkability is not valid
        //        if (tile.TileTemplate.IsGrounded && tile.Elevation == walkableHeight + 1)
        //            walkable = false;
        //    }
            
        //    // If the current tile is a surface or transitional tile then this is potentially a walkable cell.
        //    if( mode == "Surface" || mode == "Transitional" )
        //    {
        //        walkable = true;
        //        walkableHeight = tile.Elevation;
        //    }
            
        //    // If the current elevation has a "collidable" tile then it's not walkable, period
        //    if ( mode == "Collidable" )
        //    {
        //        if( walkable && )
        //        walkable = false;
        //    }

        //}

        //return walkable;
    }


    //// Polls the tile map to determine if the given position is walkable
    //bool IsWalkable( IntVector3 position )
    //{
    //    bool walkable = true;
    //    // Go through all tiles at the given position...
    //    foreach (var layer in TileTemplate.Layer.Instances)
    //    {
    //        // Index for our given position
    //        TileIndex currentIndex = new TileIndex((Vector2)position, position.z, layer);
    //        // Index for the tile above our given position.
    //        TileIndex aboveIndex = new TileIndex((Vector2)position, position.z + 1, layer);

    //        // current tile
    //        var tile = tileMap_.Get(currentIndex);
    //        if (tile != null)
    //        {
    //            if (tile.TileTemplate.TileLayerName != "Surface" || tile.TileTemplate.TileLayerName != "Transitional")
    //                walkable = false;
    //        }

    //        var aboveTile = tileMap_.Get(aboveIndex);
    //        if( aboveTile != null )
    //        {
    //            if (aboveTile.TileTemplate.IsGrounded)
    //                walkable = false;
    //        }
    //    }

    //    return walkable;
    //}

}
