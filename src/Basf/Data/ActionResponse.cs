namespace Basf.Data
{
    public class ActionResponse
    {
        public PromiseResult Promise { get; protected set; }
        public string Message { get; protected set; }
        public string Detail { get; protected set; }
        protected ActionResponse(PromiseResult promise, string message = null, string detail = null)
        {
            this.Promise = promise;
            this.Message = message;
            this.Detail = detail;
        }
        public static ActionResponse Success()
        {
            return new ActionResponse(PromiseResult.Resolved);
        }
        public static ActionResponse<T> Success<T>(T result = default(T))
        {
            return new ActionResponse<T>(PromiseResult.Resolved, result);
        }
        public static ActionResponse Fail(string message, string detail = null)
        {
            return new ActionResponse(PromiseResult.Rejected, message, detail);
        }
        public static ActionResponse<T> Fail<T>(string message, string detail = null)
        {
            return new ActionResponse<T>(PromiseResult.Rejected, message, detail, default(T));
        }
    }
    public class ActionResponse<T> : ActionResponse
    {
        public T Result { get; private set; }
        internal protected ActionResponse(PromiseResult promise, T result)
           : base(promise)
        {
            this.Result = result;
        }
        internal protected ActionResponse(PromiseResult promise, string message, string detail, T result)
            : base(promise, message, detail)
        {
            this.Result = result;
        }
    }
}
