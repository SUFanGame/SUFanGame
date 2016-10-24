using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using StevenUniverse.FanGame.Extensions;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGameEditor.Wizards;

namespace StevenUniverse.FanGameEditor.Tools
{
    public static class TemplateTools
    {
        [MenuItem("Tools/SUFanGame/Templates/Create Tile Template From Image")]
        public static void CreateTileTemplateFromImage()
        {
            //Prompt the user to select the first frame .png for the tile
            string windowTitle = "Select the TileTemplate's first frame.";
            string templatesDirectoryAbsolutePath = Utilities.ExternalDataPath + "/Templates";

            string selectedImageAbsolutePath = EditorUtility.OpenFilePanel(windowTitle, templatesDirectoryAbsolutePath,
                "png");

            if (selectedImageAbsolutePath == "")
            {
                Debug.Log("No image selected!");
            }
            else
            {
                string tileTemplateDirectoryAbsolutePath =
                    Utilities.ConvertPathToParentDirectoryPath(selectedImageAbsolutePath);
                string tileTemplateName =
                    Utilities.ConvertDirectoryPathToDirectoryName(tileTemplateDirectoryAbsolutePath);
                string jsonOutputAbsolutePath = tileTemplateDirectoryAbsolutePath + "/" + tileTemplateName + ".json";

                if (File.Exists(jsonOutputAbsolutePath))
                {
                    Debug.Log("There is already a conflictingly named .json file in this folder!");
                }
                else
                {
                    //Create and save the initial Json TileTemplate
                    TileTemplate createdTileTemplate = new TileTemplate(new Sprite[0], false, 0f, "Normal", "Main", false, false);

                    string createdTileTemplateAppDataPath =
                        Utilities.ConvertAbsolutePathToAppDataPath(jsonOutputAbsolutePath);
                    createdTileTemplate.AppDataPath = createdTileTemplateAppDataPath;
                    //Set the image
                    string selectedImageName =
                        Utilities.RemoveFileExtension(Utilities.ConvertFilePathToFileName(selectedImageAbsolutePath));
                    createdTileTemplate.AnimationSpriteNames = new string[] {selectedImageName};
                    //Save the TileTemplate
                    createdTileTemplate.Save();

                    TileInstanceEditor createdTileInstanceEditor = RegenerateAndSelectTileTemplate(createdTileTemplate);
                    TileTemplateEditorWizard.CreateWizard(createdTileInstanceEditor);
                }
            }
        }

        [MenuItem("Tools/SUFanGame/Templates/Edit Selected Tile Template(s) %#e")]
        public static void EditSelectedTileTemplates()
        {
            List<TileInstanceEditor> selectedTileInstanceEditors = new List<TileInstanceEditor>();
            List<string> selectedTileTemplateAppDataPaths = new List<string>();

            foreach (GameObject selectedGameObject in Selection.gameObjects)
            {
                TileInstanceEditor selectedTileInstanceEditor = selectedGameObject.GetComponent<TileInstanceEditor>();

                //Only add objects that have a TileInstanceEditor component
                if (selectedTileInstanceEditor != null)
                {
                    //Cache the TileTemplate AppDataPath of the selected TileInstanceEditor
                    string selectedTileTemplateAppDataPath = selectedTileInstanceEditor.TileInstance.TemplateAppDataPath;

                    //Only add non-duplicate instances
                    if (!selectedTileTemplateAppDataPaths.Contains(selectedTileTemplateAppDataPath))
                    {
                        selectedTileInstanceEditors.Add(selectedTileInstanceEditor);
                        selectedTileTemplateAppDataPaths.Add(selectedTileTemplateAppDataPath);
                    }
                }
            }

            if (selectedTileInstanceEditors.Count > 0)
            {
                TileTemplateEditorWizard.CreateWizard(selectedTileInstanceEditors.ToArray());
            }
            else
            {
                Debug.Log("There were no TileInstanceEditor components on any of the selected GameObjects!");
            }
        }

