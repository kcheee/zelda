using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    #region �̱���
    static public SkillUI instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject Skillpanel;
    public GameObject CrossHair;

    private void Start()
    {
        // ��ų â �ݱ�
        Skillpanel.SetActive(false);
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            // ��ųâ ���������� �ݰ� ���������� ����.
                if (!Skillpanel.activeSelf)
                    Skillpanel.SetActive(true);
           
        }

        #region CrossHair Onoff
        // CrossHair Onoff
        if (SkillManager.instance.skill_state == SkillManager.Skill_state.skill_bomb)
        {
            if (!CrossHair.activeSelf) CrossHair.SetActive(true);
        }
        else
            CrossHair.SetActive(false);
        #endregion

    }
}