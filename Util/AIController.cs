/**
 * Artificial Intelligence System for Unity 3D
 * Author: Kegan McGurk
 **/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI controller.
/// 
/// Manages the group of AI Units in group.
/// Meant to allow debug and feedback
/// </summary>
public class AIController : MonoBehaviour
{
	
	#region Testing
		public Text debugText;
		public Canvas canvas;
	#endregion Testing

		public GameObject centerMarker;
		public List<GameObject> aiEntitys = new List<GameObject> ();
		public GameObject debugpanel;
		public GameObject buttonprefab;
		public int currentEntity = 0;

	AIGridController mainGridController;


		/// <summary>
		/// Start this instance, setup gui, and connect to AI entitys.
		/// </summary>
		void Start ()
		{
		

		mainGridController = this.transform.gameObject.AddComponent<AIGridController> ();
		mainGridController.BuildGrid(NavMesh.CalculateTriangulation ().vertices);


				//Setup GUI
				GameObject debugPanel = Instantiate<GameObject> (debugpanel);
				debugPanel.transform.SetParent (canvas.transform);

				RectTransform t = debugPanel.transform as RectTransform;
				t.anchoredPosition = new Vector2 (t.rect.width * 0.5f + 10, t.rect.height * -0.5f - 40);

				debugText = debugPanel.GetComponentInChildren<Text> ();
				debugText.font = Font.CreateDynamicFontFromOSFont ("Arial", 14);
				debugText.color = Color.black;

				for (int i = 0; i < this.transform.childCount; i++) {
						if (this.transform.GetChild (i).gameObject.name.Contains ("AI"))
								aiEntitys.Add (this.transform.GetChild (i).gameObject);
				}
				Debug.Log ("Found " + aiEntitys.Count + " AI Entitys");

				CreateUIMenu ();
				
				foreach (GameObject ai in aiEntitys) {

			ai.GetComponent<AIEntity>().SetGridController (mainGridController);
				}

		}

		/// <summary>
		/// Run controller updates.
		/// </summary>
		void Update ()
		{
				FindAIGroups ();
				UpdateDebugText ();
				MarkCurrentEntity ();
		}

		/// <summary>
		/// Finds AI groups for possible herd behaviours.
		/// </summary>
		void FindAIGroups ()
		{
				if (!centerMarker)
						return;

				Vector3 position = Vector3.zero;
				List<Transform> groupList = new List<Transform> ();
				int largestGroup = 0;
				int groupCount = 0;

				foreach (GameObject entity in aiEntitys) {
						int c = entity.GetComponent<AIAwareness> ().entityProximityCount;
						if (c > largestGroup)
								largestGroup = c;
				}

				if (largestGroup <= 1) {
						centerMarker.GetComponent<MeshRenderer> ().enabled = false;
						return;
				} else {
						centerMarker.GetComponent<MeshRenderer> ().enabled = true;
				}
			
				foreach (GameObject entity in aiEntitys) {
						int c = entity.GetComponent<AIAwareness> ().entityProximityCount;
						if (c == largestGroup) {
								groupList.Add (entity.transform);
								position += entity.transform.position;
								groupCount++;
						}
				}


				position = new Vector3 (position.x / groupCount, 0.001f, position.z / groupCount);
				centerMarker.transform.position = position;
		}

		/// <summary>
		/// Sets the current entity.
		/// </summary>
		/// <param name="entity">Entity</param>
		void SetCurrentEntity (int entity)
		{
				Debug.Log ("Set Entity to: " + entity);
				currentEntity = entity;
		
				GameObject dropdown = canvas.transform.FindChild ("DropDownPanel").gameObject as GameObject;
		
				if (dropdown.activeSelf)
						dropdown.SetActive (false);
		}

		/// <summary>
		/// Debug overlay of the entity selected through the GUI.
		/// </summary>
		void MarkCurrentEntity ()
		{
				Debug.DrawLine (aiEntitys [currentEntity].transform.position, aiEntitys [currentEntity].GetComponent<NavMeshAgent> ().destination);

				Debug.DrawRay (aiEntitys [currentEntity].transform.position, aiEntitys [currentEntity].transform.right);
				Debug.DrawRay (aiEntitys [currentEntity].transform.position, aiEntitys [currentEntity].transform.forward);
		}

		void UpdateDebugText ()
		{
				debugText.text = "Entity: " + aiEntitys [currentEntity].name + "\n";
				foreach (AINeed need in aiEntitys[currentEntity].GetComponent<AINeedManager>().needList) {
						debugText.text += need.resource + ": " + need.current;
						if (need.inNeed)
								debugText.text += " ~Needed";
						if (need.refilling)
								debugText.text += " ~Refill";
						debugText.text += "\n";
				}
		}

		/// <summary>
		/// Creates the user interface menu.
		/// </summary>
		void CreateUIMenu ()
		{
				RectTransform dropdown = canvas.transform.FindChild ("DropDownPanel") as RectTransform;
				dropdown.sizeDelta = new Vector2 (dropdown.rect.width, (40 * aiEntitys.Count));

				for (int i = 0; i < aiEntitys.Count; i++) {
						GameObject newbutton = GameObject.Instantiate<GameObject> (buttonprefab);
						newbutton.GetComponentInChildren<Text> ().text = aiEntitys [i].name;
						newbutton.transform.SetParent (dropdown);
						int p = i;
						newbutton.GetComponent<Button> ().onClick.AddListener (() => SetCurrentEntity (p));
				}
		}
		/// <summary>
		/// Toggles the drop down.
		/// </summary>
		public void ToggleDropDown ()
		{
				GameObject dropdown = canvas.transform.FindChild ("DropDownPanel").gameObject as GameObject;
				if (dropdown.activeSelf)
						dropdown.SetActive (false);
				else
						dropdown.SetActive (true);
		}

}
