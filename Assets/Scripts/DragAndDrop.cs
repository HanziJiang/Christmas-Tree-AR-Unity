using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DragAndDrop : MonoBehaviour
{
    private bool dragging = false;
    private Transform toDrag;
 
    void Update()
    {
        if (Input.touchCount != 1)
        {
            dragging = false;
            return;
        }
 
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
                    dragging = true;
                }
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


    void SpawnObject(Touch fireTouch)
    {
        var posZ = 1;  // posZ: number of units from the camera
        var touchPos3D = new Vector3(fireTouch.position.x, fireTouch.position.y, posZ);
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(touchPos3D);
        toDrag.transform.position = touchPos;
    }

}
