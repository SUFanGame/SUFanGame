using System;
using StevenUniverse.FanGame.StrategyMap;
using UnityEngine;

namespace StevenUniverse.FanGame.Overworld
{
    [System.Serializable]
    public class Instance
    {
        [SerializeField] private string templateAppDataPath;
        [SerializeField] private int x;
        [SerializeField] private int y;
        [SerializeField] private int elevation;

        private Template template;

        //Constructor for on-the-fly instance creation
        public Instance(Template template, int x, int y, int elevation) : this(x, y, elevation)
        {
            SetTemplate(template);
        }

        public Instance(string templateAppDataPath, int x, int y, int elevation) : this(x, y, elevation)
        {
            TemplateAppDataPath = templateAppDataPath;
        }

        private Instance(int x, int y, int elevation)
        {
            this.x = x;
            this.y = y;
            Elevation = elevation;
        }

        public string TemplateAppDataPath
        {
            get { return templateAppDataPath; }
            set { templateAppDataPath = value; }
        }

        public IntVector2 Position
        {
            get { return new IntVector2(x,y); }
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        public int Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        public T GetTemplate<T>() where T : Template
        {
            if (template == null || !Application.isPlaying )
            {
                template = Template.Get<T>(TemplateAppDataPath);
            }

            return (T) template;
        }

        public void SetTemplate(Template newTemplate)
        {
            template = newTemplate;
        }

        public override string ToString()
        {
            return string.Format("Template: {0}, Position: {1}, Elevation: {2}", templateAppDataPath, Position, Elevation);
        }
    }
}