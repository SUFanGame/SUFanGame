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

        private GroupTemplate groupTemplate;

        // Use this for initialization
        protected override void Start()
        {
            if (GroupInstance == null)
            {
                GroupInstance = new GroupInstance("", 0, 0, 0);
            }

            base.Start();
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
    }
}