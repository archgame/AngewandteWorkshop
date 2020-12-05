using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    [TaskDescription("Find Next Target From Agent Manager.")]
    [TaskCategory("NavMeshAgent")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}MoveTowardsIcon.png")]
    public class FindNextTarget : Conditional
    {
        public SharedInt Visits;
        public SharedGameObject Entrance;
        public SharedGameObject Target;

        public override TaskStatus OnUpdate()
        {
            if (Visits.Value <= 0)
            {
                Target.Value = Entrance.Value;
                return TaskStatus.Success;
            }

            GameObject go = AgentManager.Instance.GetTarget(this.gameObject);
            if (go == null) { return TaskStatus.Running; }
            Target.Value = go;
            return TaskStatus.Success;
        }
    }
}