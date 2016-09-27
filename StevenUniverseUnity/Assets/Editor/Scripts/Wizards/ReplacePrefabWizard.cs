using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using StevenUniverse.FanGame.OverworldEditor;

//TODO THIS CLASS HAS NOT BEEN TESTED SINCE THE MARKER TO INSTANCEEDITOR SWITCH

namespace StevenUniverse.FanGameEditor.Wizards
{
    public class ReplacePrefabWizard : ScriptableWizard
    {
        public GameObject prefabA;
        public GameObject prefabB;

        void OnWizardCreate()
        {
            List<GameObject> toReplace = new List<GameObject>();

            GameObject[] gameObjectsInScene = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject go in gameObjectsInScene)
            {
                if (go.name == prefabA.name && go.GetComponent<InstanceEditor>() != null)
                {
                    toReplace.Add(go);
                }
            }

            Debug.Log(toReplace.Count);

            for (int i = toReplace.Count - 1; i >= 0; i--)
            {
                GameObject go = toReplace[i];
                Vector3 goPosition = go.transform.position;
                int goElevation = go.GetComponent<InstanceEditor>().Elevation;

                DestroyImmediate(go);
                GameObject test = PrefabUtility.InstantiatePrefab(prefabB) as GameObject;
                test.transform.position = goPosition;
                test.GetComponent<InstanceEditor>().Elevation = goElevation;
            }
        }

        void OnWizardUpdate()
        {
        }

        void OnWizardOtherButton()
        {
            this.Close();
        }
    }
}