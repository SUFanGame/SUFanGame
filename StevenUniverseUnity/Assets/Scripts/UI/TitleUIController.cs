using UnityEngine;
using UnityEngine.UI;
using StevenUniverse.FanGame.Extensions;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.UI
{
    public class TitleUIController : MonoBehaviour
    {
        public enum TitleMenu
        {
            Main
        }

        //TODO move this to an external application
        //Server Hosting info
        private const int DEFAULT_CONNECTIONS = 32;
        private const int DEFAULT_LISTEN_PORT = 25001;

        private bool loadMenu;
        private TitleMenu currentMenu;

        private GameObject mainPanel;

        protected void Start()
        {
            ResetToDefaults();
            mainPanel = gameObject.FindChildrenWithTag(Constants.MAIN_PANEL_TAG)[0];
        }

        protected void OnEnable()
        {
            ResetToDefaults();
        }

        private void ResetToDefaults()
        {
            loadMenu = true;
            currentMenu = TitleMenu.Main;
        }

        //TODO use this to check for changes in things like hostdata where you need to refresh a menu on change
        protected void Update()
        {
            if (loadMenu)
            {
                //Clear the main panel
                mainPanel.DestroyChildren();

                //Load the current menu
                switch (currentMenu)
                {
                    case TitleMenu.Main:
                        DisplayMainMenu();
                        break;
                    default:
                        throw new UnityException("invalid menu");
                }

                //We no longer need to load the menu
                loadMenu = false;
            }
        }

        private void ChangeMenu(TitleMenu newMenu)
        {
            currentMenu = newMenu;
            loadMenu = true;
        }

        //Display the Main Menu
        private void DisplayMainMenu()
        {
            //Main Menu Information
            mainPanel.AddText("Welcome to the SU Fan Game!");

            mainPanel.AddButton("Continue", delegate { GameController.Instance.ContinueFile(); });

            //TODO until an intro is built for SU, skip the intro. This will use the default player as defined in the player class if there is no JSON file.
            mainPanel.AddButton("New Game", delegate { GameController.Instance.ContinueFile(); /*GameController.Instance.StartNewGame();*/ });
        }
    }
}