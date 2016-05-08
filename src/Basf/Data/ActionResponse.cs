namespace Basf.Data
{
    public class ActionResponse
    {
        private static readonly ActionResponse successResponse = new ActionResponse(ActionResult.Success);
        public ActionResult Result { get; protected set; }
        /// <summary>
        /// 使用各种自定义的枚举来定义各自的错误代码
        /// </summary>
        public int Code { get; protected set; }
        public string Message { get; protected set; }
        public string Detail { get; protected set; }
        public static ActionResponse Success { get { return successResponse; } }
        protected ActionResponse(ActionResult result, int code = 0, string message = null, string detail = null)
        {
            this.Result = result;
            this.Code = code;
            this.Message = message;
            this.Detail = detail;
        }
        public static ActionResponse<T> Succeed<T>(T result = default(T))
        {
            return new ActionResponse<T>(ActionResult.Success, result);
        }
        public static ActionResponse Fail(int code, string message, string detail = null)
        {
            return new ActionResponse(ActionResult.Failed, code, message, detail);
        }
        public static ActionResponse<T> Fail<T>(int code, string message, string detail = null)
        {
            return new ActionResponse<T>(ActionResult.Failed, code, message, detail, default(T));
        }
    }
    public class ActionResponse<T> : ActionResponse
    {
        public T ReturnData { get; private set; }
        internal protected ActionResponse(ActionResult result, T returnData)
           : base(result)
        {
            this.ReturnData = returnData;
        }
        internal protected ActionResponse(ActionResult result, int code, string message, string detail, T returnData)
            : base(result, code, message, detail)
        {
            this.ReturnData = returnData;
        }
    }
}
