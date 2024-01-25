using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsPickUp : MonoBehaviour
{
    [SerializeField] private bool _isDefaultGun = false;

    [Header("Gun Type")]
    [SerializeField] private bool _isPistol;
    [SerializeField] private bool _isMachineGun;
    [SerializeField] private bool _isBurstGun;
    [SerializeField] private bool _isShotgun;
    [SerializeField] private bool _isSniper;
    [SerializeField] private bool _isMinigun;
    [Header("Cache")]
    [SerializeField] private GameObject _pistolGameobject;
    [SerializeField] private GameObject _machinegunGameobject;
    [SerializeField] private GameObject _burstgunGameobject;
    [SerializeField] private GameObject _shotgunGameobject;
    [SerializeField] private GameObject _sniperGameobject;
    [SerializeField] private GameObject _minigunGameobject;

    private AudioManager _audioManager;

    // private void Awake()
    // {
    //     _pistolGameobject = GameObject.Find("Pistol");
    //     _machinegunGameobject = GameObject.Find("MachineGun");
    //     _burstgunGameobject = GameObject.Find("BurstGun");
    //     _shotgunGameobject = GameObject.Find("Shotgun");
    //     _sniperGameobject = GameObject.Find("Sniper");
    //     _minigunGameobject = GameObject.Find("Minigun");
    // }

    private void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();

        if (_isDefaultGun)
        {
            SetActiveState(_pistolGameobject, _isPistol);
            SetActiveState(_machinegunGameobject, _isMachineGun);
            SetActiveState(_burstgunGameobject, _isBurstGun);
            SetActiveState(_shotgunGameobject, _isShotgun);
            SetActiveState(_sniperGameobject, _isSniper);
            SetActiveState(_minigunGameobject, _isMinigun);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            SetActiveState(_pistolGameobject, _isPistol);
            SetActiveState(_machinegunGameobject, _isMachineGun);
            SetActiveState(_burstgunGameobject, _isBurstGun);
            SetActiveState(_shotgunGameobject, _isShotgun);
            SetActiveState(_sniperGameobject, _isSniper);
            SetActiveState(_minigunGameobject, _isMinigun);

            Destroy(transform.parent.gameObject);
        }
         _audioManager.Play("PickUp");
    }

    private void SetActiveState(GameObject gun, bool isGun)
    {
        gun.SetActive(isGun);
    }

}
