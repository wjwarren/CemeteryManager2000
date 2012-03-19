using UnityEngine;
using UnitySteer;
using C5;
using System.Collections;

public class AngryAntPolylinePathway : Vector3Pathway {
	
	private Path aaPath;
	private IList<Vector3> aaPoints;
	
	private bool debug = false;
	
	/// <summary>
	/// CONSTRUCTOR
	/// </summary>
	public AngryAntPolylinePathway() {
		// Do not use, it's useless! ;)
	}
	
	/// <summary>
	/// CONSTRUCTOR
	/// </summary>
	///
	/// <param name="path">The AngryAnt Path to follow.</param>
	/// <param name="defaultRadius"> A default path radius to use (when there are no segments).</param>
	/// <param name="cyclic">Whether or not the path is cyclic (comes back to the start).</param>
	public AngryAntPolylinePathway(Path path, float defaultRadius, bool cyclic){
		if(debug) Debug.Log("AngryAntPolylinePathway.AngryAntPolylinePathway()");
		
		preInitialize(path, defaultRadius, cyclic);
		Initialize(aaPoints, _radius, cyclic);
	}
	
	/// <summary>
	/// Does some additional setup needed before calling the 
	/// Vector3Pathway.Initialize() method.
	/// 
	/// Most of the work is converting the AngryAnt path to 
	/// a new ArrayList<Vector3> that Vector3Pathway understands.
	/// </summary>
	/// 
	/// <param name="path">The AngryAnt Path to follow.</param>
	/// <param name="defaultRadius">A default path radius to use (when there are no segments).</param>
	/// <param name="cyclic">Whether or not the path is cyclic (comes back to the start).</param>
	protected void preInitialize(Path path, float defaultRadius, bool cyclic) {
		if(debug) Debug.Log("AngryAntPolylinePathway.preInitialize()");
		
		int pointCount = -1;
		
		this.aaPath = path;
		IsCyclic = cyclic;
		
		if(path.Segments.Count != 0) {
			int targetSegment = (int)(path.Segments.Count * 0.5);
			Connection segment = path.Segments[targetSegment];
			Waypoint waypoint = segment.From;
			
			_radius = waypoint.Radius;
			pointCount = path.Segments.Count + 3;
		} else {
			_radius = defaultRadius;
			pointCount = 2;
		}
		
		// Create all points for the polyline.
		aaPoints = new ArrayList<Vector3>(pointCount);
		// Add start
		aaPoints.Add(path.StartPosition); // Don't add start!??
		
		// Add all segment starts, if any.
		if(path.Segments.Count != 0) {
			for(int i = 0; i < path.Segments.Count; i++) {
				Connection segment = path.Segments[i];
				Waypoint waypoint = segment.From;
				
				aaPoints.Add(waypoint.Position);
				
				// Add To from last Segment.
				if(i == path.Segments.Count - 1) {
					waypoint = segment.To;
					aaPoints.Add(waypoint.Position);
				}
			}
		}
		
		// Add end point.
		aaPoints.Add(path.EndPosition);
	}
	
}