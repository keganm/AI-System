/** Potential AI modification approaches
 * 
 * Establish 'home base' (a position that is most efficient ie resources...)
 * following establishment of 'home' begin a venturing outward behaviour (similar to wander now)
 * 
 * Integrate possession of resources in some way ( Claiming, aggression, health & violence?)
 * 
 * Modify companionship behaviour to modify in both directions.
 * Dependent on Serialized personality traits.
 ******  Mind – Introverted or Extraverted
 ******  Energy – Intuitive or Observant
 ******  Nature – Thinking or Feeling
 ******  Tactics – Judging or Prospecting
 ******  Identity – Assertive or Turbulent
 * 
 * 
 * Finesse test environment
 * 
 * Create a POV cam (pip?) for selected AI?
 **/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIPlayer : MonoBehaviour {
	AIController controller;

	//Classes will be added automatically
	#region SubClassComponents
	AIMovement movement;
	AIAwareness awareness;
	AINeedManager needManager;
	#endregion SubClassComponents


	public bool needsResearch = false;
	public List<GameObject> currentResources = new List<GameObject>();

	// Use this for initialization
	void Start () {
		if (!this.transform.gameObject.GetComponent<AINeedManager> ())
						needManager = this.transform.gameObject.AddComponent<AINeedManager> ();
				else
						needManager = this.transform.GetComponent<AINeedManager> ();

		if (!this.transform.gameObject.GetComponent<AIMovement> ())
						movement = this.transform.gameObject.AddComponent<AIMovement> ();
				else
						movement = this.transform.gameObject.GetComponent<AIMovement> ();

		if (!this.transform.gameObject.GetComponent<AIAwareness> ())
						awareness = this.transform.gameObject.AddComponent<AIAwareness> ();
				else
						awareness = this.transform.gameObject.GetComponent<AIAwareness> ();
		
		ResetInitialVariables ();
	}
	
	void ResetInitialVariables(){
		currentResources.Clear ();
		needsResearch = false;
		movement.trackingState = AIEnumeration.Tracking.Aimless;
	}
	
	// Update is called once per frame
	void Update () {
		CheckCurrentResource ();
		UpdateNeeds ();
	}
	
	void UpdateNeeds()
	{
		needManager.Update ();	

		if (needManager.needed.Count > 0) {
						movement.SearchForResource (needManager.needed, needsResearch);
		}
	}
	
	public void StartRefill(GameObject other){
		if (other.layer == LayerMask.NameToLayer ("Resources"))
			if(!currentResources.Contains(other))
				currentResources.Add(other);
	}
	
	public void EndRefill(GameObject other){
		if (currentResources.Count == 0)
						return;

		if (currentResources.Contains (other)) {
						foreach (AiNeed need in needManager.needs) {
								if (other.tag == need.resource) {
										need.refilling = false;
								}
						}
						currentResources.Remove (other);
				}



	}

	public void CheckCurrentResource(){
				if (currentResources.Count == 0) {
						foreach (AiNeed need in needManager.needs)
								need.refilling = false;
						return;
				}

				foreach (GameObject currentResource in currentResources) {
						AIResource res = currentResource.GetComponent<AIResource> ();
						foreach (AiNeed need in needManager.needs) {
								if (need.resource == currentResource.tag) {
					needsResearch = false;

										if (res.ResourceAvailable ()) {
												need.refilling = true;


												if (!need.inNeed) {
														needsResearch = true;
												}

										} else {
												need.refilling = false;
												needsResearch = true;
										}
								}
						}
				}
		}
	
}