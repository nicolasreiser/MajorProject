using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManagement : MonoBehaviour
{

    public SaveData data;
    // Start is called before the first frame update
    void Start()
    {
       data = SaveManager.LoadStats();

        //if(data == null)
        //{
        //    Debug.Log("Created new save");
        //    PlayerStats stats = new PlayerStats();
            
        //    data = new SaveData(stats);
        //}
        SetCurrency(data);
        SetAbility(data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetCurrency(SaveData saveData)
    {
        CharacterMenuUI ui = GameObject.FindObjectOfType<CharacterMenuUI>();

        if (ui != null)
        {
            ui.CurrencyText.text = saveData.Currency.ToString();
        }
    }

    public int GetCurrency()
    {
        return data.Currency;
    }

    public void RemoveCurrency(int value)
    {
        data.Currency -= value;
        CharacterMenuUI ui = GameObject.FindObjectOfType<CharacterMenuUI>();

        if (ui != null)
        {
            ui.CurrencyText.text = data.Currency.ToString();

        }
    }

    public void AddCurrency(int value)
    {
        data.Currency += value;

        CharacterMenuUI ui = GameObject.FindObjectOfType<CharacterMenuUI>();

        if (ui != null)
        {
            ui.CurrencyText.text = data.Currency.ToString();

        }
    }
    public int GetBuff(int BuffIndex)
    {
        switch (BuffIndex)
        {
            case 0:
                return data.HealthBuff;
            case 1:
                return data.DamageBuff;
            case 2:
                return data.AttackspeedBuff;
            case 3:
                return data.EarningsBuff;
            default:
                return 0;
        }
    }
    public void AddBuff(int BuffIndex)
    {
        switch (BuffIndex)
        {
            case 0:
                data.HealthBuff++;
                break;
            case 1:
                data.DamageBuff++;
                break;
            case 2:
                data.AttackspeedBuff++;
                break;
            case 3:
                data.EarningsBuff++;
                break;
            default:
                break;
        }

    }

     private void SetAbility( SaveData saveData)
    {
        AbilitySelection AS = GetComponent<AbilitySelection>();
        AS.SelectAbility(saveData.AbilityType);
    }



    public void SaveProgress()
    {
        PlayerStats stats = new PlayerStats();
        stats.TotalCurrency = data.Currency;
        stats.AbilityType = data.AbilityType;
        stats.TopLevel = data.TopLevel;
        stats.HealthBuff = data.HealthBuff;
        stats.DamageBuff = data.DamageBuff;
        stats.AttackspeedBuff = data.AttackspeedBuff;
        stats.EarningsBuff = data.EarningsBuff;

        stats.AbilitiesLock = new List<bool>();
        foreach (bool item in data.AbilitiesUnlocked)
        {
            stats.AbilitiesLock.Add(item);

        }

        SaveManager.SaveStats(stats);

        Debug.Log("Progress Saved...");
    }
}
