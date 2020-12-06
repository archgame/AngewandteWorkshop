using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanical : MonoBehaviour
{
    public Transform[] Path;
    public int Weight = 3; //subjective weight to use mechanical or not
    public int Speed = 4; //animation speed

    public virtual bool MechanicalUpdate(GameObject agent)
    {
        //animate/update agent position
        agent.transform.position = Vector3.MoveTowards(
            agent.transform.position,
            EndPosition(),
            Time.deltaTime * Speed
            );

        //check if the agent has reached the end
        if (Vector3.Distance(agent.transform.position, EndPosition()) < 0.01f)
        {
            return true;
        }

        return false;
    }

    public virtual float WeightedMechanicalLength()
    {
        Debug.DrawLine(StartPosition(), EndPosition(), Color.cyan);
        return Vector3.Distance(StartPosition(), EndPosition()) / Weight;
    }

    public virtual GameObject GetStart()
    {
        return Path[0].gameObject; //assumes the start object is the first object in the path array
    }

    public virtual Vector3 StartPosition()
    {
        return Path[0].position;
    }

    public virtual Vector3 EndPosition()
    {
        return Path[1].position;
    }
}