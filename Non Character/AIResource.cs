/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// AI resource.
/// 
/// Manages resource areas placed in scene
/// Tracks its own availability, and ai interaction
/// </summary>
public class AIResource : MonoBehaviour
{
		public AIEnumeration.ResourceType resourceType;
		public float resourceAvailability = 1f;
		public float resourceConsumption = 0.01f;
		public float resourceRegrowth = 0.01f;
		public int resourceSlots = 4;
		public int playerAccessCount = 0;
		
		//For changing resource to give visual feedback
		Material mat;
		public Color initialColor;

		void Start ()
		{
				mat = this.GetComponent<MeshRenderer> ().material;
				initialColor = mat.color;
		}
	
		/// <summary>
		/// Updates the resource.
		/// </summary>
		void Update ()
		{
				int playcount = playerAccessCount;
				if (playcount > resourceSlots)
						playcount = resourceSlots;
		
				if (resourceAvailability > 0)
						resourceAvailability -= playcount * resourceConsumption;
				if (resourceAvailability < 1 && playcount == 0)
						resourceAvailability += resourceRegrowth;
		
				if (resourceAvailability < 0)
						resourceAvailability = 0;
				if (resourceAvailability > 1)
						resourceAvailability = 1;
		
				mat.color = initialColor * resourceAvailability;
		}

		/// <summary>
		/// Checks to see if the Resource is available.
		/// </summary>
		/// <returns><c>true</c>, if available, <c>false</c> otherwise.</returns>
		public bool ResourceAvailable ()
		{
				if (resourceAvailability > 0f && resourceSlots - playerAccessCount > 0)
						return true;

				return false;
		}

		/// <summary>
		/// Raises the playerAccessCount if AI entered
		/// </summary>
		/// <param name="other">Collider</param>
		void OnTriggerEnter (Collider other)
		{
				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						playerAccessCount++;
				}
		}
	
		/// <summary>
		/// Decreases the playerAccessCount if AI exits
		/// </summary>
		/// <param name="other">Collider</param>
		void OnTriggerExit (Collider other)
		{
				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						playerAccessCount--;
				}
		}
}
