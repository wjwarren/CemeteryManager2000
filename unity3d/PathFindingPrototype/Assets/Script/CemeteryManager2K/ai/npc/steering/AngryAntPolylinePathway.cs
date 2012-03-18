using UnityEngine;
using UnitySteer;
using C5;
using System.Collections;

public class AngryAntPolylinePathway : Vector3Pathway {
	
	private Path aaPath;
	private IList<Vector3> aaPoints;
	
	/**
	 * 
	 */
	public AngryAntPolylinePathway() {
	}
	
	/**
	 * 
	 */
	public AngryAntPolylinePathway(Path path, float defaultRadius, bool cyclic){
		preInitialize(path, defaultRadius, cyclic);
		Initialize(aaPoints, _radius, cyclic);
	}
	
	/**
	 * 
	 */
	protected void preInitialize(Path path, float defaultRadius, bool cyclic) {
		Debug.Log("AngryAntPolylinePathway.preInitialize()");
		Debug.Log("Segments: " + path.Segments.Count.ToString());
		
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
		
		// TEST - print all points
		for(int j = 0; j < aaPoints.Count; j++) {
			Debug.Log(" - Point: " + aaPoints[j].ToString());
		}
	}
	
}