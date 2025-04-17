using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using XInputDotNetPure;

public class Player : MonoBehaviour
{
    [SerializeField] private int Health = 4;
    public string PlayerEvolve;
    public string PlayerEvolveBack;
    public GameObject eye;
    public GameObject eyeZoom;
    public float Energy = 4f;
    public float maxEnergy = 4f;
    public float minEnergy = 0f;
    public bool canMove = true;
    public float energySpeed = 1f;
    private BloodController bloodController;
    private bool flag = false;
    private bool flagEye = false;
    private bool flagEyeZoom = false;
    private EnergyController energyController;
    public Coroutine autoSwitchToEatingCoroutine; // 跟踪自动切换协程
    public enum State{ Eating, Rebirth, Normal, Full, Over  }
    public State currentState = State.Normal;
    public Animator animator;
    [Header("ZOOMEVENT")]
    public UnityEvent OnFull;
    public UnityEvent OnEmpty;
    public UnityEvent OnHurt;
    public ParticleSystem particleTailing;
    private CircleCollider2D rb;
    public AudioSource audioSource_changeState;
    public AudioSource audioSource_eat;
    public AudioSource audioSource_hurt;
    public GameObject blood_last;
    PlayerIndex one = PlayerIndex.One;

    void Start()
    {
        rb = GetComponent<CircleCollider2D>();
        bloodController = GameObject.Find("UI_Manager").
            GetComponent<BloodController>();

        energyController = GameObject.Find("UI_Manager").
            GetComponent<EnergyController>();
        eye.SetActive(true);
        eyeZoom.SetActive(false);
        animator.SetBool("EvolvePlayer", false);
        animator.SetBool("EvolvePlayer(back)", false);
        particleTailing.Stop();

    }

    void Update()
    {
        energyChange();
        //player动画判断
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(PlayerEvolve) &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
                        && !flagEye)
        {
            eyeZoom.SetActive(true);
            flagEye = true;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(PlayerEvolveBack) &&
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
                        && !flagEyeZoom)
        {
            eye.SetActive(true);
            flagEyeZoom = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")|| collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                if (currentState == State.Normal && collision.gameObject.GetComponent<Bullet>().index == -1)
                {
                    HealthChange(-1);
                    
                }

                if(currentState == State.Eating || currentState == State.Full)
                {
                    StartShake(2f, 1f, 0.2f);
                    audioSource_eat.Play();
                    collision.gameObject.GetComponent<Bullet>().index = 0;
                    collision.gameObject.GetComponent<Bullet>().GetComponent<SpriteRenderer>().color =
                        new Color(246 / 255f, 183 / 255f, 27 / 255f);
                }
            }
            if(collision.gameObject.CompareTag("Enemy") && currentState == State.Normal)   HealthChange(-1);
        }
    }

    void HealthChange(int healthChange)
    {
        if( healthChange > 0)
        {
            Health++;
            bloodController.SpawnPrefab();
        }
        else
        {
            Health--;
            StartShake(2f, 1f, 0.3f);
            audioSource_hurt.Play();
            OnHurt?.Invoke();
            if (Health >= 1)
            {
                float temp_Energy = Energy;
                Player.State beforeState = currentState;
                if(temp_Energy + 1.5f > maxEnergy)
                {
                    beforeState = State.Eating;
                }
                else if(temp_Energy - 1.5f < minEnergy)
                {
                    beforeState = State.Normal;
                }
                bloodController.RemovePrefab();
                currentState = State.Rebirth;
                this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                //添加受击闪烁动画
                StartCoroutine(hurtFlash());

                StartCoroutine(ExecuteTask(1.5f, () => { currentState = beforeState; 
                        this.gameObject.GetComponent<CircleCollider2D>().enabled = true; })); //控制等待时间

            }
            if (Health <= 0)
            {
                currentState = State.Over;
                blood_last.SetActive(false);
                canMove = false;
            }
        }
    }

    void energyChange()
    {
        // Full状态下直接冻结所有逻辑
        if (currentState == State.Full)  return;

        if (currentState == State.Eating)
        {
            Energy -= energySpeed * Time.deltaTime;
            if (Energy <= minEnergy)
            {
                if (!flag)
                {
                    StartCoroutine(ExecuteTask(0.6f, () => { OnEmpty?.Invoke(); eyeZoom.SetActive(false); }));
                    audioSource_changeState.Play();
                    animator.SetBool("EvolvePlayer", false);
                    particleTailing.Stop();
                    animator.SetBool("EvolvePlayer(back)", true);
                    
                    flagEyeZoom = false;
                    flag = true;
                }
                Energy = minEnergy;
                StartCoroutine(ExecuteTask(1f, () =>
                {
                    currentState = State.Normal;
                    rb.radius = 1.25f;
                    flag = false;
                }));
            }
        }
        else if (currentState == State.Normal)
        {
            Energy += energySpeed * Time.deltaTime;
           
            if (Energy >= maxEnergy)
            {
                if (!flag)
                {
                    StartCoroutine(ExecuteTask(0.6f, () =>
                    {
                        OnFull?.Invoke();
                        audioSource_changeState.Play();
                         animator.SetBool("EvolvePlayer", true);
                        particleTailing.Play();
                        animator.SetBool("EvolvePlayer(back)", false);
                        
                        eye.SetActive(false);
                        flagEye = false;                        
                        energyController.AddEnergyBar();
                    }));
                       
                    flag = true;
                }
                Energy = maxEnergy;

                // 启动自动切换协程（可能被EnergyController停止）
                autoSwitchToEatingCoroutine = StartCoroutine(ExecuteTask(1f, () =>
                {
                    if (currentState != State.Full) // 仅在非Full状态时切换
                    {
                        currentState = State.Eating;
                        rb.radius = 1.7f;
                        flag = false;
                    }
                }));
            }
        }
    }

    private IEnumerator ExecuteTask(float waitTime, System.Action taskAction)
    {
        yield return new WaitForSeconds(waitTime);
        taskAction.Invoke();
    }

    private IEnumerator hurtFlash()
    {
        Material mt = this.GetComponent<SpriteRenderer>().material;
        float x = 1f;
        float maxAlphaValue = 1f;
        float alphaValue = maxAlphaValue / x;
        mt.SetFloat("_Alpha", alphaValue);
        yield return 0;
        while (mt.GetFloat("_Alpha") > 0)
        {
            x += 0.1f;
            alphaValue = maxAlphaValue / x;
            alphaValue = alphaValue < 0.02 ? 0 : alphaValue;
            mt.SetFloat("_Alpha", alphaValue);
            yield return 0;
        }
    }

    public void StartShake(float leftMotor,float rightMotor,float duration)
    {
        GamePad.SetVibration(one, leftMotor, rightMotor);
        Invoke("StopShake", duration);
    }
    void StopShake()
    {
        GamePad.SetVibration(one, 0, 0);
    }
}
