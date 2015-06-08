/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AIResourceManager : MonoBehaviour
{
		public struct GameObjectDistance
		{
				public GameObject gameObject;
				public float distance;

				public GameObjectDistance (GameObject _gameObject, float _distance)
				{
						gameObject = _gameObject;
						distance = _distance;
				}
		
				public override string ToString ()
				{
						return (string.Format ("{0},{1}", gameObject.name, distance));
				}
		}
		AIEntity parent;
		NavMeshAgent nav;
		AIMovement movement;
		AIAwareness awareness;
		AINeedManager needManager;
		public GameObject currentTarget;
		public string targetNeed = "";
		public string targetResource = "";
		public List<GameObject>resourceTargets = new List<GameObject> ();
		static float sweepTestResolution = AIInitialVariables.sweepTestResolution;
		static float minSweepTest = AIInitialVariables.minSweepTest;
		static float maxSweepTest = AIInitialVariables.maxSweepTest;


		// Use this for initialization
		void Awake ()
		{		
				parent = this.GetComponent<AIEntity> ();
				nav = this.GetComponent<NavMeshAgent> ();
				movement = this.GetComponent<AIMovement> ();
				awareness = this.GetComponent<AIAwareness> ();
				needManager = this.GetComponent<AINeedManager> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (currentTarget != null) {
						if (!SightCheckTarget () || nav.isPathStale)
								RetargetResource ();
						

						if (targetNeed == AIInitialVariables.needDictionary [AIEnumeration.ResourceType.Companionship].resource)
								SetNavToCurrentTarget ();
				

						if (!needManager.neededResources.Contains (targetNeed)) {
								currentTarget = null;
								targetNeed = "";
								targetResource = "";
								if (needManager.neededResources.Count > 0)
										BuildResourceTargets ();
						}
				} else {
						CheckForCloserResource ();
				}
		}

	
		/// <summary>
		/// Sights the check current target.
		/// </summary>
		/// <returns><c>true</c>, if resource is available, or not in sight, <c>false</c> If in sight or contact is made and resource is unavailable.</returns>
		bool SightCheckTarget ()
		{
				return SightCheckTarget (currentTarget);
		}

		/// <summary>
		/// Sights the check target.
		/// </summary>
		/// <returns><c>true</c>, if resource is available, or not in sight, <c>false</c> If in sight or contact is made and resource is unavailable.</returns>
		/// <param name="target">Target to check.</param>
		bool SightCheckTarget (GameObject target)
		{
				bool canSee = this.GetComponent<AIEntity> ().currentResources.Contains (target);
				if (!canSee)
						canSee = CheckLineOfSite (target);

				if (canSee) {
						AIResource res = target.GetComponent<AIResource> ();
			
						//Return false if we can see that there no resources are available in the target
						if (!res.ResourceAvailable ()) {
								return false;
						} else
								return true;
				}
				//Return true if can't see the resource
				return true;
		}

		/// <summary>
		/// Tries to find and map to the top resource
		/// otherwise will shift through the know resources
		/// </summary>
		void RetargetResource ()
		{
				BuildResourceTargets ();
				
		}

		/// <summary>
		/// Move the first resource target to the end and Retargets
		/// </summary>
		void ShiftResourceTargets ()
		{
				GameObject temp = resourceTargets [0];
				resourceTargets.RemoveAt (0);
				resourceTargets.Add (temp);
				RetargetResource ();
		}

		public bool CheckForCloserResource ()
		{
				List<GameObject> visibleResources = LookForResources (needManager.neededResources);
				if (visibleResources.Count <= 1)
						return false;
				float currentDistance = -1f;
				if (currentTarget != null)
						currentDistance = Vector3.Distance (this.transform.position, currentTarget.transform.position);

				visibleResources = SortListByDistance (visibleResources);
				float closestDistance = Vector3.Distance (this.transform.position, visibleResources [0].transform.position);
				if (closestDistance < currentDistance || currentDistance == -1f) {
						currentTarget = visibleResources [0];
						targetResource = visibleResources [0].transform.parent.name;
						targetNeed = AIInitialVariables.needDictionary [visibleResources [0].GetComponentInParent<AIResource> ().resourceType].resource;
						SetNavToCurrentTarget ();
				}
				return true;
		}

		public bool BuildResourceTargets ()
		{
				//Set time to wait based on AIs Tactics trait (Judging or Prospecting)
				float timeToWait = ((1f - parent.traitManager.GetNormalizedTrait (AIEnumeration.TraitType.Tactics)) * 0.5f + 0.5f) * (float)AIInitialVariables.waitTimerMax;
				parent.movement.StartWait (Mathf.FloorToInt( timeToWait ));

				float[] needValues = new float[needManager.needList.Count];
				string[] needResources = new string[needManager.needList.Count];
				int targetNeedIndex = -1;
				targetNeed = "";
				float targetNeedValue = -1f;

				if (!needManager.GetNeededResources (ref needValues, ref needResources)) {
						Debug.Log ("Error getting resources");
						return false;
				}
				if (!AIProbabilitySolver.GetResultIndex (ref targetNeedIndex, ref targetNeedValue, needValues)) {
						Debug.Log ("Error findind resultindex");
						return false;
				}

				targetNeed = needResources [targetNeedIndex];

				//TODO: add alternate behaviour here

				resourceTargets.Clear ();

				resourceTargets = LookForResource (targetNeed);
				resourceTargets.AddRange (awareness.GetResourceList (targetNeed));

				if (resourceTargets.Count == 0) {
						//Debug.Log ("Didn't find any targets for the resource");
						movement.trackingState = AIEnumeration.Tracking.Aimless;
						return false;
				}

				float[] gameobjectDistances = GetGameObjectDistanceList (resourceTargets).ToArray ();
				int targetResourceIndex = -1;
				float targetResourceValue = -1f;
				if (!AIProbabilitySolver.GetResultIndex (ref targetResourceIndex, ref targetResourceValue, gameobjectDistances, true)) {
						Debug.Log ("Error Getting resource target");
						return false;
				}
				targetResource = resourceTargets [targetResourceIndex].transform.parent.name;
				//Debug.Log ("Target Found: " + targetResource);
				currentTarget = resourceTargets [targetResourceIndex];
				SetNavToCurrentTarget ();

				return true;
		}
		
		private bool SetNavToCurrentTarget ()
		{
		
				return nav.SetDestination (currentTarget.transform.position + (Random.insideUnitSphere * currentTarget.GetComponent<Collider> ().bounds.size.magnitude * 0.25f));
		}


		/// <summary>
		/// Searches for resource Targets.
		/// </summary>
		/// <param name="resources">Resources.</param>
		/// <param name="redoSearch">If set to <c>true</c> redo search.</param>
		public void SearchForResource (AINeedManager _needManager, bool redoSearch)
		{		
				//Set time to wait based on AIs Tactics trait (Judging or Prospecting)
				float timeToWait = ((1f - parent.traitManager.GetNormalizedTrait (AIEnumeration.TraitType.Tactics)) * 0.5f + 0.5f) * (float)AIInitialVariables.waitTimerMax;
				parent.movement.StartWait (Mathf.FloorToInt( timeToWait ));

				List<string> resources = _needManager.neededResources;
				if (resourceTargets.Count == 0 || redoSearch) {
						resourceTargets.Clear ();

						resourceTargets = LookForResources (resources);
						if (resources.Count > 0)
								resourceTargets = this.GetComponent<AIAwareness> ().GetResourceList (resources);

						for (int i = resourceTargets.Count - 1; i >= 0; i--) {
								if (!SightCheckTarget (resourceTargets [i]))
										resourceTargets.RemoveAt (i);
						}

						if (resourceTargets.Count == 0) {
								movement.trackingState = AIEnumeration.Tracking.Aimless;
								return;
						}

						resourceTargets = SortListByDistance (resourceTargets);
						nav.SetDestination (resourceTargets [0].transform.position + (Random.insideUnitSphere * resourceTargets [0].GetComponent<Collider> ().bounds.size.magnitude * 0.25f));
						currentTarget = resourceTargets [0];

				} else {
						List<GameObject> tmp = LookForResources (resources);
						if (tmp.Count == 0)
								return;

						tmp = SortListByDistance (tmp);

						float destinationDistance = Vector3.Distance (this.transform.position, nav.destination) - 0.0001f;
						RaycastHit hit;
						for (int i = 0; i < tmp.Count; i++) {
								if (SightCheckTarget (tmp [i]) && tmp [i].transform.parent.gameObject != this.gameObject) {
										if (Physics.Raycast (this.transform.position, tmp [i].transform.position - this.transform.position, out hit, destinationDistance)) {

												if (tmp [i] != currentTarget) {
														if (nav.SetDestination (tmp [i].transform.position + (Random.insideUnitSphere * 0.1f))) {

																currentTarget = tmp [i];
																return;
														}
												}


										} else {
												return;
										}
								}
						}
				}
		}

		/// <summary>
		/// Looks for resources using Line of site
		/// </summary>
		/// <returns>List of resources that AI can see.</returns>
		/// <param name="names">Resource names</param>
		public List<GameObject> LookForResources (List<string> names)
		{
				List<GameObject> toReturn = new List<GameObject> ();

				for (int i = 0; i < names.Count; i++) {

						toReturn.AddRange (LookForResource (names [i]));
				}

				return toReturn;
		}
	
		/// <summary>
		/// Looks for particular resource using Line of site
		/// </summary>
		/// <returns>List of resources that AI can see.</returns>
		/// <param name="names">Resource name</param>
		public List<GameObject> LookForResource (string name)
		{
				List<GameObject> toReturn = new List<GameObject> ();

				GameObject[] tmp = GameObject.FindGameObjectsWithTag (name);
				foreach (GameObject res in tmp) {
						if (CheckLineOfSite (res))
								toReturn.Add (res);
				}

				return toReturn;
		}

		/// <summary>
		/// Clears the resourceTargets list.
		/// </summary>
		public void ClearTargetList ()
		{
				resourceTargets.Clear ();
		}
	
		/////////////////////////////////////////////////////////////////////////////////////////
		//	Utilities
		/////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Sorts the resourceTargets by navmesh distance.
		/// </summary>
		/// <returns>The list by distance.</returns>
		/// <param name="prevList">original resource list.</param>
		public List<GameObject> SortListByDistance (List<GameObject> prevList)
		{
				if (prevList.Count <= 1)
						return prevList;

				List<float> distances = new List<float> ();

				List<float> tempDistances = new List<float> ();
				List<GameObject> tempList = new List<GameObject> ();

				foreach (GameObject res in prevList) {
						distances.Add (TargetDistance (res.transform.position));
				}

				tempDistances.Add (distances [0]);
				tempList.Add (prevList [0]);

				//foreach (float dist in distances) {
				for (int j = 1; j < distances.Count; j++) {
						int i = 0;
						while (i < tempDistances.Count-1 && distances[j] > tempDistances[i])
								i++;

						tempDistances.Insert (i, distances [j]);
						tempList.Insert (i, prevList [j]);
				}

				return tempList;
		}

		/// <summary>
		/// Gets a list of distances to the gameobjects.
		/// </summary>
		/// <returns>The list of distances.</returns>
		/// <param name="gameobjects">Gameobjects to test.</param>
		public List<float> GetGameObjectDistanceList (List<GameObject> gameobjects)
		{
				List<float> toReturn = new List<float> ();

				for (int i = 0; i < gameobjects.Count; i++) {
						toReturn.Add (TargetDistance (gameobjects [i].transform.position));
				}

				return toReturn;
		}
				
		/// <summary>
		/// Finds the distance using NavMesh
		/// </summary>
		/// <returns>The distance.</returns>
		/// <param name="target">Target.</param>
		public float TargetDistance (Vector3 target)
		{
				NavMeshPath p = new NavMeshPath ();
		
				if (nav.CalculatePath (target, p)) {
						return CalculatePathDistance (p);
				}
				return -1;
		}

		/// <summary>
		/// Steps through calculated path to find total length
		/// TODO: more efficient approach
		/// </summary>
		/// <returns>The path distance.</returns>
		/// <param name="_path">Path to test</param>
		public float CalculatePathDistance (NavMeshPath _path)
		{
		
				if (_path.corners.Length < 2)
						return -1;
		
				Vector3 previousCorner = _path.corners [0];
				float pathLength = 0f;
				int i = 1;
		
				while (i < _path.corners.Length) {
						Vector3 currentCorner = _path.corners [i];
						pathLength += Vector3.Distance (previousCorner, currentCorner);
						previousCorner = currentCorner;
						i++;
				}
		
				return pathLength;
		}

		/// <summary>
		/// Checks the line of site.
		/// TODO: rework this so seeing any part of the resource will prove true
		/// </summary>
		/// <returns><c>true</c>, if line of site was checked, <c>false</c> otherwise.</returns>
		/// <param name="resource">Gameobject to test</param>
		public bool CheckLineOfSite (GameObject _target)
		{
				return CheckLineOfSite (_target, -1);
		}

		public bool CheckLineOfSite (GameObject _target, float _maxDistance)
		{
				bool hitTest;
				bool sightContact = false;
				RaycastHit hit;
				Ray r = new Ray (this.transform.position, _target.transform.position - this.transform.position);
			
				float offset = minSweepTest;
				float startDirection = r.direction.x;
			
			
				while (offset <= maxSweepTest) {
						r.direction = new Vector3 (startDirection + offset, r.direction.y, r.direction.z);
						//For Testing
						//Debug.DrawRay (r.origin, r.direction);
				
				
						if (_maxDistance == -1)
								hitTest = Physics.Raycast (r, out hit);
						else
								hitTest = Physics.Raycast (r, out hit, _maxDistance);
				
						if (hitTest) {
								if (hit.collider.gameObject.name == _target.name) {
										return true;
								}
						}
						offset += sweepTestResolution;
				}
				return sightContact;
		}
		/*
		public bool CheckLineOfSite (GameObject resource)
		{
//				Vector3 minPositionCheck = resource.transform.position.x 
		Ray r = new Ray (this.transform.position, resource.transform.position - this.transform.position);
		float resolution = 1f/50f;
		float startY = r.direction.y - 0.5f;
		float endY = r.direction.y + 0.5f;
		float currentY = startY;
		RaycastHit hit;

		while (currentY <= endY) {

			r = new Ray(this.transform.position,new Vector3(r.direction.x, currentY, r.direction.z));
			//for debug
			if(this.gameObject.name == "AI 0")
				Debug.DrawRay(r.origin,r.direction);

			if(Physics.Raycast(r, out hit, 3))
			{

				if(hit.collider.gameObject == resource)
					return true;
			}

			currentY += resolution;

				}
		return false;

				if (Physics.Raycast (this.transform.position, resource.transform.position - this.transform.position, out hit, 3)) {
						if (hit.collider.gameObject == resource)
								return true;
				}
				return false;
		}
		*/
}
