using System.Collections.Generic;
using UnityEngine;

public class ThirdPartyDispatch : MonoBehaviour
{
    private static readonly object threadLock = new object();
    private readonly Queue<ThirdPartyParam> thirdPartyParamQueue = new Queue<ThirdPartyParam>(128);
    private int count = 0;

    private void Awake() 
    {
        lock (threadLock)
        {
            count = 0;
        }
    }
    public void Register(ThirdPartyParam InParam)
    {
        lock (threadLock)
        {
            thirdPartyParamQueue.Enqueue(InParam);

            count++;
        }
    }

    private void Update()
        {
            if (count > 0)
            {
                lock (threadLock)
                {
                    ThirdPartyParam param = thirdPartyParamQueue.Dequeue();
                    count--;
                    if (param.Callback == null)
                    {
                        // ThirdPartyDispatch.Handle(param.Type,string.IsNullOrEmpty(param.JsonData) ? null :param.JsonData);
                    }
                    else
                    {
                        // param.Callback.Invoke((int) param.Type, string.IsNullOrEmpty(param.JsonData) ? null : param.JsonData);
                    }
                }
            }
        }

}