using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDataViewer : MonoBehaviour
{
    // Ÿ�� ���� ��� UI
    [SerializeField]
    private Image imageTower;
    [SerializeField]
    private TextMeshProUGUI textDamage;
    [SerializeField]
    private TextMeshProUGUI textRate;
    [SerializeField]
    private TextMeshProUGUI textRange;
    [SerializeField]
    private TextMeshProUGUI textLevel;
    [SerializeField]
    private TextMeshProUGUI textUpgradePoint;
    [SerializeField]
    private TextMeshProUGUI textSellPoint;

    private TowerWeapon currentTower;   // ���� Ÿ���� ������ ������ ����

    [SerializeField]
    private TowerAttackRange towerAttackRange;

    [SerializeField]
    private Button buttonUpgrade;

    [SerializeField]
    private SystemTextViewer systemTextViewer;

    private void Awake()
    {
        OffPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffPanel();
        }
    }

    public void OnPanel(Transform towerWeapon)
    {
        // ����ؾ��ϴ� Ÿ�� ������ �޾ƿͼ� ����
        currentTower = towerWeapon.GetComponent<TowerWeapon>();

        // Ÿ�� ���� Panel On
        gameObject.SetActive(true);

        // Ÿ�� ������ ����
        UpdateTowerData();

        // Ÿ�� ������Ʈ �ֺ��� ǥ�õǴ� Ÿ�� ���ݹ��� Sprite On
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }

    public void OffPanel()
    {
        // Ÿ�� ���� Panel Off
        gameObject.SetActive(false);

        // Ÿ�� ���� ���� Sprite On
        towerAttackRange.OffAttackRange();
    }

    // Ÿ�� Ŭ���� ����, ������Ʈ
    private void UpdateTowerData()
    {
        if ( currentTower.WeaponType == WeaponType.Cannon || currentTower.WeaponType == WeaponType.Laser)
        {
            imageTower.rectTransform.sizeDelta = new Vector3(180, 180);
            textDamage.text = "Damage : " + currentTower.Damage
                            + " + " + "<color=red>" + currentTower.AddedDamage.ToString("F1") + "</color>";
        }
        else
        {
            imageTower.rectTransform.sizeDelta = new Vector2(180, 180);

            if ( currentTower.WeaponType == WeaponType.Slow )
            {
                textDamage.text = "Slow : " + currentTower.Slow * 100 + "%";
            }
            else if ( currentTower.WeaponType == WeaponType.Buff )
            {
                textDamage.text = "Buff : " + currentTower.Buff * 100 + "%";
            }
        }

        imageTower.sprite = currentTower.TowerSprite;
        textRate.text = "Rate : " + currentTower.Rate;
        textRange.text = "Range : " + currentTower.Range;
        textLevel.text = "Level : " + currentTower.Level;
        textUpgradePoint.text = currentTower.UpgradePoint.ToString();
        textSellPoint.text = currentTower.SellPoint.ToString();

        // ���׷��̵尡 �Ұ��������� ��ư ��Ȱ��ȭ
        buttonUpgrade.interactable = currentTower.Level < currentTower.MaxLevel ? true : false;
    }

    public void OnClickEventTowerUpgrade()
    {
        // Ÿ�� ���׷��̵� �õ� (����:true, ����:false)
        bool isSuccess = currentTower.Upgrade();
        if (isSuccess == true)
        {
            // Ÿ�� ���׷��̵� �Ǿ��� ������ Ÿ�� ���� ����
            UpdateTowerData();

            // Ÿ�� �ֺ��� ���̴� ���ݹ����� ����
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            // Ÿ�� ���׷��̵忡 �ʿ��� ����� �����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Money);
        }
    }

    public void OnClickEventTowerSell()
    {
        // Ÿ�� �Ǹ�
        currentTower.Sell();

        // ������ Ÿ���� ������� Panel, ���ݹ��� off
        OffPanel();
    }
  
}
