using Dova;
using Dova.Data;
using Dova.Interfaces;
using Dova.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.Text; 

namespace Dova.Sequences
{
    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class Sequence : MarshalByRefObject, ISequence
    {
        static string connString = "";
        static string providerName = "";
        static string getSeqSp = "GetSequenceSP";
        static string getSeqTimedSp = "GetSequenceTimedSP";

        static Sequence()
        {
            ConnectionStringSettings css = Config.GetConnection(Config.SequenceConnectionName);
            if (null == css)
                css = Config.GetConnection(Config.DefaultConnectionName);
            connString = css.ConnectionString;
            providerName = css.ProviderName;
        }

        public long  Get(string seqName  )
        {
            int total = 1;
            DataHelper hp = new DataHelper(connString, providerName);
            DbParameter[] pars = new DbParameter[4];
            DbParameter startPar, endPar;
            pars[0] = DataHelper.CreateParameter(providerName, "SeqName", seqName);
            pars[1] = DataHelper.CreateParameter(providerName, "SeqNum", total);
            startPar = DataHelper.CreateParameter(providerName, "SeqStart", DbType.Int64, ParameterDirection.Output);
            endPar = DataHelper.CreateParameter(providerName, "SeqEnd", DbType.Int64, ParameterDirection.Output);
            pars[2] = startPar;
            pars[3] = endPar;
            hp.ExecuteNonQuery(getSeqSp, System.Data.CommandType.StoredProcedure, pars);
            long start = (long)startPar.Value;
            long end = (long)endPar.Value;
            //Log.Write(LogAction.Write, seqName + ":" + start + " " + end);
            if (end - start == total - 1)
                return  end;
            string msg = "多取" + seqName + "返回异常,返回数量和指定数量不合:total=" + total + " start=" + start + " end=" + end;
            Log.Error(msg);
            throw new DataException(msg);
        }


        public long[] Get(string seqName, int total, out DateTime getTime)
        {
            DataHelper hp = new DataHelper(connString, providerName);
            DbParameter[] pars = new DbParameter[5];
            DbParameter startPar, endPar, timePar;
            pars[0] = DataHelper.CreateParameter(providerName, "SeqName", seqName);
            pars[1] = DataHelper.CreateParameter(providerName, "SeqNum", total);
            startPar = DataHelper.CreateParameter(providerName, "SeqStart", DbType.Int64, ParameterDirection.Output);
            endPar = DataHelper.CreateParameter(providerName, "SeqEnd", DbType.Int64, ParameterDirection.Output);
            timePar = DataHelper.CreateParameter(providerName, "GetTime", DbType.DateTime, ParameterDirection.Output);
            pars[2] = startPar;
            pars[3] = endPar;
            pars[4] = timePar;
            hp.ExecuteNonQuery(getSeqTimedSp, System.Data.CommandType.StoredProcedure, pars);
            long start = (long)startPar.Value;
            long end = (long)endPar.Value;
            getTime = (DateTime)timePar.Value;
            //Log.Write("Sequence", seqName + ":" + start + " " + end);
            if (end - start == total - 1)
                return new long[2] { start, end };
            string msg = "多取" + seqName + "返回异常,返回数量和指定数量不合:total=" + total + " start=" + start + " end=" + end;
            Log.Error(msg);
            throw new DataException(msg);
        }

    }
}
