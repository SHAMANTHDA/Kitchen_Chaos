using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_Gameobject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_Gameobject> kitchenObjectSOGameObjectList;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;

        foreach (KitchenObjectSO_Gameobject kitchenObjectSO_Gameobject in kitchenObjectSOGameObjectList)
        {
                kitchenObjectSO_Gameobject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSO_Gameobject kitchenObjectSO_Gameobject in kitchenObjectSOGameObjectList)
        {
            if (kitchenObjectSO_Gameobject.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjectSO_Gameobject.gameObject.SetActive(true);
            }

        }
    }
}
