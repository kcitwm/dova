namespace Dova.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    public class ConsistentHash
    {
        private SortedList<long, string> ketamaNodes = new SortedList<long, string>();
        private int numReps = 200;

        public ConsistentHash(List<string> nodes, int nodeCopies)
        {
            this.ketamaNodes = new SortedList<long, string>();
            this.numReps = nodeCopies;
            int len = (this.numReps / 4);
            foreach (string str in nodes)
            {
                for (int i = 0; i < len; i++)
                {
                    byte[] digest = ConsistentHashAlgorithm.ComputeMD5(str + i);
                    for (int j = 0; j < 4; j++)
                    {
                        long num3 = ConsistentHashAlgorithm.Hash(digest, j);
                        this.ketamaNodes[num3] = str;
                        //this.ketamaNodes.Add(num3,str);
                    }
                }
            }
        }

        public int Count()
        {
            return ketamaNodes.Count;
        }

        public void Add(string node)
        {
            for (int i = 0; i < numReps; i++)
            {
                byte[] digest = ConsistentHashAlgorithm.ComputeMD5(node);
                ketamaNodes.Add(ConsistentHashAlgorithm.Hash(digest, 0) + i, node); 
            }
        }

        public void Remove(string node)
        {
            for (int i = 0; i < numReps; i++)
            {
                byte[] digest = ConsistentHashAlgorithm.ComputeMD5(node);
                ketamaNodes.Remove(ConsistentHashAlgorithm.Hash(digest, 0)+i);
            }
        }
         

        private string GetNodeForKey(long hash)
        {
            long key = hash;
            if (!this.ketamaNodes.ContainsKey(key))
            { 
                bool getted = false;
                foreach (long num2 in this.ketamaNodes.Keys)
                {
                    if (num2 > hash)
                    {
                        key = num2; getted = true; break; 
                    }
                } 
                if (!getted)
                {
                    key = this.ketamaNodes.Keys[0];
                } 
            }
            return this.ketamaNodes[key];
        }

        public string GetPrimary(string k)
        {
            byte[] digest = ConsistentHashAlgorithm.ComputeMD5(k);
            return this.GetNodeForKey(ConsistentHashAlgorithm.Hash(digest, 0));
        }

    }
}

