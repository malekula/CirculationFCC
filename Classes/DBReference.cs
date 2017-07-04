﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Circulation
{
    class DBReference:DB
    {
        public DBReference()
        { }
        internal DataTable GetAllIssuedBook()
        {
            DA.SelectCommand.CommandText = "select 1,C.PLAIN tit,D.PLAIN avt,A.IDREADER,B.FamilyName,B.[Name],B.FatherName," +
                " INV.SORT inv,A.DATE_ISSUE,A.DATE_RETURN," +
                " (case when B.Email is null then 'false' else 'true' end) email, E.PLAIN shifr" +
                " from Reservation_R..ISSUED_FCC A" +
                " left join Readers..Main B on A.IDREADER = B.NumberReader" +
                " left join BJFCC..DATAEXT CC on A.IDMAIN = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXT DD on A.IDMAIN = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXT EE on A.IDDATA = EE.IDDATA and EE.MNFIELD = 899 and EE.MSFIELD = '$j'" +
                " left join BJFCC..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID" +
                " left join BJFCC..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID" +
                " left join BJFCC..DATAEXTPLAIN E on E.IDDATAEXT = EE.ID" +
                " left join BJFCC..DATAEXT INV on A.IDDATA = INV.IDDATA and INV.MNFIELD = 899 and INV.MSFIELD = '$w'" +
                " where A.IDSTATUS = 1 ";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];

        }



        internal object GetAllOverdueBook()
        {
            DA.SelectCommand.CommandText = "select distinct 1,C.PLAIN tit,D.PLAIN avt,A.IDREADER,B.FamilyName,B.[Name],B.FatherName," +
                " INV.SORT inv,A.DATE_ISSUE,A.DATE_RETURN," +
                " (case when (B.Email is null or B.Email = '')  then 'false' else 'true' end) isemail," +
                " case when EM.DATEACTION is null then 'email не отправлялся' else CONVERT (NVARCHAR, EM.DATEACTION, 104) end emailsent, E.PLAIN shifr " +
                " from Reservation_R..ISSUED_FCC A" +
                " left join Readers..Main B on A.IDREADER = B.NumberReader" +
                " left join BJFCC..DATAEXT CC on A.IDMAIN = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXT DD on A.IDMAIN = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXT EE on A.IDDATA = EE.IDDATA and EE.MNFIELD = 899 and EE.MSFIELD = '$j'" +
                " left join BJFCC..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID" +
                " left join BJFCC..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID" +
                " left join BJFCC..DATAEXTPLAIN E on E.IDDATAEXT = EE.ID" +
                " left join Reservation_R..ISSUED_FCC_ACTIONS EM on EM.IDISSUED_FCC = A.IDREADER and EM.IDACTION = 4" + // 4 - это ACTIONTYPE = сотрудник отослал емаил. IDISSUED_FCC - номер читателя
                            " and EM.ID = (select max(z.ID) from Reservation_R..ISSUED_FCC_ACTIONS z where z.IDISSUED_FCC = A.IDREADER and z.IDACTION = 4)" +
                " left join BJFCC..DATAEXT INV on A.IDDATA = INV.IDDATA and INV.MNFIELD = 899 and INV.MSFIELD = '$w'" +
                " where A.IDSTATUS = 1 and A.DATE_RETURN < getdate()";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];
        }

        internal object GetReaderHistory(ReaderVO reader)
        {
            DA.SelectCommand.CommandText = "select 1 ID,C.PLAIN tit,D.PLAIN avt," +
                " INV.SORT inv,A.DATE_ISSUE,ret.DATEACTION DATE_RETURN" +
                " from Reservation_R..ISSUED_FCC A" +
                " left join Readers..Main B on A.IDREADER = B.NumberReader" +
                " left join BJFCC..DATAEXT CC on A.IDMAIN = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXT DD on A.IDMAIN = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID" +
                " left join BJFCC..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID" +
                " left join BJFCC..DATAEXT INV on A.IDDATA = INV.IDDATA and INV.MNFIELD = 899 and INV.MSFIELD = '$w'" +
                " left join Reservation_R..ISSUED_FCC_ACTIONS ret on ret.IDISSUED_FCC = A.ID and ret.IDACTION = 2 " +
                " where A.IDSTATUS = 2 and A.IDREADER = "+reader.ID+"order by DATE_ISSUE desc";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];
        }

        internal object GetAllBooks()
        {
            DA.SelectCommand.CommandText = "select 1 ID,C.PLAIN tit,D.PLAIN avt," +
                " INV.SORT inv" +
                " from BJFCC..MAIN A" +
                " left join BJFCC..DATAEXT CC on A.ID = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXT DD on A.ID = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID" +
                " left join BJFCC..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID" +
                " left join BJFCC..DATAEXT INV on A.ID = INV.IDMAIN and INV.MNFIELD = 899 and INV.MSFIELD = '$w'" +
                " left join BJFCC..DATAEXT klass on INV.IDDATA = klass.IDDATA and klass.MNFIELD = 921 and klass.MSFIELD = '$c' " +
                " where INV.SORT is not null and klass.SORT='Disponible'";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];
        }

        internal object GetBookNegotiability()
        {
            DA.SelectCommand.CommandText = "with F1 as  "+
                                           " ( "+
                                           " select B.IDDATA,COUNT(B.IDDATA) cnt " +
                                           " from Reservation_R..ISSUED_FCC_ACTIONS A "+
                                           " left join Reservation_R..ISSUED_FCC B on B.ID = A.IDISSUED_FCC "+
                                           " where A.IDACTION = 2 "+
                                           " group by B.IDDATA " +
                                           " ) "+
                                           " select distinct 1 ID,C.PLAIN tit,D.PLAIN avt, "+
                                           " INV.SORT inv,A.cnt "+
                                           "  from F1 A "+
                                           " left join BJFCC..DATAEXT idm on A.IDDATA = idm.IDDATA " +
                                           " left join BJFCC..DATAEXT CC on idm.IDMAIN = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a' " +
                                           "  left join BJFCC..DATAEXT DD on idm.IDMAIN = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a' " +
                                           " left join BJFCC..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID "+
                                           "  left join BJFCC..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID " +
                                           "  left join BJFCC..DATAEXT INV on A.IDDATA = INV.IDDATA and INV.MNFIELD = 899 and INV.MSFIELD = '$w'"+
                                           " order by cnt desc";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];
        }

        internal object GetBooksWithRemovedResponsibility()
        {
            DA.SelectCommand.CommandText = "select 1,C.PLAIN tit,D.PLAIN avt,A.IDREADER,B.FamilyName,B.[Name],B.FatherName," +
                " INV.SORT inv,A.DATE_ISSUE,AA.DATEACTION " +
                
                " from Reservation_R..ISSUED_FCC A" +
                " left join Reservation_R..ISSUED_FCC_ACTIONS AA on A.ID = AA.IDISSUED_FCC " +
                " left join Readers..Main B on A.IDREADER = B.NumberReader" +
                " left join BJFCC..DATAEXT CC on A.IDMAIN = CC.IDMAIN and CC.MNFIELD = 200 and CC.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXT DD on A.IDMAIN = DD.IDMAIN and DD.MNFIELD = 700 and DD.MSFIELD = '$a'" +
                " left join BJFCC..DATAEXTPLAIN C on C.IDDATAEXT = CC.ID" +
                " left join BJFCC..DATAEXTPLAIN D on D.IDDATAEXT = DD.ID" +
                " left join BJFCC..DATAEXT INV on A.IDDATA = INV.IDDATA and INV.MNFIELD = 899 and INV.MSFIELD = '$w'" +
                " where AA.IDACTION = 5";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];

        }

        internal object GetViolators()
        {
            DA.SelectCommand.CommandText = "select distinct 1,A.IDREADER,B.FamilyName,B.[Name],B.FatherName," +
                " (case when (B.Email is null or B.Email = '') then 'false' else 'true' end) isemail," +
                " case when EM.DATEACTION is null then 'email не отправлялся' else CONVERT (NVARCHAR, EM.DATEACTION, 104) end emailsent " +
                " from Reservation_R..ISSUED_FCC A" +
                " left join Readers..Main B on A.IDREADER = B.NumberReader" +
                " left join Reservation_R..ISSUED_FCC_ACTIONS EM on EM.IDISSUED_FCC = A.IDREADER and EM.IDACTION = 4" + // 4 - это ACTIONTYPE = сотрудник отослал емаил
                " where A.IDSTATUS = 1 and A.DATE_RETURN < getdate()";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];
        }
    }
}