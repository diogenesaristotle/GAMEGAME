using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SeletButton : MonoBehaviour
{
    Button cardbutton;
    TextMeshProUGUI button;
    int OverCardNumber;
    public GameManager instGameManager;

    private void Awake()
    {
        instGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cardbutton = GetComponent<Button>();
        button = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {

    }


    public void OnMouseOver()
    {
        OverCardNumber = int.Parse(button.text);
        instGameManager.GetCardFun(OverCardNumber);
    }

}

