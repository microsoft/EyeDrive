using System;
using System.Threading.Tasks;

namespace IDrive
{
    public interface IDrive
    {
        bool Connected { get; }
        Task<string> Version { get; }

        event EventHandler<Exception> CommunicationsError;
        event EventHandler<MessageEventArgs> ConnectionClosed;
        event EventHandler ConnectionOpened;

        bool Initialize();
        void Dispose();

        bool Forward();
        bool ForwardLeft();
        bool ForwardRight();
        bool Left();
        bool Reverse();
        bool ReverseLeft();
        bool ReverseRight();
        bool Right();
        bool Stop();
    }
}