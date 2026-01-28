
# QuickAid

Aplikacja mobilna wspierająca edukację pierwszej pomocy oraz lokalizację defibrylatorów AED.  
Projekt składa się z backendu (ASP.NET Core) oraz aplikacji mobilnej (Android, Kotlin + Jetpack Compose).

---

## Wymagania

Projekt był rozwijany i testowany w następującym środowisku:

- System operacyjny: Windows 10
- Backend: .NET 8, Visual Studio 2022
- Frontend: Kotlin 1.9.22, Android Studio Otter 2025.2.1
- Baza danych: SQL Server 16
- Debugowanie aplikacji mobilnej: urządzenie zewnętrzne z Android 11
- Ngrok: wersja 3.35.0

Do uruchomienia aplikacji zaleca się korzystanie z powyższych narzędzi i wersji.

---

## Instalacja i przygotowanie

1. Sklonować repozytorium:
   ```bash
   git clone https://github.com/han-ha/Quickaid
   ```

2. Uruchomić skrypt `Quickaid.sql`, który tworzy strukturę bazy danych.

3. Uruchomić skrypt `user_grants.sql`, który nada użytkownikowi bazy danych uprawnienia do wykonywania operacji na tabelach.

4. Uruchomić skrypt `populate.sql`, który dodaje do bazy dane testowe, w tym konto administratora:

   * login: `admin`
   * hasło: `adminadmin`

5. Uzupełnić parametry konfiguracyjne w plikach:

   * `appsettings.json`,
   * `appsettings.Development.json`.

6. Przygotować tunel ngrok i uzupełnić adres bazowy HTTP w pliku `Constants.kt` po stronie aplikacji mobilnej.

---

## Uruchamianie

1. W katalogu backendu uruchomić serwer:

   ```bash
   dotnet run
   ```

2. Uruchomić ngrok na porcie backendu:

   ```bash
   ngrok http <PORT_BACKENDU>
   ```

3. Uruchomić aplikację mobilną w Android Studio na emulatorze lub urządzeniu zewnętrznym.

---

## Skrypty SQL

* `Quickaid.sql` - tworzy strukturę bazy danych
* `populate.sql` - wstawia dane testowe
* `user_grants.sql` - nadaje użytkownikowi bazy danych uprawnienia do wykonywania operacji na tabelach

