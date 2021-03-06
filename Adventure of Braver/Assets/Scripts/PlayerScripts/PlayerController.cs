﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    float walkSpeed = 0, runSpeed = 0, jumpHeight = 0, strafeSpeed = 0;

    [Range(0, 1)]
    public float airControlPercent;

    [SerializeField]
    float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    [SerializeField]
    float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    Transform myTransform;
    Animator animator;
    Transform cameraT;
    CharacterController controller;
    Targeting targeting;

    public float gravity = -12;

    DialogueTrigger dialogueTrigger;

    // Use this for initialization
    void Start () {
        targeting = GetComponent<Targeting>();
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        myTransform = transform;
        dialogueTrigger = FindObjectOfType<DialogueTrigger>();
	}
	
	// Update is called once per frame
	void Update () {
        
        //インプット
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        bool running;
        if( Input.GetAxis("Dash") > 0){
            running = true;
        } else {
            running = false;
        }
        Move(inputDir, running);

        //ジャンプ
        if(Input.GetAxis("Jump") > 0)
        {
            Jump();
        }

        //ストレイフ
        if (targeting.isTargeting)
        {
            Strafe();
        }

        //アニメーション
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        if( dialogueTrigger.isTalking){
            animationSpeedPercent = 0f;
        }
    }

    //移動メソッド
    void Move( Vector2 inputDir, bool running)
    {
        if (dialogueTrigger.isTalking)
        {
            return;
        }
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocityY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
        {
            velocityY = 0;
        }
    }

    //ジャンプメソッド
    void Jump()
    {
        if(controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }

    //ジャンプ時移動スピード変更するメソッド
    float GetModifiedSmoothTime( float smoothTime)
    {
        if( controller.isGrounded)
        {
            return smoothTime;
        }

        if ( airControlPercent == 0)
        {
            return float.MaxValue;
        }

        return smoothTime / airControlPercent;
    }

    //ストレイフメソッド
    void Strafe()
    {
        if ((Input.GetAxis("Horizontal")) > 0)
        {
            //anim here
            controller.SimpleMove(myTransform.TransformDirection(Vector3.right) * Input.GetAxis("Horizontal") * strafeSpeed);
        }
        else if ((Input.GetAxis("Horizontal")) < 0)
        {
            //anim here
            controller.SimpleMove(myTransform.TransformDirection(Vector3.right) * Input.GetAxis("Horizontal") * strafeSpeed);
        }
    }
}
