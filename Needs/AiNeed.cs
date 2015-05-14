using UnityEngine;
using System.Collections;

public class AiNeed {
	public string resource;

	public float current;
	public float lossRate;
	public float regenRate;

	public bool refilling = false;
	public bool inNeed = false;

	public AiNeed(string resourceTag, float init, float loss, float regen){
				current = init;
				resource = resourceTag;
				lossRate = loss;
				regenRate = regen;
		}

	public void UpdateNeed(){
				if (refilling) {

						if (current > 0)
								current -= regenRate;
						else
								current = 0;

						if(current <= 0)
								inNeed = false;

				} else {
						if (current < 1)
								current += lossRate;
						else
								current = 1;

						if (current >= 1)
								inNeed = true;
				}
		}
}
