using UnityEngine;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Overworld.Templates;

namespace StevenUniverse.FanGame.OverworldEditor
{
    [ExecuteInEditMode]
    public class TileInstanceEditor : InstanceEditor
    {
        [SerializeField] private TileInstance tileInstance;

        private TileTemplate tileTemplate;

        // Use this for initialization
        protected override void Start()
        {
            if (TileInstance == null)
            {
                TileInstance = new TileInstance("", 0, 0, 0);
            }

            //TileTemplate = TileInstance.TileTemplate;
            base.Start();
        }

        public TileInstance TileInstance
        {
            get { return tileInstance; }
            set { tileInstance = value; }
        }

        public TileTemplate TileTemplate
        {
            get
            {
                if (tileTemplate == null)
                {
                    TileTemplate = TileInstance.TileTemplate;
                }
                return tileTemplate;
            }
            private set { tileTemplate = value; }
        }

        public override Instance Instance
        {
            get { return TileInstance; }
        }

        public int GlobalElevation
        {
            get
            {
                int globalElevation = Elevation;

                Transform parent = transform.parent;
                if (parent != null)
                {
                    GroupInstanceEditor parentGroupInstanceEditor = parent.GetComponent<GroupInstanceEditor>();
                    if (parentGroupInstanceEditor != null)
                    {
                        globalElevation += parentGroupInstanceEditor.Elevation;
                    }
                }

                return globalElevation;
            }
        }

        public void UpdateSortingOrder()
        {
            GetComponent<SpriteRenderer>().sortingOrder = (GlobalElevation*100) +
                                                          (TileInstance.TileTemplate).TileLayer.SortingValue;
        }
    }
}