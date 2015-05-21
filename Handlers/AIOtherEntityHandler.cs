/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

public class AIOtherEntityHandler : MonoBehaviour
{
		public AIEntity parent;
		//Count for debugging
		public int count;
	
		public void Init (AIEntity _parent)
		{
				parent = _parent;
		}

		/// <summary>
		/// Add entity to awareness
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter (Collider other)
		{
				if (other.gameObject.layer != LayerMask.NameToLayer ("AI"))
						return;
				AIEntity testEntity = other.GetComponentInParent<AIEntity> ();
				if (testEntity != null) {
						count++;
						parent.awareness.AddEntity (other);
						parent.AddNewEntity (other.gameObject);
				}
				
		}

	
		/// <summary>
		/// Remove entity to awareness
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerExit (Collider other)
		{
				if (other.gameObject.layer != LayerMask.NameToLayer ("AI"))
						return;
				AIEntity testEntity = other.GetComponentInParent<AIEntity> ();
				if (testEntity != null) {
						count--;
						parent.awareness.RemoveEntity (other);
						parent.RemoveEntity (other.gameObject);
				}
		}
}
