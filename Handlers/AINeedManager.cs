using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AINeedManager : MonoBehaviour  {

	public List<AiNeed> needs = new List<AiNeed>();

	public List<string> needed = new List<string> ();

	public AINeedManager()
	{
		
				Reset ();
	}
	public void Reset()
	{	
		needed.Clear ();
		needs.Clear ();

		needs.Add (new AiNeed ("Entertainment", AIInitialVariables.entertainment, AIInitialVariables.entertainmentLossRate, AIInitialVariables.entertainmentRecoveryRate));
		needs.Add (new AiNeed ("Food", AIInitialVariables.food, AIInitialVariables.foodLossRate, AIInitialVariables.foodRecoveryRate));
		needs.Add (new AiNeed ("Rest", AIInitialVariables.rest, AIInitialVariables.restLossRate, AIInitialVariables.restRecoveryRate));
		needs.Add (new AiNeed ("Companionship", AIInitialVariables.companionship, AIInitialVariables.companionshipLossRate, AIInitialVariables.companionshipRecoveryRate));
	}
	
	// Update is called once per frame
	public void Update () {
		
		bool isRefilling = false;
		bool needsResource = false;
		AiNeed neededResource = null;
		
		foreach(AiNeed need in needs){
			
			need.UpdateNeed();
			
			
			if(need.inNeed){
				needsResource = true;
				neededResource = need;
			}

			if(need.inNeed){
				AddNewNeeded(need.resource);
			}else{
				RemoveNeeded(need.resource);
			}

		}

	}

	public void AddNewNeeded(string _needed)
	{
		bool alreadyNeeded = true;

		for (int i = 0; i < needed.Count; i++) {
			if(needed[i] == _needed)
				alreadyNeeded = false;
		}

		if (alreadyNeeded)
						needed.Add (_needed);

	}

	public void RemoveNeeded(string _needed)
	{
		int inList = -1;

		for (int i = 0; i < needed.Count; i++) {
						if (needed [i] == _needed)
								inList = i;
				}

		if (inList >= 0)
						needed.RemoveAt (inList);
	}
}
