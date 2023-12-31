using UnityEngine.UI;
using UnityEngine;

public class CoolTimer : MonoBehaviour
{
    #region 싱글톤
    static public CoolTimer instance;
    private void Awake()
    {
        instance = this; 
    }
    #endregion

    public Text text_CoolTime; // 쿨타임 텍스트를 표시할 UI 요소
    public Image image_fill; // fill type을 통해 연출할 이미지
    public float time_coolTime = 2; // 쿨타임 public으로 인스펙터에서 조절할 수 있게 했다.
    private float time_current; // 진행된 시간을 저장할 필드 변수
    private bool isEnded = true; // 종료 여부를 저장할 필드 변수

    public enum CoolTime
    {
        None,
        skill_cooltime
    }
    public CoolTime cooltime;
    private void Start()
    {
        cooltime=CoolTime.None;
    }
    private void Update() // 매 프레임 쿨타임을 체크한다.
    {
        if (isEnded)
            return;
        Check_CoolTime();
    }

    void Check_CoolTime()
    {
        time_current += Time.deltaTime; //증가한 시간을 더한다.
        if (time_current < time_coolTime) //아직 쿨타임이 안됐으면
        {
            Set_FillAmount(time_current); //이미지를 갱신한다.
        }
        else if (!isEnded)//쿨타임이 다됐는데 안끝났으면
        {
            End_CoolTime(); //쿨타임을 끝낸다.
        }
    }

    void End_CoolTime()
    {
        Set_FillAmount(time_coolTime); //이미지를 갱신한다.
        isEnded = true; //끝낸다.
        text_CoolTime.gameObject.SetActive(false); //텍스트도 지워준다.
        image_fill.gameObject.SetActive(false);

        //쿨타임 끝
        cooltime = CoolTime.None;
    }

    void Trigger_Skill()
    {
        if (!isEnded) return; //아직 쿨타임이면 안한다.

        Reset_CoolTime(); // 쿨타임을 돌린다.
        // 쿨타임 설정
        
    }

    void Reset_CoolTime()
    {
        image_fill.gameObject.SetActive(true);
        text_CoolTime.gameObject.SetActive(true);
        time_current = 0;
        Set_FillAmount(0);
        isEnded = false;
    }

    void Set_FillAmount(float value)
    {
        image_fill.fillAmount = value / time_coolTime;  // 쿨타임 설정.
        text_CoolTime.text = string.Format("Rest : {0}", value.ToString("0.0"));
    }

     public void on_Btn() //버튼 입력을 받아서 스킬을 시전한 걸로 친다.
    {
        Trigger_Skill();
       
    }
}