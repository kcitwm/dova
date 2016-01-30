using System;
using System.IO;
using System.Collections.Generic; //for testing
using System.Net.Sockets;
using System.Threading; //for Semaphore and Interlocked
using System.Net;
using System.Text; //for testing
using System.Diagnostics;
using Dova.Utility; //for testing



namespace PersistentSocketAsyncServer
{   //____________________________________________________________________________
    // Implements the logic for the socket server.  

    public class SocketListener
    {
        //__variables for testing ____________________________________________

        //total clients connected to the server, excluding backlog
        public Int32 numberOfAcceptedSockets;

        static Dictionary<string, SocketListener> listeners = new Dictionary<string, SocketListener>();

        public event EventHandler<DataHolder> DataReceived;

        protected virtual void OnDataReceived(DataHolder e)
        {
            if (null != DataReceived)
            {
                DataReceived(this, e);
            }

        }

        //****for testing threads
        //Process theProcess; //for testing only
        //ProcessThreadCollection arrayOfLiveThreadsInThisProcess;   //for testing
        //HashSet<int> managedThreadIds = new HashSet<int>();  //for testing
        //HashSet<Thread> managedThreads = new HashSet<Thread>();  //for testing        
        //object that will be used to lock the HashSet of thread references 
        //that we use for testing.
        private object lockerForThreadHashSet = new object();
        //****end variables for displaying what's happening with threads        
        //__END variables for testing ____________________________________________

        //__variables that might be used in a  real app__________________________________

        //Buffers for sockets are unmanaged by .NET. 
        //So memory used for buffers gets "pinned", which makes the
        //.NET garbage collector work around it, fragmenting the memory. 
        //Circumvent this problem by putting all buffers together 
        //in one block in memory. Then we will assign a part of that space 
        //to each SocketAsyncEventArgs object, and
        //reuse that buffer space each time we reuse the SocketAsyncEventArgs object.
        //Create a large reusable set of buffers for all socket operations.
        BufferManager theBufferManager;
        //BufferManager theBufferManagerSend;

        // the socket used to listen for incoming connection requests
        Socket listenSocket;

        //A Semaphore has two parameters, the initial number of available slots
        // and the maximum number of slots. We'll make them the same. 
        //This Semaphore is used to keep from going over max connection #. (It is not about 
        //controlling threading really here.)   
        //Semaphore theMaxConnectionsEnforcer;

        SocketListenerSettings socketListenerSettings;

        PrefixHandler prefixHandler;
        MessageHandler messageHandler;

        // pool of reusable SocketAsyncEventArgs objects for accept operations
        SocketAsyncEventArgsPool poolOfAcceptEventArgs;
        // pool of reusable SocketAsyncEventArgs objects for receive and send socket operations
        SocketAsyncEventArgsPool poolOfRecEventArgs;
        //SocketAsyncEventArgsPool poolOfSendEventArgs;
        //__END variables for real app____________________________________________

        //_______________________________________________________________________________
        // Constructor.
        public SocketListener(SocketListenerSettings theSocketListenerSettings)
        {
            string key = theSocketListenerSettings.LocalEndPoint.ToString();
            if (listeners.ContainsKey(key)) return;
            this.numberOfAcceptedSockets = 0; //for testing
            this.socketListenerSettings = theSocketListenerSettings;
            this.prefixHandler = new PrefixHandler();
            this.messageHandler = new MessageHandler();

            //Allocate memory for buffers. We are using a separate buffer space for
            //receive and send, instead of sharing the buffer space, like the Microsoft
            //example does.            
            this.theBufferManager = new BufferManager(this.socketListenerSettings.BufferSize * this.socketListenerSettings.NumberOfSaeaForRecSend * this.socketListenerSettings.OpsToPreAllocate,
            this.socketListenerSettings.BufferSize * this.socketListenerSettings.OpsToPreAllocate);
            //this.theBufferManagerSend = new BufferManager(this.socketListenerSettings.BufferSize * this.socketListenerSettings.NumberOfSaeaForRecSend * this.socketListenerSettings.OpsToPreAllocate,
            //this.socketListenerSettings.BufferSize * this.socketListenerSettings.OpsToPreAllocate);
            this.poolOfRecEventArgs = new SocketAsyncEventArgsPool(this.socketListenerSettings.NumberOfSaeaForRecSend);
            //this.poolOfSendEventArgs = new SocketAsyncEventArgsPool(this.socketListenerSettings.NumberOfSaeaForRecSend);
            this.poolOfAcceptEventArgs = new SocketAsyncEventArgsPool(this.socketListenerSettings.MaxAcceptOps);

            // Create connections count enforcer
            //this.theMaxConnectionsEnforcer = new Semaphore(this.socketListenerSettings.MaxConnections, this.socketListenerSettings.MaxConnections);

            //Microsoft's example called these from Main method, which you 
            //can easily do if you wish.
            Init();
            StartListen();
            listeners.Add(key, this);
            //instance = this;
        }

        static SocketListener instance = null;

        public static SocketListener GetInstance(string endpoint)
        {
            return listeners[endpoint];
        }



        //____________________________________________________________________________
        // initializes the server by preallocating reusable buffers and 
        // context objects (SocketAsyncEventArgs objects).  
        //It is NOT mandatory that you preallocate them or reuse them. But, but it is 
        //done this way to illustrate how the API can 
        // easily be used to create reusable objects to increase server performance.

