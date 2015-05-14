using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AIResourceTarget : MonoBehaviour {
	NavMeshAgent nav;
	AIMovement movement;
	public GameObject currentTarget;

	public bool isWaiting = false;
	int wait;
	public int waitTime = 100;

	public List<GameObject>resourceTargets = new List<GameObject>();


	// Use this for initialization
	void Start () {
		nav = this.GetComponent<NavMeshAgent> ();
		movement = this.GetComponent<AIMovement> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isWaiting) {
			nav.Stop();
						wait++;
						if (wait > waitTime) {
				wait = 0;
				nav.Resume();
								isWaiting = false;
						}
				}

		if (movement.trackingState == AIEnumeration.Tracking.Searching) {
						//if there are no resources move to the next resource available in the list
						if (!SightCheckTarget ()) {
								if (resourceTargets.Count <= 1) {
										movement.trackingState = AIEnumeration.Tracking.Aimless;
										return;
								}
								RetargetResource ();
						}

						//if something in the path finding needs retargetting
						if (nav.isPathStale) {
								if (resourceTargets.Count <= 1) {
										movement.trackingState = AIEnumeration.Tracking.Aimless;
										return;
								}
								RetargetResource ();
						}
				}

	}

	bool SightCheckTarget()
	{
		RaycastHit hit;
		if(Physics.Raycast(this.transform.position,currentTarget.transform.position - this.transform.position,out hit)){
			AIResource res = currentTarget.GetComponent<AIResource>();

			//Return false if we can see that there no resources are available in the target
			if(!res.ResourceAvailable()){
				isWaiting = true;
				return false;
			}
			else
				return true;
		}
		//Return true if can't see the resource
		return true;
	}

	bool SightCheckTarget(GameObject target)
	{
		RaycastHit hit;
		bool canSee = this.GetComponent<AIPlayer>().currentResources.Contains(target);
		if (!canSee)
						canSee = Physics.Raycast (this.transform.position, target.transform.position - this.transform.position, out hit);

		if(canSee){
			AIResource res = target.GetComponent<AIResource>();
			
			//Return false if we can see that there no resources are available in the target
			if(!res.ResourceAvailable()){
				isWaiting = true;
				return false;
			}
			else
				return true;
		}
		//Return true if can't see the resource
		return true;
	}

	void RetargetResource(){
			if (nav.SetDestination (resourceTargets [0].transform.position)) {
						currentTarget = resourceTargets [0];
						return;
				} else {
						ShiftResourceTargets ();
				}
				
	}

	void ShiftResourceTargets()
	{
		GameObject temp = resourceTargets [0];
		resourceTargets.RemoveAt (0);
		resourceTargets.Add (temp);
		RetargetResource ();
	}

	
	void TargetResource(NavMeshPath target) {
		nav.SetPath (target);
		return;
	}
	
	public void SearchForResource(List<string> resources, bool redoSearch) {
		if (isWaiting)
						return;

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
						nav.SetDestination (resourceTargets [0].transform.position + (Random.insideUnitSphere * resourceTargets[0].GetComponent<Collider>().bounds.size.magnitude * 0.25f));
						currentTarget = resourceTargets [0];

				} else  {
						List<GameObject> tmp = LookForResources (resources);
						if (tmp.Count == 0)
								return;

						tmp = SortListByDistance (tmp);

						RaycastHit hit;
						float destinationDistance = Vector3.Distance (this.transform.position, nav.destination) - 0.0001f;

						for (int i = 0; i < tmp.Count; i++) {
								if (SightCheckTarget (tmp [i]) && tmp [i].transform.parent.gameObject != this.gameObject) {
										if (Physics.Raycast (this.transform.position, tmp [i].transform.position - this.transform.position, out hit, destinationDistance)) {

						if(tmp[i] != currentTarget){
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

	public List<GameObject> LookForResources(List<string> names)
	{
		List<GameObject> toReturn = new List<GameObject>();

			for (int i = 0; i < names.Count; i++) {
						GameObject[] tmp = GameObject.FindGameObjectsWithTag (names [i]);

						foreach (GameObject ret in tmp) {
								if (CheckLineOfSite (ret))
										toReturn.Add (ret);
						}
				}

		return toReturn;
	}

	public void ClearTargetList()
	{
		resourceTargets.Clear();
	}

	public List<GameObject> SortListByDistance(List<GameObject> prevList){
		if (prevList.Count <= 1)
			return prevList;

		List<float> distances = new List<float> ();

		List<float> tempDistances = new List<float> ();
		List<GameObject> tempList = new List<GameObject>();

		foreach(GameObject res in prevList){
			distances.Add(TargetDistance(res.transform.position));
		}

		tempDistances.Add (distances [0]);
		tempList.Add (prevList [0]);

		//foreach (float dist in distances) {
		for(int j = 1; j < distances.Count; j++){
			int i = 0;
			while(i < tempDistances.Count-1 && distances[j] > tempDistances[i])
				i++;

			tempDistances.Insert(i,distances[j]);
			tempList.Insert(i,prevList[j]);
		}

		return tempList;
	}
				
	public float TargetDistance(Vector3 target){
		NavMeshPath p = new NavMeshPath ();
		
		if(nav.CalculatePath(target, p)) {
			return CalculatePathDistance(p);
		}
		return -1;
	}

	public float TargetDistance(Vector3 target, out NavMeshPath _path){
		NavMeshPath p = new NavMeshPath ();
		
		if(nav.CalculatePath(target, p)) {
			_path = p;
			return CalculatePathDistance(p);
		}
		_path = null;
		return -1;
	}
	
	public float CalculatePathDistance(NavMeshPath _path){
		
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
	
	public bool LineOfSiteTest(GameObject target, bool priority){
		return this.GetComponent<AIAwareness> ().CheckResourceList (target,priority);
	}

	public bool CheckLineOfSite(GameObject resource){
		RaycastHit hit;
		if (Physics.Raycast (this.transform.position, resource.transform.position - this.transform.position, out hit, 3)) {
				if (hit.collider.gameObject == resource)
						return true;
		}
		return false;
	}
}
