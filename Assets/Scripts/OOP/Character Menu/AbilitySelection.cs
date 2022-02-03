using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelection : MonoBehaviour
{
    private CharacterSelectionManagement csm;

    public AbilityScriptableObject[] AbilitySO;


    public List<Button> AbilityButtons;

    public List<TMPro.TextMeshProUGUI> AbilityText;
    public List<Image> AbilityImages;

    public GameObject NoMoneyPanel;

    // Start is called before the first frame update
    void Awake()
    {
        csm = GetComponent<CharacterSelectionManagement>();
        InitializeAbilitiesUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectAbility(int abilityID)
    {
        
        if (csm.data.AbilitiesUnlocked[abilityID] == true)
        {
            AbilityText[abilityID].text = "Selected";
            csm.data.AbilityType = abilityID;

            AbilityButtons[abilityID].enabled = false;
            ResetText(abilityID);
        }
        else
        {
            //check if i have money to unlock

            int currentMoney = csm.GetCurrency();

            int AbilityPrice = AbilitySO[abilityID - 1].Price;

            if(AbilityPrice <= currentMoney)
            {
                // Ability purchased

                csm.RemoveCurrency(AbilityPrice);
                csm.data.AbilitiesUnlocked[abilityID] = true;
                AbilityText[abilityID].text = "Selected";
                csm.data.AbilityType = abilityID;

                AbilityButtons[abilityID].enabled = false;
                ResetText(abilityID);

            }
            else
            {
                StartCoroutine(NoCurrencyCoroutine());

                Debug.Log("NO money to unlock");

            }

        }

    }
   
    private void ResetText(int AbilityToIgnore)
    {
        if(AbilityToIgnore != 0)
        {
            AbilityText[0].text = "Select";
            if(csm.data.AbilitiesUnlocked[0] == true)
            {
                AbilityButtons[0].enabled = true;
            }

        }
        if (AbilityToIgnore != 1)
        {
            if (csm.data.AbilitiesUnlocked[1] == true)
            {
                AbilityText[1].text = "Select";
                AbilityButtons[1].enabled = true;
            }
            else
            {
                AbilityText[1].text = "Unlock";
            }

        }
        if (AbilityToIgnore != 2)
        {
            if (csm.data.AbilitiesUnlocked[2] == true)
            {
                AbilityText[2].text = "Select";
                AbilityButtons[2].enabled = true;
            }
            else
            {
                AbilityText[2].text = "Unlock";
            }

        }
        if (AbilityToIgnore != 3)
        {
            if (csm.data.AbilitiesUnlocked[3] == true)
            {
                AbilityText[3].text = "Select";
                AbilityButtons[3].enabled = true;
            }
            else
            {
                AbilityText[3].text = "Unlock";
            }

        }
    }

    private IEnumerator NoCurrencyCoroutine()
    {
        NoMoneyPanel.SetActive(true);

        yield return new WaitForSeconds(1);

        NoMoneyPanel.SetActive(false);

    }

    private void InitializeAbilitiesUI()
    {
        for (int i = 0; i < AbilityButtons.Count; i++)
        {
            AbilityImages[i].sprite = AbilitySO[i].InventoryIcon;
        }

    }
}
