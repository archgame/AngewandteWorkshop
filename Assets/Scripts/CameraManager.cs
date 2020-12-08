using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    private List<GameObject> _cameras;
    private int _currentCamera = 0;
    private Camera _followCamera;
    private GameObject _followTarget;
    private bool _following = false;

    private void Start()
    {
        _cameras = GameObject.FindGameObjectsWithTag("MainCamera").ToList();
        _cameras.Remove(this.gameObject); //remove follow camera, assuming follow camera has this script
        foreach (GameObject camera in _cameras)
        {
            camera.SetActive(false);
        }
        _cameras[0].SetActive(true);

        //set up follow camera
        _followCamera = GetComponent<Camera>(); //assume follow camera is the script gameobject
        _followCamera.enabled = false;
    }

    private void Update()
    {
        //input
        if (Input.GetMouseButtonDown(0))
        {
            _followTarget = ClickObject("Agent");
            if (_followTarget != null)
            {
                _following = true;
                _followCamera.enabled = true;
                _cameras[_currentCamera].SetActive(false);
            }
        }

        //following
        if (_following)
        {
            //exit following
            if (Input.GetKeyDown(KeyCode.LeftArrow)
                || Input.GetKeyDown(KeyCode.RightArrow) // || = OR, && = AND
                || _followTarget == null)
            {
                _following = false;
                _followTarget = null;
                _followCamera.enabled = false;
                _cameras[_currentCamera].SetActive(true);
                return;
            }

            //follow target
            Vector3 position = _followTarget.transform.position; //get target position
            position -= _followTarget.transform.forward * 4; //move position behind target
            position += Vector3.up * 2; //move position above target
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 4); //set camera position to new target
            transform.forward = Vector3.Lerp(transform.forward, _followTarget.transform.forward, Time.deltaTime * 4); //turn camera to face same direction as target
            return;
        }

        //not following
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _cameras[_currentCamera].SetActive(false);
            _currentCamera--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _cameras[_currentCamera].SetActive(false);
            _currentCamera++;
        }

        if (_currentCamera < 0) { _currentCamera = _cameras.Count - 1; } //if too small, bring to top of list
        if (_currentCamera >= _cameras.Count) { _currentCamera = 0; } //if too large, bring to bottom of list

        _cameras[_currentCamera].SetActive(true);
    }

    private GameObject ClickObject(string layer)
    {
        Vector3 screenPoint = Input.mousePosition; //mouse poisition on the screen
        Ray ray = Camera.main.ScreenPointToRay(screenPoint); //converting mouse position to world position
        RaycastHit hit;
        if (!Physics.Raycast(ray.origin, ray.direction, out hit)) return null; //was something hit?
        if (hit.transform.gameObject.layer != LayerMask.NameToLayer(layer)) return null; //was hit on the layer?

        //successful hit
        return hit.transform.gameObject;
    }
}