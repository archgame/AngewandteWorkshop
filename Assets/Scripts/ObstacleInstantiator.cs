using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleInstantiator : MonoBehaviour
{
    public GameObject ObstaclePrefab;
    public int ObstacleLimit;

    private List<GameObject> _obstacles = new List<GameObject>();

    private void Update()
    {
        //if no mouse click, don't do anything
        if (!Input.GetMouseButtonDown(0)) { return; }

        Vector3 position = ClickPosition("Floor");
        GameObject obstacle = Instantiate(ObstaclePrefab, position, Quaternion.identity);
        _obstacles.Add(obstacle);
    }

    private Vector3 ClickPosition(string layer)
    {
        Vector3 screenPoint = Input.mousePosition; //mouse poisition on the screen
        Ray ray = Camera.main.ScreenPointToRay(screenPoint); //converting mouse position to world position
        RaycastHit hit;
        if (!Physics.Raycast(ray.origin, ray.direction, out hit)) return Vector3.zero; //was something hit?
        if (hit.transform.gameObject.layer != LayerMask.NameToLayer(layer)) return Vector3.zero; //was hit on the layer?

        //successful hit
        return hit.point;
    }
}