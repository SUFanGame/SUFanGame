using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StevenUniverse.FanGame.Util;
using UnityEngine.SceneManagement;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Templates;
using System.Linq;
using StevenUniverse.FanGame.StrategyMap;
using StevenUniverse.FanGame.Overworld.Instances;
using System;

public class LoadSceneChunks : MonoBehaviour 
{
    Chunk currentChunk_ = null;

    public TileMap<ITile> tileMap_ = new TileMap<ITile>();

    public SpriteRenderer cursorSprite_;

    public GameObject pfb_WalkableTileSprite_;

    IEnumerator Start()
    {
        //var chunks = GetChunks(4, 7);
        var chunks = GetAllChunks();

        yield return StartCoroutine(LoadChunks(chunks));

        foreach( var chunk in chunks )
        {
            tileMap_.AddRange(chunk.AllInstancesFlattened);
        }

        var grid = GameObject.FindObjectOfType<Grid>();
        yield return grid.BuildGrid(tileMap_);

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

}
