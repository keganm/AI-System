/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

public class AIOtherPlayerHandler : MonoBehaviour
{
		public AIPlayer parent;
		//Count for debugging
		public int count;
	
		public void Init (AIPlayer _parent)
		{
				parent = _parent;
		}

		/// <summary>
		/// Add player to awareness
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter (Collider other)
		{
				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						count++;
						parent.awareness.AddPlayer (other);
				}
				
		}

	
		/// <summary>
		/// Remove player to awareness
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerExit (Collider other)
		{
				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						count--;
						parent.awareness.RemovePlayer (other);
				}
		}
}
