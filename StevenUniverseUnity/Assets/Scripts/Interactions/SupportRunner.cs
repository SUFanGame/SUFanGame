using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace StevenUniverse.FanGame.Interactions
{
    public class SupportRunner : MonoBehaviour
    {
        /* Use this inside the controller that initiates a support:
        SupportRunner SupportComponent = GameObject.Find("SupportObject").GetComponent<Support>();
        SupportComponent.Dialog = SupportLoader.ImportSupport("Righty_Lefty_C");
        SupportComponent.enabled = true;
        */

        /* TODO:
         * Control isn't totally taken away from main game. Need to disable other input.
         * Remove hardcoded values
         * Check if it's inefficient to do a Resources.Load<>() for every single line or if caching is built in
         */

        public GameObject canvas;

        private ScriptLine[] dialog;

        private GameObject newCanvas;
        private Text spokenText;
        private Image leftPortrait;
        private Image rightPortrait;
        private GameObject nameplate;

        private bool dontDestroyOnEnd = false; //Dialog destroyed after script is done by default
        private bool preservedCanvas = false; //True if the canvas wasn't destroyed so we don't load twice

        public bool DontDestroyOnEnd
        {
            set { dontDestroyOnEnd = value; }
        }


        public ScriptLine[] Dialog
        {
            set { dialog = value; }
        }

        public void OnEnable()
        {
            if (dialog == null)
            {
                throw new UnityException("There's no dialog set!");
            }

            if (!preservedCanvas)
            {
                //Make the gui!
                newCanvas = Instantiate(canvas) as GameObject;
                //Convenience references, this is super, super dependent on the prefab structure.
                //Reference SupportCanvas prefab for the child tree structure.
                spokenText = newCanvas.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>();
                nameplate = newCanvas.transform.GetChild(2).GetChild(0).gameObject;
                leftPortrait = newCanvas.transform.GetChild(1).gameObject.GetComponent<Image>();
                rightPortrait = newCanvas.transform.GetChild(0).gameObject.GetComponent<Image>();
            }

            StartCoroutine(DoDialog());
        }

        public IEnumerator DoDialog()
        {
            foreach (ScriptLine curDialog in dialog)
            {

                //Update the nameplate if speaker changed sides
                //These numbers need to reference the prefab's settings, currently hardcoded
                //offsetMin = new Vector2(FROM LEFT MOVE RIGHT, FROM BOTTOM MOVE UP)
                //offsetMax = new Vector2(FROM RIGHT MOVE RIGHT, FROM TOP MOVE UP)
                RectTransform nameplateBox = nameplate.GetComponent<RectTransform>();
                if (curDialog.CurrentSpeaker != curDialog.RightSpeaker)
                {  //RIGHT SIDE
                    nameplateBox.offsetMin = new Vector2(15f, 70f);
                    nameplateBox.offsetMax = new Vector2(-270f, 15f);
                }
                else
                { //LEFT SIDE
                    nameplateBox.offsetMin = new Vector2(270f, 70f);
                    nameplateBox.offsetMax = new Vector2(-15f, 15f);
                }

                //Change the text and the name of the speaker
                //Text is actually a child object of nameplate, will have to fetch it
                nameplate.transform.GetChild(0).gameObject.GetComponent<Text>().text = curDialog.CurrentSpeaker;
                spokenText.text = curDialog.Line;

                //Swap the replacee's face with the image corresponding to newExpression
                //Check if this is the proper way to load sprites
                rightPortrait.sprite = Resources.Load<Sprite>
                    ("Characters/Test/" + curDialog.RightSpeaker + "/" + curDialog.RightExpr);
                leftPortrait.sprite = Resources.Load<Sprite>
                    ("Characters/Test/" + curDialog.LeftSpeaker + "/" + curDialog.LeftExpr);

                // Prevent double key presses
                yield return new WaitForSeconds(.2f);
                // Now wait for the next key, modify this to submit button?
                yield return new WaitWhile(() => !Input.GetKeyDown(KeyCode.Space));
            }

            //Clear the GUI from the screen and return control
            if (!dontDestroyOnEnd)
            {
                Destroy(newCanvas);
                preservedCanvas = false;
            }
            else
            {
                dontDestroyOnEnd = false; //reset to default so we don't leave accident
                preservedCanvas = true;
            }
            dialog = null; //reset the dialog

            gameObject.SetActive(false); //disable itself for next support
        }
        
    }
}
