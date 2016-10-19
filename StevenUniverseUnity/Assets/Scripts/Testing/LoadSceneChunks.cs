using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StevenUniverse.FanGame.Util;
using UnityEngine.SceneManagement;
using StevenUniverse.FanGame.Overworld;
using System.Linq;
using StevenUniverse.FanGame.Battle;
using StevenUniverse.FanGame.Overworld.Instances;

public class LoadSceneChunks : MonoBehaviour 
{
    Chunk currentChunk_ = null;

    Grid grid_ = null;

    TileMap<TileInstance> tileMap_ = new TileMap<TileInstance>();

    IntVector3 lastCursorPosition_ = IntVector3.zero;
    public SpriteRenderer cursorSprite_;

    bool walkable_ = false;

    IEnumerator Start()
    {
        grid_ = GameObject.FindObjectOfType<Grid>();

        var appData = Utilities.ExternalDataPath;

        var currentSceneName = SceneManager.GetActiveScene().name;

        var chunkDirNames = Directory.GetDirectories(appData + "/Chunks/" + currentSceneName);

        chunkDirNames = chunkDirNames.Select(d => System.IO.Path.GetFileName(d)).ToArray();

        List<Chunk> chunks = new List<Chunk>();

        // Load a single isolated chunk for testing purposes.
        int numToLoad = 6;
        //int numToLoad = 2;
        for (int i = numToLoad; i < numToLoad + 1; ++i)
        //for( int i = 0; i < chunkDirNames.Length; ++i )
        {
            string dir = chunkDirNames[i];
            string dataPath = string.Format("Chunks/{0}/{1}/{1}", currentSceneName, dir);
            //Debug.LogFormat("{0}", dataPath);
            chunks.Add(Chunk.GetChunk(dataPath));
        }

        yield return StartCoroutine(LoadChunks(chunks));

        BuildGrid();
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

                    var tilesAtPos = chunk.AllInstancesFlattenedCoordinated.Get(x, y);
                    if (tilesAtPos.Length == 0)
                    {
                        Debug.LogFormat("No tiles found at {0},{1}", xPos, yPos);
                        continue;
                    }

                    string tilesString = string.Join(",", tilesAtPos.Select(t => t.TileTemplate.Name).ToArray());
                    Debug.LogFormat("Tiles at {0},{1}: {2}", xPos, yPos, tilesString);

                    return;
                }
            }

            //Debug.LogFormat("Size: {0},{1}", sizeX, sizeY);
            //Debug.LogFormat();
            //string minMax = string.Format("Min: {0},{1} Max: {2},{3}", minX, minY, maxX, maxY);
            //Rect bounds = Rect.MinMaxRect(minX, minY, maxX, maxY);

            //renderer.name = renderer.name + ":" + minMax;

            //Debug.LogFormat("Bounds for chunk {0}: {1}", chunk.Name, bounds);

            

           // instances.AddRange(chunk.AllInstances);
        }
        
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
