using System.Collections.Generic;
using UnityEngine;

namespace StevenUniverse.FanGame.Extensions
{
    public static class GameObjectExtensions
    {
        //Get components of type in array of GameObjects
        public static T[] GetComponents<T>(this GameObject[] gameObjects) where T : Component
        {
            List<T> components = new List<T>();

            //Loop through all the GameObjects
            foreach (GameObject go in gameObjects)
            {
                //Cache the T component, if there is one
                T component = go.GetComponent<T>();

                //If there was a T component, add it to the list
                if (component != null)
                {
                    components.Add(component);
                }
            }

            return components.ToArray();
        }

        public static void DestroyChildren(this GameObject parent)
        {
            //Cache the transform component of the parent GameObject to clear
            Transform parentTransform = parent.transform;

            //Destroy all the children of the panel
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                GameObject go = parentTransform.GetChild(i).gameObject;
                GameObject.Destroy(go);
            }
        }

        public static void DestroyChildrenWithName(this GameObject parent, string targetName)
        {
            //Cache the transform component of the parent GameObject to clear
            Transform parentTransform = parent.transform;

            //Destroy all the children of the panel
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                GameObject go = parentTransform.GetChild(i).gameObject;
                if (go.name == targetName)
                {
                    GameObject.Destroy(go);
                }
            }
        }

        public static string GetHierarchy(this GameObject go)
        {
            string hierarchy = go.name;

            Transform parent = go.transform.parent;

            if (parent != null)
            {
                hierarchy = parent.gameObject.GetHierarchy() + "/" + hierarchy;
            }

            return hierarchy;
        }

        public static GameObject GetHighestParent(this GameObject gameObject)
        {
            Transform highestParent = gameObject.transform;
            while (highestParent.parent != null)
            {
                highestParent = highestParent.parent;
            }
            return highestParent.gameObject;
        }

        //Same as GetComponentsInChildren but doesn't include the parent
        public static T[] GetComponentsInJustChildren<T>(this GameObject parent) where T : Component
        {
            Transform parentTransform = parent.transform;

            List<T> componentsInJustChildren = new List<T>();
            foreach (Transform child in parentTransform)
            {
                T t = child.GetComponent<T>();
                if (t != null)
                {
                    componentsInJustChildren.Add(t);
                }
            }

            return componentsInJustChildren.ToArray();
        }

        public static GameObject[] GetAllChildren(this GameObject parent)
        {
            Transform[] childTransforms = parent.GetComponentsInJustChildren<Transform>();

            List<GameObject> childGameObjects = new List<GameObject>();
            foreach (Transform t in childTransforms)
            {
                childGameObjects.Add(t.gameObject);
            }
            return childGameObjects.ToArray();
        }

        public static GameObject[] FindChildrenWithTag(this GameObject parent, string tag)
        {
            GameObject[] children = parent.GetAllChildren();
            List<GameObject> childrenWithTag = new List<GameObject>();

            foreach (GameObject go in children)
            {
                if (tag == go.tag)
                {
                    childrenWithTag.Add(go);
                }
            }
            return childrenWithTag.ToArray();
        }

        //TODO refactor to be "FindChildrenWithName" like you did for "FindChildrenWithTag"
        public static GameObject FindChildWithName(this GameObject parent, string name)
        {
            foreach (Transform child in parent.transform)
            {
                if (name == child.name)
                {
                    return child.gameObject;
                }
            }

            return null;
        }
    }
}