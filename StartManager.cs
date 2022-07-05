using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartManager : MonoBehaviour
{
    public RectTransform nowPage;
    public int nowPageNum;
    public bool startChange;
    public RectTransform[] fullPage;
    public GameManager gm;
    public void changePenal(int a)
    {
        if (startChange) return;
        int gab = nowPageNum + a;
        if (gab < 0 || gab >= fullPage.Length) return;
        nowPageNum = gab;
        startChange = true;
        StartCoroutine(pageChanger(a > 0));
    }
    IEnumerator pageChanger(bool right)
    {
        Vector3 endPoint = (!right) ? new Vector3(1650, 0, 0) : new Vector3(-1650, 0, 0);
        RectTransform t = fullPage[nowPageNum];
        t.anchoredPosition = -endPoint;
        while (Mathf.Abs(t.anchoredPosition.x) > 5)
        {
            t.anchoredPosition = Vector3.Lerp(t.anchoredPosition, Vector3.zero, 5 * Time.deltaTime);
            nowPage.anchoredPosition = Vector3.Lerp(nowPage.anchoredPosition, endPoint,  5 * Time.deltaTime);
            yield return null;
        }
        nowPage.anchoredPosition = endPoint;
        t.anchoredPosition = Vector3.zero;
        nowPage = t;

        startChange = false;
    }
    public void startGame()
    {
        LoadingSceneManager.LoadScene("Play");
    }

    public void Start()
    {
        selectedEquipNum = -1;
        selectedInvenItemNum = -1;
        initShopItems();
        initPlayerEquipment();
        initPlayerInventory();
    }
    public GameObject skillUISetPenel;

    public GameObject magicPrefab;
    public GameObject contentOfPlayerHaveSkill;


    public GameObject magicStatePenel;

    private int selectedMagicNum;

    private List<GameObject> maigcObjectPooling = new List<GameObject>();

    public void openMagicTap(GameObject penel)
    {
        penel.SetActive(true);
        settingMagicStateUI();
    }
    public void closeMagicTap(GameObject penel)
    {
        penel.SetActive(false);
        magicStatePenel.SetActive(false);
        magicPenelRelease();
    }
    private GameObject prebSelect;
    public void OnClickSKillUIPrefab(GameObject go)
    {
        if (prebSelect != null && prebSelect == go && magicStatePenel.activeSelf)
        {
            magicStatePenel.SetActive(false);
            return;
        }
        prebSelect = go;
        gm.release();
        selectedMagicNum = (int)go.transform.position.z;
        skillData sd = gm.playerHaveSkill[selectedMagicNum];
        magicStatePenel.SetActive(true);
        skillUISetPenel.transform.GetChild(0).GetComponent<Image>().sprite = sd.skillImage;
        skillUISetPenel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Lev." + sd.level + " " + sd.name;
        skillUISetPenel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = sd.info;
        gm.drawSkillPattern(sd.pattern, skillUISetPenel.transform.GetChild(3).gameObject);
    }
    public void settingMagicStateUI()
    {
        GameObject skill = null;
        int check = 0;
        for(int i = 0; i < gm.playerHaveSkill.Count; ++i)
        {
            foreach(var sk in maigcObjectPooling)
            {
                if (!sk.activeInHierarchy)
                {
                    skill = sk;
                    check = 1;
                }
            }
            if(check == 0)
            {
                skill = Instantiate(magicPrefab, contentOfPlayerHaveSkill.transform, false);
                maigcObjectPooling.Add(skill);
            }
            check = 0;
            skill.transform.GetChild(0).GetComponent<Image>().sprite = gm.playerHaveSkill[i].skillImage;
            skill.transform.position = new Vector3(skill.transform.position.x, skill.transform.position.y, i + 0.3f);
            skill.transform.SetAsLastSibling();
            skill.SetActive(true);
        }
    }
    public void magicPenelRelease()
    {
        for (int i = 0; i < maigcObjectPooling.Count; ++i)
        {
            maigcObjectPooling[i].SetActive(false);
        }
    }
    public void OnClickMagicLevelUp()
    {
        skillData sd = gm.playerHaveSkill[selectedMagicNum];

        // �ش� ��ų�������� �÷��̾� ������ȭ�� ��ų�� �䱸 ��ȭ �̻��� �����Ѵٸ�
        //if (sd.needExpForLevUp[sd.level] > )
        // { �÷��̾� ���� ��ȭ ����

        // ��ų ������ }

        skillUISetPenel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Lev." + sd.level + " " + sd.name;

    }
    public void OnClickEraseMagic()
    {
        // �ڽ�Ʈ or ��ų ���� ��ŭ ������ ����ġ �縸ŭ �÷��̾�� ��ȭ ����
        // �������� �ְ�

        magicStatePenel.SetActive(false);
        gm.playerHaveSkill.RemoveAt(selectedMagicNum);
        magicPenelRelease();
        settingMagicStateUI();
    }

    public GameObject shopItemPenel;
    public GameObject itemPopUpPenel;
    public List<ItemData> shopItems = new List<ItemData>();
    public int selectedShopItemNum;
    
    public void OnClickShopTap(GameObject penel)
    {
        penel.SetActive(true);
    }
    public void OnClickShopExitBtn(GameObject penel)
    {
        itemPopUpPenel.SetActive(false);
        penel.SetActive(false);
    }
    public void initShopItems()
    {
        int ran;
        shopItems.Clear();
        for (int i = 0; i < 6; ++i)
        {
            ran = Random.Range(0, gm.allItemData.Count);
            ItemData temp = gm.allItemData[ran];
            shopItems.Add(temp);

            Vector3 ts = shopItemPenel.transform.position;
            ts = new Vector3(ts.x, ts.y, 0.3f);

            shopItemPenel.transform.GetChild(i).GetChild(3).gameObject.SetActive(false);
            shopItemPenel.transform.GetChild(i).GetChild(2).GetComponent<Image>().sprite = temp.itemImage;
            shopItemPenel.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = temp.cost.ToString();
            shopItemPenel.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = temp.itemName + " +1";
        }
    }
    public void OnClickShopItem(GameObject item)
    {
        if ((int)shopItemPenel.transform.position.z == 1)
        {
            return;
        }
        itemPopUpPenel.SetActive(true);
        selectedShopItemNum = (int)item.transform.position.z;
        itemPopUpPenel.transform.GetChild(0).GetComponent<Image>().sprite = shopItems[selectedShopItemNum].itemImage;
        itemPopUpPenel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = shopItems[selectedShopItemNum].itemName + " +1";
        itemPopUpPenel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "���� ���� : " + shopItems[selectedShopItemNum].cost;
    }
    public void OnClickBuyItem()
    {
        // �κ��丮�� shopItems[selectedShopItemNum] �� �߰��ؼ� ������Ʈ
        Vector3 ts = shopItemPenel.transform.position;
        ts = new Vector3(ts.x, ts.y, 1.3f);

        shopItemPenel.transform.GetChild(selectedShopItemNum).GetChild(3).gameObject.SetActive(true);
        gm.inventory.Add(shopItems[selectedShopItemNum]);
        itemPopUpPenel.SetActive(false);
    }
    public void OnClickCancelItem()
    {
        itemPopUpPenel.SetActive(false);
    }
    public void OnClickShopRerollBtn()
    {
        initShopItems();
        itemPopUpPenel.SetActive(false);
    }
    
    public GameObject equipmentPenel;
    public GameObject contentOfInventory;
    public GameObject inventoryPopupPenel;
    public GameObject selectedItemPopupPenel;

    public GameObject inventoryItemPrefab;

    private int selectedEquipNum;    // ��ü�ϱ����� ���õ� ������ ���
    private int selectedInvenItemNum; // ��ü�� ���
    private List<GameObject> invenObjectPooling = new List<GameObject>();

    public GameObject itemOptionTextPrefab;

    private List<GameObject> selectedItemOptonsPooling = new List<GameObject>();

    public GameObject selectedItemOptons;

    public GameObject[] goHome; // Ȩ�� ������ �����ִ� ��� �г��� �־� Ȩ���� ���������� ���� ���

    public void OnClickEquipmentTap(GameObject penel)
    {
        penel.SetActive(true);
    }
    public void OnClickGoHomeBtn(GameObject penel)
    {
        for(int i =0;i<goHome.Length; ++i)
        {
            goHome[i].SetActive(false);
        }
    }
    public void initPlayerEquipment()
    {
        for(int i = 0; i < 4; ++i)
        {
            equipmentPenel.transform.GetChild(1).GetChild(i).GetChild(1).GetComponent<Image>().sprite = gm.playerEquipment[i].itemImage;
            equipmentPenel.transform.GetChild(1).GetChild(i).GetChild(2).GetComponent<Text>().text = "Lv." + gm.playerEquipment[i].level;
        }

        // ���â �߾� �÷��̾� �ȱ� ��� �����ϴ°� �ֱ�



    }
    public void initPlayerInventory()
    {
        GameObject item = null;
        int check = 0;
        for(int i = 0; i< gm.inventory.Count; ++i)
        {
            foreach (var temp in invenObjectPooling)
            {
                if ((int)temp.transform.GetChild(0).position.z<1)
                {
                    item = temp;
                    check = 1;
                    temp.transform.GetChild(0).position = new Vector3(0, 0, 1.3f);
                    Debug.Log("recycle");
                }
            }
            if(check == 0)
            {
                Debug.Log("sangsung");
                item = Instantiate(inventoryItemPrefab, contentOfInventory.transform, false);
                invenObjectPooling.Add(item);
            }
            check = 0;
            item.transform.GetChild(0).GetComponent<Image>().sprite = gm.inventory[i].itemImage;
            item.transform.GetChild(1).GetComponent<Text>().text = "Lv." + gm.inventory[i].level;
            item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, i + 0.3f);
            item.transform.GetChild(0).localPosition = new Vector3(0, 0, 1.3f);
            item.SetActive(true);
            item.transform.SetAsLastSibling();
        }
    }
    public void invenRelease()
    {
        for (int i = 0; i< invenObjectPooling.Count; ++i)
        {
            invenObjectPooling[i].SetActive(false);
            invenObjectPooling[i].transform.GetChild(0).position = new Vector3(0, 0, 1.3f);
        }
        for (int i = 0; i < selectedItemOptonsPooling.Count; ++i)
        {
            selectedItemOptonsPooling[i].SetActive(false);
        }
    }
    public void OnClickPlayerEquipment(GameObject equip)
    {
        // ���â���� �÷��̾ ����� �������� ���ý� �κ��丮 ����
        inventoryPopupPenel.SetActive(true);
        invenRelease();
        initPlayerInventory();
        selectedItemPopupPenel.SetActive(true);
        // ���� ������ ��� �Ǵٽ� ��� ��� ���� �������� �ʴ´�
        if (selectedEquipNum != (int)equip.transform.position.z)
        {
            selectedEquipNum = (int)equip.transform.position.z;

            selectedItemPopupPenel.transform.GetChild(0).GetComponent<Image>().sprite = gm.playerEquipment[selectedEquipNum].itemImage;
            selectedItemPopupPenel.transform.GetChild(1).GetComponent<Text>().text = "Lv." + gm.playerEquipment[selectedEquipNum].level;
            selectedItemPopupPenel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gm.playerEquipment[selectedEquipNum].name;
            initItemOptions(selectedItemOptonsPooling, itemOptionTextPrefab, selectedItemPopupPenel.transform.GetChild(3).gameObject, true);

        }
        else
        {
            // (����) ���� ���ýÿ� �� �� ��� ���ý� �κ��丮���� �����ִ� �������� �޶���Ѵ�.






            for (int i = 0; i< selectedItemOptonsPooling.Count; ++i)
            {
                selectedItemOptonsPooling[i].SetActive(true);
            }
            for (int i = 0; i < invenObjectPooling.Count; ++i)
            {
                invenObjectPooling[i].SetActive(true);
            }
        }
    }
    /*
    public void OnClickExitInventory()
    {
        selectedEquipPenel.SetActive(false);
        invenRelease();
        equipmentPenel.SetActive(true);
    }
    */
    public void initItemOptions(List<GameObject> pool, GameObject prefab, GameObject content, bool equiped = true)
    {
        GameObject option = null;
        int check = 0;  // pooling üũ��
        itemOptionRelease();    // �ɼ� �����ִ��͵� ��Ȱ��ȭ
        // ������ ����������, �κ� ���������� ����
        ItemData id = null;
        float gab = 0;
        if (equiped == true)
        {
            id = gm.playerEquipment[selectedEquipNum];
        }
        else
        {
            id = gm.inventory[selectedInvenItemNum];
        }

        // ������ �ɼ� ǥ�� ���پ�
        for (int i = 0; i < id.getState.Length; ++i)
        {   
            if (id.getState[i] == 0 && equiped)
            {
                // ������ �������� ������ 0�� ��� ǥ�� ���� ����
                continue;
            }
            else if(!equiped)
            {   // ��ȯ�� ���ϴ� �������� ������ ���� ���İ� 0�̸� ǥ������ �ʴ´�
                if (gm.playerEquipment[selectedEquipNum].getState[i] == 0 && id.getState[i] == 0)
                {
                    continue;
                }
            }


            foreach (var temp in pool)
            {
                if (!temp.activeInHierarchy)
                {
                    option = temp;
                    check = 1;
                }
            }
            if (check == 0)
            {
                option = Instantiate(prefab, content.transform, false);
                pool.Add(option);
            }
            check = 0;
            TextMeshProUGUI optionText = option.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            // ������ �ɼ� ǥ��
            if (equiped)
            {
                optionText.text = initItemStateName(optionText,i).text + id.getState[i].ToString();
            }
            else
            {
                gab = id.getState[i] - gm.playerEquipment[selectedEquipNum].getState[i];
                optionText.text = initItemStateName(optionText,i).text + id.getState[i].ToString();
                if (gab > 0)
                {   // ��ġ�� �÷��� �Ǹ� ��� ǥ��
                    optionText.text += "<color=#00ff00>" + " ( +" + gab + " )" + "</color>";
                }
                else if (gab < 0)
                {   // ��ġ�� ���̳ʽ� �Ǹ� ������ ǥ��
                    optionText.text += "<color=#ff0000>" + " ( " + gab + " )" + "</color>";
                }
            }
            option.SetActive(true);
            option.transform.SetAsLastSibling();
        }
    }
    public TextMeshProUGUI initItemStateName(TextMeshProUGUI txt, int num)
    {   // �������� ���� �̸��� �������� - ������ �ɼ� �߰��뵵�� ���
        switch (num)
        {   // �ӽ÷� ����
            case 0:
                txt.text = "���ݷ� : ";
                break;
            case 1:
                txt.text = "���� : ";
                break;
            case 2:
                txt.text = "�ִ�ü�� : ";
                break;
        }
        return txt;
    }

    public void OnClickInventoryItem(GameObject equip)
    {   // �κ��丮 �������� �����Ͽ� ���� ��񿡼� ������ ���������� ��ü�ϴ� â ����
        selectedInvenItemNum = (int)equip.transform.position.z;
        selectedItemPopupPenel.transform.GetChild(0).GetComponent<Image>().sprite = gm.inventory[selectedInvenItemNum].itemImage;
        selectedItemPopupPenel.transform.GetChild(1).GetComponent<Text>().text = "Lv." + gm.inventory[selectedInvenItemNum].level;
        selectedItemPopupPenel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gm.inventory[selectedInvenItemNum].name;

        // ��ü�ϴ� â�� ǥ���� �κ��丮���� ������ �������� �ɼ� ǥ��
        initItemOptions(selectedItemOptonsPooling, itemOptionTextPrefab, selectedItemPopupPenel.transform.GetChild(3).gameObject, false);
    }
    /*
    public void OnClickCancelExchange()
    {
        exchangePopupPenel.SetActive(false);
        exchangeOptionsRelease();
    }
    */
    
    public void itemOptionRelease()
    {
        for (int i = 0; i < selectedItemOptonsPooling.Count; ++i)
        {
            selectedItemOptonsPooling[i].SetActive(false);
        }
        for (int i = 0; i < selectedItemOptonsPooling.Count; ++i)
        {
            selectedItemOptonsPooling[i].SetActive(false);
        }
    }
    
    public void OnClickExchangeItem()
    {
        selectedItemPopupPenel.SetActive(false);
        invenRelease();
        initPlayerInventory();
        equipmentPenel.transform.GetChild(selectedEquipNum).GetChild(1).GetComponent<Image>().sprite = gm.inventory[selectedInvenItemNum].itemImage;
        equipmentPenel.transform.GetChild(selectedEquipNum).GetChild(2).GetComponent<Text>().text = "Lv."+ gm.inventory[selectedInvenItemNum].level;
        gm.playerEquipment[selectedEquipNum] = gm.inventory[selectedInvenItemNum];
        gm.inventory.RemoveAt(selectedInvenItemNum);

        selectedEquipNum = -1;
        selectedInvenItemNum = -1;
    }
}
