using System; 
using System.Data; 
using System.Drawing; 
using System.Windows.Forms; 

namespace csharprenata2 
{
    public partial class Form1 : Form // Pastikan nama kelas sesuai dengan nama form Anda (Form1)
    {
        private DatabaseManager dbManager; // Objek untuk mengelola database 
        private string selectedNIK = string.Empty; // Untuk menyimpan NIK yang sedang dipilih/diedit 

        public Form1()
        {
            InitializeComponent();
            dbManager = new DatabaseManager(); // Inisialisasi DatabaseManager 
            // Panggil metode untuk memuat data saat form pertama kali dimuat 
            this.Load += new EventHandler(Form1_Load);

            // Konfigurasi awal untuk tombol Hapus dan Ubah 
            btnHapus.Enabled = false;
            btnUbah.Enabled = false;
        }

        // Metode untuk memuat data dari database ke DataGridView 
        private void LoadDataToGrid()
        {
            DataTable dtWarga = dbManager.GetAllWarga();
            dgvWarga.DataSource = dtWarga;
            // Optional: Atur lebar kolom agar lebih rapi 
            dgvWarga.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            // Bersihkan seleksi setelah memuat ulang data 
            dgvWarga.ClearSelection();
            selectedNIK = string.Empty; // Reset NIK yang dipilih 
            btnHapus.Enabled = false;
            btnUbah.Enabled = false;
            txtNIK.ReadOnly = false; // NIK bisa diedit saat tidak ada seleksi 
        }

        // Event handler saat form dimuat pertama kali 
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDataToGrid(); // Muat data saat form pertama kali tampil 
            // Pastikan ComboBox memiliki nilai default yang dipilih 
            if (cmbJenisKelamin.Items.Count > 0) cmbJenisKelamin.SelectedIndex = 0;
            if (cmbStatusPerkawinan.Items.Count > 0) cmbStatusPerkawinan.SelectedIndex = 0;
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // Validasi input wajib dan fokus ke field yang kosong
            if (string.IsNullOrWhiteSpace(txtNIK.Text))
            {
                MessageBox.Show("NIK harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIK.Focus();
                return;
            }
            if (txtNIK.Text.Length != 10)
            {
                MessageBox.Show("NIK harus terdiri dari 10 karakter.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNIK.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNamaLengkap.Text))
            {
                MessageBox.Show("Nama Lengkap harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamaLengkap.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(cmbJenisKelamin.Text))
            {
                MessageBox.Show("Jenis Kelamin harus dipilih.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbJenisKelamin.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAlamat.Text))
            {
                MessageBox.Show("Alamat harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAlamat.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPekerjaan.Text))
            {
                MessageBox.Show("Pekerjaan harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPekerjaan.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(cmbStatusPerkawinan.Text))
            {
                MessageBox.Show("Status Perkawinan harus dipilih.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbStatusPerkawinan.Focus();
                return;
            }

            // Ambil data dari input form
            string nik = txtNIK.Text.Trim();
            string namaLengkap = txtNamaLengkap.Text.Trim();
            DateTime tanggalLahir = dtpTanggalLahir.Value;
            string jenisKelamin = cmbJenisKelamin.Text;
            string alamat = txtAlamat.Text.Trim();
            string pekerjaan = txtPekerjaan.Text.Trim();
            string statusPerkawinan = cmbStatusPerkawinan.Text;

            // Panggil metode SaveWarga dari DatabaseManager
            bool success = dbManager.SaveWarga(nik, namaLengkap, tanggalLahir, jenisKelamin, alamat, pekerjaan, statusPerkawinan);
            if (success)
            {
                MessageBox.Show("Data warga berhasil disimpan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataToGrid(); // Muat ulang data ke grid setelah penyimpanan
                ResetForm(); // Bersihkan form
            }
        }

        // Metode untuk membersihkan semua input form 
        private void ResetForm()
        {
            txtNIK.Text = string.Empty;
            txtNamaLengkap.Text = string.Empty;
            dtpTanggalLahir.Value = DateTime.Now; // Mengatur ke tanggal saat ini 
            if (cmbJenisKelamin.Items.Count > 0) cmbJenisKelamin.SelectedIndex = 0;
            if (cmbStatusPerkawinan.Items.Count > 0) cmbStatusPerkawinan.SelectedIndex = 0; txtAlamat.Text = string.Empty;
            txtPekerjaan.Text = string.Empty;
            selectedNIK = string.Empty; // Reset NIK yang sedang diedit 
            txtNIK.ReadOnly = false; // NIK bisa diubah lagi untuk input baru 
            btnSimpan.Enabled = true; // Tombol simpan selalu aktif (untuk tambah/update) 
            btnHapus.Enabled = false; // Nonaktifkan hapus 
            btnUbah.Enabled = false; // Nonaktifkan ubah 
            dgvWarga.ClearSelection(); // Hilangkan seleksi di grid 
        }



        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void dgvWarga_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Pastikan baris yang diklik valid (bukan header kolom) 
            {
                DataGridViewRow row = dgvWarga.Rows[e.RowIndex];
                // Ambil NIK dari baris yang dipilih 
                selectedNIK = row.Cells["NIK"].Value.ToString();
                // Isi form dengan data dari baris yang dipilih 
                txtNIK.Text = selectedNIK;
                txtNamaLengkap.Text = row.Cells["NamaLengkap"].Value.ToString();
                // Konversi string tanggal ke DateTime untuk DateTimePicker 
                DateTime tglLahir;
                if (DateTime.TryParse(row.Cells["TanggalLahir"].Value.ToString(), out tglLahir))
                {
                    dtpTanggalLahir.Value = tglLahir;
                }
                else
                {
                    dtpTanggalLahir.Value = DateTime.Now; // Default jika gagal konversi 
                }
                cmbJenisKelamin.Text = row.Cells["JenisKelamin"].Value.ToString();
                txtAlamat.Text = row.Cells["Alamat"].Value.ToString();
                txtPekerjaan.Text = row.Cells["Pekerjaan"].Value.ToString();
                cmbStatusPerkawinan.Text = row.Cells["StatusPerkawinan"].Value.ToString();
                // Setelah data terpilih, NIK tidak boleh diubah langsung di textbox 
                // Agar NIK menjadi kunci unik untuk update/delete



                txtNIK.ReadOnly = true;
                // Aktifkan tombol Hapus dan Ubah 
                btnHapus.Enabled = true;
                btnUbah.Enabled = true;
                // Tombol simpan tetap aktif karena bisa berfungsi sebagai "Update" 
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedNIK))
            {
                MessageBox.Show("Pilih data warga yang ingin dihapus terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Konfirmasi penghapusan 
            DialogResult dialogResult = MessageBox.Show($"Anda yakin ingin menghapus data warga dengan NIK: {selectedNIK}?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                bool success = dbManager.DeleteWarga(selectedNIK);
                if (success)
                {
                    MessageBox.Show("Data warga berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataToGrid(); // Muat ulang data ke grid 
                    ResetForm(); // Bersihkan form 
                }
            }
        }
        private void btnUbah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedNIK))
            {
                MessageBox.Show("Pilih data warga yang ingin diubah terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Set NIK menjadi readonly
            // btnSimpan sudah aktif
        } 

    }
}
// Semua event handler sudah ada dan terhubung di Designer.cs
// Tidak perlu perubahan logika di file ini.