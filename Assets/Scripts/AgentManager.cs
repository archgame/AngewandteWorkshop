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
    private int counter = -1;
    private GameObject[] _targets;

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
        _targets = GameObject.FindGameObjectsWithTag(TargetTag);
        foreach (GameObject go in _entrances)
        {
            Debug.Log(go.name);
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
    }

    public void RemoveAgent(GameObject agent)
    {
        Destroy(agent);
    }

    public GameObject GetTarget(GameObject agent)
    {
        counter++;
        if (counter >= _targets.Length) { counter = 0; }
        return _targets[counter];
    }
}