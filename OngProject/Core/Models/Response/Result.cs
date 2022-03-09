using System.Collections.Generic;

namespace OngProject.Core.Models.Response
{
    public class Result
    {
        /// <summary>
        ///     if true, the request was completed successfully.
        /// </summary>
        /// <example>true</example>
        public bool Success { get; protected set; }

        /// <summary>
        ///     Description of the failure situation.
        /// </summary>
        /// <example>Description of failure.</example>
        public string FailureMessage { get; protected set; }

        /// <summary>
        ///     List of errors produced.
        /// </summary>
        /// <example>Description of the error.</example>
        public List<string> ErrorList { get; set; }

        /// <summary>
        ///     Reports the status code of request.
        /// </summary>
        /// <example>200</example>
        public int StatusCode { get; set; }

        protected Result()
        {
            this.Success = true;
            this.StatusCode = 200;
        }

        protected Result(string message)
        {
            this.Success = false;
            this.FailureMessage = message;
            this.StatusCode = 400;
        }

        protected Result(List<string> errorList)
        {
            this.Success = false;
            this.ErrorList = errorList;
            this.StatusCode = 500;
        }

        public static Result SuccessResult()
        {
            return new Result();
        }

        public static Result FailureResult(string message)
        {
            return new Result(message);
        }

        public static Result ErrorResult(List<string> errorList)
        {
            return new Result(errorList);
        }

        public static Result SuccessResult(int status)
        {            
            var result = SuccessResult();
            result.StatusCode = status;
            return result;
        }

        public static Result FailureResult(string message, int status)
        {
            var result = FailureResult(message);
            result.StatusCode = status;
            return result;
        }

        public static Result ErrorResult(List<string> errorList, int status)
        {
            var result = ErrorResult(errorList);
            result.StatusCode = status;
            return result;
        }

        public bool isError()
        {
            return ErrorList != null && ErrorList.Count > 0;
        }
    }

    public class Result<T> : Result
    {
        /// <summary>
        ///     Data resulting from the request.
        /// </summary>
        public T Data { get; protected set; }

        protected Result(T t)
        {
            this.Success = true;
            this.Data = t;
            this.StatusCode = 200;
        }

        public static Result SuccessResult(T t)
        {
            return new Result<T>(t);
        }

        public static Result SuccessResult(T t, int status)
        {
            var result = SuccessResult(t);
            result.StatusCode = status;
            return result;
        }
    }
}
