namespace GameFrameX.ProtoExport
{
    [Mode(ModeType.Server)]
    internal class ProtoBufServerHelper : ProtoBufCSharpBaseHelper
    {
        protected override string[] GetUsingStatements() =>
        [
            "using System;",
            "using ProtoBuf;",
            "using System.Collections.Generic;",
            "using GameFrameX.NetWork.Abstractions;",
            "using GameFrameX.NetWork.Messages;"
        ];

        protected override bool ShouldGenerateDescription => true;
    }
}
