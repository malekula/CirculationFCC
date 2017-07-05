﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Circulation
{
    public enum Bases { BJFCC = 1, BJVVV };

    public class BookVO
    {
        public string BAR;
        public string INV;
        public int IDDATA;
        public int IDMAIN;
        public string TITLE;
        public string AUTHOR;
        public int IDISSUED;
        public Bases FUND;
        public BookVO() { }
        public BookVO(int IDMAIN, Bases fund)
        {
            DBBook dbb = new DBBook();

            this.BookRecord = dbb.GetBookByIDMAIN(IDMAIN, fund);
            this.IDMAIN = BookRecord[0].IDMAIN;
        }

        public BookVO(string BAR)
        {
            DBBook dbb = new DBBook();
            this.BookRecord = dbb.GetBookByBAR(BAR);
            if (BookRecord[0].Fund == Bases.BJFCC)
                this.FUND = Bases.BJFCC;
            else
                this.FUND = Bases.BJVVV;
            this.BAR = BAR;
            this.IDMAIN = BookRecord[0].IDMAIN;
            IEnumerable<BJRecord> iddata = from BJRecord x in BookRecord
                                              where x.SORT == this.BAR && x.MNFIELD == 899 && x.MSFIELD == "$w"
                                              select x;
            this.IDDATA = iddata.ToList()[0].IDDATA;
            var title = from BJRecord x in BookRecord
                        where x.MNFIELD == 200 && x.MSFIELD == "$a"
                        select x;
            this.TITLE = title.ToList()[0].PLAIN;
            if (title.Count() == 0)
            {
                this.TITLE = "";
            }
            else
            {
                this.TITLE = title.ToList()[0].PLAIN;
            }

            var author = from BJRecord x in BookRecord
                         where x.MNFIELD == 700 && x.MSFIELD == "$a"
                         select x;
            if (author.Count() == 0)
            {
                this.AUTHOR = "";
            }
            else
            {
                this.AUTHOR = author.ToList()[0].PLAIN;
            }
            this.IDISSUED = dbb.GetIDISSUED(this.IDDATA, this.FUND);

            
        }

        public List<BJRecord> BookRecord;



        internal bool IsIssued()
        {
            DBBook dbb = new DBBook();
            IEnumerable<BJRecord> iddata = from BJRecord x in BookRecord
                         where x.SORT == this.BAR && x.MNFIELD == 899 && x.MSFIELD == "$w"
                         select x;

            return dbb.IsIssued(iddata.ToList()[0].IDDATA, this.FUND);
        }
    }

}
