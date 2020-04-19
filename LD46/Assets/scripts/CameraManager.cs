using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    protected const float worldScale = 10f;

    [Range(0, 1)]
    [SerializeField]
    protected float smoothing;

    [SerializeField]
    protected Vector2 bounds;
    [SerializeField]
    protected Vector2 zoomBounds;
    [SerializeField]
    [Range(1, 10)]
    protected float zoomSpeed;

    [SerializeField]
    [Range(1, 10)]
    protected float panSpeed;
    [SerializeField]
    protected int panTolerance;

    protected new Camera camera;
    protected Vector3 center;

    protected Vector3 desiredPosition;

    void Start() {
        camera = Camera.main;
        center = camera.transform.position;
        desiredPosition = center;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) { centerCamera(); }
        else {
            float scrollwheel = Input.GetAxis("Mouse ScrollWheel");
            desiredPosition += pan(getMouseAxis()) + zoom(scrollwheel);
            clamp();
        }
        camera.transform.position = Vector3.Lerp(camera.transform.position, desiredPosition, 1-smoothing*0.99f);
    }

    private Vector2 getMouseAxis() {
        Vector2 mouse;
        mouse.x = Math.sign(Input.mousePosition.x - panTolerance) + Math.sign(Input.mousePosition.x - (Screen.width - panTolerance));
        mouse.y = Math.sign(Input.mousePosition.y - panTolerance) + Math.sign(Input.mousePosition.y - (Screen.height - panTolerance));
        return mouse;
    }

    private Vector3 zoom(float scroll) {
        return camera.transform.forward * scroll * Mathf.Pow(5, zoomSpeed) * worldScale * Time.deltaTime;
    }
    
    private Vector3 pan(Vector2 mouse) {
        return (mouse.x * camera.transform.right + Vector3.Cross(camera.transform.right, Vector3.up) * mouse.y) * panSpeed * worldScale * Time.deltaTime;
    }

    protected void clamp() {
        desiredPosition = VectorMath.clamp(desiredPosition, new Vector3(-bounds.x, zoomBounds.x, -bounds.y), new Vector3(bounds.x, zoomBounds.y, bounds.y));
    }

    private void centerCamera() { desiredPosition = center; }
}
