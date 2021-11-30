using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;

public class HealthbarUI : MonoBehaviour
{
    [SerializeField] private Transform _primaryHealthBar = null;
    [SerializeField] private Transform _secondaryHealthBar = null;
    [SerializeField] private Transform _expBar = null;
    [SerializeField] private TextMeshProUGUI _levelText = null;


    [SerializeField] private float InitialisationValues;

    private float _delayedCurrentHealth;
    private float _currentHealth;
    EntityManager entityManager;

    EntityQuery entity;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = entityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerTag>());

        _primaryHealthBar.localScale = new Vector3(InitialisationValues, 1);                 // sets up the scale of the healthbar
        _secondaryHealthBar.localScale = new Vector3(InitialisationValues, 1);
        _delayedCurrentHealth = 1;
        _expBar.localScale = new Vector3(0, 1);

    }
    void FixedUpdate()
    {
        CheckforDamage();
        CheckForExperiance();
        SetLevel();
        SetCurrentHealth();
        YellowBar(_currentHealth);
        if (_delayedCurrentHealth <= 0) _secondaryHealthBar.localScale = new Vector3(0, 1);
    }

    private void CheckforDamage()
    {
        if(!entity.IsEmpty)
        {
            Entity e = entity.GetSingletonEntity();
            PlayerData p = entityManager.GetComponentData<PlayerData>(e);

            if(p.OnHealthChange)
            {
                p.OnHealthChange = false;
                SetSize(p.CurrentHealth, p.BaseHealth);
                
                entityManager.SetComponentData(e, p);
            }
        }

    }

    private void SetCurrentHealth()
    {
        if (!entity.IsEmpty)
        {
            Entity e = entity.GetSingletonEntity();
            PlayerData p = entityManager.GetComponentData<PlayerData>(e);

            _currentHealth = (float)p.CurrentHealth/ (float)p.BaseHealth;
        }
    }
    private void CheckForExperiance()
    {
        if(!entity.IsEmpty)
        {
            Entity e = entity.GetSingletonEntity();
            PlayerData p = entityManager.GetComponentData<PlayerData>(e);

            if (p.OnExperienceChange)
            {
                p.OnExperienceChange = false;
                SetExpSize(p.Experience, p.MaxExperience);

                entityManager.SetComponentData(e, p);
            }
        }

    }

    private void SetLevel()
    {
        if (!entity.IsEmpty)
        {
            Entity e = entity.GetSingletonEntity();
            PlayerData p = entityManager.GetComponentData<PlayerData>(e);

            _levelText.text = p.Level.ToString();
        }
    }

    // fixed update used to decrease the yellow healthbar at a constant rate
    // reduced the size of the red Healthbar instantly
    public void SetSize(float currentHealth, float maxHealth)
    {
        if (currentHealth >= 0)
        {
            float size = currentHealth / maxHealth;
            _primaryHealthBar.localScale = new Vector3(size* InitialisationValues , 1);
        }
    }
    //reduces the scale of the secondary yellow healthbar slowly
    private void YellowBar(float currentHealth)
    {
        if (_delayedCurrentHealth < currentHealth)
        {
            _delayedCurrentHealth = currentHealth;
            _secondaryHealthBar.localScale = new Vector3(_delayedCurrentHealth * InitialisationValues, 1);

        }
        if (_delayedCurrentHealth > currentHealth && _delayedCurrentHealth > 0)
        {
            _delayedCurrentHealth -= Time.deltaTime / 3;
            
            _secondaryHealthBar.localScale = new Vector3(_delayedCurrentHealth * InitialisationValues, 1);
        }
    }

    public void SetExpSize(float currentExp, float maxExp)
    {
        if (currentExp >= 0)
        {
            float size = currentExp / maxExp;

            //Debug.Log($"CurrentExp : {currentExp} maxExp : {maxExp} size : {size}");
            _expBar.localScale = new Vector3(size * InitialisationValues, 1);
        }
    }
   
}
