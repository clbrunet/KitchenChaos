using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    //public static Player Instance { get; private set; }

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

    public event EventHandler OnObjectPickup;

    private void Awake()
    {
        //Assert.IsNull(Instance, "Multiple instances of Player");
        //Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying())
        {
            return;
        }
        HandleMovements();
        HandleCounterSelection();
    }

    private void HandleMovements()
    {
        Vector2 inputVector = GameInput.Instance.GetNormalizedInputVector();
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
        if (!GameManager.Instance.IsPlaying() || Time.timeScale == 0f)
        {
            return;
        }
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsPlaying() || Time.timeScale == 0f)
        {
            return;
        }
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    public bool IsWalking()
    {
        if (!GameManager.Instance.IsPlaying())
        {
            return false;
        }
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
        if (kitchenObject != null)
        {
            OnObjectPickup?.Invoke(this, EventArgs.Empty);
        }
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
