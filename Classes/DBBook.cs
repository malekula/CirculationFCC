using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Circulation
{
    class DBBook : DB
    {
        //BJFCCRecord rec;// = new BJFCCRecord();
        public DBBook()
        {
            
        }
        public List<BJFCCRecord> GetBookByIDMAIN(int IDMAIN)
        {
            DA.SelectCommand.CommandText = "select A.*,B.PLAIN from BJFCC..DATAEXT A "+
                                           " left join BJFCC..DATAEXTPLAIN B on A.ID = B.IDDATAEXT where A.IDMAIN = "+IDMAIN;
            DS = new DataSet();
            DA.Fill(DS, "t");
            List<BJFCCRecord> Book = new List<BJFCCRecord>();
            BJFCCRecord rec;
            foreach (DataRow r in DS.Tables["t"].Rows)
            {
                rec = new BJFCCRecord();
                rec.ID = (int)r["ID"];
                rec.IDDATA = (int)r["IDDATA"];
                rec.IDINLIST = (int)r["IDINLIST"];
                rec.IDMAIN = IDMAIN;
                rec.MNFIELD = (int)r["MNFIELD"];
                rec.MSFIELD = r["MSFIELD"].ToString();
                rec.PLAIN = r["PLAIN"].ToString();
                rec.SORT = r["SORT"].ToString();
                Book.Add(rec);
            }
            return Book;
        }

        public List<BJFCCRecord> GetBookByBAR(string BAR)
        {
            DA.SelectCommand.CommandText = "select A.*,B.PLAIN from BJFCC..DATAEXT A " +
                                           " left join BJFCC..DATAEXTPLAIN B on A.ID = B.IDDATAEXT " +
                                           " where A.IDMAIN = (select top 1 IDMAIN from BJFCC..DATAEXT where MNFIELD = 899 and MSFIELD = '$w' and SORT = '" + BAR + "')";
            DS = new DataSet();
            DA.Fill(DS, "t");
            List<BJFCCRecord> Book = new List<BJFCCRecord>();
            BJFCCRecord rec;
            foreach (DataRow r in DS.Tables["t"].Rows)
            {
                rec = new BJFCCRecord();
                rec.ID = (int)r["ID"];
                rec.IDDATA = (int)r["IDDATA"];
                rec.IDINLIST = (int)r["IDINLIST"];
                rec.IDMAIN = (int)r["IDMAIN"]; 
                rec.MNFIELD = (int)r["MNFIELD"];
                rec.MSFIELD = r["MSFIELD"].ToString();
                rec.PLAIN = r["PLAIN"].ToString();
                rec.SORT = r["SORT"].ToString();
                Book.Add(rec);
            }
            return Book;
        }
        public bool Exists(string BAR)
        {
            DA.SelectCommand.CommandText = "select top 1 IDMAIN from BJFCC..DATAEXT where MNFIELD = 899 and MSFIELD = '$w' and SORT = '" + BAR + "'";
            DS = new DataSet();
            int i = DA.Fill(DS, "t");
            if (i > 0) return true; else return false;

        }

        internal bool IsIssued(int IDDATA)
        {
            DA.SelectCommand.CommandText = "select IDMAIN from Reservation_R..ISSUED_FCC where IDDATA = "+IDDATA+" and IDSTATUS = 1";
            DS = new DataSet();
            int i = DA.Fill(DS, "t");
            if (i > 0) return true; else return false;
        }

        internal int GetIDISSUED(int IDDATA)
        {
            DA.SelectCommand.CommandText = "select ID from Reservation_R..ISSUED_FCC where IDDATA = " + IDDATA + " and IDSTATUS = 1";
            DS = new DataSet();
            int i = DA.Fill(DS, "t");
            if (i > 0) return (int)DS.Tables["t"].Rows[0]["ID"]; else return 0;
        }
    }
}
