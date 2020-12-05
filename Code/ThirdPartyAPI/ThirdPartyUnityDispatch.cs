using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Ada.ThirdParty.Plugins
{
    public struct ThirdPartyParam
    {
        public readonly CommandType Type;
        public readonly string JsonData;
        public readonly string ExtraData;
        public readonly Action<int, JSONNode, string> Callback;

        public ThirdPartyParam(CommandType InType, string InJsonData, string InExtraData)
        {
            Type = InType;
            JsonData = InJsonData;
            ExtraData = InExtraData;
            Callback = null;
        }

        public ThirdPartyParam(CommandType InType, string InJsonData, string InExtraData,
            Action<int, JSONNode, string> InCallback)
        {
            Type = InType;
            JsonData = InJsonData;
            ExtraData = InExtraData;
            Callback = InCallback;
        }

        public override string ToString()
        {
            return $"Command type = {Type} - JsonData = {JsonData} - ExtraData = {ExtraData} - {Callback == null}";
        }
    }

    public class ThirdPartyUnityDispatch : MonoBehaviour
    {
        private static readonly object _threadLock = new object();
        private readonly Queue<ThirdPartyParam> thirdPartyParams = new Queue<ThirdPartyParam>(128);
        private int count;

        private void Awake()
        {
            lock (_threadLock)
            {
                count = 0;
            }
        }

        public void Register(ThirdPartyParam InParam)
        {
            lock (_threadLock)
            {
//                Log.Info(
//                    $"ThirdPartyUnityDispatch register param\nCommandType = {InParam.Type}\nJsonData = {InParam.JsonData}\nExtra = {InParam.ExtraData}\nHas callback = {InParam.Callback != null}\nThread id = {Thread.CurrentThread.ManagedThreadId}",
//                    ColorName.Tan);

                thirdPartyParams.Enqueue(InParam);

                ++count;
            }
        }

        private void Update()
        {
            if (count > 0)
            {
                lock (_threadLock)
                {
                    ThirdPartyParam param = thirdPartyParams.Dequeue();

                    --count;
//                    Log.Info(
//                        $"ThirdPartyUnityDispatch Dispatch param\nCommandType = {param.Type}\nJsonData = {param.JsonData}\nExtra = {param.ExtraData}\nHas callback = {param.Callback != null}\nThread id = {Thread.CurrentThread.ManagedThreadId}",
//                        ColorName.Tan);


                    if (param.Callback == null)
                    {
                        ThirdPartyDispatch.Handle(param.Type,
                            string.IsNullOrEmpty(param.JsonData) ? null : JSONNode.Parse(param.JsonData),
                            param.ExtraData);
                    }
                    else
                    {
                        param.Callback.Invoke((int) param.Type,
                            string.IsNullOrEmpty(param.JsonData) ? null : JSONNode.Parse(param.JsonData),
                            param.ExtraData);
                    }
                }
            }
        }
    }
}