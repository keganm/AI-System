using UnityEngine;
using System.Collections;


/// <summary>
/// AI resource.
/// 
/// Manages resource areas placed in scene
/// </summary>
public class AIResource : MonoBehaviour {
	public AIEnumeration.ResourceType resourceType;

	public float resourceAvailability = 1f;
	public float resourceConsumption = 0.01f;
	public float resourceRegrowth = 0.01f;
	public int resourceSlots = 4;

	public int playerAccessCount = 0;

	Material mat;
	public Color initialColor;

	void Start () {
		mat = this.GetComponent<MeshRenderer> ().material;
		initialColor = mat.color ;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateResource ();
	}

	/// <summary>
	/// Updates the resource.
	/// </summary>
	void UpdateResource()
	{
				int playcount = playerAccessCount;
				if (playcount > resourceSlots)
						playcount = resourceSlots;

				if (resourceAvailability > 0)
						resourceAvailability -= playcount * resourceConsumption;
				if (resourceAvailability < 1 && playcount == 0)
						resourceAvailability += resourceRegrowth;

				if (resourceAvailability < 0)
						resourceAvailability = 0;
				if (resourceAvailability > 1)
						resourceAvailability = 1;

				mat.color = initialColor * resourceAvailability;
		}

	public bool ResourceAvailable()
	{
		if(resourceAvailability > 0f && resourceSlots - playerAccessCount > 0)
			return true;

			return false;
	}

	void OnTriggerEnter(Collider other){
				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						playerAccessCount++;
				}
		}
	void OnTriggerExit(Collider other){
				if (other.gameObject.layer == LayerMask.NameToLayer ("AI")) {
						playerAccessCount--;
				}
		}
/*
	public void AddPlayer()
	{
		playerAccessCount++;
	}

	public void RemovePlayer()
	{
		if (playerAccessCount > 0)
						playerAccessCount--;
	}
	*/
}
