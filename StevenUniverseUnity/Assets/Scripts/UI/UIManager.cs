using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using StevenUniverse.FanGame.Characters;
using StevenUniverse.FanGame.UI.Components;

namespace StevenUniverse.FanGame.UI
{
    public static class UIManager
    {
        private const string COMPONENTS_PATH = @"Prefabs/UI/_Components/";

        private static GameObject buttonPrefab;

        static UIManager()
        {
            buttonPrefab = Resources.Load<GameObject>(COMPONENTS_PATH + "Button");
        }

        //UI Prefabs
        private static GameObject ButtonPrefab
        {
            get { return Resources.Load<GameObject>(COMPONENTS_PATH + "Button"); }
        }

        private static GameObject TextPrefab
        {
            get { return Resources.Load<GameObject>(COMPONENTS_PATH + "Text"); }
        }

        private static GameObject InputFieldPrefab
        {
            get { return Resources.Load<GameObject>(COMPONENTS_PATH + "InputField"); }
        }

        //Add a Text to the UI with a given content
        public static Text AddText(this GameObject parent, string content)
        {
            //Create the text as a child of the given parent
            GameObject go = GameObject.Instantiate<GameObject>(TextPrefab);

            //Assign the given message to the text
            Text t = go.GetComponent<Text>();
            t.text = content;

            go.transform.SetParent(parent.transform);
            //Return the text
            return t;
        }

        //Add an Input Field to the UI
        public static InputField AddInputField(this GameObject parent, InputField.ContentType contentType)
        {
            //Create the input field as a child of the given parent
            GameObject go = GameObject.Instantiate(InputFieldPrefab) as GameObject;
            go.transform.SetParent(parent.transform);

            //Assign the given content type to the input field
            InputField inputField = go.GetComponent<InputField>();
            inputField.contentType = contentType;

            //Return the input field
            return inputField;
        }

        //Add a Button to the UI with a given message and action
        public static Button AddButton(this GameObject parent, string message, UnityAction onClick)
        {
            //Create the button as a child of the given parent
            GameObject go = GameObject.Instantiate<GameObject>(buttonPrefab);

            //Assign the given message to the button
            Text t = go.transform.Find("Text").GetComponent<Text>();
            t.text = message;

            //Assign the given action to the button
            Button b = go.GetComponent<Button>();
            b.onClick.AddListener(onClick);

            go.transform.SetParent(parent.transform);

            //Return the button
            return b;
        }

        public static Selector AddSelector(this GameObject parent, Character target, string itemGroup,
            string[] itemNames)
        {
            //Instantiate the Selector
            string selectorResourcePath = "Prefabs/UI/_Components/Selector";
            GameObject selectorGameObject =
                GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(selectorResourcePath));
            selectorGameObject.transform.SetParent(parent.transform);

            Selector selector = selectorGameObject.GetComponent<Selector>();
            selector.Init(target, itemGroup, itemNames);

            return selector;
        }
    }
}