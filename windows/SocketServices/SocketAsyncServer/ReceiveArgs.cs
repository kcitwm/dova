﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketAsyncServer
{
    public class ReceiveArgs : EventArgs
    {
        public ReceiveArgs(DataHoldingUserToken token)
        {
            this.Token = token;
        }

        public DataHoldingUserToken Token;
        public byte[] DataToSend;
    }
}
