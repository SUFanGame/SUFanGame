using System.Collections.Generic;
using UnityEngine;

namespace StevenUniverse.FanGame.Util
{
    public abstract class EnhancedEnum<T> where T : EnhancedEnum<T>
    {
        //Instance
        private string name;

        protected EnhancedEnum(string name)
        {
            Name = name;
        }

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        //Static Instances
        static EnhancedEnum()
        {
            //Manually calls the static constructor of the child class to ensure it has defined the instances before they are used
            //TODO is there a better way to do this?
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        }

        private static List<T> instances = new List<T>();

        public static T Get(string instanceName)
        {
            foreach (T instance in instances)
            {
                if (instance.Name == instanceName)
                {
                    return instance;
                }
            }

            throw new UnityException(string.Format("{0} is not a valid instance name!", instanceName));
        }

        protected static void Add(T instance)
        {
            instances.Add(instance);
        }

        public static T[] Instances
        {
            get { return instances.ToArray(); }
        }
    }
}