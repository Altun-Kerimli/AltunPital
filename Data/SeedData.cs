using AltunPital.Security;

namespace AltunPital.Data;

public static class SeedData
{
    private record ServiceSeed(
        string Name,
        string Description,
        int Duration,
        decimal BasePrice);

    private record DoctorSeed(
        string FirstName,
        string LastName,
        string Email,
        string Department,
        string Specialization,
        string Service,
        TimeOnly StartTime,
        TimeOnly EndTime);

    private record StaffSeed(
        string FirstName,
        string LastName,
        string Email,
        string Department,
        string Position);

    private record PatientSeed(
        string FirstName,
        string LastName,
        string Email);

    private record EquipmentAssignmentSeed(
        string Equipment,
        int Quantity);

    private record CitySeed(
        string City,
        string District,
        string[] Departments,
        string[] Services,
        DoctorSeed[] Doctors,
        StaffSeed[] Staff,
        PatientSeed[] Patients,
        EquipmentAssignmentSeed[] Equipment);


    public static void Initialize(
        AppDbContext db)
    {
        // =====================================================
        // HASTANE
        // =====================================================

        var hospital =
            db.Hospitals
                .FirstOrDefault(h =>
                    h.Name == "AltunPital");

        if (hospital == null)
        {
            hospital =
                new Hospital
                {
                    Name =
                        "AltunPital",

                    Description =
                        "AltunPital Sağlık Grubu"
                };

            db.Hospitals.Add(
                hospital);

            db.SaveChanges();
        }


        // =====================================================
        // GLOBAL HİZMET KATALOĞU
        // =====================================================

        ServiceSeed[] serviceCatalog =
        [
            new(
                "Genel Muayene",
                "Genel hekim muayenesi",
                30,
                850),

            new(
                "Kardiyoloji Muayenesi",
                "Kalp ve damar hastalıkları muayenesi",
                30,
                1450),

            new(
                "Ortopedi Muayenesi",
                "Kas ve iskelet sistemi muayenesi",
                30,
                1300),

            new(
                "Çocuk Sağlığı Muayenesi",
                "Çocuk sağlığı ve hastalıkları muayenesi",
                30,
                1200),

            new(
                "Dermatoloji Muayenesi",
                "Deri hastalıkları muayenesi",
                30,
                1250),

            new(
                "Nöroloji Muayenesi",
                "Sinir sistemi hastalıkları muayenesi",
                40,
                1550),

            new(
                "Dahiliye Muayenesi",
                "İç hastalıkları muayenesi",
                30,
                1150),

            new(
                "KBB Muayenesi",
                "Kulak burun boğaz muayenesi",
                30,
                1200),

            new(
                "Göz Muayenesi",
                "Göz sağlığı ve hastalıkları muayenesi",
                30,
                1350),

            new(
                "Kadın Hastalıkları Muayenesi",
                "Kadın hastalıkları ve doğum muayenesi",
                40,
                1500),

            new(
                "Fizik Tedavi Değerlendirmesi",
                "Fizik tedavi ve rehabilitasyon değerlendirmesi",
                45,
                1400),

            new(
                "MR",
                "Manyetik rezonans görüntüleme",
                45,
                2600),

            new(
                "Ultrason",
                "Ultrason görüntüleme işlemi",
                30,
                1500),

            new(
                "Laboratuvar",
                "Temel kan ve biyokimya testleri",
                20,
                650)
        ];


        foreach (var item
                 in serviceCatalog)
        {
            if (!db.HealthServices.Any(s =>
                    s.Name == item.Name))
            {
                db.HealthServices.Add(
                    new HealthService
                    {
                        Name =
                            item.Name,

                        Description =
                            item.Description,

                        DurationMinutes =
                            item.Duration
                    });
            }
        }

        db.SaveChanges();


        // =====================================================
        // GLOBAL EKİPMAN KATALOĞU
        // =====================================================

        string[] equipmentCatalog =
        [
            "MR Cihazı",
            "Dijital Röntgen Cihazı",
            "Ultrason Cihazı",
            "EKG Cihazı",
            "Defibrilatör",
            "Hasta Monitörü",
            "Laboratuvar Analizörü",
            "EEG Cihazı",
            "Endoskopi Sistemi",
            "Göz Muayene Ünitesi",
            "Fizik Tedavi Ünitesi",
            "KBB Muayene Ünitesi"
        ];


        foreach (string name
                 in equipmentCatalog)
        {
            if (!db.Equipment.Any(e =>
                    e.Name == name))
            {
                db.Equipment.Add(
                    new Equipment
                    {
                        Name =
                            name,

                        Description =
                            $"{name} için AltunPital tıbbi ekipman kaydı"
                    });
            }
        }

        db.SaveChanges();


        // =====================================================
        // 5 FARKLI ŞEHİR / ŞUBE
        // =====================================================

        CitySeed[] cities =
        [
            // =================================================
            // SAKARYA
            // =================================================

            new CitySeed(
                "Sakarya",
                "Serdivan",

                [
                    "Genel Tıp",
                    "Kardiyoloji",
                    "Çocuk Sağlığı",
                    "Radyoloji"
                ],

                [
                    "Genel Muayene",
                    "Kardiyoloji Muayenesi",
                    "Çocuk Sağlığı Muayenesi",
                    "MR",
                    "Ultrason",
                    "Laboratuvar"
                ],

                [
                    new(
                        "Mehmet",
                        "Kaya",
                        "mehmet.kaya@altunpital.com",
                        "Kardiyoloji",
                        "Kardiyoloji",
                        "Kardiyoloji Muayenesi",
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0)),

                    new(
                        "Selin",
                        "Yalçın",
                        "selin.yalcin@altunpital.com",
                        "Genel Tıp",
                        "Genel Pratisyen",
                        "Genel Muayene",
                        new TimeOnly(8, 30),
                        new TimeOnly(16, 30)),

                    new(
                        "Murat",
                        "Akın",
                        "murat.akin@altunpital.com",
                        "Çocuk Sağlığı",
                        "Çocuk Sağlığı ve Hastalıkları",
                        "Çocuk Sağlığı Muayenesi",
                        new TimeOnly(9, 30),
                        new TimeOnly(17, 30)),

                    new(
                        "Gökçe",
                        "Erdem",
                        "gokce.erdem@altunpital.com",
                        "Radyoloji",
                        "Radyoloji",
                        "MR",
                        new TimeOnly(8, 0),
                        new TimeOnly(16, 0)),

                    new(
                        "Cihan",
                        "Toprak",
                        "cihan.toprak@altunpital.com",
                        "Genel Tıp",
                        "Aile Hekimliği",
                        "Genel Muayene",
                        new TimeOnly(10, 0),
                        new TimeOnly(18, 0)),

                    new(
                        "Nihan",
                        "Keskin",
                        "nihan.keskin@altunpital.com",
                        "Radyoloji",
                        "Radyoloji",
                        "Ultrason",
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0))
                ],

                [
                    new(
                        "Ayşe",
                        "Koç",
                        "ayse.koc@altunpital.com",
                        "Kardiyoloji",
                        "Hemşire"),

                    new(
                        "Emre",
                        "Şahin",
                        "emre.sahin@altunpital.com",
                        "Radyoloji",
                        "Radyoloji Teknisyeni"),

                    new(
                        "Merve",
                        "Aksoy",
                        "merve.aksoy@altunpital.com",
                        "Çocuk Sağlığı",
                        "Hemşire"),

                    new(
                        "Can",
                        "Öztürk",
                        "can.ozturk@altunpital.com",
                        "Genel Tıp",
                        "Sağlık Teknikeri")
                ],

                [
                    new(
                        "Ali",
                        "Demir",
                        "ali.demir@test.com"),

                    new(
                        "Deniz",
                        "Yıldız",
                        "deniz.yildiz@test.com"),

                    new(
                        "Ece",
                        "Acar",
                        "ece.acar@test.com")
                ],

                [
                    new(
                        "MR Cihazı",
                        2),

                    new(
                        "Ultrason Cihazı",
                        2),

                    new(
                        "EKG Cihazı",
                        3),

                    new(
                        "Defibrilatör",
                        4),

                    new(
                        "Laboratuvar Analizörü",
                        2)
                ]
            ),


