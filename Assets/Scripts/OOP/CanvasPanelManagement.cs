using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPanelManagement : MonoBehaviour
{
    public GameObject DarkPanel;
    
    public void PanelState(bool state)
    {
        StartCoroutine(PanelStateCoroutine(state));
    }

    private IEnumerator PanelStateCoroutine( bool state)
    {
        var image = DarkPanel.GetComponent<Image>();

        if(state)
        {
            DarkPanel.SetActive(true);

            while (image.color.a < 1)
            {
                var color = image.color;
                color.a += Time.deltaTime;
                if (color.a > 1)
                    color.a = 1;
                image.color = color;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            while (image.color.a > 0)
            {
                var color = image.color;
                color.a -= Time.deltaTime;
                if (color.a <= 0)
                    color.a = 0;
                image.color = color;
                yield return new WaitForFixedUpdate();
            }
            DarkPanel.SetActive(false);

        }


    }
}
