using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
	public GameObject[] possibleItems = new GameObject[10];
	public Vector3[] itemsPosition = new Vector3[10];
	public Vector3[] itemsRotation = new Vector3[10];

	public GameObject go;

	public void CreateOrRemove(int num)//num번째 아이템을 생성
	{
		if (possibleItems[num] != null)//possibleItems의 num번째에 프리팹이 들어 있다면
		{
			if (go == null)//전에 생성한 오브젝트가 없다면
			{
				go = Instantiate(possibleItems[num]) as GameObject;//possibleItems의 num번째 프리팹의 clone을 생성
				go.transform.position = itemsPosition[num];
				go.transform.eulerAngles = itemsRotation[num];
			}
			else
			{
				Destroy(go);
			}
		}
	}
}
