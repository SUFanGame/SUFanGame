using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using SUGame.StrategyMap.Characters.Actions;
using System.Linq;

namespace SUGame.StrategyMap.UI
{
    // TODO : Re-use buttons rather than destroying them each time the menu is hidden.
    //        Account for the window appearing offscreen - snap it to always be inside the canvas

    /// <summary>
    /// Popup UI of valid actions for when a character is selected.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class CharacterActionsUI : MonoBehaviour
    {
        public Button pfb_ActionButton_;

        static CanvasGroup canvasGroup_;

        /// <summary>
        /// Buffer used when reading valid actions from a character. Used to avoid unneeded allocations.
        /// </summary>
        static List<CharacterAction> actionsBuffer_ = new List<CharacterAction>();

        public static CharacterActionsUI Instance { get; private set; }

        public static System.Action<CharacterAction> OnActionSelected_;

        Transform target_;

        RectTransform rt_;
        RectTransform canvasRT_;

        static Vector3[] corners_ = new Vector3[4];

        void Awake()
        {
            canvasGroup_ = GetComponent<CanvasGroup>();
            Instance = this;
            rt_ = transform as RectTransform;
            canvasRT_ = GetComponentInParent<Canvas>().transform as RectTransform;
        }

        /// <summary>
        /// Show the valid actions for the given character in a popup UI beside the character.
        /// </summary>
        public static void Show( MapCharacter character )
        {
            actionsBuffer_.Clear();
            ClearButtons();
            character.GetActions(actionsBuffer_);

            if( actionsBuffer_.Count == 0 )
            {
                Debug.LogWarningFormat("Attempting to load actions UI for {0}, but they have no actions!", character.name);
                return;
            }

            Instance.target_ = character.transform;

            UpdatePosition();

            // Create buttons mapping to our valid actions.
            for( int i = 0; i < actionsBuffer_.Count; ++i )
            {
                var action = actionsBuffer_[i];

                // Skip move action since it's implicit when a character is first clicked.
                if (action is MoveAction)
                    continue;

                var newButton = ((Button)Instantiate(Instance.pfb_ActionButton_, Instance.transform, false)).GetComponent<Button>();
                newButton.gameObject.layer = Instance.gameObject.layer;
                var text = newButton.GetComponentInChildren<Text>();
                text.text = action.UIName;
                newButton.onClick.AddListener(()=>ForwardAction(action));
                // Hide our UI when an option is selected.
                newButton.onClick.AddListener(Hide);
            }

            canvasGroup_.alpha = 1f;
            canvasGroup_.blocksRaycasts = true;
 
        }

        /// <summary>
        /// Forward selected action to UI listeners.
        /// </summary>
        /// <param name="selectedAction"></param>
        static void ForwardAction(CharacterAction selectedAction)
        {
            if (OnActionSelected_ != null)
                OnActionSelected_.Invoke(selectedAction);
        }

        static void UpdatePosition()
        {
            if (Instance.target_ == null)
                return;

            var windowPos = RectTransformUtility.WorldToScreenPoint(Camera.main, Instance.target_.position + Vector3.right + Vector3.up);

            Instance.transform.position = windowPos;

            Instance.SnapToParent(corners_, Instance.rt_, Instance.canvasRT_);
            
        }

        void LateUpdate()
        {
            UpdatePosition();
        }

        /// <summary>
        /// Hide the UI. Currently destroys any previously made buttons. These should be pooled instead.
        /// </summary>
        public static void Hide()
        {
            canvasGroup_.alpha = 0;
            canvasGroup_.blocksRaycasts = false;
        }

        static void ClearButtons()
        {

            var buttons = Instance.GetComponentsInChildren<Button>();

            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    Destroy(button.gameObject);
                }
            }
        }

        // TODO : This and SnapInto are probably better off in a utility class.
        void SnapToParent( Vector3[] corners, RectTransform child, RectTransform parent )
        {
            // Child corners in world space
            child.GetWorldCorners(corners);
            // Get our rect from bottom left and top right corners.
            Rect childRect = Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);

            // Same for our parent.
            parent.GetWorldCorners(corners);
            Rect parentRect = Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);

            // Get the snapped rect.
            childRect.position = SnapInto(childRect, parentRect);

            // Assign our transform to the snapped rect.
            child.offsetMin = childRect.min;
            child.offsetMax = childRect.max;
        }
        
        void OnGUI()
        {
            if (!isActiveAndEnabled || rt_ == null)
                return;
            //corners_ = GetSnappedCorners(corners_, rt_, canvasRT_);
            //GUILayout.Label("Rect: " + string.Join(",", corners_.Select(c=>c.ToString()).ToArray() ));
            //GUILayout.Label("Anchored pos: " + rt_.anchoredPosition);
        }

        /// <summary>
        /// Returns a new position for snapper where it's entirely contained with container.
        /// </summary>
        public static Vector2 SnapInto(Rect snapper, Rect container)
        {
            snapper.x = Mathf.Max(snapper.x, container.xMin);
            snapper.y = Mathf.Max(snapper.y, container.yMin);
            snapper.x = Mathf.Min(snapper.x, container.xMax - snapper.width);
            snapper.y = Mathf.Min(snapper.y, container.yMax - snapper.height);

            //Debug.LogFormat("Snapping in, NewPos: {0}", snapper.position );

            return snapper.position;
        }
    }
}
