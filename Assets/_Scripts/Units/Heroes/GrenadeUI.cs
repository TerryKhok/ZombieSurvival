using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GrenadeUI : MonoBehaviour
{
    [SerializeField] Image _grenadeUIBackground;
    [SerializeField] Image _grenadeUIImage;

    private float _reloadSpeed; //リロード速度

    public void StartCooldown(float cooldown)
    {
        _reloadSpeed = cooldown;
        StartCoroutine(DoCooldown());
    }

    IEnumerator DoCooldown()
    {
        _grenadeUIBackground.fillAmount = 0;
        _grenadeUIImage.fillAmount = 0;
        do //reload図をリロード時間よりfillする
        {
            _grenadeUIBackground.fillAmount += Time.deltaTime * (1 / _reloadSpeed);
            _grenadeUIImage.fillAmount += Time.deltaTime * (1 / _reloadSpeed);
            yield return new WaitForEndOfFrame();
        } while (_grenadeUIBackground.fillAmount < 1f && _grenadeUIImage.fillAmount < 1f);

        yield return new WaitForEndOfFrame();
    }
}
