using System;
using System.Collections;

public interface CustomAttackInterface
{
    event Action<bool> OnCustomAttackCompletion;
    public IEnumerator RunAttack();
}
