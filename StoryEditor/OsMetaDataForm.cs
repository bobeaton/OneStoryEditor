using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using NetLoc;

namespace OneStoryProjectEditor
{
    public partial class OsMetaDataForm : Form
    {
        public OsMetaDataForm(OsMetaDataModel osMetaDataModel)
        {
            InitializeComponent();

            OsMetaDataModel = osMetaDataModel;
            InitializeUserInterface(osMetaDataModel.OsProjects.First());
        }

        private void InitializeUserInterface(OsMetaDataModelRecord record)
        {
            Initialize(textBoxProjectName, record.ProjectName);
            Initialize(textBoxLanguageName, record.LanguageName);
            Initialize(textBoxEthnologueCode, record.EthnologueCode);
            Initialize(comboBoxContinent, record.Continent);
            Initialize(textBoxCountry, record.Country);
            Initialize(comboBoxManagingPartner, record.ManagingPartner);
            Initialize(textBoxLocalEntity, record.Entity);
            Initialize(comboBoxPrioritiesCategory, record.PrioritiesCategory);
            Initialize(comboBoxScriptureStatus, record.ScriptureStatus);
            Initialize(textBoxScriptureStatusDetails, record.ScriptureStatusNotes);
            Initialize(textBoxFacilitators, record.ProjectFacilitators);
            Initialize(comboBoxTeamCategory, record.PfCategory);
            Initialize(textBoxTeamAffiliation, record.PfAffiliation);
            Initialize(textBoxNotes, record.Notes);
            Initialize(comboBoxStatus, record.Status);
            Initialize(dateTimePickerStartDate, record.StartDate);
            Initialize(textBoxLcaWorkshop, record.LcaWorkshop);
            Initialize(textBoxLcaCoach, record.LcaCoach);
            Initialize(textBoxScWorkshop, record.ScWorkshop);
            Initialize(textBoxEsConsultant, record.EsConsultant);
            Initialize(textBoxEsCoach, record.EsCoach);
            Initialize(textBoxNumOfEsStoriesSent, record.EsStoriesSent.ToString(CultureInfo.InvariantCulture));
            Initialize(textBoxProcessCheck, record.ProcessCheck);
            Initialize(textBoxMultiWorkshop, record.MultiplicationWorkshop);
            Initialize(textBoxNumOfSfgs, record.NumberSfgs.ToString(CultureInfo.InvariantCulture));
            Initialize(textBoxPsConsultant, record.PsConsultant);
            Initialize(textBoxPsCoach, record.PsCoach);
            Initialize(textBoxNumInPreliminaryApproval, record.PsStoriesPrelimApprov.ToString(CultureInfo.InvariantCulture));
            Initialize(textBoxLsr, record.Lsr);
            Initialize(textBoxFinalReview, record.NumInFinalApprov.ToString(CultureInfo.InvariantCulture));
            Initialize(dateTimePickerSetFinishedDate, record.SetFinishedDate);
            Initialize(checkBoxIsUploadedToOsMedia, record.IsUploadedToOsMedia);
            Initialize(textBoxSetCopyright, record.SetCopyrighted);

            // if we're in Exploratory Phase, then don't allow editing of the ES fields
            //  (since we'll just overwrite them anyway)
            var bEsFieldsReadOnly = (record.Status == StoryProjectData.CstrOsMetaDataStatusExploratory);
            textBoxEsConsultant.Enabled =
                textBoxEsCoach.Enabled = textBoxNumOfEsStoriesSent.Enabled = !bEsFieldsReadOnly;

            var bPsFieldsReadOnly = (record.Status == StoryProjectData.CstrOsMetaDataStatusProduction);
            textBoxPsConsultant.Enabled =
                textBoxPsCoach.Enabled = textBoxNumInPreliminaryApproval.Enabled = !bPsFieldsReadOnly;
        }

        private static void Initialize(CheckBox checkBox, bool checkedState)
        {
            checkBox.Checked = checkedState;
        }

        private static void Initialize(DateTimePicker dateTimePicker, DateTime dateTime)
        {
            if (dateTime != DateTime.MinValue)
                dateTimePicker.Value = dateTime;
        }

        private static void Initialize(ComboBox comboBox, string strToInitializeWith)
        {
            if (!String.IsNullOrEmpty(strToInitializeWith))
                comboBox.SelectedItem = strToInitializeWith;
        }

