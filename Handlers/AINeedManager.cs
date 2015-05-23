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
public class AINeedManager : MonoBehaviour
{

		public List<AINeed> needList = new List<AINeed> ();
		public List<string> neededResources = new List<string> ();
		public float[] neededValues;
	
		AIResourceManager resourceManager;

		public AINeedManager ()
		{
				Reset ();
		}

		/// <summary>
		/// Clear lists and recreates from AIInitialVariables
		/// TODO: Possibly automate needList generation
		/// </summary>
		public void Reset ()
		{	
		
				resourceManager = this.GetComponent<AIResourceManager> ();
				neededResources.Clear ();
				needList.Clear ();

				//Build list of needs
				//needList.Add (new AiNeed ("Entertainment", AIInitialVariables.entertainment, AIInitialVariables.entertainmentLossRate, AIInitialVariables.entertainmentRecoveryRate));
				//	needList.Add (new AiNeed ("Food", AIInitialVariables.food, AIInitialVariables.foodLossRate, AIInitialVariables.foodRecoveryRate));
				//needList.Add (new AiNeed ("Rest", AIInitialVariables.rest, AIInitialVariables.restLossRate, AIInitialVariables.restRecoveryRate));
				//needList.Add (new AiNeed ("Companionship", AIInitialVariables.companionship, AIInitialVariables.companionshipLossRate, AIInitialVariables.companionshipRecoveryRate));

				foreach (KeyValuePair<AIEnumeration.ResourceType, AINeed> _need in AIInitialVariables.needDictionary) {
						AINeed n = new AINeed ();
						n.CopyNeed (_need.Value);
						needList.Add (n);
				}
				neededValues = new float[needList.Count];
		}

		/// <summary>
		/// Sets the arrays to needs values and names
		/// </summary>
		/// <returns><c>true</c>, if needList isn't empty, <c>false</c> otherwise.</returns>
		/// <param name="_needValues">_need values.</param>
		/// <param name="_needResources">_need resources.</param>
		public bool GetNeededResources(ref float[] _needValues, ref string[] _needResources)
		{
			if (needList.Count == 0)
						return false;

			_needValues = new float[needList.Count];
			_needResources = new string[needList.Count];
			for (int i = 0; i < needList.Count; i++) {
				_needValues[i] = needList[i].current;
				_needResources[i] = needList[i].resource;
			}

			return true;	
		}

		/// <summary>
		/// Update needs in list, and add to needResources if inNeed.
		/// </summary>
		public void Update ()
		{
				foreach (AINeed need in needList) {
						if(need.UpdateNeed ())
							resourceManager.BuildResourceTargets();

						//Add need to needed list or remove it
						if (need.inNeed) {
								AddNewNeededResource (need.resource);
						} else {
								RemoveNeededResource (need.resource);
						}
				}
		}

		/// <summary>
		/// Adds the new needed resource if not already needed
		/// </summary>
		/// <param name="_needed">resourcee</param>
		public void AddNewNeededResource (string _needed)
		{
				if (!neededResources.Contains (_needed))
						neededResources.Add (_needed);
		}

		/// <summary>
		/// Removes the resource from neededResources.
		/// </summary>
		/// <param name="_needed">resource</param>
		public void RemoveNeededResource (string _needed)
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
