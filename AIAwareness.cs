﻿/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI awareness.
/// Manage what resources the AI knows about, can see and it's relation to other AI player
/// </summary>
public class AIAwareness : MonoBehaviour
{

		Dictionary<string, List<GameObject>> resourceDictionary = new Dictionary<string, List<GameObject>> ();
		public int playerProximityCount = 0;

		/// <summary>
		/// Gets the named resource list.
		/// </summary>
		/// <returns>The resource list.</returns>
		/// <param name="name">resource</param>
		public List<GameObject> GetResourceList (string name)
		{
				if (resourceDictionary.ContainsKey (name))
						return resourceDictionary [name];
				else
						return new List<GameObject> ();
		}

		/// <summary>
		/// Gets the list of named resource list.
		/// </summary>
		/// <returns>The resource list.</returns>
		/// <param name="names">resource names</param>
		public List<GameObject> GetResourceList (List<string> names)
		{
				List<GameObject> tmp = new List<GameObject> ();

				for (int i = 0; i < names.Count; i++) {
						if (resourceDictionary.ContainsKey (names [i]))
								tmp.AddRange (resourceDictionary [names [i]]);
				}

				return tmp;
		}

		/// <summary>
		/// Checks to see if the AI knows about the resource or can see it
		/// </summary>
		/// <returns><c>true</c>, if resource list was checked, <c>false</c> otherwise.</returns>
		/// <param name="resource">Resource.</param>
		public bool CheckResourceList (GameObject resource, bool IsPriority)
		{
				if (resourceDictionary.ContainsKey (resource.tag) && IsPriority) {
						foreach (GameObject res in resourceDictionary[resource.tag]) {
								if (res == resource) {
										return true;
								}
						}
				}

				RaycastHit hit;
				if (Physics.Raycast (this.transform.position, resource.transform.position - this.transform.position, out hit, 3)) {
						if (hit.collider.gameObject == resource)
								return true;
				}
				return false;
		}

		public void AddPlayer (Collider other)
		{
				//Debug.Log ("Adding Player");
				playerProximityCount++;
		}

		public void RemovePlayer (Collider other)
		{
				//Debug.Log ("Removing Player");
				if (playerProximityCount > 0)
						playerProximityCount--;
		}

		public void AddResource (Collider other)
		{
				if (other.gameObject == this.gameObject)
						return;
				string res = other.tag;
				//Debug.Log ("Adding Resource: " + res);
				if (!resourceDictionary.ContainsKey (res)) {
						resourceDictionary [res] = new List<GameObject> ();
				}
		
				bool alreadyInList = false;
		
				for (int i = 0; i < resourceDictionary[res].Count; i++) {
						if (other.gameObject == resourceDictionary [res] [i])
								alreadyInList = true;
				}
		
				if (!alreadyInList)
						resourceDictionary [res].Add (other.gameObject);
		}
	
		void OnTriggerEnter (Collider other)
		{


		}

		void OnTriggerExit (Collider other)
		{

		}
}