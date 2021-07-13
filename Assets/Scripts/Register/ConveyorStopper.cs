using System;
using Register;
using UnityEngine;

public class ConveyorStopper : MonoBehaviour
{
    [SerializeField] bool useTrigger;
    [SerializeField] ConveyorBelt conveyorBelt;

    int numInTrigger;

    void OnTriggerEnter(Collider other) => UpdateNumInTrigger(1, other);
    void OnTriggerExit(Collider other) => UpdateNumInTrigger(-1, other);

    void UpdateNumInTrigger(int add, Collider other)
    {
        if (!useTrigger || other.transform == conveyorBelt.transform) return;
        
        if (numInTrigger + add < 0)
            throw new InvalidOperationException("Tried to update number of items in trigger to a number below zero.");
        numInTrigger += add;
        conveyorBelt.enabled = numInTrigger == 0;
    }

    public void Resume() => conveyorBelt.enabled = true;
    public void Stop() => conveyorBelt.enabled = false;
}
