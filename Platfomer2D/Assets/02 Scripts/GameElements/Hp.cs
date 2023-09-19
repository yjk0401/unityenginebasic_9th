using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Hp
{
    float hpValue { get; }
    float hpMax { get; }
    float hpMin { get; }

    void RecoverHp(object subject, float amount);
    void DepleteHp(object subject, float amount);

    event Action<float> onHpChanged;
    event Action<float> onHpRecovered;
    event Action<float> onHpDepleted;
    event Action onHpMax;
    event Action onHpMin;

}
