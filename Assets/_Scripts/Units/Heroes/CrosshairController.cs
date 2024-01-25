using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [SerializeField] RawImage _dot; //中心点
    [SerializeField] RawImage _inner; //内圓
    [SerializeField] RawImage _expanding; //動的な外圓
    [SerializeField] Image _reload; //リロード図

    private float _reloadSpeed; //リロード速度
    private float _shrinkSpeed; //リコイル程度
    private float _crosshairMaxScale; //最大大きさ

    private bool _isReloading;
    private bool _isShrinking;

    private Vector3 _crosshairOriginalScale; //初期大きさ
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Start()
    {
        _reload.enabled = false; //リロード図を非表示

        //初期設定をキャシュー
        _crosshairOriginalScale = _expanding.rectTransform.localScale;
    }


    //ーーーーーーーーーーPublic関数ーーーーーーーーーー
    //リロード関数
    public void DoReload()
    {
        if (!_isReloading)
        {
            StartCoroutine(ReloadGun());
        }
    }


    //crosshair拡大関数
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


    //銃のリロード速度を設定する
    public void SetReloadSpeed(float reloadSpeed)
    {
        _reloadSpeed = reloadSpeed;
    }

    //crosshairの縮小速度を設定する
    public void SetShrinkSpeed(float shrinkSpeed)
    {
        _shrinkSpeed = shrinkSpeed;
    }

    //crosshairのを設定する
    public void SetMaxScale(float maxScale)
    {
        _crosshairMaxScale = maxScale;
    }
    //ーーーーーーーーーーendPublic関数ーーーーーーーーーー


    //ーーーーーーーーーーCoroutineーーーーーーーーーー
    //DoReload()から呼び出し、リロード図をアニメーションする
    IEnumerator ReloadGun()
    {
        _isReloading = true;

        //reloadを表示、他非表示
        _reload.fillAmount = 0;
        _reload.enabled = true;
        _inner.enabled = false;
        _dot.enabled = false;
        _expanding.enabled = false;

        do //reload図をリロード時間よりfillする
        {
            _reload.fillAmount += Time.deltaTime * _reloadSpeed;
            yield return new WaitForEndOfFrame();
        } while (_reload.fillAmount < 1f);

        //reloadを非表示、他表示
        _reload.enabled = false;
        _inner.enabled = true;
        _dot.enabled = true;
        _expanding.enabled = true;

        //リロード完了
        _isReloading = false;

        _expanding.rectTransform.localScale = _crosshairOriginalScale;

        yield return new WaitForEndOfFrame();
    }


    //ExpandCrosshair()から呼び出し、crosshairを徐々に縮小
    IEnumerator ShrinkCrosshair()
    {
        _isShrinking = true;

        do //元のサイズより大きい時縮小する
        {
            _expanding.rectTransform.localScale = new Vector3(_expanding.rectTransform.localScale.x - Time.deltaTime * _shrinkSpeed,
                                                              _expanding.rectTransform.localScale.y - Time.deltaTime * _shrinkSpeed,
                                                              _expanding.rectTransform.localScale.z - Time.deltaTime * _shrinkSpeed);
            yield return new WaitForEndOfFrame();
        } while (_crosshairOriginalScale.x < _expanding.rectTransform.localScale.x);

        //縮小完了
        _isShrinking = false;
        yield return new WaitForEndOfFrame();
    }
    //ーーーーーーーーーーEndCoroutineーーーーーーーーーー
}
