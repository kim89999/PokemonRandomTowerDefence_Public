using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageOpen : MonoBehaviour
{
    public int maxstagenum;     // 현재 스테이지

    [SerializeField]
    private Button buttonStage;

    [SerializeField]
    private StartMusic startmusic;

    // Start is called before the first frame update
    void Start()
    {
        startmusic = GameObject.Find("BackGroundMusic").GetComponent<StartMusic>();
    }

    // Update is called once per frame
    void Update()
    {
        // 스테이지 클리어 시 활성화
        buttonStage.interactable = startmusic.stagenum >= maxstagenum ? true : false;
    }
}
