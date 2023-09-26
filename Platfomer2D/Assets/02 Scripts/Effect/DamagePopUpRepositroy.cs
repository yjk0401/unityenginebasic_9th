using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageRepository : MonoBehaviour 
{
    public static DamageRepository Instance 
    {
        get 
        {
            if (_instance == null) 
            {
                _instance = Instantiate(Resources.Load<DamageRepository>("DamageRepository"));
            }
            return _instance;
        }
    }
    private static DamageRepository _instance;

    [Serializable]
    public struct AssetPair 
    {
        public LayerMask layerMask;
        public DamagePopUp damagePopUp;
    }

    [SerializeField] private List<AssetPair> _assets;

    public DamagePopUp GetDamagePopUp(int layer) 
    {
        return _assets.Find(x => (x.layerMask & 1 << layer) > 0).damagePopUp;
    }
}