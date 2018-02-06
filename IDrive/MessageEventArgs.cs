namespace IDrive
{
    public class MessageEventArgs : System.EventArgs
    {
        public MessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
