using C5;

using UnityEngine;

using UnitySteer;
using UnitySteer.Helpers;

using System;
using System.Collections;

/// <summary>
/// Lets an NPC wander around (forever) between various destinations.
/// </summary>
[RequireComponent (typeof (RandomPathFinder))]
[RequireComponent (typeof (SteerForPathSimplified))]
[RequireComponent (typeof (AutonomousVehicle))]
[RequireComponent (typeof (Rigidbody))]
[AddComponentMenu("CemeteryManager2000/AI/NPC Controller")]
public class NPCController : MonoBehaviour {
	
	// The minimum idle time for the NPC when it reaches its destination.
	public float minIdleTime = 2.5f;
	
	// The maximum idle time for the NPC when it reaches its destination.
	public float maxIdleTime = 5.0f;
	
	// How long it takes for the character to turn a full turn (aka 360 degrees).
	public float rotationTime = 2.0f;
	
	private CharacterState state;
	
	private RandomPathFinder pathFinder;
	private SteerForPathSimplified pathFollower;
	private AngryAntPolylinePathway aaPathway;
	
	private float arrivalTime;
	private float idleTime;
	// When we need to start rotating the NPC.
	private float rotationStartTime;
	
	// The rotation needed before starting on a new path.
	private Quaternion targetRotation;
	// The rotation needed before starting on a new path.
	private Quaternion startRotation;
	// The amount of rotation needed to reach the targetRotation.
	private float deltaRotation;
	// How much to rotate per second.
	private float rotationStep;
	
	private bool debug = true;
		
	/// <summary>
	/// Use this for initialization.
	/// </summary>
	protected void Start () {
		if(debug) Debug.Log("NPCController.Start()");
		
		state = CharacterState.IDLING;
		
		pathFinder = GetComponent<RandomPathFinder>();
		pathFinder.newPathEventHandler += new NewPathEventHandler(newPathHandler);
		
		pathFollower = GetComponent<SteerForPathSimplified>();
		pathFollower.OnArrival = null;
		
		arrivalTime = 0;
		idleTime = 0;
		rotationStartTime = 0;
		
		rotationStep = (float)(Math.PI * 2) / rotationTime;
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	protected void Update () {
		if(state == CharacterState.IDLING) {
			if(shouldRotateNow()) {
				findNextPath();
			}
		} else if(state == CharacterState.ROTATING) {
			rotate();
		} else if(state == CharacterState.READY) {
			if(!continueIdling()){
				startWalking();
			}
		}
	}
	
	/// <summary>
	/// Whether or not we should continue idling.
	/// </summary>
	private bool continueIdling() {
		return (Time.time - arrivalTime < idleTime);
	}
	
	/// <summary>
	/// Whether or not we should start rotating the NPC to
	/// face the next path.
	/// </summary>
	private bool shouldRotateNow() {
		return (Time.time - arrivalTime > rotationStartTime);
	}
	
	/// <summary>
	/// Finds the next path to take.
	/// </summary>
	private void findNextPath() {
		if(debug) Debug.Log("NPCController.findNextPath()");
		
		state = CharacterState.SEEKING;
		pathFinder.findNextPath();
	}
	
	/// <summary>
	/// Makes the NPC walk.
	/// </summary>
	private void startWalking() {
		if(debug) Debug.Log("NPCController.startWalking() " + Time.time.ToString());
		
		state = CharacterState.WALKING;
		pathFollower.Path = aaPathway;
		
		pathFollower.OnArrival = arrivalHandler;
	}
	
	/// <summary>
	/// Rotates the NPC to where he should look in this point in time.
	/// </summary>
	/// 
	/// TODO: Easing!
	private void rotate() {
		//if(debug) Debug.Log("NPCController.rotate()");
		
		int rotationMultiplier = (deltaRotation < 0) ? -1 : 1;
		
		transform.Rotate(Vector3.up, rotationMultiplier * rotationStep * Time.deltaTime * Mathf.Rad2Deg, Space.Self);
		
		if(Math.Abs(targetRotation.y - transform.rotation.y) < rotationStep * Time.deltaTime * 0.25 ) {
			state = CharacterState.READY;
		}
	}
	
	/// <summary>
	/// Does some calculations and setup before starting
	/// to rotate our NPC to face the desired way.
	/// </summary>
	private void setupRotation() {
		if(debug) Debug.Log("NPCController.setupRotation()");
		
		state = CharacterState.ROTATING;
		startRotation = new Quaternion(0, transform.rotation.y, 0, 0);
		deltaRotation = targetRotation.y - startRotation.y;
	}
	
	/// <summary>
	/// Calculates the direction the NPC will be looking
	/// at based on the 2nd point in the next path.
	/// </summary>
	/// 
	/// <param name="path">The new/next path</param>
	private Quaternion getTargetRotation(IList<Vector3> path) {
		if(debug) Debug.Log("NPCController.getTargetRotation()");
		
		Quaternion rotation = new Quaternion();
		
		if(path.Count > 1) {
			Quaternion oldRotation = transform.rotation;
			transform.LookAt(path[1]);
			rotation = new Quaternion(0, transform.rotation.y, 0, 0);
			//rotation = Quaternion.LookRotation(path[1]);
			transform.rotation = oldRotation;
		} else {
			// Incorrect path, but let's not worry here.
			rotation = new Quaternion(0, transform.rotation.y, 0, 0);
		}
		
		return rotation;
	}
	
	// =================
	// EVENT HANDLERS
	// =================
	
	/// <summary>
	/// Called when we've found a new path.
	/// </summary>
	private void newPathHandler(RandomPathFinder sender, EventArgs e) {
		if(debug) Debug.Log("NPCController.newPathHandler()");
			
		Path path = pathFinder.currentPath;
		aaPathway = new AngryAntPolylinePathway(path, 0.5f, false);
		
		targetRotation = getTargetRotation(aaPathway.Path);
		if(targetRotation.y != transform.rotation.y) {
			setupRotation();
		} else if(!continueIdling()) {
			startWalking();
		}
	}
	
	/// <summary>
	/// Called when we have arrived at our destination.
	/// </summary>
	private void arrivalHandler(SteeringEvent<Vehicle> message) {
		if(debug) Debug.Log("NPCController.arrivalHandler() " + Time.time.ToString());
		
		arrivalTime = Time.time;
		idleTime = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
		
		state = CharacterState.IDLING;
		pathFollower.OnArrival = null;
		
		if(!continueIdling()) {
			findNextPath();
		} else {
			rotationStartTime = idleTime * 0.5f - rotationTime * 0.5f;
			if(rotationStartTime < 0){
				rotationStartTime = 0;
			}
			
			if(debug) Debug.Log(" - Idling for: " + idleTime.ToString());
		}
	}
}
