using UnityEngine;
using System.Collections;

public class AIOtherPlayerHandler : MonoBehaviour {
	public AIAwareness parentAwareness;
	public int count;
	// Use this for initialization
	void Start () {
	}

	void OnTriggerEnter(Collider other){
		
		if (!parentAwareness)
			parentAwareness = this.GetComponentInParent<AIAwareness> ();

				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						count++;
						parentAwareness.AddPlayer (other);
				}
				
		}

	void OnTriggerExit(Collider other){
				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						count--;
						parentAwareness.RemovePlayer (other);
				}
		}
}
