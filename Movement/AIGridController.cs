using UnityEngine;
using System.Collections;

public class AIGridController : MonoBehaviour {

	public enum CardinalDirection{North,East,South,West};

	bool gridComponentsLeft = true;

	int gridResolution = 20;

	public AIGridComponent[] gridComponents;
	public AIGridComponent fullGrid;
	int currentGrid;
	int countExplored = 0;

	public bool gridFullyExplored;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//DebugGrid ();
	}

	public AIGridComponent GetCurrentGrid()
	{
		if (gridFullyExplored)
						return fullGrid;
		else
						return gridComponents [currentGrid];
	}

	public void DoSearch(){
				if (gridFullyExplored)
								return;
				if (gridComponents [currentGrid].Search ())
						MoveToNextGrid ();
		}

	public void BuildGrid(Rect gridrect) {
		fullGrid = new AIGridComponent (gridrect.center, gridrect.width * 0.5f);

		float gridHorizontalStep = gridrect.width / gridResolution;
		float gridVerticalStep = gridrect.height / gridResolution;
		float gridHorizontalHalf = gridHorizontalStep * 0.5f;
		float gridVerticalHalf = gridVerticalStep * 0.5f;

		float gridSize = (gridHorizontalStep + gridVerticalStep) * 0.5f;

		gridComponents = new AIGridComponent[gridResolution * gridResolution];

		int indx = 0;
		for (int y = 0; y < gridResolution; y++) {
						for (int x = 0; x < gridResolution; x++) {
							Vector2 xy = new Vector2((x * gridHorizontalStep + gridHorizontalHalf) + gridrect.x, 
				                         			 (y * gridVerticalStep + gridVerticalHalf) + gridrect.y);
							gridComponents[indx] = new AIGridComponent(xy, gridSize);
							indx++;
						}
				}
		
		currentGrid = (int)Mathf.Floor(Random.Range(0, gridComponents.Length - 1));
	}

	void MoveToNextGrid()
	{
		gridFullyExplored = GridFullyExplored ();
		if (gridFullyExplored) {
						for (int i = 0; i < gridComponents.Length; i++) {
								gridComponents [i].FullyExplored = false;
								gridComponents [i].SearchCount = 0;
								gridFullyExplored = false;
								countExplored = 0;
						}
				}


		int newIndex = currentGrid;
		//North,East,South,West = 0,1,2,3
		int direction = (int)Random.Range (0, 4);
		int offset = GetOffset ((CardinalDirection)(direction));
		
		bool valid = false;
		int distance = 1;
		int tries = 0;
//		int escape = 0;
//		Debug.Log ("Explored: " + countExplored);
				while (!valid) {
						int testindex = newIndex + (offset * distance);
						bool ns = (CardinalDirection)(direction) == CardinalDirection.North || (CardinalDirection)(direction) == CardinalDirection.South;
						if (testindex > 0 
								&& testindex < gridComponents.Length
								&& (ns || Mathf.Floor (testindex / gridResolution) == Mathf.Floor (currentGrid / gridResolution))
								&& !gridComponents [testindex].FullyExplored) {
								
								valid = true;
								currentGrid = testindex;
								countExplored++;
								return;
								
						} else {
								direction++;
								if (direction > 3)
										direction = 0;
								offset = GetOffset ((CardinalDirection)((int)direction));
								tries++;
								if (tries > 4) {
										tries = 0;
										distance++;
								}
								
			
								if (distance > gridComponents.Length) {
										for (int i = 0; i < gridComponents.Length; i++) {
												if (!gridComponents [i].FullyExplored) {
														currentGrid = i;
														return;
												}
										}
										gridFullyExplored = true;
										currentGrid = 0;
										return;
								}
						}
				}
	}

	public void DebugGrid()
	{
		for (int j = 0; j < gridComponents.Length; j++) {
			Debug.DrawRay(new Vector3(gridComponents[j].Center.x, 0, gridComponents[j].Center.y), Vector3.up);
		}
		int i = currentGrid;
			Debug.DrawLine(new Vector3(gridComponents[i].Center.x - gridComponents[i].SearchSize,0,gridComponents[i].Center.y - gridComponents[i].SearchSize),
			               new Vector3(gridComponents[i].Center.x + gridComponents[i].SearchSize,0,gridComponents[i].Center.y - gridComponents[i].SearchSize));
			
			Debug.DrawLine(new Vector3(gridComponents[i].Center.x - gridComponents[i].SearchSize,0,gridComponents[i].Center.y - gridComponents[i].SearchSize),
			               new Vector3(gridComponents[i].Center.x - gridComponents[i].SearchSize,0,gridComponents[i].Center.y + gridComponents[i].SearchSize));
			
			Debug.DrawLine(new Vector3(gridComponents[i].Center.x + gridComponents[i].SearchSize,0,gridComponents[i].Center.y - gridComponents[i].SearchSize),
			               new Vector3(gridComponents[i].Center.x + gridComponents[i].SearchSize,0,gridComponents[i].Center.y + gridComponents[i].SearchSize));
			
			Debug.DrawLine(new Vector3(gridComponents[i].Center.x + gridComponents[i].SearchSize,0,gridComponents[i].Center.y + gridComponents[i].SearchSize),
			               new Vector3(gridComponents[i].Center.x - gridComponents[i].SearchSize,0,gridComponents[i].Center.y + gridComponents[i].SearchSize));
	}

	public int GetOffset(CardinalDirection dir)
	{
		if (dir == CardinalDirection.North)
						return gridResolution;
				else if (dir == CardinalDirection.East)
						return 1;
				else if (dir == CardinalDirection.South)
						return -gridResolution;
				else if (dir == CardinalDirection.West)
						return -1;
				else
						return gridResolution;
	}

	public bool GridFullyExplored(){
				return countExplored >= gridComponents.Length;
		}
}