        public void Init()
        {
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("Init method");
            //}
            //if (Program.watchThreads == true)   //for testing
            //{
            //    DealWithThreadsForTesting("Init()");
            //}

            // Allocate one large byte buffer block, which all I/O operations will 
            //use a piece of. This gaurds against memory fragmentation.
            this.theBufferManager.InitBuffer();
            //this.theBufferManagerSend.InitBuffer();

            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("Starting creation of accept SocketAsyncEventArgs pool:");
            //}

            // preallocate pool of SocketAsyncEventArgs objects for accept operations           
            for (Int32 i = 0; i < this.socketListenerSettings.MaxAcceptOps; i++)
            {
                // add SocketAsyncEventArg to the pool
                this.poolOfAcceptEventArgs.Push(CreateNewSaeaForAccept(poolOfAcceptEventArgs));
            }

            //The pool that we built ABOVE is for SocketAsyncEventArgs objects that do
            // accept operations. 
            //Now we will build a separate pool for SAEAs objects 
            //that do receive/send operations. One reason to separate them is that accept
            //operations do NOT need a buffer, but receive/send operations do. 
            //ReceiveAsync and SendAsync require
            //a parameter for buffer size in SocketAsyncEventArgs.Buffer.
            // So, create pool of SAEA objects for receive/send operations.
            SocketAsyncEventArgs eventArgRecObjectForPool;
            //SocketAsyncEventArgs eventArgSendObjectForPool;

            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("Starting creation of receive/send SocketAsyncEventArgs pool");
            //}

            Int32 tokenId;

            for (Int32 i = 0; i < this.socketListenerSettings.NumberOfSaeaForRecSend; i++)
            {
                //Allocate the SocketAsyncEventArgs object for this loop, 
                //to go in its place in the stack which will be the pool
                //for receive/send operation context objects.
                eventArgRecObjectForPool = new SocketAsyncEventArgs();
                //eventArgSendObjectForPool = new SocketAsyncEventArgs();

                // assign a byte buffer from the buffer block to 
                //this particular SocketAsyncEventArg object
                this.theBufferManager.SetBuffer(eventArgRecObjectForPool);
                //this.theBufferManagerSend.SetBuffer(eventArgSendObjectForPool);
                int tid = poolOfRecEventArgs.AssignTokenId();
                tokenId = tid + 1000000;

                //Attach the SocketAsyncEventArgs object
                //to its event handler. Since this SocketAsyncEventArgs object is 
                //used for both receive and send operations, whenever either of those 
                //completes, the IO_Completed method will be called.
                eventArgRecObjectForPool.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                //eventArgSendObjectForPool.Completed += new EventHandler<SocketAsyncEventArgs>(IO_CompletedSend);

                //We can store data in the UserToken property of SAEA object.
                // DataHoldingUserToken theTempReceiveSendUserToken = new DataHoldingUserToken(eventArgRecObjectForPool, eventArgRecObjectForPool.Offset, eventArgRecObjectForPool.Offset + this.socketListenerSettings.BufferSize, this.socketListenerSettings.ReceivePrefixLength, this.socketListenerSettings.SendPrefixLength, tokenId);
                DataHoldingUserToken theTempReceiveSendUserToken = new DataHoldingUserToken(eventArgRecObjectForPool, eventArgRecObjectForPool.Offset, eventArgRecObjectForPool.Offset + this.socketListenerSettings.BufferSize, this.socketListenerSettings.ReceivePrefixLength, this.socketListenerSettings.SendPrefixLength, tokenId);

                //We'll have an object that we call DataHolder, that we can remove from
                //the UserToken when we are finished with it. So, we can hang on to the
                //DataHolder, pass it to an app, serialize it, or whatever.
                theTempReceiveSendUserToken.CreateNewDataHolder();

                eventArgRecObjectForPool.UserToken = theTempReceiveSendUserToken;
                //eventArgSendObjectForPool.UserToken = theTempReceiveSendUserToken;

                // add this SocketAsyncEventArg object to the pool.
                this.poolOfRecEventArgs.Push(eventArgRecObjectForPool);
                //this.poolOfSendEventArgs.Push(eventArgSendObjectForPool);
            }
        }

        //____________________________________________________________________________
        // This method is called when we need to create a new SAEA object to do
        //accept operations. The reason to put it in a separate method is so that
        //we can easily add more objects to the pool if we need to.
        //You can do that if you do NOT use a buffer in the SAEA object that does
        //the accept operations.
        public SocketAsyncEventArgs CreateNewSaeaForAccept(SocketAsyncEventArgsPool pool)
        {
            //Allocate the SocketAsyncEventArgs object. 
            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();

            //SocketAsyncEventArgs.Completed is an event, (the only event,) 
            //declared in the SocketAsyncEventArgs class.
            //See http://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.completed.aspx.
            //An event handler should be attached to the event within 
            //a SocketAsyncEventArgs instance when an asynchronous socket 
            //operation is initiated, otherwise the application will not be able 
            //to determine when the operation completes.
            //Attach the event handler, which causes the calling of the 
            //AcceptEventArg_Completed object when the accept op completes.
            acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);

            AcceptOpUserToken theAcceptOpToken = new AcceptOpUserToken(pool.AssignTokenId() + 10000);
            acceptEventArg.UserToken = theAcceptOpToken;

            return acceptEventArg;

            // accept operations do NOT need a buffer.                
            //You can see that is true by looking at the
            //methods in the .NET Socket class on the Microsoft website. AcceptAsync does
            //not take require a parameter for buffer size.
        }

