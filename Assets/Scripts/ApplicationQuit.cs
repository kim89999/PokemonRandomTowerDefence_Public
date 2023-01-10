using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("EXIT");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
            Application.Quit(); // 어플리케이션 종료
#endif

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
