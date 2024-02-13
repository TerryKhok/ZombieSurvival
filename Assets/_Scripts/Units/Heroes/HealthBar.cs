using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]private Slider _currentHpSlider;
    [SerializeField]private Slider _damageTakenSlider;

    private void FixedUpdate() {
        if(_damageTakenSlider.value>_currentHpSlider.value){
            _damageTakenSlider.value-=.5f;
        }
    }

    //�ő�HP�ݒ�
    public void SetMaxHealth(int maxHealth){
        _currentHpSlider.maxValue = maxHealth;
        _damageTakenSlider.maxValue = maxHealth;
    }

    //����HP�ݒ�
    public void SetCurrentHealth(int curHealth){
        _currentHpSlider.value = curHealth;
    }


}
