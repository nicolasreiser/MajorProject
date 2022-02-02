using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelection : MonoBehaviour
{
    private CharacterSelectionManagement csm;

    public List<Button> Ability;

    public List<TMPro.TextMeshProUGUI> AbilityText;

    // Start is called before the first frame update
    void Start()
    {
        csm = GetComponent<CharacterSelectionManagement>();
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

            Ability[abilityID].enabled = false;
            ResetText(abilityID);
        }
        else
        {
            //check if i have money to unlock



            //TODO popup no money to unlock
            Debug.Log("NO money to unlock");
        }

    }
   
    private void ResetText(int AbilityToIgnore)
    {
        if(AbilityToIgnore != 0)
        {
            AbilityText[0].text = "Select";
            if(csm.data.AbilitiesUnlocked[0] == true)
            {
                Ability[0].enabled = true;
            }

        }
        if (AbilityToIgnore != 1)
        {
            if (csm.data.AbilitiesUnlocked[1] == true)
            {
                AbilityText[1].text = "Select";
                Ability[1].enabled = true;
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
                Ability[2].enabled = true;
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
                Ability[3].enabled = true;
            }
            else
            {
                AbilityText[3].text = "Unlock";
            }

        }
    }

}
