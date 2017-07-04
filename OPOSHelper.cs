using System;
namespace Test1
{
    #region class ResultCodeH
    //CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC
    public class ResultCodeH
    {
        #region ���������
        /////////////////////////////////////////////////////////////////////
        // OPOS "ResultCode" Property Constants
        /////////////////////////////////////////////////////////////////////
        public const int OPOS_SUCCESS = 0;
        public const int OPOS_E_CLOSED = 101;
        public const int OPOS_E_CLAIMED = 102;
        public const int OPOS_E_NOTCLAIMED = 103;
        public const int OPOS_E_NOSERVICE = 104;
        public const int OPOS_E_DISABLED = 105;
        public const int OPOS_E_ILLEGAL = 106;
        public const int OPOS_E_NOHARDWARE = 107;
        public const int OPOS_E_OFFLINE = 108;
        public const int OPOS_E_NOEXIST = 109;
        public const int OPOS_E_EXISTS = 110;
        public const int OPOS_E_FAILURE = 111;
        public const int OPOS_E_TIMEOUT = 112;
        public const int OPOS_E_BUSY = 113;
        public const int OPOS_E_EXTENDED = 114;
        public const int OPOS_OR_ALREADYOPEN = 301;
        public const int OPOS_OR_REGBADNAME = 302;
        public const int OPOS_OR_REGPROGID = 303;
        public const int OPOS_OR_CREATE = 304;
        public const int OPOS_OR_BADIF = 305;
        public const int OPOS_OR_FAILEDOPEN = 306;
        public const int OPOS_OR_BADVERSION = 307;
        public const int OPOS_ORS_NOPORT = 401;
        public const int OPOS_ORS_NOTSUPPORTED = 402;
        public const int OPOS_ORS_CONFIG = 403;
        #endregion
        #region Message
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string Message(int _ResultCode)
        {
            switch (_ResultCode)
            {
                case OPOS_SUCCESS:
                    return "OPOS - �������";
                case OPOS_E_CLOSED:
                    return "OPOS - ������ ������";
                case OPOS_E_CLAIMED:
                    return "OPOS - ������ ���-�� ��������";
                case OPOS_E_NOTCLAIMED:
                    return "OPOS - ������ �� ��������";
                case OPOS_E_NOSERVICE:
                    return "OPOS - ������ �� ��������������";
                case OPOS_E_DISABLED:
                    return "OPOS - ������ ��������";
                case OPOS_E_ILLEGAL:
                    return "OPOS - �����������";
                case OPOS_E_NOHARDWARE:
                    return "OPOS - ������������ �����������";
                case OPOS_E_OFFLINE:
                    return "OPOS - ������� ���������";
                case OPOS_E_NOEXIST:
                    return "OPOS - ������ �� ����������";
                case OPOS_E_EXISTS:
                    return "OPOS - ������ ��� ����������";
                case OPOS_E_FAILURE:
                    return "OPOS - ����� ����";
                case OPOS_E_TIMEOUT:
                    return "OPOS - ����� �������� �������";
                case OPOS_E_BUSY:
                    return "OPOS - ���������� ������";
                case OPOS_E_EXTENDED:
                    return "OPOS - ����������� ������";
                case OPOS_OR_ALREADYOPEN:
                    return "OPOS - ������ ��� ������";
                case OPOS_OR_REGBADNAME:
                    return "OPOS - � ������� ��� ���������� � ����� ������";
                case OPOS_OR_REGPROGID:
                    return "OPOS - ������ ������������ ���������� � �������";
                case OPOS_OR_CREATE:
                    return "OPOS - ���������� ������� ��������� ������";
                case OPOS_OR_BADIF:
                    return "OPOS - ��������� ������ �� ������������ ��� ����������� ������ ��� ��������";
                case OPOS_OR_FAILEDOPEN:
                    return "OPOS - ����������� ������ ��������";
                case OPOS_OR_BADVERSION:
                    return "OPOS - �������� ������ ���������� �������";
                case OPOS_ORS_NOPORT:
                    return "OPOS - ������ ������� � ����� �/�";
                case OPOS_ORS_NOTSUPPORTED:
                    return "OPOS - ��������� ������ �� ������������ ��������� ����������";
                case OPOS_ORS_CONFIG:
                    return "OPOS - ������ ������ ������������ �� �������";
                default:
                    return "OPOS - ����������� ��� ��������";
            }
        }
        #endregion
        #region Check
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static void Check(int _ResultCode)
        {
            if (_ResultCode != OPOS_SUCCESS)
                throw new Exception(ResultCodeH.Message(_ResultCode));
        }
        #endregion
    }
    #endregion

