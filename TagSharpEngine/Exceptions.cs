namespace TagSharpEngine.Error {
    /// <summary>
    /// Base exception class subclassed from Exception class.
    /// </summary> 
    public class TagSharpError: Exception {
        public TagSharpError(string? message): base(message) {}
    }
    
    public class CharLimitExceeded: TagSharpError {
        public CharLimitExceeded(string? message): base(message) {}
    }

    public class ProcessError: TagSharpError {
        public Exception Original;
        public Response Response;
        public Interpreter Interpreter;

        public ProcessError(Exception error, Response response, Interpreter interpreter): base(error.Message) {
            Original = error;
            Response = response;
            Interpreter = interpreter;
        }
    }

    public class StopError: TagSharpError {
        public StopError(string? message): base(message) {}
    }
}