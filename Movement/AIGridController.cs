/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using System.Collections;

/// <summary>
/// AI grid controller.
/// Creates a grid of the NavMesh to allow AI to slowly expand exploration systematically through the environment
/// Uses Cardinal directions to slowly move through grid and track the exploration
/// </summary>
public class AIGridController : MonoBehaviour
{
		//Dictates resolution to solve for
		int gridResolution = 20;
		Rect navmeshBounds;

		//GridComponent holds basic class
		public AIGridComponent[] gridComponents;
		public AIGridComponent fullGrid;

		//Track AIs movement through grid
		int currentGrid;
		int countExplored = 0;
		public bool gridFullyExplored;

		/// <summary>
		/// Gets the current gridComponent.
		/// </summary>
		/// <returns>The current gridComponent.</returns>
		public AIGridComponent GetCurrentGrid ()
		{
				if (gridFullyExplored)
						return fullGrid;
				else
						return gridComponents [currentGrid];
		}

		/// <summary>
		/// Update current grid for number of times explored.
		/// </summary>
		public void DoSearch ()
		{
				if (gridFullyExplored)
						return;
				if (gridComponents [currentGrid].Search ())
						MoveToNextGrid ();
		}

		/// <summary>
		/// Builds the gridComponents.
		/// TODO: Unify here, and pass grid components down to AI entitys
		/// </summary>
		/// <param name="verticies">Verticies from navMesh.</param>
		public void BuildGrid (Vector3[] verticies)
		{
		
				float Top = verticies [0].z;
				float Bottom = verticies [0].z;
				float Left = verticies [0].x;
				float Right = verticies [0].x;
		
				foreach (Vector3 vert in verticies) {
						if (vert.x < Left)
								Left = vert.x;
						if (vert.x > Right)
								Right = vert.x;
						if (vert.z < Top)
								Top = vert.z;
						if (vert.z > Bottom)
								Bottom = vert.z;
				}

				navmeshBounds = new Rect (Left, Top, Right - Left, Bottom - Top);

				BuildGridComponents (navmeshBounds);
		}

		/// <summary>
		/// Create full navMesh, and grid of navMesh.
		/// </summary>
		/// <param name="gridrect">navMesh rectangle.</param>
		public void BuildGridComponents (Rect gridrect)
		{
				//Component of full rectangle
				fullGrid = new AIGridComponent (gridrect.center, gridrect.width * 0.5f);

				//Find steps for creating grid array
				float gridHorizontalStep = gridrect.width / gridResolution;
				float gridVerticalStep = gridrect.height / gridResolution;
				float gridHorizontalHalf = gridHorizontalStep * 0.5f;
				float gridVerticalHalf = gridVerticalStep * 0.5f;

				float gridSize = (gridHorizontalStep + gridVerticalStep) * 0.5f;

				gridComponents = new AIGridComponent[gridResolution * gridResolution];

				//Create indexed grid (y*w+x)
				int indx = 0;
				for (int y = 0; y < gridResolution; y++) {
						for (int x = 0; x < gridResolution; x++) {
								Vector2 xy = new Vector2 ((x * gridHorizontalStep + gridHorizontalHalf) + gridrect.x, 
				                         			 (y * gridVerticalStep + gridVerticalHalf) + gridrect.y);
								gridComponents [indx] = new AIGridComponent (xy, gridSize);
								indx++;
						}
				}
				SetStartGridPoint ();
		}

		/// <summary>
		/// Copies the grid controller.
		/// </summary>
		/// <param name="other">Other Grid Controller.</param>
		public void CopyGridController (AIGridController other)
		{
				gridResolution = other.gridResolution;
				navmeshBounds = other.navmeshBounds;
				countExplored = other.countExplored;
				gridFullyExplored = other.gridFullyExplored;

				fullGrid = new AIGridComponent (Vector2.zero, -1);
				fullGrid.CopyGridComponent (other.fullGrid);
				gridComponents = new AIGridComponent [other.gridComponents.Length];
				for (int i = 0; i < other.gridComponents.Length; i++) {
						gridComponents [i] = new AIGridComponent (Vector2.zero, -1);
						gridComponents [i].CopyGridComponent (other.gridComponents [i]);
				}

				SetStartGridPoint ();
		}

		/// <summary>
		/// Sets the starting grid point.
		/// </summary>
		void SetStartGridPoint ()
		{
				//Start with random grid target
				//TODO:Base this on start point?
				currentGrid = (int)Mathf.Floor (Random.Range (0, gridComponents.Length - 1));	
		}

		/// <summary>
		/// Move the currentGrid to another component using cardinal direction.
		/// </summary>
		void MoveToNextGrid ()
		{
				//Check to see if navMesh is fully explored
				gridFullyExplored = GridFullyExplored ();
				//Reset search so AI will continue to move around environment
				//Could insert another behaviour once fully explored
				if (gridFullyExplored) {
						for (int i = 0; i < gridComponents.Length; i++) {
								gridComponents [i].FullyExplored = false;
								gridComponents [i].SearchCount = 0;
								gridFullyExplored = false;
								countExplored = 0;
						}
				}

				//Temp vars for finding next component
				int newIndex = currentGrid;
				//Random Cardinal direction
				int direction = (int)Random.Range (0, 4);
				int offset = GetOffset ((AIEnumeration.CardinalDirection)(direction));
		
				bool valid = false;
				int distance = 1;
				int tries = 0;

				//Tests adjacent grid components if already explored and sets as current if not
				while (!valid) {
						//increase distance to test if all surrounding components are searched
						int testindex = newIndex + (offset * distance);
						bool ns = (AIEnumeration.CardinalDirection)(direction) == AIEnumeration.CardinalDirection.North || (AIEnumeration.CardinalDirection)(direction) == AIEnumeration.CardinalDirection.South;
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
								offset = GetOffset ((AIEnumeration.CardinalDirection)((int)direction));
								tries++;
								if (tries > 4) {
										tries = 0;
										distance++;
								}
								
								//If distance is big enough the navMesh is fully explored
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

		/// <summary>
		/// Gets the offset of the grid (y*w + x) based off a cardinal direction.
		/// </summary>
		/// <returns>The offset of grid movement.</returns>
		/// <param name="dir">Cardinal Direction.</param>
		public int GetOffset (AIEnumeration.CardinalDirection dir)
		{
				if (dir == AIEnumeration.CardinalDirection.North)
						return gridResolution;
				else if (dir == AIEnumeration.CardinalDirection.East)
						return 1;
				else if (dir == AIEnumeration.CardinalDirection.South)
						return -gridResolution;
				else if (dir == AIEnumeration.CardinalDirection.West)
						return -1;
				else
						return gridResolution;
		}

		/// <summary>
		/// Tests if the Grid is fully explored.
		/// </summary>
		/// <returns><c>true</c>, if fully explored was grided, <c>false</c> otherwise.</returns>
		public bool GridFullyExplored ()
		{
				return countExplored >= gridComponents.Length;
		}
}
