using System.Collections.Generic;
using UnityEngine;
using StevenUniverse.FanGame.Entities;
using StevenUniverse.FanGame.Entities.EntityDrivers;
using StevenUniverse.FanGame.Util;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class Movement : Activity
    {
        [SerializeField] private List<Step> steps = new List<Step>();
        [SerializeField] private string targetName;

        private int currentStepIndex = 0;
        private EntityDriver target;

        public Movement(int id, bool allowsControl, Step[] steps, string targetName) : base(id, allowsControl)
        {
            this.steps = new List<Step>(steps);
            this.targetName = targetName;
        }

        public override void StartActivity()
        {
            base.StartActivity();

            currentStepIndex = 0;

            //Set the target's state to walking
            Target.SourceEntity.CurrentState = Entity.State.Walking;
        }

        public override void UpdateActivity()
        {
            base.UpdateActivity();

            if (!IsComplete && Target.IsAtTarget)
            {
                if (currentStepIndex < steps.Count)
                {
                    //Cache the current step
                    Step currentStep = steps[currentStepIndex];

                    //Set the target's direction to the direction of the current step
                    target.SourceEntity.CurrentDirection = currentStep.Direction;
                    //Set the target's target position to the position corresponding to the current step
                    Vector3 movement = currentStep.Direction.Vector*currentStep.StepCount;
                    Target.TargetPosition += movement;

                    //Increment the current step index
                    currentStepIndex++;
                }
                else
                {
                    //Determine if the next Activity is a Movement Activity
                    //Start by assuming it isn't
                    bool nextActivityIsMovement = false;

                    //Cach the next Activity and determine if it exists
                    Activity nextActivity = GameController.Instance.GetNextActivity();
                    if (nextActivity != null)
                    {
                        //Cast the next Activity as a Movement to see if it is one
                        Movement nextMovement = nextActivity as Movement;
                        if (nextMovement != null)
                        {
                            nextActivityIsMovement = true;
                        }
                    }

                    //If the next activity isn't a movement, return the target character to the Standing State
                    if (!nextActivityIsMovement)
                    {
                        target.SourceEntity.CurrentState = Entity.State.Standing;
                    }

                    //End the activity
                    IsComplete = true;
                }
            }
        }

        public EntityDriver Target
        {
            get
            {
                //Find the target if it hasn't been found yet
                target = GameController.Instance.FindEntity(targetName);
                return target;
            }
            private set { target = value; }
        }

        [System.Serializable]
        public class Step
        {
            [SerializeField] private string directionName;
            [SerializeField] private int stepCount;

            public Step(string directionName, int stepCount)
            {
                DirectionName = directionName;
                StepCount = stepCount;
            }

            public string DirectionName
            {
                get { return directionName; }
                private set { directionName = value; }
            }

            public int StepCount
            {
                get { return stepCount; }
                private set { stepCount = value; }
            }

            public Direction Direction
            {
                get { return Direction.Get(DirectionName); }
            }
        }
    }
}