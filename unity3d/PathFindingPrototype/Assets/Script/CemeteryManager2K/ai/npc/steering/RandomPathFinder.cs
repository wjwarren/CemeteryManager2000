using UnityEngine;
using System;
using System.Collections;

public delegate void NewPathEventHandler(RandomPathFinder sender, EventArgs e);

[RequireComponent (typeof (Navigator))]
public class RandomPathFinder : MonoBehaviour {
	
	// List of destinations
	public GameObject[] destinationsList;
	
	public event NewPathEventHandler newPathEventHandler;
	
	// The current destination.
	private Vector3 currentDestination;
	
	// The current path.
	private Path _currentPath;
	
	// Last update time.
	private float lastUpdate;
	
	/**
	 * Use this for initialization
	 */
	public void Start () {
		currentDestination = this.transform.position;
		
		//findNextPath();
		lastUpdate = Time.time;
	}
	
	/**
	 * Update is called once per frame
	 */ 
	public void Update () {
		/*float now = Time.time;
		
		if(now - lastUpdate > 1.5f) {
			findNextPath();
			lastUpdate = now;
		}*/
	}
	
	/**
	 * Finds a next path to take.
	 */
	public void findNextPath() {
		Debug.Log("RandomPathFinder.findNextPath()");
		
		// Move to current position
		// (In real life we should have arrived or are nearly arriving, 
		// but let's fake it for now.)
		//transform.position = currentDestination;
		
		// Find new destination
		currentDestination = getRandomDestination(currentDestination);
		// Find new Path
		GetComponent<Navigator>().targetPosition = currentDestination;
		
		// Show new Path
		// Happens automatically when OnDrawGizmos() is called.
	}
	
	/**
	 * Picks a next random destination.
	 */
	private Vector3 getRandomDestination(Vector3 currentDestination) {
		Debug.Log("RandomPathFinder.getRandomDestination()");
		
		Vector3 newDestination = destinationsList[ (int)UnityEngine.Random.Range(0, destinationsList.Length - 1) ].transform.position;
				
		// Make sure we don't go to the same destination 2 times in a row.
		if(newDestination == currentDestination) {
			newDestination = getRandomDestination(currentDestination);
		}
		
		return newDestination;
	}
	
	/**
	 * Retreives the current path.
	 */
	public Path currentPath {
		get {
			return this._currentPath;
		}
	}
	
	// =================
	// EVENT HANDLERS
	// =================
	
	/**
	 * Called when pathfinding via Navigator.targetPosition
	 */
	private void OnNewPath(Path path) {
		Debug.Log ("Received new Path from " + path.StartNode + " to " + path.EndNode + ". Took " + path.SeekTime + " seconds.");
		_currentPath = path;
		
		if(newPathEventHandler != null) {
			newPathEventHandler(this, EventArgs.Empty);
		}
	}
	
	/**
	 * When pathfinding via Navigator.targetPosition
	 */
	private void OnTargetUnreachable() {
		Debug.Log ("Could not pathfind to target position");
		_currentPath = null;
	}
	
	/**
	 * When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	 */
	private void OnPathAvailable(Path path) {
		Debug.Log ("Requested Path from " + path.StartNode + " to " + path.EndNode + " is now available. Took " + path.SeekTime + " seconds.");
	}
	
	/**
	 * When pathfinding via Navigator.RequestPath (startPositio, endPosition)
	 */
	private void OnPathUnavailable() {
		Debug.Log ("The requested path could not be established.");
	}
	
	/**
	 * When a path requested by a Navigator on this GameObject is no longer valid - due to a connection or node disabling or removal
	 */
	private void OnPathInvalidated (Path path) {
		Debug.Log ("The path from " + path.StartNode + " to " + path.EndNode + " is no longer valid.");
	}
	
	/**
	 * Called when visualizing the Path via the Gizmos system is required.
	 */
	private void OnDrawGizmos() {
		if (currentPath != null) {
			currentPath.OnDrawGizmos();
		}
	}

}
