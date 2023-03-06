using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameInput gameInput;

    [SerializeField] private float moveSpeed = 7f;

    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking = false;

    private BaseCounter selectedCounter = null;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private Transform kitchenObjectHoldPoint;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        Assert.IsNull(Instance, "Multiple instances of Player");
        Instance = this;
    }

    private void OnEnable()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void OnDisable()
    {
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction -= GameInput_OnInteractAlternateAction;
    }

    private void Update()
    {
        HandleMovements();
        HandleCounterSelection();
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

    private void HandleCounterSelection()
    {
        const float INTERACTION_MAX_DISTANCE = 2f;
        if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, INTERACTION_MAX_DISTANCE, countersLayerMask)
            || !raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
        {
            SetSelectedCounter(null);
            return;
        }
        SetSelectedCounter(baseCounter);
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter baseCounter)
    {
        if (selectedCounter == baseCounter)
        {
            return;
        }
        selectedCounter = baseCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { selectedCounter = baseCounter });
    }

    public Transform GetKitchenObjectParent()
    {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
