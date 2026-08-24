using UnityEngine;

public class PlayerAnimationStateController : MonoBehaviour
{
    Animator animator;

    float velocityY = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool backwardPressed = Input.GetKey("s");


        if (forwardPressed && velocityY < 2.0f)
        {
            velocityY += Time.deltaTime * acceleration;
        }

        if (backwardPressed && velocityY > -2.0f)
        {
            velocityY -= Time.deltaTime * acceleration;
        }

        if (leftPressed && velocityX > -2.0f)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (rightPressed && velocityX < 2.0f)
        {
            velocityX += Time.deltaTime * acceleration;
        }


        // decrease velocity Y
        if (!forwardPressed && velocityY > 0.0f)
        {
            velocityY -= Time.deltaTime * deceleration;
        }

        if (!backwardPressed && velocityY < 0.0f)
        {
            velocityY += Time.deltaTime * deceleration;
        }

        // reset velocityY
        if (!forwardPressed && !backwardPressed && velocityY != 0.0f && (velocityY > -2.0f && velocityY < 2.0f))
        {
            velocityX = 0.0f;
        }
        

        // decrease velocity X
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }

        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        // reset velocityX
        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -2.0f && velocityX < 2.0f))
        {
            velocityX = 0.0f;
        }

        animator.SetFloat("VelocityY", velocityY);
        animator.SetFloat("VelocityX", velocityX);
    }
}
