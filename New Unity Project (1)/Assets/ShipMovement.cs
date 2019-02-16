using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class ShipMovement : MonoBehaviour
{

	public Vector3 target;
	public float moveSpeed;

	void Update()
	{
		Vector3 direction = (target - transform.position).normalized * moveSpeed * Time.deltaTime;

		transform.Translate(direction, Space.World);
		
		transform.LookAt(target);
	}
}
