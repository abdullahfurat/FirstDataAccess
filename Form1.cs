using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstDataAccess.Models;

namespace FirstDataAccess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PhoneBookEntities db = new PhoneBookEntities();
        void Clear()
        {
            foreach (Control item in groupBox1.Controls)
            {
                if (item is TextBox)
                {
                    item.Text = "";
                }
            }

        }
        void KisiListesi()
        {
            lstPersonel.Items.Clear();
            var kisiler = db.People.ToList();

            foreach (Person person in kisiler)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = person.FirstName;
                lvi.SubItems.Add(person.LastName);
                lvi.SubItems.Add(person.Phone);
                lvi.SubItems.Add(person.Mail);
                lvi.Tag = person.Id;
                
                lstPersonel.Items.Add(lvi);
            }

        }

        void KisiListesi(string param)
        {
            lstPersonel.Items.Clear();
            var kisiler = db.People.Where
                (x=>x.FirstName.Contains(param)||
                x.LastName.Contains(param) ||
                x.Phone.Contains(param)||
                x.Mail.Contains(param)

                )  .ToList();

            foreach (Person person in kisiler)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = person.FirstName;
                lvi.SubItems.Add(person.LastName);
                lvi.SubItems.Add(person.Phone);
                lvi.SubItems.Add(person.Mail);
                lvi.Tag = person.Id;

                lstPersonel.Items.Add(lvi);
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            KisiListesi();
        }

        private void TsmYeniKayitEkle_Click(object sender, EventArgs e)
        {
            txtAd.Text = FakeData.NameData.GetFirstName();
            txtSoyad.Text = FakeData.NameData.GetSurname();
            txtPhone.Text = FakeData.PhoneNumberData.GetPhoneNumber();
            txtMail.Text = $"{txtAd.Text}.{txtSoyad.Text}@{FakeData.NameData.GetCompanyName()}.com".ToLower().Replace(" ","");
            
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            Person person = new Person();
            person.FirstName = txtAd.Text;
            person.LastName = txtSoyad.Text;
            person.Phone = txtPhone.Text;
            person.Mail = txtMail.Text;

            db.People.Add(person);
            bool result= db.SaveChanges()>0   ;
            KisiListesi();
            Clear();
            MessageBox.Show(result ? "Kayıt Eklendi" : " İşlem Hatası");
        }

        private void TsnSil_Click(object sender, EventArgs e)
        {
            if (lstPersonel.SelectedItems.Count>0)
            {
                //çoklu silmeyi dene kendine ödev
               DialogResult dr= MessageBox.Show("Kişiyi Silmek istiyor musunuz ? \nİşlem Geri Alınamaz!", "Kişi Silme Bildirimi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr==DialogResult.Yes)
                {
                    int id = (int)lstPersonel.SelectedItems[0].Tag;
                    Person silinecek = db.People.Find(id);
                    db.People.Remove(silinecek);
                    db.SaveChanges();
                    KisiListesi();
                }
                else
                {
                    MessageBox.Show("Silme İşlemi İptal edilmiştir!");
                }
                
            }
            else
            {
                MessageBox.Show("Lütfen bir kişi seçiniz!");
            }

            
        }
        Person guncellenecek;

        private void TsnDuzenle_Click(object sender, EventArgs e)
        {
            if (lstPersonel.SelectedItems.Count>0)
            {
                int id = (int)lstPersonel.SelectedItems[0].Tag;
                guncellenecek = db.People.Find(id); //Find metodu primary key değerine göre size o kişiyi teslim eder.
                txtAd.Text = guncellenecek.FirstName;
                txtSoyad.Text = guncellenecek.LastName;
                txtMail.Text = guncellenecek.Mail;
                txtPhone.Text = guncellenecek.Phone;


            }
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (guncellenecek==null)
            {
                MessageBox.Show("Lütfen bir kayıt seçiniz!");
                return;
            }
            guncellenecek.FirstName = txtAd.Text;
            guncellenecek.LastName = txtSoyad.Text;
            guncellenecek.Phone = txtPhone.Text;
            guncellenecek.Mail = txtMail.Text;
            db.SaveChanges();
            Clear();
            KisiListesi();
            guncellenecek = null;


        }

        private void TxtArama_TextChanged(object sender, EventArgs e)
        {
            KisiListesi(txtArama.Text);
        }
    }
}
