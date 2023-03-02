using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;

    [SerializeField] private float moveSpeed = 7f;
    private readonly float rotateSpeed = 10f;
    private readonly float playerHeight = 2.1f;
    private readonly float playerRadius = 0.65f;

    private bool isWalking = false;

    private void Update()
    {
        Vector2 inputVector = gameInput.GetNormalizedInputVector();
        isWalking = inputVector != Vector2.zero;
        if (!isWalking)
        {
            return;
        }
        transform.forward = Vector3.Slerp(transform.forward, new Vector3(inputVector.x, 0f, inputVector.y), rotateSpeed * Time.deltaTime);
        float moveDistance = moveSpeed * Time.deltaTime;
        TryMove(new Vector3(inputVector.x, 0f, 0f), moveDistance);
        TryMove(new Vector3(0f, 0f, inputVector.y), moveDistance);
    }

    private void TryMove(Vector3 moveDirection, float moveDistance)
    {
        bool canMove = !Physics.CapsuleCast(transform.position + playerRadius * Vector3.up,
            transform.position + (playerHeight - playerRadius) * Vector3.up, playerRadius, moveDirection, moveDistance);
        if (canMove) {
            transform.position += moveDistance * moveDirection;
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
