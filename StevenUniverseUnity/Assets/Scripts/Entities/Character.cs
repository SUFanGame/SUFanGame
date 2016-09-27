using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Entities.Customization;
using StevenUniverse.FanGame.Entities.EntityDrivers;
using StevenUniverse.FanGame.Interactions;
using StevenUniverse.FanGame.Interactions.Activities;
using StevenUniverse.FanGame.Util;
using StevenUniverse.FanGame.Util.Logic;

namespace StevenUniverse.FanGame.Entities
{
    [System.Serializable]
    public class Character : Entity
    {
        [SerializeField]
        private CharacterInstance[] characterInstances;

        [SerializeField]
        private ActivityBlock[] activityBlocks;
        [SerializeField]
        private Branch[] branches;

        [SerializeField]
        private DestroyEntity[] destroyEntityList;
        [SerializeField]
        private Dialog[] dialogList;
        [SerializeField]
        private InstantiateGameObject[] instantiateGameObjectList;
        [SerializeField]
        private Movement[] movementList;
        [SerializeField]
        private SetData[] setDataList;
        [SerializeField]
        private Wait[] waitList;
        [SerializeField]
        private WaitForBool[] waitForBoolList;

        public static Character GetCharacter(string characterAppDataPath)
        {
            return Get<Character>(characterAppDataPath);
        }

        public Character
            (
            string characterName,
            string directionName,
            string stateName,
            Outfit startingOutfit,

            CharacterInstance[] characterInstances,

            ActivityBlock[] activityBlocks,
            Branch[] branches,

            DestroyEntity[] destroyEntityList,
            Dialog[] dialogList,
            InstantiateGameObject[] instantiateGameObjectList,
            Movement[] movementList,
            SetData[] setDataList,
            Wait[] waitList,
            WaitForBool[] waitForBoolList
            ) : base(characterName, directionName, stateName, startingOutfit)
        {
            this.characterInstances = characterInstances;

            this.activityBlocks = activityBlocks;
            this.branches = branches;

            this.destroyEntityList = destroyEntityList;
            this.dialogList = dialogList;
            this.instantiateGameObjectList = instantiateGameObjectList;
            this.movementList = movementList;
            this.setDataList = setDataList;
            this.waitList = waitList;
            this.waitForBoolList = waitForBoolList;
        }

        private Interaction[] GetAllInteractions()
        {
            List<Interaction> allInteractions = new List<Interaction>();

            allInteractions.AddRange(activityBlocks);
            allInteractions.AddRange(branches);

            return allInteractions.ToArray();
        }

        public Interaction GetInteraction(int interactionID)
        {
            if (interactionID == -1)
            {
                return null;
            }

            foreach (Interaction interaction in GetAllInteractions())
            {
                if (interaction.InteractionID == interactionID)
                {
                    return interaction;
                }
            }

            throw new UnityException(string.Format("Could not find an Interaction with interactionID '{0};", interactionID));
        }

        private Activity[] GetAllActivities()
        {
            List<Activity> allActivities = new List<Activity>();

            allActivities.AddRange(destroyEntityList);
            allActivities.AddRange(dialogList);
            allActivities.AddRange(instantiateGameObjectList);
            allActivities.AddRange(movementList);
            allActivities.AddRange(setDataList);
            allActivities.AddRange(waitList);
            allActivities.AddRange(waitForBoolList);

            return allActivities.ToArray();
        }

        public Activity GetActivity(int activityID)
        {
            foreach (Activity activity in GetAllActivities())
            {
                if (activity.ActivityID == activityID)
                {
                    return activity;
                }
            }

            throw new UnityException(string.Format("Could not find an activity with activityID '{0};", activityID));
        }

