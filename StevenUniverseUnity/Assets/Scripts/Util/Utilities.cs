using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using FullSerializer;

namespace StevenUniverse.FanGame.Util
{
    public static class Utilities
    {
        public const int CHUNK_WIDTH = 20;
        public const int CHUNK_HEIGHT = 20;

        public static string ExternalDataPath
        {
            get
            {
                //Go two directories up from the application datapath, then back down one into the StevenUniverseData folder
                return Utilities.ConvertPathToParentDirectoryPath(Utilities.ConvertPathToParentDirectoryPath(Application.dataPath)) + "/StevenUniverseData";
            }
        }

        public static int[] GetIndexesOfChar(string stringToSearch, char charToFind)
        {
            List<int> matchingIndexes = new List<int>();

            for (int i = 0; i < stringToSearch.Length; i++)
            {
                if (stringToSearch[i] == charToFind)
                {
                    matchingIndexes.Add(i);
                }
            }

            return matchingIndexes.ToArray();
        }

        public static string GetDateTimeStamp()
        {
            return System.DateTime.Now.ToString().Replace(":", ".").Replace("/", ".");
        }

        //Writes a given string to a given file. Will replace the file if it already exists.
        public static void WriteStringToFile(string output, string outputPathAbsolute)
        {
            string outputPathParentDirectoryAbsolute = Utilities.ConvertPathToParentDirectoryPath(outputPathAbsolute);
            if (!Directory.Exists(outputPathParentDirectoryAbsolute))
            {
                Directory.CreateDirectory(outputPathParentDirectoryAbsolute);
            }

            using (FileStream fileStream = new FileStream(outputPathAbsolute, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(output);
                }
            }
        }

