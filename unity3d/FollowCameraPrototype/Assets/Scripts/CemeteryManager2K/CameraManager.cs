using UnityEngine;
using System.Collections;

// Place the script in the Cemetery Manager 2000 group in the component menu
[AddComponentMenu("Cemetery Manager 2000/Camera Manager")]
public class CameraManager:MonoBehaviour {
	
	// The smooth script to update.
	public SmoothFollow smoother;
	
	// Dummy target that moves on a flat plane.
	public Transform dummyTarget;
	
	// Real camera target that will be update based on the dummy and the terrain.
	public Transform realTarget;
	
	// Whether or not to move in the direction the camera is looking.
	public bool followYRotation = true;
	
	// Whether or not the camera should follow the terrain.
	public bool followTerrain = true;
	
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
	public float scrollableBorderPercentage = 15;
	
	// Scroll multiplier to apply per step.
	public float scrollMultiplier = 150;
	
	private Rect scrollRect;
	private Rect maxBounds;
	private Vector2 previousScreenSize;
	
	// Use this for initialization
	void Start() {
		previousScreenSize = new Vector2(Screen.width, Screen.height);
		
		if(dummyTarget && followYRotation) {
			Quaternion newDummyRotation = dummyTarget.transform.rotation;
			newDummyRotation.y = transform.rotation.y;
			dummyTarget.transform.rotation = newDummyRotation;
		}
		
		setScrollRect();
	}
	
	// Update is called once per frame
	void Update() {
		if(!smoother || !dummyTarget || !realTarget) return;
		
		UpdateScrolling();
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
	
	/**
	 * Updates the camera target if scrolling at the borders is turned on.
	 */
	void UpdateScrolling() {
		if(!scrollAtBorders || !isMouseInBounds()) return;
		
		// Check if screen size has changed.
		if(previousScreenSize.x != Screen.width || previousScreenSize.y != Screen.height) {
			setScrollRect();
		}
		
		Vector3 mousePos = Input.mousePosition;		
		float delta = 0;
		Vector3 movement = new Vector3(0, 0, 0);
		
		// Left
		if(mousePos.x < scrollRect.x) {
			delta = Mathf.Abs(scrollRect.x - mousePos.x) / scrollRect.x;
			if(delta > 1) delta = 1;
			
			movement += dummyTarget.transform.right * scrollMultiplier * delta * Time.deltaTime * -1;
		}
		// Right
		if(mousePos.x > scrollRect.width) {
			delta = Mathf.Abs(mousePos.x - scrollRect.width) / scrollRect.x;
			if(delta > 1) delta = 1;
			
			movement += dummyTarget.transform.right * scrollMultiplier * delta * Time.deltaTime;
		}
		// Backward
		if(mousePos.y < scrollRect.y) {
			delta = Mathf.Abs(scrollRect.y - mousePos.y) / scrollRect.x;
			if(delta > 1) delta = 1;
			
			movement += dummyTarget.transform.forward * scrollMultiplier * delta * Time.deltaTime * -1;
			
		}
		// Forward
		if(mousePos.y > scrollRect.height) {
			delta = Mathf.Abs(mousePos.y - scrollRect.height) / scrollRect.x;
			if(delta > 1) delta = 1;
			
			movement += dummyTarget.transform.forward * scrollMultiplier * delta * Time.deltaTime;
		}
		
		// Only call Translate when an actual change in position is required.
		if(movement.sqrMagnitude != 0) {
			dummyTarget.transform.Translate(movement, Space.World);
		}
	}
	
	/**
	 * Calculates the rectangle to use to determine camera scrolling.
	 */
	void setScrollRect() {
		scrollRect = new Rect();
		scrollRect.x = (float)(Screen.width * scrollableBorderPercentage * 0.01);
		scrollRect.y = (float)(Screen.height * scrollableBorderPercentage * 0.01);
		scrollRect.width = (float)(Screen.width * (100- scrollableBorderPercentage) * 0.01);
		scrollRect.height = (float)(Screen.height * (100- scrollableBorderPercentage) * 0.01);
		
		maxBounds = new Rect();
		maxBounds.x = -scrollRect.x;
		maxBounds.y = -scrollRect.y;
		maxBounds.width = Screen.width + scrollRect.x;
		maxBounds.height = Screen.height + scrollRect.y;
	}
	
	/**
	 * Determines whether or not the mouse is in the bounding
	 * area for scrolling.
	 */
	bool isMouseInBounds() {
		bool result = true;
		Vector3 mousePos = Input.mousePosition;
		
		if(mousePos.x < maxBounds.x) result = false;
		if(mousePos.x > maxBounds.width) result = false;
		if(mousePos.y < maxBounds.y) result = false;
		if(mousePos.y > maxBounds.height) result = false;
		
		return result;
	}
}