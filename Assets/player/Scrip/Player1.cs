using UnityEngine;
using System.Collections;


public class Player1 : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator anim;
    private bool canMove = true;
    private bool faceright = true;


    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public int Health = 5; //先自行设定成五吧


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0; // 关闭重力影响
    }

    private void Update()
    {
        if (canMove)
            movement();
        else
            rb.linearVelocity = Vector2.zero;
        AnimatorController();
        FlipController();
        DeathMod();
        test();
    }

    private void movement() //移动模组
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(horizontal, vertical).normalized * moveSpeed;
    }

    private void AnimatorController() //动画状态控制器
    {
        var moving = rb.linearVelocity.x != 0;
        anim.SetBool("moving", moving);
    }

    private void Flip()
    {
        faceright = !faceright;
        transform.Rotate(0, 180, 0);
    }

    private void FlipController() //翻转图片
    {
        if (rb.linearVelocity.x > 0 && !faceright)
            Flip();
        else if (rb.linearVelocity.x < 0 && faceright) Flip();
    }

    private void DeathMod() //死亡模组
    {
        if (Health <= 0)
        {
            anim.SetBool("alive", false);
            canMove = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll; //冻结当前位置 死亡就嗝屁不能键入wasd；
        }
        else
        {
            anim.SetBool("alive", true);
        }
    }

    public void test() //测试
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Health--;
            StartCoroutine(PlayHurtAnimation());
        }
    }

    private IEnumerator PlayHurtAnimation() //受伤状态的一个延时
    {
        anim.SetBool("Hurt", true);

        // 等待0.05秒
        yield return new WaitForSeconds(0.05f);

        // 0.05秒后关闭Hurt状态
        anim.SetBool("Hurt", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))

        {
            Health--;
            StartCoroutine(PlayHurtAnimation());
            // 触发扣血逻辑
        }
    }
}