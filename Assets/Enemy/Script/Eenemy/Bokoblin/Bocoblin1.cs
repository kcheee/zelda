using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

#region 목표
// 태어날 때 대기한다.
// 링크가 감지거리 내로 다가오면 쫒아간다.
// 링크가 공격거리 내로 다가오면 공격시간이 될 때까지 공격준비를 한다.
// 공격시간이 되면 링크를 공격한다.
// 공격을 한 뒤 공격 딜레이 시간동안 기다린다.
// 반복
// 공격 시간이 되기 전에 링크가 공격 거리보다 멀어지면 대기하거나 쫒아간다.
// 공격 시간이 되기 전에 링크가 공격해서 피격하면 대기 하거나 쫒아가거나 다시 공격 준비를 한다.
// 링크의 공격을 맞으면 뒤로 날아간다.
// 체력이 0이 아니라면 다시 일어나고,
// 보코블린의 체력이 0이 되면 죽고 싶다.
#endregion

public class Bocoblin1 : MonoBehaviour
{
    #region 변수
    static public Bocoblin1 instance = null;
    private void Awake()
    {
        instance = this;
    }

    // 상태
    public BocoblinState state;

    // 상태 열거
    public enum BocoblinState
    {
        Idle, Move, Air, Dodge, Wait, Attack, AttackWait, Damaged, Die
    }

    // 이동속도
    public float speed = 5;
    public float runSpeed = 8;

    // 거리
    float distance;
    public float detectDistance;
    public float attackPossibleDistance;
    public float attackDistance;
    public Transform dodgePos;

    // 시간
    float currentTime;
    public float waitTime;

    // 플레이어(링크)
    GameObject link;

    // 애니메이션
    Animator anim;

    // 체력
    public int currentHP;
    public int maxHP = 10;
    Rigidbody rb;

    // bool
    bool isWait;
    bool isAir;
    bool isAttack;
    #endregion

    #region Start
    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        link = GameObject.Find("Link");
        rb = GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();

