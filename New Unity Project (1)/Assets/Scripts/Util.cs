using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
	/// <summary>
	/// 수요량, 인구수, 판매욕구, 공급감소량
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="c"></param>
	/// <param name="d"></param>
	/// <returns></returns>
	public static float SetPrice(float a, float b, float c, float d)
	{
		float price = 0;

		price = (b - d) / (a + c);

		return price;
	}
}
