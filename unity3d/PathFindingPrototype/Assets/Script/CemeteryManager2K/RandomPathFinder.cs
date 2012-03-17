using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Navigator))]
public class RandomPathFinder : MonoBehaviour {
	
	// List of destinations
	public GameObject[] destinationsList;
	
	// The current destination.
	private Vector3 currentDestination;
	
	// The current path.
	private Path currentPath;
	
	private float lastUpdate;
	
	/**
	 * Use this for initialization
	 */
	void Start () {
		currentDestination = this.transform.position;
		
		findNextPath();
		lastUpdate = Time.time;
	}
	
	/**
	 * Update is called once per frame
	 */ 
	void Update () {
		float now = Time.time;
		
		if(now - lastUpdate > 1.5f) {
			findNextPath();
			lastUpdate = now;
		}
	}
	
	/**
	 * Finds a next path to take.
	 */
	void findNextPath() {
		// Move to current position
		// (In real life we should have arrived or are nearly arriving, 
		// but let's fake it for now.)
		transform.position = currentDestination;
		// Find new destination
		currentDestination = getRandomDestination(currentDestination);
		// Find new Path
		GetComponent<Navigator> ().targetPosition = currentDestination;
		// Show new Path
		// Happens automatically when OnDrawGizmos() is called.
	}
	
	/**
	 * Picks a next random destination.
	 */
	Vector3 getRandomDestination(Vector3 currentDestination) {
		Vector3 newDestination = destinationsList[ (int)Random.Range(0, destinationsList.Length - 1) ].transform.position;
				
		// Make sure we don't go to the same destination 2 times in a row.
		if(newDestination == currentDestination) {
			newDestination = getRandomDestination(currentDestination);
		}
		
		return newDestination;
	}
	
	// =================
	// EVENT HANDLERS
	// =================
	
	/**
	 * Called when pathfinding via Navigator.targetPosition
	 */
	void OnNewPath(Path path) {
		Debug.Log ("Received new Path from " + path.StartNode + " to " + path.EndNode + ". Took " + path.SeekTime + " seconds.");
		currentPath = path;
	}
	
	/**
	 * When pathfinding via Navigator.targetPosition
	 */
	void OnTargetUnreachable() {
		Debug.Log ("Could not pathfind to target position");
		currentPath = null;
	}
	
	/**
	 * When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	 */
	void OnPathAvailable(Path path) {
		Debug.Log ("Requested Path from " + path.StartNode + " to " + path.EndNode + " is now available. Took " + path.SeekTime + " seconds.");
	}
	
	/**
	 * When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	 */
	void OnPathUnavailable() {
		Debug.Log ("The requested path could not be established.");
	}
	
	/**
	 * When a path requested by a Navigator on this GameObject is no longer valid - due to a connection or node disabling or removal
	 */
	void OnPathInvalidated (Path path) {
		Debug.Log ("The path from " + path.StartNode + " to " + path.EndNode + " is no longer valid.");
	}
	
	/**
	 * Called when visualizing the Path via the Gizmos system is required.
	 */
	void OnDrawGizmos() {
		if (currentPath != null) {
			currentPath.OnDrawGizmos();
		}
	}

}
