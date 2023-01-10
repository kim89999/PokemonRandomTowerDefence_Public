using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgradeCondition : MonoBehaviour
{
    public GameObject[] towers;
    public TowerWeapon towerweapon;
    public string name;
    public List<GameObject> TowerList = new List<GameObject>();
    public int mCnt, pCnt = 0;

    [SerializeField]
    public TowerWeapon currentTower;   // 현재 타워의 정보를 가져와 저장 

    public void OnCondition(Transform towerWeapon)
    {
        // 출력해야하는 타워 정보를 받아와서 저장
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
    }

    public void towerInit()
    {
        for (int i = 0; i < towers.Length; i++)
        {
            towerweapon = towers[i].GetComponent<TowerWeapon>();
            name = towerweapon.Name;

            if (towers[i].ToString().Contains("Magikarp") && name.Contains("잉어킹"))
            {
                Debug.Log(name.GetType());
                TowerList.Add(towers[i]);
                mCnt++;
            }
            else if (towers[i].ToString().Contains("Pikachu") && name.Contains("피카츄"))
            {
                Debug.Log(name);
                TowerList.Add(towers[i]);
                pCnt++;
            }
        }
    }

    public void listReset()
    {
        mCnt = 0;
        pCnt = 0;
        TowerList.Clear();
    }

    public void removeTower1()
    {
        int cnt = 0;
        for (int i = 0; i < TowerList.Count; i++)
        {
            towerweapon = TowerList[i].GetComponent<TowerWeapon>();
            name = towerweapon.Name;

            if (TowerList[i].ToString().Contains("Magikarp") && name.Contains("잉어킹"))
            {
                if (TowerList[i] != currentTower)
                {
                    towerweapon.ownerTile.IsBuildTower = false;
                    Destroy(TowerList[i]);
                    cnt++;
                }
            }

            if (cnt == 2)
            {
                break;
            }
        }
    }

    public void removeTower2()
    {
        int cnt = 0;
        for (int i = 0; i < TowerList.Count; i++)
        {
            towerweapon = TowerList[i].GetComponent<TowerWeapon>();
            name = towerweapon.Name;

            if (TowerList[i].ToString().Contains("Pikachu") && name.Contains("피카츄"))
            {
                if (TowerList[i] != currentTower)
                {
                    towerweapon.ownerTile.IsBuildTower = false;
                    Destroy(TowerList[i]);
                    cnt++;
                }
            }

            if (cnt == 2)
            {
                break;
            }
        }
    }

    void Update()
    {
        towers = GameObject.FindGameObjectsWithTag("Tower");
    }


}