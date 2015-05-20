/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITraitManager : MonoBehaviour
{
		public Dictionary<AIEnumeration.TraitType, AITrait> traits = new Dictionary<AIEnumeration.TraitType, AITrait> ();
		//public List<AITrait> traits = new List<AITrait> ();
		public List<float> testVal = new List<float> ();
		public float negativeThreshold = 0.5f;
		public float positiveThreshold = 0.5f;
		
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
						//traits.Add (t);
						traits [_trait.Key] = t;
				}

				//For testing purposes
				foreach (KeyValuePair<AIEnumeration.TraitType, AITrait> _trait in traits) {
						testVal.Add (_trait.Value.value);
				}
		}

		void Update ()
		{
				int i = 0;
				foreach (KeyValuePair<AIEnumeration.TraitType, AITrait> _trait in traits) {
						traits[_trait.Key].UpdateTrait ();
						testVal [i] = traits[_trait.Key].value;
						i++;
				}

		}

		/// <summary>
		/// Tests the reaction to another AI .
		/// </summary>
		/// <param name="otherAI">Other AI entity.</param>
		public AIEnumeration.Reaction TestReaction (AITraitManager otherAI)
		{
				AIEnumeration.Reaction reaction = AIEnumeration.Reaction.Neutral;
				
				//Reaction probability determines reaction to other AI Entity when requested
				float reactionProbability = 0f;
		
				foreach (KeyValuePair<AIEnumeration.TraitType, AITrait> _trait in traits) {

						if (_trait.Key != AIEnumeration.TraitType.Age)
								reactionProbability += (1 - (Mathf.Abs (otherAI.traits [_trait.Key].value - this.traits [_trait.Key].value)));
				}

				reactionProbability *= (1f / (float)traits.Count); //TODO:throw this to the top as a static if the counts aren't going to change

				if (negativeThreshold - 1f >= reactionProbability)
						reaction = AIEnumeration.Reaction.Negative;

				if (reactionProbability > positiveThreshold)
						reaction = AIEnumeration.Reaction.Positive;

				return reaction;
		}

		/// <summary>
		/// Merges the personalities.
		/// Affectors (Age, older reduces adoption; Identity, lower reduces adoption)
		/// MergeType; negative -> personality seperation
		/// </summary>
		/// <param name="otherAI">Other AI entity</param>
		public void MergePersonalities (AITraitManager otherAI)
		{
				if (traits.Count < AIEnumeration.TraitTypeCount)
						return;

				
				float scale = GetPersonalityWeight (otherAI);
				foreach (KeyValuePair<AIEnumeration.TraitType, AITrait> _trait in traits) {
						if (_trait.Key != AIEnumeration.TraitType.Age) {
				traits[_trait.Key].SetValue (traits[_trait.Key].value + ((otherAI.traits [_trait.Key].value - traits[_trait.Key].value) * scale));

						}
				}
		}

	/// <summary>
	/// Gets the personality weight, determines how much influence other AI have.
	/// </summary>
	/// <returns>The influence</returns>
	/// <param name="otherAI">Other AI entity</param>
	public float GetPersonalityWeight(AITraitManager otherAI)
	{
		AIEnumeration.Reaction mergeType = TestReaction (otherAI);
		float ageScale = 0.001f;
		float identityScale = 0.01f;
		
		float ageFactor = traits [AIEnumeration.TraitType.Age].value * ageScale;
		float identityFactor = (traits [AIEnumeration.TraitType.Identity].value * 0.5f + 0.5f) * identityScale;
		
		float mergeScale = 1f;
		if (mergeType == AIEnumeration.Reaction.Negative)
			mergeScale *= -1f;
		else if (mergeType == AIEnumeration.Reaction.Neutral)
			mergeScale *= 0.5f;
		
		
		float finalScale = (ageFactor + identityFactor) * mergeScale;
		return finalScale;
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