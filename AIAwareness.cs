/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI awareness.
/// Manage what resources the AI knows about, can see and it's relation to other AI entity
/// </summary>
public class AIAwareness : MonoBehaviour
{
		AIEntity parentEntity = null;

		Dictionary<string, List<GameObject>> resourceDictionary = new Dictionary<string, List<GameObject>> ();
		Dictionary<AIEntity, AIEnumeration.ReactionType> reactionLookup = new Dictionary<AIEntity, AIEnumeration.ReactionType> ();
		public List<AIEntity> entityList = new List<AIEntity> ();
		public int entityProximityCount = 0;


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
		/// Sets the reaction lookup for passed entity.
		/// </summary>
		/// <param name="_entity">Other AI entity.</param>
		/// <param name="_reaction">Reaction between AI.</param>
		public void SetReactionLookup (AIEntity _entity, AIEnumeration.ReactionType _reaction)
		{
				reactionLookup [_entity] = _reaction;
		}

		/// <summary>
		/// Gets the reaction lookup of a particular Entity.
		/// </summary>
		/// <returns>The reaction or neutral if not found.</returns>
		/// <param name="_entity">Entity to Get reaction.</param>
		public AIEnumeration.ReactionType GetReactionLookup (AIEntity _entity)
		{
				if (reactionLookup.ContainsKey (_entity))
						return reactionLookup [_entity];
				else
						return AIEnumeration.ReactionType.Neutral;
		}

		/// <summary>
		/// Returns the Lookup dictionary of reactions.
		/// </summary>
		/// <returns>The reaction lookup.</returns>
		public Dictionary<AIEntity, AIEnumeration.ReactionType> GetReactionLookup ()
		{
				return reactionLookup;
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

		public void AddEntity (Collider other)
		{
				if (parentEntity == null)
						parentEntity = this.GetComponent<AIEntity> ();

				AIEntity otherAI = other.transform.GetComponentInParent<AIEntity> ();
				if (otherAI == null)
						return;
				//Debug.Log ("Adding Entity");
				entityProximityCount++;

				bool alreadyExists = false;
				for (int i = 0; i < entityList.Count; i++) {
						if (entityList [i] == otherAI)
								return;
				}
				if (!alreadyExists)
						entityList.Add (otherAI);

				SetReactionLookup(otherAI, parentEntity.traitManager.MergePersonalities(otherAI.traitManager));

		}

		public void RemoveEntity (Collider other)
		{
				//Debug.Log ("Removing Entity");
				if (entityProximityCount > 0)
						entityProximityCount--;
				
				for (int i = 0; i < entityList.Count; i++) {
						if (entityList [i].name == other.gameObject.name) {
								entityList.RemoveAt (i);
								return;
						}
				}
		}

		public void AddResource (Collider other)
		{
				if (other.gameObject == this.gameObject)
						return;
				string res = other.tag;
				if (res == AIInitialVariables.needDictionary [AIEnumeration.ResourceType.Companionship].resource) 
						return;
				
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
}
