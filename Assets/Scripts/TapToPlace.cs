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
    private GameObject placedObject;
    private bool placementPoseValid = false;
    private Pose placementPose;
    private Camera currentCamera;

    private float initialDistance;
    private Vector3 initialScale;

    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        currentCamera = FindObjectOfType<Camera>();
        placementIndicator = Instantiate(placementIndicatorPrefab);
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placedObject == null && placementPoseValid && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            print("Placing AR object...");
            ARPlaceObject();  // at the moment this just spawns the gameobject
        }

        // once the place is placed, single finger touch will `move` the object around
        else if (placedObject != null && Input.touchCount == 1) {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                SpawnBullet(touch);
            }
 
        }


        // scale using pinch involves two touches
        // we need to count both the touches, store it somewhere, measure the distance between pinch 
        // and scale gameobject depending on the pinch distance
        // we also need to ignore if the pinch distance is small (cases where two touches are registered accidentally)

        if(placedObject != null && Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            // if any one of touchzero or touchOne is cancelled or maybe ended then do nothing
            if(touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
            {
                return; // do nothing
            }

            if(touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = placedObject.transform.localScale;
                Debug.Log("Initial Distance: " + initialDistance + "GameObject Name: "
                    + placedObject.name); // Just to check in console
            }
            else // if touch is moved
            {
               var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                //if accidentally touched or pinch movement is very very small
                if (Mathf.Approximately(initialDistance, 0))
                {
                    return; // do nothing if it can be ignored where initial distance is very close to zero
                }

               var factor = currentDistance / initialDistance;
                placedObject.transform.localScale = initialScale * factor; // scale multiplied by the factor we calculated
            }
        }
        
    }

    void SpawnBullet(Touch fireTouch)
    {
        var posZ = 1;  // posZ: number of units from the camera
        var touchPos3D = new Vector3(fireTouch.position.x, fireTouch.position.y, posZ);
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touchPos3D);
        placedObject.transform.position = touchPos;
    }

    void ARPlaceObject()
    {
        placedObject = Instantiate(objectToPlacePrefab, placementPose.position, placementPose.rotation);

        // Destroy(placementIndicator);
        // enabled = false;
    }

    void UpdatePlacementPose()
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

    void UpdatePlacementIndicator()
    {
        if(placedObject == null && placementPoseValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }
}
