using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StevenUniverse.FanGame.Util;
using UnityEngine.SceneManagement;
using StevenUniverse.FanGame.Overworld;
using System.Linq;


public class LoadSceneChunks : MonoBehaviour 
{
    Chunk currentChunk_ = null; 

    IEnumerator Start()
    {

        var appData = Utilities.ExternalDataPath;

        var currentSceneName = SceneManager.GetActiveScene().name;

        var chunkDirNames = Directory.GetDirectories(appData + "/Chunks/" + currentSceneName);

        chunkDirNames = chunkDirNames.Select(d => Path.GetFileName(d)).ToArray();

        //Debug.LogFormat("{0}", string.Join(",", chunkDirNames));

        List<Chunk> chunks = new List<Chunk>();

        foreach( var dir in chunkDirNames )
        {
            string dataPath = string.Format("Chunks/{0}/{1}/{1}", currentSceneName, dir);
            //Debug.LogFormat("{0}", dataPath);
            chunks.Add(Chunk.GetChunk(dataPath));
        }

        yield return StartCoroutine(LoadChunks(chunks));
        
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


}
