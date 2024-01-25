using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]private Slider _slider;

    //ç≈ëÂHPê›íË
    public void SetMaxHealth(int maxHealth){
        _slider.maxValue = maxHealth;
    }

    //ç°ÇÃHPê›íË
    public void SetCurrentHealth(int curHealth){
        _slider.value = curHealth;
    }

}
