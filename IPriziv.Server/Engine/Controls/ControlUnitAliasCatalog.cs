using System.Text.RegularExpressions;

namespace IPriziv.Server.Engine.Controls
{
    /// <summary>
    /// Справочник соответствий в/ч ↔ тексты контролей для комплектования.
    /// Строка: род войск, ключ части, станция, алиасы через «/».
    /// </summary>
    public static class ControlUnitAliasCatalog
    {
        public sealed record Entry(
            string Branch,
            string UnitKey,
            string Station,
            IReadOnlyList<string> Aliases);

        private static readonly Regex DigitsRegex = new(@"\d{4,6}", RegexOptions.Compiled);

        public static IReadOnlyList<Entry> Entries { get; } = BuildEntries();

        private static readonly IReadOnlyDictionary<string, int> AliasFrequency = BuildAliasFrequency(Entries);

        /// <summary>
        /// Все строки каталога (ключ, станция, алиасы, род войск), связанные с токенами команды.
        /// Привязка только по ключу/номеру/станции или по уникальному алиасу.
        /// </summary>
        public static IReadOnlyList<string> ResolveRelatedTexts(IEnumerable<string> seeds)
        {
            if (seeds == null)
                return Array.Empty<string>();

            var seedList = seeds
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (seedList.Count == 0)
                return Array.Empty<string>();

            var seedNumbers = new HashSet<string>(StringComparer.Ordinal);
            foreach (var seed in seedList)
            {
                foreach (Match m in DigitsRegex.Matches(NormalizeDigits(seed)))
                {
                    var trimmed = m.Value.TrimStart('0');
                    seedNumbers.Add(trimmed.Length == 0 ? m.Value : trimmed);
                    seedNumbers.Add(m.Value);
                }
            }

            var result = new List<string>();
            foreach (var entry in Entries)
            {
                if (!EntryMatchesSeeds(entry, seedList, seedNumbers))
                    continue;

                AddUnique(result, entry.UnitKey);
                AddUnique(result, entry.Station);
                AddUnique(result, entry.Branch);
                foreach (var alias in entry.Aliases)
                    AddUnique(result, alias);
            }

            return result;
        }

