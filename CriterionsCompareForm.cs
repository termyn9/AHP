using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Saati
{
    public partial class CriterionsCompareForm : Form
    {
        Form parentForm;
        List<String> AlternativesList;
        List<String> CriterionsList;
        bool[] CriterionsTypes;
        decimal[] CriterionsWeights;
        decimal[,] CompareWeights;

        int criterionIteration = 0;
        int columnIteration = 0;
        int rowIteration = 0;
        decimal[,,] CompareAll;

        public CriterionsCompareForm(Form parentForm, List<String> AlternativesList, List<String> CriterionsList, bool[] CriterionsTypes, decimal[] CriterionsWeights)
        {
            InitializeComponent();

            this.parentForm = parentForm;
            this.AlternativesList = AlternativesList;
            this.CriterionsList = CriterionsList;
            this.CriterionsTypes = CriterionsTypes;
            this.CriterionsWeights = CriterionsWeights;

            DrawEmptyTable();

            this.CompareAll = new decimal[CriterionsList.Count, this.AlternativesList.Count, this.AlternativesList.Count];
            this.CompareWeights = new decimal[CriterionsList.Count, this.AlternativesList.Count];
        }
        private void DrawEmptyTable()
        {
            PrioritiesTableDGV.Rows.Clear();
            PrioritiesTableDGV.Columns.Clear();
            for (int i = 0; i < AlternativesList.Count; i++)
            {
                PrioritiesTableDGV.Columns.Add("column" + i.ToString(), AlternativesList[i]);
                PrioritiesTableDGV.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                PrioritiesTableDGV.Rows.Add();
                PrioritiesTableDGV.Rows[i].HeaderCell.Value = AlternativesList[i];
            }
        }

        public int trueCountCriterions(bool[] array)
        {
            var count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == true)
                    count++;
            }
            return count;
        }

        private void nextCompare()
        {
            HowMuchLBL.Text = "Критерий " + CriterionsList[criterionIteration] + " для " + AlternativesList[columnIteration] + " и " + AlternativesList[rowIteration] + "?";
        }

        private void nextCriterionIteration()
        {
            criterionIteration++;
            columnIteration = 0;
            rowIteration = 1;
            DrawEmptyTable();
        }

        private void CriterionsCompareForm_Load(object sender, EventArgs e)
        {
            rowIteration++;
            nextCompare();
        }

        private void HowMuchBTN_Click(object sender, EventArgs e)
        {
            this.CompareAll[criterionIteration, columnIteration, rowIteration] = HowMuchNUD.Value;
            this.CompareAll[criterionIteration, rowIteration, columnIteration] = 1 / HowMuchNUD.Value;

            rowIteration++;

            // если заполнили до конца 1-ый столбец, переходим к следующей
            if (rowIteration == this.AlternativesList.Count)
            {
                columnIteration++;
                rowIteration = columnIteration + 1;
            }

            // заполнение каждой колонки по итерациям строк
            if (columnIteration != this.AlternativesList.Count && rowIteration < this.AlternativesList.Count)
            {
                nextCompare();
                for (int i = 0; i < this.AlternativesList.Count; i++)
                {
                    for (int j = 0; j < this.AlternativesList.Count; j++)
                    {
                        PrioritiesTableDGV[j, i].Value = this.CompareAll[criterionIteration, i, j].ToString();
                    }
                }

                for (int i = 0; i < this.AlternativesList.Count; i++)
                {
                    this.CompareAll[criterionIteration, i, i] = 1;
                }
            }
            else   // заполнили матрицу для 1-го критерия
            {
                HowMuchBTN.Enabled = HowMuchNUD.Enabled = false;
                NextStepBTN.Enabled = true;
                HowMuchLBL.Text = "Сравнения для критерия " + CriterionsList[criterionIteration] + " произведены:";
                if (rowIteration == this.AlternativesList.Count)
                {
                    columnIteration++;
                    rowIteration = columnIteration;
                }
                for (int i = 0; i < this.AlternativesList.Count; i++)
                {
                    for (int j = 0; j < this.AlternativesList.Count; j++)
                    {
                        PrioritiesTableDGV[j, i].Value = this.CompareAll[criterionIteration, i, j].ToString();
                    }
                }
                for (int i = 0; i < this.AlternativesList.Count; i++)
                {
                    this.CompareAll[criterionIteration, i, i] = 1;
                }

                DrawEmptyTable();
                for (int i = 0; i < this.AlternativesList.Count; i++)
                {
                    for (int j = 0; j < this.AlternativesList.Count; j++)
                    {
                        PrioritiesTableDGV[j, i].Value = this.CompareAll[criterionIteration, i, j].ToString();
                    }
                }

                PrioritiesTableDGV.Columns.Add("sumstr", "Вес");
                PrioritiesTableDGV.Rows.Add();
                PrioritiesTableDGV.Rows[PrioritiesTableDGV.Rows.Count - 1].Cells[PrioritiesTableDGV.Columns.Count - 2].Value = "Сумма";
                PrioritiesTableDGV.Rows[PrioritiesTableDGV.Rows.Count - 1].Cells[PrioritiesTableDGV.Columns.Count - 1].Value = "0";
                for (int i = 0; i < this.AlternativesList.Count; i++)
                {
                    PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = "0";
                    for (int j = 0; j < this.AlternativesList.Count; j++)
                    {
                        PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = (Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value.ToString()) + Convert.ToDecimal(PrioritiesTableDGV[i, j].Value.ToString())).ToString();
                    }
                    PrioritiesTableDGV.Rows[PrioritiesTableDGV.Columns.Count - 1].Cells[i].Value = Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value.ToString());
                    for (int k = 0; k < this.AlternativesList.Count; k++)
                    {
                        PrioritiesTableDGV.Rows[k].Cells[i].Value = Convert.ToDecimal(PrioritiesTableDGV.Rows[k].Cells[i].Value) / Convert.ToDecimal(PrioritiesTableDGV.Rows[PrioritiesTableDGV.Columns.Count - 1].Cells[i].Value);
                    }
                }
                PrioritiesTableDGV.Rows[this.AlternativesList.Count].HeaderCell.Value = "Сумма";

                for (int i = 0; i < this.AlternativesList.Count; i++)
                {
                    PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = "0";
                    for (int l = 0; l < this.AlternativesList.Count; l++)
                    {
                        PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = (Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value) + Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells[l].Value) / this.AlternativesList.Count).ToString();
                        CompareWeights[criterionIteration, i] = Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value);
                    }
                }
                PrioritiesTableDGV.Rows[PrioritiesTableDGV.Rows.Count - 1].Cells[PrioritiesTableDGV.Columns.Count - 1].Value = "1";
            }
        }

        private void CriterionsCompareForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.parentForm.Close();
        }

        private void NextStepBTN_Click(object sender, EventArgs e)
        {
            HowMuchBTN.Enabled = HowMuchNUD.Enabled = true;
            NextStepBTN.Enabled = false;
            nextCriterionIteration();
            int countCheckCriteries = trueCountCriterions(this.CriterionsTypes);

            if (criterionIteration == countCheckCriteries)
            {
                HowMuchBTN.Enabled = HowMuchNUD.Enabled = NextStepBTN.Enabled = false;
                ResultForm resultForm = new ResultForm(this, AlternativesList, CriterionsList, CriterionsTypes, CriterionsWeights, CompareWeights);
                resultForm.Show();
                this.Hide();
            }
            else
            {
                nextCompare();
            }
        }
    }
}
