namespace GameFrameX.ProtoExport
{
    /// <summary>
    /// 协议生成帮助基类
    /// </summary>
    internal interface IProtoGenerateHelper
    {
        /// <summary>
        /// 开始生成
        /// </summary>
        /// <param name="inputPath">输入目录</param>
        /// <param name="outputPath">输出目录</param>
        /// <param name="namespaceName">命名空间</param>
        void Run(MessageInfoList inputPath, string outputPath, string namespaceName = "GFXHotfix");

        /// <summary>
        /// 生成后处理
        /// </summary>
        /// <param name="operationCodeInfo"></param>
        /// <param name="launcherOptionsOutputPath"></param>
        void Post(List<MessageInfoList> operationCodeInfo, string launcherOptionsOutputPath);
    }
}