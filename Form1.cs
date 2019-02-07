using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uFR;

namespace uFR_AES_tester
{
    using DL_STATUS = System.UInt32;

    public partial class Form1 : Form
    {
        DL_STATUS status;

        public Form1()
        {
            InitializeComponent();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private void bStoreAESKeyIntoReader_Click(object sender, EventArgs e)
        {
            byte[] aes_key = new byte[16];
            byte aes_key_no = Byte.Parse(InternalKeyNumberCB.Text);

            aes_key = StringToByteArray(AesKeyInternalTB.Text);

            status = (UInt32)uFCoder.uFR_int_DesfireWriteAesKey(aes_key_no, aes_key);

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireWriteAesKey = " + uFCoder.status2str((uFR.DL_STATUS)status);
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireWriteAesKey = " + uFCoder.status2str((uFR.DL_STATUS)status);
            }
        }

        private void bInternalKeysLock_Click(object sender, EventArgs e)
        {
            String pass = PasswordTB.Text;

            if (pass.Length != 8)
            {
                MessageBox.Show("Pasword must be 8 character length");
            }
            else
            {
                status = (UInt32)uFCoder.ReaderKeysLock(pass.ToCharArray());

                if (status == 0)
                {
                    StatusLabel.Text = "Operation completed";
                    StatusRTB.Text = "ReaderKeysLock = " + uFCoder.status2str((uFR.DL_STATUS)status);
                }
                else
                {
                    StatusLabel.Text = "Operation not completed";
                    StatusRTB.Text = "ReaderKeysLock = " + uFCoder.status2str((uFR.DL_STATUS)status);
                }
            }
        }

        private void bInternalKeysUnlock_Click(object sender, EventArgs e)
        {
            String pass = PasswordTB.Text;

            if (pass.Length != 8)
            {
                MessageBox.Show("Pasword must be 8 character length");
            }
            else
            {
                status = (UInt32)uFCoder.ReaderKeysUnlock(pass.ToCharArray());

                if (status == 0)
                {
                    StatusLabel.Text = "Operation completed";
                    StatusRTB.Text = "ReaderKeysUnlock = " + uFCoder.status2str((uFR.DL_STATUS)status);
                }
                else
                {
                    StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status); ;
                    StatusRTB.Text = "ReaderKeysUnlock = " + uFCoder.status2str((uFR.DL_STATUS)status);
                }
            }
        }

