using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageChange : MonoBehaviour
{
    public Image this_img;      // 기존에 존재하는 이미지
    public Sprite[] Change_img = new Sprite[2];   // 바뀌어질 이미지
    int cnt = 0;
    int i = 0;

    public void ChangeImage()
    {
        if(i < 3)
        {
            this_img.sprite = Change_img[i];
            i += 1;
        }

        cnt += 1;

        if (cnt == 4)
        {
            SceneManager.LoadScene("GameStart");
        }
    }

}
