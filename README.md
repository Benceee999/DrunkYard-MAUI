# DrunkYard - Social Party Games (.NET MAUI)

A modern, cross-platform mobile application designed for young adults to discover, play, and share social drinking and party games. Built with .NET 10 MAUI, the app combines offline-first local persistence, interactive sensor-driven mini-games, and cloud-synchronized community voting.

## Features

- **Categorized Game Catalog:** Browse games categorized by intensity (Easy to Extreme), player counts, and required equipment. Filter by favorites and sort dynamically.
- **Interactive Built-in Mini-Games:**
  - **"Never Have I Ever..." (Swipe Cards):** Tinder-style interactive swipe cards powered by gesture recognizers and pan animations. Includes 25+ curated party statements.
  - **King's Cup (Királyok Pohara):** Digital 52-card deck with automated shuffling, custom house rules, and 4th King endgame trigger logic.
- **Device Hardware & Sensor Integration:**
  - **Accelerometer (Shake Detection):** Shake the phone to draw the next card in mini-games.
  - **Haptic Feedback:** Vibrations on card swipes and shake gestures.
  - **Camera & Gallery (MediaPicker):** Attach custom images when submitting games.
  - **Native Platform Services:** Custom dependency-injected Toast notifications on Android (`Android.Widget.Toast`) with graceful fallback.
- **Community & Cloud Features:**
  - Submit user-created party games to a community candidate queue.
  - Real-time voting mechanism synchronized via REST API (MockAPI) with device-level anti-fraud prevention (`Preferences`).
- **Offline-First Persistence:** Fully operational without an internet connection using local SQLite database caching and automatic data seeding.

## Tech Stack & Architecture

- **Framework:** .NET 10 MAUI (Multi-platform App UI)
- **Language:** C# 13
- **Design Pattern:** Clean MVVM (Model-View-ViewModel) with loose coupling
- **MVVM Framework:** `CommunityToolkit.Mvvm` (Source generators, ObservableProperty, RelayCommand)
- **Persistence:** SQLite (`sqlite-net-pcl`, `SQLitePCLRaw.bundle_green`)
- **Networking:** `HttpClient` with System.Text.Json (polymorphic and enum converters)
- **Dependency Injection:** Microsoft.Extensions.DependencyInjection

## Solution Structure

- `DrunkYard`: Core presentation layer containing Views (XAML), ViewModels, AppShell routing, and platform-specific implementations.
- `DrunkYard.DataAccess`: Independent class library hosting SQLite database services, REST clients, and shared domain models.

## Getting Started

### Prerequisites
- Visual Studio 2022 (v17.12+) or .NET 10 SDK with MAUI workload installed.
- Android SDK (API 34+ recommended) or Windows 10/11 with Developer Mode enabled.

### Run the App

1. Clone the repository:
   ```bash
   git clone [https://github.com/](https://github.com/)<your-username>/DrunkYard-MAUI.git
   cd DrunkYard-MAUI
   ```
2. Restore workloads and dependencies:
   ```bash
   dotnet restore
   ```
3. Build and deploy (e.g. for Windows or connected Android device):
   ```bash
   # Windows
   dotnet build -t:Run -f net10.0-windows10.0.19041.0

   # Android
   dotnet build -t:Run -f net10.0-android
   ```

---

# DrunkYard - Közösségi Partijáték Mobilalkalmazás

Modern, többplatformos mobilalkalmazás .NET 10 MAUI alapokon, amely egy helyre gyűjti, rendszerezi és játszhatóvá teszi a népszerű társasági és bulis ivós játékokat. Az alkalmazás offline adatbázist, hardveres szenzorokat és felhő alapú közösségi szavazást kombinál.

## Fő funkciók

- **Játékkatalógus és szűrés:** Játékok böngészése nehézség (Könnyűtől Veszélyesig), játékosszám és kellékigény szerint. Valós idejű kedvenc-szűrés és rendezés.
- **Beépített interaktív minijátékok:**
  - **„Én még sosem...” (Kártyahúzás):** Kártyás, húzogatós felület gesztusfelismeréssel (`PanGestureRecognizer`), 28+ beépített szórakoztató állítással.
  - **King's Cup (Királyok Pohara):** 52 lapos francia kártyapakli generálás, keverés, kártyaszabályok követése és a 4. király behúzásakor végállapot-kezelés.
- **Eszköz- és hardverintegráció:**
  - **Gyorsulásmérő (Shake Detection):** Telefon rázására új kártya húzása.
  - **Haptikus visszajelzés (HapticFeedback):** Rezgések swipe-nál és rázáskor.
  - **Kamera és Galéria (MediaPicker):** Kép csatolása új játék beküldésekor.
  - **Platformspecifikus szolgáltatás:** Natív Android Toast értesítések (`Android.Widget.Toast`) Dependency Injection segítségével.
- **Közösségi funkciók és felhőszinkron:**
  - Saját játékötletek beküldése jóváhagyási listára.
  - Közösségi szavazás REST API-n keresztül (MockAPI), helyi csalásvédelemmel (`Preferences`).
- **Offline működés:** Az alkalmazás internet nélkül is teljes értékűen működik az előre feltöltött helyi SQLite adatbázisnak köszönhetően.

## Technológiák és architektúra

- **Keretrendszer:** .NET 10 MAUI
- **Nyelv:** C# 13
- **Architektúra:** Rétegzett MVVM (Model-View-ViewModel)
- **MVVM Toolkit:** `CommunityToolkit.Mvvm` (Source generátorok, reaktív tulajdonságok)
- **Adatbázis:** SQLite (`sqlite-net-pcl`, `SQLitePCLRaw.bundle_green`)
- **Hálózatkezelés:** `HttpClient`, `System.Text.Json`
- **Tervezés:** Dependency Injection, Repository/Service réteg

## Futtatás

1. Repozitórium klónozása:
   ```bash
   git clone [https://github.com/](https://github.com/)<your-username>/DrunkYard-MAUI.git
   cd DrunkYard-MAUI
   ```
2. Függőségek visszaállítása:
   ```bash
   dotnet restore
   ```
3. Futtatás Visual Studio-ból (F5) vagy parancssorból:
   ```bash
   # Windows futtatás
   dotnet build -t:Run -f net10.0-windows10.0.19041.0
   ```