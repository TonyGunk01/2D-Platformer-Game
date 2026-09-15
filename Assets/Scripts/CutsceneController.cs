using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("Path")]
    public GameObject pointA;

    [Header("Characters")]
    public GameObject chomper;
    public GameObject player;
    public GameObject coin;

    public float chomperSpeed = 10f;
    public float playerSpeed = 10f;

    private bool movingToCoin = true;
    private bool movingToPointA = false;
    private Animator playerAnimator;
    private Animator chomperAnimator;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (chomper == null)
            chomper = GameObject.FindGameObjectWithTag("Enemy");

        if (coin == null)
            coin = GameObject.FindGameObjectWithTag("Coin");

        if (player != null)
            playerAnimator = player.GetComponent<Animator>();

        if (chomper != null)
            chomperAnimator = chomper.GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null || coin == null || pointA == null || chomper == null)
            return;

        if (movingToCoin)
        {
            Vector3 playerPos = player.transform.position;
            Vector3 coinPos = coin.transform.position;
            float distX = Mathf.Abs(playerPos.x - coinPos.x);

            if (distX > 1f)
            {
                MoveCharacterTowards(player, coinPos, playerSpeed);
                SetAnimatorRunning(playerAnimator, true);
            }

            else
            {
                movingToCoin = false;
                movingToPointA = true;
            }
        }

        if (movingToPointA)
        {
            Vector3 target = pointA.transform.position;

            MoveCharacterTowards(player, target, playerSpeed);
            SetAnimatorRunning(playerAnimator, true);

            MoveCharacterTowards(chomper, target, chomperSpeed);

            float pDistX = Mathf.Abs(player.transform.position.x - target.x);
            float cDistX = Mathf.Abs(chomper.transform.position.x - target.x);

            if (pDistX < 0.1f && cDistX < 0.1f)
            {
                movingToPointA = false;
                SetAnimatorRunning(playerAnimator, false);
                SetAnimatorRunning(chomperAnimator, false);

                SceneManager.LoadScene("Home Page");
            }
        }
    }

    private void MoveCharacterTowards(GameObject obj, Vector3 target, float speed)
    {
        if (obj == null)
            return;

        Vector3 pos = obj.transform.position;
        Vector3 next = Vector3.MoveTowards(pos, new Vector3(target.x, pos.y, pos.z), speed * Time.deltaTime);
        obj.transform.position = next;

        float dx = target.x - pos.x;

        if (Mathf.Abs(dx) > 0.001f)
        {
            Vector3 scale = obj.transform.localScale;
            scale.x = Mathf.Sign(dx) * Mathf.Abs(scale.x);
            obj.transform.localScale = scale;
        }
    }

    private void SetAnimatorRunning(Animator anim, bool running)
    {
        if (anim == null)
            return;

        if (AnimatorHasParameter(anim, "isRunning", AnimatorControllerParameterType.Bool))
            anim.SetBool("isRunning", running);

        else if (AnimatorHasParameter(anim, "Run", AnimatorControllerParameterType.Bool))
            anim.SetBool("Run", running);

        else if (AnimatorHasParameter(anim, "Speed", AnimatorControllerParameterType.Float))
            anim.SetFloat("Speed", running ? 1f : 0f);
    }

    private bool AnimatorHasParameter(Animator anim, string name, AnimatorControllerParameterType type)
    {
        if (anim == null)
            return false;

        foreach (var p in anim.parameters)
        {
            if (p.type == type && p.name == name)
                return true;
        }

        return false;
    }
}