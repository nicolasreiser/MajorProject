using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

// script managing the permanent upgrades

public class BuffsManagement : MonoBehaviour
{
    

    public TMPro.TextMeshProUGUI[] BuffCosts;
                            
    public TMPro.TextMeshProUGUI[] BuffNumberOfUpgrades;

    private CharacterSelectionManagement csm;
    private AbilitySelection abilitySelection;

    // Start is called before the first frame update
    void Start()
    {
        csm = GetComponent<CharacterSelectionManagement>();
        abilitySelection = GetComponent<AbilitySelection>();
        Initialisation();
    }


    private void Initialisation()
    {
        
        UIUpdate();
    }

    public void UpgradeBuff( int BuffIndex)
    {
        Debug.Log("Buffing : " + BuffIndex);
        int BuffCost = BuffCostCalculation(csm.GetBuff(BuffIndex));

        if( BuffCost <= csm.GetCurrency())
        {
            csm.AddBuff(BuffIndex);
            csm.RemoveCurrency(BuffCost);
            UIUpdate();

        }
        else
        {
            abilitySelection.NoCurrencyPanelTrigger();
            // not enough money
        }
    }

    private int BuffCostCalculation( int BuffLevel)
    {
        int res = (int)(100 + Mathf.Pow(BuffLevel,2));
        return res;
    }

    private void UIUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            BuffNumberOfUpgrades[i].text = csm.GetBuff(i).ToString();
            BuffCosts[i].text = BuffCostCalculation(csm.GetBuff(i)).ToString();
        }
    }

}
