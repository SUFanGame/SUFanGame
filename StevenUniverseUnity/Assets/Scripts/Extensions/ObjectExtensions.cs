using System.Collections.Generic;
using UnityEngine;

namespace StevenUniverse.FanGame.Extensions
{
    public static class ObjectExtensions
    {
        //Returns all Objects of a certain type from an array of Objects
        public static T[] GetObjects<T>(this Object[] objects) where T : Object
        {
            List<T> convertedObjects = new List<T>();

            foreach (Object o in objects)
            {
                T convertedObject = o as T;
                if (convertedObject != null)
                {
                    convertedObjects.Add(convertedObject);
                }
            }

            return convertedObjects.ToArray();
        }
    }
}