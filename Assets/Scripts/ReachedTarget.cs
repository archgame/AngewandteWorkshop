using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public SharedFloat arriveDistance = 0.5f;
        public SharedInt ReachedCount = 0;

        public override TaskStatus OnUpdate()
        {
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