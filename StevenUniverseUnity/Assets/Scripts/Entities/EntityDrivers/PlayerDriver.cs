using UnityEngine;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Entities.EntityDrivers
{
    public class PlayerDriver : EntityDriver
    {
        private const float COOLDOWN_TIME = 0.075f;

        //Keycodes
        private const KeyCode A_KEY = KeyCode.P;
        private const KeyCode B_KEY = KeyCode.O;

        //Input
        //Directional Input
        Vector3 directionalInput = Vector3.zero;
        //Button input pressed
        private bool aButtonPressed = false;
        private bool bButtonPressed = false;
        //Button input
        private bool aButton = false;
        private bool bButton = false;

        //Camera Controller
        private PlayerCameraController playerCameraController;

        private float timeOfLastDirectionChange;

        protected override void Awake()
        {
            base.Awake();

            //Don't destroy this
            DontDestroyOnLoad(transform.gameObject);

            //TODO delete, this is to make the 'not used' errors go away
            string.Format("{0}, {1}, {2}, {3}", aButton, bButton, aButtonPressed, bButtonPressed);

            string saveAppDataPath = "Saves/Player";

            Player loadedPlayer = Player.GetPlayer(saveAppDataPath);

            if (loadedPlayer == null)
            {
                loadedPlayer = Player.CreateDefaultPlayer();
            }

            SourceEntity = loadedPlayer;

            timeOfLastDirectionChange = 0f;
            SourceEntity.OnDirectionChange += delegate { timeOfLastDirectionChange = Time.time; };
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        public void SaveToJson()
        {
            Debug.Log("saving");
            SourcePlayer.Save();
        }

        protected override void Update()
        {
            base.Update();

            //Directional input
            directionalInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

            //Button input pressed
            if (!aButtonPressed)
            {
                aButtonPressed = Input.GetKeyDown(A_KEY);
            }
            if (!bButtonPressed)
            {
                bButtonPressed = Input.GetKeyDown(B_KEY);
            }
            //Button input
            aButton = Input.GetKey(A_KEY);
            bButton = Input.GetKey(B_KEY);

            //Toggle Menu
            if (Input.GetKeyDown(KeyCode.Return) && GameController.Instance.ControlEnabled)
            {
                GameController.Instance.ToggleGameUI();
            }

            if (aButtonPressed)
            {
                //Interact with next entity
                if (GameController.Instance.ControlEnabled)
                {
                    EntityDriver nextEntity = FindNextEntity(SourcePlayer.CurrentDirection);
                    if (nextEntity != null)
                    {
                        nextEntity.SourceEntity.Interact(this.SourcePlayer);
                    }
                }
            }

            //Reset the "Pressed" inputs
            aButtonPressed = false;
            bButtonPressed = false;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsAtTarget)
            {
                if (GameController.Instance.ControlEnabled)
                {
                    //Move the Target position by the input if there is exclusively horizontal or vertical input
                    if ((directionalInput.x != 0 && directionalInput.y == 0) ||
                        (directionalInput.y != 0 && directionalInput.x == 0))
                    {
                        SourcePlayer.CurrentDirection = Direction.GetFromVector(directionalInput);
                        SourceEntity.CurrentState = Entity.State.Walking;

                        if (Time.time - COOLDOWN_TIME > timeOfLastDirectionChange)
                        {
                            TargetPosition += directionalInput;
                        }
                    }
                    else
                    {
                        SourceEntity.CurrentState = Entity.State.Standing;
                    }
                }
                //If unable to move...
                else
                {
                    //Stop the Player
                    if (SourcePlayer.CurrentState == Entity.State.Get("Walking") ||
                        SourcePlayer.CurrentState == Entity.State.Get("Running"))
                    {
                        SourcePlayer.CurrentState = Entity.State.Get("Standing");
                    }
                }
            }
            else
            {
                //If the player hasn't left their target position yet and they aren't still holding down the keys required to keep moving, stop moving towards their current target
                if (TruePosition == LastReachedTargetPosition)
                {
                    //Calculare the input required for the player to keep walking towards their target
                    Vector3 requiredInput = TargetPosition - LastReachedTargetPosition;
                    //If they aren't still holding that input, give up on the current target
                    if (directionalInput != requiredInput)
                    {
                        TargetPosition = LastReachedTargetPosition;
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //Cache the Collider2D's GameObject
            GameObject otherGameObject = other.gameObject;

            //Look for a TempGate
            TempGate otherTempGate = otherGameObject.GetComponent<TempGate>();

            if (otherTempGate != null)
            {
                MoveTo(otherTempGate.WarpPoint);
            }
        }

        public void CreatePlayerCamera()
        {
            Vector3 cameraPos = transform.position;
            cameraPos.z = -10;
            GameObject playerCamera =
                (GameObject)
                Instantiate(Resources.Load<GameObject>("Prefabs/_Entities/Player/PlayerCamera"), cameraPos,
                    Quaternion.identity);
            playerCamera.transform.SetParent(this.transform);
            playerCameraController = playerCamera.GetComponent<PlayerCameraController>();
        }

        public PlayerCameraController PlayerCameraController
        {
            get { return playerCameraController; }
        }

        public Player SourcePlayer
        {
            get { return (Player) SourceEntity; }
        }
    }
}