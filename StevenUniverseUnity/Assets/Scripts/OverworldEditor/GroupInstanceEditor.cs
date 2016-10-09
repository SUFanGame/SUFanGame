using UnityEngine;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Overworld.Templates;

namespace StevenUniverse.FanGame.OverworldEditor
{
    [ExecuteInEditMode]
    public class GroupInstanceEditor : InstanceEditor
    {
        [SerializeField] private GroupInstance groupInstance;

        [SerializeField]
        EditorInstanceMap tileMap_ = new EditorInstanceMap();

        private GroupTemplate groupTemplate;

        // Use this for initialization
        protected override void Start()
        {
            if (GroupInstance == null)
            {
                GroupInstance = new GroupInstance("", 0, 0, 0);
            }

            base.Start();

            var instances = GetComponentsInChildren<TileInstanceEditor>();

            foreach( var i in instances )
            {
                Vector2 pos = i.transform.localPosition;
                int elevation = i.Elevation;
                tileMap_.AddInstance(new Vector3(pos.x, pos.y, elevation), i);
            }
        }
        

        public GroupInstance GroupInstance
        {
            get { return groupInstance; }
            set { groupInstance = value; }
        }

        public GroupTemplate GroupTemplate
        {
            get
            {
                if (groupTemplate == null)
                {
                    GroupTemplate = GroupInstance.GroupTemplate;
                }
                return groupTemplate;
            }
            private set { groupTemplate = value; }
        }

        public override Instance Instance
        {
            get { return GroupInstance; }
        }

        public TileInstanceEditor GetTile( Vector3 pos )
        {
            Debug.LogFormat("Getting tile {0} from group {1}", pos, name);
            return tileMap_.GetInstances( pos )[0] as TileInstanceEditor;
        }
    }
}