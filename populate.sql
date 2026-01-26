USE QuickAid;
GO

INSERT INTO [aed_points] ([latitude], [longitude], [description], [added_by], [verified])
VALUES
(52.229676, 21.012229, 'Przy wejœciu do apteki', 1, 1),
(50.061947, 19.936856, 'Przy centrum handlowym', 2, 0),
(51.107885, 17.038538, 'Na dworcu kolejowym', 1, 1);
GO

INSERT INTO [quizzes] ([title], [description], [number_of_questions], [max_score])
VALUES
('Pierwsza pomoc podstawy', 'Quiz o podstawowych zasadach pierwszej pomocy', 3, 3),
('AED i defibrylatory', 'Quiz o AED', 2, 2);
GO

INSERT INTO [questions] ([question_text], [number_of_answers])
VALUES
('Jaki jest numer alarmowy w Polsce?', 4),
('Co nale¿y zrobiæ w przypadku zatrzymania kr¹¿enia?', 4),
('Gdzie najlepiej umieœciæ AED?', 3),
('Jak d³ugo nale¿y uciskaæ klatkê piersiow¹?', 4),
('Czy AED mo¿na u¿ywaæ u dzieci?', 2);
GO

INSERT INTO [quiz_questions] ([quiz_id], [question_id])
VALUES
(1, 1),
(1, 2),
(1, 4),
(2, 3),
(2, 5);
GO

INSERT INTO [answers] ([question_id], [answer_text], [is_correct])
VALUES
(1, '112', 1),
(1, '911', 0),
(1, '184', 0),
(1, '0800', 0),
(2, 'Rozpocz¹æ resuscytacjê kr¹¿eniowo-oddechow¹', 1),
(2, 'Podawaæ wodê', 0),
(2, 'Wezwaæ pomoc', 1),
(2, 'Nic nie robiæ', 0),
(3, 'Przy wejœciu do budynku', 1),
(3, 'W piwnicy', 0),
(3, 'Na dachu', 0),
(4, '30 uciœniêæ', 1),
(4, '15 uciœniêæ', 0),
(4, 'Do przyjazdu karetki', 1),
(4, '5 uciœniêæ', 0),
(5, 'Tak', 1),
(5, 'Nie', 0);
GO

INSERT INTO [articles] ([title], [content], [created_by])
VALUES
('Podstawy pierwszej pomocy – co musisz wiedzieæ',
'Pierwsza pomoc to czynnoœci wykonywane natychmiast przez osoby bêd¹ce œwiadkami zdarzenia, które maj¹ na celu podtrzymanie ¿ycia lub zapobie¿enie pogorszeniu stanu zdrowia osoby poszkodowanej, zanim przyjedzie Zespó³ Ratownictwa Medycznego.

Przede wszystkim – oceñ bezpieczeñstwo: upewnij siê, ¿e ani Ty, ani poszkodowany nie jesteœcie nara¿eni na dodatkowe zagro¿enie. Jeœli otoczenie jest niebezpieczne – nie podejmuj interwencji, wezwanie pomocy jest priorytetem.

SprawdŸ, czy poszkodowany jest przytomny – g³oœno go zawo³aj, potrz¹œnij za ramiona, zapytaj, czy Ciê s³yszy. Jeœli reaguje, zapytaj co siê sta³o i postaraj siê ustaliæ, czy wymagane jest wezwanie pomocy.

Jeœli poszkodowany nie reaguje lub jest nieprzytomny – sprawdŸ, czy oddycha. Jeœli drogi oddechowe s¹ dro¿ne i oddech nie wystêpuje – przygotuj siê do resuscytacji kr¹¿eniowo-oddechowej (RKO). Jeœli jest dostêpny defibrylator AED, poproœ kogoœ o jego przyniesienie i uruchom zgodnie z instrukcj¹.

Pamiêtaj – ka¿da minuta siê liczy: szybka reakcja i prawid³owe dzia³anie czêsto decyduj¹ o prze¿yciu poszkodowanego.', 2),

('Resuscytacja i AED – krok po kroku',
'Jeœli osoba straci³a przytomnoœæ i nie oddycha, postêpuj wed³ug poni¿szych kroków:

1. U³ó¿ poszkodowanego na plecach, na twardym i p³askim pod³o¿u.
2. Uklêknij obok klatki piersiowej. Jedn¹ rêkê po³ó¿ na czole, drug¹ na podbródku – odchyl g³owê do ty³u, aby udro¿niæ drogi oddechowe.
3. SprawdŸ oddech: obserwuj ruch klatki piersiowej, przy³ó¿ policzek do ust i nosa poszkodowanego, nas³uchaj oddechu. Czekaj ok. 10 sekund.
4. Jeœli nie ma oddechu – rozpocznij RKO: 30 uciœniêæ klatki piersiowej (oko³o 5-6 cm g³êbokoœci, tempo ~100-120 uciœniêæ/min), nastêpnie 2 oddechy ratownicze. Powtarzaj cykl 30:2 a¿ do przyjazdu pomocy lub pojawienia siê oddechu.
5. Jeœli dostêpny jest AED – poproœ towarzysz¹c¹ osobê o jego przyniesienie; po pod³¹czeniu pod¹¿aj za komunikatami urz¹dzenia, a jednoczeœnie kontynuuj uciski klatki piersiowej.

Wielokrotne i szybkie rozpoczêcie RKO oraz u¿ycie AED zwiêksza szanse na prze¿ycie poszkodowanego.', 2),

('Kiedy i jak wezwaæ pomoc – bezpieczeñstwo i numer alarmowy',
'Jeœli jesteœ œwiadkiem wypadku, upadku, utraty przytomnoœci, silnego krwotoku lub innego nag³ego zagro¿enia zdrowia – niezw³ocznie zadzwoñ pod numer alarmowy 112 (lub 999/998 jeœli znasz w³aœciwy numer s³u¿b).

Obowi¹zkiem ka¿dego z nas jest udzielenie pierwszej pomocy — zwlekanie mo¿e kosztowaæ ¿ycie. Przed podjêciem dzia³añ upewnij siê, ¿e miejsce zdarzenia jest bezpieczne, oceñ liczbê poszkodowanych, okreœl, kto potrzebuje pilnej pomocy. Mo¿esz poprosiæ œwiadków o pomoc — np. by wezwali pogotowie, przynieœli AED, opatrunki lub telefon.

Jeœli poszkodowany oddycha, ale jest nieprzytomny — u³ó¿ go w pozycji bocznej ustalonej i stale monitoruj oddech i przytomnoœæ a¿ do przyjazdu pomocy. Jeœli widzisz krwawienie, zranienia, oparzenia, z³amania — za³ó¿ ja³owy opatrunek, unieruchom koñczynê lub zastosuj inne dostêpne œrodki, jeœli masz odpowiedni¹ wiedzê.

Nie musisz byæ ratownikiem — wa¿na jest szybka, rozs¹dna i przemyœlana reakcja. Twoje dzia³ania mog¹ ocaliæ komuœ ¿ycie.', 2);
GO

INSERT INTO users (username, email, role)
VALUES ('admin', 'admin@example.com', 'admin');

INSERT INTO passwords (user_id, hashed_password, salt)
VALUES (15, 'hmGYc4KiTBHKyCoXFRPzPmHY3wmxhnmjzYlGylbFALI=', 'Bs0TKfVUG2HnAq2OBbRxZQ==');
