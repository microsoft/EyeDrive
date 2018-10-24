using IDrive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeDrive
{
    public class NullDrive : IDrive.IDrive
    {
        public bool Connected => throw new NotImplementedException();

        public Task<string> Version => throw new NotImplementedException();

        public event EventHandler<Exception> CommunicationsError;
        public event EventHandler<MessageEventArgs> ConnectionClosed;
        public event EventHandler ConnectionOpened;

        public void Dispose()
        {
        }

        public bool Forward()
        {
            throw new NotImplementedException();
        }

        public bool ForwardLeft()
        {
            throw new NotImplementedException();
        }

        public bool ForwardRight()
        {
            throw new NotImplementedException();
        }

        public bool Initialize()
        {
            return true;
        }

        public bool Left()
        {
            throw new NotImplementedException();
        }

        public bool Reverse()
        {
            throw new NotImplementedException();
        }

        public bool ReverseLeft()
        {
            throw new NotImplementedException();
        }

        public bool ReverseRight()
        {
            throw new NotImplementedException();
        }

        public bool Right()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
