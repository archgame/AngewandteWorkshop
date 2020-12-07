using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.Movement
{
    public class FindNextTargetMechOnly : FindNextTarget
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
                //check if mechanical is multi-path, if true, skip mechanical
                if (m.GetType() == typeof(MultiPath)) continue;

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
    }
}