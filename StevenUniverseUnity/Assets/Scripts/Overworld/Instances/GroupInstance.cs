using System.Collections.Generic;
using StevenUniverse.FanGame.Overworld.Templates;

namespace StevenUniverse.FanGame.Overworld.Instances
{
    [System.Serializable]
    public class GroupInstance : Instance
    {
        public GroupInstance(Template template, int x, int y, int elevation) : base(template, x, y, elevation)
        {
        }

        public GroupInstance(string templateAppDataPath, int x, int y, int elevation)
            : base(templateAppDataPath, x, y, elevation)
        {
        }

        public GroupTemplate GroupTemplate
        {
            get { return GetTemplate<GroupTemplate>(); }
            set { SetTemplate(value); }
        }

        //Build a list of tile instances with their global positions and elevations by combining their local valus with the group's values
        public TileInstance[] IndependantTileInstances
        {
            get
            {
                List<TileInstance> tileInstances = new List<TileInstance>();
                foreach (TileInstance tileInstance in GroupTemplate.TileInstances)
                {
                    tileInstances.Add(new TileInstance(tileInstance.TileTemplate, tileInstance.X + X, tileInstance.Y + Y,
                        tileInstance.Elevation + Elevation));
                }
                return tileInstances.ToArray();
            }
        }
    }
}