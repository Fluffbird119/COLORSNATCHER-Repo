using System;
using System.Collections;
using UnityEngine;

public interface CustomAttackInterface
{
    event Action<bool> OnCustomAttackCompletion;
    public IEnumerator RunAttack();
}
