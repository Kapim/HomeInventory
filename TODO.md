# TODO — HomeInventory

Plánované funkce seřazené podle priority.

---

## [P1] MAUI mobilní aplikace ← top priorita

MAUI projekt je prázdný skeleton. Cíl: feature parity s WPF desktopem.

### Infrastruktura
- [ ] Registrace služeb v `MauiProgram.cs` (Client services, ViewModels)
- [ ] MAUI implementace `INavigationService` (Shell navigation)
- [ ] MAUI implementace `IDialogService` (potvrzovací dialogy)
- [ ] MAUI implementace `IBusyService` (blokování UI při async operacích)
- [ ] MAUI implementace `INotificationsService` (snackbar / toast)

### Přihlášení
- [ ] `LoginPage` + `LoginViewModel` (přihlašovací formulář)
- [ ] Uložení JWT tokenu (`SecureStorage`)
- [ ] Automatické přihlášení při startu (pokud token platný)
- [ ] Odhlášení

### Domácnosti a navigace
- [ ] `HouseholdsPage` — výběr domácnosti po přihlášení
- [ ] `MainPage` — hlavní obrazovka s navigací mezi sekcemi
- [ ] Shell routing: Login → Households → Main

### Lokace
- [ ] `LocationsPage` — stromový seznam lokací domácnosti
- [ ] Přidat / přejmenovat / smazat lokaci
- [ ] `LocationDetailPage` — editace názvu, typu, popisu
- [ ] Přesun lokace (reparenting) — mobilní UX (výběr cílové lokace ze seznamu)

### Položky
- [ ] `ItemsPage` — seznam položek ve vybrané lokaci
- [ ] Přidat / upravit / smazat položku
- [ ] Přesun položky do jiné lokace
- [ ] `ItemSearchPage` — globální vyhledávání (ekvivalent WPF `ItemsSearchView`)

### Validace a chybové stavy
- [ ] Zobrazení API chyb (lokalizace přes `IErrorLocalizer`)
- [ ] Offline stav — informovat uživatele

---

## [P2] Tagy / kategorie položek

- [ ] Nová entita `Tag` v doméně (název, barva)
- [ ] Přiřazení tagů k `Item` (M:N)
- [ ] API endpointy: CRUD tagů, přiřazení k položce
- [ ] Filtrování položek podle tagu (API + UI)
- [ ] WPF: správa tagů v `ItemsListView` (přiřazení, odebrání)
- [ ] MAUI: správa tagů v `ItemsPage`

---

## [P3] Export / import dat ✅

- [x] API endpoint `GET /households/{id}/export` → CSV
- [x] Export obsahuje: lokace (hierarchie), položky (název, množství, lokace, popis)
- [x] API endpoint `POST /households/{id}/import` — nahrání CSV
- [x] Import: validace a report chyb
- [x] WPF: tlačítka Export / Import v LocationTreeView (ikony vedle toolbar)
- [x] MAUI: sdílení exportovaného souboru přes `Share` API (menu → Exportovat CSV)

---

## Nápady do budoucna (netriážované)

- Fotky u položek (přílohy)
- Datum expirace / záruky u položek
- Upozornění na nízký stav zásob
- Statistiky domácnosti (počty, hodnoty)
- Zpřísnění oprávnění role Child (co smí/nesmí editovat)
