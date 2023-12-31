using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBomb : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {      
        // 콜라이더를 담을 수 있는 배열을 만든다.

        // 반경 10f에 위치한 오브젝트들을 배열에 담는다
        Collider[] cols = Physics.OverlapSphere(transform.position, 8);

        // foreach문을 통해서 colls배열에 존재하는 각각에 폭발효과를 적용해준다.
        foreach (Collider coll in cols)
        {
            //Debug.Log(coll);
            // 조건문을 사용해서 특정레이어에 속한 오브젝트에만 영향을 줄 수 있다.(ex-플레이어만 날아가도록)
            if (coll.CompareTag("Bokoblin"))
            {
                Rigidbody[] rigid = coll.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in rigid)
                {
                    //rb.velocity = new Vector3(0, 0, 0);
                    //rb.angularVelocity = new Vector3(0, 0, 0);
                    rb.AddExplosionForce(3 * rb.mass, transform.position, 10, 8 * rb.mass, ForceMode.Impulse);
                }

                // 폭탄 데미지
                RagdollBokoblin.Damage = 2;
                coll.GetComponentInParent<RagdollBokoblin>().state = RagdollBokoblin.BocoblinState.Damaged;

            }
            if (coll.CompareTag("Moblin"))
            {
                Molblin1.instance.UpdateDamaged();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
