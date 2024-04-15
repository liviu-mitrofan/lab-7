using System;
using LibrarieModele;
using NivelStocareDate;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvidentaStudenti_UI_WindowsForms
{
    public partial class Form1 : Form
    {
        AdministrareStudenti_FisierText adminStudenti;

        private Label lblNume;
        private TextBox txtNume;
        private Label lblPrenume;
        private TextBox txtPrenume;
        private Label lblNote;
        private TextBox txtNote;

        private Label[] lblsNume;
        private Label[] lblsPrenume;
        private Label[] lblsNote;

        private Button btnAdauga;
        private Button btnRefresh;


        private const int LATIME_CONTROL = 100;
        private const int DIMENSIUNE_PAS_Y = 30;
        private const int DIMENSIUNE_PAS_X = 120;

        public Form1()
        {
            InitializeComponent();

            string numeFisier = ConfigurationManager.AppSettings["NumeFisier"];
            string locatieFisierSolutie = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            // Setare locatie fisier in directorul corespunzator solutiei
            // astfel incat datele din fisier sa poata fi utilizate si de alte proiecte
            string caleCompletaFisier = locatieFisierSolutie + "\\" + numeFisier;

            adminStudenti = new AdministrareStudenti_FisierText(caleCompletaFisier);
            int nrStudenti = 0;

            Student[] studenti = adminStudenti.GetStudenti(out nrStudenti);

            // Setare proprietati
            this.Size = new System.Drawing.Size(1000, 400);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(100, 100);
            this.Font = new Font("Arial", 9, FontStyle.Bold);
            this.ForeColor = Color.LimeGreen;
            this.Text = "Informatii studenti";

            // Adaugare control de tip Label pentru 'Nume'
            lblNume = new Label();
            lblNume.Width = LATIME_CONTROL;
            lblNume.Text = "Nume";
            lblNume.Left = DIMENSIUNE_PAS_X;
            lblNume.ForeColor = Color.DarkGreen;
            this.Controls.Add(lblNume);

            txtNume = new TextBox();
            txtNume.Width = LATIME_CONTROL;
            txtNume.Left = DIMENSIUNE_PAS_X;
            txtNume.Top = DIMENSIUNE_PAS_Y;
            this.Controls.Add(txtNume);

            // Adaugare control de tip Label pentru 'Prenume'
            lblPrenume = new Label();
            lblPrenume.Width = LATIME_CONTROL;
            lblPrenume.Text = "Prenume";
            lblPrenume.Left = 2 * DIMENSIUNE_PAS_X;
            lblPrenume.ForeColor = Color.DarkGreen;
            this.Controls.Add(lblPrenume);

            // Adaugare control de tip Label pentru 'Note'
            lblNote = new Label();
            lblNote.Width = LATIME_CONTROL;
            lblNote.Text = "Note";
            lblNote.Left = 3 * DIMENSIUNE_PAS_X;
            lblNote.ForeColor = Color.DarkGreen;
            this.Controls.Add(lblNote);

            // Text box for 'Prenume'
            txtPrenume = new TextBox();
            txtPrenume.Width = LATIME_CONTROL;
            txtPrenume.Left = 2 * DIMENSIUNE_PAS_X;
            txtPrenume.Top = DIMENSIUNE_PAS_Y;
            this.Controls.Add(txtPrenume);

            // Text box for 'Note'
            txtNote = new TextBox();
            txtNote.Width = 2 * LATIME_CONTROL;
            txtNote.Left = 3 * DIMENSIUNE_PAS_X;
            txtNote.Top = DIMENSIUNE_PAS_Y;
            this.Controls.Add(txtNote);

            // Button for 'Adauga'
            btnAdauga = new Button();
            btnAdauga.Text = "Adauga";
            btnAdauga.Width = LATIME_CONTROL;
            btnAdauga.Left = 5 * DIMENSIUNE_PAS_X;
            btnAdauga.Top = DIMENSIUNE_PAS_Y;
            btnAdauga.Click += new EventHandler(BtnAdauga_Click);
            this.Controls.Add(btnAdauga);

            // Button for 'Refresh'
            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.Width = LATIME_CONTROL;
            btnRefresh.Left = 5 * DIMENSIUNE_PAS_X;
            btnRefresh.Top = 2 * DIMENSIUNE_PAS_Y;
            btnRefresh.Click += new EventHandler(BtnRefresh_Click);
            this.Controls.Add(btnRefresh);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AfiseazaStudenti();
        }

        private void BtnAdauga_Click(object sender, EventArgs e)
        {
            // Validați datele introduse
            if (!ValideazaDate())
            {
                return;
            }

            // Creați un obiect de tip Student
            Student student = new Student();
            student.Nume = txtNume.Text;
            student.Prenume = txtPrenume.Text;
            string[] noteText = txtNote.Text.Split(' ');
            int[] note = new int[noteText.Length];
            for (int i = 0; i < noteText.Length; i++)
            {
                int notaValoare;
                if (Int32.TryParse(noteText[i], out notaValoare))
                {
                    note[i] = notaValoare;
                }
                else
                {
                    // Afișați eroare pentru nota invalidă
                    MessageBox.Show("Format invalid pentru nota " + noteText[i]);
                    return;
                }
            }
            student.SetNote(note);

            // Adăugați studentul în fișier și actualizați afișarea
            adminStudenti.AddStudent(student);
            AfiseazaStudenti();

            // Goliți câmpurile de introducere
            txtNume.Text = "";
            txtPrenume.Text = "";
            txtNote.Text = "";
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            AfiseazaStudenti();
        }

        private bool ValideazaDate()
        {
            // Verificați dacă câmpurile nume și prenume sunt completate
            if (string.IsNullOrEmpty(txtNume.Text) || string.IsNullOrEmpty(txtPrenume.Text))
            {
                MessageBox.Show("Numele și prenumele trebuie completate!");
                return false;
            }

            // Verificați dacă lungimea numelui și prenumelui nu depășește 15 caractere
            if (txtNume.Text.Length > 15 || txtPrenume.Text.Length > 15)
            {
                MessageBox.Show("Numele și prenumele nu pot depăși 15 caractere!");
                return false;
            }

            // Verificați formatul notelor
            string[] noteText = txtNote.Text.Split(' ');
            foreach (string notaText in noteText)
            {
                int notaValoare;
                if (!Int32.TryParse(notaText, out notaValoare))
                {
                    MessageBox.Show("Format invalid pentru nota " + notaText);
                    return false;
                }
            }

            return true;
        }
 


        private void AfiseazaStudenti()
        {
            Student[] studenti = adminStudenti.GetStudenti(out int nrStudenti);

            lblsNume = new Label[nrStudenti];
            lblsPrenume = new Label[nrStudenti];
            lblsNote = new Label[nrStudenti];

            int i = 0;
            foreach (Student student in studenti)
            {
                //adaugare control de tip Label pentru numele studentilor;
                lblsNume[i] = new Label();
                lblsNume[i].Width = LATIME_CONTROL;
                lblsNume[i].Text = student.Nume;
                lblsNume[i].Left = DIMENSIUNE_PAS_X;
                lblsNume[i].Top = (i + 1) * DIMENSIUNE_PAS_Y;
                this.Controls.Add(lblsNume[i]);

                //adaugare control de tip Label pentru prenumele studentilor
                lblsPrenume[i] = new Label();
                lblsPrenume[i].Width = LATIME_CONTROL;
                lblsPrenume[i].Text = student.Prenume;
                lblsPrenume[i].Left = 2 * DIMENSIUNE_PAS_X;
                lblsPrenume[i].Top = (i + 1) * DIMENSIUNE_PAS_Y;
                this.Controls.Add(lblsPrenume[i]);

                //adaugare control de tip Label pentru notele studentilor
                lblsNote[i] = new Label();
                lblsNote[i].Width = LATIME_CONTROL;
                lblsNote[i].Text = string.Join(" ", student.GetNote());
                lblsNote[i].Left = 3 * DIMENSIUNE_PAS_X;
                lblsNote[i].Top = (i + 1) * DIMENSIUNE_PAS_Y;
                this.Controls.Add(lblsNote[i]);
                i++;
            }
        }
    }
}
