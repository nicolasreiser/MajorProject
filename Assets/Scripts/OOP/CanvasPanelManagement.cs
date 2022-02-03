using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;

public class CanvasPanelManagement : MonoBehaviour
{
    public GameObject DarkPanel;

    public int MainMenuIndex;
    public int CharacterSelectionIndex;

    EntityManager entityManager;


    public void PanelState(bool state)
    {
        StartCoroutine(PanelStateCoroutine(state));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(PanelStateCoroutine(true));

        ClearEntities();
        
        SceneManager.LoadScene(MainMenuIndex,LoadSceneMode.Single);
    }

    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene(CharacterSelectionIndex, LoadSceneMode.Single);

    }

    public void ClearEntities()
    {

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var queryDesc = new EntityQueryDesc
        {
            Any = new ComponentType[] { ComponentType.ReadWrite<Entity>(), ComponentType.ReadWrite<Prefab>() }

        };
        EntityQuery query = entityManager.CreateEntityQuery(queryDesc); 

        entityManager.DestroyEntity(query);
        
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

    public void DeleteSaves()
    {
        SaveManager.DeleteSaves();
    }

}
