using UnityEngine;

namespace StevenUniverse.FanGame.Data.DataTypes
{
    [System.Serializable]
    public class DataInt : DataType
    {
        [SerializeField]
        private int data;

        public DataInt(string name, int data) : base(name)
        {
            this.data = data;
        }

        public int Data
        {
            get { return data; }
            set { data = value; }
        }

        public override int CompareTo(DataType other)
        {
            DataInt otherDataInt = other as DataInt;

            if (otherDataInt != null)
            {
                return Data.CompareTo(otherDataInt.Data);
            }
            else
            {
                return base.CompareTo(other);
            }
        }
    }
}