        [MenuItem("Tools/SUFanGame/Templates/Create Group Template From Selection")]
        public static void CreateGroupTemplateFromSelection()
        {
            //Cache the selected GameObject
            GameObject selectedGameObject = Selection.activeGameObject;
            //Check if there isn't a selected GameObject
            if (selectedGameObject == null)
            {
                ToolUtilities.CancelOperation("No GameObject selected!");
            }

            //Cache the selected GameObject's GroupInstanceEditor component
            GroupInstanceEditor selectedGroupInstanceEditor = selectedGameObject.GetComponent<GroupInstanceEditor>();
            //Check if the selectedGameObject doesn't have a GroupInstanceEditor component
            if (selectedGroupInstanceEditor == null)
            {
                ToolUtilities.CancelOperation("Selected GameObject had no GroupInstanceEditor component!");
            }

            //Cache the selected GroupInstanceEditor's existing template app data path
            string existingTemplateAppDataPath = selectedGroupInstanceEditor.GroupInstance.TemplateAppDataPath;
            //Check if there is currently an existing template app data path
            if (!string.IsNullOrEmpty(existingTemplateAppDataPath))
            {
                ToolUtilities.CancelOperation("Selected GroupInstanceEditor must not have a TemplateAppDataPath!");
            }

            //Cache the selected GameObject's name
            string groupTemplateName = selectedGameObject.name;
            //Check if the group template's name is null or empty
            if (string.IsNullOrEmpty(groupTemplateName))
            {
                ToolUtilities.CancelOperation("Please give the selected GameObject a name!");
            }

            //Give the instructions to the user
            if (
                !EditorUtility.DisplayDialog("Instructions",
                    string.Format("Please select a folder with the name '{0}' to store the GroupTemplate in.",
                        groupTemplateName), "Ok", "Cancel"))
            {
                ToolUtilities.CancelOperation("Operation canceled by user!");
            }

            //Prompt the user to selected a folder
            string windowTitle = string.Format("Select the '{0}' root directory.", groupTemplateName);
            string templatesDirectoryAbsolutePath = Utilities.ExternalDataPath + "/Templates";
            string selectedFolderAbsolutePath = EditorUtility.OpenFolderPanel(windowTitle,
                templatesDirectoryAbsolutePath, "");

            //Check if the selected folder path is null or empty
            if (string.IsNullOrEmpty(selectedFolderAbsolutePath))
            {
                ToolUtilities.CancelOperation("No folder selected!");
            }

            //Check if the selected folder matched the expected name
            if (Utilities.ConvertDirectoryPathToDirectoryName(selectedFolderAbsolutePath) != groupTemplateName)
            {
                ToolUtilities.CancelOperation(
                    string.Format("The selected folder '{0}' did not match the expected name '{1}'!",
                        selectedFolderAbsolutePath, groupTemplateName));
            }

            //Build the JSON path
            string jsonOutputAbsolutePath = selectedFolderAbsolutePath + "/" + groupTemplateName + ".json";
            //TODO /*Get the selected folder*/ name UNUSED?
            //string selectedFolderName = Utilities.ConvertDirectoryPathToDirectoryName(selectedFolderAbsolutePath);

            //Check if a file with the selected name already exists
            if (File.Exists(jsonOutputAbsolutePath))
            {
                ToolUtilities.CancelOperation("There is already a conflictingly named .json file in this folder!");
            }

            //Write the selected GroupTemplate to JSON
            //TODO actually build it from the child tiles, regenerate the instanceEditor from it, and replace the selection with the generated prefab

            //Get the child TileInstanceEditors of the selected GameObject
            TileInstanceEditor[] childTileInstanceEditors =
                selectedGameObject.GetComponentsInJustChildren<TileInstanceEditor>();
            //Build a list of the child TileInstances
            List<TileInstance> childTileInstances = new List<TileInstance>();
            foreach (TileInstanceEditor childTileInstanceEditor in childTileInstanceEditors)
            {
                childTileInstances.Add(childTileInstanceEditor.TileInstance);
            }

            //Create the GroupTemplate
            GroupTemplate createdGroupTemplate = new GroupTemplate(childTileInstances.ToArray());
            //Set the GroupTemplate's AppDataPath to prepare it for saving
            createdGroupTemplate.AppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(jsonOutputAbsolutePath);
            //Save the GroupTemplate
            createdGroupTemplate.Save();

            //Update the AppDataPath of the selectedGroupInstanceEditor so it is updated when the GroupInstanceEditor prefab is generated
            selectedGroupInstanceEditor.GroupInstance.TemplateAppDataPath = createdGroupTemplate.AppDataPath;

            //Generate the GroupInstanceEditor prefab
            RegenerateAndSelectGroupTemplate(createdGroupTemplate);
            //Utilities.WriteStringToFile("TEST", jsonOutputAbsolutePath);
        }

