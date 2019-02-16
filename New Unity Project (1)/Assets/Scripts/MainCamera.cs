using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	public float mouseSpeed;//마우스 속도
	public float destanceFromTarget;//타겟과의 거리
	public Transform target;//바라볼 타겟
	public Vector2 pitchMinMax;//마우스 좌표 최소치, 최대치
	public bool isScroll;

	float yaw;//마우스 y각도
	float pitch;//마우스 x각도

	private Camera cam;
	
	Vector3 afterMousePos;

	void Start()
	{
		cam = GetComponent<Camera>();
	}

	void Update()
	{
		CameraMovement();//카메라 이동
		CreateObject();//오브젝트 설치 혹은 제거
		if(isScroll) MouseWheel();//마우스 휠로 줌아웃, 줌인
	}

	void CameraMovement()//카메라 이동
	{
		transform.position = target.position - transform.forward * destanceFromTarget;//카메라의 position설정
		if (Input.GetMouseButton(0))//마우스의 0번 버튼이 눌려져 있으면
		{
			yaw += Input.GetAxis("Mouse X") * mouseSpeed;
			pitch -= Input.GetAxis("Mouse Y") * mouseSpeed;
			pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

			transform.eulerAngles = new Vector3(pitch, yaw);
			
			transform.position = target.position - transform.forward * destanceFromTarget;//카메라의 position설정
		}
	}

	void MouseWheel()//마우스 휠로 줌인, 줌아웃
	{
		if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (destanceFromTarget + 0.3f > 10)
			{
				destanceFromTarget = 10;
			}
			else
			{
				destanceFromTarget += 0.3f;
			}
		} 
		
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{ 
			if (destanceFromTarget - 0.3f < 3.3f)
			{
				destanceFromTarget = 3.3f;
			}
			else
			{
				destanceFromTarget -= 0.3f;
			}
		} 
	}

	void CreateObject()//오브젝트 설치
	{
		if (Input.GetMouseButtonDown(0))//마우스 0번 버튼 누르면
		{
			afterMousePos = Input.mousePosition;
		}
		
		if (Input.GetMouseButtonUp(0))//마우스 0번 버튼 올리면
		{
			if(Vector3.Distance(afterMousePos, Input.mousePosition) <= 20)//누를 때의 마우스 좌표와 올릴 때의 마우스 좌표가 같으면
			{
				Ray ray = cam.ScreenPointToRay(Input.mousePosition);//RaycastHit을 쓸 방향 결정 Ray
				RaycastHit hit;
				if (Physics.Raycast(transform.position, ray.direction, out hit, 20))//ray를 쏴서 오브젝트가 맞으면
				{
					if (hit.collider.gameObject.tag == "Seat")//ray에 맞은 오브젝트의 태그가 Seat이면
					{
						hit.collider.gameObject.GetComponent<Seat>().CreateOrRemove(0);//오브젝트를 Seat가 생성, 이미 생성했으면 제거
					}
				}
			}
		}
	}
}
