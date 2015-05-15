/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;
/// <summary>
/// Handles AI Character interaction with other AI in environment
/// Works in a child Collision object
/// </summary>
public class AIResourceHandler : MonoBehaviour
{
		public AIPlayer parent;

		public void Init (AIPlayer _parent)
		{
				parent = _parent;
		}

		/// <summary>
		/// If collider is a resource send to AIAwareness
		/// Call Player to start refill
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter (Collider other)
		{
				if (other.gameObject.layer == LayerMask.NameToLayer ("Resources"))
						parent.awareness.AddResource (other);
				
				parent.StartRefill (other.gameObject);
		}

		/// <summary>
		/// Let player it's no longer refilling
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerExit (Collider other)
		{
				this.transform.GetComponentInParent<AIPlayer> ().EndRefill (other.gameObject);
		}
}
