namespace API.Responses
{
    public class FrontResponse<T>
    {
        public T Value { get; set; }
        public string Error { get; set; }
        public FrontResponse(T v)
        {
            Value = v;
        }
        public FrontResponse(string msg)
        {
            Error = msg;
        }
    }
}
