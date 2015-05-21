/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Global enumerations for AI.
/// </summary>
public static class AIEnumeration
{
	
	public const int TrackingCount = 3;
	public enum Tracking
	{
		Aimless,
		Searching,
		Refilling
	};
	
	public const int ResourceTypeCount = 4;
	public enum ResourceType
	{
		Rest,
		Food,
		Entertainment,
		Companionship
	};
	
	public const int CardinalDirectionCount = 6;
	public enum CardinalDirection
	{
		North,
		East,
		South,
		West
	};
	
	
	public const int TraitTypeCount = 6;
	public enum TraitType
	{
		Age,
		Mind,
		Energy,
		Nature,
		Tactics,
		Identity
	};
	
	public const int ReactionTypes = 3;
	public enum Reaction
	{
		Negative,
		Neutral,
		Positive
	};
}


/// <summary>
/// AI initial variables.
/// </summary>
public static class AIInitialVariables
{
		//Needs
		public static Dictionary<AIEnumeration.ResourceType, AINeed> needDictionary = new Dictionary<AIEnumeration.ResourceType, AINeed> (){
				{AIEnumeration.ResourceType.Food, new AINeed("Food", 0f, 0.0001f, 0.01f)},
				{AIEnumeration.ResourceType.Rest, new AINeed("Rest", 0f, 0.00015f, 0.01f)},
				{AIEnumeration.ResourceType.Entertainment, new AINeed("Entertainment", 0f, 0.00025f, 0.01f)},
				{AIEnumeration.ResourceType.Companionship, new AINeed("Companionship", 0f, 0.00025f, 0.01f)}
		};

		//Traits (-1f will cause random value between -1 to 1)
		public static Dictionary<AIEnumeration.TraitType, AITrait> traitDictionary = new Dictionary<AIEnumeration.TraitType, AITrait> (){
				{AIEnumeration.TraitType.Age, new AITrait(AIEnumeration.TraitType.Age, "Age", 0, 0.00001f)},
				{AIEnumeration.TraitType.Mind, new AITrait(AIEnumeration.TraitType.Mind, "Mind", -1f, 0f)},
				{AIEnumeration.TraitType.Energy, new AITrait(AIEnumeration.TraitType.Energy, "Energy", -1f, 0f)},
				{AIEnumeration.TraitType.Nature, new AITrait(AIEnumeration.TraitType.Nature, "Nature", -1f, 0f)},
				{AIEnumeration.TraitType.Tactics, new AITrait(AIEnumeration.TraitType.Tactics, "Tactics", -1f, 0f)},
				{AIEnumeration.TraitType.Identity, new AITrait(AIEnumeration.TraitType.Identity, "Identity", -1f, 0f)},
		};


		//Behaviours
		public static float wanderIncrease = 0.01f;

		//Sight test variables
	
		public static float sweepTestResolution = 0.2f;
		public static float minSweepTest = -0.5f;
		public static float maxSweepTest = 0.5f;
}
