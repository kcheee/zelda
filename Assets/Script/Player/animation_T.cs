using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class animation_T : MonoBehaviour
{
    #region �̱���
    static public animation_T instance;
    private void Awake()
    {
            instance = this;
    }
    #endregion

    public enum ani_state
    {
        idle,
        move,
        dash,
        run,
        attack
    }
    public ani_state state;
    public Animator animator;

    private void Start()
    {
        state = ani_state.idle;
    }

    private void Update()
    {
        // animator�� move ������ �� ���´� move�� �ٲ�.
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) state = ani_state.idle;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("move")) state = ani_state.move;

        if (Input.GetKeyDown(KeyCode.Mouse0)) animator.SetTrigger("attack");
        if (Input.GetKeyDown(KeyCode.Mouse1)) animator.SetTrigger("attack2");
    }

    private void Moving()
    {
        //if(state == ani_state.move) 
        //{
        //    animator.SetBool("move", true);
        //}
        //else animator.SetBool("move", false);

        //if (state == ani_state.dash)
        //{
        //    animator.SetBool("dash", true);       
        //}
        //else animator.SetBool("dash", false);

        //if (state == ani_state.run)
        //{
        //    animator.SetBool("run", true);
        //}
        //else animator.SetBool("run", false);
    }

}
