using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text; //for testing

namespace SocketAsyncServer
{
    public class DataHoldingUserToken
    {
        public Mediator theMediator;
        public DataHolder theDataHolder;

        public Int32 socketHandleNumber;

        public readonly Int32 bufferOffsetReceive;
        public readonly Int32 permanentReceiveMessageOffset;
        public readonly Int32 bufferOffsetSend;
        
        private Int32 idOfThisObject; //for testing only        
               
        public  Int32 lengthOfCurrentIncomingMessage;
        
        //receiveMessageOffset is used to mark the byte position where the message
        //begins in the receive buffer. This value can sometimes be out of
        //bounds for the data stream just received. But, if it is out of bounds, the 
        //code will not access it.
        public Int32 receiveMessageOffset;        
        public Byte[] byteArrayForPrefix;        
        public readonly Int32 receivePrefixLength;
        public Int32 receivedPrefixBytesDoneCount = 0;
        public Int32 receivedMessageBytesDoneCount = 0;
        //This variable will be needed to calculate the value of the
        //receiveMessageOffset variable in one situation. Notice that the
        //name is similar but the usage is different from the variable
        //receiveSendToken.receivePrefixBytesDone.
        public Int32 recPrefixBytesDoneThisOp = 0;

        public Int32 sendBytesRemainingCount;
        public readonly Int32 sendPrefixLength;
        public Byte[] dataToSend;
        public Int32 bytesSentAlreadyCount;

        //The session ID correlates with all the data sent in a connected session.
        //It is different from the transmission ID in the DataHolder, which relates
        //to one TCP message. A connected session could have many messages, if you
        //set up your app to allow it.
        private Int32 sessionId;                

        public DataHoldingUserToken(SocketAsyncEventArgs e, Int32 rOffset, Int32 sOffset, Int32 receivePrefixLength, Int32 sendPrefixLength, Int32 identifier)
        {
            this.idOfThisObject = identifier;
           
            //Create a Mediator that has a reference to the SAEA object.
            this.theMediator = new Mediator(e);
            this.bufferOffsetReceive = rOffset;
            this.bufferOffsetSend = sOffset;
            this.receivePrefixLength = receivePrefixLength;
            this.sendPrefixLength = sendPrefixLength;
            this.receiveMessageOffset = rOffset + receivePrefixLength;
            this.permanentReceiveMessageOffset = this.receiveMessageOffset;            
        }

        //Let's use an ID for this object during testing, just so we can see what
        //is happening better if we want to.
        public Int32 TokenId
        {
            get
            {
                return this.idOfThisObject;
            }
        }

        public void CreateNewDataHolder()
        {
            theDataHolder = new DataHolder();
        }
                
        //Used to create sessionId variable in DataHoldingUserToken.
        //Called in ProcessAccept().
        public void CreateSessionId()
        {
            sessionId = Interlocked.Increment(ref Program.mainSessionId);                        
        }

        public Int32 SessionId
        {
            get
            {
                return this.sessionId;
            }
        }

        public void Reset()
        {
            this.receivedPrefixBytesDoneCount = 0;
            this.receivedMessageBytesDoneCount = 0;
            this.recPrefixBytesDoneThisOp = 0;
            this.receiveMessageOffset = this.permanentReceiveMessageOffset;
        }
    }
}
