/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// Container for grid components making up navMesh grid.
/// </summary>
public class AIGridComponent
{
		public Vector2 Center;
		public float SearchSize;
		public int SearchCount;
		//Exploration count
		public int MaxSearchCount = 0;
		public bool FullyExplored;

		/// <summary>
		/// Initializes a new instance of the <see cref="AIGridComponent"/> class.
		/// </summary>
		/// <param name="center">Center xy position.</param>
		/// <param name="size">Size of grid.</param>
		public AIGridComponent (Vector2 center, float size)
		{
				Center = center;
				//TODO: change to width and height for non squared approach
				SearchSize = size;

				SearchCount = 0;
				FullyExplored = false;
		}

		/// <summary>
		/// Center point converted to 3D Vector.
		/// </summary>
		/// <returns>The center Vector3.</returns>
		public Vector3 GetCenter3 ()
		{
				return new Vector3 (Center.x, 0, Center.y);
		}

		/// <summary>
		/// Will maintain the AI in current GridComponent till MaxSearch reached
		/// </summary>
		public bool Search ()
		{
				SearchCount++;
				if (SearchCount > MaxSearchCount)
						FullyExplored = true;

				return FullyExplored;
		}
}