        private void bChangeAesKeyCard_Click(object sender, EventArgs e)
        {
            byte[] old_key = new byte[16];
            byte[] new_key = new byte[16];
            byte aid_key_no = Byte.Parse(AIDKeyNrChangeCH.Text);
            UInt16 card_status;
            UInt16 exec_time;

            old_key = StringToByteArray(OldKeyTB.Text);
            new_key = StringToByteArray(NewKeyTB.Text);

            if (UseInternal.Checked == true)
            {
                byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);
                UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
                byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireChangeAesKey(aes_key_nr, aid, aid_key_nr_auth, new_key, aid_key_no, old_key, out card_status, out exec_time);

            }
            else
            {
                byte[] aes_key_ext = new byte[16];
                aes_key_ext = StringToByteArray(AESkeyTB.Text);
                UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
                byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireChangeAesKey_PK(aes_key_ext, aid, aid_key_nr_auth, new_key, aid_key_no, old_key, out card_status, out exec_time);

            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireChangeAesKey = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }

            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireChangeAesKey = " + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bGetKeySettings_Click(object sender, EventArgs e)
        {
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            byte setting;
            byte set_temp = 0;
            byte max_key_no;
            UInt16 card_status;
            UInt16 exec_time;

            if (UseInternal.Checked == true)
            {
                byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireGetKeySettings(aes_key_nr, aid, out setting, out max_key_no, out card_status, out exec_time);
            }
            else
            {
                byte[] aes_key_ext = new byte[16];
                aes_key_ext = StringToByteArray(AESkeyTB.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireGetKeySettings_PK(aes_key_ext, aid, out setting, out max_key_no, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireGetKeySettings = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nExecution time : " + exec_time.ToString() + " ms";
                    StatusRTB.Text += "\n\nMax key number : " + max_key_no.ToString();

                    setting &= 0x0F;
                    switch (setting)
                    {
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_CHANGE_KEY_CHANGE:
                            set_temp = 0;
                            break;
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_CHANGE_KEY_NOT_CHANGE:
                            set_temp = 1;
                            break;
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_CHANGE_KEY_CHANGE:
                            set_temp = 2;
                            break;
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_CHANGE_KEY_NOT_CHANGE:
                            set_temp = 3;
                            break;
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_NOT_CHANGE_KEY_CHANGE:
                            set_temp = 4;
                            break;
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_NOT_CHANGE_KEY_NOT_CHANGE:
                            set_temp = 5;
                            break;
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_NOT_CHANGE_KEY_CHANGE:
                            set_temp = 6;
                            break;
                        case (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_NOT_CHANGE_KEY_NOT_CHANGE:
                            set_temp = 7;
                            break;
                    }

                    int ChBxSett = int.Parse((set_temp & 0x04).ToString());
                    int chBxCreate = int.Parse((set_temp & 0x02).ToString());
                    int ChBxMaster = int.Parse((set_temp & 0x01).ToString());


                    if (ChBxSett > 0)
                        ChBxSett_1.Checked = true;
                    else
                        ChBxSett_1.Checked = false;
                    if (chBxCreate > 0)
                        ChBxCreate_1.Checked = true;
                    else
                        ChBxCreate_1.Checked = false;
                    if (ChBxMaster > 0)
                        ChBxMaster_1.Checked = true;
                    else
                        ChBxMaster_1.Checked = false;
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }

            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireGetKeySettings = " + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bGetCardUID_Click(object sender, EventArgs e)
        {
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);
            byte[] card_uid = new byte[7];
            byte card_uid_len;
            UInt16 card_status;
            UInt16 exec_time;

            if (UseInternal.Checked == true)
            {
                byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                status = (UInt32)uFCoder.uFR_int_GetDesfireUid(aes_key_nr, aid, aid_key_nr_auth, card_uid, out card_uid_len, out card_status, out exec_time);
            }
            else
            {
                byte[] aes_key_ext = new byte[16];
                aes_key_ext = StringToByteArray(AESkeyTB.Text);

                status = (UInt32)uFCoder.uFR_int_GetDesfireUid_PK(aes_key_ext, aid, aid_key_nr_auth, card_uid, out card_uid_len, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_GetDesfireUid = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nExecution time : " + exec_time.ToString() + " ms";
                    StatusRTB.Text += "\n\nCard UID : " + BitConverter.ToString(card_uid).Replace("-", " ");
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }

            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_GetDesfireUid = " + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }

        }

        private void bChangeKeySettings_Click(object sender, EventArgs e)
        {
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            byte setting = 0;
            byte set_temp = 0;
            UInt16 card_status;
            UInt16 exec_time;

            if (ChBxSett_1.Checked)
                set_temp |= 0x04;
            if (ChBxCreate_1.Checked)
                set_temp |= 0x02;
            if (ChBxMaster_1.Checked)
                set_temp |= 0x01;

            switch (set_temp)
            {
                case 0:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_CHANGE_KEY_CHANGE;
                    break;
                case 1:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_CHANGE_KEY_NOT_CHANGE;
                    break;
                case 2:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_CHANGE_KEY_CHANGE;
                    break;
                case 3:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_CHANGE_KEY_NOT_CHANGE;
                    break;
                case 4:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_NOT_CHANGE_KEY_CHANGE;
                    break;
                case 5:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_NOT_CHANGE_KEY_NOT_CHANGE;
                    break;
                case 6:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_NOT_CHANGE_KEY_CHANGE;
                    break;
                case 7:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_NOT_CHANGE_KEY_NOT_CHANGE;
                    break;
            }

            if (UseInternal.Checked == true)
            {
                byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireChangeKeySettings(aes_key_nr, aid, setting, out card_status, out exec_time);
            }
            else
            {
                byte[] aes_key_ext = new byte[16];
                aes_key_ext = StringToByteArray(AESkeyTB.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireChangeKeySettings_PK(aes_key_ext, aid, setting, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireChangeKeySettings = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nKey settings are changed\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }

            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireChangeKeySettings = " + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bMakeApplication_Click(object sender, EventArgs e)
        {
            byte setting = 0;
            byte set_temp = 0;
            UInt16 card_status;
            UInt16 exec_time;

            if (ChBxSett_1.Checked)
                set_temp |= 0x04;
            if (ChBxCreate_1.Checked)
                set_temp |= 0x02;
            if (ChBxMaster_1.Checked)
                set_temp |= 0x01;

            switch (set_temp)
            {
                case 0:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_CHANGE_KEY_CHANGE;
                    break;
                case 1:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_CHANGE_KEY_NOT_CHANGE;
                    break;
                case 2:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_CHANGE_KEY_CHANGE;
                    break;
                case 3:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_CHANGE_KEY_NOT_CHANGE;
                    break;
                case 4:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_NOT_CHANGE_KEY_CHANGE;
                    break;
                case 5:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITHOUT_AUTH_SET_NOT_CHANGE_KEY_NOT_CHANGE;
                    break;
                case 6:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_NOT_CHANGE_KEY_CHANGE;
                    break;
                case 7:
                    setting = (byte)DESFIRE_KEY_SETTINGS.DESFIRE_KEY_SET_CREATE_WITH_AUTH_SET_NOT_CHANGE_KEY_NOT_CHANGE;
                    break;
            }

            UInt32 aid = Convert.ToUInt32(CreateAppAID.Text, 16);
            byte max_key_no = Byte.Parse(CreateAppMaxKeyNo.Text);

            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked == true)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireCreateAesApplication(aes_key_nr, aid, setting, max_key_no, out card_status, out exec_time);
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireCreateAesApplication_PK(aes_key_ext, aid, setting, max_key_no, out card_status, out exec_time);
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireCreateAesApplication_no_auth(aid, setting, max_key_no, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireCreateAesApplication = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nApplication created\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }

            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireCreateAesApplication = " + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }

        }

        private void bMakeFile_Click(object sender, EventArgs e)
        {
            UInt16 card_status;
            UInt16 exec_time;
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            byte file_id = Byte.Parse(FileIDTB.Text);
            UInt32 file_size = UInt32.Parse(SizeOfFileTB.Text);
            byte read_key_no = Byte.Parse(ReadKeyNoCB.Text);
            byte write_key_no = Byte.Parse(WriteKeyNoCB.Text);
            byte read_write_key_no = Byte.Parse(RWKeyNoCB.Text);
            byte change_key_no = Byte.Parse(ChangeKeyNoCB.Text);
            byte communication_settings = 0;
            Int32 lower_limit = Int32.Parse(LowerLimitTB.Text);
            Int32 upper_limit = Int32.Parse(UpperLimitTB.Text);
            Int32 value = Int32.Parse(ValueFileTB.Text);
            byte limited_credit_enabled = 0;

            if (LimitedCreditCB.Checked)
            {
                limited_credit_enabled = 1;
            }
            else
            {
                limited_credit_enabled = 0;
            }

            if (FreeGetValueCB.Checked)
            {
                limited_credit_enabled |= 0x02;
            }

            if (FileCommMode.SelectedIndex == 0)
            {
                communication_settings = 0;
            }
            else if (FileCommMode.SelectedIndex == 1)
            {
                communication_settings = 1;
            }
            else if (FileCommMode.SelectedIndex == 2)
            {
                communication_settings = 3;
            }
            else
            {
                communication_settings = 0;
            }

            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                    if (RGStandard.Checked)
                    {
                        status = (UInt32)uFCoder.uFR_int_DesfireCreateStdDataFile(aes_key_nr, aid, file_id, file_size, read_key_no,
                                 write_key_no, read_write_key_no, change_key_no, communication_settings, out card_status, out exec_time);

                    }
                    else
                    {

                        status = (UInt32)uFCoder.uFR_int_DesfireCreateValueFile(aes_key_nr, aid, file_id, lower_limit, upper_limit, value,
                                 limited_credit_enabled, read_key_no, write_key_no, read_write_key_no, change_key_no, communication_settings,
                                 out card_status, out exec_time);
                    }
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);

                    if (RGStandard.Checked)
                    {
                        status = (UInt32)uFCoder.uFR_int_DesfireCreateStdDataFile_PK(aes_key_ext, aid, file_id, file_size, read_key_no,
                                 write_key_no, read_write_key_no, change_key_no, communication_settings, out card_status, out exec_time);

                    }
                    else
                    {

                        status = (UInt32)uFCoder.uFR_int_DesfireCreateValueFile_PK(aes_key_ext, aid, file_id, lower_limit, upper_limit, value,
                                 limited_credit_enabled, read_key_no, write_key_no, read_write_key_no, change_key_no, communication_settings,
                                 out card_status, out exec_time);
                    }
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireCreateStdDataFile_no_auth(aid, file_id, file_size, read_key_no,
                                 write_key_no, read_write_key_no, change_key_no, communication_settings, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireCreateStdDataFile = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nFile created\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireCreateStdDataFile = " + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void RGStandard_CheckedChanged(object sender, EventArgs e)
        {
            LowerLimitTB.Enabled = false;
            UpperLimitTB.Enabled = false;
            ValueFileTB.Enabled = false;
            LimitedCreditCB.Enabled = false;
            FreeGetValueCB.Enabled = false;
            SizeOfFileTB.Enabled = true;
        }

        private void RGValue_CheckedChanged(object sender, EventArgs e)
        {
            LowerLimitTB.Enabled = true;
            UpperLimitTB.Enabled = true;
            ValueFileTB.Enabled = true;
            LimitedCreditCB.Enabled = true;
            FreeGetValueCB.Enabled = true;
            SizeOfFileTB.Enabled = false;
        }

        private void bDeleteFile_Click(object sender, EventArgs e)
        {
            UInt16 card_status;
            UInt16 exec_time;
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            byte file_id = Byte.Parse(FileIDTB.Text);

            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireDeleteFile(aes_key_nr, aid, file_id, out card_status, out exec_time);
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireDeleteFile_PK(aes_key_ext, aid, file_id, out card_status, out exec_time);
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireDeleteFile_no_auth(aid, file_id, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireDeleteFile = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nFile deleted\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireDeleteFile" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bFileWriting_Click(object sender, EventArgs e)
        {
            UInt16 card_status;
            UInt16 exec_time;
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);
            byte file_id = Byte.Parse(FileIDForWork.Text);
            byte file_length = 0;
            byte communication_settings = 0;
            byte[] file_data = new byte[10000];

            if (StandardCommMode.SelectedIndex == 0)
            {
                communication_settings = 0;
            }
            else if (StandardCommMode.SelectedIndex == 1)
            {
                communication_settings = 1;
            }
            else if (StandardCommMode.SelectedIndex == 2)
            {
                communication_settings = 3;
            }
            else
            {
                communication_settings = 0;
            }

            OpenFileDialog openDialog = new OpenFileDialog();

            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                /* System.IO.StreamReader sr = new System.IO.StreamReader(openDialog.FileName);
                 String dataFromFile = sr.ReadToEnd();
                 file_data = sr.
                 file_length = (byte)dataFromFile.Length;

                 sr.Close();*/

                file_data = File.ReadAllBytes(openDialog.FileName);
                file_length = (byte)file_data.Length;
            }


            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireWriteStdDataFile(aes_key_nr, aid, aid_key_nr_auth, file_id, 0, file_length,
                             communication_settings, file_data, out card_status, out exec_time);
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireWriteStdDataFile_PK(aes_key_ext, aid, aid_key_nr_auth, file_id, 0, file_length,
                             communication_settings, file_data, out card_status, out exec_time);
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireWriteStdDataFile_no_auth(aid, aid_key_nr_auth, file_id, 0, file_length,
                             communication_settings, file_data, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireDeleteFile = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nFile written\nExecution time : " + exec_time.ToString() + " ms";
                    FileLength.Text = file_length.ToString();
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireDeleteFile" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bFormatCard_Click(object sender, EventArgs e)
        {
            UInt16 card_status;
            UInt16 exec_time;

            if (UseInternal.Checked)
            {
                byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireFormatCard(aes_key_nr, out card_status, out exec_time);
            }
            else
            {
                byte[] aes_key_ext = new byte[16];
                aes_key_ext = StringToByteArray(AESkeyTB.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireFormatCard_PK(aes_key_ext, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireFormatCard = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nCard formatted\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireFormatCard" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bGetFreeMemory_Click(object sender, EventArgs e)
        {
            UInt16 card_status;
            UInt16 exec_time;
            UInt32 free_mem_byte;

            status = (UInt32)uFCoder.uFR_int_DesfireFreeMem(out free_mem_byte, out card_status, out exec_time);

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireFreeMem = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nFree memory : " + free_mem_byte.ToString() + "\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireFreeMem" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bDEStoAES_Click(object sender, EventArgs e)
        {
            status = (UInt32)uFCoder.DES_to_AES_key_type();

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "DES_to_AES_key_type = " + uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text += "\nKey type is changed to AES\nNew AES key is 00000000000000000000000000000000";
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "DES_to_AES_key_type" + uFCoder.status2str((uFR.DL_STATUS)status);
            }
        }

        private void bSetRandomID_Click(object sender, EventArgs e)
        {
            UInt16 card_status;
            UInt16 exec_time;

            if (UseInternal.Checked)
            {
                byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);
                status = (UInt32)uFCoder.uFR_int_DesfireSetConfiguration(aes_key_nr, 1, 0, out card_status, out exec_time);
            }
            else
            {
                byte[] aes_key_ext = new byte[16];
                aes_key_ext = StringToByteArray(AESkeyTB.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireSetConfiguration_PK(aes_key_ext, 1, 0, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireSetConfiguration = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nRandom ID is set : \nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireSetConfiguration" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bDeleteApplication_Click(object sender, EventArgs e)
        {
            UInt16 card_status;
            UInt16 exec_time;
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);

            if (UseInternal.Checked)
            {

                byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);
                status = (UInt32)uFCoder.uFR_int_DesfireDeleteApplication(aes_key_nr, aid, out card_status, out exec_time);
            }
            else
            {
                byte[] aes_key_ext = new byte[16];
                aes_key_ext = StringToByteArray(AESkeyTB.Text);

                status = (UInt32)uFCoder.uFR_int_DesfireDeleteApplication_PK(aes_key_ext, aid, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireDeleteApplication = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nApplication deleted\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireDeleteApplication" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bSetSpeed_Click(object sender, EventArgs e)
        {
            byte tx_speed;
            byte rx_speed;

            tx_speed = (byte)txCB.SelectedIndex;
            rx_speed = (byte)rxCB.SelectedIndex;

            status = (UInt32)uFCoder.SetSpeedPermanently(tx_speed, rx_speed);

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "SetSpeedPermanently = " + uFCoder.status2str((uFR.DL_STATUS)status);
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "SetSpeedPermanently" + uFCoder.status2str((uFR.DL_STATUS)status);
            }
        }

        private void bGetSpeed_Click(object sender, EventArgs e)
        {
            byte tx_speed;
            byte rx_speed;

            status = (UInt32)uFCoder.GetSpeedParameters(out tx_speed, out rx_speed);

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "GetSpeedParameters = " + uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text += "\nTX : " + txCB.Items[tx_speed];
                StatusRTB.Text += "\nRX : " + rxCB.Items[rx_speed];
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "GetSpeedParameters" + uFCoder.status2str((uFR.DL_STATUS)status);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txCB.SelectedIndex = 0;
            rxCB.SelectedIndex = 0;

            foreach (Control c in this.Controls)
            {
                if (c.GetType() == typeof(GroupBox))
                {
                    foreach (Control a in c.Controls)
                    {
                        if (a.GetType() == typeof(ComboBox))
                        {
                            ComboBox cb = (ComboBox)a;
                            cb.DropDownStyle = ComboBoxStyle.DropDownList;
                        }
                    }
                }

            }

        }

        private void bReadValue_Click(object sender, EventArgs e)
        {
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            UInt16 card_status;
            UInt16 exec_time;
            byte file_id = Byte.Parse(FileIDForWork.Text);
            byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);
            byte communication_settings = 0;
            Int32 value;

            if (ValueFileCommMode.SelectedIndex == 0)
            {
                communication_settings = 0;
            } else if (ValueFileCommMode.SelectedIndex == 1)
            {
                communication_settings = 1;
            } else if (ValueFileCommMode.SelectedIndex == 2)
            {
                communication_settings = 3;
            }
            else
            {
                communication_settings = 0;
            }

            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);
                    status = (UInt32)uFCoder.uFR_int_DesfireReadValueFile(aes_key_nr, aid, aid_key_nr_auth, file_id, communication_settings, out value, out card_status, out exec_time);
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);
                    status = (UInt32)uFCoder.uFR_int_DesfireReadValueFile_PK(aes_key_ext, aid, aid_key_nr_auth, file_id, communication_settings, out value, out card_status, out exec_time);
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireReadValueFile_no_auth(aid, aid_key_nr_auth, file_id, communication_settings, out value, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireReadValueFile = " + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nExecution time : " + exec_time.ToString() + " ms";
                    ValueView.Text = value.ToString();
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireReadValueFile" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bIncreaseValue_Click(object sender, EventArgs e)
        {
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            UInt16 card_status;
            UInt16 exec_time;
            byte file_id = Byte.Parse(FileIDForWork.Text);
            byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);
            byte communication_settings = 0;
            Int32 value = Int32.Parse(ValueView.Text);

            if (ValueFileCommMode.SelectedIndex == 0)
            {
                communication_settings = 0;
            }
            else if (ValueFileCommMode.SelectedIndex == 1)
            {
                communication_settings = 1;
            }
            else if (ValueFileCommMode.SelectedIndex == 2)
            {
                communication_settings = 3;
            }
            else
            {
                communication_settings = 0;
            }

            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);
                    status = (UInt32)uFCoder.uFR_int_DesfireIncreaseValueFile(aes_key_nr, aid, aid_key_nr_auth, file_id, communication_settings, value, out card_status, out exec_time);
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);
                    status = (UInt32)uFCoder.uFR_int_DesfireIncreaseValueFile_PK(aes_key_ext, aid, aid_key_nr_auth, file_id, communication_settings, value, out card_status, out exec_time);
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireIncreaseValueFile_no_auth(aid, aid_key_nr_auth, file_id, communication_settings, value, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireIncreaseValueFile =" + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireIncreaseValueFile" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bDecreaseValue_Click(object sender, EventArgs e)
        {
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            UInt16 card_status;
            UInt16 exec_time;
            byte file_id = Byte.Parse(FileIDForWork.Text);
            byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);
            byte communication_settings = 0;
            Int32 value = Int32.Parse(ValueView.Text);

            if (ValueFileCommMode.SelectedIndex == 0)
            {
                communication_settings = 0;
            }
            else if (ValueFileCommMode.SelectedIndex == 1)
            {
                communication_settings = 1;
            }
            else if (ValueFileCommMode.SelectedIndex == 2)
            {
                communication_settings = 3;
            }
            else
            {
                communication_settings = 0;
            }

            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);
                    status = (UInt32)uFCoder.uFR_int_DesfireDecreaseValueFile(aes_key_nr, aid, aid_key_nr_auth, file_id, communication_settings, value, out card_status, out exec_time);
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);
                    status = (UInt32)uFCoder.uFR_int_DesfireDecreaseValueFile_PK(aes_key_ext, aid, aid_key_nr_auth, file_id, communication_settings, value, out card_status, out exec_time);
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireDecreaseValueFile_no_auth(aid, aid_key_nr_auth, file_id, communication_settings, value, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireDecreaseValueFile =" + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK\nExecution time : " + exec_time.ToString() + " ms";
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }
            else
            {
                StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                StatusRTB.Text = "uFR_int_DesfireDecreaseValueFile" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                 "\nExecution time : " + exec_time.ToString() + " ms";
            }
        }

        private void bFileReading_Click(object sender, EventArgs e)
        {
            UInt32 aid = Convert.ToUInt32(AIDCardAuth.Text, 16);
            UInt16 card_status;
            UInt16 exec_time;
            byte file_id = Byte.Parse(FileIDForWork.Text);
            byte aid_key_nr_auth = Byte.Parse(AIDKeyNrAuth.Text);
            byte file_length = Byte.Parse(FileLength.Text);
            byte communication_settings = 0;
            byte[] data = new byte[10000];

            if (StandardCommMode.SelectedIndex == 0)
            {
                communication_settings = 0;
            }
            else if (StandardCommMode.SelectedIndex == 1)
            {
                communication_settings = 1;
            }
            else if (StandardCommMode.SelectedIndex == 2)
            {
                communication_settings = 3;
            }
            else
            {
                communication_settings = 0;
            }

            if (MasterKeyAuthRequired.Checked)
            {
                if (UseInternal.Checked)
                {
                    byte aes_key_nr = Byte.Parse(InternalKeyNumberAuth.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireReadStdDataFile(aes_key_nr, aid, aid_key_nr_auth, file_id, 0, file_length,
                    communication_settings, data, out card_status, out exec_time);
                }
                else
                {
                    byte[] aes_key_ext = new byte[16];
                    aes_key_ext = StringToByteArray(AESkeyTB.Text);

                    status = (UInt32)uFCoder.uFR_int_DesfireReadStdDataFile_PK(aes_key_ext, aid, aid_key_nr_auth, file_id, 0, file_length,
                    communication_settings, data, out card_status, out exec_time);
                }
            }
            else
            {
                status = (UInt32)uFCoder.uFR_int_DesfireReadStdDataFile_no_auth(aid, aid_key_nr_auth, file_id, 0, file_length,
                communication_settings, data, out card_status, out exec_time);
            }

            if (status == 0)
            {
                StatusLabel.Text = "Operation completed";
                StatusRTB.Text = "uFR_int_DesfireReadStdDataFile =" + uFCoder.status2str((uFR.DL_STATUS)status);

                if (card_status == (UInt16)DESFIRE_CARD_STATUS_CODES.CARD_OPERATION_OK)
                {
                    byte[] reading = new byte[file_length];
                    Array.Copy(data, reading, file_length);

                    StatusRTB.Text += "\nCard status : CARD_OPERATION_OK";
                    StatusRTB.Text += "\n\nExecution time : " + exec_time.ToString() + " ms";

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.ShowDialog();

                    if (saveFileDialog1.FileName != "")
                    {
                        File.WriteAllBytes(saveFileDialog1.FileName, reading);
                    }
                }
                else
                {
                    StatusLabel.Text = Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status);
                    StatusRTB.Text += "\nCard status : " + Enum.GetName(typeof(DESFIRE_CARD_STATUS_CODES), card_status) + "\nExecution time : " + exec_time.ToString() + " ms";
                }
                }
                else
                {
                    StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                    StatusRTB.Text = "uFR_int_DesfireReadStdDataFile" + uFCoder.status2str((uFR.DL_STATUS)status) +
                                     "\nExecution time : " + exec_time.ToString() + " ms";
                }
            }

            private void bAEStoDES_Click(object sender, EventArgs e)
            {
                status = (UInt32)uFCoder.AES_to_DES_key_type();

                if (status == 0)
                {
                    StatusLabel.Text = "Operation completed";
                    StatusRTB.Text = "AES_to_DES_key_type =" + uFCoder.status2str((uFR.DL_STATUS)status);
                }
                else
                {
                    StatusLabel.Text = uFCoder.status2str((uFR.DL_STATUS)status);
                    StatusRTB.Text = "AES_to_DES_key_type" + uFCoder.status2str((uFR.DL_STATUS)status);
                }
            }

            private void timer1_Tick(object sender, EventArgs e)
            {
                status = (UInt32)uFCoder.ReaderOpen();

                if (status == 0)
                {
                    StatusLabel.Text = "Communication port opened";
                    timer1.Stop();
                    StatusRTB.Text = uFCoder.GetDescription();
                }
                else
                {
                    StatusLabel.Text = "Port not opened. Status is : " + uFCoder.status2str((uFR.DL_STATUS)status) + "    Searching for reader ...";
                }
            }
        }
    }
