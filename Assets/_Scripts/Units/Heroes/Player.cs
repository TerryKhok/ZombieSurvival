using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : HeroUnitBase
{
    //仮ステータス
    private int _maxHealth = 100;

    private Stats _stats;
    private int _curHealth;

    [SerializeField] private HealthBar _healthBar;

    [SerializeField] private AudioClip _someSound;

    void Start()
    {
        // Example usage of a static system
        //AudioSystem.Instance.PlaySound(_someSound);
        _curHealth = _maxHealth;
        _healthBar.SetMaxHealth(_maxHealth);
        _healthBar.SetCurrentHealth(_curHealth);
    }

    public override void ExecuteMove()
    {
        // Perform tarodev specific animation, do damage, move etc.
        // You'll obviously need to accept the move specifics as an argument to this function. 
        // I go into detail in the Grid Game #2 video
        base.ExecuteMove(); // Call this to clean up the move
    }

    //仮関数
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(20);
        }
    }

    //ダメージ処理
    public void TakeDamage(int dmg)
    {
        _curHealth = ManageHealth(_curHealth, -dmg);
        _healthBar.SetCurrentHealth(_curHealth);

        //死の処理
        if (_curHealth <= 0)
        {
            SceneManager.LoadScene("Result");
            Cursor.visible = true;  //カーソル表示
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

}
