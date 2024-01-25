using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    //�[�[�[�[�[�[�[�[�[�[�ϐ��錾�[�[�[�[�[�[�[�[�[�[
    [SerializeField] RawImage _dot; //���S�_
    [SerializeField] RawImage _inner; //����
    [SerializeField] RawImage _expanding; //���I�ȊO��
    [SerializeField] Image _reload; //�����[�h�}

    private float _reloadSpeed; //�����[�h���x
    private float _shrinkSpeed; //���R�C�����x
    private float _crosshairMaxScale; //�ő�傫��

    private bool _isReloading;
    private bool _isShrinking;

    private Vector3 _crosshairOriginalScale; //�����傫��
    //�[�[�[�[�[�[�[�[�[�[end�ϐ��錾�[�[�[�[�[�[�[�[�[�[


    private void Start()
    {
        _reload.enabled = false; //�����[�h�}���\��

        //�����ݒ���L���V���[
        _crosshairOriginalScale = _expanding.rectTransform.localScale;
    }


    //�[�[�[�[�[�[�[�[�[�[Public�֐��[�[�[�[�[�[�[�[�[�[
    //�����[�h�֐�
    public void DoReload()
    {
        if (!_isReloading)
        {
            StartCoroutine(ReloadGun());
        }
    }


    //crosshair�g��֐�
    public void ExpandCrosshair(float addScale)
    {
        if (_expanding.rectTransform.localScale.x < _crosshairMaxScale)
        {
            _expanding.rectTransform.localScale += new Vector3(addScale, addScale, addScale);
        }
        else
        {
            _expanding.rectTransform.localScale = new Vector3(_crosshairMaxScale, _crosshairMaxScale, _crosshairMaxScale);
        }

        if (!_isShrinking)
        {
            StartCoroutine(ShrinkCrosshair());
        }
    }


    //�e�̃����[�h���x��ݒ肷��
    public void SetReloadSpeed(float reloadSpeed)
    {
        _reloadSpeed = reloadSpeed;
    }

    //crosshair�̏k�����x��ݒ肷��
    public void SetShrinkSpeed(float shrinkSpeed)
    {
        _shrinkSpeed = shrinkSpeed;
    }

    //crosshair�̂�ݒ肷��
    public void SetMaxScale(float maxScale)
    {
        _crosshairMaxScale = maxScale;
    }
    //�[�[�[�[�[�[�[�[�[�[endPublic�֐��[�[�[�[�[�[�[�[�[�[


    //�[�[�[�[�[�[�[�[�[�[Coroutine�[�[�[�[�[�[�[�[�[�[
    //DoReload()����Ăяo���A�����[�h�}���A�j���[�V��������
    IEnumerator ReloadGun()
    {
        _isReloading = true;

        //reload��\���A����\��
        _reload.fillAmount = 0;
        _reload.enabled = true;
        _inner.enabled = false;
        _dot.enabled = false;
        _expanding.enabled = false;

        do //reload�}�������[�h���Ԃ��fill����
        {
            _reload.fillAmount += Time.deltaTime * _reloadSpeed;
            yield return new WaitForEndOfFrame();
        } while (_reload.fillAmount < 1f);

        //reload���\���A���\��
        _reload.enabled = false;
        _inner.enabled = true;
        _dot.enabled = true;
        _expanding.enabled = true;

        //�����[�h����
        _isReloading = false;

        _expanding.rectTransform.localScale = _crosshairOriginalScale;

        yield return new WaitForEndOfFrame();
    }


    //ExpandCrosshair()����Ăяo���Acrosshair�����X�ɏk��
    IEnumerator ShrinkCrosshair()
    {
        _isShrinking = true;

        do //���̃T�C�Y���傫�����k������
        {
            _expanding.rectTransform.localScale = new Vector3(_expanding.rectTransform.localScale.x - Time.deltaTime * _shrinkSpeed,
                                                              _expanding.rectTransform.localScale.y - Time.deltaTime * _shrinkSpeed,
                                                              _expanding.rectTransform.localScale.z - Time.deltaTime * _shrinkSpeed);
            yield return new WaitForEndOfFrame();
        } while (_crosshairOriginalScale.x < _expanding.rectTransform.localScale.x);

        //�k������
        _isShrinking = false;
        yield return new WaitForEndOfFrame();
    }
    //�[�[�[�[�[�[�[�[�[�[EndCoroutine�[�[�[�[�[�[�[�[�[�[
}
