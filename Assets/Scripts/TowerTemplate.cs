using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;          // 타워 생성을 위한 프리팹
    public GameObject followTowerPrefab;    // 임시 타워 프리팹
    public Weapon[] weapon;                 // 레벨별 타워(무기) 정보

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite; // 보여지는 타워 이미지(UI)
        public string name;  
        public float damage; 
        public float slow;    
        public float buff;    
        public float rate;    
        public float range; 
        public int point; 
        public int sell; 
    }
}
