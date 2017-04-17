namespace Basf.Data
{
    public class ActionResponse
    {
        private static readonly ActionResponse successResponse = new ActionResponse(true);
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public static ActionResponse Success { get { return successResponse; } }
        protected ActionResponse(bool isSuccess, int code = 0, string message = null)
        {
            this.IsSuccess = isSuccess;
            this.Code = code;
            this.Message = message;
        }
        public static ActionResponse<T> Succeed<T>(T result = default(T))
        {
            return new ActionResponse<T>(true, result);
        }
        public static ActionResponse Fail(int code, string message)
        {
            return new ActionResponse(false, code, message);
        }
        public static ActionResponse<T> Fail<T>(int code, string message)
        {
            return new ActionResponse<T>(false, code, message, default(T));
        }
    }
    public class ActionResponse<T> : ActionResponse
    {
        public T Result { get; private set; }
        internal protected ActionResponse(bool isSuccess, T result)
           : base(isSuccess)
        {
            this.Result = result;
        }
        internal protected ActionResponse(bool isSuccess, int code, string message, T result)
            : base(isSuccess, code, message)
        {
            this.Result = result;
        }
    }
}