        /// <summary>
        /// Текст контроля похож на известный алиас / ключ части из каталога.
        /// </summary>
        public static bool LooksLikeCatalogDestination(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var hay = text.Trim();
            foreach (var entry in Entries)
            {
                if (KeyOrNumberHits(hay, entry.UnitKey))
                    return true;
                foreach (var alias in entry.Aliases)
                {
                    if (!IsDistinctiveAlias(alias))
                        continue;
                    if (hay.Equals(alias, StringComparison.OrdinalIgnoreCase)
                        || (alias.Length >= 4 && hay.Contains(alias, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }
            }

            return false;
        }

        private static bool EntryMatchesSeeds(
            Entry entry,
            List<string> seeds,
            HashSet<string> seedNumbers)
        {
            var hyphenated = entry.UnitKey.Contains('-', StringComparison.Ordinal);

            foreach (var seed in seeds)
            {
                if (hyphenated)
                {
                    // 25801-11 / 25801-13 — только полный ключ, без «голого» 25801.
                    if (seed.Equals(entry.UnitKey, StringComparison.OrdinalIgnoreCase)
                        || seed.Contains(entry.UnitKey, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (KeyOrNumberHits(seed, entry.UnitKey)
                         || KeyOrNumberHits(entry.UnitKey, seed))
                {
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(entry.Station)
                    && seed.Equals(entry.Station, StringComparison.OrdinalIgnoreCase))
                    return true;

                // Уникальный алиас (Таманская, Кантемировская…) — можно привязать часть.
                foreach (var alias in entry.Aliases)
                {
                    if (!IsDistinctiveAlias(alias))
                        continue;
                    if (!AliasFrequency.TryGetValue(NormalizeAliasKey(alias), out var freq) || freq != 1)
                        continue;
                    if (seed.Equals(alias, StringComparison.OrdinalIgnoreCase)
                        || (alias.Length >= 6
                            && seed.Contains(alias, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }
            }

            if (!hyphenated)
            {
                foreach (Match m in DigitsRegex.Matches(NormalizeDigits(entry.UnitKey)))
                {
                    var key = m.Value.TrimStart('0');
                    if (key.Length == 0)
                        key = m.Value;
                    if (seedNumbers.Contains(key) || seedNumbers.Contains(m.Value))
                        return true;
                }
            }

            return false;
        }

        private static bool KeyOrNumberHits(string haystack, string needle)
        {
            if (string.IsNullOrWhiteSpace(haystack) || string.IsNullOrWhiteSpace(needle))
                return false;

            var h = haystack.Trim();
            var n = needle.Trim();
            if (h.Equals(n, StringComparison.OrdinalIgnoreCase))
                return true;

            if (n.Any(char.IsDigit)
                && (h.Contains(n, StringComparison.OrdinalIgnoreCase)
                    || n.Contains(h, StringComparison.OrdinalIgnoreCase)))
                return true;

            if (n.Length >= 6
                && h.Contains(n, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private static bool IsDistinctiveAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                return false;
            var a = alias.Trim();
            // «27», «Связь», «1 ТА» — слишком общие для однозначной привязки.
            if (a.Length < 4)
                return false;
            if (a.Equals("Связь", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Казаки", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Госпиталь", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Калининград", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Морской флот", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Мор. флот", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Мор. Флот", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Морская пехота", StringComparison.OrdinalIgnoreCase)
                || a.Equals("1 ТА", StringComparison.OrdinalIgnoreCase)
                || a.Equals("1 гв. ТА", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Танковая армия", StringComparison.OrdinalIgnoreCase)
                || a.Equals("1 Танковая армия", StringComparison.OrdinalIgnoreCase)
                || a.Equals("ВДВ МО", StringComparison.OrdinalIgnoreCase)
                || a.Equals("СпН", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Инженерные войска", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Пограничные органы ФСБ России", StringComparison.OrdinalIgnoreCase)
                || a.Equals("Главное управление Генерального штаба ВС РФ", StringComparison.OrdinalIgnoreCase))
                return false;
            return true;
        }

        private static string NormalizeAliasKey(string alias)
            => (alias ?? "").Trim().ToUpperInvariant();

        private static IReadOnlyDictionary<string, int> BuildAliasFrequency(IReadOnlyList<Entry> entries)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                foreach (var alias in entry.Aliases)
                {
                    var key = NormalizeAliasKey(alias);
                    if (key.Length == 0)
                        continue;
                    map[key] = map.GetValueOrDefault(key) + 1;
                }
            }
            return map;
        }

        private static string NormalizeDigits(string text)
            => Regex.Replace(text ?? "", @"(?<=\d)\s+(?=\d)", "");

        private static void AddUnique(List<string> list, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            var trimmed = value.Trim();
            if (list.Exists(x => string.Equals(x, trimmed, StringComparison.OrdinalIgnoreCase)))
                return;
            list.Add(trimmed);
        }

        private static IReadOnlyList<Entry> BuildEntries()
        {
            // Branch,UnitKey,Station: alias/alias/...
            var raw = """
СВ,23626,Алабино: 1 ТА/1 гв. ТА/Алабино/Таманская/Таманская див./ Таманская дивизия/ Танковая армия/ 1 Танковая армия/Тамань
СВ,19612,Нара: Кантемировская дивизия/Кантемировская див./Кантемировка/Юноши-казаки/Казаки/1 гв. ТА/1 ТА/1 Танковая армия/Танковая армия/дивизя Наро-Фоминск/4 ТД
СВ,61899,Москва: 27 омсбр/27 бригада/27 ОМСБР/27 бриг./27-я бригада/27
СВ,76736,Селятино: 1 ТА/1 гв. ТА
СВ,75384,Москва: Семеновский/Семновский полк/1-й отдельный стрелковый Семеновский полк
СВ,01904,Москва: Преображенский полк/Преображенский
СВ,48428,Москва: Салютный дивизион/449 отдельный салютный дивизион/449 отдельный дивизион/
СВ,63791,Москва: Сопровождение грузов
СВ,41516,Барыбино: 65 МРУЦ/Связь ГШ/65-й Межведомственный Военный учебный центр/65 МРУТС/Связь/65 БРУЦ
СВ,190 ВШП,Нара: Высшая школа поваров
СВ,Военная комендатура,Москва: Рота патрульной службы/Военная полиция/Комендатура Москвы/Комендатура
СВ,ВУНЦ СВ "ОА ВС РФ",Москва: Общевойсковая академия
СВ,3 ВОО почетного караула,Москва: 3 ВООПК
СВ,ВА РХБЗ,Кострома: Научная рота Военной академии радиационной, химической и биологический защиты/УНВ РХБЗ ВС/Научная рота радиационной, химической и биологический защиты
СВ,Военно-медицинская академия,Санкт-Петербург: Научная рота Главного военно-медицинского управления Министерства обороны Российской Федерации (ГВМУ МО)
СВ,Московское ВОКУ,Москва: МВОКУ/Научная рота Сухопутных войск (ГК СВ)/МосВОКУ/НР МосВОКУ
СВ,29760,Луга: ЛевВО/ЛенВО/6 Армия
СВ,Военная академия связи,Санкт-Петербург: Научная рота Главного управления связи Вооруженных Сил Российской Федерации (ГУС ВС)/ГУС ВС
СВ,Военная академия МТО,Санкт-Петербург: Научная рота Штаба материально-технического обеспечения Вооруженных Сил Российской Федерации (Штаб МТО)/МТО
СВ,ФГАУ ВИТ "ЭРА",Анапа: ВИТ "ЭРА"
ЦП,83497,Ярославль: Судимые
ЦП,45445,Муром 1: Инженерные войска
ЦП,55591,Нахабино: 45 Инженерно-маскировочный полк/45 имп/Николо-Урюшинск/45 имасп/45 ибр/Инженерный - 1/
ЦП,19889,Ногинск: УЦ РХБЗ
ЦП,61460,Тамбов: Научная рота войск радиоэлектронной борьбы (УНВ РЭБ ВС)/Рота войск радиоэлектронной борьбы/УНВ РЭБ ВС
ЦП,85084,Селятино: 100-й полк/Полк обеспечения
ЦП,АПиП Александрова,Москва: Хор им. Александрова/Академический ансамбль песни и пляски имени А.В. Александрова/Академический ансамбль песни и пляски им. А.В. Александрова
ЦП,73535,Звенигород: Топогеодезичисты/Топографы/47 ТГО/Топоотряд
ЦП,32516,Орудьево: Школа собаководов/Орудьево кинолог/Кинолог
ЦП,11300,Загорянская: Загорянка/Жд войска/ЖДВ/ЖД бригада
ЦП,11105,Муром 1: Инженерные войска/1 ИСБР
ВМФ,ВУНЦ ВМФ,Санкт-Петербург: Научная рота Военно-Морского Флота (ГК ВМФ)
ВМФ,77165,Калининград: Балтийский флот/Калининград/Калининград ВМФ/Балт флот/Морской флот/Морская пехота/Мор. флот
ВМФ,06982,Североморск: Северный флот/СФ/Морской флот/Мор. Флот
ВМФ,06984,Севастополь: Черноморский флот/Морская пехота/Морской флот/Мор. Флот
ВМФ,13076,Новороссийск: Черноморский флот/Морской флот/Мор. Флот
ВМФ,56529-2,Санкт-Петербург: Балтийский флот/Морской флот/Мор. Флот
ВМФ,45603-2,Домодедово: Пункт связи ВМФ
ЦОВУ,ФАУ МО РФ "ЦСКА",Ватутинки: Спортивная рота
РВСН,ВА П.Великого,Балашиха: ВА РВСН/ВА РВСН им. Петра Великого/Военная академия РВСН
РВСН,75376,Капустин Яр: Кап Яр/КапЯр
ВКС,92925,Голицыно: Краснознаменск/Научная рота специального назначения - основной состав/ПВО ПРО/1 армия/ 1А ПВО ПРО/Научная рота специального назначения
ВКС,61996,Машиностроитель: Электросталь
ВКС,28289,Дуброво: Ногинск-9
ВКС,92574,Барыбино: 1 армия ПРН ПРО/1 армия ПВО/1 армия ПВО ПРО/1А ВВС ПВО
ВКС,96103,Москва: ВКС Красногорск
ВКС,52462,Петушки: КДА
ВКС,42829,Чкаловская: Чкаловск/ВКС Чкаловск/ 8 адон
ВКС,15681,Железнодорожная: ВКС Балашиха
ВКС,01907,Калининград: Калининград
ВДВ,41450,Рязань: 137 пдп
ВДВ,93723,Наро-Фоминск: ВДВ МО
ВДВ,28337,Кубинка: ВДВ МО
ВДВ,85515,Тесницкое: Теснитское
ВДВ,54164,Щелково: ВДВ МО/38 бру
ГУ ГШ,03770,Воскресенск: Главное управление Генерального штаба ВС РФ
ГУ ГШ,47126,Воскресенск: Главное управление Генерального штаба ВС РФ
ГУ ГШ,30258,Воскресенск: Главное управление Генерального штаба ВС РФ
ГУ ГШ,92154,Подсолнечная: СпН
ГУ ГШ,25503,Воскресенск: Главное управление Генерального штаба ВС РФ
ГУ ГШ,43292,Подсолнечная: СпН
ГУ ГШ,43099,Гривно: Главное управление Генерального штаба ВС РФ
ГУ ГШ,34608,Гривно: Главное управление Генерального штаба ВС РФ
ГУ ГШ,36360,Загорянская: Главное управление Генерального штаба ВС РФ
24 ОБ,30663,Чехов-4: 24 Отдельная бригада связи/24 бригада
14 ГЦС,25801-11,Луховицы: Главный центр связи ВС РФ
14 ГЦС,25801-13,Серпухов: Главный центр связи ВС РФ
147 АБ,83466,Москва: Автобаза/АБМО/147/147 АБ/147 автобаза/от Березюка/147АБ
351 СОЦ,53982,Москва: Специальный оперативно-технический центр
НГ,5401,Москва: 55 дивизия/55-я дивизия/55 див/ 55 дивизя НГ
НГ,3272,Москва: 95 дивизия
НГ,3111,Никольское: Одон/ОДОН ВНГ
НГ,ЦВК по ОД Росгвардии,Москва: 5380/Батальон обеспечения/ ФГКУ ЦВК
НГ,3472,Никольское: ЦУС войск национальной гвардии/ЦУС НГ/Балашиха
НГ,ГВКГ войск НГ,Никольское: Госпиталь/3178/ФГКУ ГВКГ
ФСБ,2013,Нальчик: Пограничные органы ФСБ России
ФСБ,2396,Геленджик: Пограничные органы ФСБ России
ФСБ,2067,Южно-Сахалинск: Пограничные органы ФСБ России
МЧС,84411,Ногинск: Ногинск-5
МЧС,35489,Москва: Лидер
МЧС,73407,Тучково: Руза-Тучково
""";

            var list = new List<Entry>();
            foreach (var line in raw.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var colon = line.IndexOf(':');
                if (colon < 0)
                    continue;
                var head = line[..colon].Trim();
                var aliasesRaw = line[(colon + 1)..];
                var parts = head.Split(',', 3, StringSplitOptions.TrimEntries);
                if (parts.Length < 3)
                    continue;

                var aliases = aliasesRaw
                    .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(a => a.Length > 0)
                    .ToArray();

                list.Add(new Entry(parts[0], parts[1], parts[2], aliases));
            }

            return list;
        }
    }
}
