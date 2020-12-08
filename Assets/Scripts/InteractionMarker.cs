using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractionMarker : MonoBehaviour
{
    public static InteractionMarker Instance { get; private set; } //for singleton

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

    public GameObject MarkerPrefab;
    public float Increase = 0.5f;

    private List<GameObject> _markers = new List<GameObject>();

    public void Interact(Vector3 position)
    {
        //spcial condition for first marker, for first marker, just add a marker
        if (_markers.Count == 0) { AddMark(position); return; }

        //list of existing markers ordered by closest to input position
        //List<GameObject> example = _markers.OrderBy(x => x.transform.position.x).ToList(); //sort by x value
        List<GameObject> list = _markers.OrderBy(
            x => Vector3.SqrMagnitude(
                x.transform.position - position)).ToList();

        //get the closest marker
        GameObject closest = list[0];
        Vector3 scale = closest.transform.localScale;

        //test if new interaction is within range of closest existing marker
        if (Vector3.Distance(position,
            closest.transform.position) <= scale.x)
        {
            //increase existing marker
            scale.x += Increase;
            scale.y += Increase;
            scale.z += Increase;
            closest.transform.localScale = scale;
            return; //return so we don't add a new marker
        }

        //no marker close, so add new marker
        AddMark(position);
    }

    private void AddMark(Vector3 position)
    {
        GameObject mark = Instantiate(MarkerPrefab, position, Quaternion.identity);
        mark.transform.parent = transform; //parent the marker prefab to the Interaction Marker gameObject
        _markers.Add(mark);
    }
}