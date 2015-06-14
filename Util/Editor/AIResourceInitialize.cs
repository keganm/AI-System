using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AIResourceInitialize : MonoBehaviour
{
		public bool isInit = false;
		List<string> tagNames;
	
		void Awake ()
		{
				Init ();
		}

		private void Init ()
		{
				isInit = true;
				tagNames = new List<string> ();

				foreach (KeyValuePair<AIEnumeration.ResourceType, AINeed> _need in AIInitialVariables.needDictionary)
						tagNames.Add (_need.Value.resource);

				SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset") [0]);
				SerializedProperty tagsProp = tagManager.FindProperty ("tags");

				foreach (string s in tagNames) {
						bool isFound = false;

						for (int i = 0; i < tagsProp.arraySize; i++) {
								SerializedProperty t = tagsProp.GetArrayElementAtIndex (i);
								if (t.stringValue.Equals (s)) {
										isFound = true;
										break;
								}

						}

						if (!isFound) {
								tagsProp.InsertArrayElementAtIndex (0);
								SerializedProperty n = tagsProp.GetArrayElementAtIndex (0);
								n.stringValue = s;
						}
				}
				// *** save changes
				tagManager.ApplyModifiedProperties ();

		}

		void Update ()
		{
				if (!isInit)
						Init ();

				for (int i = 0; i < this.gameObject.transform.childCount; i++) {
						GameObject child = this.gameObject.transform.GetChild (i).gameObject;
			
						string childname = child.name.ToUpper();
						for (int n = 0; n < tagNames.Count; n++) {
								if (childname.Contains (tagNames [n].ToUpper())) {

										AddAIResource (child, tagNames [n]);
										break;
								}
						}
				}

				SetAIResources ();
		}

		private void AddAIResource (GameObject _child, string _resourceName)
		{
				_child.tag = _resourceName;

				AIResource childResource = _child.GetComponent<AIResource> ();
				if (childResource == null) {
						childResource = _child.AddComponent<AIResource>();
						childResource.resourceType = (AIEnumeration.ResourceType)0;
				}
		
				if (AIInitialVariables.needDictionary [childResource.resourceType].resource != _resourceName) {
					
					foreach (KeyValuePair<AIEnumeration.ResourceType, AINeed> _need in AIInitialVariables.needDictionary) {
						if (_need.Value.resource == _resourceName) {
							childResource.resourceType = _need.Key;
							return;
						}
					}
					return;
				}
		}

		private void SetAIResources()
	{
		GameObject AiManager = GameObject.Find ("AI");
		if (AiManager == null)
						return;

		AIResource[] resources = AiManager.GetComponentsInChildren<AIResource> ();
		for(int i = 0; i < resources.Length; i++)
		{
			resources[i].gameObject.tag = AIInitialVariables.needDictionary[AIEnumeration.ResourceType.Companionship].resource;
			resources[i].resourceType = AIEnumeration.ResourceType.Companionship;
		}
	}
}

