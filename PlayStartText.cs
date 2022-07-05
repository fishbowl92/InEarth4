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
        string sTemp = (gm.stageNum + 1) + " �ܰ� : " + gm.levCorrection(gm.stageNum);

        sTemp += "\n\n10���� ������ 5���� ������ ���� �����ϴ�.\n\n";

        switch (gm.seed[2])
        {
            case 1:
                sTemp += "�� ��¼�ھ��.";
                break;
            case 2:
                sTemp += "���ư��� ����ҷ���.";
                break;
            default:
                sTemp += "����� �׷�����.";
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
