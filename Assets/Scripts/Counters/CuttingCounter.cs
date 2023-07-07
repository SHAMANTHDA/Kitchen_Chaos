using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;


    public event EventHandler<IHasProgress.OnProgreessChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipieSO[] cuttingRecipieSOArray;

    private int cuttingProgress;

    public override void Interact(Player player)
    {
        if (!hasKitchenObject())
        {
            //There is no kitchenObject here
            if (player.hasKitchenObject())
            {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //Player carrying somthing that can be Cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgreessChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipieSO.cuttingProgressMax
                    });
                }
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {
            // There is a kitchen objecte here;
            if (player.hasKitchenObject())
            {
                //Player is carrying something
                if (player.GetKitchenObject() is PlateKitchenObject)
                {
                    if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                    {
                        //Player is holding a plate
                        if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                        {
                            GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }

    }

    public override void InteractAlternate(Player player)
    {
        if (hasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            
            CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgreessChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipieSO.cuttingProgressMax
            });

            //There is a kitchen object here and it can be cut
            if (cuttingProgress >= cuttingRecipieSO.cuttingProgressMax)
            {
                KitchenObjectSO outputkitchenObjectSO = GetOutputforInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputkitchenObjectSO, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputkitchenObjectSO)
    {
        CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(inputkitchenObjectSO);
        return cuttingRecipieSO != null;
    }

    private KitchenObjectSO GetOutputforInput(KitchenObjectSO inputkitchenObjectSO)
    {
        CuttingRecipieSO cuttingRecipieSO = GetCuttingRecipieSOWithInput(inputkitchenObjectSO);
        if (cuttingRecipieSO != null)
        {
            return cuttingRecipieSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingRecipieSO GetCuttingRecipieSOWithInput(KitchenObjectSO inputkitchenObjectSO)
    {

        foreach (CuttingRecipieSO cuttingRecipieSO in cuttingRecipieSOArray)
        {
            if (cuttingRecipieSO.input == inputkitchenObjectSO)
            {
                return cuttingRecipieSO;
            }
        }
        return null;
    }
}

