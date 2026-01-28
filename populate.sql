IF DB_ID('QuickAid') IS NULL
BEGIN
    PRINT 'Baza QuickAid nie istnieje. Najpierw uruchomić quickaid.sql!';
    RETURN;
END

USE [QuickAid];
GO

-- Wstawienie użytkownika admin
INSERT INTO [users] ([username], [email], [role])
VALUES (N'admin', N'admin@example.com', N'admin');

DECLARE @AdminId INT = SCOPE_IDENTITY();

-- Wstawienie hasła dla admina
INSERT INTO [passwords] ([user_id], [hashed_password], [salt])
VALUES (@AdminId, N'hmGYc4KiTBHKyCoXFRPzPmHY3wmxhnmjzYlGylbFALI=', N'Bs0TKfVUG2HnAq2OBbRxZQ==');

-- AED Points
INSERT INTO [aed_points] ([latitude], [longitude], [description], [added_by], [verified])
VALUES
(52.229676, 21.012229, N'Przy wejściu do apteki', @AdminId, 1),
(50.061947, 19.936856, N'Przy centrum handlowym', @AdminId, 0),
(51.107885, 17.038538, N'Na dworcu kolejowym', @AdminId, 1);

-- Quizy
INSERT INTO [quizzes] ([title], [description], [number_of_questions], [max_score])
VALUES
(N'Pierwsza pomoc podstawy', N'Quiz o podstawowych zasadach pierwszej pomocy', 3, 3),
(N'AED i defibrylatory', N'Quiz o AED', 2, 2);

DECLARE @Quiz1Id INT = SCOPE_IDENTITY() - 1;
DECLARE @Quiz2Id INT = @Quiz1Id + 1;

-- Pytania
INSERT INTO [questions] ([question_text], [number_of_answers])
VALUES
(N'Jaki jest numer alarmowy w Polsce?', 4),
(N'Co należy zrobić w przypadku zatrzymania krążenia?', 4),
(N'Gdzie najlepiej umieścić AED?', 3),
(N'Jak długo należy uciskać klatkę piersiową?', 4),
(N'Czy AED można używać u dzieci?', 2);

DECLARE @Q1Id INT = SCOPE_IDENTITY() - 4;
DECLARE @Q2Id INT = @Q1Id + 1;
DECLARE @Q3Id INT = @Q1Id + 2;
DECLARE @Q4Id INT = @Q1Id + 3;
DECLARE @Q5Id INT = @Q1Id + 4;

-- Powiązanie quizów z pytaniami
INSERT INTO [quiz_questions] ([quiz_id], [question_id])
VALUES
(@Quiz1Id, @Q1Id),
(@Quiz1Id, @Q2Id),
(@Quiz1Id, @Q4Id),
(@Quiz2Id, @Q3Id),
(@Quiz2Id, @Q5Id);

-- Odpowiedzi
INSERT INTO [answers] ([question_id], [answer_text], [is_correct])
VALUES
(@Q1Id, N'112', 1),
(@Q1Id, N'911', 0),
(@Q1Id, N'184', 0),
(@Q1Id, N'0800', 0),
(@Q2Id, N'Rozpocząć resuscytację krążeniowo-oddechową', 1),
(@Q2Id, N'Podawać wodę', 0),
(@Q2Id, N'Wezwać pomoc', 1),
(@Q2Id, N'Nic nie robić', 0),
(@Q3Id, N'Przy wejściu do budynku', 1),
(@Q3Id, N'W piwnicy', 0),
(@Q3Id, N'Na dachu', 0),
(@Q4Id, N'30 uciśnięć', 1),
(@Q4Id, N'15 uciśnięć', 0),
(@Q4Id, N'Do przyjazdu karetki', 1),
(@Q4Id, N'5 uciśnięć', 0),
(@Q5Id, N'Tak', 1),
(@Q5Id, N'Nie', 0);

