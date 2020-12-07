using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPath : Mechanical
{
    private Dictionary<GameObject, int> _agents = new Dictionary<GameObject, int>();

    public override Vector3 EndPosition()
    {
        return Path[Path.Length - 1].position;
    }

    public override float WeightedMechanicalLength()
    {
        float distance = 0;

        //guard statement
        if (Path.Length < 2) return distance;

        //getting the total path distance
        for (int i = 1; i < Path.Length; i++)
        {
            Vector3 vec1 = Path[i - 1].position;
            Vector3 vec2 = Path[i].position;

            Debug.DrawLine(vec1, vec2, Color.cyan);
            float d = Vector3.Distance(vec1, vec2);
            distance += d;
        }

        //we scale the return distance
        distance /= Weight;
        return distance;
    }

    public override bool MechanicalUpdate(GameObject agent)
    {
        //add the agent to the dictionary
        if (!_agents.ContainsKey(agent))
        {
            _agents.Add(agent, 1);
        }

        //get current Path index
        int pathIndex = _agents[agent];
        Vector3 pathPosition = Path[pathIndex].position;

        //animate/update agent position
        agent.transform.position = Vector3.MoveTowards(
            agent.transform.position,
            pathPosition,
            Time.deltaTime * Speed
            );

        //check if the agent has reached the end
        if (Vector3.Distance(agent.transform.position, pathPosition) < 0.01f)
        {
            pathIndex++;
            if (pathIndex == Path.Length) { _agents.Remove(agent); return true; } //end of the path
            _agents[agent] = pathIndex;
        }

        return false;
    }
}