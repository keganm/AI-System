/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITraitManager : MonoBehaviour
{
		public List<AITrait> traits = new List<AITrait> ();
		public List<float> testVal = new List<float> ();
		
		/// <summary>
		/// Create random valued traits
		/// </summary>
		void Start ()
		{
				for (int i = 0; i < AIEnumeration.TraitTypeCount; i++) {
						float val = Random.value * 2f - 1f;
						traits.Add (new AITrait ((AIEnumeration.TraitType)i, val));
				}

				foreach (AITrait t in traits) {
						testVal.Add (t.value);
				}
		}
}
