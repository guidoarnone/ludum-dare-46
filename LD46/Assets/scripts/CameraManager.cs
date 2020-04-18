using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    private Camera camera;
    public float maxCameraSize;
    public float minCameraSize;
    public float zoomSpeed;

    void Start() {
      camera = gameObject.GetComponent<Camera>();
    }

    void Update() {
      updateZoom(Input.GetAxis("Mouse ScrollWheel"));
      if(Input.GetKeyDown(KeyCode.Space)) {
        center();
      }
      pan(Input.GetAxis("Mouse X"));
    }

    private void updateZoom(float scroll) {
      float newCameraSize = camera.orthographicSize += scroll * Time.deltaTime * zoomSpeed;
      if (newCameraSize > maxCameraSize) {
        camera.orthographicSize = maxCameraSize;
      } else if(newCameraSize < minCameraSize) {
        camera.orthographicSize = minCameraSize;
      } else {
        camera.orthographicSize = newCameraSize;
      }
    }

    private void center() {

    }

    private void pan(float panvalue) {
    }

}
