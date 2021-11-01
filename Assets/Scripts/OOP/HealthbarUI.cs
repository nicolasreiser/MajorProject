using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class HealthbarUI : MonoBehaviour
{
    [SerializeField] private Transform _bar = null;
    [SerializeField] private Transform _barYellow = null;

    [SerializeField] private float InitialisationValues;

    private float _delayedCurrentHealth;

    EntityManager entityManager;

    EntityQuery entity;

    // Start is called before the first frame update
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entity = entityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerTag>());

        _bar.localScale = new Vector3(InitialisationValues, 1);                 // sets up the scale of the healthbar
        _barYellow.localScale = new Vector3(InitialisationValues, 1);
        _delayedCurrentHealth = 1;

    }

    private void CheckforDamage()
    {
        Entity e = entity.GetSingletonEntity();
        PlayerData p = entityManager.GetComponentData<PlayerData>(e);

        if(p.OnHealthChange)
        {
            p.OnHealthChange = false;
            SetSize(p.CurrentHealth, p.BaseHealth);
            YellowBar(p.CurrentHealth);
            entityManager.SetComponentData(e, p);
        }

    }
    // fixed update used to decrease the yellow healthbar at a constant rate
    void FixedUpdate()
    {
        CheckforDamage();
        if (_delayedCurrentHealth <= 0) _barYellow.localScale = new Vector3(0, 1);
    }
    // reduced the size of the red Healthbar instantly
    public void SetSize(float currentHealth, float maxHealth)
    {
        if (currentHealth >= 0)
        {
            float size = currentHealth / maxHealth;
            _bar.localScale = new Vector3(size* InitialisationValues , 1);
        }
    }
    //reduces the scale of the secondary yellow healthbar slowly
    private void YellowBar(float currentHealth)
    {
        if (_delayedCurrentHealth > currentHealth && _delayedCurrentHealth > 0)
        {
            _delayedCurrentHealth -= Time.deltaTime / 2;
            _barYellow.localScale = new Vector3(_delayedCurrentHealth * InitialisationValues, 1);
        }
    }
    public void Regen(float regenAmmount)
    {
        //float healthToArchieve = GameManager.Instance.Player.HealthCurrent + regenAmmount;
        // lets the health regenerate slowly
        //StartCoroutine(HealthRegenCoroutine(healthToArchieve));
    }
    //private IEnumerator HealthRegenCoroutine(float healthToArchieve)
    //{
    //    while (GameManager.Instance.Player.HealthCurrent < healthToArchieve)
    //    {
    //        if (GameManager.Instance.Player.HealthCurrent < GameManager.Instance.Player.HealthMax)
    //        {
    //            GameManager.Instance.Player.HealthCurrent += 0.1f;
    //            yield return new WaitForSeconds(.1f);
    //        }
    //        else
    //        {
    //            GameManager.Instance.Player.HealthCurrent = GameManager.Instance.Player.HealthMax;
    //            break;
    //        }
    //    }

    //}
}
