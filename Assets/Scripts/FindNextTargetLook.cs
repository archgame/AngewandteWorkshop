using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FindNextTargetLook : FindNextTarget
    {
        public override GameObject FindPath(GameObject agent, Vector3 destinationPosition)
        {
            Mechanical[] mechanicals = GameObject.FindObjectsOfType<Mechanical>();
            if (mechanicals.Length == 0) return null;

            //Debug.Break();

            //find agents walk distance
            Vector3 agentPosition = agent.transform.position;
            NavMeshAgent navAgent = agent.GetComponent<NavMeshAgent>();
            float distance = AgentWalkDistance(navAgent, agentPosition, destinationPosition, Color.yellow);

            //test all mechanicals for shorter distance than non-mechanical walk distance
            GameObject start = null;
            foreach (Mechanical m in mechanicals)
            {
                //can the agent see the mechanical? skip if can't see
                if (!CanAgentSee(m, "Mechanical")) continue;

                //agent to mechanical start distance
                float distToM = AgentWalkDistance(navAgent, agentPosition, m.StartPosition(), Color.green);
                //mechanical weighted distance
                float distM = m.WeightedMechanicalLength();
                //from mechanical end to target distance
                float distFromM = AgentWalkDistance(navAgent, m.EndPosition(), destinationPosition, Color.red);

                float distTotal = distToM + distM + distFromM;

                if (distance > distTotal)
                {
                    //mechanical is shorter distance
                    start = m.GetStart();
                    distance = distTotal;
                }
            }

            return start;
        }

        public bool CanAgentSee(Mechanical mechanical, string tag)
        {
            //get agent position and mechanical start positiont to make line for intersection check
            Vector3 agentPosition = transform.position;
            Vector3 startPosition = mechanical.StartPosition();

            RaycastHit hit; //intersection point
            Debug.DrawLine(agentPosition, startPosition, Color.white);
            //if nothing is hit we return true
            if (!Physics.Linecast(agentPosition, startPosition, out hit)) { return true; }

            Debug.DrawLine(agentPosition, hit.point, Color.black);
            if (hit.transform.gameObject.tag != tag) return false; //if the intersecting object not mechanical, can't see

            //if the intersecting object is mechanical, return true;
            return true;
        }
    }
}