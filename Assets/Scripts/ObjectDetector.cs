using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{

    [SerializeField]
    private TowerSpawner towerSpawner;

    [SerializeField]
    private TowerDataViewer towerDataViewer;

    [SerializeField]
    private TowerUpgradeCondition towerupgradecondition;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null;  // 마우스 픽킹으로 선택한 오브젝트 임시 저장

    private void Awake()
    {
        // "MainCamera" 태그를 갖고 있는 오브젝트 탐색 후 Camera 컴포넌트 정보 전달
        // GameObject.FindGameObjectWithTag("MainCamere").GetComponent<Camera>(); 와 동일
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if(Input.GetMouseButtonDown(0))
        {
            // 카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생성
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 2D 모니터를 통해 3D 월드의 오브젝트를 마우스로 선택하는 방법
            // 광선에 부딪하는 오브젝트를 경솔해서 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                // 광선에 부딪힌 오브젝트에 태그가 "tile"이면
                if (hit.transform.CompareTag("Tile"))
                {
                    // 타워를 생성하는 SpawnTower() 호출
                    towerSpawner.SpawnTower(hit.transform);
                }

                // 타워를 선택하면 해당 타워 정보를 출력하는 타워 정보창 On
                else if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                    towerupgradecondition.OnCondition(hit.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스를 눌렀을 때 선택한 오브젝트가 있거나 선택한 오브젝트가 타워가 아니면
            if(hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                //타워 정보 패널을 비활성화 한다
                towerDataViewer.OffPanel();
            }
            hitTransform = null;
        }
    }
}

/*
 * File : ObjectDetector
 * Desc
 *  : Raycast()를 이용한 마우스 클릭 위치에 타워 생성
 */