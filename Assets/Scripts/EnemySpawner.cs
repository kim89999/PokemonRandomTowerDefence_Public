using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyHPSliderPrefab;     // �� ü���� ��Ÿ���� Slider UI ������

    [SerializeField]
    private Transform canvasTransform;          // UI�� ǥ���ϴ� Canvas ������Ʈ�� Transform

    [SerializeField]
    private Transform[] wayPoints;              // ���� ���������� �̵� ���

    [SerializeField]
    private PlayerHP playerHP;                  // �÷��̾��� ü�� ������Ʈ

    [SerializeField]
    private PlayerPoint playerPoint;            // �÷��̾��� ����Ʈ ������Ʈ
    private Wave currentWave;                   // ���� ���̺� ����

    private List<Enemy> enemyList;              //���� �ʿ� �����ϴ� ��� ���� ����

    private StartMusic startmusic;

    // ���� ������ ������ EnemySpawner���� �ϱ� ������ Set�� �ʿ� ����.
    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        // �� ����Ʈ �޸� �Ҵ�
        enemyList = new List<Enemy>();

        // �� ���� �ڷ�ƾ �Լ� ȣ��
        //StartCoroutine("SpawnEnemy");

        startmusic = GameObject.Find("BackGroundMusic").GetComponent<StartMusic>();
    }

    public void StartWave(Wave wave)
    {
        // �Ű������� �޾ƿ� ���̺� ���� ����
        currentWave = wave;

        // ���� ���̺� ����
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        // ���� ���̺꿡�� ������ �� ����
        int spawnEnemyCount = 0;

        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // ���̺꿡 �����ϴ� ���� ������ ���� ������ �� ������ ���� �����ϵ��� �����ϰ�, �� ������Ʈ ����

            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();      // ��� ������ ���� Enemy ������Ʈ

            // this�� �� �ڽ� (�ڽ��� EnemySpawner ����)
            enemy.Setup(this, wayPoints);                   // wayPoint ������ �Ű������� Setup() ȣ��
            enemyList.Add(enemy);                           // ����Ʈ�� ��� ������ �� ���� ����

            SpawnEnemyHPSlider(clone);                      // �� ü���� ��Ÿ���� Slider UI ���� �� ����

            // ���� ���̺꿡�� ������ ���� ���� +1
            spawnEnemyCount++;
            // �� ���̺긶�� spawnTime�� �ٸ� �� �ֱ� ������ ���� ���̺�(currentWave)�� spawnTime ���
            yield return new WaitForSeconds(currentWave.spawnTime); // spawnTime �ð� ���� ���

        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int point)
    {
        // ���� ��ǥ�������� �������� ��
        if(type == EnemyDestroyType.Arrive)
        {
            // �� ������ ������ �� ���ӿ���
            GameObject Boss = GameObject.FindWithTag("Enemy");
            if (Boss.GetComponent<Enemy>().boss == true)
            {
                // ���� ����
                SceneManager.LoadScene("GameOver");
            }

            // �÷��̾��� ü�� -1
            playerHP.TakeDamage(1);
        }
        // ���� �÷��̾��� �߻�ü���� ������� ��
        else if ( type == EnemyDestroyType.Kill)
        {
            // ���� ������ ���� ��� �� ����Ʈ ȹ��
            playerPoint.CurrentPoint += point;

            // �� ������ ���� �� �������� Ŭ����
            GameObject Boss = GameObject.FindWithTag("Enemy");
            if (Boss.GetComponent<Enemy>().boss == true)
            {
                // �������� Ŭ����
                SceneManager.LoadScene("StageClear");
                startmusic.stagenum++;
            }

        }

        // ����Ʈ���� ����ϴ� �� ���� ����
        enemyList.Remove(enemy);

        // �� ������Ʈ ����
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        // �� ü���� ��Ÿ���� Slider UI ����
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        sliderClone.transform.SetParent(canvasTransform);

        // ���� �������� �ٲ� ũ�⸦ �ٽ� (1, 1, 1)�� ����
        sliderClone.transform.localScale = Vector3.one;

        // Slider UI�� �Ѿƴٴ� ����� �������� ����
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);

        // Slider UI�� �ڽ��� ü�� ������ ǥ���ϵ��� ����
        sliderClone.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}