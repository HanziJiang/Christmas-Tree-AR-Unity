using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TouchManager : MonoBehaviour
{

    public Material outlineMat; 
    private Material selectedOrigMat;
    private bool dragging = false;
    private Transform toDrag;

    private GameObject selectedObject;
    private Renderer selectedObjRenderer;

    private float initialDistance;
    private Vector3 initialScale;
 
    void Update()
    {
        if (Input.touchCount < 1)
        {
            dragging = false;
            return;
        }


        if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];
            Vector3 pos = touch.position;
    
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos);
                RaycastHit hit;
    
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "draggable")
                    {
                        toDrag = hit.transform;
                        SelectObject(toDrag.gameObject);
                        dragging = true;
                    }
                }
                else {
                    ClearSelection();
                }
            }
            if (dragging && touch.phase == TouchPhase.Moved)
            {
                // move object with touch   
                SpawnObject(touch);
            }
    
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                dragging = false;
            }


        }

        // scale using pinch involves two touches
        // we need to count both the touches, store it somewhere, measure the distance between pinch 
        // and scale gameobject depending on the pinch distance
        // we also need to ignore if the pinch distance is small (cases where two touches are registered accidentally)

        if(selectedObject != null && Input.touchCount == 2)
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
                initialScale = selectedObject.transform.localScale;
                Debug.Log("Initial Distance: " + initialDistance + "GameObject Name: "
                    + selectedObject.name); // Just to check in console
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
                selectedObject.transform.localScale = initialScale * factor; // scale multiplied by the factor we calculated
            }
        }
    }

  void SelectObject(GameObject obj) {
		if(selectedObject != null) {
			if(obj == selectedObject)
				return;

			ClearSelection();
		}
		selectedObject = obj;
        selectedObjRenderer = obj.GetComponent<Renderer>();

        selectedOrigMat = selectedObjRenderer.material;  // save the original material

        selectedObjRenderer.material = outlineMat;  // highlight the outline selected object
	}

	void ClearSelection() {
		if(selectedObject == null)
			return;

        selectedObjRenderer.material = selectedOrigMat;  //restore the original material

        selectedObject = null;
	}

    void SpawnObject(Touch fireTouch)
    {
        var posZ = 1;  // posZ: number of units from the camera
        var touchPos3D = new Vector3(fireTouch.position.x, fireTouch.position.y, posZ);
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touchPos3D);

        // this is done to avoid sudden jumps in object position when moving it
        Vector3 objCenter = selectedObjRenderer.bounds.center;
        Vector3 center2Botton = selectedObject.transform.position - objCenter;
        selectedObject.transform.position = touchPos + center2Botton/2;

        // selectedObject.transform.position = touchPos;
    }

}
