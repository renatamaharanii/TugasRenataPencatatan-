using System;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data;

namespace csharprenata2
{
    public class DatabaseManager
    {
        private readonly string dbPath; // Path lengkap ke file database

        public DatabaseManager()
        {
            // Dapatkan direktori aplikasi yang sedang berjalan
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Gabungkan dengan folder Data
            string dataFolder = Path.Combine(appDirectory, "Data");
            // Pastikan folder Data ada
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            // Path lengkap ke file database (di dalam folder Data)
            dbPath = Path.Combine(dataFolder, "warga.db");
            InitializeDatabase(); // Panggil metode untuk inisialisasi database
        }

        // Metode untuk mendapatkan string koneksi database
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={dbPath};Version=3;");
        }

        // Metode untuk menginisialisasi database (membuat file dan tabel jika belum ada)
        private void InitializeDatabase()
        {
            // Jika file database belum ada, buat yang baru
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            using (SQLiteConnection conn = GetConnection())
            {
                try
                {
                    conn.Open(); // Buka koneksi ke database
                                 // SQL query untuk membuat tabel Warga
                    string createTableQuery = @"
CREATE TABLE IF NOT EXISTS Warga (
	NIK TEXT PRIMARY KEY UNIQUE NOT NULL,
	NamaLengkap TEXT NOT NULL,
	TanggalLahir TEXT,
	JenisKelamin TEXT NOT NULL,
	Alamat TEXT,
	Pekerjaan TEXT,
	StatusPerkawinan TEXT
);";
                    using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        cmd.ExecuteNonQuery(); // Jalankan query untuk membuat tabel
                    }
                }
                catch (Exception ex)
                {
                    // Tangani error jika terjadi masalah saat membuat database/tabel
                    MessageBox.Show($"Error saat inisialisasi database: {ex.Message}", "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        // --- METODE CRUD BARU DIMULAI DI SINI --- 
// CREATE / UPDATE: Menyimpan atau memperbarui data warga 
public bool SaveWarga(string nik, string namaLengkap, DateTime tanggalLahir, string jenisKelamin, string alamat, string pekerjaan, string statusPerkawinan) 
{ 
using (SQLiteConnection conn = GetConnection()) 
{ 
try 
{ 
conn.Open(); 
// Gunakan UPSERT (INSERT OR REPLACE) jika NIK sudah ada 
// Jika NIK belum ada, maka akan insert baru 
// Jika NIK sudah ada, maka akan update data yang sudah ada 
string query = @" 
INSERT OR REPLACE INTO Warga (NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan) 
VALUES (@nik, @namaLengkap, @tanggalLahir, @jenisKelamin, @alamat, @pekerjaan, @statusPerkawinan);";
using (SQLiteCommand cmd = new SQLiteCommand(query, conn)) 
{ 
cmd.Parameters.AddWithValue("@nik", nik); 
cmd.Parameters.AddWithValue("@namaLengkap", namaLengkap); 
cmd.Parameters.AddWithValue("@tanggalLahir", tanggalLahir.ToString("yyyy-MM-dd")); // Simpan sebagai string YYYY-MM-DD 
cmd.Parameters.AddWithValue("@jenisKelamin", jenisKelamin); 
cmd.Parameters.AddWithValue("@alamat", alamat); 
cmd.Parameters.AddWithValue("@pekerjaan", pekerjaan); 
cmd.Parameters.AddWithValue("@statusPerkawinan", statusPerkawinan); 
cmd.ExecuteNonQuery(); 
return true; 
} 
} 
catch (Exception ex) 
{ 
System.Windows.Forms.MessageBox.Show($"Error saat menyimpan data warga: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error); 
return false; 
} 
} 
} 
// READ: Mengambil semua data warga 
public DataTable GetAllWarga() 
{ 
DataTable dt = new DataTable(); 
using (SQLiteConnection conn = GetConnection()) 
{ 
try 
{ 
conn.Open(); 
string query = "SELECT NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan FROM Warga ORDER BY NamaLengkap ASC;"; 
using (SQLiteCommand cmd = new SQLiteCommand(query, conn)) 
{ 
using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd)) 
{ 
adapter.Fill(dt); // Mengisi DataTable dengan hasil query 
} 
} 
} 
catch (Exception ex) 
{ 
System.Windows.Forms.MessageBox.Show($"Error saat mengambil data warga: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error); 
} 
} 
return dt; 
} 
// DELETE: Menghapus data warga berdasarkan NIK
public bool DeleteWarga(string nik) 
{ 
using (SQLiteConnection conn = GetConnection()) 
{ 
try 
{ 
conn.Open(); 
string query = "DELETE FROM Warga WHERE NIK = @nik;"; 
using (SQLiteCommand cmd = new SQLiteCommand(query, conn)) 
{ 
cmd.Parameters.AddWithValue("@nik", nik); 
int rowsAffected = cmd.ExecuteNonQuery(); 
return rowsAffected > 0; // Mengembalikan true jika ada baris yang terpengaruh (data terhapus) 
} 
} 
catch (Exception ex) 
{ 
System.Windows.Forms.MessageBox.Show($"Error saat menghapus data warga: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error); 
return false; 
} 
} 
} 
// READ (single): Mengambil data warga berdasarkan NIK (untuk pengeditan) 
public DataRow GetWargaByNIK(string nik) 
{ 
DataTable dt = new DataTable(); 
using (SQLiteConnection conn = GetConnection()) 
{ 
try 
{ 
conn.Open(); 
string query = "SELECT NIK, NamaLengkap, TanggalLahir, JenisKelamin, Alamat, Pekerjaan, StatusPerkawinan FROM Warga WHERE NIK = @nik;"; 
using (SQLiteCommand cmd = new SQLiteCommand(query, conn)) 
{ 
cmd.Parameters.AddWithValue("@nik", nik); 
using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd)) 
{ 
adapter.Fill(dt); 
} 
} 
} 
catch (Exception ex) 
{ 
System.Windows.Forms.MessageBox.Show($"Error saat mengambil data warga berdasarkan NIK: {ex.Message}", "Error Database", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error); 
} 
} 
return dt.Rows.Count > 0 ? dt.Rows[0] : null; // Mengembalikan baris pertama jika ditemukan, selain itu null
} 
    }
}

// CRUD sudah menyimpan ke folder Data sesuai permintaan.