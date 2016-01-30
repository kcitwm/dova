using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace PersistentSocketAsyncServer
{
    public sealed class SocketAsyncEventArgsPool
    {
        //just for assigning an ID so we can watch our objects while testing.
        private Int32 nextTokenId = 0;

        // Pool of reusable SocketAsyncEventArgs objects.        
        //Stack<SocketAsyncEventArgs> pool; 
        Dictionary<string, SocketAsyncEventArgs> poolWithToken; 
        Stack<SocketAsyncEventArgs> pool;
        Stack<SocketAsyncEventArgs> poolSend;
        // initializes the object pool to the specified size.
        // "capacity" = Maximum number of SocketAsyncEventArgs objects
        public SocketAsyncEventArgsPool(int capacity)
        {
             
            pool = new Stack<SocketAsyncEventArgs>(capacity);
            poolSend = new Stack<SocketAsyncEventArgs>(capacity);
            this.poolWithToken = new Dictionary<string, SocketAsyncEventArgs>(capacity); 
        }

        // The number of SocketAsyncEventArgs instances in the pool.         
        public Int32 Count
        {
            get { return this.pool.Count; }
        }

        public Int32 AssignTokenId()
        {
            Int32 tokenId = Interlocked.Increment(ref nextTokenId);
            return tokenId;
        }

        // Removes a SocketAsyncEventArgs instance from the pool.
        // returns SocketAsyncEventArgs removed from the pool.
        public SocketAsyncEventArgs Pop()
        {
            lock (pool)
            {
                return this.pool.Pop();
            }

        }

        // Add a SocketAsyncEventArg instance to the pool. 
        // "item" = SocketAsyncEventArgs instance to add to the pool.
        public void Push(SocketAsyncEventArgs item)
        {
            lock (this.pool)
            {
                this.pool.Push(item);
            }
        }
          

        public void AddWithToken(string key, SocketAsyncEventArgs item)
        {
            lock (this.poolWithToken)
            {
                poolWithToken[key] = item;
                //if (!poolWithToken.ContainsKey(key))
                //    this.poolWithToken.Add(key, item);
            }
        }



        public void RemoveToken(string key)
        {
            lock (this.poolWithToken)
            {
                this.poolWithToken.Remove(key);
            }
        }

        public SocketAsyncEventArgs GetWithToken(string key)
        {
            if ( this.poolWithToken.ContainsKey(key))
                return this.poolWithToken[key];
            return null;
        }


       


    }
}