-- Artykuły
INSERT INTO [articles] ([title], [content], [created_by])
VALUES
(N'Podstawy pierwszej pomocy – co musisz wiedzieć',
N'Pierwsza pomoc to czynności wykonywane natychmiast przez osoby będące świadkami zdarzenia, które mają na celu podtrzymanie życia lub zapobieżenie pogorszeniu stanu zdrowia osoby poszkodowanej, zanim przyjedzie Zespół Ratownictwa Medycznego.

Przede wszystkim – oceń bezpieczeństwo: upewnij się, że ani Ty, ani poszkodowany nie jesteście narażeni na dodatkowe zagrożenie. Jeśli otoczenie jest niebezpieczne – nie podejmuj interwencji, wezwanie pomocy jest priorytetem.

Sprawdź, czy poszkodowany jest przytomny – głośno go zawołaj, potrząśnij za ramiona, zapytaj, czy Cię słyszy. Jeśli reaguje, zapytaj co się stało i postaraj się ustalić, czy wymagane jest wezwanie pomocy.

Jeśli poszkodowany nie reaguje lub jest nieprzytomny – sprawdź, czy oddycha. Jeśli drogi oddechowe są drożne i oddech nie występuje – przygotuj się do resuscytacji krążeniowo-oddechowej (RKO). Jeśli jest dostępny defibrylator AED, poproś kogoś o jego przyniesienie i uruchom zgodnie z instrukcją.

Pamiętaj – każda minuta się liczy: szybka reakcja i prawidłowe działanie często decyduje o przeżyciu poszkodowanego.', @AdminId),
(N'Resuscytacja i AED – krok po kroku',
N'Jeśli osoba straciła przytomność i nie oddycha, postępuj według poniższych kroków:

1. Ułóż poszkodowanego na plecach, na twardym i płaskim podłożu.
2. Uklęknij obok klatki piersiowej. Jedną rękę połóż na czole, drugą na podbródku – odchyl głowę do tyłu, aby udrożnić drogi oddechowe.
3. Sprawdź oddech: obserwuj ruch klatki piersiowej, przyłóż policzek do ust i nosa poszkodowanego, nasłuchaj oddechu. Czekaj ok. 10 sekund.
4. Jeśli nie ma oddechu – rozpocznij RKO: 30 uciśnięć klatki piersiowej (około 5-6 cm głębokości, tempo ~100-120 uciśnięć/min), następnie 2 oddechy ratownicze. Powtarzaj cykl 30:2 aż do przyjazdu pomocy lub pojawienia się oddechu.
5. Jeśli dostępny jest AED – poproś towarzyszącą osobę o jego przyniesienie; po podłączeniu podążaj za komunikatami urządzenia, a jednocześnie kontynuuj uciski klatki piersiowej.

Wielokrotne i szybkie rozpoczęcie RKO oraz użycie AED zwiększa szanse na przeżycie poszkodowanego.', @AdminId),
(N'Kiedy i jak wezwać pomoc – bezpieczeństwo i numer alarmowy',
N'Jeśli jesteś świadkiem wypadku, upadku, utraty przytomności, silnego krwotoku lub innego nagłego zagrożenia zdrowia – niezwłocznie zadzwoń pod numer alarmowy 112 (lub 999/998 jeśli znasz właściwy numer służb).

Obowiązkiem każdego z nas jest udzielenie pierwszej pomocy — zwlekanie może kosztować życie. Przed podjęciem działań upewnij się, że miejsce zdarzenia jest bezpieczne, oceń liczbę poszkodowanych, określ, kto potrzebuje pilnej pomocy. Możesz poprosić świadków o pomoc — np. by wezwali pogotowie, przynieśli AED, opatrunki lub telefon.

Jeśli poszkodowany oddycha, ale jest nieprzytomny — ułóż go w pozycji bocznej ustalonej i stale monitoruj oddech i przytomność aż do przyjazdu pomocy. Jeśli widzisz krwawienie, zranienia, oparzenia, złamania — załóż jałowy opatrunek, unieruchom kończynę lub zastosuj inne dostępne środki, jeśli masz odpowiednią wiedzę.

Nie musisz być ratownikiem — ważna jest szybka, rozsądna i przemyślana reakcja. Twoje działania mogą ocalić komuś życie.', @AdminId);

GO
