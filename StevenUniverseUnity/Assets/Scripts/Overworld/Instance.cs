using UnityEngine;

namespace StevenUniverse.FanGame.Overworld
{
    [System.Serializable]
    public class Instance : ICoordinated
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
            X = x;
            Y = y;
            Elevation = elevation;
        }

        public string TemplateAppDataPath
        {
            get { return templateAppDataPath; }
            set { templateAppDataPath = value; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
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
                //Debug.Log(template.GetType());
            }
            return (T) template;
        }

        public void SetTemplate(Template newTemplate)
        {
            template = newTemplate;
        }

        public Vector3 Position
        {
            get { return new Vector3(X, Y, 0); }
            set
            {
                X = Mathf.FloorToInt(value.x);
                Y = Mathf.FloorToInt(value.y);
            }
        }

        public override string ToString()
        {
            return string.Format("Template: {0}, X: {1}, Y: {2}, Elevation: {3}", templateAppDataPath, X, Y, Elevation);
        }
    }
}