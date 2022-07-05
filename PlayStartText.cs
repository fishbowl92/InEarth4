using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayStartText : MonoBehaviour
{
    public GameManager gm;

    public void Start()
    {
        gm = GameManager.Instans;
        StartCoroutine(showText());
    }
    IEnumerator showText()
    {
        WaitForSeconds ws = new WaitForSeconds(0.03f);
        string sTemp = (gm.stageNum + 1) + " 단계 : " + gm.levCorrection(gm.stageNum);

        sTemp += "\n\n10개의 무리와 5명의 강적이 길을 막습니다.\n\n";

        switch (gm.seed[2])
        {
            case 1:
                sTemp += "뭐 어쩌겠어요.";
                break;
            case 2:
                sTemp += "돌아가면 고백할래요.";
                break;
            default:
                sTemp += "결단은 그런거죠.";
                break;
        }
        TextMeshProUGUI text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        text.text = "";

        for (int i = 0; i < sTemp.Length; ++i)
        {
            text.text += sTemp[i];
            if (sTemp[i] != ' ' && sTemp[i] != '\n') gm.btnSound[2].Play();
            else yield return null;
            yield return ws;
        }
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(false);
    }
}
