using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    //private static Player instance;
    //public static Player Instance { get; private set;  }

    public event EventHandler OnPickedSomething;
    public static Player instanceField;
    public static Player GetInstanceField()
    {
        return instanceField;
    }

    public event EventHandler<OnSelectedCounterChangedEventArgs>  OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float movespeed = 7f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;


    private bool isWalking;
    private Vector3 lastInteraction;
    private BaseCounter selectedCounter;
    private KitchenObject KitchenObject;

    private void Awake()
    {
        //Instance = this;
    }
    private void Start()
    {
        GameInput.Instance.OnInteracActions += GameInput_OnInteractActions;
        GameInput.Instance.OnInteracAlternateActions += GameInput_OnInteracAlternateActions; ; 
    }

    private void GameInput_OnInteracAlternateActions(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractActions(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking()
    {
        return isWalking;
    }
    private void HandleInteractions()
    {
        Vector2 inputvector = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputvector.x, 0f, inputvector.y).normalized;

        if (moveDir != Vector3.zero)
        {
            lastInteraction = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteraction, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //Has clear counter
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }
    private void HandleMovement()
    {

        Vector2 inputvector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputvector.x, 0f, inputvector.y).normalized;

        float moveDistance = movespeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Cannot move towards moveDir

            // Attempt only X movement

            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Can move only on the X
                moveDir = moveDirX;
            }
            else
            {
                // Cannot move only on the X

                // Attempt only Z movement

                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
                else
                {
                    // Cannot move in any direction
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }


        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.KitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return KitchenObject;
    }

    public void ClearKitchenObject()
    {
        KitchenObject = null;
    }

    public bool hasKitchenObject()
    {
        return KitchenObject != null;
    }
}

