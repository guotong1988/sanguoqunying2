using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectKingSceneController : MonoBehaviour {

    public GameObject selectKing;
    public GameObject selectMOD;

    public GameObject pushbuttonPrefab;
    public GameObject kingListRoot;

    public GameObject confirmBox;
    public Button btnOK;
    public Button btnCancel;

    public MapController mapCtrl;
    public KingInfoController kingInfoCtrl;

    public MenuDisplayAnim infoAnim;
    public MenuDisplayAnim mapAnim;
    public MenuDisplayAnim menuAnim;

    public Button[] modNames;

    private List<GameObject> kingNameButtons = new List<GameObject>();

    private int state = 0;
    private int kingIndex = -1;
    private bool isConfirmBoxShow = false;

    private int[] kingListNum = new int[5] { 6, 7, 5, 5, 5 };
    private Vector3 kingListFirstPos = new Vector3(-260, 150, 0);

	// Use this for initialization
	void Start () 
    {
        if (PlayerPrefs.HasKey("GamePass"))
        {
            SetSelectMOD();
        }
        else
        {
            SetSelectKing();
        }

        for (int i = 0; i < modNames.Length; i++)
        {
            modNames[i].SetButtonClickHandler(OnMODButtonClick);
            modNames[i].SetButtonData(i);
        }

        confirmBox.SetActive(false);
        btnOK.SetButtonClickHandler(OnOKButton);
        btnCancel.SetButtonClickHandler(OnCancelButton);

        OnCancelButton();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Misc.GetBack())
        {
            if (state == 0)
            {
                Misc.LoadLevel("StartScene");
                GameObject.Destroy(GameObject.Find("MouseTrack"));
            }
            else if (state == 1)
            {
                if (PlayerPrefs.HasKey("GamePass"))
                {
                    if (isConfirmBoxShow)
                    {
                        OnCancelButton();
                    }
                    else
                    {
                        SetSelectMOD();
                    }
                }
                else
                {
                    Misc.LoadLevel("StartScene");
                    GameObject.Destroy(GameObject.Find("MouseTrack"));
                }
            }
        }
	}

    void SetSelectKing()
    {
        if (selectMOD.activeSelf)
        {
            selectMOD.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.OutToLeft);

            Invoke("SelectKingEnter", 0.2f);
        }
        else
        {
            SelectKingEnter();
        }
    }

    void SelectKingEnter()
    {
        state = 1;

        selectMOD.SetActive(false);
        selectKing.SetActive(true);

        int num = kingListNum[Controller.MODSelect];
        for (int i = 0; i < num; i++)
        {
            GameObject go = (GameObject)Instantiate(pushbuttonPrefab);
            go.transform.parent = kingListRoot.transform;
            go.transform.localPosition = new Vector3(kingListFirstPos.x, kingListFirstPos.y - i * 30, 0);
            go.GetComponent<PushedButton>().SetButtonDownHandler(OnKingNameSelect);
            go.GetComponent<PushedButton>().SetButtonData(i);
            go.GetComponent<exSpriteFont>().text = ZhongWen.Instance.GetKingName(i);
            kingNameButtons.Add(go);
        }

        if (kingIndex == -1)
        {
            OnKingNameSelect(0);
        }

        infoAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromBottom);
        mapAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
        menuAnim.SetAnim(MenuDisplayAnim.AnimType.InsertFromLeft);
    }

    public void SetSelectMOD()
    {
        if (selectKing.activeSelf)
        {
            infoAnim.SetAnim(MenuDisplayAnim.AnimType.OutToBottom);
            mapAnim.SetAnim(MenuDisplayAnim.AnimType.OutToRight);
            menuAnim.SetAnim(MenuDisplayAnim.AnimType.OutToLeft);

            Invoke("SelectMODEnter", 0.2f);
        }
        else
        {
            SelectMODEnter();
        }
    }

    void SelectMODEnter()
    {
        state = 0;

        selectMOD.SetActive(true);
        selectKing.SetActive(false);

        for (int i = 0; i < kingNameButtons.Count; i++)
        {
            Destroy(kingNameButtons[i]);
        }
        kingNameButtons.Clear();

        selectMOD.GetComponent<MenuDisplayAnim>().SetAnim(MenuDisplayAnim.AnimType.InsertFromRight);
    }

    void OnMODButtonClick(object data)
    {
        int index = (int)data;

        SetSelectKing();

        Informations.Reset();
        MODLoadController.Instance.LoadMOD(index);
    }

    void OnKingNameSelect(object data)
    {
        int index = (int)data;
        if (kingIndex != index)
        {
            if (kingIndex != -1)
                kingNameButtons[kingIndex].GetComponent<PushedButton>().SetButtonState(PushedButton.ButtonState.Normal);
            kingIndex = index;
            kingNameButtons[kingIndex].GetComponent<PushedButton>().SetButtonState(PushedButton.ButtonState.Pressed);
        }
        else
        {
            return;
        }

        mapCtrl.ClearSelect();
        for (int i = 0; i < Informations.Instance.GetKingInfo(kingIndex).cities.Count; i++)
        {
            mapCtrl.SelectCity(Informations.Instance.GetKingInfo(kingIndex).cities[i]);
        }

        kingInfoCtrl.SetKing(kingIndex);

        if (!isConfirmBoxShow)
        {
            confirmBox.SetActive(true);
            isConfirmBoxShow = true;
        }
        confirmBox.transform.position = new Vector3(confirmBox.transform.position.x, 160 - 30 * kingIndex, confirmBox.transform.position.z);
    }

    void OnOKButton()
    {
        Controller.kingIndex = kingIndex;

        StrategyController.isFirstEnter = true;
        Misc.LoadLevel("InternalAffairs");
    }

    void OnCancelButton()
    {
        isConfirmBoxShow = false;
        confirmBox.SetActive(false);

        btnCancel.SetButtonState(Button.ButtonState.Normal);

        kingNameButtons[kingIndex].GetComponent<PushedButton>().SetButtonState(PushedButton.ButtonState.Normal);
        kingIndex = -1;
    }
}
