namespace Backend.Utility
{
    //Questa classe é utile per gestire meglio i risultati dei metodi che recuperano i dati, se i dati sono stati trovati allora il metodo ritornerá un oggetto Result con IsOk true
    //E il dato conservato, se invece i dati non vengono trovati allora si restituirá un oggetto con IsOk a false e un ErrorMessage
    public class Result<T>
    {
        public bool IsOk { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }

        private Result(T data)
        {
            IsOk = true;
            Data = data;
        }

        private Result(string errorMessage)
        {
            IsOk = false;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(data);
        }
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T>(errorMessage);
        }
    }
}
