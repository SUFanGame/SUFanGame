using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StevenUniverse.FanGame.Overworld;
using StevenUniverse.FanGame.Overworld.Instances;
using StevenUniverse.FanGame.Overworld.Templates;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Entities.EntityDrivers
{
    public class EntityDriver : MonoBehaviour
    {
        //Speeds
        protected const float WALK_SPEED = 0.1f;
        protected const float JUMP_SPEED = 0.15f;
        protected const float RUN_SPEED = 0.25f;

        //Movement
        private Vector3 truePosition;
        private Vector3 targetPosition;
        private Vector3 lastReachedTargetPosition;

        private Entity sourceEntity;
        private ChunkRenderer chunkRenderer = null;

        //Events
        public delegate void GenericEventHandler();

        public event GenericEventHandler OnStart;
        public event GenericEventHandler OnTargetReached;
        public event GenericEventHandler OnNextTargetReached;

        //Target Position
        public Vector3 TargetPosition
        {
            get { return targetPosition; }
            set { targetPosition = value; }
        }

        public Vector3 LastReachedTargetPosition
        {
            get { return lastReachedTargetPosition; }
            private set { lastReachedTargetPosition = value; }
        }

        //Is At Target
        public bool IsAtTarget
        {
            get
            {
                /*Debug.Log(TruePosition + ", " + TargetPosition);*/
                return TruePosition == TargetPosition;
            }
        }

        protected virtual void Awake()
        {
            TruePosition = transform.position;
        }

        protected virtual void Start()
        {
            //Configure the ChunkRenderer
            UpdateAnimationChunk();

            SourceEntity.EntityDriver = this;
            if (OnStart != null)
            {
                OnStart();
            }

            TargetPosition = TruePosition;
            LastReachedTargetPosition = TargetPosition;

            //Update the Animation every time the one of the outfit's sprites changes
            SourceEntity.Outfit.OnSpriteChange += delegate { UpdateAnimationChunk(); };
            SourceEntity.OnDirectionChange += delegate { UpdateAnimationChunk(); };
            SourceEntity.OnStateChange += delegate { UpdateAnimationChunk(); };
        }

        protected virtual void Update()
        {
        }

        //Return the EntityDriver in front of the Entity, if there is one. Otherwise, return null.
        protected EntityDriver FindNextEntity(Direction direction)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0.5f, 0.5f, 0f),
                direction.Vector, 1f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform != this.transform)
                {
                    EntityDriver hitEntityDriver = hit.transform.GetComponent<EntityDriver>();
                    if (hitEntityDriver != null)
                    {
                        int hitElevation = hitEntityDriver.CurrentElevation;
                        if (hitElevation == this.CurrentElevation || hitElevation == this.CurrentElevation - 1 ||
                            hitElevation == this.CurrentElevation + 1)
                        {
                            return hitEntityDriver;
                        }
                    }
                }
            }

            return null;
        }

        private TileInstance[] FindNextTileInstances(Direction direction)
        {
            return GameController.Instance.GetTileInstancesAtPosition(TruePosition + direction.Vector);
        }

        private TileInstance FindNextCollidableTileInstance(Direction direction)
        {
            List<TileInstance> nextCollidableTileInstances = new List<TileInstance>();

            foreach (TileInstance tileInstance in FindNextTileInstances(direction))
            {
                //Cache the TileInstance's TileTemplate
                TileTemplate tileTemplate = tileInstance.TileTemplate;
                //Start by assuming the TileInstance is not collidable
                bool collidable = false;

                //The TileInstance is the NextCollidableTile if the TileTemplate's Mode is Collidable or Surface and either...
                if (tileTemplate.TileMode == TileTemplate.Mode.Get("Collidable") ||
                    tileTemplate.TileMode == TileTemplate.Mode.Get("Surface"))
                {
                    //...The mode is Collidable and the elevation is the same as the Player
                    if (tileTemplate.TileMode == TileTemplate.Mode.Get("Collidable") &&
                        tileInstance.Elevation == CurrentElevation)
                    {
                        collidable = true;
                    }
                    //...The Tile is grounded and the elevation is 1 above the Player
                    else if (tileTemplate.IsGrounded && tileInstance.Elevation == CurrentElevation + 1)
                    {
                        collidable = true;
                    }
                }

                if (collidable)
                {
                    nextCollidableTileInstances.Add(tileInstance);
                }
            }

            if (nextCollidableTileInstances.Count > 0)
            {
                return nextCollidableTileInstances.Max();
            }
            else
            {
                return null;
            }
        }

        private TileInstance FindNextWalkableTileInstance(Direction direction)
        {
            List<TileInstance> nextWalkableTileInstances = new List<TileInstance>();

            foreach (TileInstance tileInstance in FindNextTileInstances(direction))
            {
                //Cache the TileInstance's TileTemplate
                TileTemplate tileTemplate = tileInstance.TileTemplate;
                //Start by assuming the TileInstance is not collidable
                bool walkable = false;

                //The next TileInstance is the NextWalkableTile if the mode is Surface OR Transitional AND...
                if (tileTemplate.TileMode == TileTemplate.Mode.Get("Surface") ||
                    tileTemplate.TileMode == TileTemplate.Mode.Get("Transitional"))
                {
                    //The Elevation is the same as the player OR...
                    if (tileInstance.Elevation == CurrentElevation)
                    {
                        walkable = true;
                    }
                    //TODO why doesnt this comment match up to the code? It's leftover from the last system
                    //The TileTemplate's mode is Transitional AND the TileInstance's elevation is one higher than the player's OR...
                    else if (tileTemplate.TileMode == TileTemplate.Mode.Get("Transitional") &&
                             tileInstance.Elevation == CurrentElevation + 1)
                    {
                        walkable = true;
                    }
                    //The Mode of the tile the Player is currently standing on is Transitional AND the other tile's elevation is one lower than the player's
                    else
                    {
                        TileInstance currentWalkableTileInstance = FindCurrentWalkableTileInstance();
                        if (currentWalkableTileInstance != null)
                        {
                            TileTemplate currentWalkableTileTemplate = currentWalkableTileInstance.TileTemplate;
                            if (currentWalkableTileTemplate.TileMode == TileTemplate.Mode.Get("Transitional") &&
                                tileInstance.Elevation == CurrentElevation - 1)
                            {
                                walkable = true;
                            }
                        }
                    }
                }

                if (walkable)
                {
                    nextWalkableTileInstances.Add(tileInstance);
                }
            }

            if (nextWalkableTileInstances.Count > 0)
            {
                return nextWalkableTileInstances.Max();
            }
            else
            {
                return null;
            }
        }

        private TileInstance FindCurrentWalkableTileInstance()
        {
            TileInstance[] currentTileInstances = GameController.Instance.GetTileInstancesAtPosition(TruePosition);
            List<TileInstance> currentWalkableTileInstances = new List<TileInstance>();

            //Search for a viable tile for the player
            foreach (TileInstance ti in currentTileInstances)
            {
                TileTemplate tt = ti.TileTemplate;
                //The tile is the Player's current tile if the tile's mode is Surface OR Transitional AND the tile's elevation is the same as the player's elevation
                if ((tt.TileMode == TileTemplate.Mode.Get("Surface") ||
                     tt.TileMode == TileTemplate.Mode.Get("Transitional")) && ti.Elevation == CurrentElevation)
                {
                    currentWalkableTileInstances.Add(ti);
                }
            }

            if (currentWalkableTileInstances.Count > 0)
            {
                return currentWalkableTileInstances.Max();
            }
            else
            {
                return null;
            }
        }

        protected virtual void FixedUpdate()
        {
            //Cache whether or not the Entity was at the target before moving
            bool wasAtTarget = IsAtTarget;

            //Move the Entity if they aren't at their target
            if (!IsAtTarget)
            {
                //If the Entity has already started moving from their last reached target position, we know their movement has already been verified
                bool movementAllowed = TruePosition != LastReachedTargetPosition;

                //If the Entity hasn't already been verified to be allowed, try and verify it
                if (!movementAllowed)
                {
                    //Cache the next walkable TileInstance for reference
                    TileInstance nextWalkableTileInstance = FindNextWalkableTileInstance(SourceEntity.CurrentDirection);

                    //If there is a tile to move to, verify there are no obstructions
                    if (nextWalkableTileInstance != null)
                    {
                        //Cache the next collidable TileInstance for reference
                        TileInstance nextCollidableTileInstance =
                            FindNextCollidableTileInstance(SourceEntity.CurrentDirection);
                        //Cache the next EntityDriver for reference
                        EntityDriver nextEntity = FindNextEntity(SourceEntity.CurrentDirection);

                        //If there is no collidable tile OR the walkable tile is higher than it, AND there is no Entity, the movement is allowed
                        if ((nextCollidableTileInstance == null ||
                             nextWalkableTileInstance.IsAbove(nextCollidableTileInstance)) && nextEntity == null)
                        {
                            //Allow the movement
                            movementAllowed = true;

                            //TODO move this to the player class
                            if (this is PlayerDriver)
                            {
                                GameController.Instance.StartLoadAreaAroundPlayer(false, false);
                            }

                            int nextElevation = nextWalkableTileInstance.Elevation;

                            //Change the elevation immediately if moving up
                            if (nextElevation > CurrentElevation)
                            {
                                CurrentElevation = nextElevation;
                            }
                            //Change the elevation upon reaching the next tile if moving down
                            else if (nextElevation < CurrentElevation)
                            {
                                OnNextTargetReached += delegate { CurrentElevation = nextElevation; };
                            }
                            //TODO re-implement tile events
                        }
                    }
                }

                //If the Entity has started 
                if (movementAllowed)
                {
                    TruePosition = Vector3.MoveTowards(TruePosition, TargetPosition, WALK_SPEED);
                }
            }

            //If the Entity was at the target and now they aren't, they have started moving towards a new target
            if (wasAtTarget && !IsAtTarget)
            {
            }
            //If the Entity wasn't at the target and now they are, the target has been reached
            else if (!wasAtTarget && IsAtTarget)
            {
                DoOnTargetReached();
            }
        }

        private void DoOnTargetReached()
        {
            //Record the fact that the Entity has reached the target position
            LastReachedTargetPosition = TargetPosition;

            //Run events that occur on each target reach
            if (OnTargetReached != null)
            {
                OnTargetReached();
            }
            //Run events that have been set to occur on the next target reach
            if (OnNextTargetReached != null)
            {
                OnNextTargetReached();
                OnNextTargetReached = null;
            }
        }

        protected virtual void LateUpdate()
        {
            float roundedX = Mathf.Round(TruePosition.x*Constants.PIXELS_TO_UNITS)/Constants.PIXELS_TO_UNITS;
            float roundedY = Mathf.Round(TruePosition.y*Constants.PIXELS_TO_UNITS)/Constants.PIXELS_TO_UNITS;
            float roundedZ = Mathf.Round(TruePosition.z*Constants.PIXELS_TO_UNITS)/Constants.PIXELS_TO_UNITS;
            transform.position = new Vector3(roundedX, roundedY, roundedZ);
        }

        public void UpdateAnimationChunk()
        {
            Chunk currentAnimationChunk = SourceEntity.Outfit.GetChunk(SourceEntity.CurrentState,
                SourceEntity.CurrentDirection);
            if (ChunkRenderer == null)
            {
                ChunkRenderer = ChunkRenderer.AddChunkRenderer(gameObject, false, currentAnimationChunk);
            }

            if (ChunkRenderer == null)
            {
                Debug.Log("Yoooo");
            }
            ChunkRenderer.DisplayChunk = currentAnimationChunk;
        }

        public void MoveTo(Vector3 destination)
        {
            MoveTo(new WarpPoint("", GameController.Instance.CurrentScene, destination, CurrentElevation));
        }

        public void MoveTo(WarpPoint destination)
        {
            TruePosition = destination.Position;
            TargetPosition = TruePosition;
            CurrentElevation = destination.Elevation;
            GameController.Instance.CurrentScene = destination.Scene;
        }

        public Vector3 TruePosition
        {
            get { return truePosition; }
            set { truePosition = value; }
        }

        //Shortcut for accessing the elevation of the chunkwrapper
        public int CurrentElevation
        {
            get { return ChunkRenderer.SourceChunk.CurrentElevation; }
            set { ChunkRenderer.SourceChunk.CurrentElevation = value; }
        }

        public Entity SourceEntity
        {
            get { return sourceEntity; }
            set { sourceEntity = value; }
        }

        public ChunkRenderer ChunkRenderer
        {
            get { return chunkRenderer; }
            protected set { chunkRenderer = value; }
        }
    }
}