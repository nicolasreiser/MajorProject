using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages player  currency
public class CurrencyManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI CurrencyText;

    public static CurrencyManager Instance;

    private int gold;
    public int Gold { get { return gold; } set { gold = value; } }


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        
        Gold = 0;
        CurrencyText.text = Gold.ToString();

    }


    public void AddGold( int ammount)
    {
        Gold += ammount;
        CurrencyText.text = Gold.ToString();

    }
}
