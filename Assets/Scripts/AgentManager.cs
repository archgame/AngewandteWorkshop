using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Linq;

public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance { get; private set; } //for singleton

    public string TargetTag = "Target";
    public string EntranceTag = "Entrance";
    public GameObject AgentPrefab;

    private GameObject[] _entrances;
    private Dictionary<GameObject, GameObject> _targets = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, GameObject> _agents = new Dictionary<GameObject, GameObject>();

    private void Awake()
    {
        //Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _entrances = GameObject.FindGameObjectsWithTag(EntranceTag);
        GameObject[] targets = GameObject.FindGameObjectsWithTag(TargetTag); //grabbing all targets in scene
        foreach (GameObject target in targets) //loop through each target
        {
            if (_targets.ContainsKey(target)) continue; //if target already in dictionary skip
            _targets.Add(target, null);
        }
        InstantiateAgent(AgentPrefab);
    }

    private void InstantiateAgent(GameObject prefab)
    {
        GameObject entrance = _entrances[Random.Range(0, _entrances.Length)];

        Vector3 position = entrance.transform.position;
        GameObject agent = Instantiate(prefab, position, Quaternion.identity);
        BehaviorTree behaviorTree = agent.GetComponent<BehaviorTree>();
        behaviorTree.SetVariableValue("Entrance", entrance); //assumes public tree variable Entrance
        behaviorTree.EnableBehavior();
        _agents.Add(agent, null);
    }

    public void RemoveAgent(GameObject agent)
    {
        GameObject lastTarget = _agents[agent];
        _targets[lastTarget] = null; //remove agent from target
        _agents.Remove(agent); //remove agent from agents dictionary
        Destroy(agent);
    }

    public GameObject GetTarget(GameObject agent)
    {
        //make sure the agent doesn't go to previous target
        GameObject lastTarget = _agents[agent];
        if (lastTarget != null)
        {
            _targets[lastTarget] = null; //this target is open for a new agent
        }

        GameObject[] keys = _targets.Keys.ToArray();
        keys = Shuffle(keys);
        for (int i = 0; i < keys.Length; i++)
        {
            GameObject key = keys[i];

            if (lastTarget == key) continue; //if target was previous target, skip
            if (_targets[key] != null) continue; //if target has agent assigned, skip

            _targets[key] = agent;
            _agents[agent] = key;
            return key; //key is a target
        }

        return null;
    }

    private GameObject[] Shuffle(GameObject[] objects)
    {
        GameObject tempGO;
        for (int i = 0; i < objects.Length; i++)
        {
            //Debug.Log("i: " + i);
            int rnd = Random.Range(0, objects.Length);
            tempGO = objects[rnd];
            objects[rnd] = objects[i];
            objects[i] = tempGO;
        }
        return objects;
    }
}