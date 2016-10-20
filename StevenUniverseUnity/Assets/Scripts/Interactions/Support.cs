using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* Use:
   Support SupportComponent = GameObject.Find("SupportObject").GetComponent<Support>();
   SupportComponent.Dialog = SupportLoader.ImportSupport("Righty_Lefty_C");
   SupportComponent.enabled = true;
*/

namespace StevenUniverse.FanGame.Interactions
{
    public class Support : MonoBehaviour
    {
        public GameObject canvas;

        private ScriptLine[] dialog;
        private int count;

        private GameObject newCanvas;
        private Text spokenText;
        private Image leftPortrait;
        private Image rightPortrait;
        private GameObject nameplate;

        private float timestamp; //Prevent accidential button spamming


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

            //Make the gui!
            newCanvas = Instantiate(canvas) as GameObject;
            //Convenience references, this is super, super dependent on the prefab structure.
            //Reference SupportCanvas prefab for the child tree structure.
            spokenText = newCanvas.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>();
            nameplate = newCanvas.transform.GetChild(2).GetChild(0).gameObject;
            leftPortrait = newCanvas.transform.GetChild(1).gameObject.GetComponent<Image>();
            rightPortrait = newCanvas.transform.GetChild(0).gameObject.GetComponent<Image>();

            StartCoroutine(DoDialog());
        }

        public IEnumerator DoDialog()
        {
            while (count < dialog.Length)
            {
                ScriptLine curDialog = dialog[count];

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

                count++;

                // Now wait for the next key, modify this to submit button?
                yield return StartCoroutine(DelayedWaitForKeyDown(KeyCode.Space));
            }

            //Clear the GUI from the screen and return control
            Destroy(newCanvas);
            dialog = null; //reset the dialog
            //enabled = false; //disable itself for next support
        }

        public IEnumerator DelayedWaitForKeyDown(KeyCode keyCode)
        {
            timestamp = Time.time + .2f; //prevent accidential double-presses
            while (!Input.GetKeyDown(keyCode) || Time.time <= timestamp)
                yield return null;
            }
        
    }
}
