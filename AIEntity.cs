/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 * 
 * no current dependencies
 * 
 * notes located at bottom
 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// AI entity.
/// Primary class to be attached to AI entity
/// </summary>
public class AIEntity : MonoBehaviour
{
		//Classes will be added automatically
	#region SubClassComponents
		public AIMovement movement;
		public AIAwareness awareness;
		public AINeedManager needManager;
		public AIResourceManager resourceManager;
		public AIGridController gridController;
		public AITraitManager traitManager;
		AIResourceHandler resourceHandler;
		AIOtherEntityHandler otherEntityHandler;
	#endregion SubClassComponents


		public bool needsResearch = false;
		public List<GameObject> currentResources = new List<GameObject> ();


		/// <summary>
		/// Start this instance.
		/// Finds or creates other component classes
		/// and initiates variables
		/// 
		/// AINeedManager - handles instances needs
		/// AIMovement - handles instances interaction with NavMesh and AIGridManager
		/// AIAwareness - handles instances learning mechanism of resources in environment
		/// </summary>
		void Awake ()
		{

				//Create or connect to Main sub classes
				needManager = GetOrAddComponent<AINeedManager> ();
				movement = GetOrAddComponent<AIMovement> ();
				awareness = GetOrAddComponent <AIAwareness> ();
				resourceManager = GetOrAddComponent<AIResourceManager> ();
				gridController = GetOrAddComponent<AIGridController> ();
				traitManager = GetOrAddComponent<AITraitManager> ();
				
				
				//Pass ResourceTarget and GridController to AIMovement
				movement.Init (resourceManager, gridController);

				//Find and connect to collision handlers
				resourceHandler = this.transform.GetComponentInChildren<AIResourceHandler> ();
				otherEntityHandler = this.transform.GetComponentInChildren<AIOtherEntityHandler> ();
				
				if (resourceHandler)
						resourceHandler.Init (this);
				if (otherEntityHandler)
						otherEntityHandler.Init (this);

		
				ResetInitialVariables ();
		}
		
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update ()
		{
				//Check current resources of AI
				CheckCurrentResource ();
				//Update needs in AI
				needManager.Update ();
		}

		/// <summary>
		/// Create or get and return a component
		/// </summary>
		/// <returns>The component.</returns>
		public T GetOrAddComponent<T> () 
		where T : UnityEngine.Component, new()
		{
				//Create or connect to Main sub classes
				if (!this.transform.gameObject.GetComponent<T> ())
						return this.transform.gameObject.AddComponent<T> ();
				else
						return this.transform.GetComponent<T> ();
			
		}

		public void SetGridController(AIGridController other)
		{
			gridController.CopyGridController (other);
			movement.SetGridController (gridController);
		}

		/// <summary>
		/// Resets the initial variables.
		/// </summary>
		void ResetInitialVariables ()
		{
				currentResources.Clear ();
				needsResearch = false;
				movement.trackingState = AIEnumeration.Tracking.Aimless;

				needManager.Reset ();
		}

		/// <summary>
		/// Checks the current resource.
		/// </summary>
		public void CheckCurrentResource ()
		{
				if (currentResources.Count == 0) {
						//no resources are available make sure no needs are refilling and exit
						foreach (AINeed need in needManager.needList)
								need.refilling = false;
						return;
				}
		
				foreach (GameObject currentResource in currentResources) {
						AIResource res = currentResource.GetComponent<AIResource> ();
						
						//Check availability of resources AI has connection with and determine whether need is refilling
						foreach (AINeed need in needManager.needList) {
								if (need.resource == currentResource.tag && need.inNeed) {
										needsResearch = false;
					
										if (res.ResourceAvailable ()) {
												need.refilling = true;

												if (!need.inNeed) {
														needsResearch = true;
												}
						
										} else {
												need.refilling = false;
												needsResearch = true;
										}
								}
						}
				}
		}

		/// <summary>
		/// Starts the refill.
		/// Called from AIResourceHandler
		/// </summary>
		/// <param name="collision">resource collision</param>
		public void StartRefill (GameObject collision)
		{
				if (collision.GetComponent<AIResource> ()) {
						currentResources.Add (collision);
						foreach (AINeed need in needManager.needList) {
								if (collision.tag == need.resource)
										need.refilling = true;

						}
				}
		}

		/// <summary>
		/// Ends the refill.
		/// Called from AIResourceHandler
		/// </summary>
		/// <param name="collision">resource collision</param>
		public void EndRefill (GameObject other)
		{
				if (currentResources.Count == 0)
						return;

				if (currentResources.Contains (other)) {
						foreach (AINeed need in needManager.needList) {
								if (other.tag == need.resource) {
										need.refilling = false;
								}
						}
						currentResources.Remove (other);
				}
		}

}

/** Potential AI modification approaches
 * 
 * Establish 'home base' (a position that is most efficient ie resources...)
 * following establishment of 'home' begin a venturing outward behaviour (similar to wander now)
 * 
 * Integrate possession of resources in some way ( Claiming, aggression, health & violence?)
 * 
 * Finesse test environment
 * 
 * Create a POV cam (pip?) for selected AI?
 **/