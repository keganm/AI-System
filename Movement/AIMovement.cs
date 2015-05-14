using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMovement : MonoBehaviour {
	public AIEnumeration.Tracking trackingState;
	NavMeshAgent nav;
	NavMeshPath path;

	public float velocitySmoothness = 0.1f;
	float avgVelocity = 0.0f;
	float velocityMultiplier = 1.0f;
	Animator anim;

	//Wander Behaviour Variables
	Vector3 target;
	float newTargetTimer;
	float wanderRadius = 0f;

	Rect navmeshBounds;
	AIGridController gridController;

	//Search Agent Movement Variables
	Quaternion lastRotation;
	float normalSpeed =3;
	public AIResourceTarget resourceTarget;

	// Use this for initialization
	void Start () {
		nav = this.GetComponent<NavMeshAgent> ();
		path = new NavMeshPath ();

		resourceTarget = this.gameObject.AddComponent<AIResourceTarget>();
		gridController = this.gameObject.AddComponent<AIGridController> ();


		anim = this.GetComponentInChildren<Animator> ();
		velocityMultiplier = 1.0f / nav.speed * 2f;



		CalculateNavMeshBounds ();

	}
	
	// Update is called once per frame
	void Update () {

		UpdateMovement ();	

		if (nav.desiredVelocity.sqrMagnitude > 0.0f) {
						Quaternion lookRot = Quaternion.LookRotation (nav.desiredVelocity, this.transform.up);
			this.transform.rotation = Quaternion.Lerp (this.transform.rotation, lookRot, (1.0f - nav.desiredVelocity.sqrMagnitude) * 0.1f);
				}

		avgVelocity = (avgVelocity * (1.0f - velocitySmoothness)) + (nav.velocity.magnitude * velocitySmoothness);
		anim.SetFloat ("Forward", avgVelocity * velocityMultiplier);
	}

	void UpdateMovement() {
		if (trackingState == AIEnumeration.Tracking.Refilling) {
						resourceTarget.ClearTargetList();
						path = new NavMeshPath ();
						nav.SetPath (path);
						return;
				}
		if (trackingState == AIEnumeration.Tracking.Aimless) {
						Wander ();
		}
	}

	void Wander()
	{
		//TODO: Secondary Wander to find resources when necassary

		if(wanderRadius < 10.0f)
			wanderRadius += AIInitialVariables.wanderIncrease;

		if (Vector3.Distance(this.transform.position, nav.destination) < 0.2f)
						ReTarget ();
	}

	void ReTarget(){

		if (this.gameObject.GetComponent<AINeedManager> ().needed.Count >= 1) {
			bypassGridController();
			return;
				}

		Vector3 testpoint = new Vector3 (gridController.GetCurrentGrid ().Center.x, 0, gridController.GetCurrentGrid ().Center.y);
		
		int ptry = 0;
		bool succesfull = false;
		while (!succesfull) {
			target = testpoint + (Random.insideUnitSphere * gridController.GetCurrentGrid ().SearchSize);
			target.Scale (new Vector3 (1, 0, 1));
			succesfull = nav.SetDestination (target);
			ptry++;
		}

		gridController.DoSearch();

	}

	public void bypassGridController(){
		bool succesfull = false;
		while(!succesfull){
			target = Random.insideUnitSphere * gridController.fullGrid.SearchSize;
			succesfull = nav.SetDestination(target);
		}
	}

	public void SearchForResource(List<string> resource, bool redoSearch) {
				resourceTarget.SearchForResource (resource, redoSearch);
	}

	void CalculateNavMeshBounds(){
		Vector3[] verticies = NavMesh.CalculateTriangulation ().vertices;

		float Top = verticies [0].z;
		float Bottom = verticies [0].z;
		float Left = verticies [0].x;
		float Right = verticies [0].x;

		foreach (Vector3 vert in verticies) {
			if(vert.x < Left)
				Left = vert.x;
			if(vert.x > Right)
				Right = vert.x;
			if(vert.z < Top)
				Top = vert.z;
			if(vert.z > Bottom)
				Bottom = vert.z;
				}

		//Debug.Log ("NavMesh Bounds: " + Left + " , " + Top + ", " + Right + ", " + Bottom);
		navmeshBounds = new Rect (Left, Top, Right - Left, Bottom - Top);


		gridController.BuildGrid (navmeshBounds);
	}
}
