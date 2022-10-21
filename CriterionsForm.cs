using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saati
{
    public partial class CriterionsForm : Form
    {
        Form parentForm;
        List<string> AlternativesList;

        public CriterionsForm(Form ParentForm, List<string> AlternativesList)
        {
            InitializeComponent();

            this.AlternativesList = AlternativesList;
            parentForm = ParentForm;
        }

        private void AddCriterionBTN_Click(object sender, EventArgs e)
        {
            if (AddCriterionTB.Text != "")
                CriterionsListLSTBX.Items.Add(AddCriterionTB.Text);
            else
                MessageBox.Show("Ошибка! Вы не задали критерий!");
        }

        private void CriterionsForm_Load(object sender, EventArgs e)
        {
            
        }

        private void DelteCriterionBTN_Click(object sender, EventArgs e)
        {
            if (CriterionsListLSTBX.SelectedItems.Count > 0)
                CriterionsListLSTBX.Items.RemoveAt(CriterionsListLSTBX.SelectedIndex);
            else
                MessageBox.Show("Ошибка! Для удаления выделите объект критерия");
        }

        private void CriterionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            parentForm.Close();
        }

        private void NextStepBTN_Click(object sender, EventArgs e)
        {
            var CountCriterions = CriterionsListLSTBX.Items.Count;
            if (CountCriterions > 1 && CriterionsListLSTBX.CheckedIndices.Count != 0)
            {
                List<string> CriterionsList = new List<string>();
                bool[] CriterionsTypes = new bool[CountCriterions];
                for (int i = 0; i < CountCriterions; i++)
                {
                    CriterionsList.Add(CriterionsListLSTBX.Items[i].ToString());

                    for (int j = 0; j < CriterionsListLSTBX.CheckedIndices.Count; j++)
                    {
                        if (CriterionsListLSTBX.CheckedIndices[j] == i)
                            CriterionsTypes[i] = true;
                    }
                }
                PrioritiesForm prioritiesForm = new PrioritiesForm(this, AlternativesList, CriterionsList, CriterionsTypes);
                prioritiesForm.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Для анализа недостаточно критериев!");
        }
    }
}
