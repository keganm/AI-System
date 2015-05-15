/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI need manager.
/// Manage list of basic needs per AI and tracks resources that are needed
/// </summary>
public class AINeedManager : MonoBehaviour  {

	public List<AiNeed> needList = new List<AiNeed>();
	public List<string> neededResources = new List<string> ();

	public AINeedManager()
	{
				Reset ();
	}

	/// <summary>
	/// Clear lists and recreates from AIInitialVariables
	/// TODO: Possibly automate needList generation
	/// </summary>
	public void Reset()
	{	
		neededResources.Clear ();
		needList.Clear ();

		//Build list of needs
		needList.Add (new AiNeed ("Entertainment", AIInitialVariables.entertainment, AIInitialVariables.entertainmentLossRate, AIInitialVariables.entertainmentRecoveryRate));
		needList.Add (new AiNeed ("Food", AIInitialVariables.food, AIInitialVariables.foodLossRate, AIInitialVariables.foodRecoveryRate));
		needList.Add (new AiNeed ("Rest", AIInitialVariables.rest, AIInitialVariables.restLossRate, AIInitialVariables.restRecoveryRate));
		needList.Add (new AiNeed ("Companionship", AIInitialVariables.companionship, AIInitialVariables.companionshipLossRate, AIInitialVariables.companionshipRecoveryRate));
	}

	/// <summary>
	/// Update needs in list, and add to needResources if inNeed.
	/// </summary>
	public void Update () {
		foreach(AiNeed need in needList){
			need.UpdateNeed();

			//Add need to needed list or remove it
			if(need.inNeed){
				AddNewNeededResource(need.resource);
			}else{
				RemoveNeededResource(need.resource);
			}
		}
	}

	/// <summary>
	/// Adds the new needed resource if not already needed
	/// </summary>
	/// <param name="_needed">resourcee</param>
	public void AddNewNeededResource(string _needed)
	{
		if (!neededResources.Contains(_needed))
						neededResources.Add (_needed);
	}

	/// <summary>
	/// Removes the resource from neededResources.
	/// </summary>
	/// <param name="_needed">resource</param>
	public void RemoveNeededResource(string _needed)
	{
		int inList = -1;

		for (int i = 0; i < neededResources.Count; i++) {
						if (neededResources [i] == _needed)
								inList = i;
				}

		if (inList >= 0)
						neededResources.RemoveAt (inList);
	}
}
