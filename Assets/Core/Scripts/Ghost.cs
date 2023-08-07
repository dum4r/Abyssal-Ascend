using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {
	public float       velocity;
	public Transform[] pointsWay;
	private int         indx;
	
	private void Start() { MyHorizontalAxis(); }
	private void FixedUpdate() {
		transform.position = Vector2.MoveTowards(transform.position, pointsWay[indx].position, velocity);
		if ( Vector2.Distance(transform.position, pointsWay[indx].position) < 0.2f ) {
			indx++;
			if ( indx >=  pointsWay.Length) indx = 0;
			MyHorizontalAxis();
		}
	}
	private void MyHorizontalAxis() {
		transform.localScale = new Vector3(transform.position.x > pointsWay[indx].position.x ? -8 : 8, 8, 1);
	}
}
