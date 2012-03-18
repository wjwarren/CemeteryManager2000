using UnityEngine;

using UnitySteer;
using UnitySteer.Helpers;

using System;
using System.Collections;

/**
 * Lets an NPC wander around (forever) between various destinations.
 */
[RequireComponent (typeof (RandomPathFinder))]
[RequireComponent (typeof (SteerForPathSimplified))]
public class NPCController : MonoBehaviour {

	public float minIdleTime;
	public float maxIdleTime;
	
	private RandomPathFinder pathFinder;
	private SteerForPathSimplified pathFollower;
	
	private bool hasArrived;
	private float arrivalTime;
	
	private float idleTime;
	
	/**
	 * Use this for initialization.
	 */
	protected void Start () {
		pathFinder = GetComponent<RandomPathFinder>();
		pathFinder.newPathEventHandler += new NewPathEventHandler(newPathHandler);
		
		pathFollower = GetComponent<SteerForPathSimplified>();
		pathFollower.OnArrival = arrivalHandler;
		
		findNextPath();
	}
	
	/**
	 * Update is called once per frame
	 */
	protected void Update () {
		if(hasArrived && Time.time - arrivalTime > idleTime) {
			findNextPath();
		}
	}
	
	/**
	 * 
	 */
	private void findNextPath() {
		hasArrived = false;
		pathFinder.findNextPath();
	}
	
	// =================
	// EVENT HANDLERS
	// =================
	
	/**
	 * Called when we've found a new path.
	 */
	private void newPathHandler(RandomPathFinder sender, EventArgs e) {
		Debug.Log("NPC Received new Path.");
		
		//pathFollower.Path = pathFinder.currentPath;
		Path path = pathFinder.currentPath;
		
		//AngryAntTwoPathway aaPathway = new AngryAntTwoPathway(path, 1, false);
		AngryAntPolylinePathway aaPathway = new AngryAntPolylinePathway(path, 0.5f, false);
		pathFollower.Path = aaPathway;
	}
	
	/**
	 * Called when we have arrived at our destination.
	 * TODO: Why can't I use SteeringEvent here?
	 */
	private void arrivalHandler(SteeringEvent<Vehicle> message) {
		Debug.Log("NPC arrived at destination! :D");
		
		if(minIdleTime == maxIdleTime && minIdleTime == 0) {
			findNextPath();
		} else {
			hasArrived = true;
			arrivalTime = Time.time;
			idleTime = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
			
			Debug.Log(" - Idling for: " + idleTime.ToString());
		}
	}
}
