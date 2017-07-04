﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace Circulation
{
    class DBReader : DB
    {
        public DBReader() { }

        public DataRow GetReaderByID(int ID)
        {
            DA.SelectCommand.CommandText = "select A.*,B.Photo fotka from Readers..Main A left join Readers..Photo B on A.NumberReader = B.IDReader where NumberReader = " + ID;
            DS = new DataSet();
            int i = DA.Fill(DS, "reader");
            if (i == 0) return null;
            return DS.Tables["reader"].Rows[0];
        }
        public bool Exists(int ID)
        {
            DA.SelectCommand.CommandText = "select 1 from Readers..Main where NumberReader = " + ID;
            DS = new DataSet();
            if (DA.Fill(DS, "reader") > 0) return true; else return false;
        }

        internal DataRow GetReaderByBAR(string BAR)
        {
            DA.SelectCommand.CommandText = "select top 1 A.*,B.Photo fotka from Readers..Main A left join Readers..Photo B on A.NumberReader = B.IDReader where BarCode = '" + BAR.Substring(1) + "'";
            DS = new DataSet();
            int i;
            try
            {
                i = DA.Fill(DS, "t");
            }
            catch
            {
                i = 0;
            }
            if (i > 0) return DS.Tables["t"].Rows[0];

            DA.SelectCommand.CommandText = "select top 1 A.*,B.Photo fotka from Readers..Main A left join Readers..Photo B on A.NumberReader = B.IDReader where NumberSC = '" + BAR.Substring(0, BAR.IndexOf(" ")) +
                                                                                       "' and SerialSC = '" + BAR.Substring(BAR.IndexOf(" ") + 1) + "'";
            DS = new DataSet();
            i = DA.Fill(DS, "t");
            if (i > 0) return DS.Tables["t"].Rows[0];
            return null;
        }

        internal bool IsAlreadyIssuedMoreThanFourBooks(ReaderVO r)
        {
            DA.SelectCommand.CommandText = "select * from Reservation_R..ISSUED_FCC where IDREADER = " + r.ID + " and IDSTATUS = 1";
            DS = new DataSet();
            int i = DA.Fill(DS, "t");
            if (i >= 4) return true; else return false;
        }

        internal DataTable GetFormular(int ID)
        {
            DA.SelectCommand.CommandText = "select ROW_NUMBER() over(order by A.DATE_ISSUE) num, " +
                                           " bar.SORT bar, " +
                                           " avtp.PLAIN avt, " +
                                           " titp.PLAIN tit, " +
                                           " cast(cast(A.DATE_ISSUE as varchar(11)) as datetime) iss, " +
                                           " cast(cast(A.DATE_RETURN as varchar(11)) as datetime) ret , A.ID idiss, A.IDREADER idr,E.PLAIN shifr " +
                                           " ,Reservation_R.dbo.GetProlongedTimes(A.ID, 'BJFCC') prolonged" +
                                           "  from Reservation_R..ISSUED_FCC A " +
                                           " left join BJFCC..DATAEXT tit on A.IDMAIN = tit.IDMAIN and tit.MNFIELD = 200 and tit.MSFIELD = '$a' " +
                                           " left join BJFCC..DATAEXTPLAIN titp on tit.ID = titp.IDDATAEXT " +
                                           " left join BJFCC..DATAEXT avt on A.IDMAIN = avt.IDMAIN and avt.MNFIELD = 700 and avt.MSFIELD = '$a' " +
                                           " left join BJFCC..DATAEXTPLAIN avtp on avt.ID = avtp.IDDATAEXT " +
                                           " left join BJFCC..DATAEXT bar on A.IDDATA = bar.IDDATA and bar.MNFIELD = 899 and bar.MSFIELD = '$w' " +
                                           " left join BJFCC..DATAEXT EE on A.IDDATA = EE.IDDATA and EE.MNFIELD = 899 and EE.MSFIELD = '$j'" +
                                           " left join BJFCC..DATAEXTPLAIN E on E.IDDATAEXT = EE.ID" +

                                           " where A.IDREADER = " + ID + " and A.IDSTATUS = 1";
            ;
            DS = new DataSet();
            int i = DA.Fill(DS, "formular");
            return DS.Tables["formular"];
        }

        internal void ProlongByIDISS(int IDISS, int days, int IDEMP)
        {
            DA.UpdateCommand.CommandText = "update Reservation_R..ISSUED_FCC set DATE_RETURN = dateadd(day," + days + ",DATE_RETURN) where ID = " + IDISS;
            DA.UpdateCommand.Connection.Open();
            DA.UpdateCommand.ExecuteNonQuery();
            DA.UpdateCommand.Connection.Close();

            DA.InsertCommand.Parameters.Clear();
            DA.InsertCommand.Parameters.Add("IDACTION", SqlDbType.Int);
            DA.InsertCommand.Parameters.Add("IDUSER", SqlDbType.Int);
            DA.InsertCommand.Parameters.Add("IDISSUED_FCC", SqlDbType.Int);
            DA.InsertCommand.Parameters.Add("DATEACTION", SqlDbType.DateTime);

            DA.InsertCommand.Parameters["IDACTION"].Value = 3;
            DA.InsertCommand.Parameters["IDUSER"].Value = IDEMP;
            DA.InsertCommand.Parameters["IDISSUED_FCC"].Value = IDISS;
            DA.InsertCommand.Parameters["DATEACTION"].Value = DateTime.Now;

            DA.InsertCommand.CommandText = "insert into Reservation_R..ISSUED_FCC_ACTIONS (IDACTION,IDEMP,IDISSUED_FCC,DATEACTION) values " +
                                            "(@IDACTION,@IDUSER,@IDISSUED_FCC,@DATEACTION)";
            DA.InsertCommand.Connection.Open();
            DA.InsertCommand.ExecuteNonQuery();
            DA.InsertCommand.Connection.Close();


        }

        internal DataTable GetReaderByFamily(string p)
        {
            DA.SelectCommand.CommandText = "select NumberReader, FamilyName, [Name], FatherName,DateBirth, RegistrationCity,RegistrationStreet, " +
                                           " ISNULL(Email,'') from Readers..Main where lower(FamilyName) like lower('" + p + "')+'%'";
            DS = new DataSet();
            DA.Fill(DS, "t");
            return DS.Tables["t"];
        }

        internal void AddPhoto(byte[] fotka)
        {
            DA.UpdateCommand.CommandText = "insert into Readers..Photo (IDReader,Photo) values (189245,@fotka)";
            DA.UpdateCommand.Parameters.AddWithValue("fotka", fotka);
            DA.UpdateCommand.Connection.Open();
            DA.UpdateCommand.ExecuteNonQuery();
            DA.UpdateCommand.Connection.Close();
        }
        internal string GetEmail(ReaderVO reader)
        {
            DA.SelectCommand.CommandText = "select Email from Readers..Main where NumberReader = " + reader.ID;
            DataSet D = new DataSet();
            int i = DA.Fill(D);
            if (i == 0) return "";
            if (this.IsValidEmail(D.Tables[0].Rows[0][0].ToString()))
            {
                return D.Tables[0].Rows[0][0].ToString();
            }
            else
            {
                return "";
            }

        }
        public bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

       

       
        internal string GetLastDateEmail(ReaderVO reader)
        {
            DA.SelectCommand.CommandText = "select top 1 DATEACTION from Reservation_R..ISSUED_FCC_ACTIONS where IDACTION = 4 and IDISSUED_FCC = " + reader.ID+" order by DATEACTION desc";//когда актион равен 4 - это значит емаил отослали. только в этом случае 
                                                                                                                                                        //в поле IDISSUED_FCC хранится номер читателя а не номер выдачи
                                                                                                                                                         //это пипец конечно ну а чё?
            DataSet D = new DataSet();
            int i = DA.Fill(D,"t");
            if (i == 0) return "не отправлялось";
            return Convert.ToDateTime(D.Tables["t"].Rows[0][0]).ToString("dd.MM.yyyy HH:mm");
        }

        internal string GetRealIDByGuestBar(string bar)
        {

            DA.SelectCommand.CommandText = " SELECT top 1 A.IDReaderInput id FROM Readers..Input A " +
                                 " where A.BarCodeInput = '" + bar + "'" +
                                 " order by A.DateInInput desc";
            DataSet DS = new DataSet();
            int tt = DA.Fill(DS, "tt");
            if (tt == 0) return "-1";
            return (DS.Tables["tt"].Rows[0]["id"].ToString() == "0") ? "-1" : DS.Tables["tt"].Rows[0]["id"].ToString();
        }

        internal bool IsAlreadyMarked(string bar)
        {
            DA.SelectCommand.CommandText = "SELECT * FROM " + DB.BASENAME + "..ATTENDANCE_FCC " +
                                                   " where Cast(Cast(DATEATT As VarChar(11)) As DateTime) = cast(cast(getdate() as varchar(11)) as datetime) " +
                                                   " and BAR = '" + bar + "'";
            DataSet DS = new DataSet();
            int t = DA.Fill(DS, "t");
            return (t > 0) ? true : false;
        }

        internal string GetComment(int IDReader)
        {
            DA.SelectCommand.CommandText = "SELECT Comment FROM " + DB.BASENAME + "..Comments_FCC " +
                                                   " where IDReader = " + IDReader;
            DataSet DS = new DataSet();
            int t = DA.Fill(DS, "t");
            if (t == 0) return "";
            return DS.Tables["t"].Rows[0][0].ToString();
        }

        internal void ChangeComment(int IDReader, string comment)
        {
            DA.SelectCommand.CommandText = "SELECT ID, Comment FROM " + DB.BASENAME + "..Comments_FCC " +
                                                   " where IDReader = " + IDReader;
            DataSet DS = new DataSet();
            int t = DA.Fill(DS, "t");
            if (t == 0)
            {
                DA.InsertCommand.CommandText = "insert into " + DB.BASENAME + "..Comments_FCC (IDReader, Comment) values (@IDReader, @Comment)";
                DA.InsertCommand.Parameters.Clear();
                DA.InsertCommand.Parameters.AddWithValue("IDReader", IDReader);
                DA.InsertCommand.Parameters.AddWithValue("Comment", comment);
                DA.InsertCommand.Connection.Open();
                DA.InsertCommand.ExecuteNonQuery();
                DA.InsertCommand.Connection.Close();
            }
            else
            {
                DA.UpdateCommand.Parameters.Clear();
                DA.UpdateCommand.Parameters.AddWithValue("IDReader", IDReader);
                DA.UpdateCommand.Parameters.AddWithValue("Comment", comment);
                DA.UpdateCommand.CommandText = " update " + DB.BASENAME + "..Comments_FCC " +
                                               " set Comment = @Comment " +
                                               " where IDReader = @IDReader";
                DA.UpdateCommand.Connection.Open();
                DA.UpdateCommand.ExecuteNonQuery();
                DA.UpdateCommand.Connection.Close();
            }
        }
    }
}