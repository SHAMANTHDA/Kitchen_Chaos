using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO platekitchenObjectSO;
    private float spawnPlatetimer;
    private float spawnPlatetimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update()
    {
        spawnPlatetimer += Time.deltaTime;
        if (spawnPlatetimer > spawnPlatetimerMax)
        {
            spawnPlatetimer = 0f;

            if (platesSpawnedAmount < platesSpawnedAmountMax)
            {
                platesSpawnedAmount ++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.hasKitchenObject())
        {
            //Player is empty handed
            if (platesSpawnedAmount > 0)
            {
                //Theres atleast one plate here
                platesSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(platekitchenObjectSO, player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