        //____________________________________________________________________________
        // This method starts the socket server such that it is listening for 
        // incoming connection requests.            
        public void StartListen()
        {
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("StartListen method. Before Listen operation is started.");
            //}
            //if (Program.watchThreads == true)   //for testing
            //{
            //    DealWithThreadsForTesting("StartListen()");
            //}

            // create the socket which listens for incoming connections
            listenSocket = new Socket(this.socketListenerSettings.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            byte[] beatValue = new byte[] { 1, 0, 0, 0, 0x88, 0x13, 0, 0, 0x88, 0x13, 0, 0 };
            listenSocket.IOControl(IOControlCode.KeepAliveValues, beatValue, null);
            //bind it to the port 
            listenSocket.Bind(this.socketListenerSettings.LocalEndPoint);

            // Start the listener with a backlog of however many connections.
            //"backlog" means pending connections. 
            //The backlog number is the number of clients that can wait for a
            //SocketAsyncEventArg object that will do an accept operation.
            //The listening socket keeps the backlog as a queue. The backlog allows 
            //for a certain # of excess clients waiting to be connected.
            //If the backlog is maxed out, then the client will receive an error when
            //trying to connect.
            //max # for backlog can be limited by the operating system.
            listenSocket.Listen(this.socketListenerSettings.Backlog);

            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("StartListen method Listen operation was just started.");
            //}
            //Console.WriteLine("\r\n\r\n*************************\r\n** Server is listening **\r\n*************************\r\n\r\nAfter you are finished, type 'Z' and press\r\nEnter key to terminate the server process.\r\nIf you terminate it by clicking X on the Console,\r\nthen the log will NOT write correctly.\r\n");
            Log.Write("\r\n\r\n*************************\r\n** Server is listening **\r\n*************************\r\n\r\n");

            // Calls the method which will post accepts on the listening socket.            
            // This call just occurs one time from this StartListen method. 
            // After that the StartAccept method will be called in a loop.
            StartAccept();
        }

        //____________________________________________________________________________
        // Begins an operation to accept a connection request from the client         
        public void StartAccept()
        {
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("StartAccept method");
            //}
            Log.Write("StartAccept method");
            SocketAsyncEventArgs acceptEventArg;

            //Get a SocketAsyncEventArgs object to accept the connection.                        
            //Get it from the pool if there is more than one in the pool.
            //We could use zero as bottom, but one is a little safer.            
            if (this.poolOfAcceptEventArgs.Count > 1)
            {
                try
                {
                    acceptEventArg = this.poolOfAcceptEventArgs.Pop();
                }
                //or make a new one.
                catch
                {
                    acceptEventArg = CreateNewSaeaForAccept(poolOfAcceptEventArgs);
                }
            }
            //or make a new one.
            else
            {
                acceptEventArg = CreateNewSaeaForAccept(poolOfAcceptEventArgs);
            }



            //if (Program.watchThreads == true)   //for testing
            //{
            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
            //    DealWithThreadsForTesting("StartAccept()", theAcceptOpToken);
            //}
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
            //    Program.testWriter.WriteLine("still in StartAccept, id = " + theAcceptOpToken.TokenId);
            //}
            Log.Write("still in StartAccept, id = " + ((AcceptOpUserToken)acceptEventArg.UserToken).TokenId);
            //Semaphore class is used to control access to a resource or pool of 
            //resources. Enter the semaphore by calling the WaitOne method, which is 
            //inherited from the WaitHandle class, and release the semaphore 
            //by calling the Release method. This is a mechanism to prevent exceeding
            // the max # of connections we specified. We'll do this before
            // doing AcceptAsync. If maxConnections value has been reached,
            //then the application will pause here until the Semaphore gets released,
            //which happens in the CloseClientSocket method.            
            //this.theMaxConnectionsEnforcer.WaitOne();

            //Socket.AcceptAsync begins asynchronous operation to accept the connection.
            //Note the listening socket will pass info to the SocketAsyncEventArgs
            //object that has the Socket that does the accept operation.
            //If you do not create a Socket object and put it in the SAEA object
            //before calling AcceptAsync and use the AcceptSocket property to get it,
            //then a new Socket object will be created for you by .NET.            
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
            //Socket.AcceptAsync returns true if the I/O operation is pending, i.e. is 
            //working asynchronously. The 
            //SocketAsyncEventArgs.Completed event on the acceptEventArg parameter 
            //will be raised upon completion of accept op.
            //AcceptAsync will call the AcceptEventArg_Completed
            //method when it completes, because when we created this SocketAsyncEventArgs
            //object before putting it in the pool, we set the event handler to do it.
            //AcceptAsync returns false if the I/O operation completed synchronously.            
            //The SocketAsyncEventArgs.Completed event on the acceptEventArg 
            //parameter will NOT be raised when AcceptAsync returns false.
            if (!willRaiseEvent)
            {
                //if (Program.watchProgramFlow == true)   //for testing
                //{
                //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;

                //    Program.testWriter.WriteLine("StartAccept in if (!willRaiseEvent), accept token id " + theAcceptOpToken.TokenId);
                //}

                //The code in this if (!willRaiseEvent) statement only runs 
                //when the operation was completed synchronously. It is needed because 
                //when Socket.AcceptAsync returns false, 
                //it does NOT raise the SocketAsyncEventArgs.Completed event.
                //And we need to call ProcessAccept and pass it the SAEA object.
                //This is only when a new connection is being accepted.
                // Probably only relevant in the case of a socket error.
                ProcessAccept(acceptEventArg);
            }
        }

        //____________________________________________________________________________
        // This method is the callback method associated with Socket.AcceptAsync 
        // operations and is invoked when an async accept operation completes.
        // This is only when a new connection is being accepted.
        // Notice that Socket.AcceptAsync is returning a value of true, and
        // raising the Completed event when the AcceptAsync method completes.
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            //Any code that you put in this method will NOT be called if
            //the operation completes synchronously, which will probably happen when
            //there is some kind of socket error. It might be better to put the code
            //in the ProcessAccept method.

            //if (Program.watchProgramFlow == true)   //for testing
            //{

            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)e.UserToken;
            //    Program.testWriter.WriteLine("AcceptEventArg_Completed, id " + theAcceptOpToken.TokenId);
            //}

