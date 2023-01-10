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
    private Transform hitTransform = null;  // ���콺 ��ŷ���� ������ ������Ʈ �ӽ� ����

    private void Awake()
    {
        // "MainCamera" �±׸� ���� �ִ� ������Ʈ Ž�� �� Camera ������Ʈ ���� ����
        // GameObject.FindGameObjectWithTag("MainCamere").GetComponent<Camera>(); �� ����
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        // ���콺 ���� ��ư�� ������ ��
        if(Input.GetMouseButtonDown(0))
        {
            // ī�޶� ��ġ���� ȭ���� ���콺 ��ġ�� �����ϴ� ���� ����
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 2D ����͸� ���� 3D ������ ������Ʈ�� ���콺�� �����ϴ� ���
            // ������ �ε��ϴ� ������Ʈ�� ����ؼ� hit�� ����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hitTransform = hit.transform;

                // ������ �ε��� ������Ʈ�� �±װ� "tile"�̸�
                if (hit.transform.CompareTag("Tile"))
                {
                    // Ÿ���� �����ϴ� SpawnTower() ȣ��
                    towerSpawner.SpawnTower(hit.transform);
                }

                // Ÿ���� �����ϸ� �ش� Ÿ�� ������ ����ϴ� Ÿ�� ����â On
                else if (hit.transform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPanel(hit.transform);
                    towerupgradecondition.OnCondition(hit.transform);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // ���콺�� ������ �� ������ ������Ʈ�� �ְų� ������ ������Ʈ�� Ÿ���� �ƴϸ�
            if(hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                //Ÿ�� ���� �г��� ��Ȱ��ȭ �Ѵ�
                towerDataViewer.OffPanel();
            }
            hitTransform = null;
        }
    }
}

/*
 * File : ObjectDetector
 * Desc
 *  : Raycast()�� �̿��� ���콺 Ŭ�� ��ġ�� Ÿ�� ����
 */