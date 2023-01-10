using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{

    public Slider BackGroundMusic;

    // Start is called before the first frame update
    private void Start()
    {
        if(DataController.Instance.gameData.BGMSound == 0)
        {
            Debug.Log("vlaue=0");
            BackGroundMusic.value = 0;
        }
        else if(DataController.Instance.gameData.BGMSound == 1)
        {
            Debug.Log("vlaue=1");
            BackGroundMusic.value = 1;
        }
    }

}
