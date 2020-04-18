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

    void Start() {
      camera = gameObject.GetComponent<Camera>();
      cameraCenter = camera.transform.position;
    }

    void Update() {
      updateZoom(Input.GetAxis("Mouse ScrollWheel"));
      if(Input.GetKeyDown(KeyCode.Space)) {
        center();
      }
      pan(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
    }

    private void updateZoom(float scroll) {
      float speed = (float)20 * zoomSpeed * zoomSpeed;
      float newCameraSize = camera.orthographicSize -= scroll * Time.deltaTime * speed;
      camera.orthographicSize = Mathf.Clamp(newCameraSize, minCameraSize, maxCameraSize);
    }

    private void center() {

    }

    private void pan(float mouseX, float mouseZ) {
      float newPanX = camera.transform.position.x + (mouseX * Time.deltaTime * panSpeed);
      float newPanZ = camera.transform.position.z + (mouseZ * Time.deltaTime * panSpeed * 5);
      float rescalePanX = Mathf.Clamp(newPanX, cameraCenter.x-maxPanXDistance, cameraCenter.x+maxPanXDistance) - camera.transform.position.x;
      float rescalePanZ = Mathf.Clamp(newPanZ, cameraCenter.z-maxPanZDistance, cameraCenter.z+maxPanZDistance) - camera.transform.position.z;
      camera.transform.Translate(rescalePanX,0,rescalePanZ);
    }

}