        // 하위 오브젝트들의 리지드바디에 있는 isKinetic 을 전부 꺼준다.
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rbs.Length; i++)
        {
            if (rbs[i] == rb)
                continue;   // 내거는 건너뛰고 바로 i 를 증가시킴

            //rbs[i].isKinematic = true;
            rbs[i].useGravity = false;
        }
    }
    #endregion

    #region Upadate
    // Update is called once per frame
    void Update()
    {
        if (state == BocoblinState.Idle)
        {
            UpdateIdle();
        }
        else if (state == BocoblinState.Move)
        {
            UpdateMove();
        }
        else if (state == BocoblinState.Air)
        {
            UpdateAir();
        }
        else if (state == BocoblinState.Dodge)
        {
            UpdateDodge();
        }
        else if (state == BocoblinState.Wait)
        {
            UpdateWait();
        }
        else if (state == BocoblinState.Attack)
        {
            UpdateAttack();
        }
        else if (state == BocoblinState.AttackWait)
        {
            UpdateAttackWait();
        }
        else if (state == BocoblinState.Damaged)
        {
            DamagedProcess();
        }
        else if (state == BocoblinState.Die)
        {
            UpdateDie();
        }
    }
    #endregion

    #region 공중 및 착지
    // 공중
    bool isGrounded = true;
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (isAir)
    //    {
    //        return;
    //    }
    //    else if (collision.gameObject.CompareTag("Floor") && true == isGrounded)
    //    {
    //        // 상태를 Air 로 변환한다.
    //        state = BocoblinState.Air;

    //        // Air 애니메이션 실행
    //        anim.SetBool("Air", true);
    //    }
    //}

    public GameObject bocoAvatar;
    public GameObject bocoRagdoll;
    // 착지
    //private void OnCollisionEnter(Collision collision)
    //{
    //    // 공중상태에 있다가 추락해서 바닥에 닿았을 때
    //    if (collision.gameObject.CompareTag("Floor"))
    //    {
    //        isGrounded = false;

    //        // 만약 체력이 0 이상이라면
    //        if (currentHP > 0)
    //        {   
    //            anim.SetTrigger("Down");
    //            Invoke("StandUp", 2);
    //        }
    //        // 만약 체력이 0 이하가 되면 
    //        else if (currentHP <= 0)
    //        {
    //            // 상태를 Die 로 전환한다.
    //            state = BocoblinState.Die;
    //        }
    //    }
    //}

    // 일어나기
    void StandUp()
    {
        anim.SetTrigger("StandUp");
        currentTime += Time.deltaTime;
        if(currentTime >= 3)
        {
            bocoRagdoll.transform.position = this.transform.position;
            // Idle 상태로 전환한다.
            state = BocoblinState.Idle;
            currentTime = 0;

        }
    }

    #endregion

    #region 상태함수
    private void UpdateIdle()
    {
        // rb.constraints = RigidbodyConstraints.FreezeRotation;
        //rb.freezeRotation = true;

        // 보코볼린의 x,z rotation 을 0으로 해준다.
        //Vector3 t = transform.eulerAngles;
        //transform.eulerAngles = new Vector3(0, t.y, 0);

        // 링크와의 거리를 구한다.
        Vector3 y = link.transform.position;
        y.y = transform.position.y;
        distance = Vector3.Distance(y, transform.position);

        // 만약 링크와의 거리가 감지 거리보다 가까우면
        if (distance <= detectDistance)
        {
            #region 바라보기
            // 링크가 있는 방향을 찾는다.
            Vector3 linkDir = link.transform.position - transform.position;
            linkDir.y = 0;
            linkDir.Normalize();

            // 그 방향을 바라본다.
            transform.forward = linkDir;
            #endregion

            currentTime += Time.deltaTime;

            // 2초가 지나면
            if (currentTime > 2)
            {
                // 상태를 Move 로 변환한다.
                state = BocoblinState.Move;
                currentTime = 0;
            }
        }
    }

    private void UpdateAir()
    {
        //bocoAvatar.SetActive(false);
        //bocoRagdoll.SetActive(true);
        //Rigidbody[] rbbocos = GetComponentsInChildren<Rigidbody>();
        //for (int i = 0; i < rbbocos.Length; i++)
        //{
        //    rbbocos[i].AddForce(transform.up * 7 + transform.forward * -3, ForceMode.Impulse);
        //}
        //state = BocoblinState.Die;
        currentTime += Time.deltaTime;
        if(currentTime > 5)
        {
            bocoAvatar.SetActive(true);
            bocoRagdoll.SetActive(false);
            StandUp();
            currentTime = 0;
        }
    }

    private void UpdateMove()
    {
        #region 거리재기
        // 거리를 구한다.
        Vector3 y = link.transform.position;
        y.y = transform.position.y;
        distance = Vector3.Distance(y, transform.position);
        #endregion

        // 만약 링크와의 거리가 감지 거리보다 멀어지면 Idle 상태로 돌아간다.
        if (detectDistance < distance)
        {
            // 상태를 Idle 로 전환한다.
            state = BocoblinState.Idle;
            anim.SetBool("Move", false);
        }

        // 만약 링크와의 거리가 감지거리보다 가깝고 공격가능거리보다 멀면 이동한다.
        else if (detectDistance > distance && distance > attackPossibleDistance)
        {
            #region 바라보기 및 이동
            // 링크가 있는 방향을 찾는다.
            Vector3 linkDir = link.transform.position - transform.position;
            linkDir.y = 0;
            linkDir.Normalize();

            // 그 방향을 바라본다.
            transform.forward = linkDir;
            anim.SetBool("Move", true);
            // 링크가 있는 곳으로 이동한다.
            transform.position += linkDir * speed * Time.deltaTime;
            #endregion
        }

        // 링크가 공격 거리 안으로 들어오면 기다린다.
        else if (distance <= attackPossibleDistance)
        {
            // 공격대기상태로 전환한다.
            state = BocoblinState.Wait;

            // 애니메이션
            anim.SetBool("Wait", true);
            anim.SetBool("Move", false);
        }
    }

    private void UpdateWait()
    {
        // 시간을 흐르게 한다.
        currentTime += Time.deltaTime;

        #region 거리재기 및 바라보기
        // 거리를 구한다.
        Vector3 y = link.transform.position;
        y.y = transform.position.y;
        distance = Vector3.Distance(y, transform.position);

        // 링크가 있는 방향을 찾는다.
        Vector3 linkDir = link.transform.position - transform.position;
        linkDir.y = 0;
        linkDir.Normalize();

        // 그 방향을 바라본다.
        transform.forward = linkDir;
        #endregion

        // 대기 시간 중에 링크가 공격거리 보다 멀어진다면 Idle
        if (distance > attackPossibleDistance)
        {
            // 상태를 Idle 로 전환한다.
            state = BocoblinState.Idle;
            // 애니메이션 실행
            anim.SetBool("Wait", false);
            anim.SetBool("Run", false);
        }

        // 대기 시간이 지나면 Dodge or Attack
        else if (currentTime >= waitTime)
        {
            int rValue = Random.Range(0, 10);
            // 30% 확률로 회피
            if (rValue < 3 && isWait == false)
            {
                currentTime = 0;
                state = BocoblinState.Dodge;
                // 애니메이션 실행
                anim.SetBool("Dodge", true);
            }

            // 70% 확률로 공격하러 감
            else
            {
                isWait = true;

                // 애니메이션
                anim.SetBool("Wait", false);
                anim.SetBool("Run", true);

                // AttackDistance 까지 달려감
                transform.position += linkDir * runSpeed * Time.deltaTime;

                // 만약 공격거리보다 가까워지면
                if (distance <= attackDistance)
                {
                    state = BocoblinState.Attack;

                    // 애니메이션 실행
                    anim.SetBool("Attack", true);

                    // 시간을 초기화한다.
                    currentTime = 0;

                    isWait = false;
                }
            }
        }
    }

    private void UpdateDodge()
    {
        isWait = false;
        state = BocoblinState.Move;
        // 애니메이션 실행
        anim.SetBool("Dodge", false);
        anim.SetBool("Move", true);
    }

    private void UpdateAttack()
    {
        // 애니메이션 실행
        anim.SetBool("Run", false);
        anim.SetBool("Attack", false);

        // 보코블린의 상태를 AttackWait 으로 바꾼다.
        state = BocoblinState.AttackWait;
    }

    void Attack()
    {
        print("@@@@@@@@@@@@@@@@@@@");
        // 링크의 데미지 함수를 호출한다.
        PlayerManager.instance.HP--;
    }

    void UpdateAttackWait()
    {
        currentTime += Time.deltaTime;

        #region 거리재기 및 바라보기
        // 거리를 구한다.
        Vector3 y = link.transform.position;
        y.y = transform.position.y;
        distance = Vector3.Distance(y, transform.position);

        // 링크가 있는 방향을 찾는다.
        Vector3 linkDir = link.transform.position - transform.position;
        linkDir.y = 0;
        linkDir.Normalize();

        // 그 방향을 바라본다.
        transform.forward = linkDir;
        #endregion

        // 다음 공격시간까지 대기하는 도중에 링크가 공격거리에서 멀어지면 Idle
        if (currentTime < 2 && distance > attackDistance)
        {
            // 상태를 Idle 로 바꾼다.
            state = BocoblinState.Idle;
            // 현재시간을 초기화한다.
            currentTime = 0;
            anim.SetBool("AttackWait", false);
        }

        // 4초가 지나면 다시 공격
        else if (currentTime >= 4)
        {
            //  다시 때린다.
            // 애니메이션
            anim.SetBool("AttackWait", true);
            anim.SetBool("Attack", true);
            // 상태를 Attack 으로 바꾼다.
            state = BocoblinState.Attack;
            // 현재시간을 초기화한다.
            currentTime = 0;
        }
    }

    public void DamagedProcess()
    {
        currentHP--;
        bocoAvatar.SetActive(false);
        bocoRagdoll.SetActive(true);

        if (currentHP > 0)
        {
            //애니메이션실행
            //anim.SetTrigger("Damaged");
            // 회전할 수 있게!
            //rb.freezeRotation = false;
            // rb.AddTorque(Random.insideUnitSphere * 100);

            state = BocoblinState.Air;
        }
        else if(currentHP <= 0)
        {
            // state = BocoblinState.Air;
            state = BocoblinState.Die;
        }
    }
    public SkinnedMeshRenderer bococlub;
    private void UpdateDie()
    {       
        // 1초 후에 파괴한다.
        Invoke("DieColor", 3);        
        Destroy(gameObject, 4);
        Invoke("DieEffect", 3.9f);
    }

    public GameObject dieEffectFactory;
    bool isEffect;

    public void DieColor()
    {
        // 보코블린의 몸을 까맣게 한다.
        SkinnedMeshRenderer[] mesh = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < mesh.Length; i++)
        {
            if (mesh[i] == bococlub)
            {
                continue;
            }
            mesh[i].materials[0].color = Color.black;
        }
        
    }
    public void DieEffect()
    {
        if (isEffect == false)
        {
            // 파괴할 때 검은 먼지 파티클시스템을 실행한다.
            GameObject dieEffect = Instantiate(dieEffectFactory);
            dieEffect.transform.position = bocoRagdoll.transform.position;
            isEffect = true;
        }
    }
    #endregion
}