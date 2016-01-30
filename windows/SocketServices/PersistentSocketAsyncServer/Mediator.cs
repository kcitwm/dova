using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


namespace PersistentSocketAsyncServer
{
    public class Mediator
    {
        
        private IncomingDataPreparer theIncomingDataPreparer;
        private OutgoingDataPreparer theOutgoingDataPreparer;
        private DataHolder theDataHolder;
        private SocketAsyncEventArgs saeaObject;
        private SocketAsyncEventArgs saeaObjectSend; 
       
        public Mediator(SocketAsyncEventArgs e)
        {
            
            this.saeaObject = e;
            this.theIncomingDataPreparer = new IncomingDataPreparer(saeaObject);
            this.theOutgoingDataPreparer = new OutgoingDataPreparer();            
        }

        public Mediator(SocketAsyncEventArgs e, SocketAsyncEventArgs es)
        {

            this.saeaObject = e;
            saeaObjectSend = es;
            this.theIncomingDataPreparer = new IncomingDataPreparer(saeaObject);
            this.theOutgoingDataPreparer = new OutgoingDataPreparer();
        }

         

        
        public void HandleData(DataHolder incomingDataHolder)
        {   
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    receiveSendToken = (DataHoldingUserToken)this.saeaObject.UserToken;
            //    Program.testWriter.WriteLine("Mediator HandleData() " + receiveSendToken.TokenId);
            //}
            theDataHolder = theIncomingDataPreparer.HandleReceivedData(incomingDataHolder, this.saeaObject);
        }

        //public void PrepareOutgoingData()
        //{
        //    //if (Program.watchProgramFlow == true)   //for testing
        //    //{
        //    //    receiveSendToken = (DataHoldingUserToken)this.saeaObject.UserToken;
        //    //    Program.testWriter.WriteLine("Mediator PrepareOutgoingData() " + receiveSendToken.TokenId);
        //    //}

        //    theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder);            
        //}


        //public void PrepareOutgoingDataSend(byte[] dataToSend)
        //{ 
        //    //theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder, dataToSend); //原版本，没有加发送参数
        //    theOutgoingDataPreparer.PrepareOutgoingData(saeaObjectSend, theDataHolder, dataToSend);
        //}

        public void PrepareOutgoingData(byte[] dataToSend)
        {
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    receiveSendToken = (DataHoldingUserToken)this.saeaObjectSend.UserToken;
            //    Program.testWriter.WriteLine("Mediator PrepareOutgoingData() " + receiveSendToken.TokenId);
            //}

            //theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder, dataToSend); //原版本，没有加发送参数
            theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder, dataToSend);
        }


        public void PreparePureOutgoingData(byte[] dataToSend)
        {
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    receiveSendToken = (DataHoldingUserToken)this.saeaObjectSend.UserToken;
            //    Program.testWriter.WriteLine("Mediator PrepareOutgoingData() " + receiveSendToken.TokenId);
            //}

            //theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder, dataToSend); //原版本，没有加发送参数
            theOutgoingDataPreparer.PreparePureOutgoingData(saeaObject, theDataHolder, dataToSend);
        }


        public SocketAsyncEventArgs GiveBack()
        { 
            return saeaObject;
        }

        //public SocketAsyncEventArgs GiveBackSend()
        //{ 
        //    return saeaObjectSend;
        //}

        //public void SetSendObject(SocketAsyncEventArgs e)
        //{ 
        //    this.saeaObjectSend = e;
        //}


    }
}
