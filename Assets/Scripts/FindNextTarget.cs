using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        public SharedGameObject Mechanical;

        public override TaskStatus OnUpdate()
        {
            //reset mechanical
            if (Mechanical.Value != null)
            {
                Mechanical.Value = null;
            }
            else
            {
                if (Visits.Value <= 0)
                {
                    Target.Value = Entrance.Value;
                }
                else
                {
                    GameObject go = AgentManager.Instance.GetTarget(this.gameObject);
                    if (go == null) { return TaskStatus.Running; }
                    Target.Value = go;
                }
            }

            GameObject mechanical = FindPath(this.gameObject, Target.Value.transform.position);

            if (mechanical != null) { Mechanical.Value = mechanical; }

            return TaskStatus.Success;
        }

        public GameObject FindPath(GameObject agent, Vector3 destinationPosition)
        {
            Mechanical[] mechanicals = GameObject.FindObjectsOfType<Mechanical>();
            if (mechanicals.Length == 0) return null;

            Debug.Break();

            //get walking path distance
            Vector3 agentPosition = agent.transform.position;
            NavMeshAgent navAgent = agent.GetComponent<NavMeshAgent>();
            float distance = AgentWalkDistance(navAgent, agent.transform, agentPosition, destinationPosition, Color.yellow);

            //test all conveyances
            GameObject start = null;

            foreach (Mechanical m in mechanicals)
            {
                float distToC = AgentWalkDistance(navAgent, agent.transform, agentPosition, m.StartPosition(), Color.green);
                float distC = m.WeightedMechanicalLength();
                float distFromC = AgentWalkDistance(navAgent, agent.transform, m.EndPosition(), destinationPosition, Color.red);

                if (distance > distToC + distC + distFromC)
                {
                    start = m.GetStart();
                    distance = distToC + distC + distFromC;
                }
            }

            return start;
        }

        public static float AgentWalkDistance(NavMeshAgent agent, Transform trans,
        Vector3 start, Vector3 end, Color color)
        {
            //in case they are the same position
            if (Vector3.Distance(start, end) < 0.01f) return 0;

            //move agent to the start position
            Vector3 initialPosition = trans.position;
            agent.enabled = false;
            trans.position = start;//_agent.Move(start - initialPosition);
            agent.enabled = true;

            //test to see if agent has path or not
            float distance = Mathf.Infinity;
            NavMeshPath navMeshPath = agent.path;
            if (!agent.CalculatePath(end, navMeshPath))
            {
                //reset agent to original position
                agent.enabled = false;
                trans.position = initialPosition;//_agent.Move(initialPosition - start);
                agent.enabled = true;
                return distance;
            }

            //check to see if there is a path
            Vector3[] path = navMeshPath.corners;
            if (path.Length < 2 || Vector3.Distance(path[path.Length - 1], end) > 2)
            {
                //reset agent to original position
                agent.enabled = false;
                trans.position = initialPosition;//_agent.Move(initialPosition - start);
                agent.enabled = true;
                return distance;
            }

            //get walking path distance
            distance = 0;
            for (int i = 1; i < path.Length; i++)
            {
                distance += Vector3.Distance(path[i - 1], path[i]);
                Debug.DrawLine(path[i - 1], path[i], color); //visualizing the path, not necessary to return
            }

            //reset agent to original position
            agent.enabled = false;
            trans.position = initialPosition;//_agent.Move(initialPosition - start);
            agent.enabled = true;

            return distance;
        }
    }
}