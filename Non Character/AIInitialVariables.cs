/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// AI initial variables.
/// </summary>
public static class AIInitialVariables
{
		//Needs
		public static float foodLossRate = 0.0001f;
		public static float foodRecoveryRate = 0.01f;
		public static float food = 0f;
		public static float restLossRate = 0.00015f;
		public static float restRecoveryRate = 0.01f;
		public static float rest = 0f;
		public static float entertainmentLossRate = 0.00025f;
		public static float entertainmentRecoveryRate = 0.01f;
		public static float entertainment = 0f;
		public static float companionshipLossRate = 0.00025f;
		public static float companionshipRecoveryRate = 0.01f;
		public static float companionship = 0f;


		//Behaviours
		public static float wanderIncrease = 0.01f;
}


/// <summary>
/// Global enumerations for AI.
/// </summary>
public class AIEnumeration
{
	
	public const int TrackingCount = 3;
	public enum Tracking
	{
		Aimless,
		Searching,
		Refilling}
	;
	
	public const int ResourceTypeCount = 4;
	public enum ResourceType
	{
		Rest,
		Food,
		Entertainment,
		Companionship}
	;
	
	public const int CardinalDirectionCount = 6;
	public enum CardinalDirection
	{
		North,
		East,
		South,
		West}
	;
	
	
	public const int TraitTypeCount = 6;
	public enum TraitType
	{
		Age,
		Mind,
		Energy,
		Nature,
		Tactics,
		Identity
	}
}
