using UnityEngine;
using System.Collections;

public class AIGridComponent {
	public Vector2 Center;
	public float SearchSize;
	public int SearchCount;
	public int MaxSearchCount = 0;
	public bool FullyExplored;


	public AIGridComponent (Vector2 center, float size) {
		Center = center;
		SearchSize = size;

		SearchCount = 0;
		FullyExplored = false;
	}

	public bool Search(){
		SearchCount++;
				if (SearchCount > MaxSearchCount)
						FullyExplored = true;

		return FullyExplored;
		}
}
