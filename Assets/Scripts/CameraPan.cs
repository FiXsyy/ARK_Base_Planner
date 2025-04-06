using UnityEngine;

public class CameraPan : MonoBehaviour
{
    private Vector3 mousePosDown;
    private Vector3 mouseVector;

    [SerializeField] private Camera cam;

    void LateUpdate(){
        if(Input.GetMouseButton(2) || (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))){
            //  Calculates move vector of mouse pointer
            mouseVector = cam.ScreenToWorldPoint(Input.mousePosition) - cam.transform.position;
            
            //  Gets starting position of mouse drag
            if(Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetMouseButtonDown(0)){
                mousePosDown = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            //  Changes Camera Transform
            cam.transform.position = mousePosDown - mouseVector;
        }
    }
}