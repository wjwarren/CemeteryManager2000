using UnityEngine;
using System.Collections;

// Place the script in the Camera-Control group in the component menu
[AddComponentMenu("Camera-Control/CM2000 Camera Manager")]
public class CameraManager:MonoBehaviour {
	
	// The smooth script to update.
	public SmoothFollow smoother;
	
	// Dummy target that moves on a flat plane.
	public Transform dummyTarget;
	
	// Real camera target that will be update based on the dummy and the terrain.
	public Transform realTarget;
	
	// Whether or not the camera should follow the terrain.
	public bool followTerrain;
	
	// The terrain to follow.
	public Terrain terrain;
	
	// Minimum distance to target.
	public float minDistance = 100;
	
	// Maximum distance to target.
	public float maxDistance = 200;
	
	// Multiplier for how much to step per update.
	public float zoomMultiplier = 50;
	
	// Angle in degrees of the camera with the target. (Valid values are 0-360.)
	public float angle = 45;
	
	// The offset at which the real camera target follows the dummy.
	public Vector3 followOffset = new Vector3(0, 2, -10);
	
	// Whether or not to move the camera when the mouse is moved to the border of the screen.
	public bool scrollAtBorders = true;
	
	// Percentage of the screen to use for camera movement.
	public float scrollableBorderPercentage = 10;
	
	// Use this for initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
		if(!smoother || !dummyTarget || !realTarget) return;
		
		UpdateDolly();
		UpdatePosition();
	}
	
	/**
	 * Updates the position of the camera target
	 * based on the dummy x,z position and (if used)
	 * the terrain height.
	 */
	void UpdatePosition() {
		Vector3 newPosition = new Vector3();
		newPosition.x = dummyTarget.position.x + followOffset.x;
		newPosition.z = dummyTarget.position.z + followOffset.z;

		newPosition.y = realTarget.position.y;
		if(followTerrain && terrain != null) {
			newPosition.y = terrain.SampleHeight(dummyTarget.position);
		}
		newPosition.y += followOffset.y;
		
		realTarget.position = newPosition;
	}
	
	/**
	 * Updates the distance of the camera to the target.
	 */
	void UpdateDolly() {
		float delta = Input.GetAxis("Mouse ScrollWheel") * zoomMultiplier;
		
		// Check the new distance to the cam.
		float newDistance = smoother.distance - delta;
		if(newDistance < minDistance) {
			newDistance = minDistance;
		}
		if(newDistance > maxDistance) {
			newDistance = maxDistance;
		}
		
		// Update height to maintain the same angle.
		float angleInRad = Mathf.Deg2Rad * angle;
		float newHeight = newDistance * Mathf.Sin(angleInRad);
		
		smoother.distance = newDistance;
		smoother.height = newHeight;
	}
}