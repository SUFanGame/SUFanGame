using UnityEngine;

namespace StevenUniverse.FanGame.Data.DataTypes
{
    [System.Serializable]
    public class DataBool : DataType
    {
        [SerializeField]
        private bool data;

        public DataBool(string name, bool data) : base(name)
        {
            this.data = data;
        }

        public bool Data
        {
            get { return data; }
            set { data = value; }
        }

        public override int CompareTo(DataType other)
        {
            DataBool otherDataBool = other as DataBool;

            if (otherDataBool != null)
            {
                return Data.CompareTo(otherDataBool.Data);
            }
            else
            {
                return base.CompareTo(other);
            }
        }
    }
}