using UnityEngine;

public class BulletProjectileRaycast : MonoBehaviour
{
    [SerializeField]private Transform _vfxHit;

    private Vector3 _targetPosition;

    public void Setup(Vector3 targetPosition){
        this._targetPosition = targetPosition;
    }

    private void Update(){
        float distanceBefore=Vector3.Distance(transform.position,_targetPosition);

        Vector3 moveDir = (_targetPosition - transform.position).normalized;
        float moveSpeed = 200f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfter = Vector3.Distance(transform.position,_targetPosition);

        if(distanceBefore < distanceAfter){
            Instantiate(_vfxHit,_targetPosition, Quaternion.identity);
            transform.Find("Trail").SetParent(null);
            Destroy(gameObject);
        }
    }
}
