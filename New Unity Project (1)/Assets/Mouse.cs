using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
	public Camera miniCamera;
	public GameObject ship;
		
	// Use this for initialization
	void Start ()
	{
		ship.GetComponent<ShipMovement>().target = ship.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;

			Ray ray = miniCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				Vector3 pos = new Vector3(hit.point.x,0f,hit.point.z);
				ship.GetComponent<ShipMovement>().target = pos;
			}
		}
	}
}
