using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : DecoratorNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    /// <summary>
    /// ¬Œ÷‚ğ¸”s‚ÉA¸”s‚ğ¬Œ÷‚É‚·‚é
    /// </summary>
    /// <returns></returns>
    protected override State OnUpdate() {

        return state switch
        {
            State.Running => State.Running,
            State.Success => State.Failure,
            State.Failure => State.Success,
            _ => State.Failure,
        };
    }
}