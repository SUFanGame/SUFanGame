using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SUGame.Interactions
{
    public class SupportPanel : MonoBehaviour
    {
        //TODO: make the canvas actually destroy on finish

        static SupportPanel instance_;
        public static SupportPanel Instance { get { return instance_; } }

        [SerializeField]
        public SupportCanvas prefab;
        private SupportCanvas canvas_;
        private static bool preservedCanvas = false;

        //private GameObject newCanvas;
        //public GameObject canvas;
        // Typically in Unity referencing other objects is done like this,
        // serialize the private fields and set them through the inspector.
        // This way you can move or rename the objects and the references will still be maintained.
        //[SerializeField]
        //private Text spokenText;
        //[SerializeField]
        //private Image leftPortrait;
        //[SerializeField]
        //private Image rightPortrait;
        //
        //// Speaker nameplates are disabled when not talking
        //[SerializeField]
        //GameObject nameplateLeft;
        //[SerializeField]
        //GameObject nameplateRight;
        //// Retrieve our text components through GetComponent in awake.
        //Text nameplateLeftText;
        //Text nameplateRightText;
        //private GameObject nameplate;

        public static bool DestroyOnEnd { get; set; }

        public static ScriptLine[] Dialog { get; set; }

        public void Awake()
        {
            //Debug.Log("Woke up");
            DestroyOnEnd = true; //Dialog destroyed after script is done by default
            Dialog = null;
            instance_ = this;
        }

        // Alternative to storing all the relevant references in the caller - have the 
        // canvas object keep track of what it needs to can use a simple interface
        // to interact with it as needed.
        /// <summary>
        /// Perform the current dialogue on the given canvas.
        /// </summary>
        public static IEnumerator DoDialogWithCanvas()
        {

            if (Dialog == null)
            {
                throw new UnityException("There's no dialog set!");
            }

            if (!preservedCanvas)
            {
                instance_.canvas_ = Instantiate<SupportCanvas>(instance_.prefab);
                instance_.canvas_.gameObject.SetActive(true);
                //Debug.Log("Canvas activated");
            }

            foreach (ScriptLine curDialog in Dialog)
            {
                int side = curDialog.CurrentSpeaker == curDialog.LeftSpeaker ? 0 : 1;
                instance_.canvas_.SetDialogue(side, curDialog.CurrentSpeaker, curDialog.Line);
                var leftSprite = Resources.Load<Sprite>
                    ("Characters/Test/" + curDialog.LeftSpeaker + "/" + curDialog.LeftExpr);
                var rightSprite = Resources.Load<Sprite>
                    ("Characters/Test/" + curDialog.RightSpeaker + "/" + curDialog.RightExpr);
                instance_.canvas_.SetPortraits(leftSprite, rightSprite);
                // Prevent double key presses
                yield return new WaitForSeconds(.2f);
                // Now wait for the next key
                yield return new WaitWhile(() => !Input.GetButtonDown("Submit"));
            }

            if (DestroyOnEnd)
            {
                instance_.canvas_.gameObject.SetActive(false);
                preservedCanvas = false;

                Destroy(instance_.canvas_); //For some reason it doesn't get destroyed?
            }
            else
            {
                //Leave last line preserved on screen, it will be updated when the next dialog loads
                DestroyOnEnd = true; //reset to default so we don't leave false by accident
                preservedCanvas = true; //True if the canvas wasn't destroyed so we don't load twice
            }
            //Debug.Log("Dialog finished");
            Dialog = null;
        }

        
        //public IEnumerator DoDialog()
        //{
        //    nameplateLeftText = nameplateLeft.GetComponentInChildren<Text>();
        //    nameplateRightText = nameplateRight.GetComponentInChildren<Text>();
        //    if (dialog == null)
        //    {
        //        throw new UnityException("There's no dialog set!");
        //    }
        //
        //    if (!preservedCanvas)
        //    {
        //        //Make the gui!
        //        newCanvas = Instantiate(canvas) as GameObject;
        //
        //        //Convenience references, this is super, super dependent on the prefab structure.
        //        //Reference SupportCanvas prefab for the child tree structure.
        //        spokenText = newCanvas.transform.GetChild(2).GetChild(1).gameObject.GetComponent<Text>();
        //        //nameplate = newCanvas.transform.GetChild(2).GetChild(0).gameObject;
        //        leftPortrait = newCanvas.transform.GetChild(1).gameObject.GetComponent<Image>();
        //        rightPortrait = newCanvas.transform.GetChild(0).gameObject.GetComponent<Image>();
        //    }
        //
        //    foreach (ScriptLine curDialog in dialog)
        //    {
        //
        //        //Update the nameplate if speaker changed sides
        //        //These numbers need to reference the prefab's settings, currently hardcoded
        //        //anchor is middle top of the textbox
        //        
        //        //offsetMax The offset of the upper right corner of the rectangle relative to the upper right anchor.
        //        //offsetMin The offset of the lower left corner of the rectangle relative to the lower left anchor.
        //        //RectTransform nameplateBox = nameplate.GetComponent<RectTransform>();
        //        
        //        if (curDialog.CurrentSpeaker == curDialog.RightSpeaker)
        //        {  //RIGHT SIDE
        //            nameplateLeft.SetActive(false);
        //            nameplateRight.SetActive( true );
        //            nameplateRightText.text = curDialog.CurrentSpeaker;
        //            //nameplateBox.offsetMin = new Vector2(-140f + 265f, -15f);
        //            //nameplateBox.offsetMax = new Vector2(265, 15f);
        //        }
        //        else
        //        { //LEFT SIDE
        //            nameplateRight.SetActive( false );
        //            nameplateLeft.SetActive(true);
        //            nameplateLeftText.text = curDialog.CurrentSpeaker;
        //
        //            // nameplateBox.offsetMin = new Vector2(-265f, -15f);
        //            //nameplateBox.offsetMax = new Vector2(140f - 265f, 15);
        //        }
        //
        //        //Change the text and the name of the speaker
        //        //Text is actually a child object of nameplate, will have to fetch it
        //        //nameplate.transform.GetChild(0).gameObject.GetComponent<Text>().text = curDialog.CurrentSpeaker;
        //        spokenText.text = curDialog.Line;
        //
        //        //Swap the replacee's face with the image corresponding to newExpression
        //        //Check if this is the proper way to load sprites
        //        rightPortrait.sprite = Resources.Load<Sprite>
        //            ("Characters/Test/" + curDialog.RightSpeaker + "/" + curDialog.RightExpr);
        //        leftPortrait.sprite = Resources.Load<Sprite>
        //            ("Characters/Test/" + curDialog.LeftSpeaker + "/" + curDialog.LeftExpr);
        //
        //        // Prevent double key presses
        //        yield return new WaitForSeconds(.2f);
        //        // Now wait for the next key, modify this to submit button?
        //        yield return new WaitWhile(() => !Input.GetButtonDown("Submit"));
        //    }
        //
        //    if (destroyOnEnd)
        //    {
        //        //Clear the GUI from the screen
        //        Destroy(newCanvas);
        //        preservedCanvas = false;
        //    }
        //    else
        //    {
        //        //Leave last line preserved on screen, it will be updated when the next dialog loads
        //        destroyOnEnd = true; //reset to default so we don't leave false by accident
        //        preservedCanvas = true;
        //    }
        //
        //    dialog = null; //reset the dialog
        //
        //}
    }
}