        [MenuItem("Tools/SUFanGame/Templates/Apply Selected GroupTemplate")]
        public static void ApplySelectedGroupTemplate()
        {
            //Get the selected GroupInstanceEditor, if it exists
            GroupInstanceEditor selectedGroupInstanceEditor =
                Selection.activeGameObject.GetComponent<GroupInstanceEditor>();
            if (selectedGroupInstanceEditor == null)
            {
                ToolUtilities.CancelOperation("No GroupInstanceEditor component was on the selected GameObject!");
            }

            //Load the GroupTemplate
            GroupTemplate selectedGroupTemplate = selectedGroupInstanceEditor.GroupTemplate;

            //Get the child TileInstanceEditor components
            TileInstanceEditor[] childTileInstanceEditors =
                selectedGroupInstanceEditor.gameObject.GetComponentsInJustChildren<TileInstanceEditor>();

            //Build a list of the child TileInstances
            List<TileInstance> childTileInstances = new List<TileInstance>();
            foreach (TileInstanceEditor childTileInstanceEditor in childTileInstanceEditors)
            {
                childTileInstances.Add(childTileInstanceEditor.TileInstance);
            }

            //Assign the child TileInstances to the GroupTemplate
            selectedGroupTemplate.TileInstances = childTileInstances.ToArray();

            //Save the GroupTemplate
            selectedGroupTemplate.Save();

            //Regenerate the marker
            RegenerateAndSelectGroupTemplate(selectedGroupTemplate);
        }

        public static void MoveTemplate()
        {
        }

        public static TileInstanceEditor RegenerateAndSelectTileTemplate(TileTemplate tileTemplate)
        {
            //Save the existing instances to re-instantiate them after the regeneration
            //Cache the tileTemplateAppDataPath of the tile type we are saving
            string tileTemplateAppDataPath = tileTemplate.AppDataPath;
            //Get all TileInstanceEditors in the scene
            TileInstanceEditor[] tileInstanceEditors = GameObject.FindObjectsOfType<TileInstanceEditor>();
            //Build a list of TileInstanceEditors with the same tileTemplateAppDataPath as our target
            List<TileInstanceEditor> matchingTileInstanceEditors = new List<TileInstanceEditor>();
            foreach (TileInstanceEditor tileInstanceEditor in tileInstanceEditors)
            {
                if (tileInstanceEditor.TileInstance.TemplateAppDataPath == tileTemplateAppDataPath)
                {
                    matchingTileInstanceEditors.Add(tileInstanceEditor);
                }
            }

            //Regenerate the prefab
            string regeneratedTileInstanceEditorAssetPath = InstanceEditorTools.RegnerateTileInstanceEditor(tileTemplate);
            TileInstanceEditor regeneratedTileInstanceEditor =
                AssetDatabase.LoadAssetAtPath<TileInstanceEditor>(regeneratedTileInstanceEditorAssetPath);
            AssetDatabase.Refresh();
            Selection.activeGameObject = regeneratedTileInstanceEditor.gameObject;

            //Update the changed instances (backwards loop for safe destruction)
            for (int i = matchingTileInstanceEditors.Count - 1; i >= 0; i--)
            {
                //Cache the current index
                TileInstanceEditor matchingTileInstanceEditor = matchingTileInstanceEditors[i];
                //Cache the original parent
                Transform matchingTileInstanceEditorParent = matchingTileInstanceEditor.transform.parent;

                //Instantiate a new instance of the updated prefab at the same position as the original
                GameObject instantiatedPrefab =
                    PrefabUtility.InstantiatePrefab(regeneratedTileInstanceEditor.gameObject) as GameObject;
                instantiatedPrefab.transform.position = matchingTileInstanceEditor.transform.position;

                //If the original TileInstanceEditor had a parent Transform, make it the parent of the new TileInstanceEditor
                if (matchingTileInstanceEditorParent != null)
                {
                    instantiatedPrefab.transform.SetParent(matchingTileInstanceEditorParent);
                }

                //Set the TileInstance of the new instance to match the original
                TileInstanceEditor newTileInstanceEditor = instantiatedPrefab.GetComponent<TileInstanceEditor>();
                newTileInstanceEditor.TileInstance = matchingTileInstanceEditor.TileInstance;

                //Destroy the original
                GameObject.DestroyImmediate(matchingTileInstanceEditor.gameObject);
            }

            return regeneratedTileInstanceEditor;
        }