            // =================================================
            // İSTANBUL
            // =================================================

            new CitySeed(
                "İstanbul",
                "Kadıköy",

                [
                    "Nöroloji",
                    "Dermatoloji",
                    "KBB",
                    "Göz",
                    "Dahiliye"
                ],

                [
                    "Nöroloji Muayenesi",
                    "Dermatoloji Muayenesi",
                    "KBB Muayenesi",
                    "Göz Muayenesi",
                    "Dahiliye Muayenesi",
                    "Laboratuvar"
                ],

                [
                    new(
                        "Kerem",
                        "Arslan",
                        "kerem.arslan@altunpital.com",
                        "Nöroloji",
                        "Nöroloji",
                        "Nöroloji Muayenesi",
                        new TimeOnly(9, 0),
                        new TimeOnly(18, 0)),

                    new(
                        "Derya",
                        "Tunç",
                        "derya.tunc@altunpital.com",
                        "Dermatoloji",
                        "Dermatoloji",
                        "Dermatoloji Muayenesi",
                        new TimeOnly(10, 0),
                        new TimeOnly(18, 0)),

                    new(
                        "Onur",
                        "Korkmaz",
                        "onur.korkmaz@altunpital.com",
                        "KBB",
                        "Kulak Burun Boğaz",
                        "KBB Muayenesi",
                        new TimeOnly(8, 30),
                        new TimeOnly(16, 30)),

                    new(
                        "Buse",
                        "Çetin",
                        "buse.cetin@altunpital.com",
                        "Göz",
                        "Göz Hastalıkları",
                        "Göz Muayenesi",
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0)),

                    new(
                        "Tolga",
                        "Güneş",
                        "tolga.gunes@altunpital.com",
                        "Dahiliye",
                        "İç Hastalıkları",
                        "Dahiliye Muayenesi",
                        new TimeOnly(9, 30),
                        new TimeOnly(17, 30)),

                    new(
                        "Pelin",
                        "Karaca",
                        "pelin.karaca@altunpital.com",
                        "Nöroloji",
                        "Klinik Nörofizyoloji",
                        "Nöroloji Muayenesi",
                        new TimeOnly(11, 0),
                        new TimeOnly(19, 0))
                ],