        private static void Initialize(Control textBox, string strToInitializeWith)
        {
            if (String.IsNullOrEmpty(textBox.Text))
                textBox.Text = strToInitializeWith;
        }

        public OsMetaDataModel OsMetaDataModel { get; set; }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                RetrieveData(OsMetaDataModel.OsProjects.First());
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }

        private void RetrieveData(OsMetaDataModelRecord record)
        {
            var bEsFieldsReadOnly = (record.Status == StoryProjectData.CstrOsMetaDataStatusExploratory);
            var bPsFieldsReadOnly = (record.Status == StoryProjectData.CstrOsMetaDataStatusProduction);

            record.ProjectName = Retrieve(textBoxProjectName);
            record.LanguageName = Retrieve(textBoxLanguageName);
            record.EthnologueCode = Retrieve(textBoxEthnologueCode);
            record.Continent = Retrieve(comboBoxContinent);
            record.Country = Retrieve(textBoxCountry);
            record.ManagingPartner = Retrieve(comboBoxManagingPartner);
            record.Entity = Retrieve(textBoxLocalEntity);
            record.PrioritiesCategory = Retrieve(comboBoxPrioritiesCategory);
            record.ScriptureStatus = Retrieve(comboBoxScriptureStatus);
            record.ScriptureStatusNotes = Retrieve(textBoxScriptureStatusDetails);
            // record.ProjectFacilitators = Retrieve(textBoxFacilitators);
            record.PfCategory = Retrieve(comboBoxTeamCategory);
            record.PfAffiliation = Retrieve(textBoxTeamAffiliation);
            record.Notes = Retrieve(textBoxNotes);
            record.Status = Retrieve(comboBoxStatus);
            record.StartDate = Retrieve(dateTimePickerStartDate, record.StartDate);
            record.LcaWorkshop = Retrieve(textBoxLcaWorkshop);
            record.LcaCoach = Retrieve(textBoxLcaCoach);
            record.ScWorkshop = Retrieve(textBoxScWorkshop);
            if (!bEsFieldsReadOnly)
            {
                record.EsConsultant = Retrieve(textBoxEsConsultant);
                record.EsCoach = Retrieve(textBoxEsCoach);
                record.EsStoriesSent = RetrieveInt(textBoxNumOfEsStoriesSent);
            }
            record.ProcessCheck = Retrieve(textBoxProcessCheck);
            record.MultiplicationWorkshop = Retrieve(textBoxMultiWorkshop);
            record.NumberSfgs = RetrieveInt(textBoxNumOfSfgs);

            if (!bPsFieldsReadOnly)
            {
                record.PsConsultant = Retrieve(textBoxPsConsultant);
                record.PsCoach = Retrieve(textBoxPsCoach);
                record.PsStoriesPrelimApprov = RetrieveInt(textBoxNumInPreliminaryApproval);
            }
            record.Lsr = Retrieve(textBoxLsr);
            // record.NumInFinalApprov = RetrieveInt(textBoxFinalReview);
            record.SetFinishedDate = Retrieve(dateTimePickerSetFinishedDate, record.SetFinishedDate);
            record.IsUploadedToOsMedia = Retrieve(checkBoxIsUploadedToOsMedia);
            record.SetCopyrighted = Retrieve(textBoxSetCopyright);
        }

        private static bool Retrieve(CheckBox checkBox)
        {
            return checkBox.Checked;
        }

        private static DateTime Retrieve(DateTimePicker dateTimePicker, DateTime dtCurrentValue)
        {
            return (dateTimePicker.Tag == null)
                        ? dtCurrentValue
                        : dateTimePicker.Value;
        }

        private static string Retrieve(Control textBox)
        {
            return textBox.Text;
        }

        private static int RetrieveInt(Control textBox)
        {
            var strValue = textBox.Text;
            try
            {
                return Convert.ToInt32(strValue);
            }
            catch
            {
                LocalizableMessageBox.Show(String.Format(Localizer.Str("The field named '{0}' was supposed to have a numeric value, but it wasn't. Please make the value a number."), textBox.Name), StoryEditor.OseCaption);
                throw;
            }
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            var dtPicker = sender as DateTimePicker;
            Debug.Assert(dtPicker != null, "dtPicker != null");
            dtPicker.Tag = 1;   // to indicate that it's been changed
        }
    }
}
