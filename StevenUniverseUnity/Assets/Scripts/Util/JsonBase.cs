using System.Collections.Generic;
using System.IO;
using UnityEngine;
using StevenUniverse.FanGame.Characters;

namespace StevenUniverse.FanGame.Util
{
    [System.Serializable]
    public abstract class JsonBase<T> where T : JsonBase<T>
    {
        //Class
        private static Dictionary<string, T> instances = new Dictionary<string, T>();

        public static void ClearJsonCache()
        {
            instances.Clear();
        }

        public static T2 Get<T2>(string appDataPath) where T2 : T
        {
            System.Type t2Type = typeof(T2);
            System.Type entityType = typeof(Character);

            bool skipInstanceCache = t2Type.Equals(entityType) || t2Type.IsSubclassOf(entityType);

            if (skipInstanceCache)
            {
                return LoadInstanceFromJson<T2>(appDataPath);
            }
            else
            {
                try
                {
                    return (T2)instances[appDataPath];
                }
                catch (KeyNotFoundException)
                {
                    T2 loadedInstance = LoadInstanceFromJson<T2>(appDataPath);
                    if (loadedInstance != null)
                    {
                        instances.Add(appDataPath, loadedInstance);
                    }
                    return loadedInstance;
                }
            }
        }

        public static bool UseInstanceCache { get { return true; } }

        private static T2 LoadInstanceFromJson<T2>(string appDataPath) where T2 : JsonBase<T>
        {
            T2 loadedInstance = null;

            string absolutePath = Utilities.ExternalDataPath + "/" + appDataPath + ".json";
            if (File.Exists(absolutePath))
            {
                T2 testLoadedInstance = JsonUtility.FromJson<T2>(File.ReadAllText(absolutePath));

                //Verify that the loaded Instance was of the expected type
                if (testLoadedInstance.SerializedType == typeof(T2).Name)
                {
                    loadedInstance = testLoadedInstance;
                    loadedInstance.AppDataPath = appDataPath;
                    loadedInstance.Name = Utilities.ConvertFilePathToFileName(appDataPath);
                }
            }

            return loadedInstance;
        }

        //Instance
        [SerializeField] private string serializedType;

        private string appDataPath;
        private string name;

        public JsonBase()
        {
        }

        public virtual void Save()
        {
            if (string.IsNullOrEmpty(AppDataPath))
            {
                throw new UnityException("Can't save a JsonBase-Derived class if it doesn't have a defined AppDataPath!");
            }

            serializedType = this.GetType().Name;

            string savedJson = JsonUtility.ToJson(this, true);
            string outputAbsolutePath = Utilities.ExternalDataPath + "/" + AppDataPath + ".json";

            Utilities.WriteStringToFile(savedJson, outputAbsolutePath);
        }


        public string SerializedType
        {
            get { return serializedType; }
            set { serializedType = value; }
        }

        public string AppDataPath
        {
            get { return appDataPath; }
            set { appDataPath = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}