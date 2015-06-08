/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// An attempt to handle sight collision with resources.
/// Will create a list of contact points in awareness
/// Uses cone collider
/// 
/// TODO: decide wheter this should actually be integrated
/// </summary>
public class AISightCollisionHandler : MonoBehaviour {
	
	public AIEntity parent;
	//Count for debugging
	public int count;
	
	public void Init (AIEntity _parent)
	{
		parent = _parent;
	}
	
	void OnTriggerEnter (Collider other)
	{
	}

	void OnTriggerExit (Collider other)
	{

	}
}
