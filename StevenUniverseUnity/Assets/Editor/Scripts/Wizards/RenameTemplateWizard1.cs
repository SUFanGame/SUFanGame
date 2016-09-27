using UnityEditor;

public class RenameTemplateWizard : ScriptableWizard
{
    /*
    [SerializeField]
    private Object instanceEditorDirectory;

    [SerializeField]
    private Object outputDirectory;

    private InstanceEditor instanceEditor;

    [MenuItem("Tools/SUFanGame/Templates/Rename Template")]
    public static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<RenameTemplateWizard>("Rename Template", "Move", "Cancel");
    }

    void OnWizardCreate()
    {
        //Ensure the tool is not used while there are InstanceEditors loaded in the scene
        if (Utilities.GetObjectsInScene<InstanceEditor>().Length > 0)
        {
            ToolUtilities.CancelOperation("This tool can not be used while there are InstanceEditors loaded in the scene!");
        }

        //Verify that the user wants to perform this time consuming operation
        if (!EditorUtility.DisplayDialog("Rename Template", "This tool will rename the specificied template and refactor all json files accordingly. This could take a very long time. Are you sure you wish to proceed?", "Yes", "Cancel"))
        { ToolUtilities.CancelOperation("Operation canceled by user"); }

        //Inform the user of a potential problem to make sure they have prepared the scene properly
        if (!EditorUtility.DisplayDialog("Rename Template", "You should not use this tool if there are InstanceEditors loaded in ANY SCENE, not just this one. If you attempt to move templates that have loaded InstanceEditors in other scenes, they will be disconnected and become unsavable. Be sure and save any chunk changes you have made BEFORE using this tool to avoid data loss.", "I understand, Continue", "Cancel"))
        { ToolUtilities.CancelOperation("Operation canceled by user"); }

        //Create a variable to keep track of progress for the progress bar
        float progress = 0f;
        //Display the progress bar
        EditorUtility.DisplayProgressBar("Renaming Template", "Initializing...", progress);

        string instanceEditorOutputDirectoryAssetPath = AssetDatabase.GetAssetPath(outputDirectory);

        string templateOutputDirectoryAppDataPath = Utilities.ConvertEditorInstanceAssetPathToMirroringTemplateAppDataPath(instanceEditorOutputDirectoryAssetPath);

        //Ensure the output directory exists
        string templateOutputDirectoryAbsolutePath = Utilities.ConvertAppDataPathToAbsolutePath(templateOutputDirectoryAppDataPath);
        if ( !Directory.Exists(templateOutputDirectoryAbsolutePath))
        {
            Directory.CreateDirectory(templateOutputDirectoryAbsolutePath);
        }

        //Create a Dictionary to store the original Template AppDataPaths of each InstanceEditor before it is updated
        Dictionary<InstanceEditor, string> originalTemplateAppDataPaths = new Dictionary<InstanceEditor, string>();

        //Move the Directories of each InstanceEditor to their new location
        foreach (InstanceEditor instanceEditor in instanceEditors)
        {
            //Store the original Template AppDataPath of the current InstanceEditor for future reference
            originalTemplateAppDataPaths.Add(instanceEditor, instanceEditor.Instance.TemplateAppDataPath);

            //Get the original and new instance editor directory asset paths
            string originalInstanceEditorDirectoryAssetPath = Utilities.ConvertPathToParentDirectoryPath(AssetDatabase.GetAssetPath(instanceEditor));
            string newInstanceEditorDirectoryAssetPath = instanceEditorOutputDirectoryAssetPath + "/" + Utilities.ConvertDirectoryPathToDirectoryName(originalInstanceEditorDirectoryAssetPath);

            //Move the InstanceEditor directory
            AssetDatabase.MoveAsset(originalInstanceEditorDirectoryAssetPath, newInstanceEditorDirectoryAssetPath);

            //Get the original and new template directory absolute paths
            string originalTemplateDirectoryAbsolutePath = Utilities.ConvertAppDataPathToAbsolutePath(Utilities.ConvertPathToParentDirectoryPath(instanceEditor.Instance.TemplateAppDataPath));
            string newTemplateDirectoryAbsolutePath = templateOutputDirectoryAbsolutePath + "/" + Utilities.ConvertDirectoryPathToDirectoryName(originalTemplateDirectoryAbsolutePath);

            Directory.Move(originalTemplateDirectoryAbsolutePath, newTemplateDirectoryAbsolutePath);

            //Update the InstanceEditor's TemplateAppDataPath field to point to the new location of the Template
            string newTemplateAppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(newTemplateDirectoryAbsolutePath) + "/" + instanceEditor.name;
            instanceEditor.Instance.TemplateAppDataPath = newTemplateAppDataPath;
        }

        //Update the Chunk JSON files to match the movement
        string[] chunkAbsolutePaths = Directory.GetFiles(Utilities.ExternalDataPath + "/Chunks", "*.json", SearchOption.AllDirectories);
        foreach (string chunkAbsolutePath in chunkAbsolutePaths)
        {
            bool changeMade = false;

            //Load the Chunk
            string chunkAppDataPath = Utilities.ConvertAbsolutePathToAppDataPath(chunkAbsolutePath);
            string currentChunkName = Utilities.ConvertFilePathToFileName(chunkAppDataPath);

            //Display the progress bar
            EditorUtility.DisplayProgressBar("Renaming Template", string.Format("Processing '{0}'...", currentChunkName), progress);

            //Load the Chunk's JSON file's text as a string
            string chunkJsonText = File.ReadAllText(chunkAbsolutePath);

            foreach (InstanceEditor instanceEditor in instanceEditors)
            {
                string originalTemplateAppDataPath = '"' + originalTemplateAppDataPaths[instanceEditor] + '"';
                
                string newTemplateAppDataPath = string.Format("\"{0}/{1}/{2}\"", templateOutputDirectoryAppDataPath, instanceEditor.name, instanceEditor.name);

                if (chunkJsonText.Contains(originalTemplateAppDataPath))
                {
                    chunkJsonText =  chunkJsonText.Replace(originalTemplateAppDataPath, newTemplateAppDataPath);
                    changeMade = true;
                }
            }

            //Write the changes to the JSON file
            if (changeMade)
            {
                Utilities.WriteStringToFile(chunkJsonText, chunkAbsolutePath);
            }

            //Increment the progress
            progress += 1 / (float)chunkAbsolutePaths.Length;
        }

        EditorUtility.ClearProgressBar();
    }

    void OnWizardUpdate()
    {
        //Verify that the InstanceEditor Directories are valid directories and contain valid InstanceEditors
        if (instanceEditorDirectories.Count > 0)
        {
            //Clear the InstanceEditors cache
            instanceEditors.Clear();

            //Start by assuming the input is valid
            bool inputIsValid = true;

            foreach (Object instanceEditorDirectory in instanceEditorDirectories)
            {
                string instanceEditorAbsolutePath = GetAbsolutePathOfAsset(instanceEditorDirectory);

                //If it isn't a directory, set it back to null
                if (!Directory.Exists(instanceEditorAbsolutePath))
                {
                    inputIsValid = false;
                }
                //Otherwise, make sure it's valid
                else
                {
                    string instanceEditorName = Utilities.ConvertDirectoryPathToDirectoryName(instanceEditorAbsolutePath);

                    //Ensure there is an expectedly named InstanceEditor inside the directory
                    string expectedInstanceEditorAbsolutePath = string.Format("{0}/{1}.prefab", instanceEditorAbsolutePath, instanceEditorName);
                    string expectedInstanceEditorAssetPath = Utilities.ConvertAbsolutePathToAssetPath(expectedInstanceEditorAbsolutePath);
                    GameObject expectedInstanceEditorGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(expectedInstanceEditorAssetPath);

                    //IF the asset wasn't found OR it was found but didn't have an InstanceEditor component, the input is invalid
                    if (expectedInstanceEditorGameObject == null )
                    {
                        inputIsValid = false;
                    }
                    else
                    {
                        InstanceEditor expectedInstanceEditor = expectedInstanceEditorGameObject.GetComponent<InstanceEditor>();

                        if (expectedInstanceEditor == null)
                        {
                            inputIsValid = false;
                        }
                        else
                        {
                            instanceEditors.Add(expectedInstanceEditor);
                        }
                    }
                }
            }

            if (!inputIsValid)
            {
                instanceEditorDirectories.Clear();
                Debug.Log("Attempted to assign an invalid object to the output directory field!");
            }
        }

        //Verify that the OutputDirectory is a valid directory
        if (outputDirectory != null)
        {
            string outputDirectoryAbsolutePath = GetAbsolutePathOfAsset(outputDirectory);

            //If it isn't a directory, set it back to null
            if (!Directory.Exists(outputDirectoryAbsolutePath))
            {
                outputDirectory = null;
                Debug.Log("Attempted to assign an invalid object to the output directory field!");
            }
        }
    }

    void OnWizardOtherButton()
    {
        this.Close();
    }

    private string GetAbsolutePathOfAsset(Object asset)
    {
        string assetAbsolutePath = Utilities.ConvertAssetPathToAbsolutePath(AssetDatabase.GetAssetPath(asset));

        return assetAbsolutePath;
    }
    */
}