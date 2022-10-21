﻿using System;
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
    public partial class AlternativesForm : Form
    {
        public AlternativesForm()
        {
            InitializeComponent();
        }

        private void AddAlternativeBTN_Click(object sender, EventArgs e)
        {
            if (AddAlternativeTB.Text != "")
                AlternativesListLSTBX.Items.Add(AddAlternativeTB.Text);
            else
                MessageBox.Show("Ошибка! Вы не задали новый оцениваемый объект!");
        }

        private void DelteAlternatuveBTN_Click(object sender, EventArgs e)
        {
            if (AlternativesListLSTBX.SelectedItems.Count > 0)
                AlternativesListLSTBX.Items.RemoveAt(AlternativesListLSTBX.SelectedIndex);
            else
                MessageBox.Show("Ошибка! Для удаления выделите объект");
        }

        private void NextStepBTN_Click(object sender, EventArgs e)
        {
            if (AlternativesListLSTBX.Items.Count > 1)
            {
                List<string> AlternativesList = new List<string>();
                for (int i = 0; i < AlternativesListLSTBX.Items.Count; i++)
                {
                    AlternativesList.Add(AlternativesListLSTBX.Items[i].ToString());
                }
                CriterionsForm criterionsForm = new CriterionsForm(this, AlternativesList);
                criterionsForm.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Для анализа недостаточно объектов!");
        }
    }
}
