using System;

public struct ThirdPartyParam
{
    public readonly CommandType Type;
    public readonly string JsonData;
    public readonly string ExtraData;
    public readonly Action<int,string> Callback;

    public ThirdPartyParam(CommandType InType,string InJsonData,string InExtraData,Action<int,string> InCallback = null)
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