                [
                    new(
                        "Seda",
                        "Balcı",
                        "seda.balci@altunpital.com",
                        "Nöroloji",
                        "Hemşire"),

                    new(
                        "Kaan",
                        "Ateş",
                        "kaan.ates@altunpital.com",
                        "Göz",
                        "Optometri Teknisyeni"),

                    new(
                        "İrem",
                        "Soylu",
                        "irem.soylu@altunpital.com",
                        "Dermatoloji",
                        "Sağlık Teknikeri"),

                    new(
                        "Barış",
                        "Ergin",
                        "baris.ergin@altunpital.com",
                        "KBB",
                        "KBB Teknikeri")
                ],

                [
                    new(
                        "Naz",
                        "Özkan",
                        "naz.ozkan@test.com"),

                    new(
                        "Serkan",
                        "Bulut",
                        "serkan.bulut@test.com"),

                    new(
                        "Melis",
                        "Avcı",
                        "melis.avci@test.com")
                ],

                [
                    new(
                        "EEG Cihazı",
                        3),

                    new(
                        "Göz Muayene Ünitesi",
                        4),

                    new(
                        "KBB Muayene Ünitesi",
                        3),

                    new(
                        "Hasta Monitörü",
                        6),

                    new(
                        "Laboratuvar Analizörü",
                        3)
                ]
            ),


            // =================================================
            // KOCAELİ
            // =================================================

            new CitySeed(
                "Kocaeli",
                "İzmit",

                [
                    "Ortopedi",
                    "Kardiyoloji",
                    "Genel Tıp",
                    "Radyoloji",
                    "Çocuk Sağlığı"
                ],

                [
                    "Ortopedi Muayenesi",
                    "Kardiyoloji Muayenesi",
                    "Genel Muayene",
                    "Çocuk Sağlığı Muayenesi",
                    "MR",
                    "Laboratuvar"
                ],

                [
                    new(
                        "Hakan",
                        "Duman",
                        "hakan.duman@altunpital.com",
                        "Ortopedi",
                        "Ortopedi ve Travmatoloji",
                        "Ortopedi Muayenesi",
                        new TimeOnly(8, 0),
                        new TimeOnly(16, 0)),

                    new(
                        "Aslı",
                        "Özer",
                        "asli.ozer@altunpital.com",
                        "Kardiyoloji",
                        "Kardiyoloji",
                        "Kardiyoloji Muayenesi",
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0)),

                    new(
                        "Umut",
                        "Turan",
                        "umut.turan@altunpital.com",
                        "Genel Tıp",
                        "Genel Pratisyen",
                        "Genel Muayene",
                        new TimeOnly(8, 30),
                        new TimeOnly(16, 30)),

                    new(
                        "Ceren",
                        "Polat",
                        "ceren.polat@altunpital.com",
                        "Radyoloji",
                        "Radyoloji",
                        "MR",
                        new TimeOnly(10, 0),
                        new TimeOnly(18, 0)),

                    new(
                        "Furkan",
                        "Eren",
                        "furkan.eren@altunpital.com",
                        "Çocuk Sağlığı",
                        "Çocuk Sağlığı ve Hastalıkları",
                        "Çocuk Sağlığı Muayenesi",
                        new TimeOnly(9, 30),
                        new TimeOnly(17, 30)),

                    new(
                        "Sibel",
                        "Kurt",
                        "sibel.kurt@altunpital.com",
                        "Ortopedi",
                        "Spor Hekimliği",
                        "Ortopedi Muayenesi",
                        new TimeOnly(11, 0),
                        new TimeOnly(19, 0))
                ],

                [
                    new(
                        "Gamze",
                        "Doğan",
                        "gamze.dogan@altunpital.com",
                        "Ortopedi",
                        "Hemşire"),

                    new(
                        "Oğuz",
                        "Çakır",
                        "oguz.cakir@altunpital.com",
                        "Kardiyoloji",
                        "EKG Teknisyeni"),

                    new(
                        "Hilal",
                        "Yaman",
                        "hilal.yaman@altunpital.com",
                        "Çocuk Sağlığı",
                        "Hemşire"),

                    new(
                        "Berk",
                        "Kılıç",
                        "berk.kilic@altunpital.com",
                        "Radyoloji",
                        "Radyoloji Teknisyeni")
                ],

                [
                    new(
                        "Burcu",
                        "Sezer",
                        "burcu.sezer@test.com"),

                    new(
                        "Okan",
                        "Işık",
                        "okan.isik@test.com"),

                    new(
                        "Leyla",
                        "Kaplan",
                        "leyla.kaplan@test.com")
                ],

                [
                    new(
                        "Dijital Röntgen Cihazı",
                        3),

                    new(
                        "MR Cihazı",
                        1),

                    new(
                        "EKG Cihazı",
                        2),

                    new(
                        "Defibrilatör",
                        3),

                    new(
                        "Hasta Monitörü",
                        5)
                ]
            ),


            // =================================================
            // ANKARA
            // =================================================

            new CitySeed(
                "Ankara",
                "Çankaya",

                [
                    "Dahiliye",
                    "Nöroloji",
                    "Kadın Hastalıkları",
                    "Göz",
                    "Kardiyoloji"
                ],

                [
                    "Dahiliye Muayenesi",
                    "Nöroloji Muayenesi",
                    "Kadın Hastalıkları Muayenesi",
                    "Göz Muayenesi",
                    "Kardiyoloji Muayenesi",
                    "Ultrason",
                    "Laboratuvar"
                ],

                [
                    new(
                        "Esra",
                        "Bayram",
                        "esra.bayram@altunpital.com",
                        "Dahiliye",
                        "İç Hastalıkları",
                        "Dahiliye Muayenesi",
                        new TimeOnly(8, 30),
                        new TimeOnly(16, 30)),

                    new(
                        "Volkan",
                        "Aksu",
                        "volkan.aksu@altunpital.com",
                        "Nöroloji",
                        "Nöroloji",
                        "Nöroloji Muayenesi",
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0)),

                    new(
                        "Şebnem",
                        "Güler",
                        "sebnem.guler@altunpital.com",
                        "Kadın Hastalıkları",
                        "Kadın Hastalıkları ve Doğum",
                        "Kadın Hastalıkları Muayenesi",
                        new TimeOnly(10, 0),
                        new TimeOnly(18, 0)),

                    new(
                        "Eren",
                        "Başar",
                        "eren.basar@altunpital.com",
                        "Göz",
                        "Göz Hastalıkları",
                        "Göz Muayenesi",
                        new TimeOnly(9, 30),
                        new TimeOnly(17, 30)),

                    new(
                        "Aylin",
                        "Uçar",
                        "aylin.ucar@altunpital.com",
                        "Kardiyoloji",
                        "Kardiyoloji",
                        "Kardiyoloji Muayenesi",
                        new TimeOnly(8, 0),
                        new TimeOnly(16, 0)),

                    new(
                        "Koray",
                        "Sönmez",
                        "koray.sonmez@altunpital.com",
                        "Dahiliye",
                        "Endokrinoloji",
                        "Dahiliye Muayenesi",
                        new TimeOnly(11, 0),
                        new TimeOnly(19, 0))
                ],

                [
                    new(
                        "Tuğba",
                        "Ekinci",
                        "tugba.ekinci@altunpital.com",
                        "Kadın Hastalıkları",
                        "Ebe"),

                    new(
                        "Sinan",
                        "Gök",
                        "sinan.gok@altunpital.com",
                        "Nöroloji",
                        "EEG Teknisyeni"),

                    new(
                        "Elvan",
                        "Dursun",
                        "elvan.dursun@altunpital.com",
                        "Göz",
                        "Sağlık Teknikeri"),

                    new(
                        "Yusuf",
                        "Tan",
                        "yusuf.tan@altunpital.com",
                        "Kardiyoloji",
                        "Hemşire")
                ],

                [
                    new(
                        "Gizem",
                        "Bozkurt",
                        "gizem.bozkurt@test.com"),

                    new(
                        "Mert",
                        "Ekin",
                        "mert.ekin@test.com"),

                    new(
                        "Zehra",
                        "Yüksel",
                        "zehra.yuksel@test.com")
                ],

                [
                    new(
                        "Ultrason Cihazı",
                        4),

                    new(
                        "EEG Cihazı",
                        2),

                    new(
                        "Göz Muayene Ünitesi",
                        3),

                    new(
                        "EKG Cihazı",
                        3),

                    new(
                        "Laboratuvar Analizörü",
                        4)
                ]
            ),


            // =================================================
            // BURSA
            // =================================================

            new CitySeed(
                "Bursa",
                "Nilüfer",

                [
                    "Ortopedi",
                    "Fizik Tedavi",
                    "Dermatoloji",
                    "Genel Tıp",
                    "KBB"
                ],

                [
                    "Ortopedi Muayenesi",
                    "Fizik Tedavi Değerlendirmesi",
                    "Dermatoloji Muayenesi",
                    "Genel Muayene",
                    "KBB Muayenesi",
                    "Laboratuvar"
                ],

                [
                    new(
                        "Alper",
                        "Köse",
                        "alper.kose@altunpital.com",
                        "Ortopedi",
                        "Ortopedi ve Travmatoloji",
                        "Ortopedi Muayenesi",
                        new TimeOnly(9, 0),
                        new TimeOnly(17, 0)),

                    new(
                        "İlayda",
                        "Ersoy",
                        "ilayda.ersoy@altunpital.com",
                        "Fizik Tedavi",
                        "Fizik Tedavi ve Rehabilitasyon",
                        "Fizik Tedavi Değerlendirmesi",
                        new TimeOnly(8, 30),
                        new TimeOnly(16, 30)),

                    new(
                        "Batuhan",
                        "Oral",
                        "batuhan.oral@altunpital.com",
                        "Dermatoloji",
                        "Dermatoloji",
                        "Dermatoloji Muayenesi",
                        new TimeOnly(10, 0),
                        new TimeOnly(18, 0)),

                    new(
                        "Defne",
                        "Tekin",
                        "defne.tekin@altunpital.com",
                        "Genel Tıp",
                        "Genel Pratisyen",
                        "Genel Muayene",
                        new TimeOnly(8, 0),
                        new TimeOnly(16, 0)),

                    new(
                        "Rıza",
                        "Gür",
                        "riza.gur@altunpital.com",
                        "KBB",
                        "Kulak Burun Boğaz",
                        "KBB Muayenesi",
                        new TimeOnly(9, 30),
                        new TimeOnly(17, 30)),

                    new(
                        "Lale",
                        "Önal",
                        "lale.onal@altunpital.com",
                        "Fizik Tedavi",
                        "Spor Rehabilitasyonu",
                        "Fizik Tedavi Değerlendirmesi",
                        new TimeOnly(11, 0),
                        new TimeOnly(19, 0))
                ],

                [
                    new(
                        "Cansu",
                        "Yüce",
                        "cansu.yuce@altunpital.com",
                        "Fizik Tedavi",
                        "Fizyoterapi Teknikeri"),

                    new(
                        "Recep",
                        "Kara",
                        "recep.kara@altunpital.com",
                        "Ortopedi",
                        "Hemşire"),

                    new(
                        "Damla",
                        "İnce",
                        "damla.ince@altunpital.com",
                        "Dermatoloji",
                        "Sağlık Teknikeri"),

                    new(
                        "Taylan",
                        "Özen",
                        "taylan.ozen@altunpital.com",
                        "KBB",
                        "KBB Teknikeri")
                ],

                [
                    new(
                        "Pınar",
                        "Çolakoğlu",
                        "pinar.colakoglu@test.com"),

                    new(
                        "Erhan",
                        "Mutlu",
                        "erhan.mutlu@test.com"),

                    new(
                        "Ada",
                        "Keleş",
                        "ada.keles@test.com")
                ],

                [
                    new(
                        "Fizik Tedavi Ünitesi",
                        5),

                    new(
                        "Dijital Röntgen Cihazı",
                        2),

                    new(
                        "KBB Muayene Ünitesi",
                        2),

                    new(
                        "Hasta Monitörü",
                        4),

                    new(
                        "Defibrilatör",
                        2)
                ]
            )
        ];


        // =====================================================
        // ŞEHİR VERİLERİNİ OLUŞTUR
        // =====================================================

        int cityIndex =
            0;


        foreach (CitySeed city
                 in cities)
        {
            cityIndex++;


            // -------------------------------------------------
            // ŞUBE
            // -------------------------------------------------

            string branchName =
                $"AltunPital {city.City}";


            var branch =
                db.Branches
                    .FirstOrDefault(b =>
                        b.Name ==
                        branchName);


            if (branch == null)
            {
                branch =
                    new Branch
                    {
                        HospitalId =
                            hospital.HospitalId,

                        Name =
                            branchName,

                        City =
                            city.City,

                        District =
                            city.District,

                        Address =
                            $"{city.District}, {city.City}",

                        Phone =
                            $"0{260 + cityIndex} 400 00 {cityIndex:00}",

                        OpeningTime =
                            new TimeOnly(
                                8,
                                0),

                        ClosingTime =
                            new TimeOnly(
                                19,
                                0)
                    };


                db.Branches.Add(
                    branch);

                db.SaveChanges();
            }


            // -------------------------------------------------
            // BÖLÜMLER
            // -------------------------------------------------

            foreach (string departmentName
                     in city.Departments)
            {
                if (!db.Departments.Any(d =>
                        d.BranchId ==
                            branch.BranchId &&
                        d.Name ==
                            departmentName))
                {
                    db.Departments.Add(
                        new Department
                        {
                            BranchId =
                                branch.BranchId,

                            Name =
                                departmentName,

                            Description =
                                $"{departmentName} bölümü"
                        });
                }
            }

            db.SaveChanges();


            // -------------------------------------------------
            // ŞUBE HİZMETLERİ
            // -------------------------------------------------

            foreach (string serviceName
                     in city.Services)
            {
                var catalogItem =
                    serviceCatalog.First(s =>
                        s.Name ==
                        serviceName);


                var service =
                    db.HealthServices
                        .First(s =>
                            s.Name ==
                            serviceName);


                bool exists =
                    db.BranchServices.Any(bs =>
                        bs.BranchId ==
                            branch.BranchId &&
                        bs.HealthServiceId ==
                            service.HealthServiceId);


                if (!exists)
                {
                    db.BranchServices.Add(
                        new BranchService
                        {
                            BranchId =
                                branch.BranchId,

                            HealthServiceId =
                                service.HealthServiceId,

                            Price =
                                catalogItem.BasePrice +
                                cityIndex * 75,

                            IsAvailable =
                                true
                        });
                }
            }

            db.SaveChanges();


            // -------------------------------------------------
            // EKİPMAN
            // -------------------------------------------------

            foreach (var assignment
                     in city.Equipment)
            {
                var equipment =
                    db.Equipment
                        .First(e =>
                            e.Name ==
                            assignment.Equipment);


                bool exists =
                    db.BranchEquipment.Any(be =>
                        be.BranchId ==
                            branch.BranchId &&
                        be.EquipmentId ==
                            equipment.EquipmentId);


                if (!exists)
                {
                    db.BranchEquipment.Add(
                        new BranchEquipment
                        {
                            BranchId =
                                branch.BranchId,

                            EquipmentId =
                                equipment.EquipmentId,

                            Quantity =
                                assignment.Quantity,

                            IsOperational =
                                true
                        });
                }
            }

            db.SaveChanges();


            // -------------------------------------------------
            // DOKTORLAR
            // -------------------------------------------------

            int doctorIndex =
                0;


            foreach (DoctorSeed doctorSeed
                     in city.Doctors)
            {
                doctorIndex++;


                var doctor =
                    db.Doctors
                        .FirstOrDefault(d =>
                            d.Email ==
                            doctorSeed.Email);


                if (doctor == null)
                {
                    var department =
                        db.Departments
                            .First(d =>
                                d.BranchId ==
                                    branch.BranchId &&
                                d.Name ==
                                    doctorSeed.Department);


                    doctor =
                        new Doctor
                        {
                            FirstName =
                                doctorSeed.FirstName,

                            LastName =
                                doctorSeed.LastName,

                            Email =
                                doctorSeed.Email,

                            Phone =
                                $"0555{cityIndex}{doctorIndex}1000",

                            PasswordHash =
                                PasswordHasher.HashPassword(
                                    "doktor123"),

                            EmployeeNumber =
                                $"DOC-{cityIndex:00}-{doctorIndex:00}",

                            HireDate =
                                DateTime.Today
                                    .AddYears(
                                        -(2 + doctorIndex)),

                            BranchId =
                                branch.BranchId,

                            DepartmentId =
                                department.DepartmentId,

                            Specialization =
                                doctorSeed.Specialization
                        };


                    db.Doctors.Add(
                        doctor);

                    db.SaveChanges();
                }


                var service =
                    db.HealthServices
                        .First(s =>
                            s.Name ==
                            doctorSeed.Service);


                if (!db.DoctorServices.Any(ds =>
                        ds.DoctorId ==
                            doctor.UserId &&
                        ds.HealthServiceId ==
                            service.HealthServiceId))
                {
                    db.DoctorServices.Add(
                        new DoctorService
                        {
                            DoctorId =
                                doctor.UserId,

                            HealthServiceId =
                                service.HealthServiceId
                        });
                }


                for (int day = 1;
                     day <= 5;
                     day++)
                {
                    DayOfWeek dayOfWeek =
                        (DayOfWeek)day;


                    if (!db.DoctorSchedules.Any(s =>
                            s.DoctorId ==
                                doctor.UserId &&
                            s.DayOfWeek ==
                                dayOfWeek))
                    {
                        db.DoctorSchedules.Add(
                            new DoctorSchedule
                            {
                                DoctorId =
                                    doctor.UserId,

                                DayOfWeek =
                                    dayOfWeek,

                                StartTime =
                                    doctorSeed.StartTime,

                                EndTime =
                                    doctorSeed.EndTime
                            });
                    }
                }


                db.SaveChanges();
            }


            // -------------------------------------------------
            // SAĞLIK PERSONELİ
            // -------------------------------------------------

            int staffIndex =
                0;


            foreach (StaffSeed staffSeed
                     in city.Staff)
            {
                staffIndex++;


                if (db.HealthStaff.Any(s =>
                        s.Email ==
                        staffSeed.Email))
                {
                    continue;
                }


                var department =
                    db.Departments
                        .First(d =>
                            d.BranchId ==
                                branch.BranchId &&
                            d.Name ==
                                staffSeed.Department);


                db.HealthStaff.Add(
                    new HealthStaff
                    {
                        FirstName =
                            staffSeed.FirstName,

                        LastName =
                            staffSeed.LastName,

                        Email =
                            staffSeed.Email,

                        Phone =
                            $"0532{cityIndex}{staffIndex}2000",

                        PasswordHash =
                            PasswordHasher.HashPassword(
                                "personel123"),

                        EmployeeNumber =
                            $"STAFF-{cityIndex:00}-{staffIndex:00}",

                        HireDate =
                            DateTime.Today
                                .AddYears(
                                    -(1 + staffIndex)),

                        BranchId =
                            branch.BranchId,

                        DepartmentId =
                            department.DepartmentId,

                        Position =
                            staffSeed.Position
                    });
            }

            db.SaveChanges();


            // -------------------------------------------------
            // HASTALAR
            // -------------------------------------------------

            int patientIndex =
                0;


            foreach (PatientSeed patientSeed
                     in city.Patients)
            {
                patientIndex++;


                if (db.Patients.Any(p =>
                        p.Email ==
                        patientSeed.Email))
                {
                    continue;
                }


                db.Patients.Add(
                    new Patient
                    {
                        FirstName =
                            patientSeed.FirstName,

                        LastName =
                            patientSeed.LastName,

                        Email =
                            patientSeed.Email,

                        Phone =
                            $"0542{cityIndex}{patientIndex}3000",

                        City =
                            city.City,

                        PasswordHash =
                            PasswordHasher.HashPassword(
                                "hasta123")
                    });
            }

            db.SaveChanges();


            // -------------------------------------------------
            // ÖRNEK RANDEVULAR
            // -------------------------------------------------

            var patients =
                city.Patients
                    .Select(p =>
                        db.Patients.First(x =>
                            x.Email ==
                            p.Email))
                    .ToList();


            var doctors =
                city.Doctors
                    .Take(3)
                    .Select(d =>
                        db.Doctors.First(x =>
                            x.Email ==
                            d.Email))
                    .ToList();


            AppointmentStatus[] statuses =
            [
                AppointmentStatus.Pending,
                AppointmentStatus.Approved,
                AppointmentStatus.Completed
            ];


            for (int i = 0;
                 i < 3;
                 i++)
            {
                Patient patient =
                    patients[i];

                Doctor doctor =
                    doctors[i];


                var doctorService =
                    db.DoctorServices
                        .First(ds =>
                            ds.DoctorId ==
                            doctor.UserId);


                var service =
                    db.HealthServices
                        .First(s =>
                            s.HealthServiceId ==
                            doctorService.HealthServiceId);


                DateTime start =
                    NextWeekday(
                        DateTime.Today
                            .AddDays(
                                2 + cityIndex + i),
                        (DayOfWeek)(
                            (int)DayOfWeek.Monday +
                            (i % 5)))
                    .AddHours(
                        10 + i);


                bool exists =
                    db.Appointments.Any(a =>
                        a.PatientId ==
                            patient.UserId &&
                        a.DoctorId ==
                            doctor.UserId &&
                        a.StartDateTime ==
                            start);


                if (exists)
                    continue;


                decimal price =
                    db.BranchServices
                        .Where(bs =>
                            bs.BranchId ==
                                branch.BranchId &&
                            bs.HealthServiceId ==
                                service.HealthServiceId)
                        .Select(bs =>
                            bs.Price)
                        .First();


                db.Appointments.Add(
                    new Appointment
                    {
                        PatientId =
                            patient.UserId,

                        DoctorId =
                            doctor.UserId,

                        BranchId =
                            branch.BranchId,

                        HealthServiceId =
                            service.HealthServiceId,

                        StartDateTime =
                            start,

                        EndDateTime =
                            start.AddMinutes(
                                service.DurationMinutes),

                        Price =
                            price,

                        Status =
                            statuses[i],

                        CreatedAt =
                            DateTime.Now
                                .AddDays(
                                    -(i + 1))
                    });
            }

            db.SaveChanges();
        }


        // =====================================================
        // YÖNETİCİ
        // =====================================================

        const string managerEmail =
            "manager@altunpital.com";


        if (!db.Managers.Any(m =>
                m.Email ==
                managerEmail))
        {
            var sakaryaBranch =
                db.Branches
                    .First(b =>
                        b.City ==
                        "Sakarya");


            db.Managers.Add(
                new Manager
                {
                    FirstName =
                        "Altun",

                    LastName =
                        "Pital",

                    Email =
                        managerEmail,

                    Phone =
                        "05000000000",

                    PasswordHash =
                        PasswordHasher.HashPassword(
                            "asdf1234!"),

                    EmployeeNumber =
                        "ALT-MNG-001",

                    HireDate =
                        DateTime.Today
                            .AddYears(-5),

                    BranchId =
                        sakaryaBranch.BranchId,

                    DepartmentId =
                        null
                });


            db.SaveChanges();
        }
    }


    private static DateTime NextWeekday(
        DateTime date,
        DayOfWeek targetDay)
    {
        int daysToAdd =
            (
                (int)targetDay -
                (int)date.DayOfWeek +
                7
            ) % 7;


        if (daysToAdd == 0)
        {
            daysToAdd =
                7;
        }


        return date.Date
            .AddDays(
                daysToAdd);
    }
}