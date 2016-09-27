using System;
using UnityEngine;

namespace StevenUniverse.FanGame.Data
{
    [System.Serializable]
    public abstract class DataType : IComparable<DataType>
    {
        [SerializeField]
        private string name;

        //Full constructor
        public DataType(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual int CompareTo(DataType other)
        {
            throw new Exception(string.Format("Cannot compare '{0}' to '{1}'", this.GetType().Name, other.GetType().Name));
        }
    }
}