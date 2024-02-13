using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("UI Cache")]
    [SerializeField] private GameObject _pistolUIGameobject;
    [SerializeField] private GameObject _machinegunUIGameobject;
    [SerializeField] private GameObject _burstgunUIGameobject;
    [SerializeField] private GameObject _shotgunUIGameobject;
    [SerializeField] private GameObject _sniperUIGameobject;
    [SerializeField] private GameObject _minigunUIGameobject;
    [SerializeField] private Text _uiGunText;

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
            SetGunActive();
            SetUIGunName();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            SetGunActive();
            SetUIGunName();

            Destroy(transform.parent.gameObject);
        }
        _audioManager.Play("PickUp");
    }

    private void SetActiveState(GameObject gun, bool isGun)
    {
        gun.SetActive(isGun);
    }

    private void SetGunActive()
    {
        SetActiveState(_pistolGameobject, _isPistol);
        SetActiveState(_machinegunGameobject, _isMachineGun);
        SetActiveState(_burstgunGameobject, _isBurstGun);
        SetActiveState(_shotgunGameobject, _isShotgun);
        SetActiveState(_sniperGameobject, _isSniper);
        SetActiveState(_minigunGameobject, _isMinigun);
        SetActiveState(_pistolUIGameobject, _isPistol);
        SetActiveState(_machinegunUIGameobject, _isMachineGun);
        SetActiveState(_burstgunUIGameobject, _isBurstGun);
        SetActiveState(_shotgunUIGameobject, _isShotgun);
        SetActiveState(_sniperUIGameobject, _isSniper);
        SetActiveState(_minigunUIGameobject, _isMinigun);
    }

    private void SetUIGunName(){
        if(_isPistol){
            _uiGunText.text = "Pistol";
        } else if(_isMachineGun){
            _uiGunText.text = "Machine Gun";
        }else if(_isBurstGun){
            _uiGunText.text = "Burst Gun";
        }else if(_isShotgun){
            _uiGunText.text = "Shotgun";
        }else if(_isSniper){
            _uiGunText.text = "Sniper";
        }else if(_isMinigun){
            _uiGunText.text = "Minigun";
        }
    }

}
