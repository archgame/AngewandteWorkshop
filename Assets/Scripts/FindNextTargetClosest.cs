using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FindNextTargetClosest : FindNextTarget
    {
        public override GameObject FindPath(GameObject agent, Vector3 destinationPosition)
        {
            Mechanical[] mechanicals = GameObject.FindObjectsOfType<Mechanical>();
            if (mechanicals.Length == 0) return null;

            //Debug.Break();

            //find agents walk distance
            Vector3 agentPosition = agent.transform.position;
            NavMeshAgent navAgent = agent.GetComponent<NavMeshAgent>();
            float walkDistance = AgentWalkDistance(navAgent, agentPosition, destinationPosition, Color.yellow);
            float distTo = Mathf.Infinity; //distance to closest mechanical

            //test all mechanicals for shorter distance than non-mechanical walk distance
            GameObject start = null;
            foreach (Mechanical m in mechanicals)
            {
                //agent to mechanical start distance
                float distToM = AgentWalkDistance(navAgent, agentPosition, m.StartPosition(), Color.green);
                if (distToM > distTo) continue; //if the distance to is greater, don't calculate full mechanical
                //mechanical weighted distance
                float distM = m.WeightedMechanicalLength();
                //from mechanical end to target distance
                float distFromM = AgentWalkDistance(navAgent, m.EndPosition(), destinationPosition, Color.red);

                float distTotal = distToM + distM + distFromM;

                if (walkDistance > distTotal)
                {
                    if (distTo > distToM)
                    {
                        //mechanical is shorter distance
                        start = m.GetStart();
                        distTo = distToM;
                    }
                }
            }

            return start;
        }
    }
}