        public static void ClearDirectory(string directoryPathAbsolute)
        {
            if (Directory.Exists(directoryPathAbsolute))
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPathAbsolute);

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                Directory.CreateDirectory(directoryPathAbsolute);
            }
        }

        //TODO change this to just remove from the last instance of '.'
        public static string RemoveFileExtension(string filePath, string extension)
        {
            string extensionSubstring = filePath.Substring(filePath.Length - extension.Length, extension.Length);

            if (extensionSubstring != extension)
            {
                throw new UnityException("Requested extension could not be removed because it was not found!");
            }

            return filePath.Substring(0, filePath.Length - extension.Length);
        }

        public static string RemoveFileExtension(string filePath)
        {
            int lastIndexOfPeriod = filePath.LastIndexOf('.');
            if (lastIndexOfPeriod != -1)
            {
                filePath = filePath.Substring(0, lastIndexOfPeriod);
            }

            return filePath;
        }

        public static bool FilePathHasExtension(string filePath, string extension)
        {
            return filePath.Substring(filePath.Length - extension.Length).Equals(extension);
        }

        public static string ConvertAssetPathToResourcePath(string assetPath)
        {
            //Cache the Resource Directory path
            string resourcesDirectory = Constants.RESOURCES_DIRECTORY;

            //Make sure the Resource Directory path is where it's supposed to be
            if (!assetPath.Substring(0, resourcesDirectory.Length).Equals(resourcesDirectory))
            {
                throw new UnityException("Asset was not stored in the Resources folder: " + assetPath);
            }

            //Remove the Resource Directory path from the Path
            assetPath = assetPath.Substring(resourcesDirectory.Length);

            //Make sure the extension is removed
            assetPath = RemoveFileExtension(assetPath);

            return assetPath;
        }

        public static string ConvertFilePathToFileName(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }

        public static string ConvertDirectoryPathToDirectoryName(string path)
        {
            //If the last char is a '/', remove it
            if (path[path.Length - 1] == '/')
            {
                path = path.Substring(0, path.Length - 1);
            }

            //Remove the "/" before the directory name and everything before it
            path = path.Substring(path.LastIndexOf('/') + 1);

            return path;
        }

        public static string ConvertPathToParentDirectoryPath(string filePath)
        {
            return filePath.Substring(0, filePath.LastIndexOf('/'));
        }

        public static string ConvertAssetPathToAbsolutePath(string assetPath)
        {
            string dataPathParentDirectory = ConvertPathToParentDirectoryPath(Application.dataPath);
            return dataPathParentDirectory + "/" + assetPath;
        }

        public static string ConvertAbsolutePathToAppDataPath(string absolutePath)
        {
            string fixedPath = absolutePath.Replace(@"\", "/");

            if (!fixedPath.Contains(Utilities.ExternalDataPath))
            {
                throw new UnityException(
                    "Cannot convert path to AppData format that is not in the persistent data path!");
            }

            string appDataPath = RemoveFileExtension(fixedPath.Replace(Utilities.ExternalDataPath + "/", ""));

            return appDataPath;
        }

        public static string ConvertAbsolutePathToAssetPath(string absolutePath)
        {
            string fixedPath = absolutePath.Replace(@"\", "/");

            if (!fixedPath.Contains(Application.dataPath))
            {
                throw new UnityException("Cannot convert path to Asset format that is not in the data path!");
            }

            //TODO for all 'replace's or 'contains' like this, check the individual substring and then replace that exact substring to be 100% error-proof
            string assetPath = fixedPath.Replace(Application.dataPath, "Assets");
            return assetPath;
        }

        //UNTESTED but should work
        public static string ConvertAppDataPathToAbsolutePath(string appDataPath)
        {
            string absolutePath = Utilities.ExternalDataPath + "/" + appDataPath;
            return absolutePath;
        }

        public static string ConvertTemplateAppDataPathToMirroringEditorInstanceAssetPath(string templateAppDataPath)
        {
            if (!CheckIfStartMatches(templateAppDataPath, "Templates/"))
            {
                throw new UnityException(
                    "Cannot convert path to mirroring EditorInstance AssetPath format that is not in the Template AppDataPath format!");
            }

            string mirroringInstanceEditorAssetPath = "Assets/Editor/Prefabs/EditorInstances/" +
                                                      templateAppDataPath.Replace("Templates/", "") + ".prefab";

            return mirroringInstanceEditorAssetPath;
        }

        public static string ConvertEditorInstanceAssetPathToMirroringTemplateAppDataPath(string editorInstanceAssetPath)
        {
            if (!CheckIfStartMatches(editorInstanceAssetPath, "Assets/Editor/Prefabs/EditorInstances/"))
            {
                throw new UnityException(
                    "Cannot convert path to mirroring Template AppDataPath format that is not in the EditorInstance AssetPath format!");
            }

            string mirroringTemplateAppDataPath =
                Utilities.RemoveFileExtension(editorInstanceAssetPath.Replace("Assets/Editor/Prefabs/EditorInstances/",
                    "Templates/"));

            return mirroringTemplateAppDataPath;
        }

        //Check if the start of a string matches the given string
        public static bool CheckIfStartMatches(string stringToCheck, string targetToMatch)
        {
            if (stringToCheck.Length < targetToMatch.Length)
            {
                return false;
            }

            return stringToCheck.Substring(0, targetToMatch.Length) == targetToMatch;
        }

        //Check if the end of a string matches the given string
        public static bool CheckIfEndMatches(string stringToCheck, string targetToMatch)
        {
            if (stringToCheck.Length < targetToMatch.Length)
            {
                return false;
            }

            return stringToCheck.Substring(stringToCheck.Length - targetToMatch.Length, targetToMatch.Length) ==
                   targetToMatch;
        }

        //Get objects of type in scene
        public static T[] GetObjectsInScene<T>() where T : UnityEngine.Object
        {
            return UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
        }

        public static int FloorToNearestMultiple(float value, int multiple)
        {
            return Mathf.FloorToInt(value/multiple)*multiple;
        }

        public static Vector3 FloorToNearestMultiple(Vector3 value, int xMultiple, int yMultiple)
        {
            return new Vector3(FloorToNearestMultiple(value.x, xMultiple), FloorToNearestMultiple(value.y, yMultiple), 0);
        }

        public static string ReplaceUnsafePathCharsWith(string original, string replacer)
        {
            return original
                .Replace("\\", replacer)
                .Replace("/", replacer)
                .Replace(":", replacer)
                .Replace("\"", replacer)
                .Replace("<", replacer)
                .Replace(">", replacer)
                .Replace("|", replacer);
        }
    }

    /// <summary>
    /// Helper class to deserialize Array Json. Interally uses FullSerializer
    /// Expected Format: {"Items":[ {...}, {...}, ... ] }
    /// </summary>
    public static class JsonHelper
    {

        public static string Serialize<T>(Type type, T[] array)
        {
            // Expected Format: {"Items":[ {...}, {...} ] }
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return StringSerializationAPI.Serialize(typeof(Wrapper<T>), wrapper);
        }

        public static T[] Deserialize<T>(string json)
        {
            // Expected Format: {"Items":[ {...}, {...} ] }
            Wrapper<T> wrapper = (Wrapper<T>)StringSerializationAPI.Deserialize(typeof(Wrapper<T>), json);
            return wrapper.Items;
        }        
        
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    /// <summary>
    /// The most basic use of FullSerializer without any bells or whistles.
    /// </summary>
    public static class StringSerializationAPI
    {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize(Type type, object value)
        {
            // serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        public static object Deserialize(Type type, string serializedState)
        {
            // step 1: parse the JSON data
            fsData data = fsJsonParser.Parse(serializedState);

            // step 2: deserialize the data
            object deserialized = null;
            _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
        }
    }
}