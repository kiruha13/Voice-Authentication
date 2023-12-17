using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class AudioVect
    {
        public int id { get; set; }
        public string Login { get; set; }
        public byte[] VectorData { get; set; }
        public byte[] VectorData1 { get; set; }
        public byte[] VectorData2 { get; set; }
        public byte[] ImageSpectr1 { get; set; }
        public byte[] ImageSpectr2 { get; set; }
        public byte[] ImageSpectr3 { get; set; }

    }
    public class ApplicationContext : DbContext
    {

        public DbSet<AudioVect> AudioVect { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server = KIRUHAKOR13; Database=VoiceAuthentication; Trusted_Connection=True; TrustServerCertificate=True;");

        }
        // Проверяем существование пользователя по логину
        public bool CheckIfUserExists(string login)
        {
            var user = AudioVect.FirstOrDefault(u => u.Login == login);
            return user != null;
        }
    }
}