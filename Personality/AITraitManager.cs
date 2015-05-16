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

				foreach (KeyValuePair<AIEnumeration.TraitType, AITrait> _trait in AIInitialVariables.traitDictionary) {
						AITrait t = new AITrait ();
						t.CopyTrait (_trait.Value);
						if (t.value == -1)
								t.value = Random.value * 2f - 1f;
						traits.Add (t);
				}

				//For testing purposes
				foreach (AITrait t in traits) {
						testVal.Add (t.value);
				}
		}

		/// <summary>
		/// Tests the reaction to another AI .
		/// </summary>
		/// <param name="otherAI">Other A.</param>
		public AIEnumeration.Reaction TestReaction (AITraitManager otherAI)
		{
				AIEnumeration.Reaction reaction = AIEnumeration.Reaction.Neutral;

				return reaction;
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