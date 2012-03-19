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
	
	private bool debug = false;
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start () {
		currentDestination = this.transform.position;
		
		//findNextPath();
		lastUpdate = Time.time;
	}
	
	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update () {
		/*float now = Time.time;
		
		if(now - lastUpdate > 1.5f) {
			findNextPath();
			lastUpdate = now;
		}*/
	}
	
	/// <summary>
	/// Finds a next path to take.
	/// </summary>
	public void findNextPath() {
		if(debug) Debug.Log("RandomPathFinder.findNextPath()");
		
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
	
	/// <summary>
	/// Picks a next random destination, excluding the current one.
	/// </summary>
	/// 
	/// <param name="currentDestination">The current destination.</param>
	private Vector3 getRandomDestination(Vector3 currentDestination) {
		if(debug) Debug.Log("RandomPathFinder.getRandomDestination()");
		
		Vector3 newDestination = destinationsList[ (int)UnityEngine.Random.Range(0, destinationsList.Length) ].transform.position;
				
		// Make sure we don't go to the same destination 2 times in a row.
		if(newDestination == currentDestination) {
			newDestination = getRandomDestination(currentDestination);
		}
		
		return newDestination;
	}
	
	/// <summary>
	/// Retreives the current path.
	/// </summary>
	public Path currentPath {
		get {
			return this._currentPath;
		}
	}
	
	// =================
	// EVENT HANDLERS
	// =================
	
	/// <summary>
	/// Called when pathfinding via Navigator.targetPosition.
	/// </summary>
	private void OnNewPath(Path path) {
		if(debug) Debug.Log ("Received new Path from " + path.StartNode + " to " + path.EndNode + ". Took " + path.SeekTime + " seconds.");
		_currentPath = path;
		
		if(newPathEventHandler != null) {
			newPathEventHandler(this, EventArgs.Empty);
		}
	}
	
	/// <summary>
	/// Called when pathfinding via Navigator.targetPosition.
	/// </summary>
	private void OnTargetUnreachable() {
		if(debug) Debug.Log ("Could not pathfind to target position");
		_currentPath = null;
	}
	
	/// <summary>
	/// Called when pathfinding via Navigator.RequestPath (startPositio, endPosition).
	/// </summary>
	private void OnPathAvailable(Path path) {
		if(debug) Debug.Log ("Requested Path from " + path.StartNode + " to " + path.EndNode + " is now available. Took " + path.SeekTime + " seconds.");
	}
	
	/// <summary>
	/// Called when pathfinding via Navigator.RequestPath (startPositio, endPosition)
	/// </summary>
	private void OnPathUnavailable() {
		if(debug) Debug.Log ("The requested path could not be established.");
	}
	
	/// <summary>
	/// Called when a path requested by a Navigator on this GameObject is no 
	/// longer valid - due to a connection or node disabling or removal
	/// </summary>
	private void OnPathInvalidated (Path path) {
		if(debug) Debug.Log ("The path from " + path.StartNode + " to " + path.EndNode + " is no longer valid.");
	}
	
	/// <summary>
	/// Called when visualizing the Path via the Gizmos system is required.
	/// </summary>
	private void OnDrawGizmos() {
		if (currentPath != null) {
			currentPath.OnDrawGizmos();
		}
	}

}
