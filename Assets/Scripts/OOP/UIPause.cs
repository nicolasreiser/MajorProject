using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// apuse UI management
public class UIPause : MonoBehaviour
{
    public GameObject PausePanel;

    private PauseManagement pauseManagement;
    // Start is called before the first frame update
    void Start()
    {
        pauseManagement = PauseManagement.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePausePanel()
    {
        PausePanel.SetActive(true);
        pauseManagement.IsPaused = true;
    }

    public void DisablePausePanel()
    {
        PausePanel.SetActive(false);
        pauseManagement.IsPaused = false;

    }
}
