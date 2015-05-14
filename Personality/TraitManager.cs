using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TraitManager : MonoBehaviour {
	List<Trait> traits = new List<Trait>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < Trait.TypeCount; i++) {
						traits.Add (new Trait ((Trait.TraitType)i));
				}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
