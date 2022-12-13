namespace Common.Exceptions
{
    public class ErrorResult
    {
        public virtual bool isSuccess { get; set; } 
        public virtual string code { get; set; }
        public virtual string message { get; set; }
        public virtual string version { get; set; }
    }
}
