using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Application.Common
{
    public class ResultViewModel<T>
    {
        private ResultViewModel(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = Error.None;
        }

        private ResultViewModel(Error error)
        {
            IsSuccess = false;
            Error = error;
        }


        public bool IsSuccess { get; }
        public T? Value { get; }
        public Error Error { get; }

        public static ResultViewModel<T> Success(T value) => new(value);
        public static ResultViewModel<T> Failure(Error error) => new(error);
    }
}
