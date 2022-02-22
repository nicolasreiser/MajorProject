using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// managed various buttons to start the run
public class CharacterMenuUI : MonoBehaviour
{
    public int BaseSceneIndex;
    public int MainMenuIndex;
    public GameObject DarkPanel;

    public TMPro.TextMeshProUGUI CurrencyText;

    

    public void StartRun()
    {
         StartCoroutine(PanelStatLoadSceneCoroutine(BaseSceneIndex));
    }

    public void GoToMainMenu()
    {
        StartCoroutine(PanelStatLoadSceneCoroutine(MainMenuIndex));
    }

    private IEnumerator PanelStatLoadSceneCoroutine(int SceneIndex)
    {
        var image = DarkPanel.GetComponent<Image>();

        DarkPanel.SetActive(true);

        while (image.color.a < 1)
        {
            var color = image.color;
            color.a += Time.deltaTime*2;
            if (color.a > 1)
                color.a = 1;
            image.color = color;
            yield return new WaitForFixedUpdate();
        }

        SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
    }
}
