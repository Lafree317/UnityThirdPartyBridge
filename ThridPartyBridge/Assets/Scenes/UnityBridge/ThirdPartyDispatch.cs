using System.Collections.Generic;
using UnityEngine;

public class ThirdPartyDispatch : MonoBehaviour
{
    private static readonly object threadLock = new object();
    private readonly Queue<ThirdPartyParam> thirdPartyParam = new Queue<ThirdPartyParam>(128);
    private int count = 0;

    private void Awake() 
    {
        
    }
}