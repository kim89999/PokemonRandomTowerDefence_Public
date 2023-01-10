using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerHP;           // Text - TextMeshPro UI [플레이어 체력]

    [SerializeField]
    private TextMeshProUGUI textPlayerPoint;        // Text - TextMeshPro UI [플레이어의 포인트]

    [SerializeField]
    private PlayerHP playerHP;                      // 플레이어의 체력 정보

    [SerializeField]
    private PlayerPoint playerPoint;                // 플레이어의 포인트 정보

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP + "/" + playerHP.MaxHP;
        textPlayerPoint.text = playerPoint.CurrentPoint.ToString();
    }
        
}

/*
 * Flie : TextTMPViewer.cs
 * Desc
 *  : Text-TextMeshPro UI로 표현되는 여러 정보 업데이트
 */
