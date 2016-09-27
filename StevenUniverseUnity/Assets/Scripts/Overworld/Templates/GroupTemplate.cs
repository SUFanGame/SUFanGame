using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Overworld.Instances;

namespace StevenUniverse.FanGame.Overworld.Templates
{
    [System.Serializable]
    public class GroupTemplate : Template
    {
        [SerializeField] private List<TileInstance> tiles;

        public static GroupTemplate GetGroupTemplate(string groupTemplateLocal)
        {
            return Get<GroupTemplate>(groupTemplateLocal);
        }

        public GroupTemplate(TileInstance[] tiles) : base()
        {
            TileInstances = tiles;
        }

        public TileInstance[] TileInstances
        {
            get { return tiles.ToArray(); }
            set { tiles = new List<TileInstance>(value); }
        }
    }
}