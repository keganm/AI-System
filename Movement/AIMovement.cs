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
		AIEntity parent;

		//Tracking Variables
		public AIEnumeration.Tracking trackingState;
		NavMeshAgent nav;
		NavMeshPath path;

		//Movement Variables
		public float velocitySmoothness = 0.1f;
		float avgVelocity = 0.0f;
		float velocityMultiplier = 1.0f;
		Animator anim;
		
		public int maxWaitTimer = AIInitialVariables.waitTimerMax;
		public int waitTimer = AIInitialVariables.waitTimerMax + 1;

		//Wander Behaviour Variables
		Vector3 target;
		float newTargetTimer;
		float wanderRadius = 0f;
		Rect navmeshBounds;
		AIGridController gridController;

		//Search Agent Movement Variables
		Quaternion lastRotation;
		public AIResourceManager resourceTarget;

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

		public void Init (AIResourceManager _target, AIGridController _controller)
		{
				resourceTarget = _target;
				gridController = _controller;
				parent = this.GetComponent<AIEntity> ();
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
				
				//TODO: Create a more organic but similar approach (Possibly a more predictive test process?)
				if (Random.value > parent.traitManager.GetStopAndLookAroundProbability () && !IsWaiting())
						StartWait();

				waitTimer++;
				if (waitTimer < AIInitialVariables.waitTimerMax)
						nav.velocity *= AIInitialVariables.navStopSmoothness;

				nav.speed = parent.needManager.GetNeedTotal() * AIInitialVariables.navSpeedMulti + AIInitialVariables.navSpeedBase;
		}
		
		/// <summary>
		/// Determines whether this instance is waiting.
		/// </summary>
		/// <returns><c>true</c> if this instance is waiting; otherwise, <c>false</c>.</returns>
		public bool IsWaiting()
		{
				return waitTimer < AIInitialVariables.waitTimerMax;
		}
		
		/// <summary>
		/// Starts the wait.
		/// </summary>
		public void StartWait()
		{
				maxWaitTimer = AIInitialVariables.waitTimerMax;
				waitTimer = 0;
		}

		/// <summary>
		/// Starts the wait for specified time.
		/// </summary>
		/// <param name="_waitTime">Wait time.</param>
		public void StartWait(int _waitTime)
		{
				maxWaitTimer = _waitTime;
				waitTimer = 0;
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
		/// Tests the other AI to modify movement.
		/// </summary>
		void TestOtherAI()
		{
				int eCount = 0;
				int eCountMax = 100;
				float maintain = AIInitialVariables.otherAIMovementInfluence - ((1f - parent.needManager.GetNeedTotal()) * 0.1f);
				float drift = 1f - maintain;
			
				foreach (AIEntity entity in parent.awareness.entityList) {
					AIEnumeration.ReactionType reaction = parent.awareness.GetReactionLookup(entity);
					if (reaction != AIEnumeration.ReactionType.Neutral)
						EntityShift (entity.gameObject, reaction, maintain, drift);
					
					eCount++;
					if(eCount > eCountMax)
						return;
				}
		}
	
		/// <summary>
		/// Shifts Current movement towards or away from other entities.
		/// </summary>
		/// <param name="_target">Other AI GameObject.</param>
		/// <param name="_reaction">Reaction between two the AI.</param>
		/// <param name="_maintain">Maintain direction weight.</param>
		/// <param name="_drift">Chase direction weight.</param>
		void EntityShift (GameObject _target, AIEnumeration.ReactionType _reaction, float _maintain, float _drift)
		{
				float direction = 0f;
				switch (_reaction) {
				case AIEnumeration.ReactionType.Negative:
						direction = -1f;
						break;
				case AIEnumeration.ReactionType.Positive:
						direction = 1f;
						break;
				default:
						return;
				}

				Vector3 newDirection = _target.transform.position - nav.nextPosition;
				newDirection = newDirection * direction;
				//nav.velocity = newDirection;
				nav.nextPosition = nav.nextPosition * _maintain + newDirection * _drift;
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
