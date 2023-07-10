using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * 20, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        // 구 반경으로 위치를 가져옴.
        Collider[] cols = Physics.OverlapSphere(collision.transform.position, 30);
        
        for (int i = 0; i < cols.Length; i++)
        {       
            // 폭탄 반경에 있는 오브젝트 rigidbody 가져옴
            if (cols[i].name.Contains("Boco"))
            {
                Debug.Log(i + " : " + cols[i]);
                Rigidbody rigid = cols[i].GetComponent<Rigidbody>();
                // (폭발의 힘, 영향이 미치는 구의 중심, 영향이 미치는 구의 반경, 위로 솟구치는 힘)
                rigid.AddExplosionForce(500, this.transform.position, 20, 30);
                cols[i].GetComponent<Bocoblin1>().state = Bocoblin1.BocoblinState.Damaged;
            }
        }
       


    }
}
