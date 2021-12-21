using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TapToPlace : MonoBehaviour
{
    public GameObject placementIndicatorPrefab;
    public GameObject objectToPlacePrefab;

    private ARRaycastManager arRaycastManager;
    private GameObject placementIndicator;
    private GameObject objectToPlace;
    private bool placementPoseValid = false;
    private Pose placementPose;
    private Camera currentCamera;

    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        currentCamera = FindObjectOfType<Camera>();
        placementIndicator = Instantiate(placementIndicatorPrefab);
    }

    void Update()
    {
        UpdatePlacementPose();
        updatePlacementIndicator();

        if (placementPoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            placeObject();
        }
    }

    private void placeObject()
    {
        objectToPlace = Instantiate(objectToPlacePrefab, placementPose.position, placementPose.rotation);
        Destroy(placementIndicator);
        enabled = false;
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = currentCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseValid = hits.Count > 0;
        if (placementPoseValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = currentCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z);
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void updatePlacementIndicator()
    {
        placementIndicator.SetActive(placementPoseValid);

        if (placementPoseValid)
        {
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
    }
}
