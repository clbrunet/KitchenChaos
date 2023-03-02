using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;

    [SerializeField] private float moveSpeed = 7f;

    [SerializeField] private LayerMask countersLayerMask;
    private Vector3 lastInteractionsDirection = Vector3.zero;

    private bool isWalking = false;

    private void Update()
    {
        HandleMovements();
        HandleInteractions();
    }

    private void HandleMovements()
    {
        Vector2 inputVector = gameInput.GetNormalizedInputVector();
        isWalking = inputVector != Vector2.zero;
        if (!isWalking)
        {
            return;
        }
        const float ROTATE_SPEED = 10f;
        transform.forward = Vector3.Slerp(transform.forward, new Vector3(inputVector.x, 0f, inputVector.y), ROTATE_SPEED * Time.deltaTime);
        float moveDistance = moveSpeed * Time.deltaTime;
        TryMove(new Vector3(inputVector.x, 0f, 0f), moveDistance);
        TryMove(new Vector3(0f, 0f, inputVector.y), moveDistance);
    }

    private void TryMove(Vector3 moveDirection, float moveDistance)
    {
        const float PLAYER_HEIGHT = 2.1f;
        const float PLAYER_RADIUS = 0.65f;
        if (!Physics.CapsuleCast(transform.position + PLAYER_RADIUS * Vector3.up,
            transform.position + (PLAYER_HEIGHT - PLAYER_RADIUS) * Vector3.up, PLAYER_RADIUS, moveDirection, moveDistance))
        {
            transform.position += moveDistance * moveDirection;
        }
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetNormalizedInputVector();
        Vector3 direction = new(inputVector.x, 0f, inputVector.y);
        if (direction != Vector3.zero)
        {
            lastInteractionsDirection = direction;
        }
        const float INTERACTION_MAX_DISTANCE = 2f;
        if (!Physics.Raycast(transform.position, lastInteractionsDirection, out RaycastHit raycastHit, INTERACTION_MAX_DISTANCE, countersLayerMask)
            || !raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
        {
            return;
        }
        clearCounter.Interact();
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
