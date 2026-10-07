using System;
using System.Collections.Generic;
using System.Linq;
using StavZooApp.Models;

namespace StavZooApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ZooDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if already seeded
            if (context.Animals.Any())
            {
                return;
            }

            // 1. Seed Users
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

            var users = new List<User>
            {
                new User
                {
                    Username = "keeper_alex",
                    Email = "keeper@stavzoo.ru",
                    FullName = "Александр Смирнов",
                    Role = "Employee",
                    Position = "Старший зоотехник секции копытных",
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow.AddMonths(-12)
                },
                new User
                {
                    Username = "vet_elena",
                    Email = "vet@stavzoo.ru",
                    FullName = "Елена Васильева",
                    Role = "Employee",
                    Position = "Главный ветеринарный врач",
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow.AddMonths(-10)
                },
                new User
                {
                    Username = "guest_anna",
                    Email = "guest@mail.ru",
                    FullName = "Анна Кузнецова",
                    Role = "User",
                    Position = "Посетитель зоопарка",
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // 2. Seed Animal: Alpaca (Пако и Лола)
            var alpaca = new Animal
            {
                Slug = "alpaca",
                Name = "Пако и Лола",
                Species = "Альпака (Уакая)",
                LatinName = "Vicugna pacos",
                Family = "Верблюдовые (Camelidae)",
                Origin = "Высокогорные плато Анд (Перу, Боливия, Чили)",
                EnclosureNumber = "Вольер № 14 (Южноамериканский сектор)",
                Status = "Здоровы, активны, период вынашивания потомства",
                DietSummary = "Сено высшего сорта (люцерна и тимофеевка), сочные корма (морковь, яблоки, тыква), специализированные гранулы с цинком и селеном, свежие ветки ивы.",
                Description = @"Пако и Лола — неразлучная пара очаровательных альпак породы Уакая (Huacaya) в Ставропольском зоопарке. 
Они прибыли к нам в рамках программы сохранения редких высокогорных видов. Пако — статный самец с густой шерстью цвета горячего шоколада и любознательным добрым нравом. 
Лола — грациозная белоснежная самочка с выразительными глазами и кротким характером.

Альпаки обладают удивительно мягкой шерстью, которая в семь раз теплее овечьей и не содержит ланолина, благодаря чему гипоаллергенна. 
Они очень дружелюбны, обожают общаться с посетителями через безопасное ограждение и с удовольствием позируют для фотографий.",
                CharacterTraits = "Пако очень любопытен, всегда первым подходит знакомиться и исследовать новые игрушки. Лола более осторожна, но обожает лакомства из моркови и ласковые поглаживания по шее. Вместе они создают удивительную атмосферу уюта и гармонии.",
                History = @"Пако родился 15 мая 2021 года, а Лола — 22 августа 2021 года в высокогорном питомнике. 
Весной 2023 года пара торжественно переехала в Ставропольский зоопарк в специально спроектированный открытый вольер с мягким травяным покрытием, деревянным отапливаемым домиком и навесом от солнца.
Осенью 2025 года после планового УЗИ ветеринары подтвердили успешную беременность Лолы, и теперь вся команда зоопарка с нетерпением ожидает пополнения семейства (малыша-крии) весной!",
                HeroImageUrl = "/images/paco.jpg",
                WebcamStreamUrl = "/images/alpaca-video.mp4",
                MonthlyDonationGoal = 45000,
                ArrivalDate = new DateTime(2023, 4, 12),
                BirthDate = new DateTime(2021, 5, 15)
            };

            context.Animals.Add(alpaca);
            context.SaveChanges();

            // 3. Media Items for Alpaca
            var mediaItems = new List<AnimalMedia>
            {
                new AnimalMedia
                {
                    AnimalId = alpaca.Id,
                    MediaType = "photo",
                    MediaUrl = "/images/paco.jpg",
                    ThumbnailUrl = "/images/paco.jpg",
                    Title = "Пако (самец, шоколадный окрас)",
                    Description = "Самец Пако с характерной густой шерстью цвета темного шоколада на прогулке.",
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new AnimalMedia
                {
                    AnimalId = alpaca.Id,
                    MediaType = "photo",
                    MediaUrl = "/images/lola.jpg",
                    ThumbnailUrl = "/images/lola.jpg",
                    Title = "Лола (белоснежная самочка)",
                    Description = "Белоснежная самочка Лола породы Уакая после сезонной стрижки и осмотра.",
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new AnimalMedia
                {
                    AnimalId = alpaca.Id,
                    MediaType = "photo",
                    MediaUrl = "/images/pair.jpg",
                    ThumbnailUrl = "/images/pair.jpg",
                    Title = "Пако и Лола в вольере",
                    Description = "Пара альпак отдыхает на травяном газоне в вольере №14.",
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new AnimalMedia
                {
                    AnimalId = alpaca.Id,
                    MediaType = "photo",
                    MediaUrl = "/images/zoo.jpg",
                    ThumbnailUrl = "/images/zoo.jpg",
                    Title = "Дневное кормление свежим сеном и овощами",
                    Description = "Альпаки с аппетитом поедают хрустящую морковь и люцерну.",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new AnimalMedia
                {
                    AnimalId = alpaca.Id,
                    MediaType = "video",
                    MediaUrl = "/images/alpaca-video.mp4",
                    ThumbnailUrl = "/images/paco.jpg",
                    Title = "Видео: Альпаки на дневной прогулке и кормлении",
                    Description = "Видеозапись естественного поведения Пако и Лолы в открытом секторе вольера.",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };

            context.AnimalMedias.AddRange(mediaItems);

            // 4. Diary Entries for Alpaca (Employee logs: Feeding, Mating, Offspring, Veterinary, Care, Behavior)
            var diaryEntries = new List<AnimalDiaryEntry>
            {
                new AnimalDiaryEntry
                {
                    AnimalId = alpaca.Id,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Category = "Кормёжка",
                    Title = "Утреннее и вечернее кормление + витаминный комплекс",
                    Description = "Утренний рацион: сено люцерны 3 кг, гранулированный корм для верблюдовых 800 г, морковь тертая 600 г с добавлением витамина E и селена. Лола съела всю порцию с отличным аппетитом. Пако дополнительно получил 2 яблока во время тренинга.",
                    HealthStatus = "Отличное",
                    DietDetails = "Люцерна 3кг, гранулы 800г, морковь 600г, яблоки 200г, минеральный лизунец.",
                    WeightKg = 64.5,
                    TemperatureC = 38.4,
                    AuthorName = "Александр Смирнов (Зоотехник)",
                    AuthorUserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new AnimalDiaryEntry
                {
                    AnimalId = alpaca.Id,
                    Date = DateTime.UtcNow.AddDays(-4),
                    Category = "Потомство",
                    Title = "Контрольное УЗИ Лолы — развитие плода в норме",
                    Description = "Проведено плановое ультразвуковое исследование Лолы. Срок гестации составляет ориентировочно 210 дней (из 345). Сердцебиение плода ритмичное (148 уд/мин), двигательная активность в норме. Лоле назначен усиленный кальциево-фосфорный рацион.",
                    HealthStatus = "Отличное",
                    DietDetails = "Добавлен премикс с хелатным кальцием и фолиевой кислотой.",
                    WeightKg = 59.2,
                    TemperatureC = 38.2,
                    AuthorName = "Елена Васильева (Главветврач)",
                    AuthorUserId = users[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new AnimalDiaryEntry
                {
                    AnimalId = alpaca.Id,
                    Date = DateTime.UtcNow.AddDays(-12),
                    Category = "Уход и стрижка",
                    Title = "Гигиеническая расчистка копытец и проверка шерсти",
                    Description = "Произведена плановая расчистка копытец Пако и Лолы. Состояние подошвенных мозолей идеальное, признаков пододерматита нет. Проведен осмотр шерстяного покрова: эктопаразитов не обнаружено, шерсть чистая, блестящая.",
                    HealthStatus = "Отличное",
                    DietDetails = "Стандартный рацион",
                    WeightKg = 65.0,
                    TemperatureC = 38.3,
                    AuthorName = "Александр Смирнов (Зоотехник)",
                    AuthorUserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-12)
                },
                new AnimalDiaryEntry
                {
                    AnimalId = alpaca.Id,
                    Date = DateTime.UtcNow.AddDays(-28),
                    Category = "Ветеринария",
                    Title = "Сезонная вакцинация и витаминизация",
                    Description = "Проведена плановая вакцинация против клостридиоза (вакцина 8-валентная). Внутримышечно введен комплекс витаминов A, D3, E. Температурная реакция отсутствует, животные активны, аппетит сохранен в полном объеме.",
                    HealthStatus = "Отличное",
                    DietDetails = "Стандартный рацион + травяная мука",
                    WeightKg = 64.0,
                    TemperatureC = 38.5,
                    AuthorName = "Елена Васильева (Главветврач)",
                    AuthorUserId = users[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-28)
                },
                new AnimalDiaryEntry
                {
                    AnimalId = alpaca.Id,
                    Date = DateTime.UtcNow.AddDays(-45),
                    Category = "Поведение",
                    Title = "Установка новых развивающих кормушек-головоломок",
                    Description = "В вольере установили подвесные деревянные кормушки с лабиринтом для веток. Пако проявил живой интерес, в течение 40 минут с увлечением доставал ветви ивы. Лола наблюдала, затем присоединилась. Положительное обогащение среды.",
                    HealthStatus = "Отличное",
                    DietDetails = "Свежие побеги ивы и березы",
                    WeightKg = null,
                    TemperatureC = null,
                    AuthorName = "Александр Смирнов (Зоотехник)",
                    AuthorUserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                },
                new AnimalDiaryEntry
                {
                    AnimalId = alpaca.Id,
                    Date = DateTime.UtcNow.AddDays(-90),
                    Category = "Спаривание",
                    Title = "Период гона и успешное спаривание пары",
                    Description = "Зафиксировано естественное брачное поведение Пако по отношению к Лоле (характерная вокализация 'оргл'). Спаривание прошло успешно, без признаков агрессии. Животные чувствуют себя комфортно, за Лолой установлено наблюдение.",
                    HealthStatus = "Отличное",
                    DietDetails = "Увеличен протеиновый рацион",
                    WeightKg = 63.8,
                    TemperatureC = 38.3,
                    AuthorName = "Александр Смирнов (Зоотехник)",
                    AuthorUserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                }
            };

            context.DiaryEntries.AddRange(diaryEntries);

            // 5. Initial Donations
            var donations = new List<Donation>
            {
                new Donation
                {
                    AnimalId = alpaca.Id,
                    DonorName = "Семья Вороновых",
                    Amount = 1500,
                    Target = "Корм и лакомства",
                    Message = "Пако и Лола самые милые! На самую сладкую хрустящую морковку ❤️",
                    CreatedAt = DateTime.UtcNow.AddDays(-6)
                },
                new Donation
                {
                    AnimalId = alpaca.Id,
                    DonorName = "Максим И.",
                    Amount = 5000,
                    Target = "Медицинский уход и витамины",
                    Message = "Здоровья Лоле и будущему малышу!",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new Donation
                {
                    AnimalId = alpaca.Id,
                    DonorName = "Клуб любителей животных 'Юг'",
                    Amount = 10000,
                    Target = "Обустройство вольера",
                    Message = "На новые удобные кормушки и теневой навес для альпак.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Donation
                {
                    AnimalId = alpaca.Id,
                    DonorName = "Мария С.",
                    Amount = 500,
                    Target = "Свободный донат",
                    Message = "Спасибо работникам за заботу о пушистиках!",
                    CreatedAt = DateTime.UtcNow.AddHours(-14)
                }
            };

            context.Donations.AddRange(donations);
            context.SaveChanges();
        }
    }
}
