using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentHealth : MonoBehaviour
{
    public enum HealthStatus { HEALTHY = 0, CARRIER = 1, INFECTED = 2 };

    public HealthStatus Status = HealthStatus.HEALTHY;

    [Header("UI")]
    public Slider AgentSlider;

    public Text AgentText;

    [Header("Materials")]
    public Material HealthMaterial;

    public Material CarrierMaterial;
    public Material InfectedMaterial;

    [HideInInspector]
    public int Health;

    [Header("Health")]
    [Range(0, 100)]
    public int InfectedPercentChance = 10; //chance that on start agent is infected

    public int HealthIncrease = 10;
    public int HealthDecrease = 10;
    public int CarrierHealth = 50;

    private int _maxHealth = 100;

    private ParticleSystem _particles;

    private void Start()
    {
        //setting up health
        Health = _maxHealth;
        ChangeHealth(0);

        //getting particle system
        _particles = GetComponent<ParticleSystem>();
        _particles.Stop();

        //chance entering agent is infected
        int chance = Random.Range(0, 101);
        if (chance > InfectedPercentChance) return;

        Health = 0;
        ChangeHealth(0);
    }

    private void OnParticleCollision(GameObject go)
    {
        //guard statement to make sure not self, not working
        if (go == this.gameObject) return;

        if (go.layer == LayerMask.NameToLayer("Air")) //air particles are clean
        {
            //Debug.Log("Air");
            ChangeHealth(HealthIncrease);
        }
        if (go.layer == LayerMask.NameToLayer("Agent")) //agent particles are infected
        {
            //Debug.Log("Agent");
            ChangeHealth(-HealthDecrease);
        }
    }

    public void ChangeHealth(int healthChange)
    {
        //health number
        Health += healthChange; //Health = Health + healthChange;
        if (Health > _maxHealth) { Health = _maxHealth; } //don't let Health go over Max Health
        if (Health < 0) { Health = 0; } //don't let Health go belove zero

        //changing the status based on Health
        if (Health > CarrierHealth)
        {
            UpdateStatus(HealthStatus.HEALTHY);
        }
        else if (Health > 0)
        {
            UpdateStatus(HealthStatus.CARRIER);
        }
        else
        {
            UpdateStatus(HealthStatus.INFECTED);
        }

        //Update the Agent UI
        if (AgentText != null)
        {
            AgentText.text = "Health: " + Health.ToString();
        }
        if (AgentSlider != null)
        {
            AgentSlider.value = Health / (_maxHealth * 1.00f);
        }
    }

    private void UpdateStatus(HealthStatus status)
    {
        //guard statement if status is the same
        if (Status == status) return;
        Status = status; //setting the new status

        //get mesh renderer
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        if (renderer == null) return; //guard statements

        //update material and particles based on health status
        switch ((int)status)
        {
            case 0:
                //healthy code
                if (_particles != null) { _particles.Stop(); }
                renderer.material = HealthMaterial;
                break;

            case 1:
                //carrier code
                if (_particles != null) { _particles.Stop(); }
                renderer.material = CarrierMaterial;
                break;

            case 2:
                //infected code
                if (_particles != null) { _particles.Play(); }
                renderer.material = InfectedMaterial;
                break;
        }
    }
}