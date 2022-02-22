using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;
// handles transition and credits panels
public class CanvasPanelManagement : MonoBehaviour
{
    public GameObject DarkPanel;

    public GameObject DeathPanel;

    public GameObject CreditsPanel;

    public int MainMenuIndex;
    public int CharacterSelectionIndex;

    EntityManager entityManager;


    public void PanelState(bool state, float startDelay)
    {
        StartCoroutine(PanelStateCoroutine(state, startDelay));
    }

    public void DeathLoop()
    {
        StartCoroutine(DeathCoroutine());

    }
    public void LoadMainMenu()
    {
        StartCoroutine(MainMenuCoroutine());

    }

    public void LoadCredits(bool State)
    {
        CreditsPanel.SetActive(State);
    }

    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene(CharacterSelectionIndex, LoadSceneMode.Single);

    }

    public void ClearEntities()
    {

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var queryDesc0 = new EntityQueryDesc
        {
            Any = new ComponentType[] { ComponentType.ReadWrite<Entity>(), ComponentType.ReadWrite<Prefab>() },
            Options = EntityQueryOptions.IncludePrefab
            
        };
        var queryDesc1 = new EntityQueryDesc
        {
            Any = new ComponentType[] { ComponentType.ReadWrite<Entity>(), ComponentType.ReadWrite<Prefab>() },
            Options = EntityQueryOptions.IncludeDisabled

        };

        EntityQuery query = entityManager.CreateEntityQuery(new EntityQueryDesc[] {queryDesc0,queryDesc1}); 

        entityManager.DestroyEntity(query);
        
    }

    private IEnumerator MainMenuCoroutine()
    {
        StartCoroutine(PanelStateCoroutine(true,0));

        yield return new WaitForSeconds(2);

        ClearEntities();
        SceneManager.LoadScene(MainMenuIndex, LoadSceneMode.Single);
    }
    private IEnumerator DeathCoroutine()
    {
        StartCoroutine(DeathPanelStateCoroutine());
        yield return new WaitForFixedUpdate();
        StartCoroutine(PanelStateCoroutine(true, 2));

        

        yield return new WaitForSeconds(4);

        ClearEntities();
        SceneManager.LoadScene(MainMenuIndex, LoadSceneMode.Single);


    }


    private IEnumerator PanelStateCoroutine( bool state, float startDelay)
    {
        var image = DarkPanel.GetComponent<Image>();

        yield return new WaitForSeconds(startDelay);

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

    private IEnumerator DeathPanelStateCoroutine()
    {
        var image = DeathPanel.GetComponent<Image>();

        DeathPanel.SetActive(true);

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

    public void DeleteSaves()
    {
        SaveManager.DeleteSaves();
    }

}
