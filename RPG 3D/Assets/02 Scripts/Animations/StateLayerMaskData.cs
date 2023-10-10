using UnityEngine;

[CreateAssetMenu(fileName = "new StateLayerMaskData", menuName = "RPG 3D/Animator/StateLayerMaskData")]
public class StateLayerMaskData : ScriptableObject 
{
    public UDictionary<State, AnimatorLayer> animatorLayerPairs;
}