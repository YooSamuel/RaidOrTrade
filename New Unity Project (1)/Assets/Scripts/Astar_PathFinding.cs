using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

public class Astar_PathFinding : MonoBehaviour
{
    public Camera minimapCamera;
	public Camera shipCamera;
	public Vector2 greedSize;
	public Vector2 startPos;
	public Vector2 endPos;
	public Vector2 intervalVec;
	public int radius;

	private Vector2 startPosGiz;
	private Vector2 endPosGiz;

	public GameObject player;

	public LayerMask layer;
	
	private int[,] f;
	private int[,] g;
	private int[,] h;
	private int[,] closeList;//0이면 확인 안된 목록, 1이면 열린목록, 2면 닫힌목록
	private bool[,] obstacle;
	private bool[,] cityZone;
	private int[,] parentPosX;
	private int[,] parentPosY;
	
	private float intervalX;
	private float intervalY;

	private bool isMiniMapCamera = true;
	
	void Start ()
	{
		Setting();
		CheckObstacle();
		FindCityZone();
	}
	
    void Update()
    {
	    if (isMiniMapCamera)
	    {
		    if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())return;
		    if(Input.GetMouseButtonDown(0))
		    {
			    SetStartEndPos();
			    if (closeList[(int) endPos.x, (int) endPos.y] != 2)
			    {
				    DeleteAfterCloseList();
		        
				    Astar();
				    SetWayPoints();
			    }
		    }
	    }
    }

	void SetStartEndPos()
	{
		startPos = new Vector2(Mathf.Round(player.transform.position.x / intervalVec.x),
			Mathf.Round(player.transform.position.z / intervalVec.y));
		endPos = new Vector2(Mathf.Round(FindObjectPosition().x / intervalVec.x),
			Mathf.Round(FindObjectPosition().z / intervalVec.y));
		
		startPos = new Vector2(startPos.x + intervalX * 0.5f, startPos.y + intervalY * 0.5f);
		endPos = new Vector2(endPos.x + intervalX * 0.5f, endPos.y + intervalY * 0.5f);
	}

	void DeleteAfterCloseList()
	{
		for (int x = 0; x < (int)intervalX; x++)
		{
			for (int y = 0; y < (int)intervalY; y++)
			{
				if (obstacle[x, y] == false)
				{
					if (closeList[x, y] != 0)
					{
						closeList[x, y] = 0;
					}
				}
			}
		}
	}

    void CheckObstacle()
	{
		for (int x = 0; x < (int)intervalX - 1; x++)
		{
			obstacle[x, 0] = true;
			obstacle[x, (int)intervalY - 1] = true;
			closeList[x, 0] = 2;
			closeList[x, (int)intervalY - 1] = 2;
		}
		for (int y = 0; y < (int)intervalY - 1; y++)
		{
			obstacle[0, y] = true;
			obstacle[(int)intervalX - 1, y] = true;
			closeList[0, y] = 2;
			closeList[(int)intervalX - 1, y] = 2;
		}
		
		for (int x = 0; x < (int)intervalX; x++)
		{
			for (int y = 0; y < (int)intervalY; y++)
			{
				if (Physics.CheckSphere(new Vector3(transform.position.x - greedSize.x * 0.5f + x * intervalVec.x, 0, transform.position.z - greedSize.y * 0.5f + y * intervalVec.y), 0.5f, layer))
				{
					obstacle[x, y] = true;
					closeList[x, y] = 2;
				}
			}
		}
	}

	void Astar()
	{
		Vector2 currentPos = startPos;
		while (currentPos != endPos)
		{
			CostCurculation(currentPos);
			currentPos = FindMinValue();
		}
		
		while (currentPos != startPos)
		{
			currentPos = new Vector2(parentPosX[(int) currentPos.x, (int) currentPos.y], parentPosY[(int) currentPos.x, (int) currentPos.y]);
			closeList[(int) currentPos.x, (int) currentPos.y] = 3;
		}
	}

	void SetWayPoints()
	{
		Vector2 currentPos = endPos;

		Vector3[] waypoints = new Vector3[100];
		int i = 0;
		
		while (currentPos != startPos)
		{
			waypoints[i] = new Vector3(transform.position.x - greedSize.x * 0.5f + currentPos.x * intervalVec.x, 0, transform.position.z - greedSize.y * 0.5f + currentPos.y * intervalVec.y);
			currentPos = new Vector2(parentPosX[(int) currentPos.x, (int) currentPos.y], parentPosY[(int) currentPos.x, (int) currentPos.y]);
			i++;
		}
		
		player.GetComponent<PlayerMovement>().Move(waypoints, i - 1);
	}

	void Setting()
	{
		intervalX = greedSize.x / intervalVec.x;
		intervalY = greedSize.y / intervalVec.y;
		
		closeList = new int[(int)intervalX, (int)intervalY];
		parentPosX = new int[(int)intervalX, (int)intervalY];
		parentPosY = new int[(int)intervalX, (int)intervalY];
		obstacle = new bool[(int)intervalX, (int)intervalY];
		cityZone = new bool[(int)intervalX, (int)intervalY];
		
		startPosGiz = startPos;
		endPosGiz = endPos;
		
		startPos = new Vector2(startPos.x / intervalVec.x, startPos.y / intervalVec.y);
		endPos = new Vector2(endPos.x / intervalVec.x, endPos.y / intervalVec.y);

		f = new int[(int)intervalX, (int)intervalY];
		g = new int[(int)intervalX, (int)intervalY];
		h = new int[(int)intervalX, (int)intervalY];
		for (int x = 0; x < (int)intervalX; x++)
		{
			for (int y = 0; y < (int)intervalY; y++)
			{
				parentPosX[x, y] = -1;
				parentPosY[x, y] = -1;
			}
		}
	}

	void CostCurculation(Vector2 currentPos)
	{
		closeList[(int) currentPos.x, (int) currentPos.y] = 2;
		SetParent(currentPos);
		Set_F_G_H(currentPos);
	}

	Vector2 FindMinValue()
	{
		int min = 1000000000;
		int minX = 1000000000;
		int minY = 1000000000;
		
		Vector2 parent = Vector2.zero;
		for (int x = 0; x < (int)intervalX; x++)
		{
			for (int y = 0; y < (int)intervalY; y++)
			{
				if (closeList[x, y] == 1)
				{
					if (f[x, y] < min)
					{
						parent = new Vector2(parentPosX[x, y], parentPosY[x, y]);
						if (x - parent.x != 0 && y - parent.y != 0)
						{
							
							if (closeList[x, (int)parent.y] != 2 || closeList[(int)parent.x, y] != 2)
							{
								min = f[x, y];
								minX = x;
								minY = y;
							}
							else
							{
								if (endPos == new Vector2(x, y))
								{
									parentPosX[x, y] = -1;
									parentPosY[x, y] = -1;
									closeList[x, y] = 0;
								}
							}
						}
						else
						{
							min = f[x, y];
							minX = x;
							minY = y;
						}
					}
				}
			}
		}
		return new Vector2(minX, minY);
	}

	void SetParent(Vector2 currentPos)//현재 타일 주면의 8개의 타일의 부모 정하기
	{
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				if (closeList[(int) currentPos.x + x, (int) currentPos.y + y] == 0)//아직 한번도 확인이 안되었고
				{
					parentPosX[(int) currentPos.x + x, (int) currentPos.y + y] = (int)currentPos.x;//검사한 타일의 부모를 현재 타일로 설정
					parentPosY[(int) currentPos.x + x, (int) currentPos.y + y] = (int)currentPos.y;
						
					closeList[(int) currentPos.x + x, (int) currentPos.y + y] = 1;
				}
				else if (closeList[(int) currentPos.x + x, (int) currentPos.y + y] == 1)//이미 열린 목록에 있으면
				{
					int firstGValue = g[(int) currentPos.x + x, (int) currentPos.y + y];
						
					G(currentPos, new Vector2(currentPos.x + x, currentPos.y + y));
					int	secondGValue = g[(int) currentPos.x + x, (int) currentPos.y + y] + g[(int) currentPos.x, (int) currentPos.y];
					
					if (firstGValue > secondGValue)
					{
						g[(int) currentPos.x + x, (int) currentPos.y + y] = secondGValue;
						parentPosX[(int) currentPos.x + x, (int) currentPos.y + y] = (int)currentPos.x;
						parentPosY[(int) currentPos.x + x, (int) currentPos.y + y] = (int)currentPos.y;
					}
					else
					{
						g[(int) currentPos.x + x, (int) currentPos.y + y] = firstGValue;
					}
				}
			}
		}
	}

	void Set_F_G_H(Vector2 currentPos)
	{
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				G(new Vector2(currentPos.x + x, currentPos.y + y));
				H(new Vector2(currentPos.x + x, currentPos.y + y));
				F(new Vector2(currentPos.x + x, currentPos.y + y));
			}
		}
	}

	void F(Vector2 currentPos)
	{
		f[(int) currentPos.x, (int) currentPos.y] = g[(int) currentPos.x, (int) currentPos.y] + h[(int) currentPos.x, (int) currentPos.y];
	}
	
	void G(Vector2 start, Vector2 currentPos)
	{
		if (closeList[(int) currentPos.x, (int) currentPos.y] != 2)
		{
			int cost = 0;
			Vector2 currentPos1 = currentPos;
			if (currentPos1.x - start.x == 0 ||
			    currentPos1.y - start.y == 0)
			{
				cost += 10;
			}
			else
			{
				cost += 14;
			}

			g[(int) currentPos.x, (int) currentPos.y] = cost;
		}
	}

	void G(Vector2 currentPos)
	{
		if (closeList[(int) currentPos.x, (int) currentPos.y] != 2)
		{
			if (parentPosX[(int) currentPos.x, (int) currentPos.y] != -1)
			{
				int cost = 0;
                Vector2 currentPos1 = currentPos;
                while (currentPos1 != startPos)
                {   
	                
					if (currentPos1.x - parentPosX[(int) currentPos1.x, (int) currentPos1.y] == 0 ||
                		currentPos1.y - parentPosY[(int) currentPos1.x, (int) currentPos1.y] == 0)
					{
                		cost += 10;
					}
					else
					{
                		cost += 14;
					}
					currentPos1 = new Vector2(parentPosX[(int) currentPos1.x, (int) currentPos1.y], parentPosY[(int) currentPos1.x, (int) currentPos1.y]);
				}
				g[(int) currentPos.x, (int) currentPos.y] = cost;
			}
			
		}
	}

	void H(Vector2 currentPos)
	{
		int x = Mathf.Abs((int)endPos.x - (int)currentPos.x);
		int y = Mathf.Abs((int)endPos.y - (int)currentPos.y);
		
		h[(int) currentPos.x, (int) currentPos.y] = (x + y) * 10;
	}

	public void ChangeCamera()
	{
		
		if (isMiniMapCamera)
		{
			
			shipCamera.GetComponent<Camera>().depth = 1;
			isMiniMapCamera = false;
		}
		else
		{
			
			shipCamera.GetComponent<Camera>().depth = -1;
			isMiniMapCamera = true;
		}
	}

	private void OnDrawGizmos()
	{
		if (closeList != null)
		{
			for (int x = 0; x < (int)intervalX; x++)
			{
				for (int y = 0; y < (int)intervalY; y++)
				{
					if(closeList[x, y] == 0) Gizmos.color = Color.white;//확인 안된 목록
					else if(closeList[x, y] == 1) Gizmos.color = Color.yellow;//열린목록
					else if(closeList[x, y] == 2) Gizmos.color = Color.blue;//닫힌 목록
					else if (closeList[x, y] == 3) Gizmos.color = Color.magenta;
					if (cityZone[x, y] == true) Gizmos.color = Color.cyan;
					if(obstacle[x, y] == true) Gizmos.color = Color.black;
					
					if(x == startPos.x && y == startPos.y) Gizmos.color = Color.green;//시작지점
					if(x == endPos.x && y == endPos.y) Gizmos.color = Color.red;//도착지점
					Gizmos.DrawCube(new Vector3(transform.position.x - greedSize.x * 0.5f + x * intervalVec.x, 0, transform.position.z - greedSize.y * 0.5f + y * intervalVec.y), Vector3.one * 10);//기즈모 그리기
				}
			}
		}
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(transform.position, new Vector3(greedSize.x, 1, greedSize.y));
		Gizmos.color = Color.green;
		Gizmos.DrawCube(new Vector3(transform.position.x - greedSize.x * 0.5f + startPosGiz.x, 0, transform.position.z - greedSize.y * 0.5f + startPosGiz.y) , Vector3.one * 100);
		Gizmos.color = Color.red;
		Gizmos.DrawCube(new Vector3(transform.position.x - greedSize.x * 0.5f + endPosGiz.x, 0, transform.position.z - greedSize.y * 0.5f + endPosGiz.y) , Vector3.one * 100);
		
		
	}
	
    Vector3 FindObjectPosition()
    {
        Vector3 mouseWorldPos = minimapCamera.ScreenToWorldPoint(Input.mousePosition);
        return mouseWorldPos;
    }

	void FindCityZone()
	{
		for (int x = 0; x < (int)intervalX; x++)
		{
			for (int y = 0; y < (int)intervalY; y++)
			{
				if (obstacle[x, y] == true)
				{
					for (int x1 = x - radius; x1 < x + radius + 1; x1++)
					{
						for (int y1 = y - radius; y1 < y + radius + 1; y1++)
						{
							if (x1 > 0 && x1 < intervalX)
							{
								if (y1 > 0 && y1 < intervalY)
								{
									if (obstacle[x1, y1] == false)
									{
										cityZone[x1, y1] = true;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
