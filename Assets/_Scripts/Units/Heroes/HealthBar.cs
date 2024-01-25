using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]private Slider _slider;

    //�ő�HP�ݒ�
    public void SetMaxHealth(int maxHealth){
        _slider.maxValue = maxHealth;
    }

    //����HP�ݒ�
    public void SetCurrentHealth(int curHealth){
        _slider.value = curHealth;
    }

}
