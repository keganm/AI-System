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
	public float value; //-1 to 1

/// <summary>
/// </summary>
	// Use this for initialization
	public AITrait(AIEnumeration.TraitType _type, float _val) {
		type = _type;
		value = _val;

	}
	
	// Update is called once per frame
	public void Update () {
	
	}
}

/**
 * 
 * Modify companionship behaviour to modify in both directions.
 * Dependent on Serialized personality traits.
 ******  Mind – Introverted or Extraverted
 ******  Energy – Intuitive or Observant
 ******  Nature – Thinking or Feeling
 ******  Tactics – Judging or Prospecting
 ******  Identity – Assertive or Turbulent
 *
 *
 *Maybe these instead
 *
	/// Age
	/// Aggression
	/// Extraversion
	/// Focus
	/// Happiness
	/// 
 **/