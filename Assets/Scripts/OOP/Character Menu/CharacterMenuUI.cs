using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterMenuUI : MonoBehaviour
{
    public int BaseSceneIndex;

    public TMPro.TextMeshProUGUI CurrencyText;

    // abilities here


    public void StartRun()
    {

        //todo fade in

        SceneManager.LoadScene(BaseSceneIndex, LoadSceneMode.Single);
    }

}
