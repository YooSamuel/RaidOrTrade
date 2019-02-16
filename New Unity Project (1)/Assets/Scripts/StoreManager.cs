using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
	public GameObject[] stores;

	// Use this for initialization
	void Start () {
		
	}


	IEnumerator Timer(float time)
	{
		while (true)
		{
			yield return new WaitForSeconds(time);
			
		}
	}

	void Deal()
	{
		
	}
}
