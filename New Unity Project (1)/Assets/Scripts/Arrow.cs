using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

	private Transform camera;
	public Color after;
	public Color before;

	private MeshRenderer mr1;
	private MeshRenderer mr2;
	private MeshRenderer mr3;

	private Seat st;

	private Vector3 parentPos;

	void Start()
	{
		camera = GameObject.Find("Main Camera").transform;
			
		mr1 = transform.GetChild(0).GetComponent<MeshRenderer>();
		mr2 = transform.GetChild(1).GetComponent<MeshRenderer>();
		mr3 = transform.GetChild(2).GetComponent<MeshRenderer>();

		st = transform.parent.GetComponent<Seat>();

		
	}

	void Update () {
		transform.eulerAngles = new Vector3(0, camera.eulerAngles.y, 0);
		parentPos = transform.parent.position;
		transform.position = new Vector3(parentPos.x, parentPos.y + Mathf.Sin(Time.timeSinceLevelLoad) * 0.1f, parentPos.z);

		if (st.go == null)
		{
			mr1.material.color = after;
			mr2.material.color = after;
			mr3.material.color = after;
		}
		else
		{
			mr1.material.color = before;
			mr2.material.color = before;
			mr3.material.color = before;
		}
	}
}
