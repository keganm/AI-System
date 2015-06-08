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
		public AIEntity parent;

		public void Init (AIEntity _parent)
		{
				parent = _parent;
		}

		/// <summary>
		/// If collider is a resource send to AIAwareness
		/// Call Entity to start refill
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter (Collider other)
		{
				if (other.gameObject.layer == LayerMask.NameToLayer ("Resources"))
						parent.awareness.AddResource (other);
				
				parent.StartRefill (other.gameObject);
		}

		/// <summary>
		/// Let entity it's no longer refilling
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerExit (Collider other)
		{
				this.transform.GetComponentInParent<AIEntity> ().EndRefill (other.gameObject);
		}
}