            //if (Program.watchThreads == true)   //for testing
            //{
            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)e.UserToken;
            //    DealWithThreadsForTesting("AcceptEventArg_Completed()", theAcceptOpToken);
            //}
            Log.Write("AcceptEventArg_Completed, id " + ((AcceptOpUserToken)e.UserToken).TokenId);
            ProcessAccept(e);
        }

        //____________________________________________________________________________       
        //The e parameter passed from the AcceptEventArg_Completed method
        //represents the SocketAsyncEventArgs object that did
        //the accept operation. in this method we'll do the handoff from it to the 
        //SocketAsyncEventArgs object that will do receive/send.
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            // This is when there was an error with the accept op. That should NOT
            // be happening often. It could indicate that there is a problem with
            // that socket. If there is a problem, then we would have an infinite
            // loop here, if we tried to reuse that same socket.
            if (acceptEventArgs.SocketError != SocketError.Success)
            {
                // Loop back to post another accept op. Notice that we are NOT
                // passing the SAEA object here.
                LoopToStartAccept();

                AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
                Log.Write("SocketError, accept id " + theAcceptOpToken.TokenId);

                //Let's destroy this socket, since it could be bad.
                HandleBadAccept(acceptEventArgs);

                //Jump out of the method.
                return;
            }

            Int32 max = Program.maxSimultaneousClientsThatWereConnected;
            Int32 numberOfConnectedSockets = Interlocked.Increment(ref this.numberOfAcceptedSockets);
            if (numberOfConnectedSockets > max)
            {
                Interlocked.Increment(ref Program.maxSimultaneousClientsThatWereConnected);
            }

            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
            //    Program.testWriter.WriteLine("ProcessAccept, accept id " + theAcceptOpToken.TokenId);
            //}
            Log.Write("SocketError, accept id " + ((AcceptOpUserToken)acceptEventArgs.UserToken).TokenId);



            //Now that the accept operation completed, we can start another
            //accept operation, which will do the same. Notice that we are NOT
            //passing the SAEA object here.
            LoopToStartAccept();

            // Get a SocketAsyncEventArgs object from the pool of receive/send op 
            //SocketAsyncEventArgs objects 

            //IPEndPoint point = (IPEndPoint)acceptEventArgs.AcceptSocket.RemoteEndPoint;
            //string clientpoint=point.ToString();



            SocketAsyncEventArgs receiveEventArgs = this.poolOfRecEventArgs.Pop();
            //Create sessionId in UserToken.
            DataHoldingUserToken token = ((DataHoldingUserToken)receiveEventArgs.UserToken);
            token.CreateSessionId();
            //SocketAsyncEventArgs sendEventArgs = token.theMediator.GiveBackSend();// this.poolOfSendEventArgs.Pop();  

            //A new socket was created by the AcceptAsync method. The 
            //SocketAsyncEventArgs object which did the accept operation has that 
            //socket info in its AcceptSocket property. Now we will give
            //a reference for that socket to the SocketAsyncEventArgs 
            //object which will do receive/send.
            receiveEventArgs.AcceptSocket = acceptEventArgs.AcceptSocket;
            //sendEventArgs.AcceptSocket = acceptEventArgs.AcceptSocket;

            //if ((Program.watchProgramFlow == true) || (Program.watchConnectAndDisconnect == true))
            //{
            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
            //    Program.testWriter.WriteLine("Accept id " + theAcceptOpToken.TokenId + ". RecSend id " + ((DataHoldingUserToken)receiveEventArgs.UserToken).TokenId + ".  Remote endpoint = " + IPAddress.Parse(((IPEndPoint)receiveEventArgs.AcceptSocket.RemoteEndPoint).Address.ToString()) + ": " + ((IPEndPoint)receiveEventArgs.AcceptSocket.RemoteEndPoint).Port.ToString() + ". client(s) connected = " + this.numberOfAcceptedSockets);
            //}
            //if (Program.watchThreads == true)   //for testing
            //{
            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
            //    theAcceptOpToken.socketHandleNumber = (Int32)acceptEventArgs.AcceptSocket.Handle;
            //    DealWithThreadsForTesting("ProcessAccept()", theAcceptOpToken);
            //    ((DataHoldingUserToken)receiveEventArgs.UserToken).socketHandleNumber = (Int32)receiveEventArgs.AcceptSocket.Handle;
            //}

            //We have handed off the connection info from the
            //accepting socket to the receiving socket. So, now we can
            //put the SocketAsyncEventArgs object that did the accept operation 
            //back in the pool for them. But first we will clear 
            //the socket info from that object, so it will be 
            //ready for a new socket when it comes out of the pool.
            acceptEventArgs.AcceptSocket = null;
            this.poolOfAcceptEventArgs.Push(acceptEventArgs);

            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
            //    Program.testWriter.WriteLine("back to poolOfAcceptEventArgs goes accept id " + theAcceptOpToken.TokenId);
            //}

            StartReceive(receiveEventArgs);
        }

        //____________________________________________________________________________
        //LoopToStartAccept method just sends us back to the beginning of the 
        //StartAccept method, to start the next accept operation on the next 
        //connection request that this listening socket will pass of to an 
        //accepting socket. We do NOT actually need this method. You could
        //just call StartAccept() in ProcessAccept() where we called LoopToStartAccept().
        //This method is just here to help you visualize the program flow.
        private void LoopToStartAccept()
        {
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("LoopToStartAccept");
            //}

            StartAccept();
        }


        //____________________________________________________________________________
        // Set the receive buffer and post a receive op.
        private void StartReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            try
            {

                DataHoldingUserToken receiveToken = (DataHoldingUserToken)receiveEventArgs.UserToken;
                //Set the buffer for the receive operation.
                Log.Info("StartReceive:开始接收数据:" + receiveToken.TokenId);
                if (null == receiveEventArgs.AcceptSocket || !receiveEventArgs.AcceptSocket.Connected)
                {
                    CloseClientSocket(receiveEventArgs, true);
                    return;
                }
                receiveEventArgs.SetBuffer(receiveToken.bufferOffsetReceive, this.socketListenerSettings.BufferSize);

                // Post async receive operation on the socket.  
                bool willRaiseEvent = receiveEventArgs.AcceptSocket.ReceiveAsync(receiveEventArgs);

                //Socket.ReceiveAsync returns true if the I/O operation is pending. The 
                //SocketAsyncEventArgs.Completed event on the e parameter will be raised 
                //upon completion of the operation. So, true will cause the IO_Completed
                //method to be called when the receive operation completes. 
                //That's because of the event handler we created when building
                //the pool of SocketAsyncEventArgs objects that perform receive/send.
                //It was the line that said
                //eventArgObjectForPool.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

                //Socket.ReceiveAsync returns false if I/O operation completed synchronously. 
                //In that case, the SocketAsyncEventArgs.Completed event on the e parameter 
                //will not be raised and the e object passed as a parameter may be 
                //examined immediately after the method call 
                //returns to retrieve the result of the operation.
                // It may be false in the case of a socket error.
                if (!willRaiseEvent)
                {
                    //If the op completed synchronously, we need to call ProcessReceive 
                    //method directly. This will probably be used rarely, as you will 
                    //see in testing. 
                    ProcessReceive(receiveEventArgs);
                }
            }
            catch (Exception e)
            {
                Log.Error("SocketListener.StartReceive:" + e.Message);
                CloseClientSocket(receiveEventArgs, true);
            }
        }

        //____________________________________________________________________________
        // This method is called whenever a receive or send operation completes.
        // Here "e" represents the SocketAsyncEventArgs object associated 
        //with the completed receive or send operation
        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            //Any code that you put in this method will NOT be called if
            //the operation completes synchronously, which will probably happen when
            //there is some kind of socket error.
            DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)e.UserToken;
            int n = e.BytesTransferred;
            Log.Info("IO_Completed.receiveSendToken.TokenId:" + receiveSendToken.TokenId);
            // determine which type of operation just completed and call the associated handler
            //lock (receiveSendToken)
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    default:
                        //This exception will occur if you code the Completed event of some
                        //operation to come to this method, by mistake.
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
        }

        void IO_CompletedSend(object sender, SocketAsyncEventArgs e)
        {
            //Any code that you put in this method will NOT be called if
            //the operation completes synchronously, which will probably happen when
            //there is some kind of socket error.
            DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)e.UserToken;
            Log.Info("IO_CompletedSend.receiveSendToken.TokenId:" + receiveSendToken.TokenId);
            //lock (receiveSendToken)
            {
                // determine which type of operation just completed and call the associated handler
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    default:
                        //This exception will occur if you code the Completed event of some
                        //operation to come to this method, by mistake.
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }
            }
        }




        //____________________________________________________________________________
        // This method is invoked by the IO_Completed method
        // when an asynchronous receive operation completes. 
        // If the remote host closed the connection, then the socket is closed.
        // Otherwise, we process the received data. And if a complete message was
        // received, then we do some additional processing, to 
        // respond to the client.
        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)receiveEventArgs.UserToken;
            // If there was a socket error, close the connection. This is NOT a normal
            // situation, if you get an error here.
            // In the Microsoft example code they had this error situation handled
            // at the end of ProcessReceive. Putting it here improves readability
            // by reducing nesting some.
            if (receiveEventArgs.SocketError != SocketError.Success)
            {
                //receiveSendToken.Reset();//原版本
                CloseClientSocket(receiveEventArgs, false);
                return;
            }

            // If no data was received, close the connection. This is a NORMAL
            // situation that shows when the client has finished sending data.
            if (receiveEventArgs.BytesTransferred == 0)
            {
                //receiveSendToken.Reset(); 
                CloseClientSocket(receiveEventArgs, false);
                return;
            }

            //The BytesTransferred property tells us how many bytes 
            //we need to process.
            Int32 remainingBytesToProcess = receiveEventArgs.BytesTransferred;

            //If we have not got all of the prefix already, 
            //then we need to work on it here.      
            Log.Info("ProcessReceive.remainingBytesToProcess:1需要收取的数据长度为:" + remainingBytesToProcess + "receiveSendToken.receivedPrefixBytesDoneCount:" + receiveSendToken.receivedPrefixBytesDoneCount + "socketListenerSettings.ReceivePrefixLength:" + socketListenerSettings.ReceivePrefixLength);
            if (receiveSendToken.receivedPrefixBytesDoneCount < this.socketListenerSettings.ReceivePrefixLength)
            {
                remainingBytesToProcess = prefixHandler.HandlePrefix(receiveEventArgs, receiveSendToken, remainingBytesToProcess);
                Log.Info("ProcessReceive.remainingBytesToProcess:2需要收取的数据长度为:" + remainingBytesToProcess);
                if (remainingBytesToProcess == 0 || remainingBytesToProcess == -100)//-100表示心跳包
                {
                    // We need to do another receive op, since we do not have
                    // the message yet, but remainingBytesToProcess == 0.
                    if (remainingBytesToProcess == -100)
                    {
                        SocketAsyncEventArgs sendEventArgs = this.poolOfRecEventArgs.Pop();
                        sendEventArgs.AcceptSocket = receiveEventArgs.AcceptSocket;
                        ((DataHoldingUserToken)sendEventArgs.UserToken).userTokenId = receiveSendToken.userTokenId;
                        SendPureData(sendEventArgs, BitConverter.GetBytes(-100));
                    }
                    receiveSendToken.Reset();
                    //receiveSendToken.Reset();
                    StartReceive(receiveEventArgs);
                    receiveSendToken.CreateNewDataHolder(); 
                    //Jump out of the method.
                    return;
                }
            }


            // Log.Write("ProcessReceive:receiveSendToken.receivedPrefixBytesDoneCount:" + receiveSendToken.receivedPrefixBytesDoneCount + " this.socketListenerSettings.ReceivePrefixLength:" + this.socketListenerSettings.ReceivePrefixLength);

            // If we have processed the prefix, we can work on the message now.
            // We'll arrive here when we have received enough bytes to read
            // the first byte after the prefix. 

            bool incomingTcpMessageIsReady = false;
            try
            {
                incomingTcpMessageIsReady = messageHandler.HandleMessage(receiveEventArgs, receiveSendToken, remainingBytesToProcess);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (incomingTcpMessageIsReady == true)
            {
                //Reset the variables in the UserToken, to be ready for the
                //next message that will be received on the socket in this
                //SAEA object.
                // Pass the DataHolder object to the Mediator here. The data in
                // this DataHolder can be used for all kinds of things that an
                // intelligent and creative person like you might think of.  
                //新版本
                //SocketAsyncEventArgs sendEventArgs = this.poolOfRecEventArgs.Pop();
                //sendEventArgs.AcceptSocket = receiveEventArgs.AcceptSocket;
                //sendEventArgs.UserToken = receiveSendToken;
                //receiveSendToken.theMediator.SetSendObject(sendEventArgs);
                OnDataReceived(receiveSendToken.theDataHolder);
                receiveSendToken.theMediator.HandleData(receiveSendToken.theDataHolder);
                SocketAsyncEventArgs sendEventArgs = this.poolOfRecEventArgs.Pop();
                sendEventArgs.AcceptSocket = receiveEventArgs.AcceptSocket;
                ((DataHoldingUserToken)sendEventArgs.UserToken).userTokenId = receiveSendToken.userTokenId;
                AddToken(receiveSendToken.userTokenId, receiveEventArgs);
                byte[] sendData = receiveSendToken.theDataHolder.dataMessageSend;
                receiveSendToken.Reset();
                receiveSendToken.CreateNewDataHolder();
                //新版本
                //旧版本
                //ReceiveArgs ra = new ReceiveArgs(receiveSendToken);
                //OnDataReceived(ra);
                //旧版本 
                // Create a new DataHolder for next message. 

                //receiveSendToken.theMediator.PrepareOutgoingData(receiveSendToken.theDataHolder.dataMessageSend);
                //原版本
                //StartSend(receiveSendToken.theMediator.GiveBack());
                //原版本 
                //新版本  
                if (null != sendData)
                    Send(sendEventArgs, sendData);
                //StartSend(receiveSendToken.theMediator.GiveBack());

            }
            else
            {
                // Since we have NOT gotten enough bytes for the whole message,
                // we need to do another receive op. Reset some variables first.

                // All of the data that we receive in the next receive op will be
                // message. None of it will be prefix. So, we need to move the 
                // receiveSendToken.receiveMessageOffset to the beginning of the 
                // receive buffer space for this SAEA. 

                receiveSendToken.receiveMessageOffset = receiveSendToken.bufferOffsetReceive;

                // Do NOT reset receiveSendToken.receivedPrefixBytesDoneCount here.
                // Just reset recPrefixBytesDoneThisOp.
                receiveSendToken.recPrefixBytesDoneThisOp = 0;
            }
            StartReceive(receiveEventArgs);
        }

        //____________________________________________________________________________
        //Post a send.    

        //private void StartSend(SocketAsyncEventArgs sendEventArgs)
        //{
        //    StartSend(sendEventArgs);
        //}

        private bool StartSend(SocketAsyncEventArgs sendEventArgs)
        {
            DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)sendEventArgs.UserToken;
            Log.Info("StartSend.开始发消息:" + receiveSendToken.userTokenId);
            // 版本1
            //Send(sendEventArgs.AcceptSocket, receiveSendToken.dataToSend);
            //receiveSendToken.ResetSend();
            //if (continueReceive)
            //    StartReceive(sendEventArgs);
            // return;
            //版本1  

            //sendEventArgs.SetBuffer(receiveSendToken.bufferOffsetSend, this.socketListenerSettings.BufferSize);

            //Set the buffer. You can see on Microsoft's page at 
            //http://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.setbuffer.aspx
            //that there are two overloads. One of the overloads has 3 parameters.
            //When setting the buffer, you need 3 parameters the first time you set it,
            //which we did in the Init method. The first of the three parameters
            //tells what byte array to use as the buffer. After we tell what byte array
            //to use we do not need to use the overload with 3 parameters any more.
            //(That is the whole reason for using the buffer block. You keep the same
            //byte array as buffer always, and keep it all in one block.)
            //Now we use the overload with two parameters. We tell 
            // (1) the offset and
            // (2) the number of bytes to use, starting at the offset.

            //The number of bytes to send depends on whether the message is larger than
            //the buffer or not. If it is larger than the buffer, then we will have
            //to post more than one send operation. If it is less than or equal to the
            //size of the send buffer, then we can accomplish it in one send op.
            if (receiveSendToken.sendBytesRemainingCount <= this.socketListenerSettings.BufferSize)
            {
                try
                {
                    sendEventArgs.SetBuffer(receiveSendToken.bufferOffsetSend, receiveSendToken.sendBytesRemainingCount);
                    //Copy the bytes to the buffer associated with this SAEA object.
                    Buffer.BlockCopy(receiveSendToken.dataToSend, receiveSendToken.bytesSentAlreadyCount, sendEventArgs.Buffer, receiveSendToken.bufferOffsetSend, receiveSendToken.sendBytesRemainingCount);
                }
                catch (Exception e)
                {
                    Log.Error("StartSend:" + e.Message);
                    //throw;
                    return false;
                }
            }
            else
            {
                //We cannot try to set the buffer any larger than its size.
                //So since receiveSendToken.sendBytesRemainingCount > BufferSize, we just
                //set it to the maximum size, to send the most data possible.
                sendEventArgs.SetBuffer(receiveSendToken.bufferOffsetSend, this.socketListenerSettings.BufferSize);
                //Copy the bytes to the buffer associated with this SAEA object.
                Buffer.BlockCopy(receiveSendToken.dataToSend, receiveSendToken.bytesSentAlreadyCount, sendEventArgs.Buffer, receiveSendToken.bufferOffsetSend, this.socketListenerSettings.BufferSize);

                //We'll change the value of sendUserToken.sendBytesRemainingCount
                //in the ProcessSend method.
            }

            //post asynchronous send operation  
            bool willRaiseEvent = sendEventArgs.AcceptSocket.SendAsync(sendEventArgs);
            Log.Info("异步发送结束:willRaiseEvent:" + willRaiseEvent);
            if (!willRaiseEvent)
            {
                ProcessSend(sendEventArgs);
                //    StartReceive(sendEventArgs);
            }
            //else
            //{
            //    receiveSendToken.ResetSend(); 
            //    receiveSendToken.CreateNewDataHolder();
            //    sendEventArgs.AcceptSocket = null;
            //    poolOfRecEventArgs.Push(sendEventArgs);
            //}
            return true;

        }

        public bool Send(string target, byte[] data)
        {
            SocketAsyncEventArgs receiveEventArgs = poolOfRecEventArgs.GetWithToken(target);
            if (null != receiveEventArgs)
            {
                Log.Info("获取到发送对象:" + target);
                SocketAsyncEventArgs sendEventArgs = this.poolOfRecEventArgs.Pop();
                sendEventArgs.AcceptSocket = receiveEventArgs.AcceptSocket;
                //把发送的对象赋给发送事件参数的userTokenId
                ((DataHoldingUserToken)sendEventArgs.UserToken).userTokenId = target;
                return Send(sendEventArgs, data);
            }
            return false;
        }

        public bool Send(SocketAsyncEventArgs sendEventArgs, byte[] data)
        {
            //原版本
            //SocketAsyncEventArgs sendEventArgs = this.poolOfRecEventArgs.GetWithToken(target);
            //原版本
            //新版本  
            //新版本
            try
            {
                DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)sendEventArgs.UserToken;
                if (null != receiveSendToken)
                {
                    Log.Info("开始给对象发消息:" + receiveSendToken.userTokenId);
                    //原版本
                    //receiveSendToken.theMediator.PrepareOutgoingData(data);
                    //原版本
                    //新版本
                    receiveSendToken.theMediator.PrepareOutgoingData(data);
                    //新版本
                    //原版本
                    //CloseClientSocket(sendEventArgs);
                    //原版本
                    //新版本
                    //新版本
                    return StartSend(sendEventArgs);
                    //receiveSendToken.ResetSend();
                }
            }
            catch (Exception e)
            {
                Log.Info("Send:" + e.Message);
                CloseClientSocket(sendEventArgs, true);
                return false;
            }
            return true;
        }


        public bool SendPureData(SocketAsyncEventArgs sendEventArgs, byte[] data)
        {
            //原版本
            //SocketAsyncEventArgs sendEventArgs = this.poolOfRecEventArgs.GetWithToken(target);
            //原版本
            //新版本  
            //新版本
            try
            {
                DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)sendEventArgs.UserToken;
                if (null != receiveSendToken)
                {
                    Log.Info("开始给对象发消息:" + receiveSendToken.userTokenId);
                    //原版本
                    //receiveSendToken.theMediator.PrepareOutgoingData(data);
                    //原版本
                    //新版本
                    receiveSendToken.theMediator.PreparePureOutgoingData(data);
                    //新版本
                    //原版本
                    //CloseClientSocket(sendEventArgs);
                    //原版本
                    //新版本
                    //新版本
                    return StartSend(sendEventArgs);
                    //receiveSendToken.ResetSend();
                }
            }
            catch (Exception e)
            {
                Log.Info("Send:" + e.Message);
                CloseClientSocket(sendEventArgs, true);
                return false;
            }
            return true;
        }



        #region 同步发送
        //public int Send(Socket s, byte[] data)
        //{
        //    int total = 0;
        //    int size = data.Length;
        //    Log.Info("准备发送数据长度:data.Length:" + size);
        //    int dataLeft = size;
        //    int sent;
        //    while (total < size)
        //    {
        //        sent = s.Send(data, total, dataLeft, SocketFlags.None);
        //        total += sent;
        //        dataLeft -= sent;
        //    }
        //    Log.Info("已经发送数据长度:data.Length:" + total);
        //    return total;
        //}


        //public int SendVar(Socket s, byte[] data, int lenHeader)
        //{
        //    int total = 0;
        //    int size = data.Length;
        //    int sent;

        //    byte[] dataSize = new byte[lenHeader];
        //    dataSize = BitConverter.GetBytes(size);

        //    byte[] all = new byte[dataSize.Length + data.Length];
        //    Buffer.BlockCopy(dataSize, 0, all, 0, dataSize.Length);
        //    Buffer.BlockCopy(data, 0, all, dataSize.Length, data.Length);
        //    int len = all.Length;
        //    int dataLeft = len;
        //    //sent = s.Send(dataSize);
        //    Log.Info("Socket发送数据长度信息:" + dataSize.Length);
        //    while (total < len)
        //    {
        //        sent = s.Send(all, total, dataLeft, SocketFlags.None);
        //        total += sent;
        //        dataLeft -= sent;
        //    }
        //    Log.Info("Socket发送数据长度total:" + total);
        //    return total;
        //}

        #endregion

        //____________________________________________________________________________
        // This method is called by I/O Completed() when an asynchronous send completes.  
        // If all of the data has been sent, then this method calls StartReceive
        //to start another receive op on the socket to read any additional 
        // data sent from the client. If all of the data has NOT been sent, then it 
        //calls StartSend to send more data.        
        //private void ProcessSend(SocketAsyncEventArgs sendEventArgs)
        //{
        //    ProcessSend(sendEventArgs, true);
        //}

        private void ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)sendEventArgs.UserToken;
            Log.Info("ProcessSend.receiveSendToken.TokenId:" + receiveSendToken.userTokenId);
            //if (Program.watchProgramFlow == true)   //for testing
            //{
            //    Program.testWriter.WriteLine("ProcessSend, id " + receiveSendToken.TokenId);
            //}
            //if (Program.watchThreads == true)   //for testing
            //{
            //    DealWithThreadsForTesting("ProcessSend()", receiveSendToken);
            //}

            if (sendEventArgs.SocketError == SocketError.Success)
            {

                receiveSendToken.sendBytesRemainingCount = receiveSendToken.sendBytesRemainingCount - sendEventArgs.BytesTransferred;

                if (receiveSendToken.sendBytesRemainingCount == 0)
                {
                    // If we are within this if-statement, then all the bytes in
                    // the message have been sent. 
                    // 原版本
                    receiveSendToken.ResetSend();
                    receiveSendToken.userTokenId = string.Empty;
                    receiveSendToken.CreateNewDataHolder();
                    sendEventArgs.AcceptSocket = null;
                    poolOfRecEventArgs.Push(sendEventArgs);
                    //if (receiveSendToken.continueReceive)
                    //    StartReceive(sendEventArgs);//修改测试版本，不挂起接收
                    //原版本
                }
                else
                {
                    // If some of the bytes in the message have NOT been sent,
                    // then we will need to post another send operation, after we store
                    // a count of how many bytes that we sent in this send op.                    
                    receiveSendToken.bytesSentAlreadyCount += sendEventArgs.BytesTransferred;
                    // So let's loop back to StartSend().
                    StartSend(sendEventArgs);
                }
            }
            else
            {
                //If we are in this else-statement, there was a socket error.
                //if (Program.watchProgramFlow == true)   //for testing
                //{
                //    Program.testWriter.WriteLine("ProcessSend ERROR, id " + receiveSendToken.TokenId + "\r\n");
                //}

                // We'll just close the socket if there was a
                // socket error when receiving data from the client.
                receiveSendToken.ResetSend();
                CloseClientSocket(sendEventArgs, false);
                Log.Info("处理失败：ProcessSend.receiveSendToken.TokenId:" + receiveSendToken.userTokenId + " " + sendEventArgs.SocketError.ToString());
            }
        }



        //public void SetTokenId(string target, string userTokenId)
        //{
        //    SocketAsyncEventArgs sendEventArgs = this.poolOfRecEventArgs.GetWithToken(target);
        //    DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)sendEventArgs.UserToken;
        //    receiveSendToken.userTokenId = userTokenId;
        //}
        //____________________________________________________________________________
        // Does the normal destroying of sockets after 
        // we finish receiving and sending on a connection.        
        private void CloseClientSocket(SocketAsyncEventArgs e)
        {
            CloseClientSocket(e, false);
        }


        private void CloseClientSocket(SocketAsyncEventArgs e, bool close)
        {
            if (null == e) return;
            var receiveSendToken = (e.UserToken as DataHoldingUserToken);
            receiveSendToken.Reset();
            if (receiveSendToken.theDataHolder.dataMessageReceived != null)
                receiveSendToken.CreateNewDataHolder();
            if (!close)
            {
                return;
            }
            // do a shutdown before you close the socket
            try
            {
                e.AcceptSocket.Shutdown(SocketShutdown.Both);
                e.AcceptSocket.Close();
                if (receiveSendToken.theDataHolder.dataMessageReceived != null)
                {
                    receiveSendToken.CreateNewDataHolder();
                }

                // Put the SocketAsyncEventArg back into the pool,
                // to be used by another client. This 
                this.poolOfRecEventArgs.RemoveToken(receiveSendToken.userTokenId);
                this.poolOfRecEventArgs.Push(e);

                // decrement the counter keeping track of the total number of clients 
                //connected to the server, for testing
                Interlocked.Decrement(ref this.numberOfAcceptedSockets);
            }
            // throws if socket was already closed
            catch (Exception)
            {
            }

            //This method closes the socket and releases all resources, both
            //managed and unmanaged. It publicly calls Dispose. 
            //Make sure the new DataHolder has been created for the next connection.
            //If it has, then dataMessageReceived should be null.


            //if (Program.watchConnectAndDisconnect == true)   //for testing
            //{
            //    Program.testWriter.WriteLine(receiveSendToken.TokenId + " disconnected. " + this.numberOfAcceptedSockets + " client(s) connected.");
            //}

            //Release Semaphore so that its connection counter will be decremented.
            //This must be done AFTER putting the SocketAsyncEventArg back into the pool,
            //or you can run into problems.
            //this.theMaxConnectionsEnforcer.Release();
        }

        //____________________________________________________________________________
        private void HandleBadAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            var acceptOpToken = (acceptEventArgs.UserToken as AcceptOpUserToken);
            Log.Write("Closing socket of accept id " + acceptOpToken.TokenId);
            //This method closes the socket and releases all resources, both
            //managed and unmanaged. It publicly calls Dispose.           
            acceptEventArgs.AcceptSocket.Close();

            //Put the SAEA back in the pool.
            poolOfAcceptEventArgs.Push(acceptEventArgs);
        }

        //____________________________________________________________________________
        public void CleanUpOnExit()
        {
            DisposeAllSaeaObjects();
        }

        //____________________________________________________________________________
        private void DisposeAllSaeaObjects()
        {
            SocketAsyncEventArgs eventArgs;
            while (this.poolOfAcceptEventArgs.Count > 0)
            {
                eventArgs = poolOfAcceptEventArgs.Pop();
                eventArgs.Dispose();
            }
            while (this.poolOfRecEventArgs.Count > 0)
            {
                eventArgs = poolOfRecEventArgs.Pop();
                eventArgs.Dispose();
            }
        }


        //____________________________________________________________________________
        //Display thread info.
        //Note that there is NOT a 1:1 ratio between managed threads 
        //and system (native) threads.
        //
        //Overloaded.
        //Use this one after the DataHoldingUserToken is available.
        //
        private void DealWithThreadsForTesting(string methodName, DataHoldingUserToken receiveSendToken)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" In " + methodName + ", receiveSendToken id " + receiveSendToken.TokenId + ". Thread id " + Thread.CurrentThread.ManagedThreadId + ". Socket handle " + receiveSendToken.socketHandleNumber + ".");
            sb.Append(DealWithNewThreads());

            Program.testWriter.WriteLine(sb.ToString());
        }

        //Use this for testing, when there is NOT a UserToken yet. Use in SocketListener
        //method or Init().
        private void DealWithThreadsForTesting(string methodName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" In " + methodName + ", no usertoken yet. Thread id " + Thread.CurrentThread.ManagedThreadId);
            sb.Append(DealWithNewThreads());
            Program.testWriter.WriteLine(sb.ToString());
        }

        public void AddToken(string key, SocketAsyncEventArgs arg)
        {
            if (key != string.Empty)
            {
                Log.Info("添加登录列表:userTokenId=" + key);
                this.poolOfRecEventArgs.AddWithToken(key, arg);
            }
        }

        public void RemoveToken(string key)
        {
            this.poolOfRecEventArgs.RemoveToken(key);
        }

        //____________________________________________________________________________
        //Display thread info.
        //Overloaded.
        //Use this one in method where AcceptOpUserToken is available.
        //
        private void DealWithThreadsForTesting(string methodName, AcceptOpUserToken theAcceptOpToken)
        {
            StringBuilder sb = new StringBuilder();
            string hString = hString = ". Socket handle " + theAcceptOpToken.socketHandleNumber;
            sb.Append(" In " + methodName + ", acceptToken id " + theAcceptOpToken.TokenId + ". Thread id " + Thread.CurrentThread.ManagedThreadId + hString + ".");
            sb.Append(DealWithNewThreads());
            Program.testWriter.WriteLine(sb.ToString());
        }

        //____________________________________________________________________________
        //Display thread info.
        //called by DealWithThreadsForTesting
        private string DealWithNewThreads()
        {

            //StringBuilder sb = new StringBuilder();
            //bool newThreadChecker = false;
            //lock (this.lockerForThreadHashSet)
            //{
            //    if (managedThreadIds.Add(Thread.CurrentThread.ManagedThreadId) == true)
            //    {
            //        managedThreads.Add(Thread.CurrentThread);
            //        newThreadChecker = true;
            //    }
            //}
            //if (newThreadChecker == true)
            //{

            //    //Display system threads
            //    //Note that there is NOT a 1:1 ratio between managed threads 
            //    //and system (native) threads.
            //    sb.Append("\r\n**** New managed thread.  Threading info:\r\nSystem thread numbers: ");
            //    arrayOfLiveThreadsInThisProcess = theProcess.Threads; //for testing only

            //    foreach (ProcessThread theNativeThread in arrayOfLiveThreadsInThisProcess)
            //    {
            //        sb.Append(theNativeThread.Id.ToString() + ", ");
            //    }
            //    //Display managed threads
            //    //Note that there is NOT a 1:1 ratio between managed threads 
            //    //and system (native) threads.
            //    sb.Append("\r\nManaged threads that have been used: ");               
            //    foreach (Int32 theManagedThreadId in managedThreadIds)
            //    {
            //        sb.Append(theManagedThreadId.ToString() + ", ");                    
            //    }

            //    //Managed threads above were/are being used.
            //    //Managed threads below are still being used now.
            //    sb.Append("\r\nManagedthread.IsAlive true: ");                
            //    foreach (Thread theManagedThread in managedThreads)
            //    {
            //        if (theManagedThread.IsAlive == true)
            //        {
            //            sb.Append(theManagedThread.ManagedThreadId.ToString() + ", ");
            //        }
            //    }                
            //    sb.Append("\r\nEnd thread info.");
            //}
            //return sb.ToString();
            return "";
        }
    }
}