        public void AttemptLoad()
        {
            CharacterInstance firstValidCharacterInstance = GetFirstValidCharacterInstance();

            if (firstValidCharacterInstance != null)
            {
                //Create the GameObject
                GameObject characterGameObject = new GameObject(Name);
                characterGameObject.transform.position = firstValidCharacterInstance.Position;
                characterGameObject.transform.SetParent(GameController.Instance.CharacterParent.transform);
                //Add a BoxCollider2D
                BoxCollider2D characterCollider = characterGameObject.AddComponent<BoxCollider2D>();
                characterCollider.offset = new Vector2(0.5f, 0.5f);
                characterCollider.size = new Vector2(1f, 1f);
                //Add a CharacterDriver
                CharacterDriver characterDriver = characterGameObject.AddComponent<CharacterDriver>();
                characterDriver.SourceEntity = this;
                characterDriver.SourceEntity.OnInteract +=
                    delegate { Enqueue(firstValidCharacterInstance); };
                characterDriver.OnStart +=
                    delegate { characterDriver.CurrentElevation = firstValidCharacterInstance.Elevation; };
            }
        }

        private void Enqueue(CharacterInstance characterInstance)
        {
            Interaction interactionToProcess = GetFirstValidInteractionInCharacterInstance(characterInstance);

            Enqueue(characterInstance, interactionToProcess);
        }

        public void Enqueue(CharacterInstance characterInstance, Interaction interactionToProcess)
        {
            if (interactionToProcess != null)
            {
                ActivityBlock activityBlockToProcess = interactionToProcess as ActivityBlock;
                if (activityBlockToProcess != null)
                {
                    List<Activity> activitiesToEnqueue = new List<Activity>();

                    foreach (int activityID in activityBlockToProcess.ActivityIDs)
                    {
                        activitiesToEnqueue.Add(GetActivity(activityID));
                    }

                    foreach (Activity activityToEnqueue in activitiesToEnqueue)
                    {
                        GameController.Instance.EnqueueActivity(activityToEnqueue);
                    }
                }
                /*
                else
                {
                    throw new UnityException("Non-ActivityBlock Interactions are not yet supported!");
                }*/
            }

            GameController.Instance.ProcessInteraction(this, characterInstance, interactionToProcess);
        }

        public Interaction GetFirstValidInteractionInCharacterInstance(CharacterInstance characterInstance)
        {
            foreach (CharacterInstance.InteractionStarter interactionStarter in characterInstance.InteractionStarters)
            {
                if (interactionStarter.CheckStatus())
                {
                    return GetInteraction(interactionStarter.InteractionID);
                }
            }

            return null;
        }

        public CharacterInstance GetFirstValidCharacterInstance()
        {
            foreach (CharacterInstance characterInstance in characterInstances)
            {
                if (characterInstance.CheckStatus())
                {
                    return characterInstance;
                }
            }

            return null;
        }

        //public CharacterInstance[] CharacterInstances { get { return characterInstances; } }

        [System.Serializable]
        public class CharacterInstance
        {
            [SerializeField] private Conditional spawnCondition;
            [SerializeField] private Vector3 position;
            [SerializeField] private int elevation;
            [SerializeField] private InteractionStarter[] interactionStarters;

            public CharacterInstance
            (
                Conditional spawnCondition,
                Vector3 position,
                int elevation,
                InteractionStarter[] interactionStarters
            )
            {
                SpawnCondition = spawnCondition;
                Position = position;
                Elevation = elevation;
                this.interactionStarters = interactionStarters;
            }

            public bool CheckStatus()
            {
                return spawnCondition.CheckStatus();
            }

            public Conditional SpawnCondition
            {
                get { return spawnCondition; }
                set { spawnCondition = value; }
            }

            public Vector3 Position
            {
                get { return position; }
                set { position = value; }
            }

            public int Elevation
            {
                get { return elevation; }
                set { elevation = value; }
            }

            public InteractionStarter[] InteractionStarters
            {
                get { return interactionStarters; }
                set { interactionStarters = value; }
            }

            [System.Serializable]
            public class InteractionStarter
            {
                [SerializeField]
                private Conditional conditional;
                [SerializeField]
                private int interactionID;

                public InteractionStarter(Conditional conditional, int interactionID)
                {
                    this.conditional = conditional;
                    this.interactionID = interactionID;
                }

                public bool CheckStatus()
                {
                    return conditional.CheckStatus();
                }

                public int InteractionID { get { return interactionID;} }
            }
        }
    }
}