using UnityEngine;
using System;

[Serializable]
public class NPCStats 
{
    [Header("Movement Properties")]
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _changDirectionInterval = 3.0f;
    [SerializeField] private float _stoppingDistance = 0.1f;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = Mathf.Max(0, value); }
    }

    public float ChangeDirectionInterval
    {
        get { return _changDirectionInterval; }
        set { _changDirectionInterval = Mathf.Max(1.0f, value); }
    }

    public float StoppingDistance
    {
        get { return _stoppingDistance; }
        set { _stoppingDistance = Mathf.Max(0.00f, value); }
    }

    //[Header("Resource Production")]
    //[SerializeField] private int _coinProductionRate = 10;

    //public int CoinProductionRate
    //{
    //    get { return _coinProductionRate; }
    //    set { _coinProductionRate = value; }
    //}
}
