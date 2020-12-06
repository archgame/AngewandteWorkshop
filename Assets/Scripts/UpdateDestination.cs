using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent
{
    [TaskCategory("NavMeshAgent")]
    [TaskDescription("Sets the destination of the agent")]
    public class UpdateDestination : Action
    {
        public SharedGameObject Agent;
        public SharedGameObject Target;
        public SharedGameObject Mechanical;
        private NavMeshAgent _navMeshAgent;
        private GameObject _prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(Agent.Value);
            if (currentGameObject != _prevGameObject)
            {
                _navMeshAgent = currentGameObject.GetComponent<NavMeshAgent>();
                _prevGameObject = currentGameObject;
            }
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (_navMeshAgent == null) { return TaskStatus.Failure; }

            if (Mechanical.Value != null)
            {
                //if mechanical exists, head towards mechanical
                return _navMeshAgent.SetDestination(Mechanical.Value.transform.position) ? TaskStatus.Success : TaskStatus.Failure;
            }

            return _navMeshAgent.SetDestination(Target.Value.transform.position) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}