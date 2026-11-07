using IPriziv.Server.Data.Enumerations;
using IPriziv.Server.Data.Models;
using IPriziv.Server.Data.Models.ConstantDictionaries;
using IPriziv.Server.Data.Models.Dictionaries;
using IPriziv.Server.Engine.Utils;
using Microsoft.EntityFrameworkCore;

namespace IPriziv.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ExternalTeam> ExternalTeams { get; set; }
        public DbSet<ExternalSubTeam> ExternalSubTeams { get; set; }
        public DbSet<GatheredSubTeam> GatheredSubTeams { get; set; }
        public DbSet<MilitaryUnit> MilitaryUnits { get; set; }
        public DbSet<MilitarySubject> MilitarySubjects { get; set; }
        public DbSet<ReportedSubTeam> ReportedSubTeams { get; set; }
        public DbSet<ControlList> ControlLists { get; set; }
        public DbSet<ControlPerson> ControlPersons { get; set; }
        public DbSet<ControlPersonLog> ControlStatusLogs { get; set; }
        public DbSet<ControlTelegram> ControlTelegrams { get; set; }
        public DbSet<ControlTelegramPerson> ControlTelegramPeople { get; set; }
        public DbSet<ControlRecruit> ControlRecruits { get; set; }
        public DbSet<Recruit> Recruits { get; set; }
        public DbSet<GatheredTeamManager> GatheredTeamManagers { get; set; }
        public DbSet<TeamManagerEscort> TeamManagerEscorts { get; set; }
        public DbSet<TeamLog> TeamLogs { get; set; }
        public DbSet<HidingTeam> HidingTeams { get; set; }
        public DbSet<Overnight> Overnights { get; set; }
        public DbSet<Overnighter> Overnighters { get; set; }
        public DbSet<TeamDispatching> TeamDispatchings { get; set; }
        public DbSet<RecruitStatusLog> RecruitStatusLogs { get; set; }
        public DbSet<Biometric> Biometrics { get; set; }
        public DbSet<DocumentPackage> DocumentPackages { get; set; }
        public DbSet<DispatchingSendPlan> DispatchingSendPlans { get; set; }


        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<EducationSpeciality> EducationSpecialities { get; set; }
        public DbSet<FamilyStatus> FamilyStatuses { get; set; }
        public DbSet<FederalDistrict> FederalDistricts { get; set; }
        public DbSet<SportType> SportTypes { get; set; }
        public DbSet<TransportSourcePoint> TransportSourcePoints { get; set; }

        //ic DbSet<Pass> Pass { get; set; }
        //blic DbSet<PassState> PassState { get; set; }
        //blic DbSet<PassLogs> PassLogs { get; set; }
        public DbSet<Admiss> Admisses { get; set; }
        public DbSet<CityDistrict> CityDistricts { get; set; }
        public DbSet<CityMunicipality> CityMunicipalities { get; set; }
        public DbSet<CivilSpeciality> CivilSpecialities { get; set; }
        public DbSet<ControlListType> ControlListTypes { get; set; }
        public DbSet<EducationLevel> EducationLevels { get; set; }
        public DbSet<ExternalTeamRegime> ExternalTeamRegimes { get; set; }
        public DbSet<HealthCategory> HealthCategories { get; set; }
        public DbSet<MilitaryDebtSpeciality> MilitaryDebtSpecialities { get; set; }
        public DbSet<MilitaryDistrict> MilitaryDistricts { get; set; }
        public DbSet<MilitaryOffice> MilitaryOffices { get; set; }
        public DbSet<MilitaryRank> MilitaryRanks { get; set; }
        public DbSet<MilitaryType> MilitaryTypes { get; set; }
        public DbSet<MilitaryUnitAlternativeName> MilitaryUnitAlternativeNames { get; set; }
        public DbSet<Railroad> Railroads { get; set; }
        public DbSet<RecruitRegime> RecruitRegimes { get; set; }
        public DbSet<RecruitLog> RecruitLogs { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<UniformColor> UniformColors { get; set; }
        public DbSet<Holiday> Holidays { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        /// <summary>
        /// Безопасно дополняет схему T_People (только ADD NULL-колонок, без удаления данных).
        /// </summary>
        public async Task EnsureExtendedPeopleSchemaAsync()
        {
            await Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('T_People', 'disease_articles') IS NULL
                    ALTER TABLE [T_People] ADD [disease_articles] nvarchar(250) NULL;
                IF COL_LENGTH('T_People', 'team_pin_status') IS NULL
                    ALTER TABLE [T_People] ADD [team_pin_status] int NOT NULL CONSTRAINT DF_T_People_team_pin_status DEFAULT 0;
                IF COL_LENGTH('T_People', 'npu') IS NULL
                    ALTER TABLE [T_People] ADD [npu] tinyint NULL;
                """);
        }

        /// <summary>
        /// Безопасно дополняет схему T_Control (только ADD NULL-колонок, без удаления данных).
        /// </summary>
        public async Task EnsureExtendedControlListSchemaAsync()
        {
            await Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('T_Control', 'import_count_delta') IS NULL
                    ALTER TABLE [T_Control] ADD [import_count_delta] int NULL;
                """);
        }

        /// <summary>
        /// Безопасно дополняет схему T_ComMVO (только ADD NULL-колонок, без удаления данных).
        /// </summary>
        public async Task EnsureExtendedExternalTeamSchemaAsync()
        {
            await Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('T_ComMVO', 'OrderChangeNotes') IS NULL
                    ALTER TABLE [T_ComMVO] ADD [OrderChangeNotes] nvarchar(max) NULL;
                """);
        }

        private static bool? _barcodeExists;
        private static bool CheckBarcodeExistsInDb(string connectionString)
        {
            // Используем кэширование, чтобы не проверять БД при каждом создании контекста
            if (_barcodeExists.HasValue)
                return _barcodeExists.Value;

            try
            {
                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        // Проверяем наличие колонки в системных таблицах MSSQL
                        command.CommandText = "SELECT COL_LENGTH('T_People', 'barcode')";
                        var result = command.ExecuteScalar();
                        _barcodeExists = (result != DBNull.Value && result != null);
                    }
                }
            }
            catch
            {
                // Если база еще не создана или нет доступа, считаем что колонки нет
                _barcodeExists = false;
            }

            return _barcodeExists.Value;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EducationSpeciality>().ToSqlQuery("SELECT [s].[p00], [s].[p01] FROM [S_EducationSpec] AS [s] WHERE [s].[p00] IS NOT NULL");
            modelBuilder.Entity<CityMunicipality>(e =>
            {
                e.HasOne(x => x.MilitaryOffice).WithMany().OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<MilitaryOffice>(e =>
            {
                e.HasOne(x => x.CityDistrict).WithMany().OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<MilitaryUnit>(e =>
            {
                e.HasOne(x => x.MilitaryDistrict).WithMany().OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.Station).WithMany().OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<MilitaryUnitAlternativeName>(e =>
            {
                e.HasOne(x => x.MilitaryUnit).WithMany(x => x.AlternativeNames).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Station>(e =>
            {
                e.HasOne(x => x.FederalDistrict).WithMany().OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.TransportSourcePoint).WithMany().OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.Railroad).WithMany().OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.ConnectedStation).WithMany().OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<ControlList>(e =>
            {
                e.HasOne(x => x.MilitaryUnit).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryType).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.ControlListType).WithMany().OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ControlListType>(e =>
            {
                e.Property(x => x.Id).ValueGeneratedNever();
            });
            modelBuilder.Entity<ControlPerson>(e =>
            {
                e.Property(x => x.IsExpected).HasDefaultValue(true);

                e.HasMany(x => x.Telegrams).WithMany(x => x.Persons).UsingEntity<ControlTelegramPerson>(e =>
                {
                    e.HasOne(x => x.ControlPerson).WithMany().OnDelete(DeleteBehavior.SetNull);
                    e.HasOne(x => x.ControlTelegram).WithMany(x => x.TelegramPersons).OnDelete(DeleteBehavior.Cascade);
                });
            });
            modelBuilder.Entity<ControlPersonLog>(e =>
            {
                e.HasOne(x => x.ControlPerson).WithMany(x => x.ControlStatusLogs).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ControlRecruit>(e =>
            {
                e.Property(x => x.BirthDate).IsRequired(false).HasColumnType("smalldatetime");

                e.HasOne(x => x.MilitaryOffice).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.ControlList).WithMany(x => x.ControlRecruits).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.ControlPerson).WithMany(x => x.ControlRecruits).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ControlTelegram>(e =>
            {
                e.HasOne(x => x.MilitaryOffice).WithMany().OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<DocumentPackage>(e =>
            {
                e.HasIndex(x => new { x.IsDefenceMinistry, x.Number });
                e.HasOne(x => x.ExternalTeam).WithMany(x => x.DocumentPackages).OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.TeamDispatching).WithOne(x => x.DocumentPackage).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ExternalSubTeam>(e =>
            {
                e.HasOne(x => x.ExternalTeam).WithMany(x => x.SubTeams).HasForeignKey(x => x.ExternalTeamId).OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Regime).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryDebtSpeciality).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Admiss).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.HealthCategory).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.UniformColor).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.CivilSpeciality).WithMany().OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ExternalTeam>(e =>
            {
                e.HasOne(x => x.MilitaryUnit).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryType).WithMany().OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<GatheredSubTeam>(e =>
            {
                e.HasOne(x => x.ExternalTeam).WithMany(x => x.GatheredSubTeams).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.UniformColor).WithMany().OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<GatheredTeamManager>(e =>
            {
                e.HasOne(x => x.MilitaryRank).WithMany().OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<HidingTeam>(e =>
            {
                e.HasOne(x => x.ExternalSubTeam).WithMany().OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.MainTeam).WithMany(x => x.HidedTeams).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Overnight>(e =>
            {
                e.HasOne(x => x.GatheredSubTeam).WithMany(x => x.Overnights).OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.ExternalTeam).WithMany().OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Overnighter>(e =>
            {
                e.HasOne(x => x.Recruit).WithMany().OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.Overnight).WithMany(x => x.Overnighters).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.MilitaryOffice).WithMany().OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Recruit>(e =>
            {
                e.HasOne(x => x.GatheredSubTeam).WithMany(x => x.Recruits).OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.CityMunicipality).WithMany(x => x.Recruits).HasForeignKey(x => x.CityMunicipalityId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryOffice).WithMany(x => x.Recruits).HasForeignKey(x => x.MilitaryOfficeId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Doctor_0).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Doctor_1).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryDebtSpeciality).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.FamilyStatus).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.HealthCategory).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.EducationLevel).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Admiss).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryDebtSpeciality).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TeamMilitaryDebtSpeciality).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.EducationSpeciality).WithMany().OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Biometric).WithOne(x => x.Recruit).OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.UpdatedAt).HasDefaultValueSql("getDate()");
                e.Property(x => x.CreatedAt).HasDefaultValueSql("getDate()");
            });
            modelBuilder.Entity<RecruitLog>(e =>
            {
                e.Property(x => x.IsAccidentCorrection).HasDefaultValue(false);
                e.Property(x => x.TargetState).HasDefaultValue(RecruitState.Undefined);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("getDate()");

                e.HasOne(x => x.Recruit).WithMany(x => x.RecruitLogs).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<RecruitStatusLog>(e =>
            {
                e.HasOne(x => x.Recruit).WithMany(x => x.RecruitStatusLogs).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.GatheredSubTeam).WithMany(x => x.RecruitStatusLogs).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Dispatching).WithMany().OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<ReportedSubTeam>(e =>
            {
                e.HasOne(x => x.ExternalTeam).WithMany(x => x.ReportedTeams).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.ExternalSubTeam).WithMany(x => x.ReportedSubTeams).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryUnit).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.MilitaryType).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.Station).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.TeamDispatching).WithMany().OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<SendPlanByMilitaryOffice>().HasOne(x => x.MilitaryOffice).WithMany().OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TeamDispatching>(e =>
            {
                e.HasOne(x => x.GatheredTeamManager).WithMany(x => x.TeamDispatchings).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.GatheredSubTeam).WithOne(x => x.TeamDispatching).HasForeignKey<TeamDispatching>(x => x.GatheredSubTeamId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<TeamManagerEscort>(e =>
            {
                e.HasOne(x => x.MilitaryRank).WithMany().OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.TeamManager).WithMany(x => x.ManagerEscorts).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DispatchingSendPlan>(e =>
            {
                e.HasOne(x => x.MilitaryDebtSpeciality).WithMany().OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.GatheredSubTeam).WithMany(x => x.DispatchingSendPlans).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.HasDbFunction(typeof(AppDbFunctions).GetMethod(nameof(AppDbFunctions.StringDistance), new[] { typeof(string), typeof(string) })).HasName("JaroWinklerStringDistance");
            modelBuilder.HasDbFunction(typeof(AppDbFunctions).GetMethod(nameof(AppDbFunctions.GetExternalTeamFormatedName), new[] { typeof(string) })).HasName("GetSortValue");

            //var connectionString = Database.GetDbConnection().ConnectionString;
            //if (CheckBarcodeExistsInDb(connectionString))
            //    modelBuilder.Entity<Recruit>().Property<string>("barcode");
            //else 
            //    modelBuilder.Entity<Recruit>().Ignore("barcode");

            base.OnModelCreating(modelBuilder);
        }
    }
}
