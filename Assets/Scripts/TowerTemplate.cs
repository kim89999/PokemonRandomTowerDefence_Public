using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;          // Ÿ�� ������ ���� ������
    public GameObject followTowerPrefab;    // �ӽ� Ÿ�� ������
    public Weapon[] weapon;                 // ������ Ÿ��(����) ����

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite; // �������� Ÿ�� �̹���(UI)
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
