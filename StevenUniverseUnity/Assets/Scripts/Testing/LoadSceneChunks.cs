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
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGame;
using StevenUniverse.FanGame.Overworld.Templates;

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

        //Debug.LogFormat("{0}", string.Join(",", chunkDirNames));

        List<Chunk> chunks = new List<Chunk>();

        //foreach( var dir in chunkDirNames )
        int numToLoad = 6;
        for( int i = numToLoad; i < numToLoad + 1; ++i )
        {
            string dir = chunkDirNames[i];
            string dataPath = string.Format("Chunks/{0}/{1}/{1}", currentSceneName, dir);
            //Debug.LogFormat("{0}", dataPath);
            chunks.Add(Chunk.GetChunk(dataPath));
        }

        yield return StartCoroutine(LoadChunks(chunks));

        BuildGrid( chunks );
    }

    IEnumerator LoadChunks( List<Chunk> chunks )
    {
        foreach (var chunk in chunks)
        {
            //yield return new WaitForSeconds(0.1f);

            if (chunk == null)
                Debug.LogFormat("CHUNK IS NULL");

            currentChunk_ = chunk;
            ChunkRenderer.AddChunkRenderer(new GameObject(chunk.Name), true, chunk);

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

    public void BuildGrid( List<Chunk> chunks )
    {

        List<Instance> instances = new List<Instance>();

        foreach( var chunk in chunks )
        {
            instances.AddRange(chunk.AllInstances);
        }
        


        // Add all tile instances to our map.
        foreach( var i in instances)
        {
            if( i is GroupInstance )
            {
                var group = i as GroupInstance;
                foreach( var tile in group.IndependantTileInstances)
                {
                    var index = new TileIndex(tile.Position, tile.Elevation, tile.TileTemplate.TileLayer );
                    tileMap_.AddInstance(index, tile);
                }
            }

            if( i is TileInstance )
            {
                var tile = i as TileInstance;
                var index = new TileIndex(tile.Position, tile.Elevation, tile.TileTemplate.TileLayer);
                tileMap_.AddInstance(index, tile);
            }
        }
    }
    


    // Polls the tile map to determine if the given position is walkable
    bool IsWalkable( IntVector3 position )
    {
        bool walkable = true;
        // Go through all tiles at the given position...
        foreach (var layer in TileTemplate.Layer.Instances)
        {
            // Index for our given position
            TileIndex currentIndex = new TileIndex((Vector2)position, position.z, layer);
            // Index for the tile above our given position.
            TileIndex aboveIndex = new TileIndex((Vector2)position, position.z + 1, layer);

            // current tile
            var tile = tileMap_.Get(currentIndex);
            if (tile != null)
            {
                if (tile.TileTemplate.TileLayerName != "Surface" || tile.TileTemplate.TileLayerName != "Transitional")
                    walkable = false;
            }

            var aboveTile = tileMap_.Get(aboveIndex);
            if( aboveTile != null )
            {
                if (aboveTile.TileTemplate.IsGrounded)
                    walkable = false;
            }
        }

        return walkable;
    }
}
