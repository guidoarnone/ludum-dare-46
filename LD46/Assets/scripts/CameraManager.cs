using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private Camera camera;
    [SerializeField]
    private float maxCameraSize;
    [SerializeField]
    private float minCameraSize;
    [SerializeField]
    [Range(1, 10)]
    private float zoomSpeed;
    private Vector3 cameraCenter;
    [SerializeField]
    private float maxPanXDistance;
    [SerializeField]
    private float maxPanZDistance;
    [SerializeField]
    [Range(1, 10)]
    private float panSpeed;
    [SerializeField]
    private int widthPanTolerance;
    [SerializeField]
    private int heightPanTolerance;

    void Start() {
      camera = gameObject.GetComponent<Camera>();
      cameraCenter = camera.transform.position;
    }

    void Update() {
      updateZoom(Input.GetAxis("Mouse ScrollWheel"));
      if(Input.GetKeyDown(KeyCode.Space)) {
        center();
      }
      pan(getMouseAxis());
    }

    private void updateZoom(float scroll) {
      float speed = 20 * zoomSpeed * zoomSpeed;
      float newCameraSize = camera.orthographicSize -= scroll * Time.deltaTime * speed;
      camera.orthographicSize = Mathf.Clamp(newCameraSize, minCameraSize, maxCameraSize);
    }

    private void center() {
      camera.transform.position = cameraCenter;
    }

    private void pan(Vector2 mouse) {
      float newPanX = camera.transform.position.x + (mouse.x * Time.deltaTime * panSpeed);
      float newPanZ = camera.transform.position.z + (mouse.y * Time.deltaTime * panSpeed);
      float rescalePanX = Mathf.Clamp(newPanX, cameraCenter.x-maxPanXDistance, cameraCenter.x+maxPanXDistance);
      float rescalePanZ = Mathf.Clamp(newPanZ, cameraCenter.z-maxPanZDistance, cameraCenter.z+maxPanZDistance);
      camera.transform.position = new Vector3(rescalePanX,transform.position.y,rescalePanZ);
    }

    private Vector2 getMouseAxis() {
      Vector2 mouseAxis  = new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
      if (Input.mousePosition.x >= Screen.width - widthPanTolerance) {
        mouseAxis.x = 1;
      } else if(Input.mousePosition.x <= widthPanTolerance) {
        mouseAxis.x = -1;
      }
      if (Input.mousePosition.y >= Screen.height - heightPanTolerance) {
        mouseAxis.y = 1;
      } else if(Input.mousePosition.y <= heightPanTolerance) {
        mouseAxis.y = -1;
      }
      return mouseAxis;
    }

}
