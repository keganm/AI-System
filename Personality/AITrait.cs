/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// AI trait container.
/// </summary>
public class AITrait {
	public AIEnumeration.TraitType type;
	public string name;
	public float value; //-1 to 1
	public float increase;

/// <summary>
/// </summary>
	// Use this for initialization
	public AITrait()
	{
	}

	public AITrait(AIEnumeration.TraitType _type, string _name, float _val, float _inc) {
		type = _type;
		name = _name;
		value = _val;
		increase = _inc;
	}

	public void CopyTrait(AITrait original)
	{
		type = original.type;
		name = original.name;
		value = original.value;
		increase = original.increase;
	}
	
	// Update is called once per frame
	public void Update () {
	
	}
}
