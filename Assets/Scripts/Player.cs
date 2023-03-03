using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameInput gameInput;

    [SerializeField] private float moveSpeed = 7f;

    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking = false;

    private ClearCounter selectedCounter = null;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs
    {
        public ClearCounter selectedCounter;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of Player");
        }
        Instance = this;
    }

    private void OnEnable()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void OnDisable()
    {
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
    }

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
        const float INTERACTION_MAX_DISTANCE = 2f;
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, INTERACTION_MAX_DISTANCE, countersLayerMask)
            || !raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
        {
            SetSelectedCounter(null);
            return;
        }
        SetSelectedCounter(clearCounter);
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(ClearCounter clearCounter)
    {
        if (selectedCounter == clearCounter)
        {
            return;
        }
        selectedCounter = clearCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = clearCounter });
    }
}
