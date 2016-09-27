using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.OverworldEditor;
using StevenUniverse.FanGameEditor.Tools;

namespace StevenUniverse.FanGameEditor.Wizards
{
    public class TileTemplateEditorWizard : ScriptableWizard
    {
        [SerializeField] private List<TileInstanceEditor> instances;

        [SerializeField] private TileTemplate template;

        //Apply Bools
        [SerializeField] private bool applyAll = false;
        [SerializeField] private bool applyAnimationSpriteNames = false;
        [SerializeField] private bool applySyncAnimation = false;
        [SerializeField] private bool applySecondsPerFrame = false;
        [SerializeField] private bool applyTileModeName = false;
        [SerializeField] private bool applyTileLayerName = false;
        [SerializeField] private bool applyIsGrounded = false;

        public static void CreateWizard(TileInstanceEditor instance)
        {
            CreateWizard(new TileInstanceEditor[] {instance});
        }

        public static void CreateWizard(TileInstanceEditor[] instances)
        {
            TileTemplateEditorWizard createdWizard = DisplayWizard<TileTemplateEditorWizard>("Tile Template Editor",
                "Apply", "Cancel");
            createdWizard.Instances = instances;
        }

        public TileInstanceEditor[] Instances
        {
            get { return instances.ToArray(); }
            set
            {
                instances = new List<TileInstanceEditor>(value);

                //If there is at least one instance, set the template to the first TileTemplate. Otherwise, make the template null.
                template = (Instances.Length > 0) ? Instances[0].TileTemplate : null;
            }
        }

        void OnWizardCreate()
        {
            //Create a variable to keep track of progress for the progress bar
            float progress = 0f;
            //Display the progress bar
            EditorUtility.DisplayProgressBar("Applying Tile Templates", "Initializing...", progress);

            foreach (TileInstanceEditor instance in instances)
            {
                TileTemplate instanceTemplate = instance.TileTemplate;

                //Display the progress bar
                EditorUtility.DisplayProgressBar("Applying Tile Templates", instance.name, progress);

                //Apply the selected attributes
                if (applyAnimationSpriteNames)
                {
                    instanceTemplate.AnimationSpriteNames = template.AnimationSpriteNames;
                }
                if (applySyncAnimation)
                {
                    instanceTemplate.SyncAnimation = template.SyncAnimation;
                }
                if (applySecondsPerFrame)
                {
                    instanceTemplate.SecondsPerFrame = template.SecondsPerFrame;
                }
                if (applyTileModeName)
                {
                    instanceTemplate.TileModeName = template.TileModeName;
                }
                if (applyTileLayerName)
                {
                    instanceTemplate.TileLayerName = template.TileLayerName;
                }
                if (applyIsGrounded)
                {
                    instanceTemplate.IsGrounded = template.IsGrounded;
                }

                //Save the template
                instanceTemplate.Save();

                TemplateTools.RegenerateAndSelectTileTemplate(instanceTemplate);

                //Increment the progress
                progress += 1/(float) instances.Count;
            }

            EditorUtility.ClearProgressBar();
        }

        void OnWizardUpdate()
        {
            if (applyAll)
            {
                applyAnimationSpriteNames = true;
                applySyncAnimation = true;
                applySecondsPerFrame = true;
                applyTileModeName = true;
                applyTileLayerName = true;
                applyIsGrounded = true;
            }
        }

        void OnWizardOtherButton()
        {
            this.Close();
        }
    }
}