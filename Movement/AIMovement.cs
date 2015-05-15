/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI movement.
/// 
/// Used to manage navmesh and modify movement of AI
/// </summary>
public class AIMovement : MonoBehaviour
{
		//Tracking Variables
		public AIEnumeration.Tracking trackingState;
		NavMeshAgent nav;
		NavMeshPath path;

		//Movement Variables
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
		public AIResourceTarget resourceTarget;

		/// <summary>
		/// Setup connections and components
		/// </summary>
		void Start ()
		{
				nav = this.GetComponent<NavMeshAgent> ();
				path = new NavMeshPath ();

				//TODO:Unify building the grid
				gridController.BuildGrid (NavMesh.CalculateTriangulation ().vertices);

				anim = this.GetComponentInChildren<Animator> ();

				velocityMultiplier = 1.0f / nav.speed * 2f;
		}

		public void Init (AIResourceTarget _target, AIGridController _controller)
		{
				resourceTarget = _target;
				gridController = _controller;
		}

		/// <summary>
		/// Update movement and modify navAgent movement.
		/// </summary>
		void Update ()
		{
				//Updates movement
				UpdateMovement ();	
				
				//Modify rotation
				if (nav.desiredVelocity.sqrMagnitude > 0.0f) {
						Quaternion lookRot = Quaternion.LookRotation (nav.desiredVelocity, this.transform.up);
						this.transform.rotation = Quaternion.Lerp (this.transform.rotation, lookRot, (1.0f - nav.desiredVelocity.sqrMagnitude) * 0.1f);
				}
				
				//Modify movement velocity
				avgVelocity = (avgVelocity * (1.0f - velocitySmoothness)) + (nav.velocity.magnitude * velocitySmoothness);
				anim.SetFloat ("Forward", avgVelocity * velocityMultiplier);
		}

		/// <summary>
		/// Update navMesh movement depending on TrackingState
		/// </summary>
		void UpdateMovement ()
		{
				//AI is refilling so make the AI hold
				if (trackingState == AIEnumeration.Tracking.Refilling) {
						resourceTarget.ClearTargetList ();
						path = new NavMeshPath ();
						nav.SetPath (path);
						return;
				}
				//Tracking is aimless so AI explores
				if (trackingState == AIEnumeration.Tracking.Aimless) {
						Wander ();
				}
		}

		/// <summary>
		/// Wander for AI to explore environment.
		/// TODO:Modify for a more dynamic exploration
		/// </summary>
		void Wander ()
		{
				//Wander increase could be used for an AI to explore a particular area		
				
				if (Vector3.Distance (this.transform.position, nav.destination) < 0.2f)
						ReTarget ();
		}
		
		/// <summary>
		/// Retargets AI when called
		/// If AI needs a resource, the slow exploration will be bypassed to try and find needed resource
		/// Otherwise will use the exploration grid to explore the environment
		/// </summary>
		void ReTarget ()
		{
				//Bypass GridController if there are needed resources
				if (this.gameObject.GetComponent<AINeedManager> ().neededResources.Count >= 1) {
						bypassGridController ();
						return;
				}

				//Get the current grid for AI
				AIGridComponent testGrid = gridController.GetCurrentGrid ();

				//Find a random point in grid to navigate to
				bool succesfull = false;
				while (!succesfull) {
						target = testGrid.GetCenter3 () + (Random.insideUnitSphere * testGrid.SearchSize);
						target.Scale (new Vector3 (1, 0, 1));
						succesfull = nav.SetDestination (target);
				}

				//Update current grid for next retargetting
				gridController.DoSearch ();
		}

		/// <summary>
		/// Bypass CurrentGrid and find a random point in navMesh
		/// </summary>
		public void bypassGridController ()
		{
				bool succesfull = false;
				while (!succesfull) {
						target = Random.insideUnitSphere * gridController.fullGrid.SearchSize;
						succesfull = nav.SetDestination (target);
				}
		}
}
