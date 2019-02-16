using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maping : MonoBehaviour
{
	
	
	public class island
	{
		private Vector2 position;
		private float maxRadius;
		
		public island(Vector2 position, float maxRadius)
		{
			this.position = position;
			this.maxRadius = maxRadius;
		}

		public void SetIsland()
		{
			for (int x = -1; x < 1; x++)
			{
				for (int y = -1; y < 1; y++)
				{
					//if()
				}
			}
		}
	}
}