/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// Container for AI needs.
/// </summary>
public class AINeed
{
		public string resource;
		public float current;
		public float lossRate;
		public float regenRate;
		public bool refilling = false;
		public bool inNeed = false;

	public AINeed()
	{

		}
		public AINeed (string resourceTag, float init, float loss, float regen)
		{
				current = init;
				resource = resourceTag;
				lossRate = loss;
				regenRate = regen;
		}
/// <summary>
/// Updates the need regenerates or degenerates based on refilling.
/// </summary>
		public void UpdateNeed ()
		{
				if (refilling) {

						if (current > 0)
								current -= regenRate;
						else
								current = 0;

						if (current <= 0)
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

	/// <summary>
	/// Copies the original AIneed.
	/// </summary>
	/// <param name="original">Original Need.</param>
	public void CopyNeed(AINeed original)
	{
		resource = original.resource;
		current = original.current;
		lossRate = original.lossRate;
		regenRate = original.regenRate;
		refilling = original.refilling;
		inNeed = original.inNeed;
	}
}
