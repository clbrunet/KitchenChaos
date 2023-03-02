using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;

    [SerializeField] private float moveSpeed = 7f;
    private readonly float rotateSpeed = 10f;

    private bool isWalking = false;

    private void Update()
    {
        Vector2 inputVector = gameInput.GetNormalizedInputVector();
        Vector3 moveDirection = new(inputVector.x, 0f, inputVector.y);
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
        isWalking = moveDirection != Vector3.zero;
        if (isWalking)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
