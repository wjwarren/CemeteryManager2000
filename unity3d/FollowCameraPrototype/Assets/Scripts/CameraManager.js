/**
 * Controls the distance of the camera to the target.
 */

// The smooth script to update.
var smoother:SmoothFollow;
 
// Minimum distance to target.
var minDistance:float = 100;

// Maximum distance to target.
var maxDistance:float = 200;

// Multiplier for how much to step per update.
var zoomMultiplier:float = 50;

// Angle in degrees of the camera with the target. (Valid values are 0-360.)
var angle:float = 45;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/CM2000 Camera Manager");

function Update () {
	if(!smoother) return;

	var delta:float = Input.GetAxis("Mouse ScrollWheel") * zoomMultiplier;
	
	// Check the new distance to the cam.
	var newDistance:float = smoother.distance - delta;
	if(newDistance < minDistance) {
		newDistance = minDistance;
	}
	if(newDistance > maxDistance) {
		newDistance = maxDistance;
	}
	
	// Update height to maintain the same angle.
	var angleInRad:float = Mathf.Deg2Rad * angle;
	var newHeight:float = newDistance * Mathf.Sin(angleInRad);
	
	smoother.distance = newDistance;
	smoother.height = newHeight;
}