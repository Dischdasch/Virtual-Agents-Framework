using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Third person animation
using UnityStandardAssets.Characters.ThirdPerson;
// IEnumerator
using System.Collections;
// Tasks
using System.Collections.Generic;
using VirtualAgentsFramework.AgentTasks;
// Action
using System;
// Rigs
using UnityEngine.Animations.Rigging;

namespace VirtualAgentsFramework
{
    namespace AgentTasks
    {
        public class AgentPressingTask : IAgentTask
        {
            Agent agent;
            // Rig pointingRig;

            private GameObject destinationObject = null;

            public event Action OnTaskFinished;

            public AgentPressingTask(GameObject destinationObject) //, Rig pointingRig, ascendingWeigh = true
            {
                // this.pointingRig = pointingRig;
                this.destinationObject = destinationObject;
            }

            public AgentPressingTask(Vector3 destinationCoordinates) //, Rig pointingRig, ascendingWeigh = true
            {
                // this.pointingRig = pointingRig;
                CreateDestinationObject(destinationCoordinates);
            }

            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;

                // Change agent's recursion state to active (busy)
                // weightIsChanging = true;

                // If ascending, ...
                  // (Slowly) change the pointingRig's weight to 1
                  // Wait for a fraction of a second using a WaitingTask(asap)
                  // (Recursion:) Schedule a subtask for ascending pointing Rig weight

                // If descending, ...
                  // (Slowly) change the pointingRig's weight to 0
                  // Wait for a fraction of a second using a WaitingTask(asap)
                  // (Recursion:) Schedule a subtask for descending pointing Rig weight

                // If the recursive condition is met (no more ascending or descending), break

                //TODO destroy destination object upon execution (if one was created)
            }

            public void Update()
            {
                // Is there maybe a simpler solution using a lerp? Or maybe a real-time breaking condition from the animator? I mean, the recursive idea should work but maybe there is a more elegant solution akin the animation task
            }
        }

        public class AgentPointingTask : IAgentTask
        {
            private Agent agent;
            private GameObject destinationObject = null;
            private GameObject target = null;
            private Rig twistChain;
            private Rig leftArmStretch;
            private enum Program
            {
                ascending,
                waiting,
                descending
            }
            private Program program;

            public event Action OnTaskFinished;

            public AgentPointingTask(GameObject destinationObject, Rig twistChain, Rig leftArmStretch, GameObject target)
            {
                this.destinationObject = destinationObject;
                this.twistChain = twistChain;
                this.leftArmStretch = leftArmStretch;
                this.target = target;
            }

            public AgentPointingTask(Vector3 destinationCoordinates, Rig twistChain, Rig leftArmStretch, GameObject target)
            {
                CreateDestinationObject(destinationCoordinates);
                this.twistChain = twistChain;
                this.leftArmStretch = leftArmStretch;
                this.target = target;
            }

            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                target.transform.position = destinationObject.transform.position;
                //TODO destroy destination object upon execution (if one was created)
            }

            public void Update()
            {
                switch(program)
                {
                    case Program.ascending:
                        agent.StartCoroutine(IncreaseRigWeightCoroutine(twistChain, 0.5f, program)); // Coroutine parallel
                        agent.StartCoroutine(IncreaseRigWeightCoroutine(leftArmStretch, 1f, Program.waiting));
                        Debug.Log("Ascending");
                        break;
                    case Program.waiting:
                        agent.StartCoroutine(WaitingCoroutine(1f, Program.descending));
                        Debug.Log("Waiting");
                        break;
                    case Program.descending:
                        DecreaseRigWeight(twistChain, 0f); // Procedural parallel
                        DecreaseRigWeight(leftArmStretch, 0f, true);
                        Debug.Log("Descending");
                        break;
                }
            }

            void IncreaseRigWeight(Rig rig, float targetWeight, Program nextProgram, float speed = 100f)
            {
                if(rig.weight < targetWeight)
                {
                    rig.weight += 1f / speed;
                }
                if(rig.weight == targetWeight)
                {
                    program = nextProgram;
                }
            }

            void DecreaseRigWeight(Rig rig, float targetWeight, bool last = false, float speed = 100f)
            {
                if(rig.weight > targetWeight)
                {
                    rig.weight -= 1f / speed;
                }
                if(rig.weight == targetWeight && last == true)
                {
                    // Trigger the TaskFinished event
                    OnTaskFinished();
                }
            }

            private IEnumerator IncreaseRigWeightCoroutine(Rig rig, float targetWeight, Program nextProgram, float speed = 100f)
            {
                if(rig.weight < targetWeight)
                {
                    rig.weight += 1f / speed;
                }
                yield return new WaitUntil(() => rig.weight >= targetWeight - 1f / speed); // rig.weight >= targetWeight - 1f / speed
                if(rig.weight >= targetWeight - 1f / speed)
                {
                    program = nextProgram;
                }
            }

            private IEnumerator DecreaseRigWeightCoroutine(Rig rig, float targetWeight, bool last = false, float speed = 100f)
            {
                if(rig.weight > targetWeight)
                {
                    rig.weight -= 1f / speed;
                }
                yield return new WaitUntil(() => rig.weight == targetWeight); //(targetWeight - rig.weight >= 1f / speed) || (targetWeight - rig.weight <= 1f / speed)
                if(rig.weight == targetWeight && last == true)
                {
                    // Trigger the TaskFinished event
                    OnTaskFinished();
                }
            }

            private IEnumerator WaitingCoroutine(float waitingTime, Program nextProgram, bool last = false)
            {
                yield return new WaitForSeconds(waitingTime);
                program = nextProgram; // "this" löschen
                if(last == true) // "== true" löschen
                {
                    // Trigger the TaskFinished event
                    OnTaskFinished();
                }
            }
        }
    }
}
