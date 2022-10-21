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
    public partial class PrioritiesForm : Form
    {
        Form parentForm;
        List<string> AlternativesList;
        List<string> CriterionsList;
        bool[] CriterionsTypes;

        int columnIteraion = 0;
        int rowIteration = 0;

        decimal[,] priorities;
        decimal[] CriterionsWeights;

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

        private void DrawEmptyTable(DataGridView dataGridView)
        {
            int countCheckCriteries = trueCountCriterions(this.CriterionsTypes);

            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
            for (int i = 0; i < countCheckCriteries; i++)
            {
                dataGridView.Columns.Add("column " + i.ToString(), CriterionsList[i]);
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView.Rows.Add();
                dataGridView.Rows[i].HeaderCell.Value = CriterionsList[i];
            }
        }

        public PrioritiesForm(Form parentForm, List<string> AlternativesList, List<string> CriterionsList, bool[] CriterionsTypes)
        {
            InitializeComponent();

            this.parentForm = parentForm;
            this.AlternativesList = AlternativesList;
            this.CriterionsList = CriterionsList;
            this.CriterionsTypes = CriterionsTypes;
            this.priorities = new decimal[CriterionsList.Count, CriterionsList.Count];
            this.CriterionsWeights = new decimal[CriterionsList.Count];

            DrawEmptyTable(PrioritiesTableDGV);
        }

        private void PrioritiesForm_Load(object sender, EventArgs e)
        {
            rowIteration++; // здесь и увеличили счётчик cnhj на 1, т.к. на элементе 0/0 value = 1
            nextCompare(); // начиная заполнять с 0-ой строки 1-го столбца
        }

        private void nextCompare()
        {
            HowMuchLBL.Text = "Во сколько раз критерий " + CriterionsList[columnIteraion] + " важнее "+ CriterionsList[rowIteration] + "?";
        }

        // строка значения, которое задаёт эксперт
        private void HowMuchBTN_Click(object sender, EventArgs e)
        {
            int countCheckCriteries = trueCountCriterions(this.CriterionsTypes);

            // считываем значение и заносим в матрицу приоритетов по трансполяции 1/value
            priorities[columnIteraion, rowIteration] = HowMuchNUD.Value;
            priorities[rowIteration, columnIteraion] = 1 / HowMuchNUD.Value;

            rowIteration++;

            // переход к следующему столбцу 
            if (rowIteration == countCheckCriteries)
            {
                columnIteraion++;
                rowIteration = columnIteraion + 1;
            }

            // если ячейка матрицы не конечная (НЕ последняя строка и НЕ последний столбец)
            if (columnIteraion != countCheckCriteries && rowIteration < countCheckCriteries)
            {
                nextCompare(); // следующая итерация заполнения ячейки
                for (int i = 0; i < countCheckCriteries; i++)
                {
                    for (int j = 0; j < countCheckCriteries; j++)
                    {
                        PrioritiesTableDGV[j, i].Value = this.priorities[i, j].ToString();
                    }
                }

                for (int i = 0; i < countCheckCriteries; i++)
                {
                    this.priorities[i, i] = 1;
                }
            }
            else // когда дошли до последнего столбца или строки
            {
                HowMuchBTN.Enabled = HowMuchNUD.Enabled = false;
                NextStepBTN.Enabled = true;
                HowMuchLBL.Text = "Приоритеты для всех критериев введены";

                // если столбец уже последний
                if (rowIteration == this.CriterionsList.Count)
                {
                    columnIteraion++;
                    rowIteration = columnIteraion;
                }

                for (int i = 0; i < countCheckCriteries; i++)
                {
                    for (int j = 0; j < countCheckCriteries; j++)
                    {
                        PrioritiesTableDGV[j, i].Value = this.priorities[i, j].ToString();
                    }
                }

                for (int i = 0; i < countCheckCriteries; i++)
                {
                    this.priorities[i, i] = 1;
                }

                // заполнили дополнительную матрицу для введенных весов
                DrawEmptyTable(CleanTableDGV);
                for (int i = 0; i < countCheckCriteries; i++)
                {
                    for (int j = 0; j < countCheckCriteries; j++)
                    {
                        CleanTableDGV[j, i].Value = this.priorities[i, j].ToString();
                    }
                }

                PrioritiesTableDGV.Columns.Add("sumstr", "Вес");
                PrioritiesTableDGV.Rows.Add();
                PrioritiesTableDGV.Rows[PrioritiesTableDGV.Rows.Count - 1].Cells[PrioritiesTableDGV.Columns.Count - 2].Value = "Сумма";
                PrioritiesTableDGV.Rows[PrioritiesTableDGV.Rows.Count - 1].Cells[PrioritiesTableDGV.Columns.Count - 1].Value = "0";

                for (int i = 0; i < countCheckCriteries; i++)
                {
                    PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = "0"; // 4-й столбец с весами критериев

                    for (int j = 0; j < countCheckCriteries; j++)
                    {
                       PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = (Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value.ToString()) + Convert.ToDecimal(PrioritiesTableDGV[i, j].Value.ToString())).ToString();
                    }
                    PrioritiesTableDGV.Rows[PrioritiesTableDGV.Columns.Count - 1].Cells[i].Value = Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value.ToString());

                    for (int k = 0; k < countCheckCriteries; k++)
                    {
                        PrioritiesTableDGV.Rows[k].Cells[i].Value = Convert.ToDecimal(PrioritiesTableDGV.Rows[k].Cells[i].Value) / Convert.ToDecimal(PrioritiesTableDGV.Rows[PrioritiesTableDGV.Columns.Count - 1].Cells[i].Value);
                    }
                }

                PrioritiesTableDGV.Rows[countCheckCriteries].HeaderCell.Value = "Сумма";

                for (int i = 0; i < countCheckCriteries; i++)
                {
                    PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = "0";
                    for (int l = 0; l < countCheckCriteries; l++)
                    {
                        PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value = (Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value) + Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells[l].Value) / countCheckCriteries).ToString();
                        CriterionsWeights[i] = Convert.ToDecimal(PrioritiesTableDGV.Rows[i].Cells["sumstr"].Value);
                    }
                }
                PrioritiesTableDGV.Rows[PrioritiesTableDGV.Rows.Count - 1].Cells[PrioritiesTableDGV.Columns.Count - 1].Value = "1";
            }
        }

        private void PrioritiesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.parentForm.Close();
        }

        private void NextStepBTN_Click(object sender, EventArgs e)
        {
            CriterionsCompareForm criterionsCompareForm = new CriterionsCompareForm(this, AlternativesList, CriterionsList, CriterionsTypes, CriterionsWeights);
            criterionsCompareForm.Show();
            this.Hide();
        }

        private void bCalculate_Click(object sender, EventArgs e)
        {

        }
    }
}
