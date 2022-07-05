using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGold : MonoBehaviour
{
    public IEnumerator startGetGold()
    {
        PlayManager pm = PlayManager.Instans; 
        float rot = Random.Range(0, 2 * Mathf.PI);
        Vector3 vTemp = 2.5f*Vector3.right;
        vTemp = new Vector3(vTemp.x * Mathf.Cos(rot) - vTemp.y * Mathf.Sin(rot)
            , vTemp.x * Mathf.Sin(rot) + vTemp.y * Mathf.Cos(rot));
        vTemp += transform.position;
        for (int i = 0; i < 35; ++i)
        {
            transform.position = Vector3.Lerp(transform.position, vTemp, 0.1f);
            yield return pm.fixWs;
        }
        yield return new WaitForSeconds(0.3f);
        vTemp = pm.player.transform.position + Vector3.up * 5.5f;
        while ((transform.position - vTemp).sqrMagnitude > 1f)
        {
            transform.position = Vector3.Lerp(transform.position, vTemp, 0.1f);
            yield return pm.fixWs;
        }
        pm.goldPlus(1);
        this.gameObject.SetActive(false);
        pm.getGolds.Add(gameObject);
    }
}
