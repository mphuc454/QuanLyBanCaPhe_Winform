using Microsoft.Data.Sqlite;

namespace QuanLyCaPhe.Utils;

public class DBConnect
{
    private static readonly string DbPath = ChuanBiDatabase();
    private static readonly string ConnectionString = $"Data Source={DbPath};";

    static DBConnect()
    {
        KiemTraVaCapNhatBang();
    }

    private static string ChuanBiDatabase()
    {
        string thuMuc = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuanLyCaPhe");
        Directory.CreateDirectory(thuMuc);

        string duongDan = Path.Combine(thuMuc, "QuanLyCaPhe.db");
        if (!File.Exists(duongDan))
        {
            using var stream = typeof(DBConnect).Assembly.GetManifestResourceStream("QuanLyCaPhe.db");
            if (stream != null)
            {
                using var file = File.Create(duongDan);
                stream.CopyTo(file);
            }
        }
        return duongDan;
    }

    private static void KiemTraVaCapNhatBang()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "PRAGMA table_info(MON_AN);";
            using var reader = cmd.ExecuteReader();
            bool coCotHinhAnh = false;
            while (reader.Read())
            {
                if (reader["name"].ToString() == "HinhAnh")
                {
                    coCotHinhAnh = true;
                    break;
                }
            }
            reader.Close();

            if (!coCotHinhAnh)
            {
                using var alterCmd = connection.CreateCommand();
                alterCmd.CommandText = "ALTER TABLE MON_AN ADD COLUMN HinhAnh BLOB;";
                alterCmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Đường dẫn DB: " + DbPath + "\n\n" + ex, "Lỗi khởi tạo database");
        }
    }

    public static SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var pragmaCmd = connection.CreateCommand();
        pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
        pragmaCmd.ExecuteNonQuery();

        return connection;
    }
}