    #region class StatusUpdateH
    //CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC
    public class StatusUpdateH
    {
        public const int OPOS_SUE_POWER_ONLINE = 2001; // (added in 1.3)
        public const int OPOS_SUE_POWER_OFF = 2002; // (added in 1.3)
        public const int OPOS_SUE_POWER_OFFLINE = 2003; // (added in 1.3)
        public const int OPOS_SUE_POWER_OFF_OFFLINE = 2004; // (added in 1.3)
        public const int OPOS_SUE_UF_PROGRESS = 2100; // (added in 1.9)
        public const int OPOS_SUE_UF_COMPLETE = 2200; // (added in 1.9)
        public const int OPOS_SUE_UF_COMPLETE_DEV_NOT_RESTORED = 2205; // (added in 1.9)
        public const int OPOS_SUE_UF_FAILED_DEV_OK = 2201; // (added in 1.9)
        public const int OPOS_SUE_UF_FAILED_DEV_UNRECOVERABLE = 2202; // (added in 1.9)
        public const int OPOS_SUE_UF_FAILED_DEV_NEEDS_FIRMWARE = 2203; // (added in 1.9)
        public const int OPOS_SUE_UF_FAILED_DEV_UNKNOWN = 2204; // (added in 1.9)
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string Message(int _StatusUpdateCode)
        {
            switch (_StatusUpdateCode)
            {
                case OPOS_SUE_POWER_ONLINE:
                    return "������� ��������";
                case OPOS_SUE_POWER_OFF:
                    return "������� ���������";
                case OPOS_SUE_POWER_OFFLINE:
                    return "���������� � ������ ����������������";
                case OPOS_SUE_POWER_OFF_OFFLINE:
                    return "���������� ��������� ��� � ������ ����������������";
                case OPOS_SUE_UF_PROGRESS:
                    return "Update firmware progress";
                case OPOS_SUE_UF_COMPLETE:
                    return "Update firmware complete";
                case OPOS_SUE_UF_COMPLETE_DEV_NOT_RESTORED:
                    return "Update firmware complete, device not restored";
                case OPOS_SUE_UF_FAILED_DEV_OK:
                    return "Update firmware failed, device OK";
                case OPOS_SUE_UF_FAILED_DEV_UNRECOVERABLE:
                    return "Update firmware failed, device unrecoverable";
                case OPOS_SUE_UF_FAILED_DEV_NEEDS_FIRMWARE:
                    return "Update firmware failed, device needs firmware";
                case OPOS_SUE_UF_FAILED_DEV_UNKNOWN:
                    return "Update firmware failed, device status unknown";
                default:
                    return "����������� ������ ���";
            }
        }
    }
    #endregion
    #region class PowerStateH
    //CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC
    public class PowerStateH
    {
        public const int OPOS_PS_UNKNOWN = 2000;
        public const int OPOS_PS_ONLINE = 2001;
        public const int OPOS_PS_OFF = 2002;
        public const int OPOS_PS_OFFLINE = 2003;
        public const int OPOS_PS_OFF_OFFLINE = 2004;
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public static string Message(int _PowerStateCode)
        {
            switch (_PowerStateCode)
            {
                case OPOS_PS_UNKNOWN:
                    return "����������";
                case OPOS_PS_ONLINE:
                    return "������� ��������";
                case OPOS_PS_OFF:
                    return "������� ���������";
                case OPOS_PS_OFFLINE:
                    return "����� ����������������";
                case OPOS_PS_OFF_OFFLINE:
                    return "������� ��������� ��� ����� ����������������";
                default:
                    return "����������� ��� ��������� �������";
            }
        }
    }
    #endregion
} // namespace