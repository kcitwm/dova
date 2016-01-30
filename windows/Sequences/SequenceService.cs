using System;
using System.Data;
using System.Data.Common;

using Dova.Data;
using Dova.Utility;
using Dova.Interfaces;
using Dova.Infrastructure;
using System.ServiceModel;
using Dova.Services;

namespace Dova.Sequences
{
    public class SequenceService
    {
        private static ServiceFactory<ISequence> factory;
        static ISequence iseq; 
        static SequenceService()
        {
            if (Config.SequenceServiceType == 1)
            {
                iseq = new Sequence() as ISequence;
                return ;
            }
            factory = new ServiceFactory<ISequence>(ServiceConfig.GetConfigs(Config.SequenceServiceName));
            iseq = factory.Instance();
            return;
        }

        public static long[] Get(string seqName, int total)
        {
            return iseq.Get(seqName, total);
        }

        public static long[] Get(string seqName, int total, out DateTime getTime)
        {
            return iseq.Get(seqName, total, out getTime);
        }

    }
}
