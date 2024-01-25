using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rigidbody�R���|�[�l���g���K�{
[RequireComponent(typeof(Rigidbody))]

public class moveEnemy : MonoBehaviour
{
    Rigidbody rb;

    [Header("�v���C���[�I�u�W�F�N�g��")]
    public string playerObjName = "Akaza_sum"; //�v���C���[�I�u�W�F�N�g��
    GameObject playerObj;
    Transform playerTransform;

    [Header("�ړ����x")]
    public float speed = 200.0f;

    [Header("����")]
    public float visionLength = 10.0f;
    public float visionAngle = 90.0f;

    void Start()
    {
        //Rigidbody�R���|�[�l���g���擾����
        rb = this.GetComponent<Rigidbody>();
        if (rb == null) //rididbody��������Ȃ���΃G���[
        {
            Debug.LogError("Rigidbody��������܂���");
            return;
        }

        //�v���C���[�I�u�W�F�N�g���擾����
        playerObj = GameObject.Find(playerObjName);
        if (playerObj == null) //�v���C���[�I�u�W�F�N�g��������Ȃ���΃G���[
        {
            Debug.LogError("�v���C���[�L�����N�^�[��������܂���");
            return;
        }
        playerTransform = playerObj.transform; //�v���C���[��transform���擾
    }

    void Update()
    {
        Vector3 rayVec = playerTransform.position - this.transform.position; //�G����v���C���[�ւ̕���

        float sa = Mathf.Abs(Vector3.Angle(rayVec, transform.forward));

        if (sa < visionAngle / 2)
        {
            Ray ray = new Ray(transform.position + new Vector3(0.0f, 0.7f, 0.0f), rayVec);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionLength))
            {
                if (hit.transform.gameObject.name == playerObjName)
                {


                    //�v���C���[�L�����N�^�[�֌������Ĉړ�����
                    Vector3 moveForward = rayVec * speed * Time.deltaTime;
                    rb.velocity = new Vector3(moveForward.x, rb.velocity.y, moveForward.z); //X��Z�����Ɉړ��x��K�p������

                    //�v���C���[�L�����N�^�[�̕���������
                    this.transform.LookAt(playerTransform.position);
                    this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, this.transform.eulerAngles.z);

                    Debug.DrawRay(ray.origin, ray.direction * visionLength, Color.green, 0.1f, false);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * visionLength, Color.blue, 0.1f, false);
                }

            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<Player>().TakeDamage(20);
        }
    }
}
