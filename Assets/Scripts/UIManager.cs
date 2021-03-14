using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    public Button back;
    public TMP_Text winText;
    void Start()
    {
        back.interactable = false;
    }

    public void ActivateBackButton(){
        back.interactable = true;
    }

    public void BackButton(){
        GameManager.instance.BackMove();
        if(GameManager.instance.History.Count<1){back.interactable = false;}
    }

    public void Win(){
        winText.gameObject.SetActive(true);
    }

    public void Reload(){
        GameManager.instance.Reload();
    }
}
