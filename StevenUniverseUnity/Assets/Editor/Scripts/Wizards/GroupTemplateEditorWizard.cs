/*
namespace StevenUniverse.FanGameEditor.Wizards
{
    public class GroupTemplateEditorWizard : ScriptableWizard
    {
        [SerializeField]
        private GroupInstanceEditor instance;

        [SerializeField]
        private GroupTemplate template;

        //Apply Bools
        [SerializeField]
        private bool applyAll = false;
        [SerializeField]
        private bool applyTiles = false;

        public static void CreateWizard(GroupInstanceEditor instance)
        {
            GroupTemplateEditorWizard createdWizard = DisplayWizard<GroupTemplateEditorWizard>("Group Template Editor", "Apply", "Cancel");
            createdWizard.Instance = instance;
        }

        public GroupInstanceEditor Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
                template = instance.GroupTemplate;
            }
        }

        void OnWizardCreate()
        {
            GroupTemplate instanceTemplate = instance.GroupTemplate;

            //Apply the selected attributes
            if (applyTiles)
            {
                instanceTemplate.TileInstances = template.TileInstances;
            }

            //Save the template
            instanceTemplate.Save();
        }

        void OnWizardUpdate()
        {
            if (applyAll)
            {
                applyTiles = true;
            }
        }

        void OnWizardOtherButton()
        {
            this.Close();
        }
    }
}*/

