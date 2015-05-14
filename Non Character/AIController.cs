using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI controller.
/// 
/// Manages the group of AI Units in group.
/// Meant to allow debug and feedback
/// 
/// TODO: figure out the proper term for ai units and replace player....
/// </summary>
public class AIController : MonoBehaviour {
	
	#region Testing
	public Text debugText;
	public Canvas canvas;
	#endregion Testing

	public GameObject centerMarker;
	public List<GameObject> aiPlayers = new List<GameObject>();

	public GameObject debugpanel;
	public GameObject buttonprefab;
	public int currentPlayer = 0;

	// Use this for initialization
	void Start () {
		GameObject debugPanel = Instantiate<GameObject> (debugpanel);
		debugPanel.transform.SetParent(canvas.transform);
		RectTransform t = debugPanel.transform as RectTransform;
		t.anchoredPosition = new Vector2 (t.rect.width * 0.5f + 10, t.rect.height * -0.5f - 40);
		debugText = debugPanel.GetComponentInChildren<Text>();
		debugText.font = Font.CreateDynamicFontFromOSFont ("Arial", 14);
		debugText.color = Color.black;

		for (int i = 0; i < this.transform.childCount; i++) {
						if (this.transform.GetChild (i).gameObject.name.Contains ("AI"))
								aiPlayers.Add (this.transform.GetChild (i).gameObject);
				}
		Debug.Log ("Found " + aiPlayers.Count + " AI Players");

		CreateUIMenu ();
	}
	
	// Update is called once per frame
	void Update () {
		FindAIGroups ();
		UpdateDebugText ();
		MarkCurrentPlayer ();
	}

	/// <summary>
	/// Finds AI groups for possible herd behaviours.
	/// </summary>
	void FindAIGroups(){
		if (!centerMarker)
						return;

		Vector3 position = Vector3.zero;
		List<Transform> groupList = new List<Transform> ();
		int largestGroup = 0;
		int groupCount = 0;

		foreach (GameObject player in aiPlayers) {
			int c = player.GetComponent<AIAwareness>().playerProximityCount;
			if(c > largestGroup)
				largestGroup = c;
			}

		if (largestGroup <= 1) {
						centerMarker.GetComponent<MeshRenderer> ().enabled = false;
						return;
				} else {
						centerMarker.GetComponent<MeshRenderer> ().enabled = true;
				}
			
			foreach (GameObject player in aiPlayers) {
						int c = player.GetComponent<AIAwareness> ().playerProximityCount;
						if (c == largestGroup) {
								groupList.Add (player.transform);
								position += player.transform.position;
								groupCount++;
						}
				}


		position = new Vector3 (position.x / groupCount, 0.001f, position.z / groupCount);
		centerMarker.transform.position = position;
	}

	/// <summary>
	/// Sets the current player.
	/// </summary>
	/// <param name="player">Player</param>
	void SetCurrentPlayer(int player){
		Debug.Log("Set Player to: " + player);
		currentPlayer = player;
		
		GameObject dropdown = canvas.transform.FindChild ("DropDownPanel").gameObject as GameObject;
		
		if (dropdown.activeSelf)
			dropdown.SetActive (false);
	}

	/// <summary>
	/// Debug overlay of the player selected through the GUI.
	/// </summary>
	void MarkCurrentPlayer()
	{
		Debug.DrawLine (aiPlayers [currentPlayer].transform.position, aiPlayers [currentPlayer].GetComponent<NavMeshAgent> ().destination);

		Debug.DrawRay (aiPlayers [currentPlayer].transform.position, aiPlayers [currentPlayer].transform.right);
		Debug.DrawRay (aiPlayers [currentPlayer].transform.position, aiPlayers [currentPlayer].transform.forward);
	}

	void UpdateDebugText(){
				debugText.text = "Player: " + aiPlayers [currentPlayer].name + "\n";
				foreach (AiNeed need in aiPlayers[currentPlayer].GetComponent<AINeedManager>().needs) {
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
	void CreateUIMenu(){
		RectTransform dropdown = canvas.transform.FindChild ("DropDownPanel") as RectTransform;
		dropdown.sizeDelta = new Vector2(dropdown.rect.width, (40 * aiPlayers.Count));

		for (int i = 0; i < aiPlayers.Count; i++) {
			GameObject newbutton = GameObject.Instantiate<GameObject>(buttonprefab);
			newbutton.GetComponentInChildren<Text>().text = aiPlayers[i].name;
			newbutton.transform.SetParent(dropdown);
			int p = i;
			newbutton.GetComponent<Button>().onClick.AddListener(() => SetCurrentPlayer(p));
		}
	}
	/// <summary>
	/// Toggles the drop down.
	/// </summary>
	public void ToggleDropDown(){
				GameObject dropdown = canvas.transform.FindChild ("DropDownPanel").gameObject as GameObject;
				if (dropdown.activeSelf)
						dropdown.SetActive (false);
				else
						dropdown.SetActive (true);
		}

}
