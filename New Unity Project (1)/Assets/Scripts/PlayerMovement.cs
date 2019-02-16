using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed = 1;
	IEnumerator sc;


	
	public void Move(Vector3[] wayPoints, int startValue)
	{
		if (sc != null)
		{
			StopCoroutine(sc);
		}
		sc = MoveCor(wayPoints, startValue);
		StartCoroutine(sc);
	}

	IEnumerator MoveCor(Vector3[] wayPoints, int startValue)
	{
		while (startValue >= 0)
		{
			Vector3 dir = new Vector3(wayPoints[startValue].x, transform.position.y, wayPoints[startValue].z) - transform.position;
			transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);

			if (Vector3.Distance(new Vector3(wayPoints[startValue].x, transform.position.y, wayPoints[startValue].z), transform.position) <= 1.5f)
			{
				startValue--;
				continue;
			}
			transform.LookAt(new Vector3(wayPoints[startValue].x, transform.position.y, wayPoints[startValue].z));
			
			yield return new WaitForEndOfFrame();
		}
	}
}
