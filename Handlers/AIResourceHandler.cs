using UnityEngine;
using System.Collections;


/**
 * Handles AI Character interaction with Resources in environment
 **/
public class AIResourceHandler : MonoBehaviour {
	public AIAwareness parentAwareness;
	// Use this for initialization
	void Start () {
	}

	void OnTriggerEnter(Collider other){
		
		if (!parentAwareness)
			parentAwareness = this.GetComponentInParent<AIAwareness> ();

				if (other.gameObject.layer == LayerMask.NameToLayer ("Resources"))
						parentAwareness.AddResource (other);
				
				this.transform.GetComponentInParent<AIPlayer> ().StartRefill (other.gameObject);
		}

	void OnTriggerExit(Collider other){
		
		this.transform.GetComponentInParent<AIPlayer> ().EndRefill (other.gameObject);
		}
}
