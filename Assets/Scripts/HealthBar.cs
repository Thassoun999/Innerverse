using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // This script will go on our canvas
    [SerializeField] private Image _healthbarSprite; // foreground
    private GameObject _healthbarCanvas; // Entire parent (this needs to be disabled on start or awake??)

    private Camera _cam;

    void Awake()
    {
        _healthbarCanvas = gameObject;
        _healthbarCanvas.SetActive(false);
    }

    void Start() 
    {
        _cam = Camera.main;
    }

    void Update()
    {
        // Ensures the healthbar is always billboarding (facing the camera)
        // transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position); -- Isometric games don't really need this, just set it manually
    }

    public void UpdateHealthBar(float maxHealth, float currHealth) {
        _healthbarSprite.fillAmount = currHealth / maxHealth;
    }

    public void ToggleView(bool value) {
        _healthbarCanvas.SetActive(value);
    }
}
