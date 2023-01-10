using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchSceneChange : MonoBehaviour
{
    public string scenename;    // Scene 이름 설정

    private GameObject Music;

    public void SceneChange()
    {
        Debug.Log("SceneChange!!");
        SceneManager.LoadScene(scenename);
    }

    // Start is called before the first frame update
    void Start()
    {
        //UI게임오브젝트 검색
        Music = GameObject.Find("BackGroundMusic");
    }

    public void btnClick()
    {
        GameObject.Destroy(Music);
        Debug.Log("Music OFF");
    }
}
