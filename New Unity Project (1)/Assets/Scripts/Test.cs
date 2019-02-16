using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public float a;
	public float b;
	public float c;
	public float d;

	private TextMesh tm;

	// Use this for initialization
	void Start ()
	{
		tm = GetComponent<TextMesh>();
	}

	void Update()
	{
		tm.text = Util.SetPrice(a, b, c, d).ToString();
	}
}
