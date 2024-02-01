using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Companion : AbstractStateBehaviour<CompanionState>
{
   private void Start() {
    ChangeState(CompanionState.Idle);
   }
}
