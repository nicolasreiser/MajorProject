using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PauseManagement : MonoBehaviour
{
    public static PauseManagement Instance;

    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } set { isPaused = value; } }   

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        IsPaused = false;
    }

}
