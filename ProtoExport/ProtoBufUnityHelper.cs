namespace GameFrameX.ProtoExport
{
    [Mode(ModeType.Unity)]
    internal class ProtoBufUnityHelper : ProtoBufCSharpBaseHelper
    {
        protected override string[] GetUsingStatements() =>
        [
            "using System;",
            "using ProtoBuf;",
            "using System.Collections.Generic;",
            "using GameFrameX.Network.Runtime;"
        ];

        protected override bool ShouldGenerateDescription => false;
    }
}
