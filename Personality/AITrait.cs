/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// AI trait container.
/// </summary>
public class AITrait
{
		public AIEnumeration.TraitType type;
		public string name;
		public float value; //-1 to 1
		public float increase;

		public AITrait ()
		{
		}

		public AITrait (AIEnumeration.TraitType _type, string _name, float _val, float _inc)
		{
				type = _type;
				name = _name;
				value = _val;
				increase = _inc;
		}

		public void CopyTrait (AITrait original)
		{
				type = original.type;
				name = original.name;
				value = original.value;
				increase = original.increase;
		}

		public void SetValue (float _val)
		{
				value = _val;
				if (value < -1) {
						value = -1;
						return;
				}
				if (value > 1) {
						value = 1;
						return;
				}
		}

		/// <summary>
		/// Update trait.
		/// </summary>
		public void UpdateTrait ()
		{
				SetValue (value + increase);
		}
}
