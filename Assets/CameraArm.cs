using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArm : MonoBehaviour {

	[SerializeField] private Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (target == null)
		{
			Debug.LogWarning("Attach target to camera arm!");
			return;	
		}	

		this.transform.position = target.transform.position;
	}
}
