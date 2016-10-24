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

        //[SerializeField]
        //EditorInstanceMap tileMap_ = new EditorInstanceMap();

        private GroupTemplate groupTemplate;

        // Use this for initialization
        protected override void Start()
        {
            if (GroupInstance == null)
            {
                GroupInstance = new GroupInstance("", 0, 0, 0);
            }

            base.Start();

            //var instances = GetComponentsInChildren<TileInstanceEditor>();

            //// Add each of this group's tiles to it's tile map using their local position/elevation as the key.
            //foreach( var i in groupInstance.IndependantTileInstances )
            //{
            //    //Debug.LogFormat("Adding tile in tile group at {0}", index);
                
            //    tileMap_.AddInstance(i.transform.localPosition, i.Elevation, i);
            //}
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

        ///// <summary>
        ///// Retrieve a tile belonging to this group at the given positon and elevation.
        ///// The given values are assumed to be LOCAL, relative to the group.
        ///// </summary>
        ///// <param name="pos"></param>
        ///// <returns></returns>
        //public TileInstanceEditor GetTile( Vector2 pos, int elevation )
        //{
        //    //pos -= transform.position;
        //    //Debug.LogFormat("Getting tile {0} from group {1}", pos, name);
        //    return tileMap_.GetInstances( pos, elevation )[0] as TileInstanceEditor;
        //}
    }
}