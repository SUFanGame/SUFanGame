using UnityEngine;
using UnityEditor;
using SUGame.World;
using SUGame.Util;
using SUGame.Interactions;
using System.IO;

namespace SUGame.SUGameEditor.MapEditing
{

    public class CutsceneEditor : EditorWindow
    {

        //This will need to be set, either as a new scene or imported one
        Scene[] OpenScene_ = new Scene[] { };
        Vector2 scrollpos;


        [MenuItem("Tools/CutsceneEditor")]
        public static void OpenWindow()
        {
            GetWindow<CutsceneEditor>("Cutscene Editor");

        }

        //Opens up the editor window
        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            if (GUILayout.Button("New"))
            {
                //make a "are you sure?" box
                OpenScene_ = new Scene[] { };
            }
            if (GUILayout.Button("Open"))
            {
                //open a pre-existing cutscene
                string path = EditorUtility.OpenFilePanel("Open Cutscene", "/Resources/Cutscenes/", "json");
                path = path.Substring(path.IndexOf("/Cutscenes/") + 11);
                path = path.Substring(0, path.Length - 5); //I miss python
                OpenScene_ = CutsceneLoader.ImportCutscene(path);
                //Debug.Log("opened!");
            }
            if (GUILayout.Button("Save"))
            {
                //serialize and save the currently open scene
                string path = EditorUtility.SaveFilePanel("Save Cutscene", "/Resources/Cutscenes/", "", "json");
                string json = JsonHelper.Serialize<Scene>(typeof(Scene), OpenScene_);
                File.WriteAllText(path, json);
            }
            if (GUILayout.Button("Play"))
            {
                //Launch the game in a limited state and test
            }
            EditorGUILayout.EndHorizontal();


            //See the cutscene laid out
            scrollpos = EditorGUILayout.BeginScrollView(scrollpos);
            for (int i = 0; i < OpenScene_.Length; i++)
            {
                Scene frame = OpenScene_[i];
                EditorGUILayout.LabelField("Scene" + i);
                if (frame.CameraChange != null)
                {
                    EditorGUILayout.SelectableLabel(frame.CameraChange.ToString());
                }
                if (frame.CharaAction != null)
                {
                    foreach (CutsceneCharacterAction act in frame.CharaAction)
                    {
                        EditorGUILayout.SelectableLabel(act.ToString());
                    }
                }
                if (frame.DialogFileName != null)
                {
                    EditorGUILayout.SelectableLabel(frame.DialogFileName);
                }
            }
            EditorGUILayout.EndScrollView();

            //Add some new stuff
        }
    }
}