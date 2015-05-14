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
 **/

using UnityEngine;
using System.Collections;

public class Trait {
	public enum TraitType
	{
		Age,
		Mind,
		Energy,
		Nature,
		Tactics,
		Identity
	}
	public const int TypeCount = 6;
	public TraitType type;

/// <summary>
	/// Age
	/// Aggression
	/// Extraversion
	/// Focus
	/// Happiness
	/// 
/// </summary>
	// Use this for initialization
	public Trait(TraitType _type) {
		type = _type;
	}
	
	// Update is called once per frame
	public void Update () {
	
	}
}