        public static GroupInstanceEditor RegenerateAndSelectGroupTemplate(GroupTemplate groupTemplate)
        {
            //Save the existing instances to re-instantiate them after the regeneration
            //Cache the groupTemplateAppDataPath of the group type we are saving
            string groupTemplateAppDataPath = groupTemplate.AppDataPath;
            //Get all GroupInstanceEditors in the scene
            GroupInstanceEditor[] groupInstanceEditors = GameObject.FindObjectsOfType<GroupInstanceEditor>();
            //Build a list of GroupInstanceEditors with the same groupTemplateAppDataPath as our target
            List<GroupInstanceEditor> matchingGroupInstanceEditors = new List<GroupInstanceEditor>();
            foreach (GroupInstanceEditor groupInstanceEditor in groupInstanceEditors)
            {
                if (groupInstanceEditor.GroupInstance.TemplateAppDataPath == groupTemplateAppDataPath)
                {
                    matchingGroupInstanceEditors.Add(groupInstanceEditor);
                }
            }

            //Regenerate the prefab
            string regeneratedGroupInstanceEditorAssetPath =
                InstanceEditorTools.RegenerateGroupInstanceEditor(groupTemplate);
            GroupInstanceEditor regeneratedGroupInstanceEditor =
                AssetDatabase.LoadAssetAtPath<GroupInstanceEditor>(regeneratedGroupInstanceEditorAssetPath);
            AssetDatabase.Refresh();
            Selection.activeGameObject = regeneratedGroupInstanceEditor.gameObject;

            //Update the changed instances (backwards loop for safe destruction)
            for (int i = matchingGroupInstanceEditors.Count - 1; i >= 0; i--)
            {
                //Cache the current index
                GroupInstanceEditor matchingGroupInstanceEditor = matchingGroupInstanceEditors[i];
                //Cache the original parent
                Transform matchingGroupInstanceEditorParent = matchingGroupInstanceEditor.transform.parent;

                //Instantiate a new instance of the updated prefab at the same position as the original
                GameObject instantiatedPrefab =
                    PrefabUtility.InstantiatePrefab(regeneratedGroupInstanceEditor.gameObject) as GameObject;
                instantiatedPrefab.transform.position = matchingGroupInstanceEditor.transform.position;

                //If the original GroupInstanceEditor had a parent Transform, make it the parent of the new GroupInstanceEditor
                if (matchingGroupInstanceEditorParent != null)
                {
                    instantiatedPrefab.transform.SetParent(matchingGroupInstanceEditorParent);
                }

                //Set the TileInstance of the new instance to match the original
                GroupInstanceEditor newGroupInstanceEditor = instantiatedPrefab.GetComponent<GroupInstanceEditor>();
                newGroupInstanceEditor.GroupInstance = matchingGroupInstanceEditor.GroupInstance;

                //Destroy the original
                GameObject.DestroyImmediate(matchingGroupInstanceEditor.gameObject);
            }

            return regeneratedGroupInstanceEditor;
        }
    }
}