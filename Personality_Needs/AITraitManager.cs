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
		public float negativeThreshold = 1.35f;
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
						traits [_trait.Key].UpdateTrait ();
						testVal [i] = traits [_trait.Key].value;
						i++;
				}

		}

		/// <summary>
		/// Tests the reaction to another AI .
		/// </summary>
		/// <param name="otherAI">Other AI entity.</param>
		public AIEnumeration.ReactionType TestReaction (AITraitManager otherAI)
		{
				AIEnumeration.ReactionType reaction = AIEnumeration.ReactionType.Neutral;
				
				//Reaction probability determines reaction to other AI Entity when requested
				float reactionProbability = 0f;
		
				foreach (KeyValuePair<AIEnumeration.TraitType, AITrait> _trait in traits) {

						if (_trait.Key != AIEnumeration.TraitType.Age)
								reactionProbability += (1 - (Mathf.Abs (otherAI.traits [_trait.Key].value - this.traits [_trait.Key].value)));
				}

				reactionProbability *= (1f / (float)traits.Count); //TODO:throw this to the top as a static if the counts aren't going to change

				if (negativeThreshold - 1f >= reactionProbability)
						reaction = AIEnumeration.ReactionType.Negative;

				if (reactionProbability > positiveThreshold)
						reaction = AIEnumeration.ReactionType.Positive;

				return reaction;
		}

		/// <summary>
		/// Merges the personalities.
		/// Affectors (Age, older reduces adoption; Identity, lower reduces adoption)
		/// MergeType; negative -> personality seperation
		/// </summary>
		/// <param name="otherAI">Other AI entity</param>
		public AIEnumeration.ReactionType MergePersonalities (AITraitManager otherAI)
		{
				if (traits.Count < AIEnumeration.TraitTypeCount)
						return AIEnumeration.ReactionType.Neutral;

		
				AIEnumeration.ReactionType mergeType = TestReaction (otherAI);
				float scale = GetPersonalityWeight (otherAI, mergeType);
				foreach (KeyValuePair<AIEnumeration.TraitType, AITrait> _trait in traits) {
						if (_trait.Key != AIEnumeration.TraitType.Age) {
								traits [_trait.Key].SetValue (traits [_trait.Key].value + ((otherAI.traits [_trait.Key].value - traits [_trait.Key].value) * scale));

						}
				}
				return mergeType;
		}

		/// <summary>
		/// Gets the personality weight, determines how much influence other AI have.
		/// </summary>
		/// <returns>The influence</returns>
		/// <param name="otherAI">Other AI entity</param>
		public float GetPersonalityWeight (AITraitManager otherAI, AIEnumeration.ReactionType mergeType)
		{
				float ageScale = 0.001f;
				float identityScale = 0.01f;
		
				float ageFactor = traits [AIEnumeration.TraitType.Age].value * ageScale;
				float identityFactor = (traits [AIEnumeration.TraitType.Identity].value * 0.5f + 0.5f) * identityScale;
		
				float mergeScale = 1f;
				if (mergeType == AIEnumeration.ReactionType.Negative)
						mergeScale *= -1f;
				else if (mergeType == AIEnumeration.ReactionType.Neutral)
						mergeScale *= 0.5f;
		
		
				float finalScale = (ageFactor + identityFactor) * mergeScale;
				return finalScale;
		}
		
		public float GetStopAndLookAroundProbability ()
		{
				float probability = 0f;
				probability += traits [AIEnumeration.TraitType.Age].value * 0.2f;
				probability += traits [AIEnumeration.TraitType.Energy].value * 0.3f;
				probability += traits [AIEnumeration.TraitType.Nature].value * 0.1f;
				probability += traits [AIEnumeration.TraitType.Tactics].value * 0.3f;
				probability += traits [AIEnumeration.TraitType.Identity].value * 0.1f;
				
				probability *= 0.0001f;
				probability += 0.9999f;
				return probability;
		}

		/// <summary>
		/// Gets the normalized trait (should be -1 to 1).
		/// </summary>
		/// <returns>The normalized trait.</returns>
		/// <param name="_trait">Trait to be returned.</param>
		public float GetNormalizedTrait(AIEnumeration.TraitType _trait)
		{
				return traits [_trait].value * 0.5f + 0.5f;
		}
}
