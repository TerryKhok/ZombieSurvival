using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [SerializeField] HealthBar _healthbar;

    [SerializeField] ThirdPersonMovement _playerController;
    [SerializeField] ThirdPersonShooterController _playerShooterController;

    private Animator _animator;
    private AudioManager _audioManager;
    private int _animIDHurt;

    private void Start()
    {
        _healthbar.SetMaxHealth(maxHealth);
        _healthbar.SetCurrentHealth(currentHealth);
        _playerController = GetComponent<ThirdPersonMovement>();
        _animator = GetComponent<Animator>();
        _audioManager = FindObjectOfType<AudioManager>();

        _animIDHurt = Animator.StringToHash("Hurt");
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _healthbar.SetCurrentHealth(currentHealth);
        if(damage > 0){
        _animator.SetTrigger(_animIDHurt);
        _audioManager.Play("PlayerHurt");
        }else{
            //heal effectß
        }
    }


    public override void Die()
    {
        base.Die();
        _playerShooterController = FindAnyObjectByType<ThirdPersonShooterController>();
        _playerController.IsDead = true;
        _playerShooterController.IsDead = true;
    }
}
