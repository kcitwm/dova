using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


namespace SocketAsyncServer
{
    public class Mediator
    {
        
        private IncomingDataPreparer theIncomingDataPreparer;
        private OutgoingDataPreparer theOutgoingDataPreparer;
        private DataHolder theDataHolder;
        private SocketAsyncEventArgs saeaObject;
        private DataHoldingUserToken receiveSendToken;
       
        public Mediator(SocketAsyncEventArgs e)
        {
            
            this.saeaObject = e;
            this.theIncomingDataPreparer = new IncomingDataPreparer(saeaObject);
            this.theOutgoingDataPreparer = new OutgoingDataPreparer();            
        }

        
        public void HandleData(DataHolder incomingDataHolder)
        {   
            if (Program.watchProgramFlow == true)   //for testing
            {
                receiveSendToken = (DataHoldingUserToken)this.saeaObject.UserToken;
                Program.testWriter.WriteLine("Mediator HandleData() " + receiveSendToken.TokenId);
            }
            theDataHolder = theIncomingDataPreparer.HandleReceivedData(incomingDataHolder, this.saeaObject);
        }

        public void PrepareOutgoingData()
        { 
            theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder);            
        }


        public void PrepareOutgoingData(byte[] dataToSend)
        { 
            theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder, dataToSend);
        }

        public SocketAsyncEventArgs GiveBack()
        {
            if (Program.watchProgramFlow == true)   //for testing
            {
                receiveSendToken = (DataHoldingUserToken)this.saeaObject.UserToken;
                Program.testWriter.WriteLine("Mediator GiveBack() " + receiveSendToken.TokenId);
            }
            return saeaObject;
        }
    }
}
