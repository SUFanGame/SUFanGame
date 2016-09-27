using UnityEngine;
using StevenUniverse.FanGame.Extensions;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.UI
{
    public class GameUIController : MonoBehaviour
    {
        public enum GameMenu
        {
            Start,
            Bag,
            User,
        }

        private bool loadMenu = true;
        private GameMenu currentMenu = GameMenu.Start;

        private GameObject mainPanel;

        //UI Prefabs
        private static GameObject PokemonUIPrefab
        {
            get { return Resources.Load("Prefabs/UI/PokemonUI/PokemonUI") as GameObject; }
        }

        private static GameObject WarpUIPrefab
        {
            get { return Resources.Load("Prefabs/UI/WarpUI/WarpUI") as GameObject; }
        }

        protected void Awake()
        {
            mainPanel = gameObject.FindChildrenWithTag(Constants.MAIN_PANEL_TAG)[0];
        }

        protected void Update()
        {
            if (loadMenu)
            {
                //Clear the main panel
                mainPanel.DestroyChildren();

                //Load the current menu
                switch (currentMenu)
                {
                    case GameMenu.Start:
                        DisplayStartMenu();
                        break;
                    case GameMenu.Bag:
                        DisplayBagMenu();
                        break;
                    case GameMenu.User:
                        DisplayUserMenu();
                        break;
                    default:
                        throw new UnityException("invalid menu");
                }

                //We no longer need to load the menu
                loadMenu = false;
            }
        }

        protected void OnDisable()
        {
            ChangeMenu(GameMenu.Start);
        }

        private void ChangeMenu(GameMenu newMenu)
        {
            currentMenu = newMenu;
            loadMenu = true;

            CloseSecondaryMenus();
        }

        public void DisplayStartMenu()
        {
            //mainPanel.AddButton("Bag", delegate { ChangeMenu(GameMenu.Bag); });
            //mainPanel.AddButton("User", delegate { ChangeMenu(GameMenu.User); });
            mainPanel.AddButton("Save", delegate { GameController.Instance.CurrentPlayer.SaveToJson(); });
        }

        private void CloseSecondaryMenus()
        {
            //No secondary menus in this stripped-down version of the framework, but there will eventuall be FE-style menus
        }

        public void DisplayBagMenu()
        {
            //GameController.Instance.ToggleBagUI(true);
            mainPanel.AddButton("Back", delegate { ChangeMenu(GameMenu.Start); });
        }

        public void DisplayUserMenu()
        {
            //GameController.Instance.ToggleUserUI(true);
            mainPanel.AddButton("Back", delegate { ChangeMenu(GameMenu.Start); });
        }
    }
}