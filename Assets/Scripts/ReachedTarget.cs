using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Find next target from Agent Manager.")]
    [TaskCategory("NavMeshAgent")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}LightSearchIcon.png")]
    public class ReachedTarget : Action
    {
        public SharedGameObject Agent;
        public SharedGameObject Target;
        public SharedGameObject Entrance;
        public SharedGameObject Mechanical;
        public SharedFloat arriveDistance = 0.75f;
        public SharedInt ReachedCount = 0;

        private bool startMechanical = false; //true if agent has reached mechanical Start

        public override TaskStatus OnUpdate()
        {
            //check if mechanical is existing and use
            if (Mechanical.Value != null)
            {
                if (!startMechanical)
                {
                    //if agent is still walking towards mechanical
                    Vector3 mechPos = Mechanical.Value.transform.position;

                    if (Vector3.Magnitude(transform.position - mechPos) < arriveDistance.Value)
                    {
                        startMechanical = true; //agent has reached the start of the mechanical
                    }
                }
                else
                {
                    //animating along mechanical
                    this.GetComponent<NavMeshAgent>().enabled = false;
                    if (Mechanical.Value.GetComponentInParent<Mechanical>().MechanicalUpdate(this.gameObject))
                    {
                        this.GetComponent<NavMeshAgent>().enabled = true;
                        startMechanical = false;
                        return TaskStatus.Success; //once agent reaches the end of the mechanical, we return success
                    }
                }
                return TaskStatus.Running;
            }

            Vector3 position = Target.Value.transform.position;

            if (Vector3.Magnitude(transform.position - position) < arriveDistance.Value)
            {
                if (Target.Value == Entrance.Value)
                {
                    AgentManager.Instance.RemoveAgent(this.gameObject);
                    return TaskStatus.Success;
                }
                ReachedCount.Value--;
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }
    }
}