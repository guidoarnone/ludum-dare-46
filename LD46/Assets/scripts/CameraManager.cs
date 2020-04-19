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
    [Range(1, 1000)]
    private float zoomSpeed;
    private Vector3 cameraCenter;
    [SerializeField]
    private float maxPanXDistance;
    [SerializeField]
    private float maxPanZDistance;
    [SerializeField]
    [Range(1, 1000)]
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
        Vector3 new_position = camera.transform.position +
            mouse.x * Time.deltaTime * panSpeed * camera.transform.right +
            Vector3.Cross(camera.transform.right, Vector3.up) * mouse.y * Time.deltaTime * panSpeed;
      float rescalePanX = Mathf.Clamp(new_position.x, cameraCenter.x-maxPanXDistance, cameraCenter.x+maxPanXDistance);
      float rescalePanZ = Mathf.Clamp(new_position.z, cameraCenter.z-maxPanZDistance, cameraCenter.z+maxPanZDistance);
        camera.transform.position = new Vector3(rescalePanX, camera.transform.position.y, rescalePanZ);
    }

    private Vector2 getMouseAxis() {
      Vector2 mouseAxis  = new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y")).normalized;
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
